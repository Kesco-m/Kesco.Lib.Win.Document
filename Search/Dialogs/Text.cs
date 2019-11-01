using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class Text : Base
    {
        public RadioButton пЌачинаетс€;
        public RadioButton п—одержит;
        public TextBox п“екст;

        private Container components;

        public Text()
        {
            InitializeComponent();
        }

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
                new System.ComponentModel.ComponentResourceManager(typeof (Text));
            this.пЌачинаетс€ = new System.Windows.Forms.RadioButton();
            this.п—одержит = new System.Windows.Forms.RadioButton();
            this.п“екст = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // пЌачинаетс€
            // 
            resources.ApplyResources(this.пЌачинаетс€, "пЌачинаетс€");
            this.пЌачинаетс€.Name = "пЌачинаетс€";
            // 
            // п—одержит
            // 
            resources.ApplyResources(this.п—одержит, "п—одержит");
            this.п—одержит.Name = "п—одержит";
            // 
            // п“екст
            // 
            resources.ApplyResources(this.п“екст, "п“екст");
            this.п“екст.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.п“екст.Name = "п“екст";
            // 
            // Text
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.п“екст);
            this.Controls.Add(this.п—одержит);
            this.Controls.Add(this.пЌачинаетс€);
            this.Name = "Text";
            this.Load += new System.EventHandler(this.Text_Load);
            this.Controls.SetChildIndex(this.пЌачинаетс€, 0);
            this.Controls.SetChildIndex(this.п—одержит, 0);
            this.Controls.SetChildIndex(this.п“екст, 0);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        protected override void FillElement()
        {
            elOption.SetAttribute("value", п“екст.Text);
            elOption.SetAttribute("mode", (п—одержит.Checked ? "inText" : "startsWith"));
        }

        protected override void FillForm()
        {
            п“екст.Text = elOption.GetAttribute("value");

            switch (elOption.GetAttribute("mode").ToLower())
            {
                case "intext":
                    п—одержит.Checked = true;
                    break;
                default:
                    пЌачинаетс€.Checked = true;
                    break;
            }
        }

        private void Text_Load(object sender, EventArgs e)
        {
        }
    }
}