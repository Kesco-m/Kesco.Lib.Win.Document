namespace Kesco.Lib.Win.Document.Dialogs
{
    partial class ColorCompressionDialog
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
			System.Windows.Forms.Button CancelBut;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorCompressionDialog));
			this.panel1 = new System.Windows.Forms.Panel();
			this.SaveBut = new System.Windows.Forms.Button();
			this.ImagiesTabControl = new System.Windows.Forms.TabControl();
			this.toolbarImageList = new System.Windows.Forms.ImageList(this.components);
			this.colorCompressionBlock1 = new Kesco.Lib.Win.Document.Dialogs.ColorCompressionBlock();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panelImage = new System.Windows.Forms.Panel();
			CancelBut = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// CancelBut
			// 
			resources.ApplyResources(CancelBut, "CancelBut");
			CancelBut.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			CancelBut.Name = "CancelBut";
			CancelBut.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(CancelBut);
			this.panel1.Controls.Add(this.SaveBut);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// SaveBut
			// 
			resources.ApplyResources(this.SaveBut, "SaveBut");
			this.SaveBut.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SaveBut.Name = "SaveBut";
			this.SaveBut.UseVisualStyleBackColor = true;
			this.SaveBut.Click += new System.EventHandler(this.SaveBut_Click);
			// 
			// ImagiesTabControl
			// 
			resources.ApplyResources(this.ImagiesTabControl, "ImagiesTabControl");
			this.ImagiesTabControl.Name = "ImagiesTabControl";
			this.ImagiesTabControl.SelectedIndex = 0;
			this.ImagiesTabControl.SelectedIndexChanged += new System.EventHandler(this.ImagiesTabControl_SelectedIndexChanged);
			this.ImagiesTabControl.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.ImagiesTabControl_Deselecting);
			// 
			// toolbarImageList
			// 
			this.toolbarImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolbarImageList.ImageStream")));
			this.toolbarImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.toolbarImageList.Images.SetKeyName(0, "");
			this.toolbarImageList.Images.SetKeyName(1, "");
			this.toolbarImageList.Images.SetKeyName(2, "");
			this.toolbarImageList.Images.SetKeyName(3, "");
			this.toolbarImageList.Images.SetKeyName(4, "");
			this.toolbarImageList.Images.SetKeyName(5, "");
			this.toolbarImageList.Images.SetKeyName(6, "");
			this.toolbarImageList.Images.SetKeyName(7, "");
			// 
			// colorCompressionBlock1
			// 
			resources.ApplyResources(this.colorCompressionBlock1, "colorCompressionBlock1");
			this.colorCompressionBlock1.Name = "colorCompressionBlock1";
			// 
			// panel2
			// 
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Controls.Add(this.panelImage);
			this.panel2.Name = "panel2";
			// 
			// panelImage
			// 
			resources.ApplyResources(this.panelImage, "panelImage");
			this.panelImage.Name = "panelImage";
			this.panelImage.Paint += new System.Windows.Forms.PaintEventHandler(this.panelImage_Paint);
			// 
			// ColorCompressionDialog
			// 
			this.AcceptButton = this.SaveBut;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = CancelBut;
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.colorCompressionBlock1);
			this.Controls.Add(this.ImagiesTabControl);
			this.Controls.Add(this.panel1);
			this.DoubleBuffered = true;
			this.Name = "ColorCompressionDialog";
			this.ShowIcon = false;
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl ImagiesTabControl;
		private System.Windows.Forms.Button SaveBut;
		internal System.Windows.Forms.ImageList toolbarImageList;
		private ColorCompressionBlock colorCompressionBlock1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panelImage;
    }
}