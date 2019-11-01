using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class Бухгалтерии : Base
    {
        private ColumnHeader cHName;
        private ListView lvБухгалтерии;
        private IContainer components;

        public Бухгалтерии()
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

        #region Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof (Бухгалтерии));
            this.lvБухгалтерии = new System.Windows.Forms.ListView();
            this.cHName = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lvБухгалтерии
            // 
            this.lvБухгалтерии.AccessibleDescription = resources.GetString("lvБухгалтерии.AccessibleDescription");
            this.lvБухгалтерии.AccessibleName = resources.GetString("lvБухгалтерии.AccessibleName");
            this.lvБухгалтерии.Alignment =
                ((System.Windows.Forms.ListViewAlignment) (resources.GetObject("lvБухгалтерии.Alignment")));
            this.lvБухгалтерии.Anchor =
                ((System.Windows.Forms.AnchorStyles) (resources.GetObject("lvБухгалтерии.Anchor")));
            this.lvБухгалтерии.BackgroundImage =
                ((System.Drawing.Image) (resources.GetObject("lvБухгалтерии.BackgroundImage")));
            this.lvБухгалтерии.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                                                    {
                                                        this.cHName
                                                    });
            this.lvБухгалтерии.Dock = ((System.Windows.Forms.DockStyle) (resources.GetObject("lvБухгалтерии.Dock")));
            this.lvБухгалтерии.Enabled = ((bool) (resources.GetObject("lvБухгалтерии.Enabled")));
            this.lvБухгалтерии.Font = ((System.Drawing.Font) (resources.GetObject("lvБухгалтерии.Font")));
            this.lvБухгалтерии.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvБухгалтерии.HideSelection = false;
            this.lvБухгалтерии.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("lvБухгалтерии.ImeMode")));
            this.lvБухгалтерии.LabelWrap = ((bool) (resources.GetObject("lvБухгалтерии.LabelWrap")));
            this.lvБухгалтерии.Location = ((System.Drawing.Point) (resources.GetObject("lvБухгалтерии.Location")));
            this.lvБухгалтерии.MultiSelect = false;
            this.lvБухгалтерии.Name = "lvБухгалтерии";
            this.lvБухгалтерии.RightToLeft =
                ((System.Windows.Forms.RightToLeft) (resources.GetObject("lvБухгалтерии.RightToLeft")));
            this.lvБухгалтерии.Size = ((System.Drawing.Size) (resources.GetObject("lvБухгалтерии.Size")));
            this.lvБухгалтерии.TabIndex = ((int) (resources.GetObject("lvБухгалтерии.TabIndex")));
            this.lvБухгалтерии.Text = resources.GetString("lvБухгалтерии.Text");
            this.lvБухгалтерии.View = System.Windows.Forms.View.Details;
            this.lvБухгалтерии.Visible = ((bool) (resources.GetObject("lvБухгалтерии.Visible")));
            // 
            // cHName
            // 
            this.cHName.Text = resources.GetString("cHName.Text");
            this.cHName.TextAlign =
                ((System.Windows.Forms.HorizontalAlignment) (resources.GetObject("cHName.TextAlign")));
            this.cHName.Width = ((int) (resources.GetObject("cHName.Width")));
            // 
            // Бухгалтерии
            // 
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScaleBaseSize = ((System.Drawing.Size) (resources.GetObject("$this.AutoScaleBaseSize")));
            this.AutoScroll = ((bool) (resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size) (resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size) (resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image) (resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = ((System.Drawing.Size) (resources.GetObject("$this.ClientSize")));
            this.Controls.Add(this.lvБухгалтерии);
            this.Enabled = ((bool) (resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font) (resources.GetObject("$this.Font")));
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point) (resources.GetObject("$this.Location")));
            this.MaximumSize = ((System.Drawing.Size) (resources.GetObject("$this.MaximumSize")));
            this.MinimumSize = ((System.Drawing.Size) (resources.GetObject("$this.MinimumSize")));
            this.Name = "Бухгалтерии";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("$this.RightToLeft")));
            this.StartPosition = ((System.Windows.Forms.FormStartPosition) (resources.GetObject("$this.StartPosition")));
            this.Text = resources.GetString("$this.Text");
            this.Controls.SetChildIndex(this.lvБухгалтерии, 0);
            this.ResumeLayout(false);
        }

        #endregion

        protected override void FillElement()
        {
            if (lvБухгалтерии.SelectedItems.Count > 0)
            {
                var dr = (DataRow) lvБухгалтерии.SelectedItems[0].Tag;
                elOption.SetAttribute("value", dr[Environment.BuhData.BuhPersonIDField].ToString());
            }
        }

        protected override void FillForm()
        {
            string id = elOption.GetAttribute("value");
            if (Environment.IC == null)
                return;
            for (int i = 0; i < Environment.IC.Rows.Count; i++)
            {
                DataRow dr = Environment.IC.Rows[i];
                ListViewItem item = new ListViewItem(dr[Environment.BuhData.BuhField].ToString()) {Tag = dr};
                lvБухгалтерии.Items.Add(item);
                if (item.Tag.ToString() == id)
                    item.Selected = true;
            }
            cHName.Width = Environment.IC.Rows.Count < 9
                               ? lvБухгалтерии.Width - 5
                               : lvБухгалтерии.Width - SystemInformation.VerticalScrollBarWidth - 5;
        }
    }
}