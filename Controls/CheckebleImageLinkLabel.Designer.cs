
namespace Kesco.Lib.Win.Document.Controls
{
	partial class CheckebleImageLinkLabel
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором компонентов

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBoxImage = new System.Windows.Forms.PictureBox();
			this.linkLabelText = new System.Windows.Forms.LinkLabel();
			this.pictureBoxClose = new System.Windows.Forms.PictureBox();
			this.checkBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxClose)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxImage
			// 
			this.pictureBoxImage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBoxImage.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxImage.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
			this.pictureBoxImage.Name = "pictureBoxImage";
			this.pictureBoxImage.Size = new System.Drawing.Size(20, 13);
			this.pictureBoxImage.TabIndex = 0;
			this.pictureBoxImage.TabStop = false;
			// 
			// linkLabelText
			// 
			this.linkLabelText.AutoSize = true;
			this.linkLabelText.Dock = System.Windows.Forms.DockStyle.Fill;
			this.linkLabelText.Location = new System.Drawing.Point(23, 0);
			this.linkLabelText.Name = "linkLabelText";
			this.linkLabelText.Size = new System.Drawing.Size(55, 13);
			this.linkLabelText.TabIndex = 0;
			this.linkLabelText.TabStop = true;
			this.linkLabelText.Text = "linkLabel1";
			this.linkLabelText.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelText_LinkClicked);
			// 
			// pictureBoxClose
			// 
			this.pictureBoxClose.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBoxClose.Location = new System.Drawing.Point(81, 0);
			this.pictureBoxClose.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
			this.pictureBoxClose.Name = "pictureBoxClose";
			this.pictureBoxClose.Size = new System.Drawing.Size(20, 13);
			this.pictureBoxClose.TabIndex = 0;
			this.pictureBoxClose.TabStop = false;
			this.pictureBoxClose.Click += new System.EventHandler(this.pictureBoxClose_Click);
			// 
			// checkBox
			// 
			this.checkBox.AutoSize = true;
			this.checkBox.Location = new System.Drawing.Point(0, 0);
			this.checkBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
			this.checkBox.Name = "checkBox";
			this.checkBox.Size = new System.Drawing.Size(24, 24);
			this.checkBox.TabIndex = 0;
			this.checkBox.UseVisualStyleBackColor = true;
			// 
			// ImageLinkLabel
			// 
			this.AutoSize = true;
			this.Controls.Add(this.checkBox);
			this.Controls.Add(this.pictureBoxImage);
			this.Controls.Add(this.linkLabelText);
			this.Controls.Add(this.pictureBoxClose);
			this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
			this.Size = new System.Drawing.Size(200, 24);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxClose)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel linkLabelText;
		private System.Windows.Forms.PictureBox pictureBoxImage;
		private System.Windows.Forms.PictureBox pictureBoxClose;
		private System.Windows.Forms.CheckBox checkBox;
	}
}
