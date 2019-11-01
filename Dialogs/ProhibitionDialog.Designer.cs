using Kesco.Lib.Win.Document.Controls;

namespace Kesco.Lib.Win.Document.Dialogs
{
    partial class ProhibitionDialog
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
			this.button1 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.newWindowDocumentButton = new Kesco.Lib.Win.Document.Controls.NewWindowDocumentButton(this.components);
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(145, 50);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(82, 22);
			this.button1.TabIndex = 0;
			this.button1.Text = "Ok";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(30, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "label1";
			// 
			// newWindowDocumentButton
			// 
			this.newWindowDocumentButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.newWindowDocumentButton.Enabled = false;
			this.newWindowDocumentButton.ImageIndex = 0;
			this.newWindowDocumentButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.newWindowDocumentButton.Location = new System.Drawing.Point(11, 47);
			this.newWindowDocumentButton.Name = "newWindowDocumentButton";
			this.newWindowDocumentButton.Size = new System.Drawing.Size(25, 25);
			this.newWindowDocumentButton.TabIndex = 3;
			this.newWindowDocumentButton.TabStop = false;
			this.newWindowDocumentButton.UseMnemonic = false;
			this.newWindowDocumentButton.UseVisualStyleBackColor = true;
			this.newWindowDocumentButton.Visible = false;
			this.newWindowDocumentButton.Click += new System.EventHandler(this.newWindowDocumentButton_Click);
			// 
			// ProhibitionDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button1;
			this.ClientSize = new System.Drawing.Size(373, 85);
			this.Controls.Add(this.newWindowDocumentButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProhibitionDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ProhibitionDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private NewWindowDocumentButton newWindowDocumentButton;
    }
}