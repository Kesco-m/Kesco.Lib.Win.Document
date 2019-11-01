using Kesco.Lib.Win.Document.Blocks;
using Kesco.Lib.Win.Document.ListViews;

namespace Kesco.Lib.Win.Document.Dialogs
{
    partial class MailingListShareDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MailingListShareDialog));
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonDeleteEmployees = new System.Windows.Forms.Button();
            this.list = new DelableListView();
            this.gbAddEmployees = new System.Windows.Forms.GroupBox();
			this.employeeBlock = new EmployeeBlock();
            this.labelMailingList = new System.Windows.Forms.Label();
            this.gbAddEmployees.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            resources.ApplyResources(this.buttonSave, "buttonSave");
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonDeleteEmployees
            // 
            resources.ApplyResources(this.buttonDeleteEmployees, "buttonDeleteEmployees");
            this.buttonDeleteEmployees.Name = "buttonDeleteEmployees";
            this.buttonDeleteEmployees.UseVisualStyleBackColor = true;
            this.buttonDeleteEmployees.Click += new System.EventHandler(this.buttonDeleteEmployees_Click);
            // 
            // list
            // 
            resources.ApplyResources(this.list, "list");
            this.list.FullRowSelect = true;
            this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.list.HideSelection = false;
            this.list.MultiSelect = false;
            this.list.Name = "list";
            this.list.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.Details;
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            // 
            // gbAddEmployees
            // 
            resources.ApplyResources(this.gbAddEmployees, "gbAddEmployees");
            this.gbAddEmployees.Controls.Add(this.employeeBlock);
            this.gbAddEmployees.Name = "gbAddEmployees";
            this.gbAddEmployees.TabStop = false;
            // 
            // employeeBlock
            // 
            resources.ApplyResources(this.employeeBlock, "employeeBlock");
            this.employeeBlock.BackColor = System.Drawing.SystemColors.Control;
            this.employeeBlock.Name = "employeeBlock";
            this.employeeBlock.ParamStr = "clid=3&UserAccountDisabled=0&return=2";
			this.employeeBlock.EmployeeSelected += new EmployeeBlockEventHandler(this.employeeBlock_EmployeeSelected);
            // 
            // labelMailingList
            // 
            resources.ApplyResources(this.labelMailingList, "labelMailingList");
            this.labelMailingList.Name = "labelMailingList";
            // 
            // MailingListShareDialog
            // 
            this.AcceptButton = this.buttonSave;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonDeleteEmployees);
            this.Controls.Add(this.labelMailingList);
            this.Controls.Add(this.list);
            this.Controls.Add(this.gbAddEmployees);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MailingListShareDialog";
            this.Load += new System.EventHandler(this.MailingListShareDialog_Load);
            this.gbAddEmployees.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Blocks.EmployeeBlock employeeBlock;
        private DelableListView list;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox gbAddEmployees;
        private System.Windows.Forms.Button buttonDeleteEmployees;
        private System.Windows.Forms.Label labelMailingList;
    }
}