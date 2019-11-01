#region

using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

#endregion

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class BusinessProjectList : FreeDialog
    {
        private Panel panel1;
        private Button bnOk;
        private Button bnCancel;
        private Panel panel2;
        private TreeView tvProjects;
        private DataSet ds;

        private Container components;

        public DataRow BusinessProjectRow { get; set; }

        public BusinessProjectList()
        {
            InitializeComponent();
            InitTree();
        }

        private void InitTree()
        {
            ds = Environment.BusinessProjectData.GetOpenBusinessProjects();
            if (ds == null || ds.Tables.Count <= 0)
                return;

            DataColumn parentColumn =
                ds.Tables[0].Columns[Environment.BusinessProjectData.IDField];
            DataColumn childColumn =
                ds.Tables[0].Columns["Parent"];

            DataRelation relCustOrder = new DataRelation("ÊîäÁèçíåñÏðîåêòàParent", parentColumn, childColumn, false);
            ds.Relations.Add(relCustOrder);

            var keys = new DataColumn[1];
            keys[0] = parentColumn;
            ds.Tables[0].PrimaryKey = keys;

            FillTree(-1, null);
        }

        private void FillTree(int id, TreeNode parentNode)
        {
            if (id < 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row.GetParentRow("ÊîäÁèçíåñÏðîåêòàParent") != null) 
                        continue;
                    var node = new TreeNode(row[Environment.BusinessProjectData.NameField].ToString()) {Tag = row};
                    FillTree((int) row[Environment.BusinessProjectData.IDField], node);
                    tvProjects.Nodes.Add(node);
                }
            }
            else
            {
                DataRow parent = ds.Tables[0].Rows.Find(id.ToString());
                DataRow[] rows = parent.GetChildRows("ÊîäÁèçíåñÏðîåêòàParent");
                foreach (DataRow t in rows)
                {
                    var node = new TreeNode(t[Environment.BusinessProjectData.NameField].ToString()) {Tag = t};
                    FillTree((int) t[Environment.BusinessProjectData.IDField], node);
                    parentNode.Nodes.Add(node);
                }
            }
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
                new System.ComponentModel.ComponentResourceManager(typeof (BusinessProjectList));
            this.panel1 = new System.Windows.Forms.Panel();
            this.bnOk = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tvProjects = new System.Windows.Forms.TreeView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bnOk);
            this.panel1.Controls.Add(this.bnCancel);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // bnOk
            // 
            resources.ApplyResources(this.bnOk, "bnOk");
            this.bnOk.Name = "bnOk";
            this.bnOk.Click += new System.EventHandler(this.bnOk_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.bnCancel, "bnCancel");
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Click += new System.EventHandler(this.bnCancel_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tvProjects);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // tvProjects
            // 
            resources.ApplyResources(this.tvProjects, "tvProjects");
            this.tvProjects.FullRowSelect = true;
            this.tvProjects.ItemHeight = 16;
            this.tvProjects.Name = "tvProjects";
            // 
            // BusinessProjectList
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "BusinessProjectList";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private void bnOk_Click(object sender, EventArgs e)
        {
            if (tvProjects.SelectedNode == null)
                return;
            BusinessProjectRow = tvProjects.SelectedNode.Tag as DataRow;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void bnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}