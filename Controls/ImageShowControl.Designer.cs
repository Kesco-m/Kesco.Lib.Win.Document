namespace Kesco.Lib.Win.Document.Controls
{
	partial class ImageShowControl
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageShowControl));
			this.buttonLoad = new System.Windows.Forms.Button();
			this.labelNoImage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonLoad
			// 
			resources.ApplyResources(this.buttonLoad, "buttonLoad");
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
			// 
			// labelNoImage
			// 
			resources.ApplyResources(this.labelNoImage, "labelNoImage");
			this.labelNoImage.Name = "labelNoImage";
			// 
			// ImageShowControl
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.buttonLoad);
			this.Controls.Add(this.labelNoImage);
			this.Name = "ImageShowControl";
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private System.Windows.Forms.Label labelNoImage;
		private System.Windows.Forms.Button buttonLoad;

		#endregion
	}
}
