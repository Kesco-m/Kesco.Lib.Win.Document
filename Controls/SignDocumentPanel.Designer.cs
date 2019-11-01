namespace Kesco.Lib.Win.Document.Controls
{
	partial class SignDocumentPanel
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
			MouseEventMessageFilter.Instance.MouseMove -= filter_MouseMove;
			MouseEventMessageFilter.Instance.MouseLeave -= filter_MouseLeave;

			if(disposing && (components != null))
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignDocumentPanel));
			this.llFinalSign = new System.Windows.Forms.LinkLabel();
			this.llSign = new System.Windows.Forms.LinkLabel();
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.lLCancel = new System.Windows.Forms.LinkLabel();
			this.ttDropDown = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// llFinalSign
			// 
			resources.ApplyResources(this.llFinalSign, "llFinalSign");
			this.llFinalSign.LinkColor = System.Drawing.Color.CornflowerBlue;
			this.llFinalSign.Name = "llFinalSign";
			this.llFinalSign.TabStop = true;
			this.llFinalSign.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llFinalSign_LinkClicked);
			this.llFinalSign.VisibleChanged += new System.EventHandler(this.ll_VisibleChanged);
			// 
			// llSign
			// 
			resources.ApplyResources(this.llSign, "llSign");
			this.llSign.Name = "llSign";
			this.llSign.TabStop = true;
			this.llSign.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llSign_LinkClicked);
			this.llSign.VisibleChanged += new System.EventHandler(this.ll_VisibleChanged);
			// 
			// tableLayoutPanel
			// 
			resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
			this.tableLayoutPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.SizeChanged += new System.EventHandler(this.tableLayoutPanel_SizeChanged);
			this.tableLayoutPanel.VisibleChanged += new System.EventHandler(this.tableLayoutPanel_VisibleChanged);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.SystemColors.Window;
			this.imageList.Images.SetKeyName(0, "Remove.bmp");
			// 
			// lLCancel
			// 
			resources.ApplyResources(this.lLCancel, "lLCancel");
			this.lLCancel.LinkColor = System.Drawing.Color.CornflowerBlue;
			this.lLCancel.Name = "lLCancel";
			this.lLCancel.TabStop = true;
			this.lLCancel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lLCancel_LinkClicked);
			this.lLCancel.VisibleChanged += new System.EventHandler(this.ll_VisibleChanged);
			// 
			// SignDocumentPanel
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Controls.Add(this.tableLayoutPanel);
			this.Controls.Add(this.llSign);
			this.Controls.Add(this.llFinalSign);
			this.Controls.Add(this.lLCancel);
			this.DoubleBuffered = true;
			this.Name = "SignDocumentPanel";
			this.ClientSizeChanged += new System.EventHandler(this.SignDocumentPanel_ClientSizeChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.LinkLabel llFinalSign;
        private System.Windows.Forms.LinkLabel llSign;
		private System.Windows.Forms.LinkLabel lLCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolTip ttDropDown;
	}
}
