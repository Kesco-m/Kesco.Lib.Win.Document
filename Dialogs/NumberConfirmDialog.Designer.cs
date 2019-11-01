namespace Kesco.Lib.Win.Document.Dialogs
{
	partial class NumberConfirmDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NumberConfirmDialog));
			this.buttonNoNumber = new System.Windows.Forms.Button();
			this.buttonWithNumber = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labelText = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonNoNumber
			// 
			this.buttonNoNumber.AccessibleDescription = null;
			this.buttonNoNumber.AccessibleName = null;
			resources.ApplyResources(this.buttonNoNumber, "buttonNoNumber");
			this.buttonNoNumber.BackgroundImage = null;
			this.buttonNoNumber.DialogResult = System.Windows.Forms.DialogResult.No;
			this.buttonNoNumber.Font = null;
			this.buttonNoNumber.Name = "buttonNoNumber";
			this.buttonNoNumber.Click += new System.EventHandler(this.buttonNoNumber_Click);
			// 
			// buttonWithNumber
			// 
			this.buttonWithNumber.AccessibleDescription = null;
			this.buttonWithNumber.AccessibleName = null;
			resources.ApplyResources(this.buttonWithNumber, "buttonWithNumber");
			this.buttonWithNumber.BackgroundImage = null;
			this.buttonWithNumber.Font = null;
			this.buttonWithNumber.Name = "buttonWithNumber";
			this.buttonWithNumber.Click += new System.EventHandler(this.buttonWithNumber_Click);
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
			// panel1
			// 
			this.panel1.AccessibleDescription = null;
			this.panel1.AccessibleName = null;
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.BackgroundImage = null;
			this.panel1.Controls.Add(this.labelText);
			this.panel1.Font = null;
			this.panel1.Name = "panel1";
			// 
			// labelText
			// 
			this.labelText.AccessibleDescription = null;
			this.labelText.AccessibleName = null;
			resources.ApplyResources(this.labelText, "labelText");
			this.labelText.Font = null;
			this.labelText.Name = "labelText";
			// 
			// NumberConfirmDialog
			// 
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonWithNumber);
			this.Controls.Add(this.buttonNoNumber);
			this.Font = null;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = null;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NumberConfirmDialog";
			this.ShowInTaskbar = false;
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonNoNumber;
		private System.Windows.Forms.Button buttonWithNumber;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelText;
	}
}