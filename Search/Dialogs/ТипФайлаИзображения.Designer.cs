namespace Kesco.Lib.Win.Document.Search.Dialogs
{
	partial class ТипФайлаИзображения
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ТипФайлаИзображения));
			this.checkBoxTIFF = new System.Windows.Forms.CheckBox();
			this.checkBoxPDF = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkBoxTIFF
			// 
			resources.ApplyResources(this.checkBoxTIFF, "checkBoxTIFF");
			this.checkBoxTIFF.Name = "checkBoxTIFF";
			this.checkBoxTIFF.UseVisualStyleBackColor = true;
			this.checkBoxTIFF.CheckedChanged += new System.EventHandler(this.checkBoxTIFF_CheckedChanged);
			// 
			// checkBoxPDF
			// 
			resources.ApplyResources(this.checkBoxPDF, "checkBoxPDF");
			this.checkBoxPDF.Name = "checkBoxPDF";
			this.checkBoxPDF.UseVisualStyleBackColor = true;
			this.checkBoxPDF.CheckedChanged += new System.EventHandler(this.checkBoxPDF_CheckedChanged);
			// 
			// ТипФайлаИзображения
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.checkBoxTIFF);
			this.Controls.Add(this.checkBoxPDF);
			this.Name = "ТипФайлаИзображения";
			this.Controls.SetChildIndex(this.checkBoxPDF, 0);
			this.Controls.SetChildIndex(this.checkBoxTIFF, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxTIFF;
		private System.Windows.Forms.CheckBox checkBoxPDF;
	}
}
