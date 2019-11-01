using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Dialogs;
using Kesco.Lib.Win.Error;

namespace Kesco.Lib.Win.Document.Controls
{
    public class MenuList : UserControl
    {
        public TreeViewEventHandler SelectNode;
        private int docID;
        private DateTime docData;
        private IntPtr tooltipShownID;
        private bool hasChild;
        private DataSet MainLinkSet;
        private DataSet ChildLinkSet;

        private byte mode = 1;
        private byte sort;
        private int oldLinkCount = 15;

        private TreeView treeLinks;
        private ContextMenu linksContextMenu;
        private ImageList imageList;
        private MenuItem menuUp;
        private MenuItem menuDown;
        private MenuItem menuDel;
        private MenuItem menuOpen;
        private ToolTip toolTip;
        private IContainer components;

        public MenuList(int docID, string zoom)
        {
            this.docID = docID;
            Zoom = zoom;
            InitializeComponent();

            Cursor = Cursors.WaitCursor;
            treeLinks.Visible = false;
            tooltipShownID = IntPtr.Zero;
			sort = (byte)Environment.Settings.LoadIntOption("LinkSort", 2);
 			mode = (byte)Environment.Settings.LoadIntOption("LinkMode", 1);
            oldLinkCount = Environment.Settings.LoadIntOption("OldLinkCount", 15);
            if (oldLinkCount < 0)
                oldLinkCount = 0;
            FillDate();
        }

        public MenuList() : this(0, "100")
        {
        }

        #region Event

        public event MouseEventHandler TreeMouseUp;

        protected internal void OnTreeMouseUp(object sender, MouseEventArgs e)
        {
            if (TreeMouseUp != null)
                TreeMouseUp(sender, e);
        }

        public event KeyEventHandler TreeKeysUp;

        protected internal void OnTreeKeysUp(object sender, KeyEventArgs e)
        {
            if (TreeKeysUp != null)
                TreeKeysUp(sender, e);
        }

        public event EventHandler NeedClose;

        protected internal void OnNeedClose()
        {
            if (NeedClose != null)
                NeedClose(this, EventArgs.Empty);
        }

        public event LinkDeleted DeleteLink;

        protected internal void OnDeleteLink(int parentDocID, int childDocID)
        {
            if (DeleteLink != null)
                DeleteLink(parentDocID, childDocID);
        }

        public event EventHandler OpenWindow;

        protected internal void OnOpenWindow()
        {
            if (OpenWindow != null)
                OpenWindow(this, EventArgs.Empty);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (treeLinks != null)
                {
                    Console.WriteLine("{0}: Remove node begin", DateTime.Now.ToString("HH:mm:ss fff"));
                    treeLinks.BeforeExpand -= treeLinks_BeforeExpand;
                    treeLinks.VisibleChanged -= treeLinks_VisibleChanged;
                    treeLinks.AfterSelect -= treeLinks_AfterSelect;
                    treeLinks.MouseMove -= treeLinks_MouseMove;
                    treeLinks.MouseDown -= treeLinks_MouseUp;
                    treeLinks.KeyDown -= treeLinks_KeyDown;
                    treeLinks.MouseLeave -= treeLinks_MouseLeave;
                    ClearTree();
                    treeLinks.Dispose();
                    treeLinks = null;
                    Console.WriteLine("{0}: Remove node end", DateTime.Now.ToString("HH:mm:ss fff"));
                }
                Console.WriteLine("{0}: Tables dispose begin", DateTime.Now.ToString("HH:mm:ss fff"));
                if (MainLinkSet != null)
                {
                    MainLinkSet.Dispose();
                    MainLinkSet = null;
                }
                if (ChildLinkSet != null)
                {
                    ChildLinkSet.Dispose();
                    ChildLinkSet = null;
                }

                Console.WriteLine("{0}: Tables dispose begin", DateTime.Now.ToString("HH:mm:ss fff"));
                Console.WriteLine("{0}: Component dispose begin", DateTime.Now.ToString("HH:mm:ss fff"));
                if (components != null)
                {
                    components.Dispose();
                }
                Console.WriteLine("{0}: Component dispose end", DateTime.Now.ToString("HH:mm:ss fff"));
            }
            Console.WriteLine("{0}: Dispose begin", DateTime.Now.ToString("HH:mm:ss fff"));
            base.Dispose(disposing);
            Console.WriteLine("{0}: Dispose end", DateTime.Now.ToString("HH:mm:ss fff"));
        }

        #region Component Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (MenuList));
            this.linksContextMenu = new System.Windows.Forms.ContextMenu();
            this.menuUp = new System.Windows.Forms.MenuItem();
            this.menuDown = new System.Windows.Forms.MenuItem();
            this.menuDel = new System.Windows.Forms.MenuItem();
            this.menuOpen = new System.Windows.Forms.MenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.treeLinks = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // linksContextMenu
            // 
            this.linksContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
                                                         {
                                                             this.menuUp,
                                                             this.menuDown,
                                                             this.menuDel,
                                                             this.menuOpen
                                                         });
            this.linksContextMenu.Popup += new System.EventHandler(this.linksContextMenu_Popup);
            // 
            // menuUp
            // 
            this.menuUp.Index = 0;
            resources.ApplyResources(this.menuUp, "menuUp");
            this.menuUp.Click += new System.EventHandler(this.menuUp_Click);
            // 
            // menuDown
            // 
            this.menuDown.Index = 1;
            resources.ApplyResources(this.menuDown, "menuDown");
            this.menuDown.Click += new System.EventHandler(this.menuDown_Click);
            // 
            // menuDel
            // 
            this.menuDel.Index = 2;
            resources.ApplyResources(this.menuDel, "menuDel");
            this.menuDel.Click += new System.EventHandler(this.menuDel_Click);
            // 
            // menuOpen
            // 
            this.menuOpen.Index = 3;
            resources.ApplyResources(this.menuOpen, "menuOpen");
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream =
                ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.SystemColors.Control;
            this.imageList.Images.SetKeyName(0, "");
            this.imageList.Images.SetKeyName(1, "");
            this.imageList.Images.SetKeyName(2, "");
            this.imageList.Images.SetKeyName(3, "");
            this.imageList.Images.SetKeyName(4, "");
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 0;
            this.toolTip.InitialDelay = 50;
            this.toolTip.ReshowDelay = 100;
            this.toolTip.UseFading = false;
            // 
            // treeLinks
            // 
            resources.ApplyResources(this.treeLinks, "treeLinks");
            this.treeLinks.ContextMenu = this.linksContextMenu;
            this.treeLinks.FullRowSelect = true;
            this.treeLinks.HideSelection = false;
            this.treeLinks.ImageList = this.imageList;
            this.treeLinks.ItemHeight = 16;
            this.treeLinks.Name = "treeLinks";
            this.treeLinks.BeforeExpand +=
                new System.Windows.Forms.TreeViewCancelEventHandler(this.treeLinks_BeforeExpand);
            this.treeLinks.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeLinks_AfterSelect);
            this.treeLinks.VisibleChanged += new System.EventHandler(this.treeLinks_VisibleChanged);
            this.treeLinks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeLinks_KeyDown);
            this.treeLinks.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeLinks_MouseUp);
            this.treeLinks.MouseLeave += new System.EventHandler(this.treeLinks_MouseLeave);
            this.treeLinks.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeLinks_MouseMove);
            // 
            // MenuList
            // 
            this.Controls.Add(this.treeLinks);
            this.DoubleBuffered = true;
            this.Name = "MenuList";
            resources.ApplyResources(this, "$this");
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MenuList_Paint);
            this.ResumeLayout(false);
        }

        #endregion

        #region Control Work

        private void MenuList_Paint(object sender, PaintEventArgs e)
        {
            var myPen = new Pen(Color.Black, 1);
            var myRectangle = new Rectangle(0, 0, Width - 1, Height - 1);
            e.Graphics.DrawRectangle(myPen, myRectangle);
        }

        #endregion

        #region Accessors

        public int DocID
        {
            get { return docID; }
            set
            {
                docID = value;

                ClearTree();
                Cursor = Cursors.WaitCursor;
                treeLinks.Visible = false;
                treeLinks.BeginUpdate();
                FillDate();
                treeLinks.EndUpdate();
            }
        }

        public Controls.DocControl.CheckLinkDocMethod CheckLinkDoc { get; set; }
        public bool NoClose { get; set; }
        public string Zoom { get; set; }

        public int VisibleNodesCount
        {
            get { return treeLinks.VisibleCount; }
        }

        #endregion

        #region Menu

        private void menuUp_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeLinks.SelectedNode;
            var dr = ((DataRow) tn.Tag);
            if ((int) dr["Direction"] == 1)
            {
                try
                {
                    Environment.DocLinksData.ChangeChildDocLinkOrder(
                        (int) ((DataRow) tn.PrevNode.Tag)[Environment.DocLinksData.IDField],
                        (int) dr[Environment.DocLinksData.IDField]);

                    if (ChildLinkSet != null)
                    {
                        ChildLinkSet.Dispose();
                        ChildLinkSet = null;
                    }
                    ChildLinkSet = Environment.DocLinksData.GetChildLinksData(docID);
                    hasChild = ChildLinkSet.Tables[0].Rows.Count > 0 || ChildLinkSet.Tables[1].Rows.Count > 0;
                    treeLinks.BeginUpdate();
                    TreeNode ctn = treeLinks.Nodes[treeLinks.Nodes.Count - 1];
                    treeLinks.Nodes.Remove(ctn);
                    ctn.Nodes.Clear();
                    ctn = CreateChildNode();
                    treeLinks.Nodes.Add(ctn);
                    treeLinks.EndUpdate();
                }
                catch (Exception ex)
                {
                    Data.Env.WriteToLog(ex);
                }
            }
            else
            {
                try
                {
                    Environment.DocLinksData.ChangeParentDocLinkOrder(
                        (int) ((DataRow) tn.PrevNode.Tag)[Environment.DocLinksData.IDField],
                        (int) dr[Environment.DocLinksData.IDField]);

                    if (MainLinkSet != null)
                    {
                        MainLinkSet.Dispose();
                        MainLinkSet = null;
                    }
                    MainLinkSet = Environment.DocLinksData.GetMainLinksData(docID);
                    treeLinks.BeginUpdate();
                    TreeNode mtn = treeLinks.Nodes[0];
                    treeLinks.Nodes.Remove(mtn);
                    mtn.Nodes.Clear();
                    mtn = CreateMainNode();
                    treeLinks.Nodes.Insert(0, mtn);
                    treeLinks.EndUpdate();
                }
                catch (Exception ex)
                {
                    Data.Env.WriteToLog(ex);
                }
            }
        }

        private void menuDown_Click(object sender, EventArgs e)
        {
            TreeNode tn = treeLinks.SelectedNode;
            var dr = ((DataRow) tn.Tag);
            if ((int) dr["Direction"] == 1)
            {
                try
                {
                    Environment.DocLinksData.ChangeChildDocLinkOrder((int) dr[Environment.DocLinksData.IDField],
                                                                     (int)
                                                                     ((DataRow) tn.NextNode.Tag)[
                                                                         Environment.DocLinksData.IDField]);

                    if (ChildLinkSet != null)
                    {
                        ChildLinkSet.Dispose();
                        ChildLinkSet = null;
                    }
                    ChildLinkSet = Environment.DocLinksData.GetChildLinksData(docID);
                    hasChild = ChildLinkSet.Tables[0].Rows.Count > 0 || ChildLinkSet.Tables[1].Rows.Count > 0;
                    treeLinks.BeginUpdate();
                    TreeNode ctn = treeLinks.Nodes[treeLinks.Nodes.Count - 1];
                    treeLinks.Nodes.Remove(ctn);
                    ctn.Nodes.Clear();
                    ctn = CreateChildNode();
                    treeLinks.Nodes.Add(ctn);
                    treeLinks.EndUpdate();
                }
                catch (Exception ex)
                {
                    Data.Env.WriteToLog(ex);
                }
            }
            else
            {
                try
                {
                    Environment.DocLinksData.ChangeParentDocLinkOrder((int) dr[Environment.DocLinksData.IDField],
                                                                      (int)
                                                                      ((DataRow) tn.NextNode.Tag)[
                                                                          Environment.DocLinksData.IDField]);

                    if (MainLinkSet != null)
                    {
                        MainLinkSet.Dispose();
                        MainLinkSet = null;
                    }
                    MainLinkSet = Environment.DocLinksData.GetMainLinksData(docID);
                    treeLinks.BeginUpdate();
                    TreeNode mtn = treeLinks.Nodes[0];
                    treeLinks.Nodes.Remove(mtn);
                    mtn.Nodes.Clear();
                    mtn = CreateMainNode();
                    treeLinks.Nodes.Insert(0, mtn);
                    treeLinks.EndUpdate();
                }
                catch (Exception ex)
                {
                    Data.Env.WriteToLog(ex);
                }
            }
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
        }

        private void LinkOpenDoc_Click(object sender, EventArgs e)
        {
            var item = sender as MenuItem;
            if (item == null)
                return;

            Match m = Regex.Match(item.Text, @"^\[(?<id>\d+)\]");
            if (!m.Success)
                return;

            var dialog = new LinkTypeDialog(Int32.Parse(item.Tag.ToString()),
                                            Int32.Parse(m.Groups["id"].Value));
            dialog.DialogEvent += LinkTypeDialog_DialogEvent;
            dialog.Show();
        }

        private void Update_MenuLinkOpenDoc(int linkDocId)
        {
            if (Environment.OpenDocs.Count == 0)
            {
                menuOpen.Enabled = false;
                return;
            }
            menuOpen.MenuItems.Clear();
            List<int> curDocLinkIDs = RefreshLinks(linkDocId);
            foreach (var item in from t in Environment.OpenDocs
                                 where t.Key != linkDocId && curDocLinkIDs != null && !curDocLinkIDs.Contains(t.Key)
                                 select new MenuItem {Tag = linkDocId, Text = DBDocString.Format(t.Key)})
            {
                item.Click += LinkOpenDoc_Click;
                menuOpen.MenuItems.Add(item);
            }

            menuOpen.Enabled = (menuOpen.MenuItems.Count > 0);
        }

        private List<int> RefreshLinks(int linkDocId)
        {
            var curDocLinkIDs = new List<int>();
            if (linkDocId > 0)
            {
                using (DataSet ds = Environment.DocLinksData.GetLinkIDs(linkDocId))
                {
                    if (ds != null)
                    {
                        using (DataTable dt = ds.Tables[0])
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                DataRow dr = dt.Rows[i];
                                var docID = (int) dr[Environment.DocData.IDField];

                                curDocLinkIDs.Add(docID);
                            }
                        }
                    }
                }
            }
            return curDocLinkIDs;
        }

        private void LinkTypeDialog_DialogEvent(object source, DialogEventArgs e)
        {
            Focus();

            var dialog = e.Dialog as LinkTypeDialog;
            if (dialog == null || dialog.DialogResult != DialogResult.OK)
                return;
            int parentID = dialog.OneID;
            int childID = dialog.TwoID;

            if (!dialog.Basic)
            {
                parentID = dialog.TwoID;
                childID = dialog.OneID;
            }

            if (CheckLinkDoc(parentID, childID))
                Environment.DocLinksData.AddDocLink(parentID, childID);
        }

        private void menuDel_Click(object sender, EventArgs e)
        {
            NoClose = true;
            try
            {
                if (treeLinks.SelectedNode == null)
                    return;

                TreeNode tn = treeLinks.SelectedNode;
                if (tn.Tag == null)
                    return;
                var dr = (DataRow) tn.Tag;
                if (dr[Environment.DocLinksData.SubFieldIDField] != null &&
                    !dr[Environment.DocLinksData.SubFieldIDField].Equals(DBNull.Value))
                {
                    MessageForm.Show(
                        Environment.StringResources.GetString("MenuList_menuDel_Click_Message2"),
                        Environment.StringResources.GetString("Warning"));
                    NoClose = false;
                    return;
                }
                string docString = DBDocString.Format(docID);

                if (MessageBox.Show(
                    Environment.StringResources.GetString("MenuList_menuDel_Click_Message1") + " " +
                    docString + System.Environment.NewLine + Environment.StringResources.GetString("And") + " " +
                    tn.Text + "?", Environment.StringResources.GetString("Confirmation"),
                    MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    Environment.DocLinksData.DelLink((int) dr[Environment.DocLinksData.IDField]);
                    bool child = (int) dr["Direction"] == 1 ? true : false;
                    OnDeleteLink((child ? docID : (int) dr[Environment.DocLinksData.IDField]),
                                 (child ? (int) dr[Environment.DocLinksData.IDField] : docID));
                    if (!child && MainLinkSet != null)
                    {
                        MainLinkSet.Dispose();
                        MainLinkSet = null;
                        MainLinkSet = Environment.DocLinksData.GetMainLinksData(docID);
                    }
                    if (child && ChildLinkSet != null)
                    {
                        ChildLinkSet.Dispose();
                        ChildLinkSet = null;
                        ChildLinkSet = Environment.DocLinksData.GetChildLinksData(docID);
                    }
                    treeLinks.BeginUpdate();
                    for (int i = treeLinks.Nodes.Count - 1; i >= 0; i--)
                    {
                        treeLinks.Nodes[i].Remove();
                    }
                    CreateNode();

                    if (treeLinks.Nodes.Count == 0)
                        OnNeedClose();
                    treeLinks.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
            finally
            {
                NoClose = false;
            }
        }

        public void linksContextMenu_Popup(object sender, EventArgs e)
        {
            Point p = PointToClient(MousePosition);
            TreeNode tn = treeLinks.GetNodeAt(p);
            if (tn != null)
                treeLinks.SelectedNode = tn;
            else
                tn = treeLinks.SelectedNode;
            menuDown.Enabled = false;
            if (tn.Tag == null || !(tn.Tag is DataRow))
            {
                menuUp.Enabled = false;
                menuDel.Enabled = false;
                menuOpen.Enabled = false;
            }
            else
            {
                menuDel.Enabled = true;
                menuOpen.Enabled = true;
                menuUp.Enabled = sort == 0 && (tn.Index != 0 && tn.PrevNode.Tag != null);
                menuDown.Enabled = sort == 0 && (tn.Index < tn.Parent.Nodes.Count - 1 && tn.NextNode.Tag != null);
            }

            if (menuOpen.Enabled)
            {
                Update_MenuLinkOpenDoc(GetLinkedDocID());
            }
        }

        #endregion

        #region Methods

        private void FillDate()
        {
            if (docID > 0 && Environment.IsConnectedDocs)
            {
                object obj = Environment.DocData.GetField(Environment.DocData.DateField, docID);
                if (obj != null && obj != DBNull.Value)
                {
                    docData = (DateTime) obj;
                }
                else
                    docData = DateTime.MinValue;
                MainLinkSet = Environment.DocLinksData.GetMainLinksData(docID);
                ChildLinkSet = Environment.DocLinksData.GetChildLinksData(docID);
                hasChild = ChildLinkSet != null && ChildLinkSet.Tables != null && ChildLinkSet.Tables.Count > 0 &&
                           (ChildLinkSet.Tables[0].Rows.Count > 0 || ChildLinkSet.Tables[1].Rows.Count > 0);
                CreateNode();
            }
        }


        public int GetLinksCount()
        {
            int result = 0;
            try
            {
                if (ChildLinkSet != null && ChildLinkSet.Tables != null && ChildLinkSet.Tables.Count > 0)
                {
                    result += ChildLinkSet.Tables[0].Rows.Count + ChildLinkSet.Tables[1].Rows.Count;
                    result += ChildLinkSet.Tables.Count > 1 ? ChildLinkSet.Tables[1].Rows.Count : 0;
                }
                if (MainLinkSet != null && MainLinkSet.Tables != null && MainLinkSet.Tables.Count > 0)
                {
                    result += MainLinkSet.Tables[0].Rows.Count + MainLinkSet.Tables[1].Rows.Count;
                    result += MainLinkSet.Tables.Count > 1 ? MainLinkSet.Tables[1].Rows.Count : 0;
                }
            }
            catch (NullReferenceException)
            {
                return 0;
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }

            return result;
        }

        public int GetLinkedDocID()
        {
            if (selectedRow != null)
            {
                var selectDocId = (int) selectedRow[Environment.DocLinksData.DocumentIDField];
                return selectDocId;
            }
            return 0;
        }

        #endregion

        #region Tree work

        private void ClearTree(TreeNode node)
        {
            for (int i = node.Nodes.Count - 1; i >= 0; i--)
            {
                ClearTree(node.Nodes[i]);
                node.Nodes[i].Remove();
            }
        }

        private void ClearTree()
        {
            treeLinks.BeginUpdate();
            for (int i = treeLinks.Nodes.Count - 1; i >= 0; i--)
            {
                ClearTree(treeLinks.Nodes[i]);
                treeLinks.Nodes[i].Remove();
            }
            treeLinks.EndUpdate();
        }

        /// <summary>
        ///   Создание ветки документов-оснований в дереве.
        /// </summary>
        /// <returns> </returns>
        private TreeNode CreateMainNode()
        {
            if (Disposing || IsDisposed)
                return null;
            TreeNode rMtn = null;
            try
            {
                if (MainLinkSet != null && MainLinkSet.Tables.Count > 0)
                {
                    int count = 0;
                    DataTable dt = MainLinkSet.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        rMtn = new TreeNode(Environment.StringResources.GetString("MainDocuments"))
                                   {ImageIndex = 0, ForeColor = Color.Violet, SelectedImageIndex = 0};
                        for (int i = 0; i < dt.Rows.Count; ++i)
                        {
                            if (Disposing)
                                return null;
                            var sTN = new TreeNode(dt.Rows[i]["Поле"].ToString())
                                          {
                                              ImageIndex = 1,
                                              ForeColor = Color.Violet,
                                              SelectedImageIndex = 1,
                                              Tag = (int) dt.Rows[i]["КодПоляДокумента"]
                                          };
                            sTN.Nodes.Insert(0, new TreeNode("Почетный пустой нод"));

                            rMtn.Nodes.Add(sTN);
                        }
                        count = rMtn.Nodes.Count;
                    }

                    DataRow[] dR = MainLinkSet.Tables.Count > 1 ? MainLinkSet.Tables[1].Select() : null;
                    if (dR != null && dR.Length > 0)
                    {
                        if (rMtn == null)
                            rMtn = new TreeNode(Environment.StringResources.GetString("MainDocuments"))
                                       {ImageIndex = 0, ForeColor = Color.Violet, SelectedImageIndex = 0};
                        if (dR.Length < oldLinkCount)
                            CreateTree(rMtn, dR, true, Color.Violet, count, true, !hasChild);
                        else
                        {
                            var drs = dR.Select(
                                x => new KeyValuePair<int, string>((int) x[Environment.DocTypeData.IDField],
                                                                   x[Environment.DocTypeData.NameField].
                                                                       ToString())).Distinct().OrderBy(
                                                                           x => x.Value);

                            foreach (KeyValuePair<int, string> p in drs)
                            {
                                if (Disposing || IsDisposed)
                                    return null;
                                var sTN = new TreeNode(p.Value)
                                              {
                                                  ImageIndex = 1,
                                                  ForeColor = Color.Violet,
                                                  SelectedImageIndex = 1,
                                                  Tag = p
                                              };
                                sTN.Nodes.Insert(0, new TreeNode("Почетный пустой нод") {Tag = 0});

                                rMtn.Nodes.Add(sTN);
                            }
                            if (!hasChild)
                            {
                                Cursor = Cursors.Default;
                                treeLinks.Visible = true;
                            }
                        }
                    }
                    else if (!hasChild)
                    {
                        Cursor = Cursors.Default;
                        treeLinks.Visible = true;
                    }
                }
            }

            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
            return rMtn;
        }

        /// <summary>
        ///   Создание ветки вытекающих документов в дереве.
        /// </summary>
        private TreeNode CreateChildNode()
        {
            if (Disposing || IsDisposed)
                return null;

            TreeNode rStn = null;

            try
            {
                if (ChildLinkSet != null)
                {
                    int count = 0;
                    if (ChildLinkSet.Tables.Count > 0 && ChildLinkSet.Tables[0].Rows.Count > 0)
                    {
                        rStn = new TreeNode(Environment.StringResources.GetString("SubsequentDocuments"))
                                   {
                                       ImageIndex = 0,
                                       ForeColor = Color.RoyalBlue,
                                       SelectedImageIndex = 0
                                   };

                        for (int i = 0; i < ChildLinkSet.Tables[0].Rows.Count; ++i)
                        {
                            if (Disposing)
                                return null;
                            var sTN = new TreeNode(ChildLinkSet.Tables[0].Rows[i]["Поле"].ToString())
                                          {
                                              ImageIndex = 1,
                                              ForeColor = Color.RoyalBlue,
                                              SelectedImageIndex = 1,
                                              Tag = 0 - (int) ChildLinkSet.Tables[0].Rows[i]["КодПоляДокумента"]
                                          };
                            sTN.Nodes.Insert(0, new TreeNode("Почетный пустой нод"));

                            rStn.Nodes.Add(sTN);
                        }
                        count = rStn.Nodes.Count;
                    }

                    if (ChildLinkSet.Tables.Count > 1 && ChildLinkSet.Tables[1].Rows.Count > 0)
                    {
                        if (rStn == null)
                            rStn = new TreeNode(Environment.StringResources.GetString("SubsequentDocuments"))
                                       {
                                           ImageIndex = 0,
                                           ForeColor = Color.RoyalBlue,
                                           SelectedImageIndex = 0
                                       };
                        if (ChildLinkSet.Tables[1].Rows.Count < oldLinkCount)
                            CreateTree(rStn, ChildLinkSet.Tables[1].Rows.Cast<DataRow>().ToArray(), false,
                                       Color.RoyalBlue,
                                       count, true, hasChild);
                        else
                        {
                            var drs =
                                ChildLinkSet.Tables[1].Rows.Cast<DataRow>().Select<DataRow, KeyValuePair<int, string>>(
                                    x =>
                                    new KeyValuePair<int, string>(-(int) x[Environment.DocTypeData.IDField],
                                                                  x[Environment.DocTypeData.NameField].ToString())).
                                    Distinct
                                    ().OrderBy(x => x.Value);
                            foreach (KeyValuePair<int, string> p in drs)
                            {
                                if (Disposing || IsDisposed)
                                    return null;
                                var sTN = new TreeNode(p.Value)
                                              {
                                                  ImageIndex = 1,
                                                  ForeColor = Color.RoyalBlue,
                                                  SelectedImageIndex = 1,
                                                  Tag = p
                                              };
                                sTN.Nodes.Insert(0, new TreeNode("Почетный пустой нод") {Tag = 0});

                                rStn.Nodes.Add(sTN);
                            }
                            Cursor = Cursors.Default;
                            treeLinks.Visible = true;
                        }
                    }
                    else
                    {
                        Cursor = Cursors.Default;
                        treeLinks.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
            return rStn;
        }

        private void CreateNode()
        {
            if (Disposing)
                return;
            treeLinks.BeginUpdate();
            try
            {
                TreeNode rMtn = CreateMainNode();
                if (rMtn != null)
                {
                    rMtn.Expand();
                    rMtn.Collapse();
                    rMtn.Expand();
                    if (InvokeRequired)
                        BeginInvoke(new Action<TreeNodeCollection, TreeNode>(AddNode),
                                    new object[] {treeLinks.Nodes, rMtn});
                    else
                        treeLinks.Nodes.Add(rMtn);
                    rMtn.Expand();
                }

                TreeNode rStn = CreateChildNode();
                if (rStn != null)
                {
                    if (mode == 1)
                        rStn.Nodes.Cast<TreeNode>().OrderBy(x => x.Text);
                    rStn.Expand();
                    rStn.Collapse();
                    if (InvokeRequired)
                        BeginInvoke(new Action<TreeNodeCollection, TreeNode>(AddNode),
                                    new object[] {treeLinks.Nodes, rStn});
                    else
                        treeLinks.Nodes.Add(rStn);
                }
            }
            catch (NullReferenceException)
            {
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
                ErrorShower.OnShowError(this, ex.Message, "");
            }
            finally
            {
                if (treeLinks.Nodes.Count > 0)
                {
                    if (InvokeRequired)
                        BeginInvoke(new Action<TreeNodeCollection, TreeNode>(ExpandNode),
                                    new object[] {treeLinks.Nodes, null});
                    else
                        ExpandNode(treeLinks.Nodes, null);
                }
                treeLinks.EndUpdate();
            }
        }

        private void treeLinks_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TreeNode tr = e.Node;
                selectedRow = tr.Tag as DataRow;
            }
            catch
            {
                selectedRow = null;
            }
        }

        private void ExpandNode(TreeNodeCollection coll, TreeNode node)
        {
            if (Disposing || IsDisposed)
                return;
            try
            {
                for (int i = 0; i < coll.Count; ++i)
                    coll[i].Expand();
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        private void AddNode(TreeNodeCollection coll, TreeNode node)
        {
            if (Disposing || IsDisposed)
                return;
            try
            {
                Thread.CurrentThread.CurrentUICulture = Environment.CurCultureInfo;
                coll.Add(node);
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        private void AddNodes(TreeNodeCollection coll, TreeNode[] nodes, bool force)
        {
            if (Disposing || IsDisposed)
                return;
            try
            {
                Thread.CurrentThread.CurrentUICulture = Environment.CurCultureInfo;
                coll.AddRange(nodes);
                if (force)

                    nodes[0].Parent.Expand();
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        private DataRow selectedRow;

        private void treeLinks_MouseUp(object sender, MouseEventArgs e)
        {
            if (Disposing || IsDisposed)
                return;
            try
            {
                TreeNode tr = treeLinks.GetNodeAt(e.X, e.Y);
                if (tr != null)
                    if (tr.Tag != null && tr.Tag is DataRow)
                    {
                        selectedRow = (DataRow) tr.Tag;
                        OnTreeMouseUp(sender, e);
                    }
                    else
                        selectedRow = null;
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        private void treeLinks_KeyDown(object sender, KeyEventArgs e)
        {
            if (Disposing || IsDisposed)
                return;
            try
            {
                TreeNode tr = treeLinks.SelectedNode;
                if (tr != null)
                    if (tr.Tag != null && tr.Tag is DataRow)
                    {
                        selectedRow = (DataRow) tr.Tag;
                        OnTreeKeysUp(sender, e);
                    }
                    else
                        selectedRow = null;
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        private TreeNode CreateTreeNode(DataRow dr, bool slave, Color color)
        {
            if (Disposing || IsDisposed)
                return null;
            TreeNode tn = null;
            try
            {
                string nodeText = DBDocString.Format(dr);
                tn = new TreeNode(nodeText);
                if (docData != DateTime.MinValue && !dr.IsNull(Environment.DocData.DateField) &&
                    ((slave && docData < (DateTime) dr[Environment.DocData.DateField]) ||
                     (!slave && docData > (DateTime) dr[Environment.DocData.DateField])))
                {
                    tn.ImageIndex = 4;
                    tn.SelectedImageIndex = 4;
                }
                tn.ForeColor = color;
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
            return tn;
        }

        private void CreateTree(TreeNode rtn, DataRow[] dr, bool slave, Color color, int count, bool force, bool restore)
        {
            if (dr.Length <= 0) return;
            if (Disposing || IsDisposed)
                return;
            try
            {
                var nodes = new TreeNode[100];
                int j = 0;
                foreach (DataRow t in dr)
                {
                    TreeNode tn = CreateTreeNode(t, slave, color);
                    if (tn != null)
                    {
                        tn.Tag = t;

                        if (!Disposing)
                        {
                            if (j < 100)
                            {
                                nodes[j] = tn;
                                ++j;
                            }
                            else
                            {
                                j = 1;
                                AddNodes(rtn.Nodes, nodes, force);

                                nodes = new TreeNode[100];
                                nodes[0] = tn;
                            }
                        }
                    }
                }
                if (j != 0)
                {
                    var endNodes = new TreeNode[j];
                    for (int i = 0; i < j; ++i)
                        endNodes[i] = nodes[i];
                    AddNodes(rtn.Nodes, endNodes, force);
                }
                else if (nodes[0] != null)
                {
                    rtn.Nodes.Add(nodes[0]);
                    if (force)
                        nodes[0].Parent.Expand();
                }
                if (restore)
                {
                    Cursor = Cursors.Default;
                    treeLinks.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        private void treeLinks_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Expand)
            {
                TreeNode tr = e.Node;
                if (tr.Tag is int)
                {
                    if (!tr.IsExpanded)
                    {
                        treeLinks.BeginUpdate();
                        var fieldID = (int) tr.Tag;
                        tr.Tag = null;
                        if (fieldID > 0)
                        {
                            DataTable dotherTable = Environment.DocLinksData.GetMainLinksData(docID, fieldID).Tables[0];
                            DataRow[] dr = dotherTable.Select();
                            CreateTree(tr, dr, true, Color.Violet, 0, true, false);
                        }
                        else
                        {
                            DataTable dotherTable =
                                Environment.DocLinksData.GetChildLinksData(docID, -fieldID).Tables[0];
                            DataRow[] dr = dotherTable.Select();
                            CreateTree(tr, dr, false, Color.RoyalBlue, 0, true, false);
                        }
                        tr.Nodes.RemoveAt(0);
                        treeLinks.EndUpdate();
                    }
                }
                else if (tr.Tag is KeyValuePair<int, string>)
                    if (!tr.IsExpanded && tr.Nodes[0].Tag is int)
                    {
                        treeLinks.BeginUpdate();
                        int typeID = ((KeyValuePair<int, string>) tr.Tag).Key;
                        tr.Tag = null;
                        if (typeID > 0)
                        {
                            var val =
                                MainLinkSet.Tables[1].Rows.Cast<DataRow>().Where(
                                    x => typeID.Equals(x[Environment.DocTypeData.IDField]));
                            DataRow[] dr = null;
                            if (sort == 0)
                                dr = val.ToArray();
                            else if (sort == 1)
                                dr =
                                    val.OrderBy(
                                        x =>
                                        (x[Environment.DocData.DateField] is DateTime)
                                            ? (DateTime) x[Environment.DocData.DateField]
                                            : DateTime.MinValue).ToArray();
                            else
                                dr =
                                    val.OrderByDescending(
                                        x =>
                                        (x[Environment.DocData.DateField] is DateTime)
                                            ? (DateTime) x[Environment.DocData.DateField]
                                            : DateTime.MinValue).ToArray();
                            CreateTree(tr, dr, true, Color.Violet, 0, true, false);
                        }
                        else
                        {
                            var val =
                                ChildLinkSet.Tables[1].Rows.Cast<DataRow>().Where(
                                    x => (-typeID).Equals(x[Environment.DocTypeData.IDField]));
                            DataRow[] dr = null;
                            if (sort == 0)
                                dr = val.ToArray();
                            else if (sort == 1)
                                dr =
                                    val.OrderBy(
                                        x =>
                                        (x[Environment.DocData.DateField] is DateTime)
                                            ? (DateTime) x[Environment.DocData.DateField]
                                            : DateTime.MinValue).ToArray();
                            else
                                dr =
                                    val.OrderByDescending(
                                        x =>
                                        (x[Environment.DocData.DateField] is DateTime)
                                            ? (DateTime) x[Environment.DocData.DateField]
                                            : DateTime.MinValue).ToArray();
                            CreateTree(tr, dr, false, Color.RoyalBlue, 0, true, false);
                        }
                        tr.Nodes.RemoveAt(0);
                        treeLinks.EndUpdate();
                    }
            }
        }

        private void treeLinks_VisibleChanged(object sender, EventArgs e)
        {
            if (treeLinks != null && treeLinks.Visible && treeLinks.Nodes.Count > 0)
                for (int i = 0; i < treeLinks.Nodes.Count; ++i)
                    treeLinks.Nodes[i].Expand();
        }

        #endregion

        private void treeLinks_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                // Get the node at the current mouse pointer location.
                TreeNode theNode = treeLinks.GetNodeAt(e.X, e.Y);

                // Set a ToolTip only if the mouse pointer is actually paused on a node.
                if (theNode != null && e.X > 0 && theNode.Tag != null && theNode.Tag is DataRow)
                {
                    IntPtr testID = theNode.Handle;
                    if (!testID.Equals( tooltipShownID))
                    {
                        if (!toolTip.Active)
                        {
                            toolTip.Active = true;
                        }
                        tooltipShownID = testID;
                        var dr = theNode.Tag as DataRow;
                        if (theNode.ImageIndex == 4)
                        {
                            toolTip.Show(
                                Environment.StringResources.GetString("Control_MenuList_ToolTip") +
                                System.Environment.NewLine +
                                dr[Environment.DocLinksData.EmpField] + System.Environment.NewLine +
                                ((DateTime) dr[Environment.DocLinksData.EditedField]).ToString(), this,
                                treeLinks.Left + theNode.Bounds.Left, treeLinks.Top + theNode.Bounds.Bottom);
                        }
                        else
                        {
                            toolTip.Show(
                                dr[Environment.DocLinksData.EmpField] + System.Environment.NewLine +
                                ((DateTime) dr[Environment.DocLinksData.EditedField]).ToString(), this,
                                treeLinks.Left + theNode.Bounds.Left, treeLinks.Top + theNode.Bounds.Bottom);
                        }
                    }
                }
                else // Pointer is not over a node so clear the ToolTip.
                {
                    tooltipShownID = IntPtr.Zero;
                    if (toolTip.Active)
                        toolTip.Active = false;
                }
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        private void treeLinks_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                tooltipShownID = IntPtr.Zero;
                if (toolTip.Active)
                    toolTip.Active = false;
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }
    }

    public delegate void LinkDeleted(int parentDocID, int childDocID);
}