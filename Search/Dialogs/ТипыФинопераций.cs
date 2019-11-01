using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class ��������������� : Base
    {
        private ColumnHeader cHName;
        private ListView lv����;

        /// <summary>
        ///   Required designer variable.
        /// </summary>
        private Container components;

        public ���������������()
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
            var resources = new System.Resources.ResourceManager(typeof (���������������));
            this.lv���� = new System.Windows.Forms.ListView();
            this.cHName = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lv����
            // 
            this.lv����.AccessibleDescription = resources.GetString("lv����.AccessibleDescription");
            this.lv����.AccessibleName = resources.GetString("lv����.AccessibleName");
            this.lv����.Alignment = ((System.Windows.Forms.ListViewAlignment) (resources.GetObject("lv����.Alignment")));
            this.lv����.Anchor = ((System.Windows.Forms.AnchorStyles) (resources.GetObject("lv����.Anchor")));
            this.lv����.BackgroundImage = ((System.Drawing.Image) (resources.GetObject("lv����.BackgroundImage")));
            this.lv����.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                                             {
                                                 this.cHName
                                             });
            this.lv����.Dock = ((System.Windows.Forms.DockStyle) (resources.GetObject("lv����.Dock")));
            this.lv����.Enabled = ((bool) (resources.GetObject("lv����.Enabled")));
            this.lv����.Font = ((System.Drawing.Font) (resources.GetObject("lv����.Font")));
            this.lv����.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv����.HideSelection = false;
            this.lv����.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("lv����.ImeMode")));
            this.lv����.LabelWrap = ((bool) (resources.GetObject("lv����.LabelWrap")));
            this.lv����.Location = ((System.Drawing.Point) (resources.GetObject("lv����.Location")));
            this.lv����.MultiSelect = false;
            this.lv����.Name = "lv����";
            this.lv����.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("lv����.RightToLeft")));
            this.lv����.Size = ((System.Drawing.Size) (resources.GetObject("lv����.Size")));
            this.lv����.TabIndex = ((int) (resources.GetObject("lv����.TabIndex")));
            this.lv����.Text = resources.GetString("lv����.Text");
            this.lv����.View = System.Windows.Forms.View.Details;
            this.lv����.Visible = ((bool) (resources.GetObject("lv����.Visible")));
            // 
            // cHName
            // 
            this.cHName.Text = resources.GetString("cHName.Text");
            this.cHName.TextAlign =
                ((System.Windows.Forms.HorizontalAlignment) (resources.GetObject("cHName.TextAlign")));
            this.cHName.Width = ((int) (resources.GetObject("cHName.Width")));
            // 
            // ���������������
            // 
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScaleBaseSize = ((System.Drawing.Size) (resources.GetObject("$this.AutoScaleBaseSize")));
            this.AutoScroll = ((bool) (resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size) (resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size) (resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image) (resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = ((System.Drawing.Size) (resources.GetObject("$this.ClientSize")));
            this.Controls.Add(this.lv����);
            this.Enabled = ((bool) (resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font) (resources.GetObject("$this.Font")));
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode) (resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point) (resources.GetObject("$this.Location")));
            this.MaximumSize = ((System.Drawing.Size) (resources.GetObject("$this.MaximumSize")));
            this.MinimumSize = ((System.Drawing.Size) (resources.GetObject("$this.MinimumSize")));
            this.Name = "���������������";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft) (resources.GetObject("$this.RightToLeft")));
            this.StartPosition = ((System.Windows.Forms.FormStartPosition) (resources.GetObject("$this.StartPosition")));
            this.Text = resources.GetString("$this.Text");
            this.Controls.SetChildIndex(this.lv����, 0);
            this.ResumeLayout(false);
        }

        #endregion

        protected override void FillElement()
        {
            if (lv����.SelectedItems.Count > 0)
                elOption.SetAttribute("value", lv����.SelectedItems[0].Tag.ToString());
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
                    lv����.Items.Add(item);
                    if (item.Tag.ToString() == id)
                        item.Selected = true;
                }
                cHName.Width = dt.Rows.Count < 9
                                   ? lv����.Width - 5
                                   : lv����.Width - SystemInformation.VerticalScrollBarWidth - 4;

                dr.Close();
                dr.Dispose();
                dt.Dispose();
            }
        }
    }
}