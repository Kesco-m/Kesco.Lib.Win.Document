namespace Kesco.Lib.Win.Document.Dialogs
{
    partial class SaveChangesDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveChangesDialog));
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.radioButSave = new System.Windows.Forms.RadioButton();
			this.radioButDontSave = new System.Windows.Forms.RadioButton();
			this.chBoxOpenCopy = new System.Windows.Forms.CheckBox();
			this.readOnlyWarnlabel = new System.Windows.Forms.Label();
			this.radioButActCopy = new System.Windows.Forms.RadioButton();
			this.radioButActOrig = new System.Windows.Forms.RadioButton();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// radioButSave
			// 
			resources.ApplyResources(this.radioButSave, "radioButSave");
			this.radioButSave.Checked = true;
			this.radioButSave.Name = "radioButSave";
			this.radioButSave.TabStop = true;
			this.radioButSave.UseVisualStyleBackColor = true;
			// 
			// radioButDontSave
			// 
			resources.ApplyResources(this.radioButDontSave, "radioButDontSave");
			this.radioButDontSave.Name = "radioButDontSave";
			this.radioButDontSave.UseVisualStyleBackColor = true;
			// 
			// chBoxOpenCopy
			// 
			resources.ApplyResources(this.chBoxOpenCopy, "chBoxOpenCopy");
			this.chBoxOpenCopy.Name = "chBoxOpenCopy";
			this.chBoxOpenCopy.UseVisualStyleBackColor = true;
			// 
			// readOnlyWarnlabel
			// 
			resources.ApplyResources(this.readOnlyWarnlabel, "readOnlyWarnlabel");
			this.readOnlyWarnlabel.Name = "readOnlyWarnlabel";
			// 
			// radioButActCopy
			// 
			resources.ApplyResources(this.radioButActCopy, "radioButActCopy");
			this.radioButActCopy.Name = "radioButActCopy";
			this.radioButActCopy.UseVisualStyleBackColor = true;
			// 
			// radioButActOrig
			// 
			resources.ApplyResources(this.radioButActOrig, "radioButActOrig");
			this.radioButActOrig.Checked = true;
			this.radioButActOrig.Name = "radioButActOrig";
			this.radioButActOrig.TabStop = true;
			this.radioButActOrig.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.radioButActOrig);
			this.panel2.Controls.Add(this.radioButActCopy);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.radioButSave);
			this.panel1.Controls.Add(this.radioButDontSave);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// SaveChangesDialog
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.readOnlyWarnlabel);
			this.Controls.Add(this.chBoxOpenCopy);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SaveChangesDialog";
			this.TopMost = true;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SaveChangesDialog_FormClosed);
			this.Load += new System.EventHandler(this.SaveChangesDialog_Load);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RadioButton radioButSave;
        private System.Windows.Forms.RadioButton radioButDontSave;
        private System.Windows.Forms.CheckBox chBoxOpenCopy;
        private System.Windows.Forms.Label readOnlyWarnlabel;
        private System.Windows.Forms.RadioButton radioButActCopy;
        private System.Windows.Forms.RadioButton radioButActOrig;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
    }
}