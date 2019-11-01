using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class TextList : TemplateList
    {
        private string selectKey;

        private TextBox tbText;
        private Label label1;
        private Button bAdd;

        private Container components;

        public TextList()
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
                new System.ComponentModel.ComponentResourceManager(typeof (TextList));
            this.tbText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bAdd = new System.Windows.Forms.Button();
            this.panelList.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelList
            // 
            this.panelList.AccessibleDescription = null;
            this.panelList.AccessibleName = null;
            resources.ApplyResources(this.panelList, "panelList");
            this.panelList.BackgroundImage = null;
            this.panelList.Font = null;
            // 
            // lb
            // 
            this.lb.AccessibleDescription = null;
            this.lb.AccessibleName = null;
            resources.ApplyResources(this.lb, "lb");
            this.lb.BackgroundImage = null;
            this.lb.Font = null;
            this.lb.SelectedIndexChanged += new System.EventHandler(this.lb_SelectedIndexChanged);
            // 
            // tbText
            // 
            this.tbText.AccessibleDescription = null;
            this.tbText.AccessibleName = null;
            resources.ApplyResources(this.tbText, "tbText");
            this.tbText.BackgroundImage = null;
            this.tbText.Font = null;
            this.tbText.Name = "tbText";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // bAdd
            // 
            this.bAdd.AccessibleDescription = null;
            this.bAdd.AccessibleName = null;
            resources.ApplyResources(this.bAdd, "bAdd");
            this.bAdd.BackgroundImage = null;
            this.bAdd.Font = null;
            this.bAdd.Name = "bAdd";
            this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
            // 
            // TextList
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.bAdd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbText);
            this.Font = null;
            this.Icon = null;
            this.Name = "TextList";
            this.Controls.SetChildIndex(this.panelList, 0);
            this.Controls.SetChildIndex(this.tbText, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.bAdd, 0);
            this.panelList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private void bAdd_Click(object sender, EventArgs e)
        {
            string s = Regex.Replace(tbText.Text.Replace(',', ' ').Trim(), "[ ]{1,}", " ");
            if (s.Length != 0 && s != selectKey)
            {
                if (selectKey != null)
                    RemoveKey(selectKey);
                AddKey(s);
                tbText.Text = "";
            }
            tbText.Focus();
        }

        public override void RemoveKey(string key)
        {
            base.RemoveKey(key);
            selectKey = null;
        }

        public override void RemoveAllKeys()
        {
            base.RemoveAllKeys();
            selectKey = null;
        }

        protected override void FillElement()
        {
            base.FillElement();
            var o = (ListOption) option;
            string[] S = o.GetValues(false);
            if (S.Length == 0)
            {
                string s = Regex.Replace(tbText.Text.Replace(',', ' ').Trim(), "[ ]{1,}", " ");
                if (s.Length != 0)
                    o.SetValue(s);
            }
        }

        private void lb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lb.SelectedItems.Count > 0)
                tbText.Text = selectKey = keys[lb.SelectedIndex] as string;
            else
                tbText.Text = selectKey = null;
        }
    }
}