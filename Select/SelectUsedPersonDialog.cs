using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Items;

namespace Kesco.Lib.Win.Document.Select
{
    public class SelectUsedPersonDialog : FreeDialog
    {
        private SynchronizedCollection<Keys> keyLocker;

        private ListView list;
        private Button buttonOK;
        private Button buttonCancel;
        private ColumnHeader columnHeader1;
        private Button buttonSearch;

        private Container components;

        public SelectUsedPersonDialog(DataRow[] rows)
        {
            Index = -1;
            InitializeComponent();

            Rows = rows;

            keyLocker = new SynchronizedCollection<Keys>();
        }

        #region Accessors

        public int Index { get; private set; }

        public DataRow[] Rows { get; private set; }

        #endregion

        /// <summary>
        ///   Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectUsedPersonDialog));
			this.list = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonSearch = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// list
			// 
			resources.ApplyResources(this.list, "list");
			this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.list.FullRowSelect = true;
			this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.list.HideSelection = false;
			this.list.MultiSelect = false;
			this.list.Name = "list";
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
			this.list.DoubleClick += new System.EventHandler(this.list_DoubleClick);
			// 
			// columnHeader1
			// 
			resources.ApplyResources(this.columnHeader1, "columnHeader1");
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonSearch
			// 
			resources.ApplyResources(this.buttonSearch, "buttonSearch");
			this.buttonSearch.Name = "buttonSearch";
			this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
			// 
			// SelectUsedPersonDialog
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.list);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonSearch);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectUsedPersonDialog";
			this.Load += new System.EventHandler(this.SelectUsedPersonDialog_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SelectUsedPersonDialog_KeyDown);
			this.ResumeLayout(false);

        }

        #endregion

        private void SelectUsedPersonDialog_Load(object sender, EventArgs e)
        {
            Text = Environment.StringResources.GetString("Choice") + " " +
                        Environment.PersonWord.GetForm(Cases.R, false, false);

            foreach (DataRow dr in Rows)
            {
                AddPerson(
                    (int) dr[Environment.PersonsUsedData.IDField],
                    (string) dr[Environment.PersonsUsedData.NameField]);
            }

            if (list.Items.Count > 0)
                list.Items[0].Selected = true;
        }

        public void AddPerson(int id, string name)
        {
            if (!list.Items.Cast<ListItem>().Any(item => item.ID == id && string.Equals(item.Name, name)))
                list.Items.Add(new ListItem(id, name) {ToolTipText = name});
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = (list.SelectedItems.Count > 0);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ManageIndex();
        }

        private void ManageIndex()
        {
            if (list.SelectedItems.Count > 0)
            {
                Index = list.SelectedIndices[0];
                End(DialogResult.OK);
            }
            else
                Index = -1;
        }

        private void list_DoubleClick(object sender, EventArgs e)
        {
            ManageIndex();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            End(DialogResult.Retry);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void SelectUsedPersonDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (keyLocker.Contains(e.KeyData))
                return;
            
            keyLocker.Add(e.KeyData);
            try
            {
                if (e.KeyData == Keys.Enter)
                    ManageIndex();
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
            finally
            {
                keyLocker.Remove(e.KeyData);
            }
        }
    }
}