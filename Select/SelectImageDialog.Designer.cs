namespace Kesco.Lib.Win.Document.Select
{
	partial class SelectImageDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectImageDialog));
            this.listViewImages = new System.Windows.Forms.ListView();
            this.varImageList = new System.Windows.Forms.ImageList(this.components);
            this.radioButtonBefore = new System.Windows.Forms.RadioButton();
            this.radioButtonBetween = new System.Windows.Forms.RadioButton();
            this.radioButtonAfter = new System.Windows.Forms.RadioButton();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxCho = new System.Windows.Forms.TextBox();
            this.docControl1 = new Kesco.Lib.Win.Document.Controls.DocControl();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.SuspendLayout();
            // 
            // listViewImages
            // 
            resources.ApplyResources(this.listViewImages, "listViewImages");
            this.listViewImages.HideSelection = false;
            this.listViewImages.LargeImageList = this.varImageList;
            this.listViewImages.MultiSelect = false;
            this.listViewImages.Name = "listViewImages";
            this.listViewImages.SmallImageList = this.varImageList;
            this.listViewImages.StateImageList = this.varImageList;
            this.listViewImages.UseCompatibleStateImageBehavior = false;
            this.listViewImages.SelectedIndexChanged += new System.EventHandler(this.listViewImages_SelectedIndexChanged);
            // 
            // varImageList
            // 
            this.varImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("varImageList.ImageStream")));
            this.varImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.varImageList.Images.SetKeyName(0, "");
            this.varImageList.Images.SetKeyName(1, "");
            this.varImageList.Images.SetKeyName(2, "");
            this.varImageList.Images.SetKeyName(3, "");
            this.varImageList.Images.SetKeyName(4, "DocPrint.gif");
            this.varImageList.Images.SetKeyName(5, "DocMainPrint.gif");
            this.varImageList.Images.SetKeyName(6, "DocOrigPrint.gif");
            this.varImageList.Images.SetKeyName(7, "DocMainOrigPrint.gif");
            this.varImageList.Images.SetKeyName(8, "");
            this.varImageList.Images.SetKeyName(9, "");
            this.varImageList.Images.SetKeyName(10, "");
            this.varImageList.Images.SetKeyName(11, "");
            this.varImageList.Images.SetKeyName(12, "");
            this.varImageList.Images.SetKeyName(13, "");
            this.varImageList.Images.SetKeyName(14, "");
            this.varImageList.Images.SetKeyName(15, "");
            this.varImageList.Images.SetKeyName(16, "");
            this.varImageList.Images.SetKeyName(17, "DocPDF.gif");
            this.varImageList.Images.SetKeyName(18, "DocMainPDF.gif");
            this.varImageList.Images.SetKeyName(19, "DocPDFOrig.gif");
            this.varImageList.Images.SetKeyName(20, "DocMainPDFOrig.gif");
            // 
            // radioButtonBefore
            // 
            resources.ApplyResources(this.radioButtonBefore, "radioButtonBefore");
            this.radioButtonBefore.Name = "radioButtonBefore";
            this.radioButtonBefore.UseVisualStyleBackColor = true;
            this.radioButtonBefore.CheckedChanged += new System.EventHandler(this.radioButtonBefore_CheckedChanged);
            // 
            // radioButtonBetween
            // 
            resources.ApplyResources(this.radioButtonBetween, "radioButtonBetween");
            this.radioButtonBetween.Name = "radioButtonBetween";
            this.radioButtonBetween.UseVisualStyleBackColor = true;
            this.radioButtonBetween.CheckedChanged += new System.EventHandler(this.radioButtonBetween_CheckedChanged);
            // 
            // radioButtonAfter
            // 
            resources.ApplyResources(this.radioButtonAfter, "radioButtonAfter");
            this.radioButtonAfter.Checked = true;
            this.radioButtonAfter.Name = "radioButtonAfter";
            this.radioButtonAfter.TabStop = true;
            this.radioButtonAfter.UseVisualStyleBackColor = true;
            this.radioButtonAfter.CheckedChanged += new System.EventHandler(this.radioButtonAfter_CheckedChanged);
            // 
            // numericUpDown1
            // 
            resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // numericUpDown2
            // 
            resources.ApplyResources(this.numericUpDown2, "numericUpDown2");
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxCho
            // 
            this.textBoxCho.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxCho.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCho.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.textBoxCho, "textBoxCho");
            this.textBoxCho.Name = "textBoxCho";
            // 
            // docControl1
            // 
            resources.ApplyResources(this.docControl1, "docControl1");
            this.docControl1.AnnotationDraw = false;
            this.docControl1.CurDocString = null;
            this.docControl1.DocumentID = 0;
            this.docControl1.EmpName = null;
            this.docControl1.ForceRelicate = false;
            this.docControl1.ImageID = -1;
            this.docControl1.ImagesPanelOrientation = Kesco.Lib.Win.ImageControl.ImageControl.TypeThumbnailPanelOrientation.Top;
            this.docControl1.IsEditNotes = false;
            this.docControl1.IsMain = false;
            this.docControl1.IsMoveImage = true;
            this.docControl1.Name = "docControl1";
            this.docControl1.Page = 0;
            this.docControl1.PersonParamStr = "clid=4&return=1";
            this.docControl1.SelectionMode = false;
            this.docControl1.ShowThumbPanel = true;
            this.docControl1.ShowToolBar = true;
            this.docControl1.ShowWebPanel = false;
            this.docControl1.SplinterPlace = new System.Drawing.Point(180, 50);
            this.docControl1.Subscribe = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.docControl1.WatchOnFile = false;
            this.docControl1.Zoom = 100;
            this.docControl1.ZoomText = "";
            this.docControl1.PageChanged += new System.EventHandler(this.docControl1_PageChanged);
            this.docControl1.LoadComplete += new System.EventHandler(this.docControl1_LoadComplete);
            // 
            // SelectImageDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.docControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.radioButtonAfter);
            this.Controls.Add(this.radioButtonBetween);
            this.Controls.Add(this.radioButtonBefore);
            this.Controls.Add(this.listViewImages);
            this.Controls.Add(this.textBoxCho);
            this.Name = "SelectImageDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Closed += new System.EventHandler(this.SelectImageDialog_Closed);
            this.Load += new System.EventHandler(this.SelectImageDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listViewImages;
        private System.Windows.Forms.RadioButton radioButtonBefore;
        private System.Windows.Forms.RadioButton radioButtonBetween;
        private System.Windows.Forms.RadioButton radioButtonAfter;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ImageList varImageList;
		private System.Windows.Forms.TextBox textBoxCho;
		private Controls.DocControl docControl1;
	}
}