using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Controls.PdfViewControl
{
	partial class PDFView
	{
		/// <summary>
		/// Требуется переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.PagePreview != null)
				{
					this.PagePreview.SelectedIndexChanged -= PagePreviewSelectedIndexChanged;
					this.PagePreview.Items.Clear();
					if (InvokeRequired)
						Invoke((MethodInvoker)(PagePreview.Dispose));
					else
						PagePreview.Dispose();
					this.PagePreview = null;
				}

				if (PagePreviewImages != null)
				{
					PagePreviewImages.Images.Clear();
					if (InvokeRequired)
						Invoke((MethodInvoker)(PagePreviewImages.Dispose));
					else
						PagePreviewImages.Dispose();
					PagePreviewImages = null;
				}

				if (this.PageView != null)
				{
					Subscribe_PageView_ClientSizeChanged(false);
					if (InvokeRequired)
						Invoke((MethodInvoker)(PageView.Dispose));
					else
						PageView.Dispose();
					this.PageView = null;
				}

				if (DocPages != null)
					DocPages.Clear();
			}

			System.GC.Collect();

			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором компонентов

		/// <summary>
		/// Обязательный метод для поддержки конструктора - не изменяйте 
		/// содержимое данного метода при помощи редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PDFView));
            this.Splitter = new System.Windows.Forms.SplitContainer();
            this.PagePreviewImages = new System.Windows.Forms.ImageList(this.components);
            this.buttonLoad = new System.Windows.Forms.Button();
            this.labelNoImage = new System.Windows.Forms.Label();
			this.PagePreview = new Kesco.Lib.Win.Document.Controls.PdfViewControl.ListViewEx();
            this.Pages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PageView = new PicturePanel();
            this.Splitter.Panel1.SuspendLayout();
            this.Splitter.Panel2.SuspendLayout();
            this.Splitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // Splitter
            // 
            resources.ApplyResources(this.Splitter, "Splitter");
            this.Splitter.Name = "Splitter";
            // 
            // Splitter.Panel1
            // 
            this.Splitter.Panel1.Controls.Add(this.PagePreview);
            // 
            // Splitter.Panel2
            // 
            this.Splitter.Panel2.Controls.Add(this.PageView);
            this.Splitter.TabStop = false;
            // 
            // PagePreviewImages
            // 
            this.PagePreviewImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            resources.ApplyResources(this.PagePreviewImages, "PagePreviewImages");
            this.PagePreviewImages.TransparentColor = System.Drawing.Color.Transparent;
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
            // PagePreview
            // 
            this.PagePreview.BackColor = System.Drawing.SystemColors.Window;
            this.PagePreview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Pages});
            resources.ApplyResources(this.PagePreview, "PagePreview");
            this.PagePreview.LargeImageList = this.PagePreviewImages;
            this.PagePreview.MultiSelect = false;
            this.PagePreview.Name = "PagePreview";
            this.PagePreview.OwnerDraw = true;
            this.PagePreview.UseCompatibleStateImageBehavior = false;
            // 
            // Pages
            // 
            resources.ApplyResources(this.Pages, "Pages");
            // 
            // PageView
            // 
            resources.ApplyResources(this.PageView, "PageView");
            this.PageView.BackColor = System.Drawing.SystemColors.Control;
            this.PageView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PageView.CurrentDocID = 0;
            this.PageView.CurrentPixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            this.PageView.CurrentStamp = null;
            this.PageView.CurrentStampID = 0;
            this.PageView.Image = null;
            this.PageView.Name = "PageView";
            this.PageView.ScrollHorizontal = 0;
            this.PageView.ScrollVertical = 0;
            this.PageView.WorkMode = PicturePanel.WorkingMode.None;
            this.PageView.Zoom = 100D;
            // 
            // PDFView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelNoImage);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.Splitter);
            this.DoubleBuffered = true;
            this.Name = "PDFView";
            this.Splitter.Panel1.ResumeLayout(false);
            this.Splitter.Panel2.ResumeLayout(false);
            this.Splitter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer Splitter;
		private ListViewEx PagePreview;
		private PicturePanel PageView;
		private System.Windows.Forms.ImageList PagePreviewImages;
		private System.Windows.Forms.ColumnHeader Pages;
		private System.Windows.Forms.Button buttonLoad;
		private System.Windows.Forms.Label labelNoImage;
	}
}
