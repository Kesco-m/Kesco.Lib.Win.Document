using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
    partial class CheckTypeDialogCountry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckTypeDialogCountry));
            this.pbQuestion = new System.Windows.Forms.PictureBox();
            this.lblConfirmationDescription = new System.Windows.Forms.Label();
            this.lblConfirmationQuestion = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbQuestion)).BeginInit();
            this.SuspendLayout();
            // 
            // pbQuestion
            // 
            this.pbQuestion.BackColor = System.Drawing.SystemColors.Control;
            this.pbQuestion.Image = ((System.Drawing.Image)(resources.GetObject("pbQuestion.Image")));
            this.pbQuestion.Location = new System.Drawing.Point(16, 27);
            this.pbQuestion.Name = "pbQuestion";
            this.pbQuestion.Size = new System.Drawing.Size(32, 32);
            this.pbQuestion.TabIndex = 0;
            this.pbQuestion.TabStop = false;
            // 
            // lblConfirmationDescription
            // 
            resources.ApplyResources(this.lblConfirmationDescription, "lblConfirmationDescription");
            this.lblConfirmationDescription.AutoSize = true;
            this.lblConfirmationDescription.Location = new System.Drawing.Point(63, 9);
            this.lblConfirmationDescription.Name = "lblConfirmationDescription";
            this.lblConfirmationDescription.Size = new System.Drawing.Size(0, 13);
            this.lblConfirmationDescription.TabIndex = 1;
            // 
            // lblConfirmationQuestion
            // 
            resources.ApplyResources(this.lblConfirmationQuestion, "lblConfirmationQuestion");
            this.lblConfirmationQuestion.AutoSize = true;
            this.lblConfirmationQuestion.Location = new System.Drawing.Point(63, 31);
            this.lblConfirmationQuestion.Name = "lblConfirmationQuestion";
            this.lblConfirmationQuestion.Size = new System.Drawing.Size(0, 13);
            this.lblConfirmationQuestion.TabIndex = 2;
            // 
            // btnYes
            // 
            resources.ApplyResources(this.btnYes, "btnYes");
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnYes.Location = new System.Drawing.Point(152, 55);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(75, 23);
            this.btnYes.TabIndex = 3;
            this.btnYes.UseVisualStyleBackColor = true;
            // 
            // btnNo
            // 
            resources.ApplyResources(this.btnNo, "btnNo");
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.Location = new System.Drawing.Point(233, 55);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(75, 23);
            this.btnNo.TabIndex = 4;
            this.btnNo.UseVisualStyleBackColor = true;
            // 
            // CheckTypeDialogCountry
            // 
            this.AcceptButton = this.btnYes;
            resources.ApplyResources(this, "$this");
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnNo;
            this.ClientSize = new System.Drawing.Size(460, 86);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.lblConfirmationQuestion);
            this.Controls.Add(this.lblConfirmationDescription);
            this.Controls.Add(this.pbQuestion);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckTypeDialogCountry";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.pbQuestion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbQuestion;
        private System.Windows.Forms.Label lblConfirmationDescription;
        private System.Windows.Forms.Label lblConfirmationQuestion;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
    }
}