namespace Kesco.Lib.Win.Document.Select
{
	partial class SelectPrintingFormDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectPrintingFormDialog));
            this.splitContainerType = new System.Windows.Forms.SplitContainer();
            this.groupBoxType = new System.Windows.Forms.GroupBox();
            this.types = new System.Windows.Forms.ListView();
            this.groupBoxPrintForm = new System.Windows.Forms.GroupBox();
            this.lvPrintForms = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxPrintForm = new System.Windows.Forms.CheckBox();
            this.checkBoxType = new System.Windows.Forms.CheckBox();
            this.filter = new System.Windows.Forms.TextBox();
            this.labelFind = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.splitContainerType.Panel1.SuspendLayout();
            this.splitContainerType.Panel2.SuspendLayout();
            this.splitContainerType.SuspendLayout();
            this.groupBoxType.SuspendLayout();
            this.groupBoxPrintForm.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerType
            // 
            resources.ApplyResources(this.splitContainerType, "splitContainerType");
            this.splitContainerType.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerType.Name = "splitContainerType";
            // 
            // splitContainerType.Panel1
            // 
            this.splitContainerType.Panel1.Controls.Add(this.groupBoxType);
            // 
            // splitContainerType.Panel2
            // 
            this.splitContainerType.Panel2.Controls.Add(this.groupBoxPrintForm);
            // 
            // groupBoxType
            // 
            this.groupBoxType.Controls.Add(this.types);
            resources.ApplyResources(this.groupBoxType, "groupBoxType");
            this.groupBoxType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxType.Name = "groupBoxType";
            this.groupBoxType.TabStop = false;
            // 
            // types
            // 
            resources.ApplyResources(this.types, "types");
            this.types.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.types.HideSelection = false;
            this.types.MultiSelect = false;
            this.types.Name = "types";
            this.types.ShowGroups = false;
            this.types.ShowItemToolTips = true;
            this.types.UseCompatibleStateImageBehavior = false;
            this.types.View = System.Windows.Forms.View.Details;
            this.types.SelectedIndexChanged += new System.EventHandler(this.types_SelectedIndexChanged);
            this.types.SizeChanged += new System.EventHandler(this.lv_SizeChanged);
            this.types.Enter += new System.EventHandler(this.lv_Enter);
            // 
            // groupBoxPrintForm
            // 
            this.groupBoxPrintForm.Controls.Add(this.lvPrintForms);
            resources.ApplyResources(this.groupBoxPrintForm, "groupBoxPrintForm");
            this.groupBoxPrintForm.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBoxPrintForm.Name = "groupBoxPrintForm";
            this.groupBoxPrintForm.TabStop = false;
            // 
            // lvPrintForms
            // 
            resources.ApplyResources(this.lvPrintForms, "lvPrintForms");
            this.lvPrintForms.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvPrintForms.HideSelection = false;
            this.lvPrintForms.Name = "lvPrintForms";
            this.lvPrintForms.ShowGroups = false;
            this.lvPrintForms.ShowItemToolTips = true;
            this.lvPrintForms.UseCompatibleStateImageBehavior = false;
            this.lvPrintForms.View = System.Windows.Forms.View.Details;
            this.lvPrintForms.SelectedIndexChanged += new System.EventHandler(this.lvPrintForms_SelectedIndexChanged);
            this.lvPrintForms.SizeChanged += new System.EventHandler(this.lv_SizeChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxPrintForm);
            this.panel1.Controls.Add(this.checkBoxType);
            this.panel1.Controls.Add(this.filter);
            this.panel1.Controls.Add(this.labelFind);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // checkBoxPrintForm
            // 
            resources.ApplyResources(this.checkBoxPrintForm, "checkBoxPrintForm");
            this.checkBoxPrintForm.Checked = true;
            this.checkBoxPrintForm.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPrintForm.Name = "checkBoxPrintForm";
            this.checkBoxPrintForm.UseVisualStyleBackColor = true;
            // 
            // checkBoxType
            // 
            resources.ApplyResources(this.checkBoxType, "checkBoxType");
            this.checkBoxType.Checked = true;
            this.checkBoxType.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxType.Name = "checkBoxType";
            this.checkBoxType.UseVisualStyleBackColor = true;
            // 
            // filter
            // 
            resources.ApplyResources(this.filter, "filter");
            this.filter.Name = "filter";
            this.filter.TextChanged += new System.EventHandler(this.filter_TextChanged);
            // 
            // labelFind
            // 
            resources.ApplyResources(this.labelFind, "labelFind");
            this.labelFind.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelFind.Name = "labelFind";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ButtonCancel);
            this.panel2.Controls.Add(this.buttonOK);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // ButtonCancel
            // 
            resources.ApplyResources(this.ButtonCancel, "ButtonCancel");
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // SelectPrintingFormDialog
            // 
            this.CancelButton = this.ButtonCancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.splitContainerType);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectPrintingFormDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.SelectPrintingFormDialog_Load);
            this.splitContainerType.Panel1.ResumeLayout(false);
            this.splitContainerType.Panel2.ResumeLayout(false);
            this.splitContainerType.ResumeLayout(false);
            this.groupBoxType.ResumeLayout(false);
            this.groupBoxPrintForm.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox filter;
		private System.Windows.Forms.Label labelFind;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.SplitContainer splitContainerType;
		private System.Windows.Forms.GroupBox groupBoxType;
		private System.Windows.Forms.GroupBox groupBoxPrintForm;
		private System.Windows.Forms.CheckBox checkBoxPrintForm;
		private System.Windows.Forms.CheckBox checkBoxType;
		private System.Windows.Forms.Button ButtonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ListView types;
		private System.Windows.Forms.ListView lvPrintForms;
	}
}
