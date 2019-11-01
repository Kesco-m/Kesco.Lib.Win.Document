using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class “ипы‘инопераций : Base
    {
        private ColumnHeader cHName;
        private ListView lv“ипы;

        /// <summary>
        ///   Required designer variable.
        /// </summary>
        private Container components;

        public “ипы‘инопераций()
        {
            InitializeComponent();
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

        #region Component Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof (“ипы‘инопераций));
            this.lv“ипы = new System.Windows.Forms.ListView();
            this.cHName = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lv“ипы
            // 
            this.lv“ипы.AccessibleDescription = resources.GetString("lv“ипы.AccessibleDescription");
            this.lv“ипы.AccessibleName = resources.GetString("lv“ипы.AccessibleName");
            this.lv“ипы.Alignment = ((System.Windows.Forms.ListViewAlignment) (resources.GetObject("lv“ипы.Alignment")));
            this.lv“ипы.Anchor = ((System.Windows.Forms.AnchorStyles) (resources.GetObject("lv“ипы.Anchor")));
            this.lv“ипы.BackgroundImage = ((System.Drawing.Image) (resources.GetObject("lv“ипы.BackgroundImage")));
            this.lv“ипы.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                                             {
                                                 this.cHName
                                             });
            this.lv“ипы.Dock = ((System.Windows.Forms.DockStyle) (resources.GetObject("lv“ипы.Dock")));
            this.lv“ипы.Enabled = ((bool) (resources.GetObject("lv“ипы.Enabled")));
            this.lv“ипы.Font = ((System.Drawing.Font) (resources.GetObject("lv“ипы.Font")));
            this.lv“ипы.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv“ипы.HideSelection = false;
            this.lv“ипы.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("lv“ипы.ImeMode")));
            this.lv“ипы.LabelWrap = ((bool) (resources.GetObject("lv“ипы.LabelWrap")));
            this.lv“ипы.Location = ((System.Drawing.Point) (resources.GetObject("lv“ипы.Location")));
            this.lv“ипы.MultiSelect = false;
            this.lv“ипы.Name = "lv“ипы";
            this.lv“ипы.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("lv“ипы.RightToLeft")));
            this.lv“ипы.Size = ((System.Drawing.Size) (resources.GetObject("lv“ипы.Size")));
            this.lv“ипы.TabIndex = ((int) (resources.GetObject("lv“ипы.TabIndex")));
            this.lv“ипы.Text = resources.GetString("lv“ипы.Text");
            this.lv“ипы.View = System.Windows.Forms.View.Details;
            this.lv“ипы.Visible = ((bool) (resources.GetObject("lv“ипы.Visible")));
            // 
            // cHName
            // 
            this.cHName.Text = resources.GetString("cHName.Text");
            this.cHName.TextAlign =
                ((System.Windows.Forms.HorizontalAlignment) (resources.GetObject("cHName.TextAlign")));
            this.cHName.Width = ((int) (resources.GetObject("cHName.Width")));
            // 
            // “ипы‘инопераций
            // 
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScaleBaseSize = ((System.Drawing.Size) (resources.GetObject("$this.AutoScaleBaseSize")));
            this.AutoScroll = ((bool) (resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size) (resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size) (resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image) (resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = ((System.Drawing.Size) (resources.GetObject("$this.ClientSize")));
            this.Controls.Add(this.lv“ипы);
            this.Enabled = ((bool) (resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font) (resources.GetObject("$this.Font")));
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point) (resources.GetObject("$this.Location")));
            this.MaximumSize = ((System.Drawing.Size) (resources.GetObject("$this.MaximumSize")));
            this.MinimumSize = ((System.Drawing.Size) (resources.GetObject("$this.MinimumSize")));
            this.Name = "“ипы‘инопераций";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("$this.RightToLeft")));
            this.StartPosition = ((System.Windows.Forms.FormStartPosition) (resources.GetObject("$this.StartPosition")));
            this.Text = resources.GetString("$this.Text");
            this.Controls.SetChildIndex(this.lv“ипы, 0);
            this.ResumeLayout(false);
        }

        #endregion

        protected override void FillElement()
        {
            if (lv“ипы.SelectedItems.Count > 0)
                elOption.SetAttribute("value", lv“ипы.SelectedItems[0].Tag.ToString());
        }

        protected override void FillForm()
        {
            string id = elOption.GetAttribute("value");
            using (
                DataTable dt =
                    Environment.TransactionTypeData.GetData(Environment.CurCultureInfo.TwoLetterISOLanguageName))
            using (DataTableReader dr = dt.CreateDataReader())
            {
                while (dr.Read())
                {
                    ListViewItem item = new ListViewItem(dr[Environment.TransactionTypeData.NameField].ToString())
                                            {Tag = dr[Environment.TransactionTypeData.IDField]};
                    lv“ипы.Items.Add(item);
                    if (item.Tag.ToString() == id)
                        item.Selected = true;
                }
                cHName.Width = dt.Rows.Count < 9
                                   ? lv“ипы.Width - 5
                                   : lv“ипы.Width - SystemInformation.VerticalScrollBarWidth - 4;

                dr.Close();
                dr.Dispose();
                dt.Dispose();
            }
        }
    }
}