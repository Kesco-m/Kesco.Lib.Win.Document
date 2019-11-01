using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
	public class КодДокумента : Base
	{
		internal TextBox пКодДокумента;
		private Label label1;
		private Button buttonAdd;
		private Button buttonDelete;
		private ListView lv;
		private ColumnHeader chID;

		private Container components;

		public КодДокумента()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///   Required method for Designer support - do not modify the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			var resources =
				new System.ComponentModel.ComponentResourceManager(typeof(КодДокумента));
			this.пКодДокумента = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lv = new System.Windows.Forms.ListView();
			this.chID = new System.Windows.Forms.ColumnHeader();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// пКодДокумента
			// 
			resources.ApplyResources(this.пКодДокумента, "пКодДокумента");
			this.пКодДокумента.Name = "пКодДокумента";
			this.пКодДокумента.TextChanged += new System.EventHandler(this.пКодДокумента_TextChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// lv
			// 
			this.lv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                                         {
                                             this.chID
                                         });
			this.lv.FullRowSelect = true;
			this.lv.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lv.HideSelection = false;
			resources.ApplyResources(this.lv, "lv");
			this.lv.Name = "lv";
			this.lv.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lv.UseCompatibleStateImageBehavior = false;
			this.lv.View = System.Windows.Forms.View.Details;
			this.lv.SelectedIndexChanged += new System.EventHandler(this.lv_SelectedIndexChanged);
			// 
			// chID
			// 
			resources.ApplyResources(this.chID, "chID");
			// 
			// buttonAdd
			// 
			resources.ApplyResources(this.buttonAdd, "buttonAdd");
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonDelete
			// 
			resources.ApplyResources(this.buttonDelete, "buttonDelete");
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// КодДокумента
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.lv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.пКодДокумента);
			this.Name = "КодДокумента";
			this.Controls.SetChildIndex(this.пКодДокумента, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.lv, 0);
			this.Controls.SetChildIndex(this.buttonAdd, 0);
			this.Controls.SetChildIndex(this.buttonDelete, 0);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		internal override void DialogBase_Load(object sender, EventArgs e)
		{
			base.DialogBase_Load(sender, e);
			chID.Width = lv.Width - SystemInformation.VerticalScrollBarWidth - 4;
		}

		protected override void FillElement()
		{
			string id = пКодДокумента.Text.Replace(".", "").Trim();
			string val = id;
			if((val.Length > 0 && !Regex.IsMatch(val, "^[1-9][0-9]{0,9}$")) || (val.Length == 0 && lv.Items.Count == 0))
				throw new Exception(Environment.StringResources.GetString("Search_Dialogs_КодДокумента_Error1"));
			for(int i = 0; i < lv.Items.Count; i++)
			{
				if(id != lv.Items[i].Text)
					val += ((val.Length > 0) ? "," : "") + lv.Items[i].Text;
			}
			elOption.SetAttribute("value", val);
			base.FillElement();
		}

		protected override void FillForm()
		{
			if(elOption != null)
			{
				var r = new Regex("^\\d{1,9}$");
				string[] val = elOption.GetAttribute("value").Split(',');

				foreach(string t in val.Where(t => r.IsMatch(t)))
					lv.Items.Add(new ListViewItem(t));
			}
			base.FillForm();
		}


		private void lv_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonDelete.Enabled = lv.SelectedItems.Count > 0;
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			if(!Regex.IsMatch(пКодДокумента.Text.Replace(".", "").Trim(), "^[1-9][0-9]{0,9}$"))
			{
				Error.ErrorShower.OnShowError(this, Environment.StringResources.GetString("Search_Dialogs_КодДокумента_Error1"), "");
				return;
			}
			string idstr = пКодДокумента.Text.Replace(".", "").Trim();
			for(int i = 0; i < lv.Items.Count; i++)
			{
				if(idstr == lv.Items[i].Text)
					return;
			}
			пКодДокумента.Text = "";
			lv.Items.Add(new ListViewItem(idstr));
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if(lv.SelectedItems.Count == 1)
				lv.Items.Remove(lv.SelectedItems[0]);
		}

		private void пКодДокумента_TextChanged(object sender, EventArgs e)
		{
			buttonAdd.Enabled = (пКодДокумента.Text.Trim().Length > 0);
		}
	}
}