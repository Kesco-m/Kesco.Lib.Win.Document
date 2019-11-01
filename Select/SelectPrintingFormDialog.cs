using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Blocks.Parsers;
using Kesco.Lib.Win.Document.Items;

namespace Kesco.Lib.Win.Document.Select
{
	public partial class SelectPrintingFormDialog : FreeDialog
	{
		private ArrayList printTypes;
		private int typeID;

		private DocTypeParser docTypeParser;
		private DataSet dsTypes;
		private DataSet dsPrintForms;

		public SelectPrintingFormDialog()
		{
			InitializeComponent();
			printTypes = new ArrayList();
		}

		public string[] PrintTypes
		{
			get { return printTypes.Count > 0 ? (string[])printTypes.ToArray(typeof(string)) : null; }
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if(printTypes.Count > 0)
				End(DialogResult.OK);
		}

		private void ButtonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void SelectPrintingFormDialog_Load(object sender, EventArgs e)
		{
			buttonOK.Enabled = false;
			types.Columns.Add("DocumentType", types.Width - SystemInformation.VerticalScrollBarWidth,
							  HorizontalAlignment.Left);
			types.FullRowSelect = true;
			lvPrintForms.Columns.Add("DocumentType", lvPrintForms.Width - SystemInformation.VerticalScrollBarWidth,
									 HorizontalAlignment.Left);
			lvPrintForms.FullRowSelect = true;
			dsTypes = Environment.DocTypeData.GetDocPrintTypes();
			dsPrintForms = Environment.PrintData.GetPrintForms();
			docTypeParser = new DocTypeParser(Environment.DocTypeData, dsTypes.Tables[0]);
			FillTypes();
			FillPrintForms();
		}

		private void FillTypes()
		{
			types.Items.Clear();

			string txt = filter.Text.Trim();

			if(txt == "" || !checkBoxType.Checked)
			{
				if(dsTypes != null && dsTypes.Tables.Count > 0)
				{
					DataTable dt = dsTypes.Tables[0];
					for(int i = 0; i < dt.Rows.Count; i++)
						AddRow(dt.Rows[i], types);
				}
			}
			else
			{
				DataRow[] drs = docTypeParser.ParseQuery(txt);
				if(drs != null && drs.Length > 0)
					foreach(DataRow t in drs)
						AddRow(t, types);
			}
		}

		private void FillPrintForms()
		{
			lvPrintForms.Items.Clear();
			string txt = filter.Text.Trim();

			if(txt == "" || !checkBoxPrintForm.Checked)
			{
				if(dsPrintForms != null && dsPrintForms.Tables.Count > 0)
				{
					DataTable dt = dsPrintForms.Tables[0];
					var drs = dt.Rows.Cast<DataRow>().Distinct(new DRComparer());
					foreach(DataRow dr in drs)
						AddRowLV(dr);
				}
			}
			else
			{
				DataRow[] drs = ParseQuery(txt);
				var ds = drs.Distinct(new DRComparer());
				foreach(DataRow dr in ds)
					AddRowLV(dr);
			}
		}

		private class DRComparer : IEqualityComparer<DataRow>
		{
			public bool Equals(DataRow x, DataRow y)
			{
				//Check whether the compared objects reference the same data.
				if(CultureObjPrint(x).Equals(CultureObjPrint(y))) return true;
				return false;
			}

			// If Equals() returns true for a pair of objects 
			// then GetHashCode() must return the same value for these objects.

			public int GetHashCode(DataRow x)
			{
				return CultureObjPrint(x).GetHashCode();
			}

			private object CultureObjPrint(DataRow dr)
			{
				return dr[Environment.PrintData.NameField];
			}
		}

		private void FillPrintFormsByType(int typeID)
		{
			this.typeID = typeID;
			lvPrintForms.Items.Clear();
			if(dsPrintForms != null && dsPrintForms.Tables.Count > 0)
			{
				var rows =
					dsPrintForms.Tables[0].Rows.Cast<DataRow>().Where(
						x => x[Environment.PrintData.DocTypeField].Equals(typeID)).ToList();
				foreach(DataRow dr in rows)
					AddRowLV(dr);
			}
			buttonOK.Enabled = lvPrintForms.SelectedItems.Count > 0;
		}

		private void AddRowLV(DataRow dr)
		{
			string obj = CultureObjPrint(dr) as string;
			if(string.IsNullOrEmpty(obj))
				return;
			var id = (int)dr[Environment.PrintData.IDField];
			if(id <= 0)
				return;

			lvPrintForms.Items.Add(new ListItem(id, obj) { ToolTipText = obj });
		}

		private void AddRow(DataRow dr, ListView lv)
		{
			string obj = CultureObjTypes(dr) as string;
			if(string.IsNullOrEmpty(obj))
				return;
			var id = (int)dr[Environment.DocTypeData.IDField];
			if(id <= 0)
				return;

			var item = new ListItem(id, obj) { ToolTipText = obj };
			lv.Items.Add(item);
		}

		private object CultureObjTypes(DataRow dr)
		{
			return Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? dr[Environment.DocTypeData.NameField] : dr[Environment.DocTypeData.TypeDocField];
		}

		private object CultureObjPrint(DataRow dr)
		{
			return dr[Environment.PrintData.NameField];
		}

		private void types_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(types.SelectedItems.Count == 0)
				return;

			var item = (ListItem)types.SelectedItems[0];
			int typeID = item.ID;

			FillPrintFormsByType(typeID);
		}

		private void filter_TextChanged(object sender, EventArgs e)
		{
			if(checkBoxType.Checked)
				FillTypes();
			FillPrintForms();
		}

		public DataRow[] ParseQuery(string txt)
		{
			if(Environment.PrintData == null)
				throw new Exception("DocTypeParser: " + Environment.StringResources.GetString("Select_SelectTypeDialog_SelectType_Load_Error1")
								+ " data.");

			if(dsPrintForms == null || dsPrintForms.Tables.Count < 1)
				throw new Exception("DocTypeParser: " + Environment.StringResources.GetString("Select_SelectTypeDialog_SelectType_Load_Error1") +
							" table.");

			txt = Regex.Replace(
				Replacer.ReplaceRusLat(txt),
				"[" + Regex.Escape(@"]\*+?|{[()^$.#%'") + "]{1,}",
				"",
				RegexOptions.IgnoreCase);

			txt = txt.Replace("-", " ");
			txt = txt.Replace("  ", " ");

			if(txt.Length == 0)
				return null;

			string[] w = Regex.Split(txt, @"\s");
			if(w.Length > 0)
			{
				string field = Environment.PrintData.NameField;
				List<DataRow> rows =
					dsPrintForms.Tables[0].Rows.Cast<DataRow>().Where<DataRow>(
						x =>
						Replacer.ReplaceRusLat(x[field].ToString()).StartsWith(w[0], true,
																					   CultureInfo.
																						   InvariantCulture)).ToList();
				for(int i = 1; i < w.Length; i++)
					rows = rows.Where(x => Replacer.ReplaceRusLat(x[field].ToString()).ToLower().Contains(w[i].ToLower())).ToList();
				try
				{
					return rows.ToArray();
				}
				catch
				{
				}
			}

			return null;
		}

		private void lvPrintForms_SelectedIndexChanged(object sender, EventArgs e)
		{
			printTypes.Clear();
			for(int i = 0; i < lvPrintForms.SelectedItems.Count; i++)
				printTypes.Add(lvPrintForms.SelectedItems[i].Text);
			buttonOK.Enabled = printTypes.Count > 0;
		}

		private void lv_SizeChanged(object sender, EventArgs e)
		{
			var lv = sender as ListView;
			if(lv != null)
				lv.Columns[0].Width = lv.Width - SystemInformation.VerticalScrollBarWidth - 4;
		}

		private void lv_Enter(object sender, EventArgs e)
		{
			var lv = sender as ListView;
			if(lv == null || lv.SelectedItems.Count <= 0)
				return;
			var item = (ListItem)lv.SelectedItems[0];
			int type = item.ID;
			if(typeID != type)
				FillPrintFormsByType(type);
		}
	}
}