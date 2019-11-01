namespace Kesco.Lib.Win.Document.Select
{
    partial class SelectTypeDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox groupSynonyms;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListView types;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView subtypes;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox filter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView synonyms;
        private System.Windows.Forms.RadioButton radioButtonIN;
        private System.Windows.Forms.RadioButton radioButtonOut;
        private System.Windows.Forms.Panel panelInOut;
        private System.Windows.Forms.Panel panelPreviosSelect;
        private System.Windows.Forms.LinkLabel labelReturn;
        private System.Windows.Forms.CheckBox cbSimilar;
        private System.Windows.Forms.Panel panelSubtypes;
        private System.Windows.Forms.CheckBox cbSubtypes;
        private System.Windows.Forms.Panel panelSimilar;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectTypeDialog));
			this.panel2 = new System.Windows.Forms.Panel();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.types = new System.Windows.Forms.ListView();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel5 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.subtypes = new System.Windows.Forms.ListView();
			this.panelSubtypes = new System.Windows.Forms.Panel();
			this.cbSubtypes = new System.Windows.Forms.CheckBox();
			this.panelSimilar = new System.Windows.Forms.Panel();
			this.cbSimilar = new System.Windows.Forms.CheckBox();
			this.groupSynonyms = new System.Windows.Forms.GroupBox();
			this.synonyms = new System.Windows.Forms.ListView();
			this.panelInOut = new System.Windows.Forms.Panel();
			this.radioButtonOut = new System.Windows.Forms.RadioButton();
			this.radioButtonIN = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.filter = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panelPreviosSelect = new System.Windows.Forms.Panel();
			this.labelReturn = new System.Windows.Forms.LinkLabel();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panel5.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panelSubtypes.SuspendLayout();
			this.panelSimilar.SuspendLayout();
			this.groupSynonyms.SuspendLayout();
			this.panelInOut.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panelPreviosSelect.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel2
			// 
			this.panel2.AccessibleDescription = null;
			this.panel2.AccessibleName = null;
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.BackgroundImage = null;
			this.panel2.Controls.Add(this.buttonOK);
			this.panel2.Controls.Add(this.buttonCancel);
			this.panel2.Font = null;
			this.panel2.Name = "panel2";
			// 
			// buttonOK
			// 
			this.buttonOK.AccessibleDescription = null;
			this.buttonOK.AccessibleName = null;
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.BackgroundImage = null;
			this.buttonOK.Font = null;
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.AccessibleDescription = null;
			this.buttonCancel.AccessibleName = null;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.BackgroundImage = null;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Font = null;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// types
			// 
			this.types.AccessibleDescription = null;
			this.types.AccessibleName = null;
			resources.ApplyResources(this.types, "types");
			this.types.BackgroundImage = null;
			this.types.Font = null;
			this.types.FullRowSelect = true;
			this.types.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.types.HideSelection = false;
			this.types.MultiSelect = false;
			this.types.Name = "types";
			this.types.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.types.UseCompatibleStateImageBehavior = false;
			this.types.View = System.Windows.Forms.View.Details;
			this.types.SelectedIndexChanged += new System.EventHandler(this.types_SelectedIndexChanged);
			this.types.DoubleClick += new System.EventHandler(this.types_DoubleClick);
			this.types.Enter += new System.EventHandler(this.types_SelectedIndexChanged);
			// 
			// panel3
			// 
			this.panel3.AccessibleDescription = null;
			this.panel3.AccessibleName = null;
			resources.ApplyResources(this.panel3, "panel3");
			this.panel3.BackgroundImage = null;
			this.panel3.Controls.Add(this.panel4);
			this.panel3.Controls.Add(this.splitter1);
			this.panel3.Controls.Add(this.panel5);
			this.panel3.Font = null;
			this.panel3.Name = "panel3";
			// 
			// panel4
			// 
			this.panel4.AccessibleDescription = null;
			this.panel4.AccessibleName = null;
			resources.ApplyResources(this.panel4, "panel4");
			this.panel4.BackgroundImage = null;
			this.panel4.Controls.Add(this.groupBox1);
			this.panel4.Font = null;
			this.panel4.Name = "panel4";
			// 
			// groupBox1
			// 
			this.groupBox1.AccessibleDescription = null;
			this.groupBox1.AccessibleName = null;
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.BackgroundImage = null;
			this.groupBox1.Controls.Add(this.types);
			this.groupBox1.Font = null;
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// splitter1
			// 
			this.splitter1.AccessibleDescription = null;
			this.splitter1.AccessibleName = null;
			resources.ApplyResources(this.splitter1, "splitter1");
			this.splitter1.BackgroundImage = null;
			this.splitter1.Font = null;
			this.splitter1.Name = "splitter1";
			this.splitter1.TabStop = false;
			this.splitter1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter1_SplitterMoved);
			// 
			// panel5
			// 
			this.panel5.AccessibleDescription = null;
			this.panel5.AccessibleName = null;
			resources.ApplyResources(this.panel5, "panel5");
			this.panel5.BackgroundImage = null;
			this.panel5.Controls.Add(this.groupBox2);
			this.panel5.Controls.Add(this.panelSubtypes);
			this.panel5.Controls.Add(this.panelSimilar);
			this.panel5.Controls.Add(this.groupSynonyms);
			this.panel5.Controls.Add(this.panelInOut);
			this.panel5.Font = null;
			this.panel5.Name = "panel5";
			// 
			// groupBox2
			// 
			this.groupBox2.AccessibleDescription = null;
			this.groupBox2.AccessibleName = null;
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.BackgroundImage = null;
			this.groupBox2.Controls.Add(this.subtypes);
			this.groupBox2.Font = null;
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// subtypes
			// 
			this.subtypes.AccessibleDescription = null;
			this.subtypes.AccessibleName = null;
			resources.ApplyResources(this.subtypes, "subtypes");
			this.subtypes.BackgroundImage = null;
			this.subtypes.Font = null;
			this.subtypes.FullRowSelect = true;
			this.subtypes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.subtypes.MultiSelect = false;
			this.subtypes.Name = "subtypes";
			this.subtypes.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.subtypes.UseCompatibleStateImageBehavior = false;
			this.subtypes.View = System.Windows.Forms.View.Details;
			this.subtypes.SelectedIndexChanged += new System.EventHandler(this.subtypes_SelectedIndexChanged);
			this.subtypes.DoubleClick += new System.EventHandler(this.subtypes_DoubleClick);
			this.subtypes.Enter += new System.EventHandler(this.subtypes_SelectedIndexChanged);
			// 
			// panelSubtypes
			// 
			this.panelSubtypes.AccessibleDescription = null;
			this.panelSubtypes.AccessibleName = null;
			resources.ApplyResources(this.panelSubtypes, "panelSubtypes");
			this.panelSubtypes.BackgroundImage = null;
			this.panelSubtypes.Controls.Add(this.cbSubtypes);
			this.panelSubtypes.Font = null;
			this.panelSubtypes.Name = "panelSubtypes";
			// 
			// cbSubtypes
			// 
			this.cbSubtypes.AccessibleDescription = null;
			this.cbSubtypes.AccessibleName = null;
			resources.ApplyResources(this.cbSubtypes, "cbSubtypes");
			this.cbSubtypes.BackgroundImage = null;
			this.cbSubtypes.Font = null;
			this.cbSubtypes.Name = "cbSubtypes";
			// 
			// panelSimilar
			// 
			this.panelSimilar.AccessibleDescription = null;
			this.panelSimilar.AccessibleName = null;
			resources.ApplyResources(this.panelSimilar, "panelSimilar");
			this.panelSimilar.BackgroundImage = null;
			this.panelSimilar.Controls.Add(this.cbSimilar);
			this.panelSimilar.Font = null;
			this.panelSimilar.Name = "panelSimilar";
			// 
			// cbSimilar
			// 
			this.cbSimilar.AccessibleDescription = null;
			this.cbSimilar.AccessibleName = null;
			resources.ApplyResources(this.cbSimilar, "cbSimilar");
			this.cbSimilar.BackgroundImage = null;
			this.cbSimilar.Font = null;
			this.cbSimilar.Name = "cbSimilar";
			// 
			// groupSynonyms
			// 
			this.groupSynonyms.AccessibleDescription = null;
			this.groupSynonyms.AccessibleName = null;
			resources.ApplyResources(this.groupSynonyms, "groupSynonyms");
			this.groupSynonyms.BackgroundImage = null;
			this.groupSynonyms.Controls.Add(this.synonyms);
			this.groupSynonyms.Font = null;
			this.groupSynonyms.Name = "groupSynonyms";
			this.groupSynonyms.TabStop = false;
			// 
			// synonyms
			// 
			this.synonyms.AccessibleDescription = null;
			this.synonyms.AccessibleName = null;
			resources.ApplyResources(this.synonyms, "synonyms");
			this.synonyms.BackgroundImage = null;
			this.synonyms.Font = null;
			this.synonyms.FullRowSelect = true;
			this.synonyms.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.synonyms.MultiSelect = false;
			this.synonyms.Name = "synonyms";
			this.synonyms.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.synonyms.UseCompatibleStateImageBehavior = false;
			this.synonyms.View = System.Windows.Forms.View.Details;
			this.synonyms.SelectedIndexChanged += new System.EventHandler(this.synonyms_SelectedIndexChanged);
			this.synonyms.DoubleClick += new System.EventHandler(this.synonyms_DoubleClick);
			this.synonyms.Enter += new System.EventHandler(this.synonyms_SelectedIndexChanged);
			// 
			// panelInOut
			// 
			this.panelInOut.AccessibleDescription = null;
			this.panelInOut.AccessibleName = null;
			resources.ApplyResources(this.panelInOut, "panelInOut");
			this.panelInOut.BackgroundImage = null;
			this.panelInOut.Controls.Add(this.radioButtonOut);
			this.panelInOut.Controls.Add(this.radioButtonIN);
			this.panelInOut.Font = null;
			this.panelInOut.Name = "panelInOut";
			// 
			// radioButtonOut
			// 
			this.radioButtonOut.AccessibleDescription = null;
			this.radioButtonOut.AccessibleName = null;
			resources.ApplyResources(this.radioButtonOut, "radioButtonOut");
			this.radioButtonOut.BackgroundImage = null;
			this.radioButtonOut.Font = null;
			this.radioButtonOut.Name = "radioButtonOut";
			this.radioButtonOut.TabStop = true;
			// 
			// radioButtonIN
			// 
			this.radioButtonIN.AccessibleDescription = null;
			this.radioButtonIN.AccessibleName = null;
			resources.ApplyResources(this.radioButtonIN, "radioButtonIN");
			this.radioButtonIN.BackgroundImage = null;
			this.radioButtonIN.Font = null;
			this.radioButtonIN.Name = "radioButtonIN";
			this.radioButtonIN.TabStop = true;
			// 
			// panel1
			// 
			this.panel1.AccessibleDescription = null;
			this.panel1.AccessibleName = null;
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.BackgroundImage = null;
			this.panel1.Controls.Add(this.filter);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Font = null;
			this.panel1.Name = "panel1";
			// 
			// filter
			// 
			this.filter.AccessibleDescription = null;
			this.filter.AccessibleName = null;
			resources.ApplyResources(this.filter, "filter");
			this.filter.BackgroundImage = null;
			this.filter.Font = null;
			this.filter.Name = "filter";
			this.filter.TextChanged += new System.EventHandler(this.filter_TextChanged);
			// 
			// label1
			// 
			this.label1.AccessibleDescription = null;
			this.label1.AccessibleName = null;
			resources.ApplyResources(this.label1, "label1");
			this.label1.Font = null;
			this.label1.Name = "label1";
			// 
			// panelPreviosSelect
			// 
			this.panelPreviosSelect.AccessibleDescription = null;
			this.panelPreviosSelect.AccessibleName = null;
			resources.ApplyResources(this.panelPreviosSelect, "panelPreviosSelect");
			this.panelPreviosSelect.BackgroundImage = null;
			this.panelPreviosSelect.Controls.Add(this.labelReturn);
			this.panelPreviosSelect.Font = null;
			this.panelPreviosSelect.Name = "panelPreviosSelect";
			// 
			// labelReturn
			// 
			this.labelReturn.AccessibleDescription = null;
			this.labelReturn.AccessibleName = null;
			resources.ApplyResources(this.labelReturn, "labelReturn");
			this.labelReturn.Font = null;
			this.labelReturn.Name = "labelReturn";
			this.labelReturn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelReturn_LinkClicked);
			this.labelReturn.Click += new System.EventHandler(this.labelReturn_Click);
			// 
			// SelectTypeDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.BackgroundImage = null;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panelPreviosSelect);
			this.Controls.Add(this.panel2);
			this.Font = null;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = null;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectTypeDialog";
			this.Load += new System.EventHandler(this.SelectType_Load);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SelectTypeDialog_KeyUp);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.panelSubtypes.ResumeLayout(false);
			this.panelSimilar.ResumeLayout(false);
			this.groupSynonyms.ResumeLayout(false);
			this.panelInOut.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panelPreviosSelect.ResumeLayout(false);
			this.ResumeLayout(false);

        }
        #endregion
    }
}