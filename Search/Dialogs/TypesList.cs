using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class TypesList : FreeDialog
    {
        public ArrayList IdList = new ArrayList();
        private TreeView tvTypes;
        private Panel panel1;
        private Panel panel2;
        private Button bnCancel;
        private Button bnOk;
        public ArrayList TempIdList = new ArrayList();

        private Container components;

        public TypesList()
        {
            InitializeComponent();
            InitTree();
        }

        public TypesList(ArrayList IdList)
        {
            InitializeComponent();
            TempIdList = IdList;
            InitTree();
        }

        private DataSet ds;

        private void InitTree()
        {
            ds = Environment.TypesPersonData.GetAllTypes();
            if (ds != null && ds.Tables.Count > 0)
            {
                DataColumn parentColumn =
                    ds.Tables[0].Columns[" Ó‰“ÂÏ˚ÀËˆ‡"];
                DataColumn childColumn =
                    ds.Tables[0].Columns["Parent"];

                DataRelation relCustOrder = new DataRelation(" Ó‰“ÂÏ˚ÀËˆ‡Parent", parentColumn, childColumn, false);
                ds.Relations.Add(relCustOrder);

                var keys = new DataColumn[1];
                keys[0] = parentColumn;
                ds.Tables[0].PrimaryKey = keys;

                FillTree(-1, null);
            }
        }

        private void FillTree(int id, TreeNode parentNode)
        {
            if (id < 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row.GetParentRow(" Ó‰“ÂÏ˚ÀËˆ‡Parent") == null)
                    {
                        var node = new TreeNode(row[Environment.TypesPersonData.NameField].ToString()) {Tag = row};
                        if (TempIdList.Contains((int) row[Environment.TypesPersonData.IDField]))
                            node.Checked = true;
                        FillTree((int) row[Environment.TypesPersonData.IDField], node);
                        tvTypes.Nodes.Add(node);
                    }
                }
            }
            else
            {
                DataRow parent = ds.Tables[0].Rows.Find(id.ToString());
                foreach (DataRow t in parent.GetChildRows(" Ó‰“ÂÏ˚ÀËˆ‡Parent"))
                {
                    var node = new TreeNode(t[Environment.TypesPersonData.NameField].ToString())
                                   {
                                       Tag = t,
                                       Checked = TempIdList.Contains((int) t[Environment.TypesPersonData.IDField])
                                   };
                    FillTree((int) t[Environment.TypesPersonData.IDField], node);
                    parentNode.Nodes.Add(node);
                }
            }
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

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (TypesList));
            this.tvTypes = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bnOk = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvTypes
            // 
            this.tvTypes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tvTypes.CheckBoxes = true;
            resources.ApplyResources(this.tvTypes, "tvTypes");
            this.tvTypes.FullRowSelect = true;
            this.tvTypes.ItemHeight = 16;
            this.tvTypes.Name = "tvTypes";
            this.tvTypes.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvTypes_AfterCheck);
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
            this.panel2.Controls.Add(this.tvTypes);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // TypesList
            // 
            this.AcceptButton = this.bnOk;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.bnCancel;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "TypesList";

            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private void tvTypes_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) 
                return;
            TreeNode node = e.Node;
            if (node.Checked)
            {
                ClearChildNodes(node);
                ClearParentNodes(node);
            }
        }

        private void ClearChildNodes(TreeNode parentNode)
        {
            if (parentNode.Nodes.Count > 0)
            {
                foreach (TreeNode node in parentNode.Nodes)
                {
                    node.Checked = false;
                    ClearChildNodes(node);
                }
            }
        }

        private void ClearParentNodes(TreeNode childNode)
        {
            if (childNode.Parent != null)
            {
                childNode.Parent.Checked = false;
                ClearParentNodes(childNode.Parent);
            }
        }

        public struct TypesInfo
        {
            public int ID;
            public string Name;
        }

        private void FillIdList(TreeNode node)
        {
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode tn in node.Nodes)
                {
                    if (tn.Checked)
                    {
                        var row = tn.Tag as DataRow;
                        IdList.Add(new TypesInfo
                                     {
                                         ID = Convert.ToInt32(row[Environment.TypesPersonData.IDField]),
                                         Name = (string) row[Environment.TypesPersonData.NameField]
                                     });
                    }
                    FillIdList(tn);
                }
            }
        }

        private void bnOk_Click(object sender, EventArgs e)
        {
            IdList.Clear();
            if (tvTypes.Nodes != null)
                foreach (TreeNode tn in tvTypes.Nodes)
                {
                    if (tn.Checked)
                    {
                        var row = tn.Tag as DataRow;
                        IdList.Add(new TypesInfo
                                     {
                                         ID = Convert.ToInt32(row[Environment.TypesPersonData.IDField]),
                                         Name = (string) row[Environment.TypesPersonData.NameField]
                                     });
                    }
                    FillIdList(tn);
                }
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