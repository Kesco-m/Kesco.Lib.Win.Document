using System;
using System.ComponentModel;
using System.Windows.Forms;


namespace Kesco.Lib.Win.Document.Select
{
    public class SelectArchiveDialog : FreeDialog
    {
        private bool clickParent;
		private Trees.DTree tree;
        private Button buttonOK;
        private Button buttonCancel;
        private IContainer components;

        private ImageList archiveImageList;

        private int archiveID;

        public SelectArchiveDialog(int archiveID, bool useParent)
        {
            InitializeComponent();

            this.archiveID = archiveID;
            clickParent = useParent;
        }

        #region Accessors

        public Kesco.Lib.Win.Trees.DTreeNode ArchiveNode
        {
            get { return tree.SelectedNode; }
        }

        #endregion

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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectArchiveDialog));
			this.tree = new Kesco.Lib.Win.Trees.DTree();
			this.archiveImageList = new System.Windows.Forms.ImageList(this.components);
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tree
			// 
			resources.ApplyResources(this.tree, "tree");
			this.tree.AllowAdd = false;
			this.tree.AllowDelete = false;
			this.tree.AllowEdit = false;
			this.tree.AllowFind = false;
			this.tree.AllowMove = false;
			this.tree.ColorInsert = System.Drawing.Color.SkyBlue;
			this.tree.ColorMove = System.Drawing.Color.CornflowerBlue;
			this.tree.ConnectionString = null;
			this.tree.FullLoad = true;
			this.tree.HideSelection = false;
			this.tree.IDField = "КодХранилища";
			this.tree.ImageList = this.archiveImageList;
			this.tree.ItemHeight = 16;
			this.tree.Name = "tree";
			this.tree.NodeType = typeof(Kesco.Lib.Win.Trees.DTreeNode);
			this.tree.SelectedNode = null;
			this.tree.TableName = "Документы.dbo.Хранилища";
			this.tree.TextField = "Хранилище";
			this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
			this.tree.DoubleClick += new System.EventHandler(this.tree_DoubleClick);
			// 
			// archiveImageList
			// 
			this.archiveImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("archiveImageList.ImageStream")));
			this.archiveImageList.TransparentColor = System.Drawing.SystemColors.Window;
			this.archiveImageList.Images.SetKeyName(0, "");
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// SelectArchiveDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.tree);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectArchiveDialog";
			this.Load += new System.EventHandler(this.SelectArchive_Load);
			this.ResumeLayout(false);

        }

        #endregion

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            buttonOK.Enabled = tree.SelectedNode != null && (clickParent || tree.SelectedNode.Nodes.Count == 0);
        }

        private void SelectArchive_Load(object sender, EventArgs e)
        {
            tree.ConnectionString = Environment.ConnectionStringDocument;

            string queryString =
                "SELECT * FROM " + tree.TableName + " WHERE " + tree.IDField + " <> 0";

            tree.SetQueryString(queryString);
            tree.Fill();
        }

        private void tree_DoubleClick(object sender, EventArgs e)
        {
            if (tree.SelectedNode != null && (clickParent || tree.SelectedNode.Nodes.Count == 0))
                End(DialogResult.OK);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }
    }
}