using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class Text : Base
    {
        public RadioButton �����������;
        public RadioButton ���������;
        public TextBox ������;

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
            this.����������� = new System.Windows.Forms.RadioButton();
            this.��������� = new System.Windows.Forms.RadioButton();
            this.������ = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // �����������
            // 
            resources.ApplyResources(this.�����������, "�����������");
            this.�����������.Name = "�����������";
            // 
            // ���������
            // 
            resources.ApplyResources(this.���������, "���������");
            this.���������.Name = "���������";
            // 
            // ������
            // 
            resources.ApplyResources(this.������, "������");
            this.������.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.������.Name = "������";
            // 
            // Text
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.������);
            this.Controls.Add(this.���������);
            this.Controls.Add(this.�����������);
            this.Name = "Text";
            this.Load += new System.EventHandler(this.Text_Load);
            this.Controls.SetChildIndex(this.�����������, 0);
            this.Controls.SetChildIndex(this.���������, 0);
            this.Controls.SetChildIndex(this.������, 0);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        protected override void FillElement()
        {
            elOption.SetAttribute("value", ������.Text);
            elOption.SetAttribute("mode", (���������.Checked ? "inText" : "startsWith"));
        }

        protected override void FillForm()
        {
            ������.Text = elOption.GetAttribute("value");

            switch (elOption.GetAttribute("mode").ToLower())
            {
                case "intext":
                    ���������.Checked = true;
                    break;
                default:
                    �����������.Checked = true;
                    break;
            }
        }

        private void Text_Load(object sender, EventArgs e)
        {
        }
    }
}