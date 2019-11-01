namespace Kesco.Lib.Win.Document.Blocks
{
	partial class PersonBaseBlock
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.showBox = new System.Windows.Forms.ComboBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.searchBox = new Lib.Win.UpDownTextBox();
            this.SuspendLayout();
            // 
            // showBox
            // 
            this.showBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.showBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.showBox.Location = new System.Drawing.Point(4, 4);
            this.showBox.MaxDropDownItems = 11;
            this.showBox.Name = "showBox";
            this.showBox.Size = new System.Drawing.Size(157, 21);
            this.showBox.TabIndex = 2;
            this.showBox.TabStop = false;
            this.showBox.DropDown += new System.EventHandler(this.showBox_DropDown);
            this.showBox.SelectionChangeCommitted += new System.EventHandler(this.showBox_SelectionChangeCommitted);
            this.showBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.showBox_KeyDown);
            this.showBox.Leave += new System.EventHandler(this.showBox_Leave);
            // 
            // buttonSearch
            // 
            this.buttonSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonSearch.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonSearch.Location = new System.Drawing.Point(167, 2);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(58, 23);
            this.buttonSearch.TabIndex = 1;
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.HideSelection = false;
            this.searchBox.Location = new System.Drawing.Point(4, 4);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(157, 20);
            this.searchBox.TabIndex = 0;
            this.searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchBox_KeyDown);
            this.searchBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.searchBox_KeyUp);
            this.searchBox.Leave += new System.EventHandler(this.searchBox_Leave);
            // 
            // PersonBaseBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.showBox);
            this.Controls.Add(this.buttonSearch);
            this.DoubleBuffered = true;
            this.Name = "PersonBaseBlock";
            this.Size = new System.Drawing.Size(229, 29);
            this.Load += new System.EventHandler(this.PersonBaseBlock_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox showBox;
		private System.Windows.Forms.Button buttonSearch;
		private Lib.Win.UpDownTextBox searchBox;
	}
}
