using System;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class NumericInterval : Base
    {
        private Label label1;
        private RadioButton rb3;
        private RadioButton rb0;
        private Label label4;
        private RadioButton rb1;
        private RadioButton rb2;
        private TextBox tbMin0;
        private TextBox tbMax1;
        private TextBox tbMin2;
        private TextBox tbMin3;
        private TextBox tbMax3;

		private Type searchType;

        private Container components;

        public NumericInterval()
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
                new System.ComponentModel.ComponentResourceManager(typeof (NumericInterval));
            this.label1 = new System.Windows.Forms.Label();
            this.rb3 = new System.Windows.Forms.RadioButton();
            this.rb1 = new System.Windows.Forms.RadioButton();
            this.rb2 = new System.Windows.Forms.RadioButton();
            this.rb0 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.tbMin0 = new System.Windows.Forms.TextBox();
            this.tbMax1 = new System.Windows.Forms.TextBox();
            this.tbMin2 = new System.Windows.Forms.TextBox();
            this.tbMin3 = new System.Windows.Forms.TextBox();
            this.tbMax3 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // rb3
            // 
            resources.ApplyResources(this.rb3, "rb3");
            this.rb3.Name = "rb3";
            this.rb3.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rb1
            // 
            resources.ApplyResources(this.rb1, "rb1");
            this.rb1.Name = "rb1";
            this.rb1.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rb2
            // 
            resources.ApplyResources(this.rb2, "rb2");
            this.rb2.Name = "rb2";
            this.rb2.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rb0
            // 
            resources.ApplyResources(this.rb0, "rb0");
            this.rb0.Name = "rb0";
            this.rb0.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // label4
            // 
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tbMin0
            // 
            this.tbMin0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.tbMin0, "tbMin0");
            this.tbMin0.Name = "tbMin0";
            this.tbMin0.TextChanged += new System.EventHandler(this.tb_TextChanged);
            // 
            // tbMax1
            // 
            this.tbMax1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.tbMax1, "tbMax1");
            this.tbMax1.Name = "tbMax1";
            this.tbMax1.TextChanged += new System.EventHandler(this.tb_TextChanged);
            // 
            // tbMin2
            // 
            this.tbMin2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.tbMin2, "tbMin2");
            this.tbMin2.Name = "tbMin2";
            this.tbMin2.TextChanged += new System.EventHandler(this.tb_TextChanged);
            // 
            // tbMin3
            // 
            this.tbMin3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.tbMin3, "tbMin3");
            this.tbMin3.Name = "tbMin3";
            this.tbMin3.TextChanged += new System.EventHandler(this.tb_TextChanged);
            // 
            // tbMax3
            // 
            this.tbMax3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.tbMax3, "tbMax3");
            this.tbMax3.Name = "tbMax3";
            this.tbMax3.TextChanged += new System.EventHandler(this.tb_TextChanged);
            // 
            // DecimalInterval
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tbMax3);
            this.Controls.Add(this.tbMin3);
            this.Controls.Add(this.tbMin2);
            this.Controls.Add(this.tbMax1);
            this.Controls.Add(this.tbMin0);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rb0);
            this.Controls.Add(this.rb2);
            this.Controls.Add(this.rb1);
            this.Controls.Add(this.rb3);
            this.Name = "DecimalInterval";

            this.Controls.SetChildIndex(this.rb3, 0);
            this.Controls.SetChildIndex(this.rb1, 0);
            this.Controls.SetChildIndex(this.rb2, 0);
            this.Controls.SetChildIndex(this.rb0, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.tbMin0, 0);
            this.Controls.SetChildIndex(this.tbMax1, 0);
            this.Controls.SetChildIndex(this.tbMin2, 0);
            this.Controls.SetChildIndex(this.tbMin3, 0);
            this.Controls.SetChildIndex(this.tbMax3, 0);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        protected override void FillForm()
        {
			if(option is MinMaxOption)
				searchType = ((MinMaxOption)option).GetSearchType();
			//searchType = option.Meta.GetAttribute(elOption.Name + "Type"
            string min = elOption.GetAttribute("min");
            string max = elOption.GetAttribute("max");

            int flags = (min.Length > 0 ? 0x0001 : 0) +
                        (max.Length > 0 ? 0x0010 : 0) +
                        (min.Equals(max) ? 0x0100 : 0);
            if (min.Length > 0)
                tbMin3.Text =
                    tbMin2.Text =
                    tbMin0.Text = min;
            if (max.Length > 0)
                tbMax3.Text =
                    tbMax1.Text = max;
            switch (flags)
            {
                case 0x0001:
                    rb2.Checked = true;
                    break;
                case 0x0010:
                    rb1.Checked = true;
                    break;
                case 0x0011:
                    rb3.Checked = true;
                    break;
                default:
                    rb0.Checked = true;
                    break;
            }
        }

        protected override void FillElement()
        {
            if (rb0.Checked)
            {
                elOption.SetAttribute("min", tbMin0.Text.Replace(",", ".").Trim());
                elOption.SetAttribute("max", tbMin0.Text.Replace(",", ".").Trim());
            }
            if (rb1.Checked)
            {
                elOption.RemoveAttribute("min");
                elOption.SetAttribute("max", tbMax1.Text.Replace(",", ".").Trim());
            }
            if (rb2.Checked)
            {
                elOption.SetAttribute("min", tbMin2.Text.Replace(",", ".").Trim());
                elOption.RemoveAttribute("max");
            }
            if (rb3.Checked)
            {
                elOption.SetAttribute("min", tbMin3.Text.Replace(",", ".").Trim());
                elOption.SetAttribute("max", tbMax3.Text.Replace(",", ".").Trim());
            }
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            tbMin0.Enabled = rb0.Checked;
            tbMax1.Enabled = rb1.Checked;
            tbMin2.Enabled = rb2.Checked;
            tbMax3.Enabled = tbMin3.Enabled = rb3.Checked;
        }

		private void tb_TextChanged(object sender, EventArgs e)
		{
			var tb = sender as TextBox;
			if(tb == null)
				return;
			decimal res = 0;
			int reti = 0;
			if(searchType != null)
				if(res.GetType() == searchType)
				{
					if(decimal.TryParse(tb.Text.Replace(".", ",").Replace(" ", "").Trim(), out res))
					{
						tb.Text = res.ToString().Replace(",", ".") +
								  (tb.Text.Replace(".", ",").Trim().EndsWith(",") ? "." : "");
						tb.Select(tb.Text.Length, 0);
						return;
					}
				}
				else
					if(searchType == reti.GetType())
					{
						string txt = System.Text.RegularExpressions.Regex.Replace(tb.Text,"\\D","");
						if(int.TryParse(txt, out reti))
						{
							tb.Text = reti.ToString();
							tb.Select(tb.Text.Length, 0);
							return;
						}
					}
			tb.Text = "";
		}
    }
}