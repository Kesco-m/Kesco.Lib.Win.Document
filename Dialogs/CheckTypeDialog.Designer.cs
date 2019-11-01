namespace Kesco.Lib.Win.Document.Dialogs
{
	partial class CheckTypeDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckTypeDialog));
			this.labelText = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// labelText
			// 
			resources.ApplyResources(this.labelText, "labelText");
			this.labelText.Name = "labelText";
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
		    this.button1.AutoSize = true;
		    this.button1.Location = new System.Drawing.Point(147, 35);
		    this.button1.Size = new System.Drawing.Size(111, 23);
		    this.button1.TabIndex = 2;
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.button2.DialogResult = System.Windows.Forms.DialogResult.No;
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
		    this.button2.AutoSize = true;
		    this.button2.Location = new System.Drawing.Point(262, 35);
		    this.button2.Size = new System.Drawing.Size(130, 23);
		    this.button2.TabIndex = 1;
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// CheckTypeDialog
			// 
			this.AcceptButton = this.button2;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.CancelButton = this.button2;
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.labelText);
			this.DoubleBuffered = true;
		    this.ClientSize = new System.Drawing.Size(510, 70);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CheckTypeDialog";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelText;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}