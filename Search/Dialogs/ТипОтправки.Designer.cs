namespace Kesco.Lib.Win.Document.Search.Dialogs
{
	partial class ТипОтправки
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ТипОтправки));
			this.checkBoxEmail = new System.Windows.Forms.CheckBox();
			this.checkBoxFax = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkBoxEmail
			// 
			resources.ApplyResources(this.checkBoxEmail, "checkBoxEmail");
			this.checkBoxEmail.Name = "checkBoxEmail";
			this.checkBoxEmail.UseVisualStyleBackColor = true;
			this.checkBoxEmail.CheckedChanged += new System.EventHandler(this.checkBoxEmail_CheckedChanged);
			// 
			// checkBoxFax
			// 
			resources.ApplyResources(this.checkBoxFax, "checkBoxFax");
			this.checkBoxFax.Name = "checkBoxFax";
			this.checkBoxFax.UseVisualStyleBackColor = true;
			this.checkBoxFax.CheckedChanged += new System.EventHandler(this.checkBoxFax_CheckedChanged);
			// 
			// ТипОтправки
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.checkBoxEmail);
			this.Controls.Add(this.checkBoxFax);
			this.Name = "ТипОтправки";
			this.Controls.SetChildIndex(this.checkBoxFax, 0);
			this.Controls.SetChildIndex(this.checkBoxEmail, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxEmail;
		private System.Windows.Forms.CheckBox checkBoxFax;
	}
}
