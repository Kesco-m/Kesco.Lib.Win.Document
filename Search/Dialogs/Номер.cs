using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class ����� : Base
    {
        public TextBox ������;
        private CheckBox checkBoxNoNumber;
        private Label label;

        private void InitializeComponent()
        {
            var resources =
                new ComponentResourceManager(typeof (�����));
            ������ = new TextBox();
            label = new Label();
            checkBoxNoNumber = new CheckBox();
            SuspendLayout();
            // 
            // ������
            // 
            ������.AccessibleDescription = null;
            ������.AccessibleName = null;
            resources.ApplyResources(������, "������");
            ������.BackgroundImage = null;
            ������.Font = null;
            ������.Name = "������";
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
            // �����
            // 
            AccessibleDescription = null;
            AccessibleName = null;
            resources.ApplyResources(this, "$this");
            BackgroundImage = null;
            Controls.Add(checkBoxNoNumber);
            Controls.Add(label);
            Controls.Add(������);
            Font = null;
            Icon = null;
            Name = "�����";
            Controls.SetChildIndex(������, 0);
            Controls.SetChildIndex(label, 0);
            Controls.SetChildIndex(checkBoxNoNumber, 0);
            ResumeLayout(false);
            PerformLayout();
        }

        public �����()
        {
            InitializeComponent();
        }

        protected override void FillElement()
        {
            elOption.SetAttribute("value", ������.Text);
            elOption.SetAttribute("mode", checkBoxNoNumber.Checked ? "No" : "");
        }

        protected override void FillForm()
        {
            ������.Text = elOption.GetAttribute("value");
            checkBoxNoNumber.Checked = elOption.GetAttribute("mode").ToLower().Equals("no");
            ������.Enabled = !checkBoxNoNumber.Checked;
        }

        private void checkBoxNoNumber_CheckedChanged(object sender, EventArgs e)
        {
            ������.Enabled = !checkBoxNoNumber.Checked;
        }
    }
}