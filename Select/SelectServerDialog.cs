using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Items;

namespace Kesco.Lib.Win.Document.Select
{
    public class SelectServerDialog : FreeDialog
    {
        private SynchronizedCollection<Keys> keyLocker;

        private int serverID;

        private ListView list;

        private Button buttonOK;
        private Button buttonCancel;
        private ColumnHeader columnHeader1;
        private Container components;

        public SelectServerDialog(int serverID)
        {
            InitializeComponent();

            this.serverID = serverID;

            keyLocker = new SynchronizedCollection<Keys>();
        }

        #region Accessors

        public ListItem SelectedItem
        {
            get { return list.SelectedItems.Count > 0 ? list.SelectedItems[0] as ListItem : null; }
        }

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectServerDialog));
			this.list = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
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
			// SelectServerDialog
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.list);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectServerDialog";
			this.Load += new System.EventHandler(this.SelectServerDialog_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SelectUsedPersonDialog_KeyDown);
			this.ResumeLayout(false);

        }

        #endregion

        private void SelectServerDialog_Load(object sender, EventArgs e)
        {
            try
            {
                foreach (ServerInfo info in Environment.GetServers())
                {
                    list.Items.Add(new ListItem(info.ID, info.Name) {ToolTipText = info.Name});
                    if (info.ID == serverID)
                        list.Items[list.Items.Count - 1].Selected = true;
                }

                if (list.SelectedItems.Count == 0 && list.Items.Count > 0)
                    list.Items[0].Selected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Environment.StringResources.GetString("Error"));
                Data.Env.WriteToLog(ex);
            }
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
                End(DialogResult.OK);
        }

        private void list_DoubleClick(object sender, EventArgs e)
        {
            ManageIndex();
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