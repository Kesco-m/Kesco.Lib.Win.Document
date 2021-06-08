using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Checkers;
using Kesco.Lib.Win.Document.Items;
using Kesco.Lib.Win.Document.Objects;
using Kesco.Lib.Win.Document.Select;
using Kesco.Lib.Win.Error;
using Kesco.Lib.Win.ImageControl;
using Kesco.Lib.Win.Web;

namespace Kesco.Lib.Win.Document.Controls
{
	/// <summary>
	///   Контрол для отображения документа.
	/// </summary>
	public class DocControl : UserControl
	{
		#region My parameter

		public delegate bool CheckLinkDocMethod(int parentID, int childID);

		private string fileName;
		private int curDocID;
		private int imgID = -1;
		private int page;
		private bool isArchive;
		public static bool saving;
		private bool showToolBar;
		private bool newEForm;
		private bool addEForm;

		private bool showWebPanel;
		private bool hasImage;
		private bool hasData;

		private bool canPrintEForm;
		private bool compliteLoading;
		private ServerInfo curServer;

		private Guid subscribe = Guid.Empty;

		private object lockObj = new object();

		internal static int templateDocTypeID = -1;
		public Components.DocControlComponent docComponent = new Components.DocControlComponent();

		// инфа по открытым диалогам сохранения части
		private static SynchronizedCollection<KeyValuePair<string, Dialogs.SavePartDialog>> savePartTable;

		private System.Threading.Timer timer;
		private System.Timers.Timer webTimer;
		private System.Timers.Timer transTimer;
		private const int webTimerInterval = 2000;
		private const int transTimerInterval = 3000;
		private Receive.Receive rcv;

		private delegate void ImageIndexChangedDelegate(VariantListItem item, int index);

		private bool force;
		private bool sendMail;
		private SynchronizedCollection<int> parentDocIDs;
		private SynchronizedCollection<int> childDocIDs;
		private string sendString;

		private const int FileChanged_PAUSE = 3000;
		private FileSystemWatcher _watcher = new FileSystemWatcher();
		private System.Timers.Timer reloadTm = new System.Timers.Timer(FileChanged_PAUSE);

		#endregion

		private Panel toolBarPanel;
		private ImageList varImageList;
		private ImageList toolBarImageList;
		private FileSystemWatcher faxWatcher;
		private ImageShowControl image;
		private SplitContainer splitContainerWhole;
		private SplitContainer splitContainerDocument;
		private SplitContainer splitContainerBrowser;
		private ListView variantList;
		private ExtendedBrowserControl browser;
		private Panel panelMain;
		private TransactionControl transactionControl;
		private ExtendedBrowserControl exportBrowser;
		private ExtendedBrowserControl printBrowser;
		private ToolStrip toolStrip;
		private ToolStripButton toolStripThumb;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripButton toolStripPrevPage;
		private ToolStripTextBox toolStripPage;
		private ToolStripButton toolStripNextPage;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripComboBox toolStripComboZoom;
		private ToolStripButton toolStripZoomIn;
		private ToolStripButton toolStripZoomOut;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripButton toolStripRotateLeft;
		private ToolStripButton toolStripRotateRight;
		private ToolStripSeparator toolStripSeparator4;
		private ToolStripButton toolStripPrint;
		private ContextMenuStrip contextMenuStripBrowser;
		private ToolStripMenuItem toolStripMenuItemRefresh;
		private ToolStripMenuItem toolStripMenuItemText;
		private ToolStripMenuItem toolStripMenuItemProperties;
		private ToolStripMenuItem ToolStripMenuItemCut;
		private ToolStripMenuItem ToolStripMenuItemCopy;
		private ToolStripMenuItem ToolStripMenuItemPaste;
		private ToolStripMenuItem ToolStripMenuItemSelectAll;
		private SplitContainer splitContainerDocumentSign;
		private SignDocumentPanel signDocumentPanel;
		public Kesco.Lib.Win.Document.Controls.PdfViewControl.PDFView imagePDF;

		private IContainer components;

		#region Events

		public event SplitterEventHandler SplitterPlaceChanged;

		protected internal void OnSplitterChange(object sender, SplitterEventArgs e)
		{
			if(SplitterPlaceChanged != null)
				SplitterPlaceChanged(sender, e);
		}

		public event MouseEventHandler VarListMouseUp;

		protected internal void OnVarListMouseUp(object sender, MouseEventArgs e)
		{
			if(VarListMouseUp != null)
				VarListMouseUp(sender, e);
		}

		public event EventHandler VarListIndexChange;

		protected internal void OnVarListIndexChange(object sender)
		{
			if(VarListIndexChange != null)
				VarListIndexChange(sender, EventArgs.Empty);
		}

		public event EventHandler PageChanged;

		protected internal void OnPageChange(object sender, EventArgs e)
		{
			try
			{
				if(PageChanged != null)
					PageChanged(sender, e);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, "Error occured while calling PageChanged event handler");
			}

			string newText = string.Empty;
			try
			{
				if(image != null && image.ImageDisplayed)
					newText = image.Page.ToString();
				else if(imagePDF != null && imagePDF.ImageDisplayed)
					newText = imagePDF.Page.ToString();

				if(toolStrip.InvokeRequired)
					toolStrip.Invoke((MethodInvoker)(delegate
					{
                        // Заявка №27393
                        // FIX System.NullReferenceException
					    if (!IsDisposed && !Disposing)
                            toolStripPage.Text = newText;
					}));
				else
					toolStripPage.Text = newText;
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, string.Format("newText = {1}", newText));
			}
		}

		public event Components.DocumentSavedEventHandle DocChanged;

		protected internal void OnDocChanged(object sender, Components.DocumentSavedEventArgs e)
		{
			if(DocChanged != null)
				DocChanged(sender, e);
		}

		public event Components.DocumentSavedEventHandle FaxInSave;

		protected internal void OnFaxInSave(object sender, Components.DocumentSavedEventArgs e)
		{
			if(FaxInSave != null)
				FaxInSave(sender, e);
		}

		public event EventHandler MarkEnd;

		protected internal void OnMarkEnd(object sender, MarkEndEventArgs e)
		{
			if(MarkEnd != null)
				MarkEnd(sender, e);
		}

		public event EventHandler NeedSave;

		protected internal void OnNeedSave()
		{
			if(NeedSave != null)
				NeedSave(this, EventArgs.Empty);
		}

		public event RenamedEventHandler FileChanged;

		protected internal void OnFileChanged(RenamedEventArgs args)
		{
			if(FileChanged != null)
				FileChanged(this, args);
		}

		protected internal void OnFileChanged(FileSystemEventArgs args)
		{
			var rargs = new RenamedEventArgs(args.ChangeType, args.FullPath, args.Name, args.Name);
			if(FileChanged != null)
				FileChanged(this, rargs);
		}

		// Событие добавления связи
		public event Components.LinkDocEventHandler LinkDoc;

		internal void OnLinkDoc(int docID)
		{
			if(LinkDoc != null)
				LinkDoc(this, new Components.LinkDocEventArgs(docID));
		}

		// Событие загрузки данных по документу
		public event EventHandler LoadComplete;

		internal void OnLoadComplete()
		{
			try
			{
				if(curDocID > 0)
					compliteLoading = true;
				if(LoadComplete != null)
					LoadComplete(this, new EventArgs());

				if(File.Exists(fileName))
				{
                    Console.WriteLine("{0}: OnLoadComplete.SelectTool", DateTime.Now.ToString("HH:mm:ss fff"));
					SelectionMode = false;
					AnnotationDraw = false;
					SelectTool(0);
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		public event Components.FaxInContactCreatedEventHandle NeedRefreshGrid;

		protected internal void OnNeedRefreshGrid(object sender, Components.NogeIdEventArgs e)
		{
			if(NeedRefreshGrid != null)
				NeedRefreshGrid(sender, e);
		}


		public event ImageControl.ImageControl.ToolSelectedHandler ToolSelected;

		internal void OnToolSelected(ImageControl.ImageControl.ToolSelectedEventArgs e)
		{
			if(ToolSelected != null)
				ToolSelected.DynamicInvoke(new object[] { this, e });
		}

		public event EventHandler ImageSigned;

		internal void OnImageSigned()
		{
			if(ImageSigned != null)
				ImageSigned(this, EventArgs.Empty);
		}

		public void OnDocSaved(object sender, Components.DocumentSavedEventArgs e)
		{
			if(e.IsFax)
				OnFaxInSave(sender, e);
			else
				OnDocChanged(sender, e);
		}

		#endregion

		public DocControl()
		{
			try
			{
				// This call is required by the Windows.Forms Form Designer
				InitializeComponent();
				if(DesignMode)
					return;
				Environment.Init();
				splitContainerWhole.SplitterDistance = Environment.Layout.LoadIntOption("SpliterImagePlace", splitContainerWhole.SplitterDistance);
				splitContainerDocument.SplitterDistance = Environment.Layout.LoadIntOption("SpliterDataPlace", splitContainerDocument.SplitterDistance);
				splitContainerBrowser.SplitterDistance = Environment.Layout.LoadIntOption("SplitterExportPlace", splitContainerBrowser.SplitterDistance);
				variantList.Height = splitContainerWhole.SplitterDistance;
				ClearDoc();
				if(Environment.CurEmp != null)
					image.SetCurrentAnnotationGroup(Environment.CurEmp.ShortName);

				signDocumentPanel.NeedRefresh += signDocumentPanel_NeedRefresh;
				signDocumentPanel.MinSizeChanged += signDocumentPanel_MinSizeChanged;
				signDocumentPanel.AddingDocSign += signDocumentPanel_AddingDocSign;
				signDocumentPanel.RemovingDocSign += signDocumentPanel_RemovingDocSign;
				signDocumentPanel.NeedRefreshStamp += signDocumentPanel_NeedRefreshStamp;
				signDocumentPanel.AddedDocSign += signDocumentPanel_edDocSign;
				signDocumentPanel.RemovedDocSign += signDocumentPanel_edDocSign;

				_watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;

				reloadTm.AutoReset = false;
				reloadTm.Enabled = false;
				reloadTm.Elapsed += reloadTm_Elapsed;
			}
			catch(Exception ex)
			{
				ErrorShower.OnShowError(this, ex.Message, "");
			}
		}

		private void signDocumentPanel_edDocSign(object sender, Components.DocumentSavedEventArgs e)
		{
			OnImageSigned();
			OnDocChanged(sender, e);
		}

		private void signDocumentPanel_NeedRefreshStamp()
		{
			if(image.ImageDisplayed)
				image.ReloadStamps();
			else if(imagePDF.ImageDisplayed)
				imagePDF.ReloadStamps();
		}

		private void signDocumentPanel_RemovingDocSign()
		{
			if(imgID > 0)
				TestImage();
		}

		private bool signDocumentPanel_AddingDocSign(int docID, int imageID)
		{
			if(this.imgID == imageID)
				TestImage();
			bool ret = docID == curDocID && ((imageID == -3)?0:imageID) == this.imgID;
			if(!ret)
				Data.Env.WriteToLog(string.Format("Не совпадают данные контрола подписей и отображения документа!\n Данные контрола подписей: код документа = {0}, код изображения = {1}\n" +
			"Данные контрола изображения: код документа = {2}, код изображения = {3}\n",docID, imageID, curDocID,imgID));
			return ret;
		}

		private void splitContainerDocumentSign_SizeChanged(object sender, EventArgs e)
		{
			SetSignSplitterDistance(false);
		}

		private void signDocumentPanel_MinSizeChanged(int obj)
		{
			SetSignSplitterDistance(true);
		}

		private void splitContainerDocumentSign_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			SetSignSplitterDistance(true);
		}

		private bool showAllWidthSign = true;

		private void SetSignSplitterDistance(bool force)
		{
			if(splitContainerDocumentSign.ClientSize.Width >
				splitContainerDocumentSign.Panel1MinSize + signDocumentPanel.MinWidth)
			//Панель с подписями может поместится вся            
			{
				if(force || !showAllWidthSign)
				{
					splitContainerDocumentSign.Panel2MinSize = signDocumentPanel.MinWidth;
					splitContainerDocumentSign.SplitterDistance = splitContainerDocumentSign.ClientSize.Width -
																  signDocumentPanel.MinWidth;

					if(splitContainerDocumentSign.FixedPanel != FixedPanel.Panel2)
						splitContainerDocumentSign.FixedPanel = FixedPanel.Panel2;

					showAllWidthSign = true;
				}
			}
			else //Панель с подписями надо показывать частично
			{
				if(force || showAllWidthSign)
				{
					splitContainerDocumentSign.Panel2MinSize = 0;

					splitContainerDocumentSign.SplitterDistance = splitContainerDocumentSign.Panel1MinSize;

					if(splitContainerDocumentSign.FixedPanel == FixedPanel.Panel2)
						splitContainerDocumentSign.FixedPanel = FixedPanel.None;

					showAllWidthSign = false;
				}
			}
		}

		private void signDocumentPanel_NeedRefresh()
		{
			if(hasData)
				BrowserRefresh(false);
		}

		/// <summary>
		///   Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				variantList.SelectedIndexChanged -= variantList_SelectedIndexChanged;
				if(timer != null)
				{
					//timer.Elapsed -= timer_Elapsed;
					timer.Dispose();
					timer = null;
				}
				if(webTimer != null)
				{
					webTimer.Elapsed -= webTimer_Elapsed;
					webTimer.Stop();
					webTimer.Dispose();
					webTimer = null;
				}
				DeleteReceiver();
				if(components != null)
				{
					components.Dispose();
				}
			}
			if(imagePDF != null)
				imagePDF.FileName = "";
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		///   Required method for Designer support - do not modify the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocControl));
            this.splitContainerWhole = new System.Windows.Forms.SplitContainer();
            this.splitContainerDocumentSign = new System.Windows.Forms.SplitContainer();
            this.variantList = new System.Windows.Forms.ListView();
            this.varImageList = new System.Windows.Forms.ImageList(this.components);
            this.signDocumentPanel = new Kesco.Lib.Win.Document.Controls.SignDocumentPanel();
            this.splitContainerDocument = new System.Windows.Forms.SplitContainer();
            this.imagePDF = new Kesco.Lib.Win.Document.Controls.PdfViewControl.PDFView();
            this.image = new Kesco.Lib.Win.Document.Controls.ImageShowControl();
            this.splitContainerBrowser = new System.Windows.Forms.SplitContainer();
            this.browser = new Kesco.Lib.Win.Web.ExtendedBrowserControl();
            this.contextMenuStripBrowser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemCut = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemText = new System.Windows.Forms.ToolStripMenuItem();
            this.exportBrowser = new Kesco.Lib.Win.Web.ExtendedBrowserControl();
            this.ToolStripMenuItemSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBarPanel = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolBarImageList = new System.Windows.Forms.ImageList(this.components);
            this.toolStripThumb = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripPrevPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripPage = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripNextPage = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboZoom = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripZoomOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripRotateLeft = new System.Windows.Forms.ToolStripButton();
            this.toolStripRotateRight = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripPrint = new System.Windows.Forms.ToolStripButton();
            this.panelMain = new System.Windows.Forms.Panel();
            this.printBrowser = new Kesco.Lib.Win.Web.ExtendedBrowserControl();
            this.transactionControl = new Kesco.Lib.Win.Document.Controls.TransactionControl();
            this.faxWatcher = new System.IO.FileSystemWatcher();
            this.splitContainerWhole.Panel1.SuspendLayout();
            this.splitContainerWhole.Panel2.SuspendLayout();
            this.splitContainerWhole.SuspendLayout();
            this.splitContainerDocumentSign.Panel1.SuspendLayout();
            this.splitContainerDocumentSign.Panel2.SuspendLayout();
            this.splitContainerDocumentSign.SuspendLayout();
            this.splitContainerDocument.Panel1.SuspendLayout();
            this.splitContainerDocument.Panel2.SuspendLayout();
            this.splitContainerDocument.SuspendLayout();
            this.splitContainerBrowser.Panel1.SuspendLayout();
            this.splitContainerBrowser.Panel2.SuspendLayout();
            this.splitContainerBrowser.SuspendLayout();
            this.contextMenuStripBrowser.SuspendLayout();
            this.toolBarPanel.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.faxWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerWhole
            // 
            resources.ApplyResources(this.splitContainerWhole, "splitContainerWhole");
            this.splitContainerWhole.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerWhole.Name = "splitContainerWhole";
            // 
            // splitContainerWhole.Panel1
            // 
            this.splitContainerWhole.Panel1.Controls.Add(this.splitContainerDocumentSign);
            this.splitContainerWhole.Panel1Collapsed = true;
            // 
            // splitContainerWhole.Panel2
            // 
            this.splitContainerWhole.Panel2.Controls.Add(this.splitContainerDocument);
            this.splitContainerWhole.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerWhole_SplitterMoved);
            // 
            // splitContainerDocumentSign
            // 
            resources.ApplyResources(this.splitContainerDocumentSign, "splitContainerDocumentSign");
            this.splitContainerDocumentSign.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerDocumentSign.Name = "splitContainerDocumentSign";
            // 
            // splitContainerDocumentSign.Panel1
            // 
            this.splitContainerDocumentSign.Panel1.Controls.Add(this.variantList);
            // 
            // splitContainerDocumentSign.Panel2
            // 
            this.splitContainerDocumentSign.Panel2.Controls.Add(this.signDocumentPanel);
            this.splitContainerDocumentSign.Panel2Collapsed = true;
            this.splitContainerDocumentSign.SizeChanged += new System.EventHandler(this.splitContainerDocumentSign_SizeChanged);
            this.splitContainerDocumentSign.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitContainerDocumentSign_MouseDoubleClick);
            // 
            // variantList
            // 
            resources.ApplyResources(this.variantList, "variantList");
            this.variantList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.variantList.HideSelection = false;
            this.variantList.LargeImageList = this.varImageList;
            this.variantList.MultiSelect = false;
            this.variantList.Name = "variantList";
            this.variantList.SmallImageList = this.varImageList;
            this.variantList.StateImageList = this.varImageList;
            this.variantList.UseCompatibleStateImageBehavior = false;
            this.variantList.SelectedIndexChanged += new System.EventHandler(this.variantList_SelectedIndexChanged);
            this.variantList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.variantList_MouseUp);
            // 
            // varImageList
            // 
            this.varImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("varImageList.ImageStream")));
            this.varImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.varImageList.Images.SetKeyName(0, "");
            this.varImageList.Images.SetKeyName(1, "");
            this.varImageList.Images.SetKeyName(2, "");
            this.varImageList.Images.SetKeyName(3, "");
            this.varImageList.Images.SetKeyName(4, "DocPrint.gif");
            this.varImageList.Images.SetKeyName(5, "DocMainPrint.gif");
            this.varImageList.Images.SetKeyName(6, "DocOrigPrint.gif");
            this.varImageList.Images.SetKeyName(7, "DocMainOrigPrint.gif");
            this.varImageList.Images.SetKeyName(8, "");
            this.varImageList.Images.SetKeyName(9, "");
            this.varImageList.Images.SetKeyName(10, "");
            this.varImageList.Images.SetKeyName(11, "");
            this.varImageList.Images.SetKeyName(12, "");
            this.varImageList.Images.SetKeyName(13, "");
            this.varImageList.Images.SetKeyName(14, "");
            this.varImageList.Images.SetKeyName(15, "");
            this.varImageList.Images.SetKeyName(16, "");
            this.varImageList.Images.SetKeyName(17, "DocPDF.gif");
            this.varImageList.Images.SetKeyName(18, "DocMainPDF.gif");
            this.varImageList.Images.SetKeyName(19, "DocPDFOrig.gif");
            this.varImageList.Images.SetKeyName(20, "DocMainPDFOrig.gif");
            // 
            // signDocumentPanel
            // 
            this.signDocumentPanel.BackColor = System.Drawing.Color.White;
            this.signDocumentPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.signDocumentPanel, "signDocumentPanel");
            this.signDocumentPanel.Name = "signDocumentPanel";
            // 
            // splitContainerDocument
            // 
            resources.ApplyResources(this.splitContainerDocument, "splitContainerDocument");
            this.splitContainerDocument.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerDocument.Name = "splitContainerDocument";
            // 
            // splitContainerDocument.Panel1
            // 
            this.splitContainerDocument.Panel1.Controls.Add(this.imagePDF);
            this.splitContainerDocument.Panel1.Controls.Add(this.image);
            // 
            // splitContainerDocument.Panel2
            // 
            this.splitContainerDocument.Panel2.Controls.Add(this.splitContainerBrowser);
            this.splitContainerDocument.Panel2Collapsed = true;
            this.splitContainerDocument.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerDocument_SplitterMoved);
            // 
            // imagePDF
            // 
            this.imagePDF.CurrentStamp = null;
            this.imagePDF.CurrentStampID = 0;
            resources.ApplyResources(this.imagePDF, "imagePDF");
            this.imagePDF.FileName = "";
            this.imagePDF.ForceReplicate = false;
            this.imagePDF.ImageID = 0;
            this.imagePDF.IsEditNotes = false;
            this.imagePDF.IsMoveImage = false;
            this.imagePDF.IsSelectionMode = false;
            this.imagePDF.Name = "imagePDF";
            this.imagePDF.NeedPreview = true;
            this.imagePDF.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.imagePDF.Page = 1;
            this.imagePDF.ScrollPositionX = 0;
            this.imagePDF.ScrollPositionY = 0;
            this.imagePDF.SelectionMode = false;
            this.imagePDF.ShowThumbPanel = true;
            this.imagePDF.SplinterPlace = 485;
            this.imagePDF.TabStop = false;
            this.imagePDF.UseLock = false;
            this.imagePDF.FileNameChanged += new Kesco.Lib.Win.ImageControl.ImageControl.FileNameChangedHandler(this.image_FileNameChanged);
            this.imagePDF.PageChanged += new System.EventHandler(this.image_PageChanged);
            this.imagePDF.ImageLoad += new System.EventHandler(this.image_ImageLoad);
            // 
            // image
            // 
            this.image.AnnotationDraw = false;
            this.image.CurrentStamp = null;
            this.image.CurrentStampID = 0;
            this.image.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.image, "image");
            this.image.DPI = 96F;
            this.image.ForceReplicate = false;
            this.image.Image = null;
            this.image.ImageID = 0;
            this.image.ImageResolutionX = 1;
            this.image.ImageResolutionY = 1;
            this.image.IsAnnuled = false;
            this.image.IsCorrectScaleDrawThumbnailPanel = true;
            this.image.IsEditNotes = false;
            this.image.IsMoveImage = true;
            this.image.IsSelectionMode = false;
            this.image.IsVerifyFile = true;
            this.image.Name = "image";
            this.image.Page = 0;
            this.image.SaveStampsInternal = true;
            this.image.ScrollPositionX = 0;
            this.image.ScrollPositionY = 0;
            this.image.ShowThumbPanel = false;
            this.image.SplinterPlace = 200;
            this.image.ThumbnailPanelOrientation = Kesco.Lib.Win.ImageControl.ImageControl.TypeThumbnailPanelOrientation.Left;
            this.image.TypeWorkThumbnailImagesPanel = 3;
            this.image.UseLock = false;
            this.image.Zoom = 100;
            this.image.ScanComplete += new Kesco.Lib.Win.ImageControl.ImageControl.ScanCompleteHandler(this.image_ScanComplete);
            this.image.FileNameChanged += new Kesco.Lib.Win.ImageControl.ImageControl.FileNameChangedHandler(this.image_FileNameChanged);
            this.image.SplinterPlaceChanged += new System.Windows.Forms.SplitterEventHandler(this.image_SplinterPlaceChanged);
            this.image.PageChanged += new System.EventHandler(this.image_PageChanged);
            this.image.NeedSave += new Kesco.Lib.Win.ImageControl.SaveEventHandler(this.image_NeedSave);
            this.image.MarkEnd += new Kesco.Lib.Win.ImageControl.ImageControl.MarkEndEventHandler(this.image_MarkEnd);
            this.image.ImageLoad += new System.EventHandler(this.image_ImageLoad);
            this.image.ToolSelected += new Kesco.Lib.Win.ImageControl.ImageControl.ToolSelectedHandler(this.image_ToolSelected);
            // 
            // splitContainerBrowser
            // 
            resources.ApplyResources(this.splitContainerBrowser, "splitContainerBrowser");
            this.splitContainerBrowser.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerBrowser.Name = "splitContainerBrowser";
            // 
            // splitContainerBrowser.Panel1
            // 
            this.splitContainerBrowser.Panel1.Controls.Add(this.browser);
            // 
            // splitContainerBrowser.Panel2
            // 
            this.splitContainerBrowser.Panel2.Controls.Add(this.exportBrowser);
            this.splitContainerBrowser.Panel2Collapsed = true;
            this.splitContainerBrowser.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerBrowser_SplitterMoved);
            // 
            // browser
            // 
            this.browser.AllowWebBrowserDrop = false;
            this.browser.ContextMenuStrip = this.contextMenuStripBrowser;
            resources.ApplyResources(this.browser, "browser");
            this.browser.EnableInternalReloader = true;
			this.browser.IsWebBrowserContextMenuEnabled = false;
			this.browser.PreviewKeyDown += new PreviewKeyDownEventHandler(browser_PreviewKeyDown);
            this.browser.Name = "browser";
            this.browser.SelfNavigate = false;
            this.browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.browser_DocumentCompleted);
            // 
            // contextMenuStripBrowser
            // 
            this.contextMenuStripBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemCut,
            this.ToolStripMenuItemCopy,
            this.ToolStripMenuItemPaste,
            this.toolStripMenuItemRefresh,
            this.toolStripMenuItemText});
            this.contextMenuStripBrowser.Name = "contextMenuStripBrowser";
            this.contextMenuStripBrowser.ShowImageMargin = false;
            resources.ApplyResources(this.contextMenuStripBrowser, "contextMenuStripBrowser");
            this.contextMenuStripBrowser.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripBrowser_Opening);
            // 
            // ToolStripMenuItemCut
            // 
            this.ToolStripMenuItemCut.Name = "ToolStripMenuItemCut";
            resources.ApplyResources(this.ToolStripMenuItemCut, "ToolStripMenuItemCut");
            // 
            // ToolStripMenuItemCopy
            // 
            this.ToolStripMenuItemCopy.Name = "ToolStripMenuItemCopy";
            resources.ApplyResources(this.ToolStripMenuItemCopy, "ToolStripMenuItemCopy");
            this.ToolStripMenuItemCopy.Click += new System.EventHandler(this.ToolStripMenuItemCopy_Click);
            // 
            // ToolStripMenuItemPaste
            // 
            this.ToolStripMenuItemPaste.Name = "ToolStripMenuItemPaste";
            resources.ApplyResources(this.ToolStripMenuItemPaste, "ToolStripMenuItemPaste");
            // 
            // toolStripMenuItemRefresh
            // 
            this.toolStripMenuItemRefresh.Name = "toolStripMenuItemRefresh";
            resources.ApplyResources(this.toolStripMenuItemRefresh, "toolStripMenuItemRefresh");
            this.toolStripMenuItemRefresh.Click += new System.EventHandler(this.toolStripMenuItemRefresh_Click);
            // 
            // toolStripMenuItemText
            // 
            this.toolStripMenuItemText.Name = "toolStripMenuItemText";
            resources.ApplyResources(this.toolStripMenuItemText, "toolStripMenuItemText");
            this.toolStripMenuItemText.Click += new System.EventHandler(this.toolStripMenuItemText_Click);
            // 
            // exportBrowser
            // 
            this.exportBrowser.AllowWebBrowserDrop = false;
            resources.ApplyResources(this.exportBrowser, "exportBrowser");
            this.exportBrowser.EnableInternalReloader = true;
            this.exportBrowser.Name = "exportBrowser";
            this.exportBrowser.ScrollBarsEnabled = false;
            this.exportBrowser.SelfNavigate = false;
            // 
            // ToolStripMenuItemSelectAll
            // 
            this.ToolStripMenuItemSelectAll.Name = "ToolStripMenuItemSelectAll";
            resources.ApplyResources(this.ToolStripMenuItemSelectAll, "ToolStripMenuItemSelectAll");
            this.ToolStripMenuItemSelectAll.Click += new System.EventHandler(this.ToolStripMenuItemSelectAll_Click);
            // 
            // toolStripMenuItemProperties
            // 
            this.toolStripMenuItemProperties.Name = "toolStripMenuItemProperties";
            resources.ApplyResources(this.toolStripMenuItemProperties, "toolStripMenuItemProperties");
            this.toolStripMenuItemProperties.Click += new System.EventHandler(this.toolStripMenuItemProperties_Click);
            // 
            // toolBarPanel
            // 
            this.toolBarPanel.Controls.Add(this.toolStrip);
            resources.ApplyResources(this.toolBarPanel, "toolBarPanel");
            this.toolBarPanel.Name = "toolBarPanel";
            // 
            // toolStrip
            // 
            this.toolStrip.ImageList = this.toolBarImageList;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripThumb,
            this.toolStripSeparator1,
            this.toolStripPrevPage,
            this.toolStripPage,
            this.toolStripNextPage,
            this.toolStripSeparator2,
            this.toolStripComboZoom,
            this.toolStripZoomIn,
            this.toolStripZoomOut,
            this.toolStripSeparator3,
            this.toolStripRotateLeft,
            this.toolStripRotateRight,
            this.toolStripSeparator4,
            this.toolStripPrint});
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // toolBarImageList
            // 
            this.toolBarImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolBarImageList.ImageStream")));
            this.toolBarImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.toolBarImageList.Images.SetKeyName(0, "");
            this.toolBarImageList.Images.SetKeyName(1, "");
            this.toolBarImageList.Images.SetKeyName(2, "");
            this.toolBarImageList.Images.SetKeyName(3, "");
            this.toolBarImageList.Images.SetKeyName(4, "");
            this.toolBarImageList.Images.SetKeyName(5, "");
            this.toolBarImageList.Images.SetKeyName(6, "");
            this.toolBarImageList.Images.SetKeyName(7, "");
            // 
            // toolStripThumb
            // 
            this.toolStripThumb.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripThumb, "toolStripThumb");
            this.toolStripThumb.Name = "toolStripThumb";
            this.toolStripThumb.Click += new System.EventHandler(this.toolStripThumb_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripPrevPage
            // 
            this.toolStripPrevPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripPrevPage, "toolStripPrevPage");
            this.toolStripPrevPage.Name = "toolStripPrevPage";
            this.toolStripPrevPage.Click += new System.EventHandler(this.toolStripPrevPage_Click);
            // 
            // toolStripPage
            // 
            this.toolStripPage.Name = "toolStripPage";
            resources.ApplyResources(this.toolStripPage, "toolStripPage");
            this.toolStripPage.TextChanged += new System.EventHandler(this.toolStripPage_TextChanged);
            // 
            // toolStripNextPage
            // 
            this.toolStripNextPage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripNextPage, "toolStripNextPage");
            this.toolStripNextPage.Name = "toolStripNextPage";
            this.toolStripNextPage.Click += new System.EventHandler(this.toolStripNextPage_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripComboZoom
            // 
            this.toolStripComboZoom.Items.AddRange(new object[] {
            resources.GetString("toolStripComboZoom.Items"),
            resources.GetString("toolStripComboZoom.Items1"),
            resources.GetString("toolStripComboZoom.Items2"),
            resources.GetString("toolStripComboZoom.Items3"),
            resources.GetString("toolStripComboZoom.Items4"),
            resources.GetString("toolStripComboZoom.Items5"),
            resources.GetString("toolStripComboZoom.Items6"),
            resources.GetString("toolStripComboZoom.Items7"),
            resources.GetString("toolStripComboZoom.Items8")});
            this.toolStripComboZoom.Name = "toolStripComboZoom";
            resources.ApplyResources(this.toolStripComboZoom, "toolStripComboZoom");
            this.toolStripComboZoom.TextChanged += new System.EventHandler(this.toolStripComboZoom_TextChanged);
            // 
            // toolStripZoomIn
            // 
            this.toolStripZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripZoomIn, "toolStripZoomIn");
            this.toolStripZoomIn.Name = "toolStripZoomIn";
            this.toolStripZoomIn.Click += new System.EventHandler(this.toolStripZoomIn_Click);
            // 
            // toolStripZoomOut
            // 
            this.toolStripZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripZoomOut, "toolStripZoomOut");
            this.toolStripZoomOut.Name = "toolStripZoomOut";
            this.toolStripZoomOut.Click += new System.EventHandler(this.toolStripZoomOut_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolStripRotateLeft
            // 
            this.toolStripRotateLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripRotateLeft, "toolStripRotateLeft");
            this.toolStripRotateLeft.Name = "toolStripRotateLeft";
            this.toolStripRotateLeft.Click += new System.EventHandler(this.toolStripRotateLeft_Click);
            // 
            // toolStripRotateRight
            // 
            this.toolStripRotateRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripRotateRight, "toolStripRotateRight");
            this.toolStripRotateRight.Name = "toolStripRotateRight";
            this.toolStripRotateRight.Click += new System.EventHandler(this.toolStripRotateRight_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolStripPrint
            // 
            this.toolStripPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStripPrint, "toolStripPrint");
            this.toolStripPrint.Name = "toolStripPrint";
            this.toolStripPrint.Click += new System.EventHandler(this.toolStripPrint_Click);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.splitContainerWhole);
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // printBrowser
            // 
            this.printBrowser.EnableInternalReloader = false;
            resources.ApplyResources(this.printBrowser, "printBrowser");
            this.printBrowser.Name = "printBrowser";
            this.printBrowser.SelfNavigate = false;
            // 
            // transactionControl
            // 
            resources.ApplyResources(this.transactionControl, "transactionControl");
            this.transactionControl.DocumentID = 0;
            this.transactionControl.Name = "transactionControl";
            // 
            // faxWatcher
            // 
            this.faxWatcher.EnableRaisingEvents = true;
            this.faxWatcher.NotifyFilter = System.IO.NotifyFilters.FileName;
            this.faxWatcher.SynchronizingObject = this.image;
            // 
            // DocControl
            // 
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.toolBarPanel);
            this.Controls.Add(this.transactionControl);
            this.Controls.Add(this.printBrowser);
            this.DoubleBuffered = true;
            this.Name = "DocControl";
            resources.ApplyResources(this, "$this");
            this.Load += new System.EventHandler(this.DocControl_Load);
            this.HandleDestroyed += new System.EventHandler(this.DocControl_HandleDestroyed);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DocControl_KeyUp);
            this.splitContainerWhole.Panel1.ResumeLayout(false);
            this.splitContainerWhole.Panel2.ResumeLayout(false);
            this.splitContainerWhole.ResumeLayout(false);
            this.splitContainerDocumentSign.Panel1.ResumeLayout(false);
            this.splitContainerDocumentSign.Panel2.ResumeLayout(false);
            this.splitContainerDocumentSign.ResumeLayout(false);
            this.splitContainerDocument.Panel1.ResumeLayout(false);
            this.splitContainerDocument.Panel2.ResumeLayout(false);
            this.splitContainerDocument.ResumeLayout(false);
            this.splitContainerBrowser.Panel1.ResumeLayout(false);
            this.splitContainerBrowser.Panel2.ResumeLayout(false);
            this.splitContainerBrowser.ResumeLayout(false);
            this.contextMenuStripBrowser.ResumeLayout(false);
            this.toolBarPanel.ResumeLayout(false);
            this.toolBarPanel.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.faxWatcher)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		public bool IsPDFMode
		{
			get { return imagePDF != null && imagePDF.ImageDisplayed; }
		}

		#region Accessors

		[DefaultValue(null), Editor("System.Windows.Forms.Design.FileNameEditor, System.Design", typeof(UITypeEditor))]
		public string FileName
		{
			get { return fileName; }
			set
			{
				DocumentID = 0;
				fileName = value ?? string.Empty;
				faxWatcher.Filter = "";
				faxWatcher.Created -= faxWatcher_Created;
				faxWatcher.Renamed -= faxWatcher_Renamed;

				WatchOnFile = false;

				if(string.IsNullOrEmpty(fileName))
				{
					ClearDoc();
					return;
				}

				if(File.Exists(fileName))
					try
					{
						bool isPDF = Environment.IsPdf(fileName);
						string real_fileName = fileName;

						subscribe = Guid.Empty;

						if(!isPDF)
						{
							int sPos = SplinterPlace.X;
							image.Visible = true;
							if(imagePDF.ImageDisplayed)
								imagePDF.FileName = "";
							imagePDF.Visible = false;
							panelMain.Visible = true;
							image.SplinterPlace = sPos;
							image.CanSave = imgID < 1;
							image.FileName = real_fileName; // _fileName;
							ImageType = "TIF";
						}
						else
						{
							if(image.ImageDisplayed)
								image.FileName = "";
							image.Visible = false;

							int sPos = SplinterPlace.X;
							imagePDF.Visible = true;
							panelMain.Visible = true;
							imagePDF.SplinterPlace = sPos;

							fileName = value;
							imagePDF.CanSave = imgID < 1;
							imagePDF.FileName = real_fileName; // _fileName;
							imagePDF.Focus();
							ImageType = "PDF";
						}

						int fType = GetFileType.GetType(fileName);
						switch(fType)
						{
							case (int)GetFileType.SFIndex.CatalogScan:
								CreateScanerComponent(isPDF);
								break;
							case (int)GetFileType.SFIndex.FaxIn:
								break;
							default:
								CreateFileComponent(isPDF, fType == (int)GetFileType.SFIndex.Outlook);
								break;
						}
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex, string.Concat("FileName: ", value));
					}
				else
				{
					try
					{
						if(!string.IsNullOrEmpty(fileName))
						{
							var fi = new FileInfo(fileName);
							faxWatcher.Filter = fi.Name;
							faxWatcher.Path = fi.DirectoryName;
						}
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}

					ClearDoc();
				}
			}
		}

		public int DocumentID
		{
			get { return curDocID; }
			set
			{
				compliteLoading = false;
				image.IsSelectionMode = false;
				CursorSleep();
				if(timer != null)
				{
					timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
					timer.Dispose();
					timer = null;
				}
				DeleteReceiver();
				hasImage = false;
				if(curDocID != value && value >= 0)
				{
					subscribe = Guid.Empty;
					addEForm = false;
					curDocID = value;
					isArchive = (curDocID > 0);
					image.SaveStampsInternal = curDocID < 1;
					imgID = -1;
					RefreshDoc();
				}
				transactionControl.DocumentID = value;
			}
		}

		public int ImageID
		{
			get { return imgID; }
			set
			{
				if(value < 0)
					return;

				if(imgID > 0 && imgID != value)
					DeleteReceiver();

				if(variantList == null || variantList.Items.Count <= 0)
					return;

				if(value == 0 && showWebPanel)
				{
					imgID = value;
					variantList.Items[0].Selected = true;
				}
				else
					for(int i = 0; i < variantList.Items.Count; i++)
					{
						var item = variantList.Items[i] as VariantListItem;
						if(item == null)
							continue;
						if(value > 0)
							switch(item.Type)
							{
								case VariantType.MainImageOriginal:
								case VariantType.ImageOriginal:
								case VariantType.MainImage:
								case VariantType.Image:
									if(item.ID == ImageID)
									{
										imgID = value;
										if(item.Selected)
											item.Selected = false;
										item.Selected = true;
									}
									break;
							}
						else
							switch(item.Type)
							{
								case VariantType.Data:
									imgID = value;
									item.Selected = true;
									ShowHTMLDoc();
									break;
							}
					}
			}
		}

		public string ImageType { get; private set; }

		public bool ForceRelicate { get; set; }

		public DateTime ImageDate { get; internal set; }

		public int ImageResolutionX
		{
			get
			{
				return image != null && image.ImageDisplayed ? image.ImageResolutionX :
					imagePDF != null && imagePDF.ImageDisplayed ? imagePDF.ImageResolutionX : -1;
			}
		}

		public int ImageResolutionY
		{
			get
			{
				return image != null && image.ImageDisplayed ? image.ImageResolutionY :
					imagePDF != null && imagePDF.ImageDisplayed ? imagePDF.ImageResolutionY : -1;
			}
		}

		public string EmpName
		{
			get { return Environment.EmpName; }
			set { Environment.EmpName = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UseLock
		{
			get { return image.UseLock; }
			set
			{
				if(Environment.TmpFilesContains(fileName))
					value = false;
				image.UseLock = imagePDF.UseLock = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ConnectionStringDocument
		{
			get { return Environment.ConnectionStringDocument; }
			set { Environment.ConnectionStringDocument = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ConnectionStringAccounting
		{
			get { return Environment.ConnectionStringAccounting; }
			set { Environment.ConnectionStringAccounting = value; }
		}

		public string PersonParamStr
		{
			get { return Environment.PersonParamStr; }
			set { Environment.PersonParamStr = value; }
		}

		[DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanSave { get; set; }

		public bool ShowThumbPanel
		{
			get { return image.ShowThumbPanel; }
			set { image.ShowThumbPanel = imagePDF.ShowThumbPanel = value; }
		}

		public ImageControl.ImageControl.TypeThumbnailPanelOrientation ImagesPanelOrientation
		{
			get { return image.ThumbnailPanelOrientation; }
			set
			{
				image.ThumbnailPanelOrientation = value;
				imagePDF.Orientation = value == ImageControl.ImageControl.TypeThumbnailPanelOrientation.Top ? Orientation.Horizontal : Orientation.Vertical;
			}
		}

		public bool Is1CItem
		{
			get
			{
				if(variantList.SelectedItems.Count == 0)
					return false;
				var item = variantList.SelectedItems[0] as VariantListItem;
				return item != null && item.Type == VariantType.ICConnect;
			}
		}

		private void ImageIndexChanged(VariantListItem item, int index)
		{
			if(item == null || curDocID <= 0)
				return;
			bool isSent = Environment.BuhParamDocData.IsSentDocToIc(curDocID, item.ID, item.Type == VariantType.ICConnect ? 0 : 1);

			switch(item.Type)
			{
				case VariantType.ICConnect:
					item.ImageIndex = isSent ? 15 : (int)VariantType.ICConnect;
					break;
				case VariantType.ICMRConnect:
					item.ImageIndex = isSent ? 16 : (int)VariantType.ICMRConnect;
					break;
			}
		}

		public bool ShowWebPanel
		{
			get { return showWebPanel; }
			set
			{
				showWebPanel = value;
				if(!hasData)
				{
					if(curDocID > 0)
					{
						int docTypeID = Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, curDocID,
																		   -1);
						if(Environment.DocTypeData.GetDocBoolField(Environment.DocTypeData.FormPresentField, docTypeID))
						{
							string url =
								Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, docTypeID).ToString();
							if(string.IsNullOrEmpty(url))
								url = Environment.SettingsURLString;
							try
							{
								url = url.IndexOf("id=") > 0
										  ? url.Replace("id=", "id=" + curDocID.ToString())
										  : (url + ((url.IndexOf("?") > 0) ? "&" : "?") + "id=" + curDocID.ToString());
							}
							catch
							{
								url = url.IndexOf("id=") > 0
										  ? url.Replace("id=",
														"id=" + curDocID.ToString() + "&docview=" + (IsMain ? "1" : "2"))
										  : (url + ((url.IndexOf("?") > 0) ? "&" : "?") + "id=" + curDocID.ToString());
							}
							Environment.IEOpenOnURL(url);
						}
					}
				}
				WebShow();
			}
		}

		[DefaultValue(false)]
		public bool ShowToolBar
		{
			get { return showToolBar; }
			set
			{
				showToolBar = value;
				toolBarPanel.Visible = showToolBar;
				toolBarPanel.Cursor = Cursors.Default;
			}
		}

		public string CurDocString { get; set; }

		public int Zoom
		{
			get
			{
				return imagePDF.ImageDisplayed ? imagePDF.Zoom : image.Zoom;
			}
			set { image.Zoom = imagePDF.Zoom = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CompliteLoading
		{
			get { return compliteLoading; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VarListHeight
		{
			get { return splitContainerWhole.Panel1.Height; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollPositionX
		{
			get
			{
				return (DesignerDetector.IsComponentInDesignMode(this)
							? 0
							: (!IsPDFMode ? image.ScrollPositionX : imagePDF.ScrollPositionX));
			}
			set
			{
				if(!IsPDFMode)
					image.ScrollPositionX = value;
				else
					imagePDF.ScrollPositionX = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ScrollPositionY
		{
			get
			{
				return (DesignerDetector.IsComponentInDesignMode(this)
							? 0
							: (!IsPDFMode ? image.ScrollPositionY : imagePDF.ScrollPositionY));
			}
			set
			{
				if(!IsPDFMode)
					image.ScrollPositionY = value;
				else
					imagePDF.ScrollPositionY = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasData
		{
			get { return showWebPanel && (hasData || addEForm); }
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool HasDocLinks { get; private set; }

		/// <summary>
		///   Указавает на запуск в основном окне
		/// </summary>
		public bool IsMain { get; set; }// конторл загружен в основной форме. для передачи в электронную.

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsSigned
		{
			get
			{
				image.CanSave = imagePDF.CanSave = !signDocumentPanel.IsSigned;
				return signDocumentPanel.IsSigned;
			}
		}

		/// <summary>
		///   Изображение текущего выбранного штампа
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Image CurrentStamp
		{
			get
			{
				return image.ImageDisplayed ? image.CurrentStamp : imagePDF.CurrentStamp;
			}
			set
			{
				if(image.ImageDisplayed)
					image.CurrentStamp = value;
				else
					imagePDF.CurrentStamp = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CurrentStampID
		{
			get
			{
				return image.ImageDisplayed ? image.CurrentStampID : imagePDF.CurrentStampID;
			}
			set
			{
				if(image.ImageDisplayed)
					image.CurrentStampID = value;
				else
					imagePDF.CurrentStampID = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsReadonly { get; private set; }

        /// <summary>
        ///   Всегда отображать окно настроек печати
        /// </summary>
        public bool AlwaysShow
        {
            get
            {
                return Convert.ToBoolean(new Options.Root("DocView\\Document\\Print").LoadStringOption("AlwaysShow", true.ToString()));
            }
            set
            {
                Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\\Software\\Kesco\\DocView\\Document\\Print", "AlwaysShow", value);
            }
        }

		#endregion

		public void ChangeImageIC()
		{
			if(variantList.SelectedItems == null || variantList.SelectedItems.Count <= 0)
				return;
			var item = variantList.SelectedItems[0] as VariantListItem;
			if(item == null || item.Type != VariantType.ICConnect)
				return;

			ImageIndexChangedDelegate wd = ImageIndexChanged;
			BeginInvoke(wd, new object[] { item, 15 });
		}

		#region FileWatcher

		private bool isWatchOnFile;

		[Browsable(false)]
		public bool WatchOnFile
		{
			get { return isWatchOnFile; }
			set
			{
				if(_watcher == null)
					isWatchOnFile = false;
				else
				{
					_watcher.Changed -= OnFileChanged;
					_watcher.Deleted -= OnFileDeleted;
					_watcher.Renamed -= OnFileRenamed;

					if(!File.Exists(fileName) || !value || Environment.GetTmpFileByValue(fileName) != null)
					{
						isWatchOnFile = false;
						_watcher.EnableRaisingEvents = false;
					}
					else
					{
						isWatchOnFile = value;
						_watcher.Changed += OnFileChanged;
						_watcher.Deleted += OnFileDeleted;
						_watcher.Renamed += OnFileRenamed;

						_watcher.Path = Path.GetDirectoryName(fileName);
						_watcher.Filter = Path.GetFileName(fileName);
						_watcher.EnableRaisingEvents = value;
					}
				}
			}
		}

		public event EventHandler NeedToRefresh;

		private void OnFileChanged(object source, FileSystemEventArgs e)
		{
			if(File.Exists(fileName))
				fileWatcherMethod();
			else
				OnFileDeleted(source, e);
		}

		private void OnFileDeleted(object source, FileSystemEventArgs e)
		{
			ErrorShower.OnShowError(this, string.Format(Environment.StringResources.GetString("DocControl_FileDeleted"), fileName), Environment.StringResources.GetString("Warning"));
			if(NeedToRefresh != null)
				NeedToRefresh(null, null);
		}

		private void OnFileRenamed(object sender, RenamedEventArgs e)
		{
			if(NeedToRefresh != null)
				NeedToRefresh(e.FullPath, null);
		}

		private void fileWatcherMethod()
		{
			try
			{
				if(!IsPDFMode)
				{
					reloadTm.Interval = FileChanged_PAUSE;
					reloadTm.Stop();
					reloadTm.Start();
				}
				else
					reloadTm_Elapsed(null, null);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private void reloadTm_Elapsed(object sender, ElapsedEventArgs e)
		{
			reloadTm.Stop();
			if(Disposing || IsDisposed)
				return;

			if(!InvokeRequired)
				return;
			try
			{
				Invoke((MethodInvoker)(() =>
									  {
										  int p = Page;
										  bool w = WatchOnFile;

										  if(File.Exists(fileName))
										  {
											  if(curDocID < 1)
												  LoadFile(fileName, p);
										  }
										  WatchOnFile = w;
									  }));
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		#region Annotation

		public bool IsNotesAllShow
		{
			get { return image.IsNotesAllShow; }
		}

		public bool IsNotesAllHide
		{
			get { return image.IsNotesAllHide; }
		}

		public bool IsNotesOnlySelfShow
		{
			get { return image.IsNotesOnlySelfShow; }
		}

		public void HideAnnotationGroup(object groupName)
		{
			image.HideAnnotationGroup(groupName);
		}

		public void ShowAnnotationGroup(object groupName)
		{
			image.ShowAnnotationGroup(groupName);
		}

		public string GetCurrentAnnotationGroup()
		{
			return image.GetCurrentAnnotationGroup();
		}

		public string GetAnnotationGroup(int index)
		{
			return image.GetAnnotationGroup((short)index);
		}

		public void ShowAttribsDialog(object mark)
		{
			image.ShowAttribsDialog(mark);
		}

		public void SelectTool(int tool)
		{
			if(tool != 9 && CurrentStamp != null)
			{
				CurrentStamp = null;
				if(image.ImageDisplayed)
					image.ReloadStamps();
				else if(imagePDF.ImageDisplayed)
					imagePDF.ReloadStamps();
			}

			if(tool != 9 || curDocID < 1)
			{
				if(tool == 1 && curDocID > 0 && Page == 1)
					if(image.ImageDisplayed)
						image.TestImage();
					else if(imagePDF.ImageDisplayed)
						imagePDF.TestImage(Environment.ActionBefore.None);
				image.SelectTool((short)tool);
				imagePDF.SelectTool((short)tool);
			}
			else
			{
				if(image.ImageDisplayed)
				{
					image.TestImage();
					image.SelectTool((short)tool, curDocID);
				}
				else if(imagePDF.ImageDisplayed)
				{
					imagePDF.TestImage(Environment.ActionBefore.None);
					imagePDF.SelectTool((short)tool, curDocID);
				}
			}
		}

		public bool AnnotationDraw
		{
			get { return image.AnnotationDraw; }
			set { image.AnnotationDraw = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int AnnotationGroupCount
		{
			get { return (DesignerDetector.IsComponentInDesignMode(this) ? 0 : image.AnnotationGroupCount); }
		}

		#endregion

		#region Browser

		public void RefreshEForm()
		{
			RefreshEForm(false);
		}

		public void RefreshEForm(bool force)
		{
			if(force)
			{
				if(webTimer != null)
				{
					webTimer.Elapsed -= webTimer_Elapsed;
					webTimer.Stop();
				}
				BrowserRefresh(true);
			}
			else
			{
				if(webTimer == null)
				{
					webTimer = new System.Timers.Timer();
				}
				else
				{
					webTimer.Elapsed -= webTimer_Elapsed;
					webTimer.Stop();
				}
				webTimer.Elapsed += webTimer_Elapsed;
				webTimer.AutoReset = false;
				webTimer.Interval = webTimerInterval;
				webTimer.Start();
			}
		}

		public void RefreshSigns()
		{
			signDocumentPanel.LoadDocInfo(curDocID, imgID > 0 ? (int?)imgID : null);
			if(imgID <= 0)
				return;
			if(image.ImageDisplayed)
				image.ReloadStamps();
			else if(imagePDF.ImageDisplayed)
				imagePDF.ReloadStamps();

		}

		private void BrowserRefresh(bool force = false)
		{
			if(InvokeRequired)
				BeginInvoke(new BrowserRefreshDelegate(BrowserRefresh), force);
			else if(browser != null)
			{
				Log.Logger.EnterMethod(this, "BrowserRefresh()");
				Uri intUri = browser.InternalUri;
				if(browser.Url != null && browser.Url.AbsoluteUri != "" && browser.Url.AbsoluteUri != "about:blank")
					browser.Stop();
				try
				{
					if(force || !browser.IsBusy)
					{
						if(intUri != null)
							browser.NavigateTo(intUri);
						else if(browser.Url != null && browser.Url.AbsoluteUri != "" && browser.Url.AbsoluteUri != "about:blank")
							browser.Navigate(browser.Url);
					}
					else
						RefreshEForm();
				}
				catch
				{
					RefreshEForm();
				}
				finally
				{
					Log.Logger.LeaveMethod(this, "BrowserRefresh() url = " + ((intUri != null) ? intUri.AbsoluteUri : (browser.Url != null)?browser.Url.AbsoluteUri :"null"));
				}
			}
		}

		private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			Log.Logger.EnterMethod(this, "browser_DocumentCompleted()");
			if(browser.Url == null || string.IsNullOrEmpty(browser.Url.AbsoluteUri) || browser.Url.AbsoluteUri.Equals("about:blank"))
				return;
			try
			{
			if(!compliteLoading)
				OnLoadComplete();
			if(!newEForm)
				return;
			try
			{
				string idName = browser.Url.PathAndQuery;
				int trim = 0;
				if((trim = idName.ToLower().IndexOf("&id=")) >= 0 || (trim = idName.ToLower().IndexOf("?id=")) >= 0)
				{
					idName = idName.Substring(trim + 4, idName.Length - trim - 4);
					if((trim = idName.ToLower().IndexOf("&")) >= 0)
						idName = idName.Substring(0, trim);

					int id = Int32.Parse(idName);
					if(Environment.DocDataData.IsDataPresent(id))
					{
						newEForm = false;

						Environment.WorkDocData.AddDocToEmployee(id, Environment.CurEmp.ID);
						OnDocChanged(this, new Components.DocumentSavedEventArgs(id, 0, true, false, false));
					}
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}
			finally
			{
				CursorWake();
				Log.Logger.LeaveMethod(this, "browser_DocumentCompleted()");
			}
		}

		private void RestoreBrowser()
		{
			var resources = new ComponentResourceManager(typeof(DocControl));
			printBrowser = new ExtendedBrowserControl();
			SuspendLayout();
			// 
			// printBrowser
			// 
			printBrowser.AccessibleDescription = null;
			printBrowser.AccessibleName = null;
			resources.ApplyResources(printBrowser, "printBrowser");
			printBrowser.EnableInternalReloader = false;
			printBrowser.Name = "printBrowser";
			printBrowser.SelfNavigate = false;
			Controls.Add(printBrowser);
			ResumeLayout(false);
		}

		private void browser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			
		}

		private void imagePDF_ZoomChanged(object sender, EventArgs e)
		{
			UpdateNavigation();
		}

		#endregion

		#region Cursor

		private void CursorSleep()
		{
			if(Cursor != Cursors.WaitCursor)
			{
				//   Application.DoEvents();
				Cursor = Cursors.WaitCursor;
			}
		}

		private void CursorWake()
		{
			if(Cursor != Cursors.Default)
			{
				Cursor = Cursors.Default;
				//   Application.DoEvents();
			}
		}

		#endregion

		#region File

		private void ClearDoc()
		{
			if(Disposing || IsDisposed)
				return;
			if(reloadTm != null)
				reloadTm.Stop();

			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(ClearDoc));
				return;
			}

			splitContainerBrowser.Panel2Collapsed = true;
			splitContainerDocument.Panel2Collapsed = true;
			splitContainerWhole.Panel1Collapsed = true;
			using(Log.Logger.DurationMetter("ClearDoc() ForceNavigate"))
			if(!browser.Disposing && browser.Url != null && browser.Url.AbsoluteUri != "about:blank")
			{
				browser.Stop();
				browser.ForceNavigate("about:blank");
			}
			compliteLoading = false;
			try
			{
				WatchOnFile = false;

				image.FileName = imagePDF.FileName = "";
				imgID = -1;
				page = 0;
				if(docComponent != null)
				{
					docComponent.Dispose();
					docComponent = null;
				}

				docComponent = new Components.DocControlComponent();
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			DeleteReceiver();

			if(variantList.Items.Count == 0)
				variantList.Clear();
			ForceRelicate = false;
			addEForm = false;
			canPrintEForm = false;
			hasData = false;
			HasDocLinks = false;
			hasImage = false;
			UpdateNavigation();
			panelMain.Visible = false;
			ImageDate = DateTime.MinValue;
		}


		public int SendFax()
		{
			//return SendFaxNew();
			if(!Environment.IsFaxSender(curDocID))
				return curDocID;

			TestImage(Environment.ActionBefore.SendFaxAndMail);

			string oldFileName = fileName;
			int docID = curDocID;
			int imID = imgID;

			Objects.TmpFile tf = Environment.GetTmpFileByKey(this.fileName);
			if(tf != null && tf.CurAct == Environment.ActionBefore.SendFaxAndMail)
			{
				if(!File.Exists(tf.TmpFullName))
					SaveToTmpCopy(fileName, tf.TmpFullName, IsPDFMode);

				oldFileName = tf.TmpFullName;
				docID = -1;
				imID = -1;
			}

			if(Environment.DocToSend.ContainsKey(oldFileName))
			{
				var dialog = Environment.DocToSend[oldFileName] as Dialogs.SendFaxDialog;
				if(dialog != null)
				{
					dialog.BringToFront();
					dialog.Activate();
				}
			}
			else
			{
				if(File.Exists(oldFileName))
				{
					var dialog = new Dialogs.SendFaxDialog(oldFileName, CurDocString, docID, imID, IsPDFMode);
					dialog.DialogEvent += SendFaxDialog_DialogEvent;
					dialog.Show();
					Environment.DocToSend.TryAdd(oldFileName, dialog);

					if(tf == null || tf.CurAct == Environment.ActionBefore.SendFaxAndMail)
					{
						if(tf == null)
							tf = Environment.GetTmpFile(oldFileName);
						if(tf != null)
							tf.LinkCnt++;
					}
				}
			}
			return -1;
		}

		private void SendFaxDialog_DialogEvent(object source, DialogEventArgs e)
		{
			var dialog = e.Dialog as Dialogs.SendFaxDialog;

			if(dialog == null || string.IsNullOrEmpty(dialog.FileName))
				return;
			Form f = null;
			if(Environment.DocToSend.ContainsKey(dialog.FileName))
				Environment.DocToSend.TryRemove(dialog.FileName, out f);
			if(Document.Environment.TmpFilesContains(dialog.FileName))
			{
				Document.Objects.TmpFile tf = Environment.GetTmpFileByValue(dialog.FileName);
				if(tf != null)
				{
					tf.CurAct = Environment.ActionBefore.None;
					tf.LinkCnt--;
				}
			}
		}

		public void SendFax(int personID, string phoneNumber)
		{
			if(Environment.IsFaxSender(curDocID))
			{
				var dialog = new Dialogs.SendFaxDialog(fileName, CurDocString, curDocID, imgID, personID, phoneNumber);
				dialog.DialogEvent += SendFaxDialog_DialogEvent;
				dialog.Show();
			}
		}

		public int SendFax(int faxID)
		{
			return SendFaxNew(faxID);
			if(!Environment.IsFaxSender())
				return 0;

			var dialog = new Dialogs.SendFaxDialog(faxID, imgID);
			dialog.DialogEvent += SendFaxDialog_DialogEvent;
			dialog.Show();
			return -1;
		}

		#endregion

		#region Image

		private void image_PageChanged(object sender, EventArgs e)
		{
			try
			{
				UpdateNavigation();
				OnPageChange(sender, e);
				page = imagePDF.ImageDisplayed ? imagePDF.Page : image.Page;
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private void image_SplinterPlaceChanged(object sender, SplitterEventArgs e)
		{
			try
			{
				OnSplitterChange(sender, e);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private void image_MarkEnd(object sender, MarkEndEventArgs e)
		{
			try
			{
				OnMarkEnd(sender, e);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private void image_NeedSave(Lib.Win.ImageControl.SaveEventArgs NeedChange)
		{
			if(!CanSave)
			{
				OnNeedSave();
				NeedChange.Save = true;
				return;
			}

			Environment.ActionBefore act = (NeedChange is Document.SaveEventArgs) ? (NeedChange as Document.SaveEventArgs).Action : Environment.ActionBefore.None;

			if(!docComponent.IsFax())
			{
				TmpFile tf = Document.Environment.TmpFiles.FirstOrDefault(x => x.Key == this.fileName).Value;

				try
				{
					if(tf != null)
					{
						//string file = (curDocID > 0 ? Environment.StringResources.GetString("SaveChangesDialog_Doc1") : Environment.StringResources.GetString("SaveChangesDialog_File1")).ToLower();
						//MessageBox.Show(string.Format(StringResources.DocControl_CopyExists, file), StringResources.Warning);
						new Dialogs.ProhibitionDialog(curDocID > 0, tf.TmpFullName, tf.DocString).ShowDialog();
					}
					else
					{
						// Проверка возможности выполнить виртуальный поворот 
						bool isCanSaveVirtualRotation = CanSaveVirtualRotation();
						bool userConfirmSave = false;

						bool save = false;

						Dialogs.SaveChangesDialog sch = null;
						Form form = null;
						// возможно this.fileName - имя временного файла
						if(Environment.DocToSave.TryGetValue(this.fileName, out form) && form != null)
						{
							form.BringToFront();
							form.Activate();
						}
						else
							Environment.DocToSave.TryAdd(this.fileName, null);
						if(signDocumentPanel.IsSignInternal)
						{
							Lib.Win.MessageForm mf = new Lib.Win.MessageForm(Environment.StringResources.GetString("DocControl_image_NeedSave_Message1") + " (" +
									this.CurDocString + ", " + Environment.StringResources.GetString("Page") + " " + this.Page.ToString() + ")?", Environment.StringResources.GetString("Save1"),
									MessageBoxButtons.YesNo);
							if(!Environment.DocToSave.TryUpdate(this.fileName, mf, null))
								return;
							userConfirmSave = save = (mf.ShowDialog(this.FindForm()) == DialogResult.Yes);
						}
						else
							if(tf != null) //Document.Environment.TmpFilesContains(this.fileName) && IsMain)
						{
							Lib.Win.MessageForm mf = new Lib.Win.MessageForm(Environment.StringResources.GetString("DocControl_image_NeedSave_Message1") + " (" +
									tf.DocString + ", " + Environment.StringResources.GetString("Page") + " " + this.Page.ToString() + ")?", Environment.StringResources.GetString("Save1"),
									MessageBoxButtons.YesNo);
							if(!Environment.DocToSave.TryUpdate(this.fileName, mf, null))
								return;
							save = (mf.ShowDialog(this.FindForm()) == DialogResult.Yes);
						}
						else
						{
							if(isCanSaveVirtualRotation)
							{
								// Простой диалог да||нет
								var mf = new MessageForm(Environment.StringResources.GetString("DocControl_image_NeedSave_Message1") + " (" +
									CurDocString + ", " + Environment.StringResources.GetString("Page") + " " + Page + ")?", Environment.StringResources.GetString("Save1"), MessageBoxButtons.YesNo);
								if(!Environment.DocToSave.TryUpdate(this.fileName, mf, null))
									return;
								userConfirmSave = save = (mf.ShowDialog(this.FindForm()) == DialogResult.Yes);
							}
							else
							{
								sch = new Dialogs.SaveChangesDialog(Path.GetFileName(this.fileName), IsReadonly, act, (docComponent.IsDocument() ? curDocID : -1), Environment.Settings.Folders.Add("SaveChanges"));
								if(!Environment.DocToSave.TryUpdate(this.fileName, sch, null))
									return;
								userConfirmSave = save = (sch.ShowDialog() == DialogResult.Yes);
							}
						}
						Environment.DocToSave.TryRemove(this.fileName, out form);


						if(save)
						{
							if(userConfirmSave && isCanSaveVirtualRotation)
							{

								if(image.ImageDisplayed)
									image.SetVirtualRotation();
								else
									imagePDF.SetVirtualRotation();

								if(signDocumentPanel.IsSignInternal)
								{
									// Позиция штампа
									SaveDocImage(true);
								}


								// Сохранение позиций штампов
								if(IsPDFMode)
									imagePDF.SaveStampsPositions();
								else
									image.SaveStampsPositions();

								NeedChange.Save = true;
								return;
							}

							if(sch == null || sch.Result == Dialogs.DelPartDialogResult.Yes)
							{
								saving = true;
								if(docComponent.IsDocument())
								{
									DeleteReceiver();
									SaveDocImage();
								}
								else if(docComponent.IsFromScaner())
								{
									image.Save();
									docComponent.Save();
								}
								else
								{
									if(image.ImageDisplayed)
										image.Save();
									else
										imagePDF.Save();
								}
								NeedChange.Save = true;
								saving = false;

								if(tf != null)
									tf.Modified = true;

								return;
							}
							else
							{
								if(sch != null && (sch.Result == Dialogs.DelPartDialogResult.CreateCopy || sch.Result == Dialogs.DelPartDialogResult.ShowCopy))
								{
									string real_fileName = Environment.GenerateFullFileName(Path.GetExtension(this.fileName).TrimStart('.'));
									if(!SaveAs(real_fileName))
									{
										NeedChange.Save = false;

										if(this.docComponent.IsDocument())
										{
											if(image.ImageID == imgID)
											{
												page = image.Page;
												image.LoadImagePage(imgID, page - 1, true);
											}
											else if(imagePDF.ImageID == imgID)
											{
												page = imagePDF.Page;
												imagePDF.LoadImagePage(imgID, page, true);
											}
										}
										else
											LoadFile(this.fileName, this.page, true);
										return;
									}

									Environment.AddTmpFile(this.fileName, real_fileName, this.IsPDFMode);
									tf = Document.Environment.GetTmpFile(this.fileName);
									tf.DocId = curDocID;
									tf.CurAct = act;
									tf.Modified = true;
									tf.DocString = !string.IsNullOrEmpty(CurDocString) ? CurDocString : Document.DBDocString.Format(curDocID);
									tf.DocString = !string.IsNullOrEmpty(tf.DocString) ? tf.DocString : Path.GetFileName(this.fileName);
									tf.DocString += " (" + StringResources.SaveChangesDialog_Changed.Trim() + ")";
									tf.CurPage = (imagePDF.ImageDisplayed ? imagePDF.Page : image.Page);
									tf.PageCount = (imagePDF.ImageDisplayed ? imagePDF.PageCount : image.PageCount);
									tf.IsPdf = imagePDF.ImageDisplayed;
									tf.SrvInfo = this.docComponent.ServerInfo;

									if(sch.Result == Dialogs.DelPartDialogResult.ShowCopy || act == Environment.ActionBefore.Save)
									{
										Components.DocumentSavedEventArgs doc = new Components.DocumentSavedEventArgs(real_fileName, tf.DocString) { BeforeAct = act };
										Environment.OnNewWindow(this, doc);
									}
									else
									{
										//tf.AscBeforeClose = false;
										if(act == Environment.ActionBefore.None || act == Environment.ActionBefore.LeaveFile)
											Environment.RemoveTmpFile(this.fileName);
									}

									if(act != Environment.ActionBefore.LeaveFile)
									{
										if(this.docComponent.IsDocument())
										{
											if(image.ImageID == imgID)
											{
												page = image.Page;
												image.LoadImagePage(imgID, page - 1, true);
											}
											else if(imagePDF.ImageID == 0 && imgID > 0 || imagePDF.ImageID == imgID)
											{
												page = imagePDF.Page;
												imagePDF.LoadImagePage(imgID, page, true);
											}
										}
										else
											LoadFile(this.fileName, this.page, true);
									}
								}
							}
						}
					}
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
				}
			}

			// Тк сохранение отменено, востанавиваю штампы
			if(image.ImageID > 0 && image.ImageDisplayed)
				image.ReloadStamps();
			else if(imagePDF.ImageID > 0 && imagePDF.ImageDisplayed)
				imagePDF.ReloadStamps();

			NeedChange.Save = false;
		}

		/// <summary>
		/// Проверка возможности выполнить виртуальный поворот 
		/// </summary>
		/// <returns></returns>
		private bool CanSaveVirtualRotation()
		{
			// Если подписано или имеет электронную форму и подпись документа с датой >= даты изображения
			var isCanSaveVirtualRotation = IsSigned || (ImageID > 0 && Environment.DocImageData.IsImageSigned(ImageID));
			image.CanSave = imagePDF.CanSave = !isCanSaveVirtualRotation;
			isCanSaveVirtualRotation = isCanSaveVirtualRotation || externalSave;
			return isCanSaveVirtualRotation;
		}

		private void image_ImageLoad(object sender, EventArgs e)
		{
			try
			{
				if(!compliteLoading)
					OnLoadComplete();
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// загрузка документа в архиве
		/// </summary>
		/// <param name="documentID">Код документа</param>
		/// <param name="imageID">Опциональный код изображения. Для выбора по умолчанию значание &lt; 0</param>
		/// <param name="pageID">Код страницы. Опциональный параметр</param>
		public void LoadDocument(int documentID, int imageID, int pageID)
		{
			Console.WriteLine("{0}: LoadDocument start", DateTime.Now.ToString("HH:mm:ss fff"));
			compliteLoading = false;
			image.IsSelectionMode = false;
			if(timer != null)
			{
				timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
				timer.Dispose();
				timer = null;
			}
			DeleteReceiver();
			hasImage = false;
			transactionControl.DocumentID = documentID;
			if(curDocID != documentID && documentID >= 0)
			{
				if(imgID != imageID)
				{
					image.FileName = null;
					imagePDF.FileName = null;
				}
				subscribe = Guid.Empty;
				addEForm = false;
				curDocID = documentID;
				isArchive = (curDocID > 0);
				imgID = imageID;
				if(imgID < -1)
					imgID = -1;
				page = pageID;
				RefreshDoc();
			}
			if(documentID < 0)
				ClearSignDocumentPanel();

			int scrX = ScrollPositionX;
			int scrY = ScrollPositionY;

			Console.WriteLine("{0}: LoadDocument end", DateTime.Now.ToString("HH:mm:ss fff"));
			//if(IsPDFMode)
			//{
			//    imagePDF.Page = pageID;
			//    imagePDF.Focus();

			//    ScrollPositionX = scrX;
			//    ScrollPositionY = scrY;
			//}
		}

		/// <summary>
		/// Загрузка изображения из файла.
		/// <para>
		/// <note type="caution"> Нумерация <paramref name="page"/> начинается с 1</note>
		/// </para>
		/// </summary>
		/// <param name="filename">имя файла</param>
		/// <param name="page">Номер страницы</param>
		/// <param name="cleanAll">очистка контрола</param>
		public void LoadFile(string filename, int page, bool cleanAll)
		{
			if(cleanAll)
				ClearDoc();
			fileName = filename;
			faxWatcher.Filter = "";
			faxWatcher.Created -= faxWatcher_Created;
			faxWatcher.Renamed -= faxWatcher_Renamed;

			WatchOnFile = false;
			if(page < 1)
				page = 1;
			if(File.Exists(fileName))
			{
				bool isPDF = Environment.IsPdf(fileName);

				imagePDF.Visible = isPDF;
				image.Visible = !isPDF;

				string real_fileName = fileName;

				if(!string.IsNullOrEmpty(fileName))
				{

					subscribe = Guid.Empty;

					if(!isPDF)
                    {
                        int sPos = SplinterPlace.X;
                        panelMain.Visible = true;
                        image.SplinterPlace = sPos;
						image.CanSave = imgID < 1;
						image.LoadFile(real_fileName /*filename*/, page - 1);
						ImageType = "TIF";
					}
					else
					{
						int sPos = SplinterPlace.X;
						panelMain.Visible = true;
						imagePDF.SplinterPlace = sPos;
						imagePDF.CanSave = imgID < 1;
						imagePDF.FileName = real_fileName; // _fileName;
						if(imagePDF.Zoom != Zoom)
							imagePDF.Zoom = Zoom;
						imagePDF.Page = page;
						imagePDF.Focus();
						ImageType = "PDF";
					}

					int fType = GetFileType.GetType(fileName);
					switch(fType)
					{
						case (int)GetFileType.SFIndex.CatalogScan:
							CreateScanerComponent(isPDF);
							break;
						default:
							CreateFileComponent(isPDF, fType == (int)GetFileType.SFIndex.Outlook);
							break;
					}
				}
				else
				{
					if(image.ImageDisplayed)
						image.FileName = filename;
					else if(imagePDF.ImageDisplayed)
						imagePDF.FileName = filename;
				}
			}
			else
			{
				try
				{
					if(!string.IsNullOrEmpty(fileName))
					{
						var fi = new FileInfo(fileName);
						faxWatcher.Filter = fi.Name;
						faxWatcher.Path = fi.DirectoryName;
					}
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
				}
			}
		}

		public void LoadFile(string filename, int page)
		{
			LoadFile(filename, page, true);
		}

		public bool LoadImage(int imageID, int page)
		{
			if(imageID  < 1)
				return false;
			DataRow dr = Environment.DocImageData.GetDocImage(imageID);
			if(dr!= null)
			{
				//splitContainerBrowser.Panel2Collapsed = true;
				//splitContainerBrowser.Panel1Collapsed = false;
				//splitContainerWhole.Panel1Collapsed = true;
				//splitContainerWhole.Panel2Collapsed = false;
				//splitContainerDocument.Visible = true;
				//splitContainerDocument.Panel1Collapsed = true;
				ShowDataPanel(false);
				panelMain.Visible = true;

				compliteLoading = false;
				imgID = imageID;

				if(string.IsNullOrEmpty(toolStripComboZoom.Text))
					toolStripComboZoom.Text =  Environment.StringResources.GetString("ToWindow");
				if(dr[Environment.DocImageData.ImageTypeField].ToString().Equals("pdf", StringComparison.CurrentCultureIgnoreCase))
				{
					if(image.Visible)
					{
						image.FileName = "";
						image.Visible = false;
					}
					else
						image.SendToBack();

					int sPos = SplinterPlace.X;
					panelMain.Visible = true;
					imagePDF.Visible = true;
					imagePDF.SplinterPlace = sPos;
					imagePDF.LoadImagePage(imgID, page);
					ImageType = "PDF";
					CreateDocumentComponent(curDocID, imgID, imagePDF.CurrentServer);
				}
				else
				{
					int sPos = SplinterPlace.X;
					if(imagePDF.Visible)
					{
						imagePDF.FileName = "";
						imagePDF.Visible = false;
					}
					image.Visible = true;
					image.SplinterPlace = sPos;
					ImageType = "TIF";
					image.LoadImagePage(imgID, page - 1);
					CreateDocumentComponent(curDocID, imgID, image.CurrentServer);
				}

				OnVarListIndexChange(this);
				CursorWake();
			}
			return false;
		}

		public void RotateLeft()
		{
			if(image.ImageDisplayed)
				image.RotateLeft();
			else if(imagePDF.ImageDisplayed)
				imagePDF.RotateLeft();
		}

		public void RotateRight()
		{
			if(image.ImageDisplayed)
				image.RotateRight();
			else if(imagePDF.ImageDisplayed)
				imagePDF.RotateRight();
		}

		public void Save()
		{
			if(CanSave && (image.ImageDisplayed || imagePDF.ImageDisplayed))
			{
				docComponent.Contr = this;
				docComponent.Save();
			}
		}

		public bool SaveAs(string filename)
		{
			return (image.ImageDisplayed && image.SaveAs(filename))
				|| (imagePDF.ImageDisplayed && imagePDF.SaveAs(filename));
		}

		public bool SaveDocument()
		{
			try
			{
				if(docComponent.IsDocument())
					SaveDocImage();
				else
					image.Save();
				return true;
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				return false;
			}
		}

		public void SavePart()
		{
			TmpFile tf = Environment.GetTmpFileByKey(FileName);
			if(!ImageDisplayed && tf == null)
				return;

			string oldFileName = (imagePDF.ImageDisplayed ? imagePDF.FileName : (image.ImageDisplayed ? image.FileName : ""));
			int imId = (ImageID > 0 ? ImageID : -1);
			int p = (Page > 0 ? Page : 1);
			int pCnt = (PageCount > 0 ? PageCount : 1);
			string dString = CurDocString;
			bool isPdf = imagePDF.ImageDisplayed;

			if(tf != null && tf.CurAct == Environment.ActionBefore.SavePart)
			{
				if(!File.Exists(tf.TmpFullName))
					SaveToTmpCopy(oldFileName, tf.TmpFullName, IsPDFMode);

				oldFileName = tf.TmpFullName;
				imId = -1;
				p = tf.CurPage;
				pCnt = tf.PageCount;
				dString = tf.DocString;
				isPdf = tf.IsPdf;
			}

			Dialogs.SavePartDialog dialog;
			if(ContainSavePartDialog(oldFileName))
			{
				dialog = GetSavePartDialog(oldFileName);
				if(dialog != null && !dialog.Delete)
				{
					dialog.BringToFront();
					return;
				}
				if(dialog != null)
					dialog.Close();
			}

			if(Environment.DocToSave.ContainsKey(oldFileName))
			{
				Form a_dialog = null;
				Environment.DocToSave.TryRemove(oldFileName, out a_dialog);
				a_dialog = a_dialog as Dialogs.AddDBDocDialog;
				if(a_dialog != null)
				{
					a_dialog.Close();
				}
			}


			dialog = new Dialogs.SavePartDialog(oldFileName, false, docComponent.RetrieveScanDate(), dString, p, imId, pCnt);
			dialog.IsPdf = isPdf;
			AddSavePartDialog(dialog);

			if(tf == null || tf.CurAct == Environment.ActionBefore.SavePart)
			{
				if(tf == null)
					tf = Environment.GetTmpFile(oldFileName);
				if(tf != null)
					tf.LinkCnt++;
			}

			dialog.DialogEvent += SavePartDialog_DialogEvent;
			dialog.Show();
		}

		public void SaveSelected()
		{
			if(signDocumentPanel.IsSignInternal)
				return;
			Image timage = null;
			if(imagePDF.ImageDisplayed)
				timage = imagePDF.CreateImageListFromSelectedImage();
			else
				timage = image.CreateImageListFromSelectedImage();
			if(timage != null)
			{
				int faxID = 0;
				if(imgID > 0)
					faxID = Environment.FaxInData.GetFaxID(imgID);
				else if(docComponent.IsFax())
					faxID = docComponent.ID;
				var dialog = new Dialogs.AddDBDocDialog(docComponent.ServerInfo, Dialogs.AddDBDocDialog.TypeSave.SaveSelected,
							docComponent.RetrieveScanDate(), false, faxID, "", docComponent.IsFax(), docComponent.IsScaner(), "",
														   Components.SaveFaxInfo.Empty, timage, 1);
				dialog.DialogEvent += SaveSelected_DialogEvent;
				dialog.Show();
			}
		}

		/// <summary>
		///   Получить изображение выделенной области
		/// </summary>
		/// <returns> </returns>
		public Image GetSelectedRectImage()
		{
			return image.CreateImageListFromSelectedImage();
		}

		private void image_ScanComplete(object sender, ScanCompleteArgs e)
		{
			if(e.ScanType == Scaner.ScanType.ScanAfter || e.ScanType == Scaner.ScanType.ScanBefore)
				SaveDocImage();
		}

		private void SaveDocImage(bool saveStampOnly = false)
		{
			if(imgID > 0 && (image.ImageDisplayed || imagePDF.ImageDisplayed))
			{
				CursorSleep();
				string ext = image.ImageDisplayed ? "TIF" : "PDF";
				string name = Environment.GenerateFileName(ext);
				string file = null;
				string newFileName = docComponent.ServerInfo.Path + "\\Temp\\" + name;
				int pageCount = 0;
				try
				{
					Data.Temp.Objects.StampItem info = null;
					FileInfo fi = null;
					if(image.ImageDisplayed)
					{
						file = image.FileName;
						if(!image.SaveAs(newFileName, true))
							return;
						info = image.GetEditedDSP();
						fi = new FileInfo(newFileName);
						if(fi == null || !fi.Exists || fi.Length < 100)
							return;
						pageCount = image.GetFilePagesCount(newFileName);
					}
					else if(imagePDF.ImageDisplayed)
					{
						file = imagePDF.FileName;
						if(!imagePDF.SaveAs(newFileName))
							return;
						info = imagePDF.GetEditedDSP();
						fi = new FileInfo(newFileName);
						if(fi == null || !fi.Exists || fi.Length < 100)
							return;
						pageCount = imagePDF.GetFilePagesCount(newFileName);
					}
					if(fi == null)
						return;
					if(pageCount < 1)
						return;

				    if (!saveStampOnly)
				    {
                        while (
                                !Environment.DocImageData.DocImageFileUpdate(docComponent.ServerInfo.ID, name, curDocID, imgID, ext, pageCount) && !File.Exists(file) && fi.Exists && fi.Length > 100)
                                if (MessageBox.Show(Environment.StringResources.GetString("NotSaved"),
                                                    Environment.StringResources.GetString("Confirmation"),
                                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error) == DialogResult.No)
                                break;
				    }

					if(info != null)
						Environment.DocSignatureData.UpdateStampDSP(info.ImageID, info.X, info.Y);
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
					ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
				}
				finally
				{
					CursorWake();
				}
			}
		}

		private void SavePartDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Focus();

			var dialog = e.Dialog as Dialogs.SavePartDialog;
			if(dialog == null)
				return;

			TmpFile tf = Environment.GetTmpFileByValue(dialog.FileName);
			if(dialog.DialogResult == DialogResult.OK)
			{
				string oldFileName = dialog.FileName;
				if(string.IsNullOrWhiteSpace(oldFileName))
					return;
				Form dial = null;
				if(Environment.DocToSave.TryGetValue(oldFileName, out dial))
				{
					if(dial != null && !dial.IsDisposed)
						dial.Show();
					return;
				}
				int minPage = dialog.MinPage;
				int maxPage = dialog.MaxPage;
				int imageID = dialog.ImageID;
				if(image.ImageDisplayed || imagePDF.ImageDisplayed || tf != null)
				{
					int faxID = 0;
					if(imageID > 0)
					{
						faxID = Environment.FaxInData.GetFaxID(imageID);
					}
					Dialogs.AddDBDocDialog saveDialog = new Dialogs.AddDBDocDialog((tf == null || tf.SrvInfo == null ? this.docComponent.ServerInfo : tf.SrvInfo), Dialogs.AddDBDocDialog.TypeSave.SavePart, dialog.ScanDate, dialog.MessageNeeded, faxID, dialog.DocStr, dialog.ImageID, oldFileName, minPage, maxPage, dialog.IsPdf, maxPage - minPage + 1);
					saveDialog.DialogEvent += new Lib.Win.DialogEventHandler(SavePart_AddDBDocDialog_DialogEvent);
					saveDialog.Show();

					Environment.DocToSave.TryAdd(oldFileName, saveDialog);
				}
			}
			else
			{
				if(tf != null)
				{
					tf.CurAct = Environment.ActionBefore.None;
					tf.LinkCnt--;
				}
			}
		}

		private void SaveSelected_DialogEvent(object source, DialogEventArgs e)
		{
			Focus();

			var dialog = e.Dialog as Dialogs.AddDBDocDialog;
			if(dialog == null)
				return;
			if(dialog.DocID > 0 && dialog.ImageID > 0)
			{
				if(dialog.NeedOpenDoc)
					Environment.OnNewWindow(this, new Components.DocumentSavedEventArgs(dialog.DocID, -1));
				SaveImageDialog_DialogEvent(source, e);
			}
		}

		private void SavePart_AddDBDocDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Focus();

			var dialog = e.Dialog as Dialogs.AddDBDocDialog;
			if(dialog == null)
				return;
			Form form = null;
			if(!string.IsNullOrWhiteSpace(dialog.OldFileName))
			{
				Environment.DocToSave.TryRemove(dialog.OldFileName, out form);
				form = null;
			}

			TmpFile tf = Environment.GetTmpFileByKey(dialog.OldFileName);

			switch(dialog.DialogResult)
			{
				case DialogResult.Yes:
				case DialogResult.Retry:
				case DialogResult.OK:
					{
						string oldFileName = dialog.OldFileName;

						Dialogs.SaveChangesDialog dp = null;
						bool delPart = true;
						bool rdonly = dialog.OldFileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase) ? IsReadonly : !Environment.IsAccessible(dialog.OldFileName);
						if(dialog.DocID > 0)
							rdonly = false;

						if(tf != null)
						{
							delPart = false;// имя файла в ключе - нельзя удалять при наличии копии
						}
						else
						{
							string docstring = oldFileName;
							tf = Document.Environment.GetTmpFileByValue(dialog.OldFileName);
							// возможно dialog.FileName - имя временного файла
							if(tf != null)
							{
								docstring = tf.DocString; //originalName;
								tf.CurAct = Environment.ActionBefore.None;
								//if(!tf.AscBeforeClose)
								//    delPart = false;
								//else
								delPart = (MessageBox.Show(string.Format(Environment.StringResources.GetString("DocControl_DelPartQuery_1"), docstring), Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes);
							}
							else
							{
								dp = new Dialogs.SaveChangesDialog(docstring, rdonly, Document.Environment.ActionBefore.DelPartAfterSave, dialog.OldImageID, Environment.Settings.Folders.Add("SaveChanges"));
								delPart = (dp.ShowDialog() == DialogResult.Yes && (dp.Result == Dialogs.DelPartDialogResult.Yes || dp.Result == Dialogs.DelPartDialogResult.ShowCopy));
							}
						}
						if(delPart)
						{
							if(dp == null || dp.Result == Dialogs.DelPartDialogResult.Yes)
							{
								if(dialog.OldImageID > 0)
								{
									int oldImageID = dialog.OldImageID;
									string ext = (dialog.isPDF) ? "pdf" : "tif";
									string name = Environment.GenerateFileName(ext);
									string newFileName = dialog.Server.Path + "\\Temp\\" + name;

									while(File.Exists(newFileName))
									{
										name = Environment.GenerateFileName(ext);
										newFileName = dialog.Server.Path + "\\Temp\\" + name;
									}
									try
									{
										DataRow dr = Environment.DocImageData.GetDocImage(oldImageID);
										if(dr != null)
										{
											var docID = (int)dr[Environment.DocImageData.DocIDField];
											File.Copy(oldFileName, newFileName, true);
											int count = 0;
											if(File.Exists(newFileName))
											{
												if(!dialog.isPDF)
												{
													image.DelPart(newFileName, dialog.MinPage, dialog.MaxPage, false);
													count = image.GetFilePagesCount(newFileName);
												}
												else
												{
													if(oldFileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase))
														imagePDF.DelPart(newFileName, dialog.MinPage, dialog.MaxPage, !dialog.GotoDoc);
													else
														Environment.PDFHelper.DelPart(oldFileName, newFileName, dialog.MinPage, dialog.MaxPage);
													count = imagePDF.GetFilePagesCount(newFileName);
												}
											}
											if(count < 1)
												throw new Exception("отсутсвуют стрнаницы в сохраняемом файле");
											while(
												!Environment.DocImageData.DocImageFileUpdate(dialog.Server.ID, name, docID, oldImageID, (imagePDF.ImageDisplayed ? "PDF" : "TIF"), count) && !File.Exists(oldFileName) && File.Exists(newFileName))
												if(MessageBox.Show(Environment.StringResources.GetString("NotSaved"), Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error) == DialogResult.No)
													break;
											if(oldFileName.Equals(image.FileName, StringComparison.CurrentCultureIgnoreCase))
												image.FileName = oldFileName;
										}
									}
									catch(Exception ex)
									{
										ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
										Data.Env.WriteToLog(ex, String.Format("DocID = {0}\nNewFileName = {1}\nImageID = {2}", curDocID, newFileName, imgID));
									}
									Environment.LogEmailData.UpdateEmail(oldImageID, dialog.ImageID);
								}
								else
								{
									try
									{
										if(!dialog.isPDF)
											image.DelPart(oldFileName, dialog.MinPage, dialog.MaxPage, false);
										else if(oldFileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase) && IsMain)
											imagePDF.DelPart(oldFileName, dialog.MinPage, dialog.MaxPage, !dialog.GotoDoc);
										else
											Environment.PDFHelper.DelPart(oldFileName, dialog.MinPage, dialog.MaxPage);
									}
									catch(Exception ex)
									{
										Data.Env.WriteToLog(ex);
										Error.ErrorShower.OnShowError(this, ex.Message, StringResources.Error);
									}
								}

								if(tf != null)
									tf.Modified = true;
							}
							else if(dp.Result == Dialogs.DelPartDialogResult.ShowCopy)
							{
								oldFileName = Environment.GenerateFullFileName(Path.GetExtension(dialog.OldFileName).TrimStart('.'));
								File.Copy(dialog.OldFileName, oldFileName, true);
								File.SetAttributes(oldFileName, FileAttributes.Normal);

								if(!dialog.isPDF)
									image.DelPart(oldFileName, dialog.MinPage, dialog.MaxPage, false);
								else
									Environment.PDFHelper.DelPart(oldFileName, dialog.MinPage, dialog.MaxPage);

								Environment.AddTmpFile(dialog.OldFileName, oldFileName, dialog.isPDF);
								tf = Document.Environment.GetTmpFile(dialog.OldFileName);
								tf.CurAct = Environment.ActionBefore.None;

								int dId = 0;
								if(dialog.OldImageID > 0)
								{
									DataRow dr = Environment.DocImageData.GetDocImage(dialog.OldImageID);
									if(dr != null)
										dId = (int)dr[Environment.DocImageData.DocIDField];
								}

								tf.DocString = !string.IsNullOrEmpty(dialog.DocString) ? dialog.DocString : Document.DBDocString.Format(dId);
								tf.DocString = !string.IsNullOrEmpty(tf.DocString) ? tf.DocString : Path.GetFileName(dialog.OldFileName);
								tf.DocString += " (" + StringResources.SaveChangesDialog_Changed.Trim() + ")";
								tf.DocId = dId;
								tf.Modified = true;
								tf.IsReadOnly = rdonly;
								tf.SrvInfo = dialog.Server;
								tf.IsPdf = dialog.isPDF;

								Components.DocumentSavedEventArgs doc = new Components.DocumentSavedEventArgs(oldFileName, tf.DocString) { BeforeAct = Environment.ActionBefore.None };
								Environment.OnNewWindow(this, doc);
							}
						}

						bool work = dialog.DialogResult != DialogResult.Yes;
						if(work && dialog.AddToWork)
							Environment.WorkDocData.AddDocToEmployee(dialog.DocID, Environment.CurEmp.ID);

						if(dialog.ImageID > 0 && dialog.FaxID > 0)
						{
							Environment.FaxData.CheckFax(dialog.FaxID, dialog.ImageID);
						}

						if(work)
						{
							force = !dialog.AddToWork;
							sendMail = dialog.SendMessage && !dialog.CreateSlaveEForm;
							parentDocIDs = dialog.ParentDocIDs;
							childDocIDs = dialog.ChildDocIDs;
							sendString = Environment.StringResources.GetString("DocControl_SavePart_AddDBDocDialog_DialogEvent_Message1") + " " + dialog.DocString;
							if(dialog.NeedOpenDoc && dialog.DocID > 0)
								Environment.OnNewWindow(this, new Components.DocumentSavedEventArgs(dialog.DocID, dialog.ImageID));
							Send(dialog.DocID);
						}

						OnDocChanged(this,  new Components.DocumentSavedEventArgs(dialog.DocID, dialog.ImageID, work && dialog.GotoDoc, work && dialog.CreateEForm, dialog.CreateSlaveEForm));
						OnPageChange(this, e);
					}

					break;
			}

			tf = Environment.GetTmpFileByValue(dialog.OldFileName);
			if(tf != null)
			{
				tf.CurAct = Environment.ActionBefore.None;
				tf.LinkCnt--;
			}
		}

		private void SendMessageAfterSave_DialogEvent(object source, DialogEventArgs e)
		{
			Focus();

			var dialog = e.Dialog as Dialogs.SendMessageDialog;
			if(dialog == null)
				return;
			if(dialog.DialogResult == DialogResult.OK)
				return;
			if(dialog.Forced)
				foreach(int docID in dialog.DocIDs)
					Environment.WorkDocData.AddDocToEmployee(docID, Environment.CurEmp.ID);
		}

		#region Print

		public void PrintAction(bool print, int docID, int imageID, string fileName, int startPage, int endPage,
								int countPage, string docName, short copyCount)
		{
			try
			{
				TmpFile tf = Environment.GetTmpFileByKey(fileName);
				if(tf != null && tf.CurAct == Environment.ActionBefore.Print)
				{
					if(!File.Exists(tf.TmpFullName))
						SaveToTmpCopy(fileName, tf.TmpFullName, IsPDFMode);

					fileName = tf.TmpFullName;
					docID = 0;
					imageID = -1;
				}

				if(Environment.DocToPrint.Contains(fileName))
				{
					var dialog = Environment.DocToPrint[fileName] as Dialogs.DocPrintDialog;
					if(dialog != null)
					{
						dialog.BringToFront();
						dialog.Activate();
					}
				}
				else
				{
					if(File.Exists(fileName))
					{
						var dialog = new Dialogs.DocPrintDialog(docID, imageID, fileName, IsPDFMode, startPage, endPage, countPage,
																	Environment.Settings.Folders.Add("Print"), docName);

						if(print)
						{
							if(!dialog.AlwaysShow)
								dialog.Print();
							else
							{
								dialog.DialogEvent += DocPrintDialog_DialogEvent;
								dialog.Show();
							}
						}
						else
							dialog.Show();

						if(!print || dialog.AlwaysShow)
						{
							Environment.DocToPrint.Add(fileName, dialog);
							if(tf == null || tf.CurAct == Environment.ActionBefore.Print)
							{
								if(tf == null)
									tf = Environment.GetTmpFile(fileName);
								if(tf != null)
									tf.LinkCnt++;
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, fileName);
				MessageBox.Show(ex.Message, Environment.StringResources.GetString("Error"));
			}
		}

		private void DocPrintDialog_DialogEvent(object source, DialogEventArgs e)
		{
			var dialog = e.Dialog as Dialogs.DocPrintDialog;
			if(dialog == null)
				return;

			if(!string.IsNullOrEmpty(dialog.FileName))
			{
				if(Environment.DocToPrint.Contains(dialog.FileName))
					Environment.DocToPrint.Remove(dialog.FileName);

				Document.Objects.TmpFile tf = Environment.GetTmpFileByValue(dialog.FileName);
				if(tf != null)
				{
					tf.CurAct = Environment.ActionBefore.None;
					tf.LinkCnt--;
				}
			}

			if(dialog.DialogResult == DialogResult.OK)
				dialog.Print();
			else if(TestPrinter.CheckPrinterExists())
				TestPrinter.SetPrinterProfile(ProfileType.TiffProfile);
		}

		public void Print()
		{
			TestImage(Environment.ActionBefore.Print);
			ImageControl.ImageControl.PrintActionHandler handler = PrintAction;
			if(imagePDF.ImageDisplayed)
				imagePDF.Print(handler, curDocID, imgID, CurDocString, 1);
			else
				image.Print(handler, curDocID, imgID, CurDocString, 1);
		}

		public void PrintSelection()
		{
			if(signDocumentPanel.IsSignInternal)
				return;
			ImageControl.ImageControl.PrintActionHandler handler = PrintAction;
			if(imagePDF.ImageDisplayed)
				imagePDF.PrintSelection(handler, CurDocString, 1);
			else
				image.PrintSelection(handler, curDocID, imgID, CurDocString, 1);
		}

		public void SelectScanner()
		{
			image.SelectScanner();
		}

		public void PrintSettings()
		{
			try
			{
				var dialog = new Dialogs.DocPrintDialog(0, Environment.Settings.Folders.Add("Print"), 0, Environment.StringResources.GetString("PrintSettings"));
			    dialog.Text = Environment.StringResources.GetString("PrintSettings");
				dialog.Show();
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
			}
		}

		public void PrintPage()
		{
			TestImage(Environment.ActionBefore.Print);
			ImageControl.ImageControl.PrintActionHandler handler = PrintAction;

			if(imagePDF.ImageDisplayed)
				imagePDF.PrintPage(handler, curDocID, imgID, imagePDF.Page, CurDocString, 1);
			else
				image.PrintPage(handler, curDocID, imgID, image.Page, CurDocString, 1);
		}

		public void PrintEForm()
		{
			try
			{
				var dialog = new Dialogs.DocPrintDialog(Environment.Settings.Folders.Add("Print"),
																   curDocID, CurDocString);
				dialog.DialogEvent += printDialog_Browser_DialogEvent;
				dialog.Show();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, Environment.StringResources.GetString("Error"));
			}
		}

		public bool PrintDocument()
		{
			int docTypeID = 0;
			if(hasData && canPrintEForm)
				docTypeID = Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, curDocID);

			try
			{
				var dialog = new Dialogs.DocPrintDialog(docTypeID, Environment.Settings.Folders.Add("Print"),
																   curDocID, CurDocString);
				dialog.DialogEvent += printDialog_Multi_DialogEvent;
				dialog.Show();
				return true;
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
				return false;
			}
		}

		public bool PrintDocument(int docID, bool printImage, bool mainOnly, bool eForm, ref Hashtable eFormSelection,
								  short countCopies)
		{
			if(printImage)
			{
				if(mainOnly)
				{
					object obj = Environment.DocData.GetField(Environment.DocData.MainImageIDField, docID);
					if(obj != DBNull.Value)
						PrintImageAll(docID, (int)obj, countCopies);
				}
				else
				{
					using(DataTable dti = Environment.DocImageData.GetDocImages(docID))
					using(DataTableReader dr = dti.CreateDataReader())
					{
						if(dr.HasRows)
						{
							while(dr.Read())
							{
								var imgId = (int)dr[Environment.DocImageData.IDField];

								PrintImageAll(docID, imgId, countCopies);
							}
						}
						dr.Close();
						dr.Dispose();
						dti.Dispose();
					}
				}
			}

			// есть ли данные у документа
			if(eForm && Environment.DocDataData.IsDataPresent(docID))
			{
				int docTypeID = Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, docID, -1);
				if(docTypeID > -1)
				{
					if(eFormSelection == null)
						eFormSelection = new Hashtable();
					if(eFormSelection.ContainsKey(docTypeID))
					{
						var list = eFormSelection[docTypeID] as List<PrinterObjectClass>;
						if(list != null)
							using(DataTable dt = Environment.PrintData.GetEFormPrintTypeData(docTypeID, docID))
							{
								bool show = false;
								if(dt.Rows.Count > 0)
								{
									foreach(PrinterObjectClass t in list)
									{
										show = true;
										for(int i = 0; i < dt.Rows.Count; i++)
										{
											if(dt.Rows[i][Environment.PrintData.IDField].Equals(t.TypeID))
											{
												show = false;
												break;
											}
										}
										if(show)
											break;
									}
									if(show)
									{
										var dialog = new Dialogs.PrintEFormSelectDialog(docTypeID, docID);
										if(dialog.ShowDialog() == DialogResult.OK)
										{
											list = dialog.PrinterObjectList;
											eFormSelection.Add(docTypeID, list);
										}
									}
								}
								dt.Dispose();
							}
						Action<List<PrinterObjectClass>, int, short> switchPrintEform = StartSwitchPrintEform;

						if(list != null)
							switchPrintEform.BeginInvoke(list, docID, countCopies, FinishSwitchPrintEform, switchPrintEform);
					}
					else
					{
						using(DataTable dt = Environment.PrintData.GetEFormPrintTypeData(docTypeID, docID))
						{
							Data.DALC.Documents.SettingsPrintForm[] settings = Environment.SettingsPrintForm.GetSettings(docTypeID);
							if(settings != null)
							{
								var list = new List<PrinterObjectClass>();
								foreach(DataRow dr in dt.Rows)
									list.AddRange(from t in settings
												  where dr[Environment.PrintData.IDField].Equals(t.PrintID)
												  select
													  new PrinterObjectClass((int)dr[Environment.PrintData.IDField],
																			 dr[Environment.PrintData.NameField].
																				 ToString(),
																			 dr[Environment.PrintData.URL].ToString(),
																			 Convert.ToInt32(
																				 dr[Environment.PrintData.PrintIDField]),
																			 (short)
																			 dr[Environment.PrintData.PaperSizeField]));
								using(DataTable dtpt = Environment.PrintData.GetEFormPrintTypeData(docTypeID, docID))
								{
									bool show = false;
									if(dtpt.Rows.Count > 0)
									{
										foreach(PrinterObjectClass t in list)
										{
											show = true;
											for(int i = 0; i < dtpt.Rows.Count; i++)
											{
												if(dtpt.Rows[i][Environment.PrintData.IDField].Equals(t.PrintID))
												{
													show = false;
													break;
												}
											}
											if(show)
												break;
										}
										if(show)
										{
											var dialog = new Dialogs.PrintEFormSelectDialog(docTypeID, docID);

											list = dialog.ShowDialog() == DialogResult.OK ? dialog.PrinterObjectList : new List<PrinterObjectClass>();
										}
									}
									if(list.Count > 0)
									{
										eFormSelection.Add(docTypeID, list);
										try
										{
											Action<List<PrinterObjectClass>, int, short> switchPrintEform =
												StartSwitchPrintEform;

											if(list != null)
												switchPrintEform.BeginInvoke(list, docID, countCopies, FinishSwitchPrintEform,
																			 switchPrintEform);
										}
										catch(Exception ex)
										{
											Data.Env.WriteToLog(ex);
											ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
										}
										return true;
									}
									dtpt.Dispose();
								}
							}
							try
							{
								var dialog = new Dialogs.PrintEFormSelectDialog(docTypeID, docID);
								if(dialog.ShowDialog() == DialogResult.OK)
								{
									List<PrinterObjectClass> list = dialog.PrinterObjectList;
									eFormSelection.Add(docTypeID, list);

									Action<List<PrinterObjectClass>, int, short> switchPrintEform = StartSwitchPrintEform;

									if(list != null)
										switchPrintEform.BeginInvoke(list, docID, countCopies, FinishSwitchPrintEform, switchPrintEform);
								}
							}
							catch(Exception ex)
							{
								Data.Env.WriteToLog(ex);
								ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
							}
							dt.Dispose();
						}
					}
				}
			}
			return true;
		}

		private static void PrintImageAll(int docID, int imageID, short countCopies)
		{
			if(imageID > 0)
			{
				if(Environment.DocSignatureData.IsDocSignedDSP(docID, imageID))
					return;
				bool isPdf =
					"pdf".Equals(
						Environment.DocImageData.GetField(Environment.DocImageData.ImageTypeField, imageID).ToString(),
						StringComparison.CurrentCultureIgnoreCase);

				using(
					var dial = new Dialogs.DocPrintDialog(docID, imageID, "", 1, 1, 0,
														  Environment.Settings.Folders.Add("Print"), countCopies, isPdf)
					)
					dial.Print(true);
			}
		}

		#endregion

		public bool SetImagePalette(int i)
		{
			if(IsPDFMode)
			{
				image.SetImagePalette(i);
				return imagePDF.SetImagePalette(i);
			}
			else
			{
				imagePDF.SetImagePalette(i);
				return image.SetImagePalette(i);
			}
		}

		public int GetImagePalette()
		{
			return IsPDFMode ? imagePDF.GetImagePalette() : image.GetImagePalette();
		}

		public bool SetDisplayScaleAlgorithm(int type)
		{
			if(IsPDFMode)
			{
				image.SetDisplayScaleAlgorithm(type);
				return imagePDF.SetDisplayScaleAlgorithm(type);
			}
			else
			{
				imagePDF.SetDisplayScaleAlgorithm(type);
				return image.SetDisplayScaleAlgorithm(type);
			}
		}

		public int GetDisplayScaleAlgorithm()
		{
			return IsPDFMode ? imagePDF.GetDisplayScaleAlgorithm() : image.GetDisplayScaleAlgorithm();
		}

		public void SetSelection()
		{
			image.SetSelection();
		}

		public bool RectDrawn()
		{
			return image.RectDrawn() || imagePDF.RectDrawn();
		}

		public void SetImage(int imageID, bool isPdf)
		{
			panelMain.Visible = true;
			if(isPdf)
			{
				image.Visible = false;
				imagePDF.ImageID = imageID;
				imagePDF.Visible = true;
				image.ImageID = 0;
			}
			else
			{
				imagePDF.Visible = false;
				image.ImageID = imageID;
				image.Visible = true;
				imagePDF.ImageID = 0;
			}
		}

		public bool CreateEForm(bool createNew, int ID)
		{
			return CreateEForm(createNew, ID, 0, 0, SelectTypeDialog.Direction.No);
		}

		public bool CreateEForm(bool createNew, int ID, int pDID)
		{
			ClearDoc();
			panelMain.Visible = true;
			newEForm = true;
			CursorSleep();

			splitContainerDocument.Panel2Collapsed = false;

			string url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, ID).ToString();
			if(string.IsNullOrEmpty(url))
				throw new Exception("Нет формы ответа");

			url += (url.IndexOf("?") > 0) ? "&" : "?";
			url += "docview=" + (IsMain ? "1" : "2") + "&docid=" + pDID.ToString();
			if(browser.Url == null || browser.Url.AbsoluteUri != url)
			{
				browser.SelfNavigate = true;
				browser.Url = new Uri(url);
			}
			CursorWake();
			return true;
		}

		public bool CreateEForm(bool createNew, int ID, int pDID, int fieldID,
								SelectTypeDialog.Direction outGoing)
		{
			if(createNew)
			{
				ClearDoc();
				panelMain.Visible = true;
				newEForm = true;
				CursorSleep();

				splitContainerDocument.Panel2Collapsed = false;

				string url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, ID).ToString();
				if(string.IsNullOrEmpty(url))
					url = Environment.EFormString + ID + "&";
				else
					url += (url.IndexOf("?") > 0) ? "&" : "?";

				url += "docview=" + (IsMain ? "1" : "2") +
					   ((Environment.PersonID > 0) ? ("&currentperson=" + Environment.PersonID.ToString()) : "") +
					   ((pDID > 0)
							? "&DocID=" + pDID.ToString() + ((fieldID > 0) ? "fieldid=" + fieldID.ToString() : "")
							: "") +
					   (outGoing.Equals(SelectTypeDialog.Direction.Out)
							? "&docDir=out"
							: (outGoing.Equals(SelectTypeDialog.Direction.In) ? "&docDir=in" : ""));

				if(browser.Url == null || browser.Url.AbsoluteUri != url)
				{
					browser.SelfNavigate = true;
					browser.Url = new Uri(url);
				}
				CursorWake();
				return true;
			}

			if(hasData)
			{
				if(!showWebPanel)
				{
					for(int i = 0; i < variantList.Items.Count; i++)
					{
						var item = variantList.Items[i] as VariantListItem;
						if(item != null && item.Type == VariantType.Data)
						{
							item.Selected = true;
							break;
						}
					}
				}
				else
				{
					ShowHTMLDoc();
				}
			}
			else
			{
				string url = "";
				int docTypeID = Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, curDocID, -1);
				if(docTypeID > -1)
					url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, docTypeID).ToString();
				if(string.IsNullOrEmpty(url))
					url = Environment.SettingsURLString;

				url = url.IndexOf("id=") > 0
						  ? url.Replace("id=", "id=" + curDocID.ToString())
						  : (url + ((url.IndexOf("?") > 0) ? "&" : "?") + "id=" + curDocID.ToString());
				Environment.IEOpenOnURL(url);
			}
			return false;
		}

		public void MovePage(int page)
		{
			if(!IsPDFMode)
				image.MovePage(page);
			else
				imagePDF.MovePage(page);
		}

		private void UpdateNavigation()
		{
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(UpdateNavigation));
				return;
			}
			if(Disposing || IsDisposed)
				return;
			try
			{
				if(image != null || imagePDF != null)
				{
					toolStripThumb.CheckState = ((image != null && image.ShowThumbPanel) || (imagePDF != null && imagePDF.ShowThumbPanel))
													? CheckState.Checked
													: CheckState.Unchecked;
					if(image != null && image.ImageDisplayed)
					{
						toolStripComboZoom.Enabled = //true;
							toolStripPage.Enabled = //true;
							toolStripRotateLeft.Enabled = //true;
							toolStripRotateRight.Enabled = // true;
							toolStripZoomIn.Enabled = // true;
							toolStripZoomOut.Enabled = true;
						toolStripPage.Text = image.Page.ToString();
						toolStripPrevPage.Enabled = image.Page > 1;
						toolStripNextPage.Enabled = image.Page < image.PageCount;
					}
					else if(imagePDF != null && imagePDF.ImageDisplayed)
					{
						toolStripComboZoom.Enabled = //true;
							toolStripPage.Enabled = //true;
							toolStripRotateLeft.Enabled = //true;
							toolStripRotateRight.Enabled = // true;
							toolStripZoomIn.Enabled = // true;
							toolStripZoomOut.Enabled = true;
						toolStripPage.Text = imagePDF.Page.ToString();
						toolStripPrevPage.Enabled = imagePDF.Page > 1;
						toolStripNextPage.Enabled = imagePDF.Page < imagePDF.PageCount;
					}
					else
					{
						toolStripPage.Text = "";
						toolStripComboZoom.Enabled = //false;
							toolStripPage.Enabled = //false;
							toolStripPrevPage.Enabled = //false;
							toolStripNextPage.Enabled = //false;
							toolStripRotateLeft.Enabled = //false;
							toolStripRotateRight.Enabled = //false;
							toolStripZoomIn.Enabled = //false;
							toolStripZoomOut.Enabled = false;
					}
					toolStripPrint.Enabled = curDocID > 0 || (image != null && image.ImageDisplayed) ||
											 (imagePDF != null && imagePDF.ImageDisplayed);
				}
				else
				{
					toolStripComboZoom.Enabled = false;
					toolStripPage.Enabled = false;
					toolStripPage.Text = "";
					toolStripPrevPage.Enabled = false;
					toolStripNextPage.Enabled = false;
					toolStripRotateLeft.Enabled = false;
					toolStripRotateRight.Enabled = false;
					toolStripZoomIn.Enabled = false;
					toolStripZoomOut.Enabled = false;
					toolStripPrint.Enabled = false;
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		public string SaveToTmpCopy(string srcFile, string tmpFileName, bool ispdf)
		{
			string newFileName = tmpFileName;
			if(File.Exists(srcFile))
			{
				if(string.IsNullOrEmpty(newFileName))
					newFileName = Environment.GenerateFullFileName(Path.GetExtension(srcFile).TrimStart('.'));

				try
				{
					File.Copy(srcFile, newFileName, true);
					File.SetAttributes(newFileName, FileAttributes.Normal);
				}
				catch
				{
					return "";
				}

				//Environment.AddTmpFile(srcFile, newFileName, ispdf);
			}
			return newFileName;
		}

		#endregion

		#region Properites

		public string ZoomText
		{
			get { return toolStripComboZoom.Text; }
			set { toolStripComboZoom.Text = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanPrint
		{
			get
			{
				try
				{
                    return canPrintEForm || ((imgID < 1 || !IsSignInternal) && ((image != null && image.ImageDisplayed) || (imagePDF != null && imagePDF.ImageDisplayed))
							   && !(curDocID != 0 && imgID == 0));
				}
				catch
				{
					return false;
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanSendOut
		{
			get
			{
				return ImageID < 0 || (!signDocumentPanel.IsSignInternal && !signDocumentPanel.IsSignCancel);
			}
		}

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSignInternal
        {
            get
            {
                return ImageID > 1 && signDocumentPanel.IsSignInternal;
            }
        }


		public Point SplinterPlace
		{
			get { return new Point((!IsPDFMode ? image.SplinterPlace : imagePDF.SplinterPlace), variantList.Height); }
			set
			{
				image.SplinterPlace = imagePDF.SplinterPlace = value.X;
				variantList.Height = value.Y;
			}
		}

		public bool SelectionMode
		{
			get { return image.IsSelectionMode; }
			set
			{
				image.IsSelectionMode = value;
				imagePDF.SelectionMode = value;
			}
		}

		public bool IsEditNotes
		{
			get { return image.IsEditNotes || imagePDF.IsEditNotes; }
			set
			{
				image.IsEditNotes = value;
				imagePDF.IsEditNotes = value;
			}
		}

		public bool IsMoveImage
		{
			get { return image.IsMoveImage || imagePDF.IsMoveImage; }
			set
			{
				image.IsMoveImage = value;
				imagePDF.IsMoveImage = value;
			}
		}

		public bool Modified
		{
			get { return (image.ImageDisplayed && image.Modified) || (imagePDF.ImageDisplayed && imagePDF.Modified); }
		}

		public bool ImageDisplayed
		{
			get
			{
				return ((DocumentID == 0) || (showWebPanel || imgID > 0)) &&
					   ((image != null && image.ImageDisplayed) || (imagePDF != null && imagePDF.ImageDisplayed));
			}
		}

		public int Page
		{
			get
			{
				if(imagePDF != null && imagePDF.ImageDisplayed)
					return imagePDF.Page;
				if(image != null && image.ImageDisplayed)
					return image.Page;
				return 0;
			}
			set
			{
				if(imagePDF != null && imagePDF.ImageDisplayed)
				{
					if(imagePDF.Page != value)
						imagePDF.Page = (value > 0) ? value : 1;
				}
				else if(image != null && image.Page != value)
					if(image.ImageDisplayed)
					{
						if(image.Modified && CanSave && !docComponent.IsFax())
						{
							var mf = new MessageForm(Environment.StringResources.GetString("DocControl_image_NeedSave_Message1") + " (" +
									CurDocString + ", " + Environment.StringResources.GetString("Page") + " " +
									image.Page.ToString() + ")?", Environment.StringResources.GetString("Save1"),
									MessageBoxButtons.YesNo);
							if(mf.ShowDialog(FindForm()) == DialogResult.Yes)
							{
								try
								{
									if(docComponent.IsDocument())
										SaveDocImage();
									else
										image.Save();
								}
								catch(Exception ex)
								{
									Data.Env.WriteToLog(ex);
									ErrorShower.OnShowError(this, Environment.StringResources.GetString("DocControl_Page_Error1") +
															":" + System.Environment.NewLine + System.Environment.NewLine + ex.Message,
															Environment.StringResources.GetString("FileNotSaved"));
								}
							}
						}
						else
							OnNeedSave();
						image.Page = value;
					}
					else
					{
						try
						{
							if(imgID > 0 && image.Page == 0)
								image.Page = 1;
						}
						catch(Exception ex)
						{
							Data.Env.WriteToLog(ex);
						}
					}
			}
		}

		public int PageCount
		{
			get
			{
				return imagePDF.ImageDisplayed ? imagePDF.PageCount : image.PageCount;
			}
		}

		public int ImageCount
		{
			get { return variantList.Items.Count; }
		}

		public Guid Subscribe
		{
			get { return subscribe; }
			set { subscribe = value; }
		}

		#endregion

		#region Save Style

		public Components.DocControlComponent SetStyle(Components.DocControlComponent component)
		{
			return component;
		}

		public Components.DocControlComponent SetNullComponent()
		{
			if(docComponent.FileName != null)
				docComponent = new Components.DocControlComponent(Container, this);
			return docComponent;
		}

		public Components.DocControlComponent CreateDocumentComponent(int docID, int imgID, ServerInfo server)
		{
			if(!docComponent.IsDocument())
				docComponent = new Components.DocumentComponent(components, this);

			docComponent.ID = docID;
			docComponent.ImageID = imgID;
			docComponent.ServerInfo = server;
			StartReceiverTimer();
			return docComponent;
		}

		public Components.DocControlComponent CreateFaxInComponent(int faxID)
		{
			if(!docComponent.IsFaxIn())
			{
				docComponent = new Components.SaveFaxInComponent(components, this);
				if(Environment.IsConnectedDocs)
					docComponent.ServerInfo = Environment.GetRandomLocalServer();
			}
			docComponent.ID = faxID;
			if(!image.ImageDisplayed && faxWatcher.Path.Length > 0 && faxWatcher.Filter.Length > 0)
				if(File.Exists(faxWatcher.Path + "\\" + faxWatcher.Filter))
					image.FileName = faxWatcher.Path + "\\" + faxWatcher.Filter;
				else
				{
					faxWatcher.Created += faxWatcher_Created;
					faxWatcher.Renamed += faxWatcher_Renamed;
				}

			return docComponent;
		}

		public Components.DocControlComponent CreateFaxOutComponent(int faxID)
		{
			if(!docComponent.IsFaxOut())
			{
				docComponent = new Components.SaveFaxOutComponent(components, this);
				if(Environment.IsConnectedDocs)
					docComponent.ServerInfo = Environment.GetRandomLocalServer();
			}
			docComponent.ID = faxID;
			return docComponent;
		}

		public Components.DocControlComponent CreateScanerComponent()
		{
			return CreateScanerComponent(false);
		}

		public Components.DocControlComponent CreateScanerComponent(bool isPdf)
		{
			if(!docComponent.IsScaner())
			{
				docComponent = new Components.SaveScanerComponent(components, this);
				if(Environment.IsConnectedDocs)
					docComponent.ServerInfo = Environment.GetRandomLocalServer();
			}
			docComponent.IsPdf = isPdf;
			return docComponent;
		}

		public Components.DocControlComponent CreateFromScanerComponent()
		{
			if(!docComponent.IsScaner())
			{
				docComponent = new Components.SaveFromScanerComponent(components, this) { Contr = this };
				if(Environment.IsConnectedDocs)
					docComponent.ServerInfo = Environment.GetRandomLocalServer();
			}
			return docComponent;
		}

		public Components.DocControlComponent CreateFileComponent(bool isPdf, bool isOutlook)
		{
			if(!(docComponent is Components.SaveToArchiveComponent))
				docComponent = new Components.SaveToArchiveComponent(components, this);
			if(Environment.IsConnectedDocs)
				docComponent.ServerInfo = Environment.GetRandomLocalServer();
			docComponent.IsPdf = isPdf;
			docComponent.IsOutlook = isOutlook;
			return docComponent;
		}

		public static void docComponent_FaxInContactCreated(object sender, Components.NogeIdEventArgs e)
		{
			var docControl = sender as DocControl;
			if(docControl != null)
				docControl.OnNeedRefreshGrid(sender, e);
		}

		#endregion

		#region Scan

		/// <summary>
		///   Обратный вызов. Сохранение нового изображения
		/// </summary>
		private void ScanNewDocumentForCurrentDocHandler(object arg)
		{
			if(arg != null)
			{
				string file = arg.ToString();
				if(file != null && file != "" && File.Exists(file) && curDocID > 0)
				{
					var mf = new MessageForm( Environment.StringResources.GetString("DocControl_image_NeedSave_Message1") + " (" + CurDocString + ", " + Environment.StringResources.GetString("Page") + " " +
							Page.ToString() + ")?", Environment.StringResources.GetString("Save1"), MessageBoxButtons.YesNo);
					if(mf.ShowDialog(FindForm()) == DialogResult.Yes)
					{
						DateTime creationTime = DateTime.Now;
						ServerInfo server = null;
						string fileName = null;
						if(MoveFile(file, ref creationTime, out fileName, out server))
						{
							int imgID = 0;
							Environment.DocImageData.DocImageInsert(server.ID, fileName, ref imgID, ref curDocID, 0, "", DateTime.MinValue, "", "", false, creationTime, 0, true, "TIF", PageCount);
						}
					}
				}
			}
			RefreshDoc();
		}

		private bool MoveFile(string file, ref DateTime creationTime, out string fileName,
							  out ServerInfo server)
		{
			var fileInfo = new FileInfo(file);
            Console.WriteLine("{0}: file = {1}", DateTime.Now.ToString("HH:mm:ss fff"), file);
			server = Environment.GetRandomLocalServer();
			fileName = Environment.GenerateFileName("tif");
			string path = server.Path + "\\TEMP\\" + fileName;
			Console.WriteLine("{0}: path = {1}",DateTime.Now.ToString("HH:mm:ss fff"), path);
			if(File.Exists(path))
				File.Delete(path);
			creationTime = fileInfo.CreationTimeUtc;
			File.Move(fileInfo.FullName, path);
			File.Delete(fileInfo.FullName);
			return File.Exists(path);
		}

		/// <summary>
		///   Добавление нового изображения текщему документу со сканер
		/// </summary>
		/// <returns> </returns>
		public void ScanNewCurrentDoc()
		{
			string scanFileName = Path.GetTempPath() + Environment.GenerateFileName("tif");
			image.Scan(ScanNewDocumentForCurrentDocHandler, scanFileName);
		}

		/// <summary>
		///   Метод который надо выполнить в вызывающем коде
		/// </summary>
		private ImageControl.Scaner.CallbackHandler CurrentCallback;

		private void ScanNewDocumentHandler(object arg)
		{
			if(arg != null)
			{
				string fileNameTemp = arg.ToString();
				if(fileNameTemp != null && fileNameTemp != "" && File.Exists(fileNameTemp))
				{
					//image.SetModifiedForNewDocument(true);
					fileName = fileNameTemp;
					splitContainerBrowser.Panel2Collapsed = true;
					splitContainerDocument.Panel2Collapsed = true;
					splitContainerWhole.Panel1Collapsed = true;
					panelMain.Visible = true;
					CreateFromScanerComponent();
					UpdateNavigation();
					if(CurrentCallback != null)
						CurrentCallback(arg);
				}
				else
				{
					CurrentCallback = null;
					ClearDoc();
				}
			}
			else
				ClearDoc();
		}

		public void ScanNewDocument(ImageControl.Scaner.CallbackHandler callback)
		{
			panelMain.Visible = true;

			CurrentCallback = callback;
			string scanFileName = Path.GetTempPath() + Environment.GenerateFileName("tif");
			image.Scan(ScanNewDocumentHandler, scanFileName);
		}

		#endregion

		#region Send

		private void SendFromChild(int docID)
		{
			if(childDocIDs.Count > 0)
			{
                Console.WriteLine("{0}: Start link form", DateTime.Now.ToString("HH:mm:ss fff"));
				if(!Environment.DocLinksData.CheckDocLinkExists(docID, childDocIDs[0]))
					Environment.DocLinksData.AddDocLink(docID, childDocIDs[0]);
                Console.WriteLine("{0}: Parent: {1}, Child: {2}", DateTime.Now.ToString("HH:mm:ss fff"), docID, childDocIDs[0]);
				childDocIDs.RemoveAt(0);
				SendFromChild(docID);
			}
			else
			{
				if(sendMail)
				{
					sendMail = false;
                    Console.WriteLine("{0}: Send Message", DateTime.Now.ToString("HH:mm:ss fff"));
					var senddialog = new Dialogs.SendMessageDialog(docID, sendString, force);
					senddialog.DialogEvent += SendMessageAfterSave_DialogEvent;
					senddialog.Show();
				}
			}
		}

		private void Send(int docID)
		{
			if(parentDocIDs.Count > 0)
			{
                Console.WriteLine("{0}: Start link form", DateTime.Now.ToString("HH:mm:ss fff"));
				if(!Environment.DocLinksData.CheckDocLinkExists(parentDocIDs[0], docID))
					Environment.DocLinksData.AddDocLink(parentDocIDs[0], docID);
                Console.WriteLine("{0}: Parent: {1}, Child: {2}", DateTime.Now.ToString("HH:mm:ss fff"), parentDocIDs[0], docID);
				parentDocIDs.RemoveAt(0);
				Send(docID);
			}
			else
			{
				SendFromChild(docID);
			}
		}

		#endregion

		#region Splitter

		private void splitContainerBrowser_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if(!splitContainerBrowser.Panel1Collapsed && !splitContainerBrowser.Panel2Collapsed)
			{
				Environment.Layout.OptionForced<int>("SplitterExportPlace").Value = splitContainerBrowser.SplitterDistance;
			}
		}

		private void splitContainerDocument_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if(!splitContainerDocument.Panel1Collapsed && !splitContainerDocument.Panel2Collapsed)
			{
				Environment.Layout.OptionForced<int>("SpliterDataPlace").Value = splitContainerDocument.SplitterDistance;
			}
		}

		private void splitContainerWhole_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if(!splitContainerWhole.Panel1Collapsed && !splitContainerWhole.Panel2Collapsed)
			{
				Environment.Layout.OptionForced<int>("SpliterImagePlace").Value = splitContainerWhole.SplitterDistance;
			}

			signDocumentPanel.DoSizeChanged();
		}

		#endregion

		#region ToolBar

		private void toolStripThumb_Click(object sender, EventArgs e)
		{
			bool show = toolStripThumb.CheckState != CheckState.Checked;
			toolStripThumb.CheckState = show ? CheckState.Checked : CheckState.Unchecked;
			image.ShowThumbPanel = imagePDF.ShowThumbPanel = show;
		}

		private void toolStripPrevPage_Click(object sender, EventArgs e)
		{
			Page--;
		}

		private void toolStripNextPage_Click(object sender, EventArgs e)
		{
			Page++;
		}

		private void toolStripComboZoom_TextChanged(object sender, EventArgs e)
		{
			string zoom = toolStripComboZoom.Text.ToLower();

			if(zoom == Environment.StringResources.GetString("ToWindow"))
			{
				image.FitTo(0, true);
				imagePDF.FitTo(0, true);
			}
			else if(zoom == Environment.StringResources.GetString("ToWidth"))
			{
				image.FitTo(1, true);
				imagePDF.FitTo(1, true);
			}
			else if(zoom == Environment.StringResources.GetString("ToHeight"))
			{
				image.FitTo(2, true);
				imagePDF.FitTo(2, true);
			}
			else
				try
				{
					zoom = zoom.Replace("%", String.Empty);
					int intZ = Int32.Parse(zoom);
					if(intZ < ImageControl.ImageControl.MinZoom)
					{
						throw new Exception("zoom value out of range");
					}

					if(intZ > ImageControl.ImageControl.MaxZoom)
					{
						MessageBox.Show(
							Environment.StringResources.GetString("DocControl_zoomCombo_SelectedIndexChanged_Error1") +
							" : " + ImageControl.ImageControl.MaxZoom.ToString() + "%",
							Environment.StringResources.GetString("InputError"));
						throw new Exception("Upper Overflow");
					}

					if(Zoom != intZ)
					{
						Zoom = intZ;
						//ZoomText = Zoom.ToString();
					}
				}
				catch(Exception ex)
				{
					if(ex.Message == "Upper Overflow")
					{
						toolStripComboZoom.Text = ImageControl.ImageControl.MaxZoom.ToString() + "%";
						toolStripComboZoom.Focus();
					}
				}
			UpdateNavigation();
		}

		private void toolStripZoomIn_Click(object sender, EventArgs e)
		{
			ZoomText = (Zoom * 2).ToString();
		}

		private void toolStripZoomOut_Click(object sender, EventArgs e)
		{
			ZoomText = (Zoom / 2).ToString();
		}

		private void toolStripRotateLeft_Click(object sender, EventArgs e)
		{
			RotateLeft();
		}

		private void toolStripRotateRight_Click(object sender, EventArgs e)
		{
			RotateRight();
		}

		private void toolStripPrint_Click(object sender, EventArgs e)
		{
			if(image.ImageDisplayed || imagePDF.ImageDisplayed)
				PrintPage();
			else
			{
				var dialog = new Dialogs.DocPrintDialog(curDocID, 0, "", 1, 1, 1,
																		   Environment.Settings.Folders.Add("Print"), 1);
				dialog.DialogEvent += printDialog_Browser_DialogEvent;
				dialog.Show();
			}
		}

		private void toolStripPage_TextChanged(object sender, EventArgs e)
		{
			int page = 0;
			if(image != null && image.ImageDisplayed && Int32.TryParse(toolStripPage.Text, out page) && image.Page != page && page > 0 &&
				page <= image.PageCount)
			{
				image.Page = page;
			}
			UpdateNavigation();
		}

		#endregion

		#region VariantList

		public void RefreshDoc()
		{
			if(Disposing || IsDisposed)
				return;
			try
			{
				lock(lockObj)
				{
					variantList.SelectedIndexChanged -= variantList_SelectedIndexChanged;
					variantList.Items.Clear();

                    // Лечение reentrant call to SetCurrentCellAddressCore гребаного грида
					//Application.DoEvents();

					if(curDocID > 0)
					{
						panelMain.Visible = true;
						using(Log.Logger.DurationMetter("RefreshDoc() ForceNavigate"))
						if(browser.Url != null && browser.Url.AbsoluteUri != "about:blank")
						{
							browser.ForceNavigate("about:blank");
						}
						subscribe = Guid.NewGuid();
						if(imgID > -2)
						{
							try
							{
								fileName = "";

                                using (Log.Logger.DurationMetter("RefreshDoc() ReloadIcons"))
								    ReloadIcons();
							}
							catch(Exception ex)
							{
								Data.Env.WriteToLog(ex);
							}

							UpdateNavigation();
						}

						UpdateSignDocumentPanel();
					}
					else
					{
						UpdateNavigation();
						splitContainerBrowser.Panel2Collapsed = true;
						splitContainerDocument.Panel2Collapsed = true;
						splitContainerWhole.Panel1Collapsed = true;
						panelMain.Visible = false;

						ClearSignDocumentPanel();
					}

                    // Лечение reentrant call to SetCurrentCellAddressCore гребаного грида
					//Application.DoEvents();
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private void variantList_SelectedIndexChanged(object sender, EventArgs e)
		{
			canPrintEForm = false;

            // Лечение reentrant call to SetCurrentCellAddressCore гребаного грида
			// Application.DoEvents();

			DeleteReceiver();

			if(imgID > 0)
				TestImage();

			if(variantList.SelectedItems.Count == 0)
				return;

			var item = variantList.SelectedItems[0] as VariantListItem;
			if(item == null)
				return;

			ImageDate = item.CreateTime;
			splitContainerBrowser.Panel2Collapsed = true;
			if(item.Type != VariantType.Data && item.Type != VariantType.ICConnect &&
				item.Type != VariantType.ICMRConnect && item.Type != VariantType.ScalaConnect)
			{
				splitContainerDocument.Panel1Collapsed = !hasImage;
				ShowDataPanel(CanShowSplit());

				bool change = imgID != item.ID;
				compliteLoading = false;
				imgID = item.ID;

				if(item.IsPDF())
				{
					if(image.Visible)
					{
						image.FileName = "";
						image.Visible = false;
					}
					else
						image.SendToBack();

					int sPos = SplinterPlace.X;
					panelMain.Visible = true;
					imagePDF.Visible = true;
					imagePDF.SplinterPlace = sPos;
					if(change)
						imagePDF.ImageID = imgID;
					else
						imagePDF.LoadImagePage(imgID, page);
					ImageType = "PDF";
					CreateDocumentComponent(curDocID, imgID, imagePDF.CurrentServer);
				}
				else
				{
                    if (change)
                        page = 1;

                    int sPos = SplinterPlace.X;
					if(imagePDF.Visible)
					{
						imagePDF.FileName = "";
						imagePDF.Visible = false;
                    }
                    image.Visible = true;
                    image.SplinterPlace = sPos;
					ImageType = "TIF";
					image.LoadImagePage(imgID, page - 1);
					CreateDocumentComponent(curDocID, imgID, image.CurrentServer);
				}
				OnVarListIndexChange(sender);
				CursorWake();
			}
			else if(item.Type == VariantType.Data)
			{
				splitContainerDocument.Panel1Collapsed = !CanShowSplit();
				ShowDataPanel(true);
				if(imgID != 0)
					imgID = 0;

#if AdvancedLogging
                using (Log.Logger.DurationMetter("DocControl ShowHTMLDoc"))
#endif
				ShowHTMLDoc();
			}
			else
			{
				splitContainerDocument.Panel1Collapsed = hasData;
				if(hasData)
				{
					if(imgID != 0)
						imgID = 0;
					ShowHTMLDoc();
				}
				else
				{
					splitContainerBrowser.Panel1Collapsed = true;
					splitContainerDocument.Panel2Collapsed = false;
				}

				string url = null;
				switch(item.Type)
				{
					case VariantType.ICConnect:
						url = Environment.IcExportString + curDocID.ToString() + "&buhid=" + item.ID.ToString();
						break;
					case VariantType.ICMRConnect:
						url = Environment.ICMRExportString + curDocID.ToString() + "&buhid=" + item.ID.ToString();
						break;
					case VariantType.ScalaConnect:
						url = Environment.ScalaExportString + curDocID.ToString() + "&buhid=" + item.ID.ToString();
						break;
				}
				CursorSleep();
				int contl = Environment.Layout.LoadIntOption("SplitterExportPlace", splitContainerBrowser.SplitterDistance);
				splitContainerBrowser.Panel2Collapsed = false;
				splitContainerBrowser.SplitterDistance = contl;
				exportBrowser.NavigateTo(new Uri(url));
				CursorWake();
				OnVarListIndexChange(this);
			}
			signDocumentPanel.LoadDocInfo(curDocID, imgID > 0 ? (int?)imgID : null);

            Environment.UndoredoStack.RemoveFromStackRotateImage();
		}

		private void variantList_MouseUp(object sender, MouseEventArgs e)
		{
			switch(e.Button)
			{
				case MouseButtons.Right:
					{
						if(variantList.SelectedItems.Count > 0)
						{
							var item = variantList.SelectedItems[0] as VariantListItem;
							if(item != null)
								switch(item.Type)
								{
									case VariantType.MainImageOriginalPrinted:
									case VariantType.MainImagePrinted:
									case VariantType.ImageOriginalPrinted:
									case VariantType.ImagePrinted:
									case VariantType.MainImageOriginal:
									case VariantType.MainImage:
									case VariantType.ImageOriginal:
									case VariantType.Image:
										{
											var propertiesItem = new MenuItem();
											var setBeforeItem = new MenuItem();
											var setAfterItem = new MenuItem();
											var setMainItem = new MenuItem();
											var newDocItem = new MenuItem();
											var delDocItem = new MenuItem();
											var separator = new MenuItem();
											var separator2 = new MenuItem();
											switch(item.Type)
											{
												case VariantType.MainImageOriginalPrinted:
												case VariantType.MainImagePrinted:
												case VariantType.ImageOriginalPrinted:
												case VariantType.ImagePrinted:
													propertiesItem.Text = Environment.StringResources.GetString("PrintedImageProperties");
													break;
												default:
													propertiesItem.Text = Environment.StringResources.GetString("ImageProperties");
													break;
											}
											propertiesItem.Click += variantList_propertiesItem_Click;

											setBeforeItem.Text = Environment.StringResources.GetString("SetBefore");
											setBeforeItem.Enabled = !signDocumentPanel.IsSigned && !IsPDFMode;
											setBeforeItem.Click += setBeforeItem_Click;

											setAfterItem.Text = Environment.StringResources.GetString("SetAfter");
											setAfterItem.Enabled = !signDocumentPanel.IsSigned && !IsPDFMode;
											setAfterItem.Click += setAfterItem_Click;

											setMainItem.Text =
												Environment.StringResources.GetString("DocControl_variantList_MouseUp_Message1");
											setMainItem.Click += variantList_SetMainItem_Click;
											switch(item.Type)
											{
												case VariantType.MainImageOriginalPrinted:
												case VariantType.MainImagePrinted:
												case VariantType.ImageOriginalPrinted:
												case VariantType.ImagePrinted:
													delDocItem.Text =
														Environment.StringResources.GetString("DocControl_variantList_MouseUp_Message4");
													break;
												default:
													delDocItem.Text =
														Environment.StringResources.GetString("DocControl_variantList_MouseUp_Message2");
													break;
											}

											delDocItem.Click += variantList_DelDocItem_Click;

											newDocItem.Text =
												Environment.StringResources.GetString("DocControl_variantList_MouseUp_Message3");
											newDocItem.Click += variantList_newDocItem_Click;

											separator.Text = "-";
											separator2.Text = "-";
											var contextMenu = new ContextMenu();
											switch(item.Type)
											{
												case VariantType.ImageOriginalPrinted:
												case VariantType.ImagePrinted:
												case VariantType.ImageOriginal:
												case VariantType.Image:
													contextMenu.MenuItems.AddRange(new[]
                                                                                   {
                                                                                       setMainItem, delDocItem, newDocItem,
                                                                                       separator,
                                                                                       setBeforeItem, setAfterItem, separator2,
                                                                                       propertiesItem
                                                                                   });
													break;
												case VariantType.MainImageOriginalPrinted:
												case VariantType.MainImagePrinted:
													contextMenu.MenuItems.AddRange(new[]
                                                                                   {
                                                                                       delDocItem, /*newDocItem,*/ separator,
                                                                                       setBeforeItem, setAfterItem, separator2,
                                                                                       propertiesItem
                                                                                   });
													break;
												default:
													contextMenu.MenuItems.AddRange(new[]
                                                                                   {
                                                                                       setBeforeItem, setAfterItem, separator,
                                                                                       propertiesItem
                                                                                   });
													break;
											}
											contextMenu.Show(variantList, new Point(e.X, e.Y));
										}
										break;
									case VariantType.Data:
										{
											var mi = new MenuItem(Environment.StringResources.GetString("DeleteEForm"),
																  variantList_DelEForm_Click);
											var contextMenu = new ContextMenu(new[] { mi });
											contextMenu.Show(variantList, new Point(e.X, e.Y));
										}
										break;
								}
						}
					}
					break;
			}
			OnVarListMouseUp(sender, e);
		}

		private void setAfterItem_Click(object sender, EventArgs e)
		{
			image.ScanAfter(image.Page);
		}

		private void setBeforeItem_Click(object sender, EventArgs e)
		{
			image.ScanBefore(image.Page);
		}

		private void variantList_DelDocItem_Click(object sender, EventArgs e)
		{
			var mf = new MessageForm(
				Environment.StringResources.GetString("DocControl_variantList_DelClick_Message1") +
				Environment.StringResources.GetString("DocControl_variantList_DelDocItem_Click_Message1") + " " +
				variantList.SelectedItems[0].Text + System.Environment.NewLine +
				Environment.StringResources.GetString("DocumentD") + " " + CurDocString + " " +
				Environment.StringResources.GetString("DocControl_variantList_DelDocItem_Click_Message2")
				+ System.Environment.NewLine +
				Environment.StringResources.GetString("DocControl_variantList_DelDocItem_Click_Message3")
				+ System.Environment.NewLine + System.Environment.NewLine +
				Environment.StringResources.GetString("DocControl_variantList_DelDocItem_Click_Message4"),
				Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNo,
				MessageBoxDefaultButton.Button2) { Tag = imgID };
			mf.DialogEvent += DelDocItem_DialogEvent;
			mf.Show();
		}

		private void variantList_DelEForm_Click(object sender, EventArgs e)
		{
			if(Environment.DocSignatureData.IsImageSign(curDocID, null))
			{
				MessageForm.Show(
					Environment.StringResources.GetString("DocControl_variantList_DelEformClick_Error"),
					Environment.StringResources.GetString("DocControl_variantList_DelEformClick_ErrorTitle"));
				return;
			}
			var mf = new MessageForm(
				Environment.StringResources.GetString("DocControl_variantList_DelClick_Message1") +
				((hasImage)
					 ? (Environment.StringResources.GetString("DocControl_variantList_DelEForm_Click_Message1") + " " +
						CurDocString)
					 : (Environment.StringResources.GetString("Document") + " " + CurDocString)) + " " +
				Environment.StringResources.GetString("DocControl_variantList_DelDocItem_Click_Message2") +
				System.Environment.NewLine +
				Environment.StringResources.GetString("DocControl_variantList_DelEForm_Click_Message2") +
				System.Environment.NewLine + System.Environment.NewLine +
				((hasImage)
					 ? Environment.StringResources.GetString("DocControl_variantList_DelEForm_Click_Message3")
					 : Environment.StringResources.GetString("DocControl_variantList_DelEForm_Click_Message4")),
				Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNo,
				MessageBoxDefaultButton.Button2) { Tag = curDocID };
			mf.DialogEvent += DelEformItem_DialogEvent;
			mf.Show();
		}

		private void variantList_propertiesItem_Click(object sender, EventArgs e)
		{
			DocImageProperties();
		}

		private void variantList_SetMainItem_Click(object sender, EventArgs e)
		{
			if(variantList.SelectedItems.Count > 0)
			{
				var item = (VariantListItem)variantList.SelectedItems[0];

				if(item != null &&
					(item.Type == VariantType.Image || item.Type == VariantType.ImageOriginal ||
					 item.Type == VariantType.ImagePrinted || item.Type == VariantType.ImageOriginalPrinted))
				{
                    var origMainImage = Environment.DocData.GetDocIntField(Environment.DocData.MainImageIDField, curDocID);

                    if (Environment.DocData.SetMainImage(curDocID, item.ID))
                        UndoRedoStackSetMainImage(origMainImage, ImageID);

					RefreshDoc();
				}
			}
		}

        /// <summary>
        /// Формирование команды Undo SetMainImage
        /// </summary>
        /// <param name="origMainImage"></param>
        /// <param name="newMainImage"></param>
        private void UndoRedoStackSetMainImage(int origMainImage, int newMainImage)
        {
            Func<object[], bool> undoDelegate = UndoRedoCommands.UndoSetMainImage;
            Func<object[], bool> redoDelegate = UndoRedoCommands.RedoSetMainImage;

            var undoText = Environment.StringResources.GetString("PropertiesDocImageDialog_SetMainImage_Undo");
            var redoText = Environment.StringResources.GetString("PropertiesDocImageDialog_SetMainImage_Redo");

            Environment.UndoredoStack.Add("SetMainImage", "SetMainImage", undoText, redoText, null, new object[] { curDocID, undoDelegate, redoDelegate, origMainImage, newMainImage }, Environment.CurEmp.ID);
        }

		private void variantList_newDocItem_Click(object sender, EventArgs e)
		{
			if(variantList.SelectedItems.Count > 0)
			{
				var item = (VariantListItem)variantList.SelectedItems[0];

				if(item != null && (item.Type == VariantType.Image || item.Type == VariantType.ImageOriginal))
				{
                    var dialog = new Dialogs.AddDBDocDialog(docComponent.ServerInfo, item.ID, PageCount, IsPDFMode);
					dialog.DialogEvent += SaveImageDialog_DialogEvent;
					dialog.Show();
				}
			}
		}

		private void ShowDataPanel(bool show)
		{
			splitContainerDocument.Panel2Collapsed = !show;
		}

		private void SaveImageDialog_DialogEvent(object sender, DialogEventArgs e)
		{
			var dialog = e.Dialog as Dialogs.AddDBDocDialog;
			if(dialog == null)
				return;
			switch(dialog.DialogResult)
			{
				case DialogResult.Yes:
				case DialogResult.Retry:
				case DialogResult.OK:
					{
						bool work = dialog.DialogResult != DialogResult.Yes;
						if(work && dialog.AddToWork)
							Environment.WorkDocData.AddDocToEmployee(dialog.DocID, Environment.CurEmp.ID);
						if(work && dialog.NeedOpenDoc && dialog.DocID > 0)
							Environment.OnNewWindow(this, new Components.DocumentSavedEventArgs(dialog.DocID, -1));

						RefreshDoc();
						if(work)
						{
							force = !dialog.AddToWork;
							sendMail = dialog.SendMessage && !dialog.CreateSlaveEForm;
							parentDocIDs = dialog.ParentDocIDs;
							childDocIDs = dialog.ChildDocIDs;
							sendString = Environment.StringResources.GetString("ImageSaved");
							Send(dialog.DocID);
						}
						OnDocChanged(this, new Components.DocumentSavedEventArgs(dialog.DocID, dialog.ImageID, work && dialog.GotoDoc,
																work && dialog.CreateEForm, dialog.CreateSlaveEForm));
					}
					break;
			}
		}

		#endregion

		#region Sign

		private void UpdateSignDocumentPanel()
		{
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(UpdateSignDocumentPanel));
				return;
			}
			try
			{
				if(splitContainerDocumentSign.Panel2Collapsed)
				{
					splitContainerDocumentSign.Panel2Collapsed = false;
					signDocumentPanel_MinSizeChanged(signDocumentPanel.MinWidth);
				}
			}
			catch(Exception ex)
			{
				splitContainerDocumentSign.Panel2Collapsed = true;

				Data.Env.WriteToLog(ex);
				ErrorShower.OnShowError(null, ex.Message, "");
			}
		}

		private void ClearSignDocumentPanel()
		{
			if(InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(ClearSignDocumentPanel));
				return;
			}
			splitContainerDocumentSign.Panel2Collapsed = true;
			signDocumentPanel.ClearPanel();
		}

		#endregion

		#region Zoom

		public void FitTo(int option)
		{
			if(imagePDF.InvokeRequired)
				imagePDF.Invoke(new Action<short, bool>(imagePDF.FitTo),
								new object[] { (short)option, imagePDF.ImageDisplayed });
			else
				imagePDF.FitTo((short)option, imagePDF.ImageDisplayed);

			if(image.InvokeRequired)
				image.Invoke(new Action<short, bool>(image.FitTo),
									new object[] { (short)option, image.ImageDisplayed });
			else
				image.FitTo((short)option, image.ImageDisplayed);
		}

		public void ZoomToSelection()
		{
			if(imagePDF.ImageDisplayed)
				imagePDF.ZoomToSelection();
			else
				image.ZoomToSelection();
		}

		#endregion

		#region DocImageProperties

		/// <summary>
		/// Значение Value скролинга относительно Max value (100% - 1, 0% - 0)
		/// </summary>
		public double ActualImageVerticalScrollValue
		{
			get
			{
				if(image.ImageDisplayed)
					return image.ActualImageVerticalScrollValue;

				if(imagePDF.ImageDisplayed)
					return imagePDF.ActualImageVerticalScrollValue;

				return -1;
			}
		}

		/// <summary>
		/// Эффективная максимальная граница значения скролинга Value (min значение 0)
		/// </summary>
		public double ActualImageVerticalScrollMaxValue
		{
			get
			{
				if(image.ImageDisplayed)
					return image.ActualImageVerticalScrollMaxValue;

				if(imagePDF.ImageDisplayed)
					return imagePDF.ActualImageVerticalScrollMaxValue;

				return -1;
			}
		}

		/// <summary>
		/// Значение Value скролинга относительно Max value (100% - 1, 0% - 0)
		/// </summary>
		public double ActualImageHorisontalScrollValue
		{
			get
			{
				if(image.ImageDisplayed)
					return image.ActualImageHorisontallScrollValue;

				if(imagePDF.ImageDisplayed)
					return imagePDF.ActualImageHorisontallScrollValue;

				return -1;
			}
		}

		/// <summary>
		/// Эффективная максимальная граница значения скролинга Value (min значение 0)
		/// </summary>
		public double ActualImageHoriszontalScrollMaxValue
		{
			get
			{
				if(image.ImageDisplayed)
					return image.ActualImageHorisontalScrollMaxValue;

				if(imagePDF.ImageDisplayed)
					return imagePDF.ActualImageHorisontalScrollMaxValue;

				return -1;
			}
		}

		/// <summary>
		/// Эффективная максимальная граница значения скролинга Value (min значение 0)
		/// </summary>
		public double ImageControlHeght
		{
			get
			{
				if(image.ImageDisplayed)
					return image.ActualImageHorisontalScrollMaxValue;

				if(imagePDF.ImageDisplayed)
					return imagePDF.ActualImageHorisontalScrollMaxValue;

				return -1;
			}
		}

		/// <summary>
		/// Ширина и
		/// </summary>
		public double ImageControlWidth
		{
			get
			{
				if(image.ImageDisplayed)
					return image.ActualImageHorisontalScrollMaxValue;

				if(imagePDF.ImageDisplayed)
					return imagePDF.ActualImageHorisontalScrollMaxValue;

				return -1;
			}
		}

		private void DocImageProperties()
		{
			if(imgID <= 0)
				return;
			var dialog = new PropertiesDialogs.PropertiesDocImageDialog(imgID, curDocID);
			dialog.DialogEvent += PropertiesDocImageDialog_DialogEvent;
			dialog.Show();
			if(image.ImageDisplayed)
			{
				dialog.ResolutionX = image.ImageResolutionX;
				dialog.ResolutionY = image.ImageResolutionY;
				dialog.PixelHeight = image.ImageHeight;
				dialog.PixelWigth = image.ImageWidth;

				if(image.ImagePixelFormat == PixelFormat.Format1bppIndexed)
				{
					dialog.ColorType = "Черно-белый";
				}
				else if(image.ImagePixelFormat == PixelFormat.Format4bppIndexed)
				{
					dialog.ColorType = "4bit";
				}
				else if(image.ImagePixelFormat == PixelFormat.Format8bppIndexed)
				{
					dialog.ColorType = "8bit";
					if(image.ImagePalette != null)
						if(image.ImagePalette.Flags == (int)PaletteFlags.GrayScale)
							dialog.ColorType += " градации серого";
						else
							dialog.ColorType += " цветной";
				}
				else if(image.ImagePixelFormat == PixelFormat.Format24bppRgb)
				{
					dialog.ColorType = "24bit цветной";
				}
				else if(image.ImagePixelFormat == PixelFormat.Format32bppArgb ||
						 image.ImagePixelFormat == PixelFormat.Format32bppPArgb ||
						 image.ImagePixelFormat == PixelFormat.Format32bppRgb)
				{
					dialog.ColorType = "32bit цветной";
				}
				if(string.IsNullOrEmpty(dialog.ColorType))
					dialog.ColorType = "Неизвестен";
				dialog.FillCurrentPage();
			}
			else if(imagePDF.ImageDisplayed)
			{
				dialog.ResolutionX = imagePDF.ImageResolutionX;
				dialog.ResolutionY = imagePDF.ImageResolutionY;
				dialog.PixelHeight = imagePDF.ImageHeight;
				dialog.PixelWigth = imagePDF.ImageWidth;

				dialog.ColorType = Environment.StringResources.GetString("PrinterOp_Status2");
				dialog.FillCurrentPage();
			}
		}

		private void PropertiesDocImageDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Focus();

			if(e.Dialog.DialogResult == DialogResult.OK)
			{
				RefreshDoc();
			}
		}

		#endregion

		private void DocControl_KeyUp(object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
				case Keys.Left:
					if(image.ImageDisplayed && Page > 1)
						Page--;
					break;
				case Keys.Right:
					if(image.ImageDisplayed && Page < PageCount)
						Page++;
					break;
				case Keys.Control | Keys.Left:
					if(variantList.Items.Count > 0)
					{
						if(variantList.SelectedIndices.Count > 0)
						{
							if(variantList.SelectedIndices[0] > 0)
								variantList.Items[variantList.SelectedIndices[0] - 1].Selected = true;
						}
						else
							variantList.Items[0].Selected = true;
					}
					break;
				case Keys.Control | Keys.Right:
					if(variantList.Items.Count > 0)
					{
						if(variantList.SelectedIndices.Count > 0)
						{
							if(variantList.SelectedIndices[0] < variantList.Items.Count)
								variantList.Items[variantList.SelectedIndices[0] + 1].Selected = true;

						}
						else
							variantList.Items[0].Selected = true;
					}
					break;
			}
		}

		#region Delete Part

		public void DeletePart()
		{
			if(!ImageDisplayed)
				return;

			string oldFileName = (imagePDF.ImageDisplayed ? imagePDF.FileName : image.FileName);

			TmpFile tf = Environment.GetTmpFileByKey(fileName);
			if(tf != null)
			{
				string file = (curDocID > 0 ? Environment.StringResources.GetString("SaveChangesDialog_Doc1") : Environment.StringResources.GetString("SaveChangesDialog_File1")).ToLower();
				MessageBox.Show(string.Format(StringResources.DocControl_CopyExists, file), StringResources.Warning);
				return;
			}
			if(Environment.TmpFilesContains(oldFileName))
			{
				tf = Environment.GetTmpFile(oldFileName);
				//if (tf.CurAct == Environment.ActionBefore.DelPart)
				//{
				//    if (!File.Exists(tf.TmpFullName))
				//        SaveToTmpCopy(oldFileName, tf.TmpFullName, IsPDFMode);
				if(tf != null)
					oldFileName = tf.TmpFullName;
				//}
			}

			Dialogs.SavePartDialog dialog;
			if(ContainSavePartDialog(oldFileName))
			{
				dialog = GetSavePartDialog(oldFileName);
				if(dialog != null && dialog.Delete)
				{
					dialog.BringToFront();
					return;
				}
				if(dialog != null)
					dialog.Close();
			}
			dialog = imagePDF.ImageDisplayed
				? new Dialogs.SavePartDialog(oldFileName, imagePDF.Page, imagePDF.PageCount) { IsPdf = true }
				: new Dialogs.SavePartDialog(oldFileName, image.Page, image.PageCount);
			dialog.Delete = true;
			AddSavePartDialog(dialog);

			if(tf != null)
				tf.LinkCnt++;

			dialog.DialogEvent += DeletePartDialog_DialogEvent;
			dialog.Show();
		}

		private void DeletePartDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Focus();

			var dialog = e.Dialog as Dialogs.SavePartDialog;

			TmpFile tf = Document.Environment.GetTmpFileByValue(dialog.FileName);

			if(dialog != null && dialog.DialogResult == DialogResult.OK)
			{
				bool save = false;
				bool rdonly = dialog.FileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase) ? IsReadonly : !Environment.IsAccessible(fileName);
				if(dialog.ImageID > 0)
					rdonly = false;
				int docID = dialog.FileName.Equals(fileName, StringComparison.CurrentCultureIgnoreCase) ? curDocID : -1;
				Dialogs.SaveChangesDialog sch = null;
				if(tf != null)
				{
					tf.CurAct = Environment.ActionBefore.None;
					save = MessageBox.Show(string.Format(Environment.StringResources.GetString("DocControl_DelPartQuery"), dialog.MinPage, dialog.MaxPage), Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes;
				}
				else
				{
					sch = new Dialogs.SaveChangesDialog(Path.GetFileName(dialog.FileName), rdonly, Environment.ActionBefore.DelPart, docID, Environment.Settings.Folders.Add("SaveChanges"));
					save = (sch.ShowDialog() == DialogResult.Yes);
				}

				if(save)
				{
					if(sch == null || sch.Result == Dialogs.DelPartDialogResult.Yes)
					{
						if(!dialog.IsPdf)
							image.DelPart(dialog.FileName, dialog.MinPage, dialog.MaxPage, false);
						else
							Environment.PDFHelper.DelPart(dialog.FileName, dialog.MinPage, dialog.MaxPage);

						if(tf != null)
							tf.Modified = true;
					}
					else if(sch.Result == Dialogs.DelPartDialogResult.ShowCopy)
					{
						string real_fileName = Environment.GenerateFullFileName(Path.GetExtension(dialog.FileName).TrimStart('.'));
						File.Copy(dialog.FileName, real_fileName, true);
						File.SetAttributes(real_fileName, FileAttributes.Normal);

						if(!dialog.IsPdf)
							image.DelPart(real_fileName, dialog.MinPage, dialog.MaxPage, false);
						else
							Environment.PDFHelper.DelPart(real_fileName, dialog.MinPage, dialog.MaxPage);

						Environment.AddTmpFile(dialog.FileName, real_fileName, this.IsPDFMode);
						TmpFile nFile = Document.Environment.GetTmpFile(dialog.FileName);
						nFile.CurAct = Environment.ActionBefore.None;
						nFile.DocString = !string.IsNullOrEmpty(dialog.DocStr) ? dialog.DocStr : Path.GetFileName(dialog.FileName);
						nFile.Modified = true;
						nFile.IsReadOnly = rdonly;
						int dId = 0;
						if(dialog.ImageID > 0)
						{
							DataRow dr = Environment.DocImageData.GetDocImage(dialog.ImageID);
							if(dr != null)
								dId = (int)dr[Environment.DocImageData.DocIDField];
						}

						nFile.DocId = dId;
						nFile.SrvInfo = docComponent.ServerInfo;
						Components.DocumentSavedEventArgs doc = new Components.DocumentSavedEventArgs(real_fileName, nFile.DocString) { BeforeAct = Environment.ActionBefore.None };
						Environment.OnNewWindow(this, doc);
					}
				}
			}
			if(tf != null)
			{
				tf.CurAct = Environment.ActionBefore.None;
				//if(tf.Window == null)
				//    tf.AscBeforeClose = false;

				tf.LinkCnt--;
			}
		}

		public void TestImage()
		{
			TestImage(Environment.ActionBefore.None);
		}

		public void TestImage(Environment.ActionBefore act)
		{
			lock(lockObj)
			{
				if(imagePDF.ImageDisplayed)
					imagePDF.TestImage(act);
				else if(image.ImageDisplayed)
					image.TestImage(act);
			}
		}

		#endregion

		#region receiver

		private void StartReceiverTimer()
		{
			if(rcv == null)
			{
				if(timer == null)
				{
					timer = new System.Threading.Timer(Timer_Elapsed, null, 50, System.Threading.Timeout.Infinite);
				}
				else
				{
					timer.Change(50, 0);
				}
			}
		}

		private void Timer_Elapsed(object state)
		{
			if(Disposing || IsDisposed)
				return;
			CreateReceive();
		}

		private void CreateReceive()
		{
			if(timer != null)
			{
				timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
				timer.Dispose();
				timer = null;
			}
			CreateReceiver();
		}

        /// <summary>
        /// Создание прослушивателя
        /// </summary>
		private void CreateReceiver()
		{
            // Отписка от старого экземпляра
			DeleteReceiver();

			if(imgID > 0)
			{
			    int? archivId = null;

			    if (docComponent != null && docComponent.serverInfo != null)
			        archivId = docComponent.serverInfo.ID;

                //  Подписка на изображение imgID архива archivId
				rcv = new Receive.Receive(Environment.ConnectionStringDocument, Environment.SubscribeTable, 15, imgID, archivId??0, 3);

				rcv.Start();
				rcv.Received += rcv_Received;
			}
		}

		private void DeleteReceiver()
		{
			if(rcv != null)
			{
				rcv.Received -= rcv_Received;
				ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
					{
						Receive.Receive rc = obj as Receive.Receive;
						rc.Exit();
						rc = null;
					}), rcv);
				rcv = null;
			}
		}

		private void rcv_Received(string receivedString, int parameter)
		{
			Match m = Regex.Match(receivedString, @"^(?<code>\d*)(\|(?<msg>.*))?$");

			if(!m.Success)
				return;

#if AdvancedLogging
            Log.Logger.Message("DocControl rcv_Received receivedString = " + receivedString);
#endif

			int messCode = int.Parse(m.Groups["code"].Value);
			string msg = m.Groups["msg"].Value;

			switch(messCode)
			{
				case 15: // image changed
					if(InvokeRequired)
						BeginInvoke(new Action<string>(ImageChangedMessageHandler), msg);
					else
						ImageChangedMessageHandler(msg);
					break;

				case 16: // приход изображения

					break;

				default:
					return;
			}
		}

        /// <summary>
        /// Обработчик сообщений изменения изображения
        /// </summary>
        /// <param name="receivedString"></param>
		private void ImageChangedMessageHandler(string receivedString)
		{
			var match = Regex.Match(receivedString, @"^(?<code>\d+)(\|(?<archiv>\d+))?\|(?<type>[pis])$");

			if(!match.Success)
				return;

			int testImgId;
			int testArchivId = 0;
			string type = string.Empty;
			if(!int.TryParse(match.Groups["code"].Value, out testImgId))
				return;
			if(!string.IsNullOrEmpty(match.Groups["archiv"].Value))
				int.TryParse(match.Groups["archiv"].Value, out testArchivId);
			type = match.Groups["type"].Value;

			// Если совпал идентифиатор сообщения
			// И совпал идентификатор архива ИЛИ сообщение по странице изображения
			if(testImgId == imgID && docComponent != null && docComponent.serverInfo != null)
			{
				if(testArchivId.Equals(docComponent.serverInfo.ID) || "p".Equals(type))
				{
					if(image.Modified)
					{
						if(MessageBox.Show(
							Environment.StringResources.GetString("DocControl_ImageChanged_Message1") +
							System.Environment.NewLine +
							Environment.StringResources.GetString("DocControl_ImageChanged_Message2"),
							Environment.StringResources.GetString("Changes"), MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						{
							ReloadImagePage(docComponent.serverInfo.ID, imgID);
						}
					}
					else
#if AdvancedLogging
					    using (Log.Logger.DurationMetter("ReloadImage"));
#endif

                    if ("p".Equals(type))
                        SyncImagePageRotations(imgID);
                    else
                        ReloadImagePage(docComponent.serverInfo.ID, imgID);
				}
				if("s".Equals(type))
				{
					CurrentStamp = null;
					if(image.ImageDisplayed)
						image.ReloadStamps();
					else if(imagePDF.ImageDisplayed)
						imagePDF.ReloadStamps();
				}
			}
		}

		#endregion

		#region reload

		private void ReloadIcons()
		{
			if(curDocID <= 0)
				return;

			ArrayList list = new ArrayList();
			// получаем основное изображение
			int mainImageID = Environment.DocData.GetDocIntField(Environment.DocData.MainImageIDField, curDocID);

			variantList.SelectedIndexChanged -= variantList_SelectedIndexChanged;
			if(variantList.Items.Count > 0)
				variantList.Items.Clear();

			ListItem selecteditem = null; // индекс текущего изображения

			// есть ли данные у документа
			hasData = Environment.DocDataData.IsDataPresent(curDocID);
			HasDocLinks = Environment.DocLinksData.HasLinks(curDocID);

			using(DataTable dt = Environment.DocImageData.GetDocImages(curDocID))
			using(DataTableReader dr = dt.CreateDataReader())
			{
				hasImage = dr.HasRows;
				while(dr.Read())
					try
					{
						var imgId = (int)dr[Environment.DocImageData.IDField];
						var createDate = (DateTime)dr[Environment.DocImageData.CreateDateField];
						var editedDate = (DateTime)dr[Environment.DocImageData.EditedField];
						var archiveID = (int)dr[Environment.DocImageData.ArchiveIDField];
						int printed = dr[Environment.DocImageData.PrintedField].Equals(DBNull.Value) ? 0
										  : Convert.ToInt32(dr[Environment.DocImageData.PrintedField]);
						var item = new VariantListItem(
							imgId,
							VariantType.Image,
							createDate.ToLocalTime().ToString("dd.MM.yyyy"))
									   {
										   CreateTime = createDate,
										   EditedTime = editedDate,
										   Printed = printed > 0,
										   ImageType = dr[Environment.DocImageData.ImageTypeField].ToString()
									   };

						if(item.Printed)
						{
							object a1 = Environment.PrintData.GetField(Environment.PrintData.NameField, printed);
							if(a1 != null)
								item.Text = a1.ToString();
						}
						if(imgId == mainImageID)
						{
							if(archiveID != 0)
							{
								if(printed > 0)
								{
									item.ImageIndex = (int)VariantType.MainImageOriginalPrinted;
									item.Type = VariantType.MainImageOriginalPrinted;
								}
								else
								{
									item.ImageIndex = (CanShowSplit()) ? 8 : (item.IsPDF() ? 20 : (int)VariantType.MainImageOriginal);
									item.Type = VariantType.MainImageOriginal;
								}
							}
							else
							{
								if(printed > 0)
								{
									item.ImageIndex = (int)VariantType.MainImagePrinted;
									item.Type = VariantType.MainImagePrinted;
								}
								else
								{
									item.ImageIndex = (CanShowSplit()) ? 14 : (item.IsPDF() ? 18 : (int)VariantType.MainImage);
									item.Type = VariantType.MainImage;
								}
							}
						}
						else
						{
							if(archiveID != 0)
							{
								if(printed > 0)
								{
									item.ImageIndex = (int)VariantType.ImageOriginalPrinted;
									item.Type = VariantType.ImageOriginalPrinted;
								}
								else
								{
									item.ImageIndex = (CanShowSplit()) ? 13 : (item.IsPDF() ? 19 : (int)VariantType.ImageOriginal);
									item.Type = VariantType.ImageOriginal;
								}
							}
							else
							{
								if(printed > 0)
								{
									item.ImageIndex = (int)VariantType.ImagePrinted;
									item.Type = VariantType.ImagePrinted;
								}
								else
									item.ImageIndex = (CanShowSplit()) ? 12 : (item.IsPDF() ? 17 : (int)VariantType.Image);
							}
						}
						list.Add(item);
						if(imgId == imgID || (imgID < 0 && mainImageID == imgId))
							selecteditem = item;
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				dr.Close();
				dr.Dispose();
				dt.Dispose();
			}
			if(!hasImage)
				image.FileName = "";

			if((!showWebPanel && hasData) || (!hasImage && hasData))
			{
				var item = new VariantListItem(
					curDocID,
					VariantType.Data,
					Environment.StringResources.GetString("EForm")) { ImageIndex = (int)VariantType.Data };
				list.Add(item);
				if(imgID == 0)
					selecteditem = item;
			}

			int docTypeID = Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, curDocID);
			if(docTypeID > 0)
			{
				if(Environment.Is1CDictionaryType(docTypeID) || (hasData && Environment.Is1CType(docTypeID)))
				{
					try
					{
						if(Environment.IsConnectedBuh)
						{
							string dsPersonsString = Environment.DocData.GetDocPersonsIDs(curDocID).TrimStart(',').TrimEnd(',');

							if(dsPersonsString.Length > 0)
							{
								List<int> hBuhParam = null;
								using(DataTable dtPersons = Environment.BuhData.BuhPersons(dsPersonsString))
								using(DataTableReader drPerson = dtPersons.CreateDataReader())
								{
									if(drPerson.HasRows)
									{
										hBuhParam = Environment.BuhParamDocData.GetSentDocToIc(curDocID, 0);
									}

									while(drPerson.Read())
									{
										if(drPerson[Environment.BuhData.BuhPersonIDField] != null &&
											drPerson[Environment.BuhData.BuhPersonIDField] is int)
										{
											var perID = (int)drPerson[Environment.BuhData.BuhPersonIDField];
											Console.WriteLine("{0}: Buh ID: {1}", DateTime.Now.ToString("HH:mm:ss fff"), perID);

											string personStr = drPerson[Environment.BuhData.BuhField].ToString();
											bool test = variantList.Items.Cast<VariantListItem>().Any(titem => titem.Type == VariantType.ICConnect && titem.Text == personStr);
											Console.WriteLine("{0}: Has person - {1}", DateTime.Now.ToString("HH:mm:ss fff"), test.ToString());
											if(!test)
											{
												var item = new VariantListItem(perID, VariantType.ICConnect, personStr)
															   {
																   ImageIndex = hBuhParam != null && hBuhParam.Contains(perID) ? 15 : (int)VariantType.ICConnect
															   };
												list.Add(item);
											}
										}
									}
									drPerson.Close();
								}


								if(hasData && Environment.Is1CFoodType(docTypeID) ||
									Environment.Is1CDictionaryType(docTypeID))
								{
									using(DataTable dtPersons = Environment.BuhData.BuhIcFoodPersons(dsPersonsString))
									using(DataTableReader drPerson = dtPersons.CreateDataReader())
									{
										if(drPerson.HasRows)
										{
											hBuhParam = Environment.BuhParamDocData.GetSentDocToIc(curDocID, 0);
										}

										while(drPerson.Read())
										{
											if(drPerson[Environment.BuhData.BuhPersonIDField] is int)
											{
												var perID = (int)drPerson[Environment.BuhData.BuhPersonIDField];
												Console.WriteLine("{0}: food buh ID: {1}", DateTime.Now.ToString("HH:mm:ss fff"), perID);
												string personStr = drPerson[Environment.BuhData.BuhField].ToString();
												bool test = variantList.Items.Cast<VariantListItem>().Any(titem => titem.Type == VariantType.ICMRConnect && titem.Text == personStr);
												Console.WriteLine("{0}: Has person - {1}", DateTime.Now.ToString("HH:mm:ss fff"), test);
												if(!test)
												{
													var item = new VariantListItem(perID, VariantType.ICMRConnect, personStr)
																   {
																	   ImageIndex = hBuhParam != null && hBuhParam.Contains(perID) ? 16 : (int)VariantType.ICMRConnect
																   };
													list.Add(item);
												}
											}
										}
										drPerson.Close();
										drPerson.Dispose();
										dtPersons.Dispose();
									}
								}
							}
						}
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
			}

			splitContainerWhole.Panel1Collapsed = false;
			if(list.Count == 0)
				ClearDoc();


			// если ничего не отметили, отмечаем первый элемент
			if(showWebPanel)
			{
				if(hasData)
					ShowHTMLDoc();
			}

			lock(variantList)
			{
				if(list.Count > 0)
				{
					var items = new VariantListItem[list.Count];
					list.CopyTo(items);
					RecalcIconPlace(items);
					variantList.SelectedIndexChanged += variantList_SelectedIndexChanged;
					if(!IsDisposed || !Disposing)
					try
					{
						if(selecteditem != null && variantList.Items != null && variantList.Items.Contains(selecteditem))
						{
							ListViewItem it = variantList.Items[variantList.Items.IndexOf(selecteditem)];
							if(it != null)
								it.Selected = true;
						}
						else if(variantList.Items.Count > 0)
							variantList.Items[0].Selected = true;
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				else
					variantList.SelectedIndexChanged += variantList_SelectedIndexChanged;
			}
		}

		/// <summary>
		///   пересортировка иконок отображения документа
		/// </summary>
		private void RecalcIconPlace(VariantListItem[] items)
		{
			var printed = new ArrayList();
			int eformindex = -1;
			int _1cindex = -1;
			if(items == null)
			{
				if(variantList.Items.Count < 2)
					return;
				items = new VariantListItem[variantList.Items.Count];
				variantList.Items.CopyTo(items, 0);
				variantList.Items.Clear();
			}
			else
				switch(items.Length)
				{
					case 0:
						return;
					case 1:
						variantList.Items.Add(items[0]);
						return;
				}

			for(int i = 0; i < items.Length; i++)
			{
				if(items[i].Printed)
					printed.Add(i);
				switch(items[i].Type)
				{
					case VariantType.Data:
						eformindex = i;
						break;
					case VariantType.ScalaConnect:
					case VariantType.ICMRConnect:
					case VariantType.ICConnect:
						if(_1cindex == -1)
							_1cindex = i;
						break;
				}
			}
			if(eformindex > -1 && _1cindex > -1)
				if(eformindex > _1cindex)
				{
					VariantListItem ci = items[eformindex];
					for(int k = eformindex; k > _1cindex; k--)
					{
						items[k] = items[k - 1];
						if(printed.Contains(k - 1))
						{
							printed.Remove(k - 1);
							printed.Add(k);
						}
					}
					items[_1cindex] = ci;
					eformindex = _1cindex;
					_1cindex += 1;
				}
			if(_1cindex > -1 || eformindex > -1 && printed.Count > 0)
			{
				int index = eformindex;
				if(index == -1)
					index = _1cindex - 1;
				for(int j = printed.Count - 1; j >= 0; j--)
				{
					if((int)printed[j] < index)
					{
						MoveUp((int)printed[j], index, items);
						index--;
					}
					else if((int)printed[j] > _1cindex && _1cindex > -1)
					{
						MoveDown((int)printed[j], _1cindex, items);
						_1cindex++;
					}
				}
			}
			else if(printed.Count < items.Length && printed.Count > 0)
			{
				int index = items.Length - 1;
				for(int j = printed.Count - 1; j >= 0; j--)
				{
					if((int)printed[j] == index)
						index--;
					else if((int)printed[j] < index)
					{
						MoveUp((int)printed[j], index, items);
						index--;
					}
				}
			}
			variantList.Items.AddRange(items);
		}

		/// <summary>
		///   передвижение элемента массива вниз
		/// </summary>
		/// <param name="startIndex"> элемент массива, который перемещается </param>
		/// <param name="endIndex"> место куда перемещается </param>
		/// <param name="items"> массив элементов </param>
		private static void MoveDown(int startIndex, int endIndex, VariantListItem[] items)
		{
			VariantListItem ci = items[startIndex];
			for(int k = startIndex; k > endIndex; k--)
			{
				items[k] = items[k - 1];
			}
			items[endIndex] = ci;
		}

		/// <summary>
		///   передвижение элемента массива вверх
		/// </summary>
		/// <param name="startIndex"> элемент массива, который перемещается </param>
		/// <param name="endIndex"> место куда перемещается </param>
		/// <param name="items"> массив элементов </param>
		private static void MoveUp(int startIndex, int endIndex, VariantListItem[] items)
		{
			VariantListItem ci = items[startIndex];
			for(int k = startIndex; k < endIndex; k++)
			{
				items[k] = items[k + 1];
			}
			items[endIndex] = ci;
		}

		public void ReloadTran(bool force)
		{
			if(force)
			{
				if(transTimer != null)
				{
					transTimer.Stop();
					transTimer.Elapsed -= transTimer_Elapsed;
				}
				transactionControl.DocumentID = curDocID;
			}
			else
			{
				if(transTimer == null)
				{
					transTimer = new System.Timers.Timer();
				}
				else
				{
					transTimer.Stop();
					transTimer.Elapsed -= transTimer_Elapsed;
				}
				transTimer.Elapsed += transTimer_Elapsed;
				transTimer.AutoReset = false;
				transTimer.Interval = transTimerInterval;
				transTimer.Start();
			}
		}

        /// <summary>
        /// Загрузить текущую страницу изображения
        /// Опционно страницу changedPage
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="imgId"></param>
        /// <param name="changedPage">Опция идентификатор страницы для перезагрузки; -1 перезагрузить все</param>
		private void ReloadImagePage(int serverId, int imgId, int changedPage = -1)
		{
			curServer = Environment.GetLocalServer(serverId);

            // Выбранная пользователем страница изображения может не совпадать с измененной.
 
			if(curServer != null)
			{
                // Формирование полного пути файла изображения
				fileName = curServer.Path + "\\" + Environment.GetFileNameFromID(imgId) + "." + ImageType;

                // Проверка существования файла
				if(File.Exists(fileName))
				{
					int curPage;

                    // Проверка, изображение TIFF
					if(image.ImageID == imgId)
					{
                        // Сохранение номера текущей страницы
						curPage = image.Page;

                        // Если получен номер страницы изображения(в настоящее время не работает, всегда получаем -1)
                        if (changedPage >= 0 && changedPage != curPage)
                            image.LoadImagePage(imgId, changedPage);

                        // В настоящее время всегда вызывается и происходит перезагрузка всего изображения
						image.LoadImagePage(imgId, curPage - 1);
					}
					else if(imagePDF.ImageID == imgId)
					{
					    if (changedPage == -1)
					    {
                            // Перезагрузка pdf

					        try
					        {
					            var p = imagePDF.Page;
                                imagePDF.ForceReload = true;
                                imagePDF.FileName = imagePDF.FileName;
					            imagePDF.Page = p;

                                imagePDF.LoadImagePage(imgId, p);
					        }
					        finally
					        {
					            imagePDF.ForceReload = false;
					        }
					    }
					    else
					    {
                            // Перезагрузка измененной страницы
                            curPage = imagePDF.Page;

                            if (changedPage >= 0 && changedPage != curPage)
                                imagePDF.LoadImagePage(imgId, changedPage);

                            imagePDF.LoadImagePage(imgId, curPage);
					    }
					}

					CreateDocumentComponent(curDocID, imgId, curServer);
				}
			}
		}

        /// <summary>
        /// Синхронизация виртуальных поворотов
        /// </summary>
        /// <param name="imgId"></param>
        private void SyncImagePageRotations(int imgId)
	    {
            if(image.ImageID == imgId)
                image.SyncImagePageRotations();
            else if(imagePDF.ImageID == imgId)
                imagePDF.SyncImagePageRotations();
	    }

		#endregion

		#region Print

		private AutoResetEvent syncPrintEForm = new AutoResetEvent(true);

		private void printDialog_Browser_DialogEvent(object source, DialogEventArgs e)
		{
			var dialog = e.Dialog as Dialogs.DocPrintDialog;
			if(dialog != null && (dialog.DialogResult == DialogResult.OK && dialog.WebMode))
			{
				Action<List<PrinterObjectClass>, int, short> switchPrintEform = StartSwitchPrintEform;

				if(dialog.PrinterObjectsList != null)
					switchPrintEform.BeginInvoke(dialog.PrinterObjectsList, dialog.DocID, dialog.CopiesCount, FinishSwitchPrintEform, switchPrintEform);
			}
		}

		private void FinishSwitchPrintEform(IAsyncResult ar)
		{
			try
			{
				var res = (Action<List<PrinterObjectClass>, int, short>)ar.AsyncState;
				res.EndInvoke(ar);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private void StartSwitchPrintEform(List<PrinterObjectClass> printerObject, int printID, short copiesCount)
		{
			foreach(PrinterObjectClass po in printerObject)
			{
				GC.Collect();
                Console.WriteLine("{0}: printing form {1}", DateTime.Now.ToString("HH:mm:ss fff"), po.TypeID);
				if(po.PrintType != 1)
					syncPrintEForm.WaitOne(30000, false);
				if(InvokeRequired)
					Invoke(new Data.Action<int, string, int, int, short, short>(SwitchPrintEform),
						   new object[] { po.PrintType, po.URL, printID, po.TypeID, po.PaperSize, copiesCount });
				else
					SwitchPrintEform(po.PrintType, po.URL, printID, po.TypeID, po.PaperSize, copiesCount);
			}
		}

		private void SwitchPrintEform(int printTypeID, string url, int docID, int printID, short paperSize, short copiesCount)
		{
			switch(printTypeID)
			{
				case 0:
					StartPrintEformWithOrientation(printTypeID, url, docID, printID, paperSize, copiesCount);
					break;
				case 2:
				case 3:
					StartPrintEForm(PrintUrl(url, docID, printID, true), printTypeID, paperSize, copiesCount);
					break;
				case 1:
					if(url.StartsWith("http"))
					{
						StartPrintEformWithOrientation(printTypeID, url, docID, printID, paperSize, copiesCount);
					}
					else
					{
						try
						{
							Console.WriteLine("{0}: Report print start", DateTime.Now.ToString("HH:mm:ss fff"));
							Environment.Report.PrintReport(Dialogs.DocPrintDialog.Printer, url, docID, printID,
														   paperSize, copiesCount);
							Console.WriteLine("{0}: Report print end", DateTime.Now.ToString("HH:mm:ss fff"));
						}
						catch(Exception ex)
						{
							Console.WriteLine(ex.Message);
							Data.Env.WriteToLog(ex);
						}
					}
					break;
			}
		}

		private void StartPrintEformWithOrientation(int printTypeID, string url, int docID, int printID, short paperSize, short copiesCount)
		{
			string printurl = PrintUrl(url, docID, printID, false);
			try
			{
				var webClient = new WebClient { Credentials = CredentialCache.DefaultCredentials };
				byte[] ret = webClient.DownloadData(printurl + "&orientation=1");
				string orient = Encoding.Default.GetString(ret);
				printTypeID = orient.Trim().ToLower() == "landscape" ? 3 : 2;
			}
			catch(Exception ex)
			{
				printTypeID = 2;
				Data.Env.WriteToLog(ex, "Нет автоориентации у печатной формы  документа " + docID.ToString());
			}
			StartPrintEForm(PrintUrl(url, docID, printID, true), printTypeID, paperSize, copiesCount);
		}

		internal static string PrintUrl(string url, int docID, int printID, bool full)
		{
			string retVal = url;
			if(full)
				if(retVal.IndexOf("?") > 0)
					retVal = retVal.Replace("?",
											"?docviewprint=" + docID.ToString() + "&docviewtypeid=" + printID.ToString() +
											"&");
				else
					retVal += "?docviewprint=" + docID.ToString() + "&docviewtypeid=" + printID.ToString();

			return retVal.EndsWith("=") ? retVal + docID.ToString() : retVal + "&id=" + docID.ToString();
		}

		private void StartPrintEForm(string url, int printTypeID, short paperSize, short copiesCount)
		{
			Console.WriteLine("{0}: print e-form started", DateTime.Now.ToString("HH:mm:ss fff"));
			if(printBrowser == null)
				RestoreBrowser();
			try
			{
				printBrowser.Tag = new Classes.Tag { TypeID = printTypeID, PaperSize = paperSize, CopiesCount = copiesCount };
				printBrowser.DocumentCompleted += printBrowser_DocumentCompleted;
				printBrowser.SelfNavigate = true;
				printBrowser.Url = new Uri(url);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private void printBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			var printbrowser = sender as ExtendedBrowserControl;
			if(printbrowser != null)
			{
				printbrowser.DocumentCompleted -= printBrowser_DocumentCompleted;
				if(printbrowser.Tag is Classes.Tag)
				{
					Classes.Tag tag = (Classes.Tag)printbrowser.Tag;
					Console.WriteLine("{0}: print orintation {1}", DateTime.Now.ToString("HH:mm:ss fff"), tag.TypeID);
					printbrowser.Print(Dialogs.DocPrintDialog.Printer, Environment.ShowHelpString + "printtemplate" + ((tag.TypeID == 3) ? "landscape" : "") +
								  ".html" + ((tag.CopiesCount > 1) ? ("?numcopies=" + tag.CopiesCount.ToString()) : ""));
					Application.DoEvents();
					try
					{
						if(printbrowser.Version.Major < 7)
						{
							Controls.Remove(printbrowser);
							if(printbrowser == printBrowser)
								printBrowser = null;
							//printbrowser.DocumentCompleted -= printBrowser_DocumentCompleted;
							printbrowser.Dispose();
							printbrowser = null;
						}
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
					Console.WriteLine("{0}: releace event", DateTime.Now.ToString("HH:mm:ss fff"));
					try
					{
						if(syncPrintEForm != null)
							syncPrintEForm.Set();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
					Console.WriteLine("{0}: set event", DateTime.Now.ToString("HH:mm:ss fff"));
				}
				else
				{
					printbrowser.Print(Dialogs.DocPrintDialog.Printer, Environment.ShowHelpString + "printtemplate.html");
					syncPrintEForm.Set();
					try
					{
						if(printbrowser.Version.Major < 7)
						{
							Controls.Remove(printbrowser);
							if(printbrowser == printBrowser)
								printBrowser = null;
							//printbrowser.DocumentCompleted -= printBrowser_DocumentCompleted;
							printbrowser.Dispose();
							printbrowser = null;
						}
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
			}
		}

		private void printDialog_Multi_DialogEvent(object source, DialogEventArgs e)
		{
			var dialog = e.Dialog as Dialogs.DocPrintDialog;
			if(dialog != null && dialog.DialogResult == DialogResult.OK)
			{
				var ht = new Hashtable();
				if(dialog.WebMode)
				{
					if(dialog.PrinterObjectsList != null)
						ht.Add(dialog.DocTypeID, dialog.PrinterObjectsList);
				}
				PrintDocument(dialog.DocID, dialog.PrintImage, dialog.PrintOnlyMain, dialog.PrintEForm, ref ht,
							  dialog.CopiesCount);
				ht.Clear();
			}
		}

		#endregion

		#region Save Part Dialog

		public static void AddSavePartDialog(Dialogs.SavePartDialog dialog)
		{
			if(savePartTable == null)
				savePartTable = new SynchronizedCollection<KeyValuePair<string, Dialogs.SavePartDialog>>();

			var item = new KeyValuePair<string, Dialogs.SavePartDialog>(dialog.FileName, dialog);
			if(!savePartTable.Contains(item))
				savePartTable.Add(item);
		}

		public static bool ContainSavePartDialog(string fileName)
		{
			if(savePartTable == null)
				return false;

			bool result = false;
			for(int i = 0; i < savePartTable.Count && !result; i++)
				if(savePartTable[i].Key == fileName)
					result = true;

			return result;
		}

		public static void RemoveSavePartDialog(string fileName)
		{
			if(savePartTable == null)
				return;

			for(int i = savePartTable.Count - 1; i > -1; i--)
				if(savePartTable[i].Key == fileName)
					savePartTable.RemoveAt(i);
		}

		public static Dialogs.SavePartDialog GetSavePartDialog(string fileName)
		{
			if(savePartTable == null)
				return null;

			return (from t in savePartTable where t.Key == fileName select t.Value).FirstOrDefault();
		}

		#endregion

		#region Delete doc item

		private void DelDocItem_DialogEvent(object source, DialogEventArgs e)
		{
			if(e.Dialog.DialogResult != DialogResult.Yes)
				return;

			var mf = e.Dialog as MessageForm;

		    if (mf != null)
		    {
		        int imageId = (int)mf.Tag;

		        ServerInfo serverInfo;

                string filename = GetFile(imageId, imagePDF.ImageDisplayed ? ".pdf" : ".tif", out serverInfo);

		        if (!string.IsNullOrEmpty(filename))
		        {
                    string tmpFileName = Path.GetTempFileName();

                    File.Copy(filename, tmpFileName, true);

                    var isDeleted = Environment.DocImageData.Delete(imageId);

                    if (isDeleted)
                        UndoRedoStackRemoveImageFromDoc(DocumentID, imageId, serverInfo, tmpFileName, imagePDF.ImageDisplayed ? "PDF" : "TIF");
		        }
		    }
		   
			RefreshDoc();
		}

        private string GetFile(int imageId, string extension, out ServerInfo serverInfo)
        {
            string filename = "";
            serverInfo = null;

            List<int> servers = Environment.DocImageData.GetLocalDocImageServers(imageId, Environment.GetLocalServersString());

            foreach (int t in servers)
            {
                serverInfo = Environment.GetLocalServer(t);
                filename = Path.Combine(serverInfo.Path, Environment.GetFileNameFromID(imageId)) + extension;

                if (File.Exists(filename))
                    break;
                filename = "";
            }

            return filename;
        }

	    /// <summary>
	    /// Формирование команды Undo RemoveImageFromDoc
	    /// </summary>
	    /// <param name="documentId"></param>
	    /// <param name="imageId"></param>
	    /// <param name="server"></param>
	    /// <param name="tmpFileName"> Копия оригинального файла</param>
	    /// <param name="imageType"></param>
	    /// <param name="image"></param>
	    private void UndoRedoStackRemoveImageFromDoc(int documentId, int imageId, ServerInfo server, string tmpFileName, string imageType)
        {
            Func<object[], bool> delegate1 = AddImageToDoc;
            Func<object[], bool> delegate2 = RemoveImageFromDoc;

            var undoText = Environment.StringResources.GetString("MainForm.REDOAddImageToDoc");
            var redoText = Environment.StringResources.GetString("MainForm.UNDOAddImageToDoc");

            Environment.UndoredoStack.Add("RemoveImageFromDoc", "RemoveImageFromDoc", undoText, redoText, null, new object[] { documentId, delegate1, delegate2, imageId, server, tmpFileName, imageType}, 0);
        }

        /// <summary>
        /// UNDO DocImage Delete
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private bool AddImageToDoc(object[] o)
        {
            int documentId = (int)o[0];
            int addImageId = (int)o[3];
            var server = (ServerInfo)o[4];
            string tmpFileNameRedo = (string)o[5];
            string imageType = (string)o[6];


            RefreshDoc();

            return false;
        }

        /// <summary>
        /// REDO DocImage Delete
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private bool RemoveImageFromDoc(object[] o)
        {
            int imageId = (int)o[4];
            string tmpFileNameUndo = (string)o[8];

            // Сохранение копии файла
            string tmpFileNameUndoTmp = Path.GetTempFileName();
            File.Copy(tmpFileNameUndo, tmpFileNameUndoTmp, true);

            var isDeleted = Environment.DocImageData.Delete(imageId);

            RefreshDoc();

            return isDeleted;
        }

		private void DelEformItem_DialogEvent(object source, DialogEventArgs e)
		{
			if(e.Dialog.DialogResult != DialogResult.Yes)
				return;
			var mf = e.Dialog as MessageForm;
			if(mf == null || !(mf.Tag is int))
				return;

			var docID = (int)mf.Tag;
			int delType = 0;
			var deletedImages = new ArrayList();
			if(hasImage)
				for(int i = 0; i < variantList.Items.Count; i++)
				{
					var item = variantList.Items[i] as VariantListItem;
					if(item == null)
						continue;
					if(item.Printed)
					{
						switch(delType)
						{
							case 0:
								delType = 2;
								break;
							case 1:
								delType = 3;
								break;
						}
						deletedImages.Add(item.ID);
					}
					else
						switch(item.Type)
						{
							case VariantType.MainImageOriginal:
							case VariantType.MainImage:
							case VariantType.ImageOriginal:
							case VariantType.Image:
								switch(delType)
								{
									case 0:
										delType = 1;
										break;
									case 3:
										delType = 3;
										break;
								}
								break;
						}
				}
			if(delType == 2)
			{
				if((new MessageForm(Environment.StringResources.GetString("DocControl_DelEformItem_DialogEvent_Message1"),
						Environment.StringResources.GetString("Warning"), MessageBoxButtons.YesNo)).ShowDialog() == DialogResult.Yes)
				{
					// 28960 Запрет повторного вызова sp_DeleteDoc
					Data.Repository.IDocumentRepository documentRepository = new Data.Repository.DocumentRepository();
					if(documentRepository.DeleteDoc(-1, docID, true))
					{
						ClearDoc();
						OnDocChanged(this, new Components.DocumentSavedEventArgs(curDocID, 0));
					}
				}
				return;
			}
			if(delType == 0)
			{
				Data.Repository.IDocumentRepository documentRepository = new Data.Repository.DocumentRepository();
				if(documentRepository.DeleteDoc(-1, docID, true))
				{
					ClearDoc();
					OnDocChanged(this, new Components.DocumentSavedEventArgs(curDocID, 0));
				}
				return;
			}
			if(Environment.DocDataData.Delete(docID))
			{
				if(hasImage)
					RefreshDoc();
				else
					ClearDoc();
				OnDocChanged(this, new Components.DocumentSavedEventArgs(curDocID, 0));
			}
		}

		#endregion

		#region Web

		private void WebShow()
		{
			if(curDocID > 0 && hasImage && (hasData || addEForm))
			{
				ReDrowIcon();

				if(imgID != 0)
					ShowDataPanel(true);
				else
					splitContainerDocument.Panel1Collapsed = true;

				if(showWebPanel)
					ShowHTMLDoc();
				else
					splitContainerDocument.Panel2Collapsed = true;
			}
		}

		private void ShowHTMLDoc()
		{
			Log.Logger.EnterMethod(this, "ShowHTMLDoc()");
			string url = null;
			canPrintEForm = false;
			CursorSleep();

			hasData = Environment.DocDataData.IsDataPresent(curDocID);
			if(hasData)
				addEForm = false;
			if(!showWebPanel)
				docComponent = new Components.DocControlComponent();

			// код типа документа
			int docTypeID = Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, curDocID, -1);
			if(docTypeID > -1)
			{
				canPrintEForm = Environment.PrintData.CanPrintEForm(docTypeID);
				url = Environment.DocTypeData.GetField(Environment.DocTypeData.URLField, docTypeID).ToString();
			}
			else
				canPrintEForm = false;

			if(string.IsNullOrEmpty(url))
				url = Environment.SettingsURLString;

			try
			{
				try
				{
					url = url.IndexOf("id=") > 0
							  ? url.Replace("id=",
											"id=" + curDocID.ToString() + "&docview=" +
											((subscribe != Guid.Empty) ? (subscribe.ToString()) : "1"))
							  : (url + ((url.IndexOf("?") > 0) ? "&" : "?") + "id=" + curDocID.ToString() + "&docview=" +
								 ((subscribe != Guid.Empty) ? (subscribe.ToString()) : "1"));
				}
				catch
				{
					url = url.IndexOf("id=") > 0
							  ? url.Replace("id=", "id=" + curDocID.ToString() + "&docview=" + (IsMain ? "1" : "2"))
							  : (url + ((url.IndexOf("?") > 0) ? "&" : "?") + "id=" + curDocID.ToString() + "&docview=" +
								 (IsMain ? "1" : "2"));
				}

				if(!showWebPanel || !hasImage)
					url += "&NoSign=1";
				ShowDataPanel(true);
				if(browser.Url == null || browser.Url.AbsoluteUri != url)
				{
					browser.NavigateTo(new Uri(url));
				}
				OnVarListIndexChange(this);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				ErrorShower.OnShowError(this, ex.Message, "Ошибка отображения");
			}
			finally
			{
				Log.Logger.LeaveMethod(this, "ShowHTMLDoc() url = " + (url == null ? "null" : url));
			}
		}

		private bool CanShowSplit()
		{
			return showWebPanel && hasImage && (hasData || addEForm);
		}

		private void ReDrowIcon()
		{
			CursorSleep();
			VariantListItem eform = null;
			variantList.SuspendLayout();
			for(int i = 0; i < variantList.Items.Count; i++)
			{
				var item = variantList.Items[i] as VariantListItem;
				if(item == null)
					continue;
				switch(item.Type)
				{
					case VariantType.Image:
						item.ImageIndex = !CanShowSplit() ? (int)VariantType.Image : 12;
						break;
					case VariantType.ImageOriginal:
						item.ImageIndex = !CanShowSplit() ? (item.IsPDF() ? 19 : (int)VariantType.ImageOriginal) : (int)VariantType.Data;
						break;
					case VariantType.MainImage:
						item.ImageIndex = !CanShowSplit() ? (int)VariantType.MainImage : 13;
						break;
					case VariantType.MainImageOriginal:
						item.ImageIndex = !CanShowSplit() ? (item.IsPDF() ? 20 : (int)VariantType.MainImageOriginal) : 14;
						break;
					case VariantType.Data:
						eform = item;
						break;
				}
			}
			variantList.ResumeLayout();
			if((!showWebPanel && (hasData || addEForm)) || !hasImage)
			{
				var item = new VariantListItem(curDocID, VariantType.Data, Environment.StringResources.GetString("EForm")) { ImageIndex = (int)VariantType.Data };
				variantList.Items.Add(item);
			}
			else if(eform != null)
			{
				int index = -1;
				if(eform.Selected)
					index = eform.Index > 0 ? eform.Index - 1 : 0;
				variantList.Items.Remove(eform);
				if(index > -1 && variantList.Items.Count > 0)
					variantList.Items[index].Selected = true;
			}
			RecalcIconPlace(null);
			CursorWake();
		}

		#endregion

		/// <summary>
		/// получение количества страниц в файле
		/// </summary>
		/// <param name="fileName">имя файла</param>
		/// <param name="isPDF">где проверять, в pdf ша изображении</param>
		/// <returns>количество страниц</returns>
		public int GetFilePagesCount(string fileName, bool isPDF)
		{
			if(!isPDF)
				return image.GetFilePagesCount(fileName);
			else
				return imagePDF.GetFilePagesCount(fileName);
		}

		private void image_FileNameChanged(object sender, FileNameChangedArgs e)
		{
			try
			{
				fileName = e.FileName;
                if (imgID <= 0 && !string.IsNullOrEmpty(fileName))
					IsReadonly = !Environment.IsAccessible(fileName);
				else
					IsReadonly = false;
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private void DocControl_Load(object sender, EventArgs e)
		{
			if(!DesignerDetector.IsComponentInDesignMode(this))
				try
				{
					if(splitContainerWhole.Height > 50)
						splitContainerWhole.SplitterDistance = Environment.Layout.LoadIntOption("SpliterImagePlace", splitContainerWhole.SplitterDistance);
					if(!splitContainerDocument.Visible || splitContainerDocument.Height > 50)
						splitContainerDocument.SplitterDistance = Environment.Layout.LoadIntOption("SpliterDataPlace", splitContainerDocument.SplitterDistance);
					if(!splitContainerBrowser.Visible || splitContainerBrowser.Height > 50)
						splitContainerBrowser.SplitterDistance = Environment.Layout.LoadIntOption("SplitterExportPlace", splitContainerBrowser.SplitterDistance);
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
				}
			imagePDF.Visible = imagePDF.ImageDisplayed;
			imagePDF.NeedSave += image_NeedSave;
			imagePDF.ToolSelected += image_ToolSelected;
		}

		private void DocControl_HandleDestroyed(object sender, EventArgs e)
		{
			Environment.Settings.Save();
		}

		private void faxWatcher_Created(object sender, FileSystemEventArgs e)
		{
			faxWatcher.Created -= faxWatcher_Created;
			faxWatcher.Renamed -= faxWatcher_Renamed;
			if(!image.ImageDisplayed && faxWatcher.Path.Length > 0 && faxWatcher.Filter.Length > 0)
			{
				image.FileName = faxWatcher.Path + "\\" + faxWatcher.Filter;
				UpdateNavigation();
			}
		}

		private void faxWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			faxWatcher.Created -= faxWatcher_Created;
			faxWatcher.Renamed -= faxWatcher_Renamed;
			if(!image.ImageDisplayed && faxWatcher.Path.Length > 0 && faxWatcher.Filter.Length > 0)
			{
				image.FileName = faxWatcher.Path + "\\" + faxWatcher.Filter;
				UpdateNavigation();
			}
		}

		private void image_ToolSelected(object sender, ImageControl.ImageControl.ToolSelectedEventArgs e)
		{
			OnToolSelected(e);
		}

		private void webTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if(Disposing || IsDisposed)
				return;
			webTimer.Elapsed -= webTimer_Elapsed;
			webTimer.Stop();
			BrowserRefresh();
		}

		private void transTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if(Disposing || IsDisposed)
				return;
			var timer1 = sender as System.Timers.Timer;
			if(timer1 != null)
			{
				timer1.Elapsed -= transTimer_Elapsed;
				timer1.Stop();
				timer1 = null;
			}
			if(!InvokeRequired)
				transactionControl.DocumentID = curDocID;
			else
				BeginInvoke(new ElapsedEventHandler(transTimer_Elapsed), new[] { sender, e });
		}

		private delegate void BrowserRefreshDelegate(bool force);

		private void toolStripMenuItemRefresh_Click(object sender, EventArgs e)
		{
			Environment.OnNeedRefresh(this.FindForm(), EventArgs.Empty);
			//browser.Refresh();
		}

		private void toolStripMenuItemText_Click(object sender, EventArgs e)
		{
		}

		private void toolStripMenuItemProperties_Click(object sender, EventArgs e)
		{
			browser.ShowPropertiesDialog();
		}

		private void contextMenuStripBrowser_Opening(object sender, CancelEventArgs e)
		{
			if(browser.Document != null)
			{
				if(browser.IsCommandEnable(NativeMethods.OLECMDID.OLECMDID_COPY))
				{
					ToolStripMenuItemCut.Visible = true;
					ToolStripMenuItemCut.Enabled = false;
					ToolStripMenuItemCopy.Visible = true;
					ToolStripMenuItemCopy.Enabled = true;
					ToolStripMenuItemPaste.Visible = true;
					ToolStripMenuItemPaste.Enabled = false;
				}
				else
				{
					ToolStripMenuItemCut.Visible = false;
					ToolStripMenuItemCut.Enabled = false;
					ToolStripMenuItemCopy.Visible = false;
					ToolStripMenuItemCopy.Enabled = false; 
					ToolStripMenuItemPaste.Visible = false;
					ToolStripMenuItemPaste.Enabled = false;
				}
				ToolStripMenuItemSelectAll.Visible = true;
				ToolStripMenuItemSelectAll.Enabled = true;
			}
			else
			{
				ToolStripMenuItemCut.Visible = false;
				ToolStripMenuItemCut.Enabled = false;
				ToolStripMenuItemCopy.Visible = false;
				ToolStripMenuItemCopy.Enabled = false;
				ToolStripMenuItemPaste.Visible = false;
				ToolStripMenuItemPaste.Enabled = false;
				ToolStripMenuItemSelectAll.Visible = false;
				ToolStripMenuItemSelectAll.Enabled = false;
			}
		}

		private void ToolStripMenuItemCopy_Click(object sender, EventArgs e)
		{
			if(browser.Document != null)
			{
				browser.ExecCopy();
			}
		}

		private void ToolStripMenuItemSelectAll_Click(object sender, EventArgs e)
		{
			if(browser.Document != null)
			{
				browser.Document.ExecCommand("SelectAll", false, null);
			}
		}

		public bool HasAnnotation()
		{
			return image.HasAnnotation() || imagePDF.HasAnnotation();
		}

		public void RefreshLinks()
		{
			if(!Environment.IsConnectedDocs)
				return;
			HasDocLinks = Environment.DocLinksData.HasLinks(curDocID);
		}

		public void ShowProperties()
		{
			ShowProperties(true);
		}

		public void ShowProperties(bool showfilename)
		{
			if(IsPDFMode)
			{
				imagePDF.ShowProperties();
			}
			else
			{
				if(showfilename)
					image.ShowProperties();
				else
					image.ShowProperties(CurDocString);
			}
		}

		public string GetImageType()
		{
			return image.GetImageType();
		}

		public int SendFaxNew()
		{
			//return SendFax();
			if(!Environment.IsFaxSender(curDocID))
				return curDocID;
			TestImage(Environment.ActionBefore.SendFaxAndMail);

			string oldFileName = fileName;
			int docID = curDocID;
			int imID = imgID;

			Objects.TmpFile tf = Environment.GetTmpFileByKey(this.fileName);
			if(tf != null && tf.CurAct == Environment.ActionBefore.SendFaxAndMail)
			{
				if(!File.Exists(tf.TmpFullName))
					SaveToTmpCopy(fileName, tf.TmpFullName, IsPDFMode);

				oldFileName = tf.TmpFullName;
				docID = -1;
				imID = -1;
			}

			if(Environment.DocToSend.ContainsKey(oldFileName))
			{
				var dialog = Environment.DocToSend[oldFileName] as Dialogs.SendOutDialog;
				if(dialog != null)
				{
					dialog.BringToFront();
					dialog.Activate();
				}
			}
			else
			{
				Dialogs.SendOutDialog dialog = new Dialogs.SendOutDialog();
				if(curDocID > 0)
					dialog.ImageAdd(curDocID, this.ImageID);
				else
					dialog.FileAdd(oldFileName, CurDocString);
				dialog.Show();
			}
			return -1;
		}

		public int SendFaxNew(int faxID)
		{
			//return SendFax(faxID);
			if(!Environment.IsFaxSender())
				return 0;
				
			Dialogs.SendOutDialog dialog = new Dialogs.SendOutDialog();
			dialog.FaxAdd(curDocID, this.ImageID, faxID);
			dialog.DialogEvent += FaxOutDialog_DialogEvent;
			dialog.Show();
			Environment.DocToSend.TryAdd(fileName, dialog);
			return -1;
		}

		private void FaxOutDialog_DialogEvent(object source, DialogEventArgs e)
		{
			var dialog = e.Dialog as Dialogs.SendOutDialog;

			/*if(dialog == null || string.IsNullOrEmpty(dialog.FileName))
				return;
			Form f = null;
			if(Environment.DocToSend.ContainsKey(dialog.FileName))
				Environment.DocToSend.TryRemove(dialog.FileName, out f);
			if(Document.Environment.TmpFilesContains(dialog.FileName))
			{
				Document.Objects.TmpFile tf = Environment.GetTmpFileByValue(dialog.FileName);
				if(tf != null)
				{
					tf.CurAct = Environment.ActionBefore.None;
					tf.LinkCnt--;
				}
			}*/
		}

		internal void LoadImage(Components.ImageToSend imgToSend)
		{
			if(!string.IsNullOrEmpty(imgToSend.SendFileName))
				LoadFile(imgToSend.SendFileName, 0);
			else
				if(imgToSend.ImageID > 0)
					LoadImage(imgToSend.ImageID, 0);
				else
					LoadFile(imgToSend.SourceFileName, 0);
		}

		private bool externalSave = false;

		[DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ExternalSave
		{
			get{ return externalSave;}
			set {externalSave = true;}
		}

		internal void SetVisiblePages(List<int> list)
		{
			if(list != null && list.Count > 0)
			{
				if(image.ImageDisplayed)
					image.SetVisiblePages(list.Select(x => x - 1).ToList());
			}
		}
	}
}