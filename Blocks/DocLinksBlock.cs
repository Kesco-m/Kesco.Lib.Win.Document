using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Dialogs;
using Kesco.Lib.Win.Document.Items;
using Kesco.Lib.Win.Document.ListViews;

namespace Kesco.Lib.Win.Document.Blocks
{
    public class DocLinksBlock : UserControl
    {
        public event BlockEventHandler Added;
        private Timer checkOpenDocs = new Timer();

        private Button delButton;
        private DocBlock docBlock;
        private SelectableColorListView listDocs;
        private ColumnHeader columnText;
        private ColumnHeader columnLinkType;
        private ContextMenu contextMenuOpenDoc;
        private Button bnOpenDocs;
        private Container components;

        #region Accessors

        public int[] PersonIDs
        {
            get { return docBlock.PersonIDs; }
            set { docBlock.PersonIDs = value; }
        }

        public int Count
        {
            get { return listDocs.Items.Count; }
        }

        public ListItem this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                    return listDocs.Items[index] as ListItem;
                
                throw new Exception("PersonBlock: " + Environment.StringResources.GetString("Index") + " " + index +
                                    " " +
                                    Environment.StringResources.GetString("Dialog_CheckedControlCollection_Error1") +
                                    ": " + Count + ")");
            }
        }

        public int LoadDocLinks(int docID)
        {
            int returnCount = 0;
            using (DataTable dt = Environment.DocLinksData.GetLinksDocs(docID, false, true))
            using (DataTableReader dr = dt.CreateDataReader())
            {
                while (dr.Read())
                {
                    var documentID = (int) dr[Environment.DocLinksData.DocumentIDField];
                    if (AddDoc(documentID, true, true))
                        returnCount++;
                }
                dr.Close();
                dr.Dispose();
                dt.Dispose();
            }

            using (DataTable dt = Environment.DocLinksData.GetLinksDocs(docID, false, false))
            using (DataTableReader dr = dt.CreateDataReader())
            {
                while (dr.Read())
                {
                    var documentID = (int) dr[Environment.DocLinksData.DocumentIDField];
                    if (AddDoc(documentID, false, true))
                        returnCount++;
                }
                dr.Close();
                dr.Dispose();
                dt.Dispose();
            }
            return returnCount;
        }

        public DateTime DocDate { get; set; }

        #endregion

        public DocLinksBlock()
        {
            InitializeComponent();
            UpdateControl();

            bnOpenDocs.Click += bnOpenDocs_Click;

            checkOpenDocs.Interval = 1000;
            checkOpenDocs.Tick += checkOpenDocs_Tick;
            checkOpenDocs.Start();
        }

        private void checkOpenDocs_Tick(object sender, EventArgs e)
        {
            checkOpenDocs.Stop();

            bnOpenDocs.Enabled = Environment.OpenDocs.Count > 0;

            checkOpenDocs.Start();
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

        #region Component Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (DocLinksBlock));
            this.docBlock = new DocBlock();
            this.listDocs = new SelectableColorListView();
            this.columnText = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.columnLinkType = ((System.Windows.Forms.ColumnHeader) (new System.Windows.Forms.ColumnHeader()));
            this.delButton = new System.Windows.Forms.Button();
            this.contextMenuOpenDoc = new System.Windows.Forms.ContextMenu();
            this.bnOpenDocs = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // docBlock
            // 
            resources.ApplyResources(this.docBlock, "docBlock");
            this.docBlock.Name = "docBlock";
            this.docBlock.PersonIDs = null;
            this.docBlock.Selected += new BlockEventHandler(this.docBlock_Selected);
            // 
            // listDocs
            // 
            resources.ApplyResources(this.listDocs, "listDocs");
            this.listDocs.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.listDocs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                                               {
                                                   this.columnText,
                                                   this.columnLinkType
                                               });
            this.listDocs.FullRowSelect = true;
            this.listDocs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listDocs.HideSelection = false;
            this.listDocs.IsSelectedForeGroundColorEnable = false;
            this.listDocs.MultiSelect = false;
            this.listDocs.Name = "listDocs";
            this.listDocs.SelectedBackGroundColor = System.Drawing.Color.LightGray;
            this.listDocs.SelectedForeGroundColor = System.Drawing.Color.Orange;
            this.listDocs.ShowItemToolTips = true;
            this.listDocs.UseCompatibleStateImageBehavior = false;
            this.listDocs.View = System.Windows.Forms.View.Details;
            this.listDocs.SelectedIndexChanged += new System.EventHandler(this.listDocs_SelectedIndexChanged);
            this.listDocs.SizeChanged += new System.EventHandler(this.listDocs_SizeChanged);
            // 
            // columnText
            // 
            resources.ApplyResources(this.columnText, "columnText");
            // 
            // columnLinkType
            // 
            resources.ApplyResources(this.columnLinkType, "columnLinkType");
            // 
            // delButton
            // 
            resources.ApplyResources(this.delButton, "delButton");
            this.delButton.Name = "delButton";
            this.delButton.Click += new System.EventHandler(this.delButton_Click);
            // 
            // contextMenuOpenDoc
            // 
            this.contextMenuOpenDoc.Popup += new System.EventHandler(this.contextMenuOpenDoc_Popup);
            // 
            // bnOpenDocs
            // 
            resources.ApplyResources(this.bnOpenDocs, "bnOpenDocs");
            this.bnOpenDocs.Name = "bnOpenDocs";
            // 
            // DocLinksBlock
            // 
            this.Controls.Add(this.bnOpenDocs);
            this.Controls.Add(this.delButton);
            this.Controls.Add(this.listDocs);
            this.Controls.Add(this.docBlock);
            this.DoubleBuffered = true;
            this.Name = "DocLinksBlock";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
        }

        #endregion

        private void docBlock_Selected(object source, BlockEventArgs e)
        {
            if (e.ID > 0)
            {
                var linkTypeDialog =
                    new LinkTypeDialog(Environment.StringResources.GetString("CreatedDocument"), e.ID, DocDate);
                linkTypeDialog.DialogEvent += linkTypeDialog_DialogEvent;
                linkTypeDialog.Owner = FindForm();
                linkTypeDialog.Show();
            }
        }

        private void delButton_Click(object sender, EventArgs e)
        {
            if (listDocs.SelectedItems.Count > 0)
            {
                var item = listDocs.SelectedItems[0] as BoolListItem;
                if (item != null && !item.Check)
                {
                    docBlock.UsedDocsIDs.Remove(item.ID);
                    listDocs.SelectedItems[0].Remove();
                    UpdateControl();
                }
            }
        }

        private void listDocs_SizeChanged(object sender, EventArgs e)
        {
            columnText.Width = listDocs.Width - (listDocs.Scrollable ? 24 : 4);
        }

        private void UpdateControl()
        {
            delButton.Enabled = listDocs.SelectedItems.Count > 0;
        }

        private void listDocs_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControl();
        }

        private void linkTypeDialog_DialogEvent(object source, DialogEventArgs e)
        {
            var dialog = e.Dialog as LinkTypeDialog;
            if (dialog != null && dialog.DialogResult == DialogResult.OK)
                AddDoc(dialog.TwoID, !dialog.Basic, false);
        }

        private bool AddDoc(int docID, bool main, bool fix)
        {
            var item = listDocs.Items.Cast<BoolListItem>().FirstOrDefault(old => old.ID == docID);
            if (item != null)
            {
                if (!item.Check)
                    item.Check = true;
                return false;
            }
            
            string docString = DBDocString.Format(docID);
            listDocs.Items.Add(new BoolListItem(docID,
                                                new[] {docString, main.ToString()}, false)
                                   {ForeColor = main ? Color.Violet : Color.RoyalBlue, ToolTipText = docString});
            docBlock.UsedDocsIDs.Add(docID);
            docBlock.Text = "";
            UpdateControl();

            if (Added != null)
                Added(this, new BlockEventArgs(docID, docString));

            return true;
        }

        private void contextMenuOpenDoc_Popup(object sender, EventArgs e)
        {
            contextMenuOpenDoc.MenuItems.Clear();
            try
            {
                foreach (var item in from t in Environment.OpenDocs where !docBlock.UsedDocsIDs.Contains(t.Key) select new MenuItem {Text = DBDocString.Format(t.Key)})
                {
                    item.Click += item_Click;

                    contextMenuOpenDoc.MenuItems.Add(item);
                }
            }
            catch
            {
            }
        }


        private void item_Click(object sender, EventArgs e)
        {
            var item = (MenuItem) sender;
            Match m = Regex.Match(item.Text,@"^\[(?<id>\d+)\]");

            if (m.Success)
            {
                var linkTypeDialog =
                    new LinkTypeDialog(Environment.StringResources.GetString("CreatedDocument"),
                                               Int32.Parse(m.Groups["id"].Value), DocDate);
                linkTypeDialog.DialogEvent += linkTypeDialog_DialogEvent;
                linkTypeDialog.Owner = FindForm();
                linkTypeDialog.Show();
            }
        }

        private void bnOpenDocs_Click(object sender, EventArgs e)
        {
            contextMenuOpenDoc.Show(bnOpenDocs, new Point(0, bnOpenDocs.Height));
        }
    }
}