using System;
using System.Data;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Blocks.Parsers;
using Kesco.Lib.Win.Document.Items;

namespace Kesco.Lib.Win.Document.Select
{
	public partial class SelectTypeDialog : FreeDialog
	{
		public enum Direction
		{
			No = 0,
			In,
			Out
		}

		private int runTypeID;
		private Direction inOut;

		private bool showCheckBoxes;

		private bool formNeeded;
		private string pattern;

		private DataTable dt;

		private DocTypeParser docTypeParser;

		public SelectTypeDialog(int typeID, bool formNeeded, string pattern, int runTypeID, Direction inOut,
								bool showCheckBoxes)
		{
			InitializeComponent();

			this.TypeID = typeID;
			this.formNeeded = formNeeded;
			this.pattern = pattern;
			this.runTypeID = runTypeID;
			panelPreviosSelect.Visible = (runTypeID > 0);
			this.inOut = inOut;
			this.showCheckBoxes = showCheckBoxes;
			if(formNeeded)
			{
				if(runTypeID > 0)
				{
					bool dirNeeded = (Environment.PersonID > 0) ||
									 Environment.DocTypeData.GetDocBoolField(Environment.DocTypeData.OutGoingField, runTypeID);
					labelReturn.Text = Environment.StringResources.GetString("CreateNew") + " " +
									   (dirNeeded ? ((inOut.Equals(Direction.Out))
												   ? Environment.StringResources.GetString("Outgoing") + " "
												   : Environment.StringResources.GetString("Incoming") + " ")
											: "") +
									   Environment.StringResources.GetString("Select_SelectTypeDialog_Message1") + " " +
									   Environment.DocTypeData.GetDocType(runTypeID, Environment.CurCultureInfo.TwoLetterISOLanguageName) + " (Ctrl+F8)";
					labelReturn.LinkArea = new LinkArea(0, labelReturn.Text.Length - 10);
					labelReturn.LinkVisited = true;
				}
				groupSynonyms.Visible = false;
			}

			panelSimilar.Visible = showCheckBoxes;
			panelSubtypes.Visible = showCheckBoxes;

			if(!DesignerDetector.IsComponentInDesignMode(this))
				dt = Environment.DocTypeData.GetDocTypes(formNeeded);
		}

		public SelectTypeDialog(int typeID, bool formNeeded, string pattern, bool showCheckBoxes)
			: this(typeID, formNeeded, pattern, 0, Direction.No, showCheckBoxes)
		{
		}

		public SelectTypeDialog(int typeID, bool formNeeded, string pattern) : this(typeID, formNeeded, pattern, false)
		{
		}

		public SelectTypeDialog(int typeID, bool formNeeded) : this(typeID, formNeeded, null)
		{
		}

		public SelectTypeDialog(int typeID, bool formNeeded, bool showCheckBoxes) 
			: this(typeID, formNeeded, null, showCheckBoxes)
		{
		}

		public SelectTypeDialog(int typeID) : this(typeID, false, null)
		{
		}

		public SelectTypeDialog(string pattern) : this(0, false, pattern)
		{
		}

		#region Accessors

		public int TypeID { get; private set; }

		public string Type { get; private set; }

		public Direction Out
		{
			get
			{
				if(formNeeded && panelInOut.Visible)
					return radioButtonOut.Checked ? Direction.Out : Direction.In;
				return Direction.No;
			}
		}

		public bool SimilarChecked
		{
			get { return cbSimilar.Checked; }
			set { cbSimilar.Checked = value; }
		}

		public bool SubTypesChecked
		{
			get { return cbSubtypes.Checked; }
			set { cbSubtypes.Checked = value; }
		}

		#endregion

		private void SelectType_Load(object sender, EventArgs e)
		{
			if(dt == null || dt.Rows.Count == 0)
			{
				throw new Exception("SelectTypeDialog: " + Environment.StringResources.GetString("Select_SelectTypeDialog_SelectType_Load_Error1") +
					" table.");
			}

			docTypeParser = new DocTypeParser(Environment.DocTypeData, dt);

			types.Columns.Add(Environment.StringResources.GetString("DocumentType"),
							  types.Width - SystemInformation.VerticalScrollBarWidth - 4, HorizontalAlignment.Left);
			subtypes.Columns.Add(Environment.StringResources.GetString("DocumentType"),
								 subtypes.Width - SystemInformation.VerticalScrollBarWidth - 4, HorizontalAlignment.Left);
			synonyms.Columns.Add(Environment.StringResources.GetString("DocumentType"),
								 synonyms.Width - SystemInformation.VerticalScrollBarWidth - 4, HorizontalAlignment.Left);
			FillTypes();
			if(pattern != null)
			{
				filter.Text = pattern;
				filter.SelectAll();
			}
		}

		private void AddRow(DataRow dr, ListView lv)
		{
			string obj = CultureObj(dr) as string;
			if(string.IsNullOrEmpty(obj))
				return;

			var id = (int)dr[Environment.DocTypeData.IDField];
			if(id <= 0)
				return;

			lv.Items.Add(new ListItem(id, obj) { ToolTipText = obj });
		}

		private object CultureObj(DataRow dr)
		{
			return Environment.CurCultureInfo.Name.StartsWith("ru") ? dr[Environment.DocTypeData.NameField] : dr[Environment.DocTypeData.TypeDocField];
		}

		private void FillTypes()
		{
			types.Items.Clear();

			string txt = filter.Text.Trim();

			if(string.IsNullOrEmpty(txt))
			{
				if(dt != null && dt.Rows.Count > 0)
				{
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

			if(TypeID != 0)
				for(int i = 0; i < types.Items.Count; i++)
				{
					var item = (ListItem)types.Items[i];
					if(item.ID == TypeID)
					{
						TypeID = 0;
						item.Selected = true;
						item.EnsureVisible();
						break;
					}
				}
		}

		private void types_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(types.SelectedItems.Count > 0)
			{
				var item = (ListItem)types.SelectedItems[0];
				TypeID = item.ID;
				Type = item.Text;

				FillSubtypes(TypeID);
				if(!formNeeded)
					FillSynonyms(TypeID);
				else
					panelInOut.Visible = Environment.PersonID > 0 && Environment.DocTypeData.GetDocBoolField(Environment.DocTypeData.OutGoingField, TypeID);
			}
			else
				TypeID = 0;

			buttonOK.Enabled = (TypeID != 0);
		}

		private void filter_TextChanged(object sender, EventArgs e)
		{
			FillTypes();
		}

		private void FillSubtypes(int typeID)
		{
			subtypes.Items.Clear();

			using(DataSet ds = Environment.DocTypeData.GetChildDocTypes(typeID, formNeeded))
			{
				if(ds != null)
				{
					using(DataTable dt = ds.Tables[0])
					{
						for(int i = 0; i < dt.Rows.Count; i++)
							AddRow(dt.Rows[i], subtypes);
						cbSubtypes.Checked = dt.Rows.Count > 0;
					}
				}
			}
		}

		private void FillSynonyms(int typeID)
		{
			synonyms.Items.Clear();

			using(DataSet ds = Environment.DocTypeData.GetSynonymDocTypes(typeID, formNeeded, showCheckBoxes))
			{
				if(ds == null)
					return;
				using(DataTable dt = ds.Tables[0])
				{
					for(int i = 0; i < dt.Rows.Count; i++)
						AddRow(dt.Rows[i], synonyms);
				}

			}
			groupSynonyms.Text = showCheckBoxes || Environment.DocTypeData.IsParentSynonymGroup(typeID)
									 ? Environment.StringResources.GetString("Similar")
									 : Environment.StringResources.GetString("SpecifyTypes");
		}

		private void subtypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(subtypes.SelectedItems.Count > 0)
			{
				synonyms.SelectedItems.Clear();

				var item = (ListItem)subtypes.SelectedItems[0];
				TypeID = item.ID;
				Type = item.Text;
				if(formNeeded)
				{
					panelInOut.Visible = Environment.PersonID > 0 &&
										 Environment.DocTypeData.GetDocBoolField(Environment.DocTypeData.OutGoingField,
																				 TypeID);
				}
			}
			else
			{
				if(synonyms.SelectedItems.Count != 0)
				{
					panelInOut.Visible = false;
					TypeID = 0;
				}
			}

			buttonOK.Enabled = (TypeID != 0);
		}

		private void synonyms_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(synonyms.SelectedItems.Count > 0)
			{
				subtypes.SelectedItems.Clear();

				var item = (ListItem)synonyms.SelectedItems[0];
				TypeID = item.ID;
				Type = item.Text;
			}
			else
			{
				if(subtypes.SelectedItems.Count != 0)
				{
					panelInOut.Visible = false;
					TypeID = 0;
				}
			}

			buttonOK.Enabled = (TypeID != 0);
		}

		private void subtypes_DoubleClick(object sender, EventArgs e)
		{
			if(subtypes.SelectedItems.Count == 0)
				return;
			panelInOut.Visible = false;
			var li = (ListItem)subtypes.SelectedItems[0];
			MarkType(li.ID);
		}

		private void synonyms_DoubleClick(object sender, EventArgs e)
		{
			if(synonyms.SelectedItems.Count == 0)
				return;
			var li = (ListItem)synonyms.SelectedItems[0];
			MarkType(li.ID);
		}

		private void MarkType(int id)
		{
			filter.Text = "";
			for(int i = 0; i < types.Items.Count; i++)
			{
				var li = (ListItem)types.Items[i];
				if(li.ID == id)
				{
					li.Selected = true;
					li.EnsureVisible();
				}
			}
		}

		private void types_DoubleClick(object sender, EventArgs e)
		{
			if(types.SelectedItems.Count > 0)
				if(formNeeded && panelInOut.Visible && (!radioButtonIN.Checked && !radioButtonOut.Checked))
					MessageForm.Show(
						Environment.StringResources.GetString("Select_SelectTypeDialog_types_DoubleClick_Message1"),
						Environment.StringResources.GetString("Warning"));
				else
					End(DialogResult.OK);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if(formNeeded && panelInOut.Visible && (!radioButtonIN.Checked && !radioButtonOut.Checked))
				MessageForm.Show(
					Environment.StringResources.GetString("Select_SelectTypeDialog_types_DoubleClick_Message1"),
					Environment.StringResources.GetString("Warning"));
			else
				End(DialogResult.OK);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			End(DialogResult.Cancel);
		}

		private void SelectTypeDialog_KeyUp(object sender, KeyEventArgs e)
		{
			if(panelPreviosSelect.Visible && e.KeyData == (Keys.F8 | Keys.Control))
			{
				ReturnPrevios();
			}
		}

		private void ReturnPrevios()
		{
			if(runTypeID <= 0)
				return;
			TypeID = runTypeID;
			if(panelInOut.Visible)
				if(inOut.Equals(Direction.Out))
					radioButtonOut.Checked = true;
				else
					radioButtonIN.Checked = true;
			End(DialogResult.OK);
		}

		private void labelReturn_Click(object sender, EventArgs e)
		{
		}

		private void labelReturn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			ReturnPrevios();
		}

		private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			types.Columns[0].Width = types.Width - SystemInformation.VerticalScrollBarWidth - 4;
			subtypes.Columns[0].Width = subtypes.Width - SystemInformation.VerticalScrollBarWidth - 4;
			synonyms.Columns[0].Width = synonyms.Width - SystemInformation.VerticalScrollBarWidth - 4;
		}
	}
}