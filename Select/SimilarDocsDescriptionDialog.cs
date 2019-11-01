using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Select
{
    public class SimilarDocsDescriptionDialog : FreeDialog
    {
        private Label label;
        private Button buttonCancel;
        private RadioButton rb1;
        private RadioButton rb2;
        private RadioButton rb3;
        private RadioButton rb4;
        private Button buttonOk;

        private string description = string.Empty;

        /// <summary>
        ///   Required designer variable.
        /// </summary>
        private Container components;

        public SimilarDocsDescriptionDialog(string firstDescr, string secondDescr)
        {
            InitializeComponent();

            rb1.Text = firstDescr;
            rb2.Text = secondDescr;
            rb3.Text = string.Concat(firstDescr, " ", secondDescr);
            rb4.Text = string.Concat(secondDescr, " ", firstDescr);

            rb2.Top = rb1.Top + rb1.Height + 8;
            rb3.Top = rb2.Top + rb2.Height + 8;
            rb4.Top = rb3.Top + rb3.Height + 8;

            rb1.Checked = true;

            Height = 16 + rb4.Top + rb4.Height + 16 + buttonOk.Height + 16;
        }

        public string Description
        {
            get { return description; }
        }

        /// <summary>
        ///   Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (SimilarDocsDescriptionDialog));
            this.label = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.rb1 = new System.Windows.Forms.RadioButton();
            this.rb2 = new System.Windows.Forms.RadioButton();
            this.rb3 = new System.Windows.Forms.RadioButton();
            this.rb4 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label
            // 
            resources.ApplyResources(this.label, "label");
            this.label.Name = "label";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // rb1
            // 
            this.rb1.AutoEllipsis = true;
            resources.ApplyResources(this.rb1, "rb1");
            this.rb1.Name = "rb1";
            this.rb1.TabStop = true;
            this.rb1.UseVisualStyleBackColor = true;
            this.rb1.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rb2
            // 
            this.rb2.AutoEllipsis = true;
            resources.ApplyResources(this.rb2, "rb2");
            this.rb2.Name = "rb2";
            this.rb2.TabStop = true;
            this.rb2.UseVisualStyleBackColor = true;
            this.rb2.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rb3
            // 
            this.rb3.AutoEllipsis = true;
            resources.ApplyResources(this.rb3, "rb3");
            this.rb3.Name = "rb3";
            this.rb3.TabStop = true;
            this.rb3.UseVisualStyleBackColor = true;
            this.rb3.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rb4
            // 
            this.rb4.AutoEllipsis = true;
            resources.ApplyResources(this.rb4, "rb4");
            this.rb4.Name = "rb4";
            this.rb4.TabStop = true;
            this.rb4.UseVisualStyleBackColor = true;
            this.rb4.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // SimilarDocsDescriptionDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.ControlBox = false;
            this.Controls.Add(this.rb4);
            this.Controls.Add(this.rb3);
            this.Controls.Add(this.rb2);
            this.Controls.Add(this.rb1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label);
            this.MaximizeBox = false;
            this.Name = "SimilarDocsDescriptionDialog";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private void buttonOk_Click(object sender, EventArgs e)
        {
            End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            buttonOk.Enabled = rb1.Checked || rb2.Checked || rb3.Checked || rb4.Checked;

            description = buttonOk.Enabled ? ((RadioButton) sender).Text : string.Empty;
        }
    }
}