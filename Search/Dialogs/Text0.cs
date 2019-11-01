using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class Text0 : Base
    {
        private TextBox tb;
        private Label label1;
        private Label label2;

        private Container components;

        public Text0()
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
            var resources = new System.Resources.ResourceManager(typeof (Text0));
            this.tb = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tb
            // 
            this.tb.AccessibleDescription = resources.GetString("tb.AccessibleDescription");
            this.tb.AccessibleName = resources.GetString("tb.AccessibleName");
            this.tb.Anchor = ((System.Windows.Forms.AnchorStyles) (resources.GetObject("tb.Anchor")));
            this.tb.AutoSize = ((bool) (resources.GetObject("tb.AutoSize")));
            this.tb.BackgroundImage = ((System.Drawing.Image) (resources.GetObject("tb.BackgroundImage")));
            this.tb.Dock = ((System.Windows.Forms.DockStyle) (resources.GetObject("tb.Dock")));
            this.tb.Enabled = ((bool) (resources.GetObject("tb.Enabled")));
            this.tb.Font = ((System.Drawing.Font) (resources.GetObject("tb.Font")));
            this.tb.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("tb.ImeMode")));
            this.tb.Location = ((System.Drawing.Point) (resources.GetObject("tb.Location")));
            this.tb.MaxLength = ((int) (resources.GetObject("tb.MaxLength")));
            this.tb.Multiline = ((bool) (resources.GetObject("tb.Multiline")));
            this.tb.Name = "tb";
            this.tb.PasswordChar = ((char) (resources.GetObject("tb.PasswordChar")));
            this.tb.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("tb.RightToLeft")));
            this.tb.ScrollBars = ((System.Windows.Forms.ScrollBars) (resources.GetObject("tb.ScrollBars")));
            this.tb.Size = ((System.Drawing.Size) (resources.GetObject("tb.Size")));
            this.tb.TabIndex = ((int) (resources.GetObject("tb.TabIndex")));
            this.tb.Text = resources.GetString("tb.Text");
            this.tb.TextAlign = ((System.Windows.Forms.HorizontalAlignment) (resources.GetObject("tb.TextAlign")));
            this.tb.Visible = ((bool) (resources.GetObject("tb.Visible")));
            this.tb.WordWrap = ((bool) (resources.GetObject("tb.WordWrap")));
            // 
            // label1
            // 
            this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
            this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles) (resources.GetObject("label1.Anchor")));
            this.label1.AutoSize = ((bool) (resources.GetObject("label1.AutoSize")));
            this.label1.Dock = ((System.Windows.Forms.DockStyle) (resources.GetObject("label1.Dock")));
            this.label1.Enabled = ((bool) (resources.GetObject("label1.Enabled")));
            this.label1.Font = ((System.Drawing.Font) (resources.GetObject("label1.Font")));
            this.label1.Image = ((System.Drawing.Image) (resources.GetObject("label1.Image")));
            this.label1.ImageAlign = ((System.Drawing.ContentAlignment) (resources.GetObject("label1.ImageAlign")));
            this.label1.ImageIndex = ((int) (resources.GetObject("label1.ImageIndex")));
            this.label1.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("label1.ImeMode")));
            this.label1.Location = ((System.Drawing.Point) (resources.GetObject("label1.Location")));
            this.label1.Name = "label1";
            this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("label1.RightToLeft")));
            this.label1.Size = ((System.Drawing.Size) (resources.GetObject("label1.Size")));
            this.label1.TabIndex = ((int) (resources.GetObject("label1.TabIndex")));
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.TextAlign = ((System.Drawing.ContentAlignment) (resources.GetObject("label1.TextAlign")));
            this.label1.Visible = ((bool) (resources.GetObject("label1.Visible")));
            // 
            // label2
            // 
            this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
            this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles) (resources.GetObject("label2.Anchor")));
            this.label2.AutoSize = ((bool) (resources.GetObject("label2.AutoSize")));
            this.label2.Dock = ((System.Windows.Forms.DockStyle) (resources.GetObject("label2.Dock")));
            this.label2.Enabled = ((bool) (resources.GetObject("label2.Enabled")));
            this.label2.Font = ((System.Drawing.Font) (resources.GetObject("label2.Font")));
            this.label2.Image = ((System.Drawing.Image) (resources.GetObject("label2.Image")));
            this.label2.ImageAlign = ((System.Drawing.ContentAlignment) (resources.GetObject("label2.ImageAlign")));
            this.label2.ImageIndex = ((int) (resources.GetObject("label2.ImageIndex")));
            this.label2.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("label2.ImeMode")));
            this.label2.Location = ((System.Drawing.Point) (resources.GetObject("label2.Location")));
            this.label2.Name = "label2";
            this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("label2.RightToLeft")));
            this.label2.Size = ((System.Drawing.Size) (resources.GetObject("label2.Size")));
            this.label2.TabIndex = ((int) (resources.GetObject("label2.TabIndex")));
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.TextAlign = ((System.Drawing.ContentAlignment) (resources.GetObject("label2.TextAlign")));
            this.label2.Visible = ((bool) (resources.GetObject("label2.Visible")));
            // 
            // Text0
            // 
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScaleBaseSize = ((System.Drawing.Size) (resources.GetObject("$this.AutoScaleBaseSize")));
            this.AutoScroll = ((bool) (resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size) (resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size) (resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image) (resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = ((System.Drawing.Size) (resources.GetObject("$this.ClientSize")));
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb);
            this.Enabled = ((bool) (resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font) (resources.GetObject("$this.Font")));
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point) (resources.GetObject("$this.Location")));
            this.MaximumSize = ((System.Drawing.Size) (resources.GetObject("$this.MaximumSize")));
            this.MinimumSize = ((System.Drawing.Size) (resources.GetObject("$this.MinimumSize")));
            this.Name = "Text0";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("$this.RightToLeft")));
            this.StartPosition = ((System.Windows.Forms.FormStartPosition) (resources.GetObject("$this.StartPosition")));
            this.Text = resources.GetString("$this.Text");
            this.Load += new System.EventHandler(this.Text0_Load);
            this.Controls.SetChildIndex(this.tb, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.ResumeLayout(false);
        }

        #endregion

        protected override void FillElement()
        {
            elOption.SetAttribute("value", tb.Text);
            base.FillElement();
        }

        protected override void FillForm()
        {
            tb.Text = elOption.GetAttribute("value");
            base.FillForm();
        }

        private void Text0_Load(object sender, EventArgs e)
        {
        }
    }
}