using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class Номер : Base
    {
        public TextBox пТекст;
        private CheckBox checkBoxNoNumber;
        private Label label;

        private void InitializeComponent()
        {
            var resources =
                new ComponentResourceManager(typeof (Номер));
            пТекст = new TextBox();
            label = new Label();
            checkBoxNoNumber = new CheckBox();
            SuspendLayout();
            // 
            // пТекст
            // 
            пТекст.AccessibleDescription = null;
            пТекст.AccessibleName = null;
            resources.ApplyResources(пТекст, "пТекст");
            пТекст.BackgroundImage = null;
            пТекст.Font = null;
            пТекст.Name = "пТекст";
            // 
            // label
            // 
            label.AccessibleDescription = null;
            label.AccessibleName = null;
            resources.ApplyResources(label, "label");
            label.Font = null;
            label.Name = "label";
            // 
            // checkBoxNoNumber
            // 
            checkBoxNoNumber.AccessibleDescription = null;
            checkBoxNoNumber.AccessibleName = null;
            resources.ApplyResources(checkBoxNoNumber, "checkBoxNoNumber");
            checkBoxNoNumber.BackgroundImage = null;
            checkBoxNoNumber.Font = null;
            checkBoxNoNumber.Name = "checkBoxNoNumber";
            checkBoxNoNumber.UseVisualStyleBackColor = true;
            checkBoxNoNumber.CheckedChanged += checkBoxNoNumber_CheckedChanged;
            // 
            // Номер
            // 
            AccessibleDescription = null;
            AccessibleName = null;
            resources.ApplyResources(this, "$this");
            BackgroundImage = null;
            Controls.Add(checkBoxNoNumber);
            Controls.Add(label);
            Controls.Add(пТекст);
            Font = null;
            Icon = null;
            Name = "Номер";
            Controls.SetChildIndex(пТекст, 0);
            Controls.SetChildIndex(label, 0);
            Controls.SetChildIndex(checkBoxNoNumber, 0);
            ResumeLayout(false);
            PerformLayout();
        }

        public Номер()
        {
            InitializeComponent();
        }

        protected override void FillElement()
        {
            elOption.SetAttribute("value", пТекст.Text);
            elOption.SetAttribute("mode", checkBoxNoNumber.Checked ? "No" : "");
        }

        protected override void FillForm()
        {
            пТекст.Text = elOption.GetAttribute("value");
            checkBoxNoNumber.Checked = elOption.GetAttribute("mode").ToLower().Equals("no");
            пТекст.Enabled = !checkBoxNoNumber.Checked;
        }

        private void checkBoxNoNumber_CheckedChanged(object sender, EventArgs e)
        {
            пТекст.Enabled = !checkBoxNoNumber.Checked;
        }
    }
}