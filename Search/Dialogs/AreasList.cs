using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class AreasList : FreeDialog
    {
        private AreasDataGrid dataGridAreas;
        private DataTable dt;
        private Button button1;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private TextBox tbSearch;
        private Label label1;
        private DataGridTableStyle dataGridTableStyle1;
        private DataGridTextBoxColumn dataGridTextBoxColumn1;

        private Container components;

        public AreasList()
        {
            ID = -1;
            InitializeComponent();

            dt = Environment.AreaData.GetAreas();
            dataGridAreas.SizeChanged += dataGridAreas_SizeChanged;
        }

        public AreasList(string search)
            : this()
        {
            this.search = search;
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
                new System.ComponentModel.ComponentResourceManager(typeof (AreasList));
            this.dataGridAreas = new AreasDataGrid();
            this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
            this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize) (this.dataGridAreas)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridAreas
            // 
            this.dataGridAreas.CaptionVisible = false;
            this.dataGridAreas.DataMember = "";
            resources.ApplyResources(this.dataGridAreas, "dataGridAreas");
            this.dataGridAreas.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGridAreas.Name = "dataGridAreas";
            this.dataGridAreas.ParentRowsVisible = false;
            this.dataGridAreas.RowHeadersVisible = false;
            this.dataGridAreas.TableStyles.AddRange(new System.Windows.Forms.DataGridTableStyle[]
                                                        {
                                                            this.dataGridTableStyle1
                                                        });
            this.dataGridAreas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridAreas_MouseDown);
            // 
            // dataGridTableStyle1
            // 
            this.dataGridTableStyle1.DataGrid = this.dataGridAreas;
            this.dataGridTableStyle1.GridColumnStyles.AddRange(new System.Windows.Forms.DataGridColumnStyle[]
                                                                   {
                                                                       this.dataGridTextBoxColumn1
                                                                   });
            this.dataGridTableStyle1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGridTableStyle1.MappingName = "Справочники.dbo.vwСтраны";
            this.dataGridTableStyle1.RowHeadersVisible = false;
            // 
            // dataGridTextBoxColumn1
            // 
            this.dataGridTextBoxColumn1.Format = "";
            this.dataGridTextBoxColumn1.FormatInfo = null;
            resources.ApplyResources(this.dataGridTextBoxColumn1, "dataGridTextBoxColumn1");
            this.dataGridTextBoxColumn1.ReadOnly = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbSearch);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label1
            // 
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tbSearch
            // 
            resources.ApplyResources(this.tbSearch, "tbSearch");
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.TextChanged += new System.EventHandler(this.tbSearch_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dataGridAreas);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // AreasList
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "AreasList";

            this.Load += new System.EventHandler(this.AreasList_Load);
            ((System.ComponentModel.ISupportInitialize) (this.dataGridAreas)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private void AreasList_Load(object sender, EventArgs e)
        {
            if (dt != null)
            {
                dataGridAreas.DataSource = dt;
                if (search != "")
                {
                    tbSearch.Text = search;
                    tbSearch_TextChanged(this, EventArgs.Empty);
                }
            }
            ColumnSizeChange();
        }

        private string search = "";

        public string Area { get; private set; }
        public int ID { get; private set; }

        private void dataGridAreas_MouseDown(object sender, MouseEventArgs e)
        {
            DataGrid.HitTestInfo hi = dataGridAreas.HitTest(new Point(e.X, e.Y));
            if (hi.Type != DataGrid.HitTestType.Cell)
                return;
            var cm = (CurrencyManager) BindingContext[dataGridAreas.DataSource];
            cm.Position = hi.Row;
            var drv = (DataRowView) cm.Current;
            if (drv == null)
                return;

            ID = (int) drv[0];
            Area = (string) drv[1];
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            for (int n = 0; n < dt.Rows.Count; n++)
            {
                if (dt.Rows[n]["Страна"].ToString().ToLower().StartsWith(tbSearch.Text.ToLower()))
                {
                    var cm = (CurrencyManager) BindingContext[dataGridAreas.DataSource];
                    cm.Position = n;

                    break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ColumnSizeChange()
        {
            dataGridTextBoxColumn1.Width = dataGridAreas.Width - dataGridAreas.VertScrollBarWidth - 4;
        }

        private void dataGridAreas_SizeChanged(object sender, EventArgs e)
        {
            ColumnSizeChange();
        }
    }

    public class AreasDataGrid : DataGrid
    {
        public int VertScrollBarWidth
        {
            get { return VertScrollBar.Visible && VertScrollBar.Enabled ? VertScrollBar.Width : 0; }
        }
    }
}