using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Select;
using Kesco.Lib.Win.Trees;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class Хранилище : Base
    {
        private int ArchiveID;
        private Button btSelect;
        private TextBox tbText;
        private CheckBox cbSubNodes;
        private Container components;

        public Хранилище()
        {
            InitializeComponent();
        }

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
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (Хранилище));
            this.btSelect = new System.Windows.Forms.Button();
            this.tbText = new System.Windows.Forms.TextBox();
            this.cbSubNodes = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btSelect
            // 
            resources.ApplyResources(this.btSelect, "btSelect");
            this.btSelect.Name = "btSelect";
            this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
            // 
            // tbText
            // 
            resources.ApplyResources(this.tbText, "tbText");
            this.tbText.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tbText.Name = "tbText";
            // 
            // cbSubNodes
            // 
            resources.ApplyResources(this.cbSubNodes, "cbSubNodes");
            this.cbSubNodes.Name = "cbSubNodes";
            // 
            // Хранилище
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.cbSubNodes);
            this.Controls.Add(this.tbText);
            this.Controls.Add(this.btSelect);
            this.Name = "Хранилище";
            this.Controls.SetChildIndex(this.btSelect, 0);
            this.Controls.SetChildIndex(this.tbText, 0);
            this.Controls.SetChildIndex(this.cbSubNodes, 0);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        protected override void FillForm()
        {
            var r = new Regex("^true$", RegexOptions.IgnoreCase);
            cbSubNodes.Checked = r.IsMatch(elOption.GetAttribute("subnodes"));

            r = new Regex("^\\d{1,9}$");

            ArchiveID = (r.IsMatch(elOption.GetAttribute("value")) ? int.Parse(elOption.GetAttribute("value")) : 0);
            tbText.Text = (ArchiveID == 0)
                              ? Environment.StringResources.GetString("NotSelected")
                              : Data.DALC.Documents.Search.Image.Хранилище.GetName(ArchiveID);

            base.FillForm();
        }

        protected override void FillElement()
        {
            elOption.SetAttribute("value", ArchiveID.ToString());
            elOption.SetAttribute("subnodes", cbSubNodes.Checked.ToString().ToLower());
            base.FillElement();
        }

        private void btSelect_Click(object sender, EventArgs e)
        {
            var dialog = new SelectArchiveDialog(ArchiveID, true);
            dialog.DialogEvent += dialog_DialogEvent;
            ShowSubForm(dialog);
            Enabled = false;
        }

        public void dialog_DialogEvent(object sender, DialogEventArgs e)
        {
            Enabled = true;
            Focus();

            var dialog = (SelectArchiveDialog) e.Dialog;
            if (dialog.DialogResult != DialogResult.OK)
                return;
            DTreeNode node = dialog.ArchiveNode;
            if (node == null) 
                return;

            ArchiveID = node.ID;
            tbText.Text = node.Text;
            cbSubNodes.Checked = node.Nodes.Count > 0;
        }
    }
}