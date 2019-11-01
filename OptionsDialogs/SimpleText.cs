using System;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Options;

namespace Kesco.Lib.Win.Document.OptionsDialogs
{
	public class SimpleText : Kesco.Lib.Win.Document.OptionsDialogs.Base
    {
        private TextBox tbText;
        private Label lCaption;

        private Container components;

        public SimpleText(SimpleTextOption option) : base(option)
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SimpleText));
            this.DoubleBuffered = true;
            this.tbText = new System.Windows.Forms.TextBox();
            this.lCaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbText
            // 
            this.tbText.AccessibleDescription = resources.GetString("tbText.AccessibleDescription");
            this.tbText.AccessibleName = resources.GetString("tbText.AccessibleName");
            this.tbText.Anchor = ((System.Windows.Forms.AnchorStyles) (resources.GetObject("tbText.Anchor")));
            this.tbText.AutoSize = ((bool) (resources.GetObject("tbText.AutoSize")));
            this.tbText.BackgroundImage = ((System.Drawing.Image) (resources.GetObject("tbText.BackgroundImage")));
            this.tbText.Dock = ((System.Windows.Forms.DockStyle) (resources.GetObject("tbText.Dock")));
            this.tbText.Enabled = ((bool) (resources.GetObject("tbText.Enabled")));
            this.tbText.Font = ((System.Drawing.Font) (resources.GetObject("tbText.Font")));
            this.tbText.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("tbText.ImeMode")));
            this.tbText.Location = ((System.Drawing.Point) (resources.GetObject("tbText.Location")));
            this.tbText.MaxLength = ((int) (resources.GetObject("tbText.MaxLength")));
            this.tbText.Multiline = ((bool) (resources.GetObject("tbText.Multiline")));
            this.tbText.Name = "tbText";
            this.tbText.PasswordChar = ((char) (resources.GetObject("tbText.PasswordChar")));
            this.tbText.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("tbText.RightToLeft")));
            this.tbText.ScrollBars = ((System.Windows.Forms.ScrollBars) (resources.GetObject("tbText.ScrollBars")));
            this.tbText.Size = ((System.Drawing.Size) (resources.GetObject("tbText.Size")));
            this.tbText.TabIndex = ((int) (resources.GetObject("tbText.TabIndex")));
            this.tbText.Text = resources.GetString("tbText.Text");
            this.tbText.TextAlign =
                ((System.Windows.Forms.HorizontalAlignment) (resources.GetObject("tbText.TextAlign")));
            this.tbText.Visible = ((bool) (resources.GetObject("tbText.Visible")));
            this.tbText.WordWrap = ((bool) (resources.GetObject("tbText.WordWrap")));
            this.tbText.TextChanged += new System.EventHandler(this.tbText_TextChanged);
            // 
            // lCaption
            // 
            this.lCaption.AccessibleDescription = resources.GetString("lCaption.AccessibleDescription");
            this.lCaption.AccessibleName = resources.GetString("lCaption.AccessibleName");
            this.lCaption.Anchor = ((System.Windows.Forms.AnchorStyles) (resources.GetObject("lCaption.Anchor")));
            this.lCaption.AutoSize = ((bool) (resources.GetObject("lCaption.AutoSize")));
            this.lCaption.Dock = ((System.Windows.Forms.DockStyle) (resources.GetObject("lCaption.Dock")));
            this.lCaption.Enabled = ((bool) (resources.GetObject("lCaption.Enabled")));
            this.lCaption.Font = ((System.Drawing.Font) (resources.GetObject("lCaption.Font")));
            this.lCaption.Image = ((System.Drawing.Image) (resources.GetObject("lCaption.Image")));
            this.lCaption.ImageAlign = ((System.Drawing.ContentAlignment) (resources.GetObject("lCaption.ImageAlign")));
            this.lCaption.ImageIndex = ((int) (resources.GetObject("lCaption.ImageIndex")));
            this.lCaption.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("lCaption.ImeMode")));
            this.lCaption.Location = ((System.Drawing.Point) (resources.GetObject("lCaption.Location")));
            this.lCaption.Name = "lCaption";
            this.lCaption.RightToLeft =
                ((System.Windows.Forms.RightToLeft) (resources.GetObject("lCaption.RightToLeft")));
            this.lCaption.Size = ((System.Drawing.Size) (resources.GetObject("lCaption.Size")));
            this.lCaption.TabIndex = ((int) (resources.GetObject("lCaption.TabIndex")));
            this.lCaption.Text = resources.GetString("lCaption.Text");
            this.lCaption.TextAlign = ((System.Drawing.ContentAlignment) (resources.GetObject("lCaption.TextAlign")));
            this.lCaption.Visible = ((bool) (resources.GetObject("lCaption.Visible")));
            // 
            // SimpleText
            // 
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScaleBaseSize = ((System.Drawing.Size) (resources.GetObject("$this.AutoScaleBaseSize")));
            this.AutoScroll = ((bool) (resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size) (resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size) (resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image) (resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = ((System.Drawing.Size) (resources.GetObject("$this.ClientSize")));
            this.Controls.Add(this.lCaption);
            this.Controls.Add(this.tbText);
            this.Enabled = ((bool) (resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font) (resources.GetObject("$this.Font")));
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point) (resources.GetObject("$this.Location")));
            this.MaximumSize = ((System.Drawing.Size) (resources.GetObject("$this.MaximumSize")));
            this.MinimumSize = ((System.Drawing.Size) (resources.GetObject("$this.MinimumSize")));
            this.Name = "SimpleText";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("$this.RightToLeft")));
            this.StartPosition = ((System.Windows.Forms.FormStartPosition) (resources.GetObject("$this.StartPosition")));
            this.Text = resources.GetString("$this.Text");
            this.Load += new System.EventHandler(this.SimpleText_Load);
            this.Controls.SetChildIndex(this.tbText, 0);
            this.Controls.SetChildIndex(this.lCaption, 0);
            this.ResumeLayout(false);
        }

        #endregion

        private SimpleTextOption Option
        {
            get { return (SimpleTextOption) option; }
        }

        private void SimpleText_Load(object sender, EventArgs e)
        {
            tbText.Text = Option.Text;
            lCaption.Text = Option.GetCaption();
        }

        private void tbText_TextChanged(object sender, EventArgs e)
        {
            Option.Text = tbText.Text;
        }
    }
}