using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Controls;
using Kesco.Lib.Win.Document.Controls.PdfViewControl;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public class PrintAllDialog : Form
    {
        private int row;
        private int[] values;
        private List<PrinterObjectClass> printerObject;
        private int poindex;
        private bool cancel;
        private bool printImage;
        private bool mainOnly;
        private bool eForm;
        private bool block;
        private Hashtable eFormSelection;
        private System.Timers.Timer timer;
        private short copiescount;
        private PrintOrientation orientation; // ориентация страницы
        private PrintScale scaleMode; // масштаб печати (два режима)
        private bool annotations; // печатать ли аннотации
        private PrinterInfo printerInfo; // информация о принторе
        private PrinterOp prn;

        private Options.Folder formLayout; // пункт в реестре

        private Label label1;
        private Button buttonCancel;
        private ImageShowControl imgEdit;
        private Panel panelBrowser;
        private Panel panelImg;
        private Kesco.Lib.Win.Web.ExtendedBrowserControl webBrowser;
        private Label label2;
        private Panel panelReport;
        private Label labelNum;

        private Container components;

        private PDFView pdfEdit;
        private bool _isPDF;

        public PrintAllDialog(int[] values)
        {
            this.values = values;
            using (
                var dialog = new DocPrintDialog(0, Environment.Settings.Folders.Add("Print"), 0,
                                                Environment.StringResources.GetString("Dialog_PrintAllDialog_Title1"))
                                 {PrintImage = false})
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    printImage = dialog.PrintImage;
                    mainOnly = dialog.PrintOnlyMain;
                    eForm = dialog.PrintEForm;
                    orientation = dialog.Orientation;
                    scaleMode = dialog.ScaleMode;
                    annotations = dialog.Annotations;
                    copiescount = dialog.CopiesCount;
                    prn = new PrinterOp(DocPrintDialog.Printer, this);
                    printerInfo = Environment.Printers.List[DocPrintDialog.Printer] as PrinterInfo ?? prn.GetInfo();
                }
            }

            if (printImage || eForm)
            {
                InitializeComponent();
            }
            else
            {
                Load += PrintAllDialog_Load;
                values = null;
                return;
            }

            InitLayout();
        }

        public PrintAllDialog(int[] docIDs, PrintAllFoldersType folderType)
        {
			switch(folderType)
			{
				case PrintAllFoldersType.WorkFolder:
				case PrintAllFoldersType.SearchFolder:
				case PrintAllFoldersType.InquiryFolder:
					values = new int[docIDs.Length];
					docIDs.CopyTo(values, 0);
					break;
				case PrintAllFoldersType.ArchivFolder:
					break;
			}

            using (
                var dialog = new DocPrintDialog(0, Environment.Settings.Folders.Add("Print"), 0,
                                                Environment.StringResources.GetString("Dialog_PrintAllDialog_Title1")))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    printImage = dialog.PrintImage;
                    mainOnly = dialog.PrintOnlyMain;
                    eForm = dialog.PrintEForm;
                    orientation = dialog.Orientation;
                    scaleMode = dialog.ScaleMode;
                    annotations = dialog.Annotations;
                    copiescount = dialog.CopiesCount;
                    prn = new PrinterOp(DocPrintDialog.Printer, this);
                    printerInfo = Environment.Printers.List[DocPrintDialog.Printer] as PrinterInfo ?? prn.GetInfo();
                }
            }

            if (printImage || eForm)
            {
                InitializeComponent();
            }
            else
            {
                Load += PrintAllDialog_Load;
                values = null;
                return;
            }

            InitLayoutProperties();
        }

        public PrintAllDialog(int folderID, int empID, PrintAllFoldersType folderType)
        {
            switch (folderType)
            {
                case PrintAllFoldersType.WorkFolder:
                    using (
                        var dt = Environment.DocData.GetWorkFolderDocs(folderID, empID,
                                                                       Environment.CurCultureInfo.
                                                                           TwoLetterISOLanguageName))
                    {
                        if (dt.Rows.Count > 0)
                        {
                            values = new int[dt.Rows.Count];
                            for (int i = 0; i < values.Length; i++)
                                values[i] = (int) dt.Rows[i][Environment.DocData.IDField];
                        }
                        else
                        {
                            Load += PrintAllDialog_Load;
                            values = null;
                            return;
                        }
                        dt.Dispose();
                    }
                    break;
                case PrintAllFoldersType.ArchivFolder:
                    break;
                case PrintAllFoldersType.SearchFolder:
                    using (
                        var dt = Environment.DocData.GetFoundDocs(Environment.CurCultureInfo.Name, empID,
                                                                  Environment.UserSettings.PersonID))
                    {
                        if (dt.Rows.Count > 0)
                        {
                            values = new int[dt.Rows.Count];
                            for (int i = 0; i < values.Length; i++)
                                values[i] = (int) dt.Rows[i][Environment.DocData.IDField];
                        }
                        else
                        {
                            Load += PrintAllDialog_Load;
                            values = null;
                            return;
                        }
                        dt.Dispose();
                    }
                    break;
                case PrintAllFoldersType.InquiryFolder:
                    string xml = Environment.QueryData.GetField(Environment.QueryData.XMLField, folderID).ToString();
                    string sql = Data.DALC.Documents.Search.Options.GetSQL(xml);
                    using (
                        DataTable dt = Environment.DocData.GetQueryDocs(sql,
                                                                        Environment.CurCultureInfo.
                                                                            TwoLetterISOLanguageName, empID,
                                                                        Environment.UserSettings.PersonID))
                    {
                        if (dt.Rows.Count > 0)
                        {
                            values = new int[dt.Rows.Count];
                            for (int i = 0; i < values.Length; i++)
                                values[i] = (int) dt.Rows[i][Environment.DocData.IDField];
                        }
                        else
                        {
                            Load += PrintAllDialog_Load;
                            values = null;
                            return;
                        }
                        dt.Dispose();
                    }
                    break;
            }

            using (
                var dialog = new DocPrintDialog(0, Environment.Settings.Folders.Add("Print"), values,
                                                Environment.StringResources.GetString("Dialog_PrintAllDialog_Title1")))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    printImage = dialog.PrintImage;
                    mainOnly = dialog.PrintOnlyMain;
                    eForm = dialog.PrintEForm;
                    orientation = dialog.Orientation;
                    scaleMode = dialog.ScaleMode;
                    annotations = dialog.Annotations;
                    copiescount = dialog.CopiesCount;
                    prn = new PrinterOp(DocPrintDialog.Printer, this);
                    printerInfo = Environment.Printers.List[DocPrintDialog.Printer] as PrinterInfo ?? prn.GetInfo();
                }
            }

            if (printImage || eForm)
            {
                InitializeComponent();
            }
            else
            {
                Load += PrintAllDialog_Load;
                values = null;
                return;
            }

            InitLayoutProperties();
        }

        private void InitLayoutProperties()
        {
            formLayout = Environment.Layout.Folders.Add(Name);
            Width = formLayout.LoadIntOption("Width", Width);
            Height = formLayout.LoadIntOption("Height", Height);
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
                new System.ComponentModel.ComponentResourceManager(typeof (PrintAllDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panelBrowser = new System.Windows.Forms.Panel();
            this.webBrowser = new Lib.Win.Web.ExtendedBrowserControl();
            this.panelImg = new System.Windows.Forms.Panel();
            this.pdfEdit = new PDFView();
            this.imgEdit = new ImageShowControl();
            this.panelReport = new System.Windows.Forms.Panel();
            this.labelNum = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelBrowser.SuspendLayout();
            this.panelImg.SuspendLayout();
            this.panelReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // panelBrowser
            // 
            resources.ApplyResources(this.panelBrowser, "panelBrowser");
            this.panelBrowser.Controls.Add(this.webBrowser);
            this.panelBrowser.Name = "panelBrowser";
            // 
            // webBrowser
            // 
            this.webBrowser.AllowWebBrowserDrop = false;
            resources.ApplyResources(this.webBrowser, "webBrowser");
            this.webBrowser.EnableInternalReloader = false;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.SelfNavigate = false;
			this.webBrowser.NeedEvent = true;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            this.webBrowser.DocumentCompleted +=new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
			this.webBrowser.PrintCompleted += Browser_PrintCompleted;
			// 
			// panelImg
			// 
			resources.ApplyResources(this.panelImg, "panelImg");
            this.panelImg.Controls.Add(this.pdfEdit);
            this.panelImg.Controls.Add(this.imgEdit);
            this.panelImg.Name = "panelImg";
            // 
            // pdfEdit
            // 
            this.pdfEdit.CurrentStamp = null;
            this.pdfEdit.CurrentStampID = 0;
            resources.ApplyResources(this.pdfEdit, "pdfEdit");
            this.pdfEdit.FileName = "";
            this.pdfEdit.ForceReplicate = false;
            this.pdfEdit.ImageID = 0;
            this.pdfEdit.IsEditNotes = false;
            this.pdfEdit.IsMoveImage = false;
            this.pdfEdit.IsSelectionMode = false;
            this.pdfEdit.Name = "pdfEdit";
            this.pdfEdit.NeedPreview = false;
            this.pdfEdit.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.pdfEdit.Page = 0;
            this.pdfEdit.ScrollPositionX = 0;
            this.pdfEdit.ScrollPositionY = 0;
            this.pdfEdit.SelectionMode = false;
            this.pdfEdit.ShowThumbPanel = false;
            this.pdfEdit.SplinterPlace = 25;
            this.pdfEdit.UseLock = false;
            // 
            // imgEdit
            // 
            this.imgEdit.AnnotationDraw = false;
            this.imgEdit.CurrentStamp = null;
            this.imgEdit.CurrentStampID = 0;
            this.imgEdit.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.imgEdit, "imgEdit");
            this.imgEdit.DPI = 96F;
            this.imgEdit.ForceReplicate = false;
            this.imgEdit.Image = null;
            this.imgEdit.ImageID = 0;
            this.imgEdit.ImageResolutionX = 1;
            this.imgEdit.ImageResolutionY = 1;
            this.imgEdit.IsCorrectScaleDrawThumbnailPanel = true;
            this.imgEdit.IsEditNotes = false;
            this.imgEdit.IsMoveImage = true;
            this.imgEdit.IsSelectionMode = false;
            this.imgEdit.IsVerifyFile = true;
            this.imgEdit.Name = "imgEdit";
            this.imgEdit.Page = 0;
            this.imgEdit.SaveStampsInternal = true;
            this.imgEdit.ScrollPositionX = 0;
            this.imgEdit.ScrollPositionY = 0;
            this.imgEdit.IsSelectionMode = false;
            this.imgEdit.ShowThumbPanel = false;
            this.imgEdit.SplinterPlace = 140;
            this.imgEdit.ThumbnailPanelOrientation =
                Lib.Win.ImageControl.ImageControl.TypeThumbnailPanelOrientation.Left;
            this.imgEdit.TypeWorkThumbnailImagesPanel = 3;
            this.imgEdit.UseLock = false;
            this.imgEdit.Zoom = 100;
            // 
            // panelReport
            // 
            resources.ApplyResources(this.panelReport, "panelReport");
            this.panelReport.Controls.Add(this.labelNum);
            this.panelReport.Controls.Add(this.label2);
            this.panelReport.Name = "panelReport";
            // 
            // labelNum
            // 
            resources.ApplyResources(this.labelNum, "labelNum");
            this.labelNum.Name = "labelNum";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // PrintAllDialog
            // 
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.ControlBox = false;
            this.Controls.Add(this.panelBrowser);
            this.Controls.Add(this.panelReport);
            this.Controls.Add(this.panelImg);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Name = "PrintAllDialog";
            this.Closed += new System.EventHandler(this.PrintAllDialog_Closed);
            this.Load += new System.EventHandler(this.PrintAllDialog_Load);
            this.panelBrowser.ResumeLayout(false);
            this.panelImg.ResumeLayout(false);
            this.panelReport.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

		private void PrintDoc(int docID)
		{
			if(printerObject != null)
				printerObject.Clear();
			poindex = 0;
			if(docID < 1)
			{
				OnPrintDoc();
				return;
			}
			if(printImage && !cancel)
			{
				Console.WriteLine("{0}: printed ID: {1}", DateTime.Now.ToString("HH:mm:ss fff"), docID);
				if(mainOnly)
				{
					object obj = Environment.DocData.GetField(Environment.DocData.MainImageIDField, docID);
					if(obj != null && obj is int && !cancel)
					{
						int mainImageID = (int)obj;
						bool isPDF = "pdf".Equals(Environment.DocImageData.GetField(Environment.DocImageData.ImageTypeField, mainImageID).ToString(), StringComparison.CurrentCultureIgnoreCase);
						PrintImage(docID, mainImageID, isPDF);
					}
				}
				else
				{
					using(DataTable dt = Environment.DocImageData.GetDocImages(docID))
					using(DataTableReader dr = dt.CreateDataReader())
					{
						while(dr.Read())
						{
							var imgId = (int)dr[Environment.DocImageData.IDField];
							if(imgId > 0 && !cancel)
							{
								bool isPDF = "pdf".Equals(dr[Environment.DocImageData.ImageTypeField].ToString(), StringComparison.CurrentCultureIgnoreCase);
								PrintImage(docID, imgId, isPDF);
							}
						}
						dr.Close();
						dr.Dispose();
						dt.Dispose();
					}
				}
			}

			// есть ли данные у документа
			if(eForm && Environment.DocDataData.IsDataPresent(docID))
			{
				object obj = Environment.DocData.GetField(Environment.DocData.DocTypeIDField, docID);
				if(!obj.Equals(DBNull.Value))
				{
					var docTypeID = (int)obj;
					if(eFormSelection == null)
						eFormSelection = new Hashtable();

					if(eFormSelection.ContainsKey(docTypeID))
					{
						var list = eFormSelection[docTypeID] as List<PrinterObjectClass>;

						bool show = false;
						using(DataTable dt = Environment.PrintData.GetEFormPrintTypeData(docTypeID, docID))
						{
							foreach(PrinterObjectClass t in list)
							{
								show = !dt.Rows.Cast<DataRow>().Any(x => x[Environment.PrintData.IDField].Equals(t.TypeID));
								if(show)
									break;
							}
							if(show || list.Count == 0)
							{
								using(var dialog = new PrintEFormSelectDialog(docTypeID, docID))
								{
									if(dialog.ShowDialog() == DialogResult.OK)
									{
										list = dialog.PrinterObjectList;
										eFormSelection[docTypeID] = list;
									}
								}
							}
							dt.Dispose();
						}
						if(list != null)
						{
							poindex = 0;
							printerObject = new List<PrinterObjectClass>(list);
							StartSwitchPrintEform();
						}
					}
					else
					{
						if(Environment.PrintData.CanPrintEForm(docTypeID))
						{
							block = true;
							try
							{
								panelBrowser.Visible = false;
								panelImg.Visible = false;
								using(var dialog = new PrintEFormSelectDialog(docTypeID, docID))
								{
									if(dialog.ShowDialog() == DialogResult.OK)
									{
										List<PrinterObjectClass> list = dialog.PrinterObjectList;
										eFormSelection.Add(docTypeID, list);

										if(list != null)
										{
											poindex = 0;
											printerObject = new List<PrinterObjectClass>(list);
											StartSwitchPrintEform();
										}
									}
								}
							}
							catch(Exception ex)
							{
								Data.Env.WriteToLog(ex);
								MessageForm.Show(ex.Message, Environment.StringResources.GetString("Error"));
							}
						}
						else
							OnPrintDoc();
					}
				}
			}
			else
				OnPrintDoc();
		}

        private void PrintImage(int docID, int imageID, bool isPDF)
        {
            _isPDF = isPDF;
            pdfEdit.Visible = isPDF;
            imgEdit.Visible = !isPDF;

            panelBrowser.Visible = false;
            panelImg.Visible = true;

            if (!isPDF)
            {
                imgEdit.ImageID = imageID;
                imgEdit.FitTo(0, true);
            }
            else
            {
                pdfEdit.ImageID = imageID;
                pdfEdit.FitTo(0, true);
            }

            panelImg.BringToFront();
            if (!cancel)
            {
                ImageControl.ImageControl.PrintActionHandler handler = PrintAction;
                if (!isPDF)
                    imgEdit.Print(handler, docID, imageID, "document", copiescount);
                else
                    pdfEdit.Print(handler, docID, imageID, "document", copiescount);
            }
            imgEdit.FileName = pdfEdit.FileName = "";
        }

        public void PrintAction(bool print, int docID, int imageID, string fileName, int startPage, int endPage,
                                int countPage, string docName, short copiesCount)
        {
            try
            {
                using (var dialog = new DocPrintDialog(docID, imageID, fileName, startPage, endPage, countPage, Environment.Settings.Folders.Add("Print"), docName, copiesCount, _isPDF))
                {
                    dialog.Print();
                }
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex, fileName);
                MessageBox.Show(ex.Message, Environment.StringResources.GetString("Error"));
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            cancel = true;
            Close();
        }

        private AutoResetEvent syncPrintEForm = new AutoResetEvent(true);

		private void StartSwitchPrintEform()
		{
			int docID = values[row - 1];
			if(poindex < printerObject.Count)
			{
				poindex++;
				PrinterObjectClass po = printerObject[poindex - 1];
				GC.Collect();
				syncPrintEForm.WaitOne(30000, false);
				Console.WriteLine("{0}: printing form {1}", DateTime.Now.ToString("HH:mm:ss fff"), po.TypeID);
				if(InvokeRequired)
					Invoke(new Data.Action<int, string, int, int, short, short>(SwitchPrintEform), new object[] { po.PrintType, po.URL, docID, po.TypeID, po.PaperSize, copiescount });
				else
					SwitchPrintEform(po.PrintType, po.URL, docID, po.TypeID, po.PaperSize, copiescount);
			}
			else
			{
				if(timer == null)
					timer = new System.Timers.Timer(200) { AutoReset = false };
				else
				{
					timer.Elapsed -= timer_Elapsed;
					timer.Interval = 200;
				}
				timer.Elapsed += timer_Elapsed;
				timer.Start();
			}
		}

		private void SwitchPrintEform(int printTypeID, string url, int docID, int printID, short paperSize, short copiesCount)
		{
			switch(printTypeID)
			{
				case 0:
					StartPrintEFormWithOrientation(printTypeID, url, docID, printID, paperSize);
					break;
				case 2:
				case 3:
					if(InvokeRequired)
						Invoke(new Action<string, int, short>(StartPrintEForm), new object[] { DocControl.PrintUrl(url, docID, printID, true), printTypeID, paperSize });
					else
						StartPrintEForm(DocControl.PrintUrl(url, docID, printID, true), printTypeID, paperSize);
					break;
				case 1:
					if(url.StartsWith("http"))
					{
						StartPrintEFormWithOrientation(printTypeID, url, docID, printID, paperSize);
					}
					else
					{
						try
						{
							panelReport.Visible = true;
							labelNum.Text = docID.ToString();
							Console.WriteLine("{0}: Report print start", DateTime.Now.ToString("HH:mm:ss fff"));
							Environment.Report.EndPrint += Report_EndPrint;
							if(
								!Environment.Report.PrintReport(DocPrintDialog.Printer, url, docID, printID,
																paperSize, copiescount))
							{
								Environment.Report.EndPrint -= Report_EndPrint;
								block = false;
								panelReport.Visible = false;
								StartSwitchPrintEform();
							}
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

        private void StartPrintEFormWithOrientation(int printTypeID, string url, int docID, int printID, short paperSize)
        {
            string printurl = DocControl.PrintUrl(url, docID, printID, true);
            try
            {
                var webClient = new WebClient {Credentials = CredentialCache.DefaultCredentials};
                byte[] ret = webClient.DownloadData(printurl + "&orientation=1");
                string orient = Encoding.Default.GetString(ret);
                printTypeID = orient.Trim().ToLower() == "landscape" ? 3 : 2;
            }
            catch (Exception ex)
            {
                printTypeID = 2;
                Data.Env.WriteToLog(ex);
            }
            if (InvokeRequired)
                Invoke(new Action<string, int, short>(StartPrintEForm),
                       new object[] {printurl, printTypeID, paperSize});
            else
                StartPrintEForm(printurl, printTypeID, paperSize);
        }

        private void StartPrintEForm(string url, int printTypeID, short paperSize)
        {
            panelBrowser.Visible = true;
            if (webBrowser == null)
                if (InvokeRequired)
                {
                    Console.WriteLine("{0}: Need invoke broswer restore", DateTime.Now.ToString("HH:mm:ss fff"));
                    return;
                }
                else
                    RestoreBrowser();
            panelImg.Visible = false;
            webBrowser.Tag = new Classes.Tag {PaperSize = paperSize, TypeID = printTypeID};
            panelBrowser.BringToFront();
            webBrowser.SelfNavigate = true;
            webBrowser.Navigate(url);
        }

		private void RestoreBrowser()
		{
			// 
			// webBrowser
			// 
			ComponentResourceManager resources = new ComponentResourceManager(typeof(PrintAllDialog));
			webBrowser = new Kesco.Lib.Win.Web.ExtendedBrowserControl();
			panelBrowser.SuspendLayout();
			SuspendLayout();
			webBrowser.AllowWebBrowserDrop = false;
			resources.ApplyResources(webBrowser, "webBrowser");
			webBrowser.EnableInternalReloader = false;
			webBrowser.IsWebBrowserContextMenuEnabled = false;
			webBrowser.Name = "webBrowser";
			webBrowser.SelfNavigate = false;
			webBrowser.NeedEvent = true;
			webBrowser.WebBrowserShortcutsEnabled = false;
			webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
			webBrowser.PrintCompleted += Browser_PrintCompleted;
			panelBrowser.Controls.Add(webBrowser);
			panelBrowser.ResumeLayout(false);
			ResumeLayout(false);
		}

        private void Report_EndPrint(object sender, PrintEventArgs e)
        {
            Environment.Report.EndPrint -= Report_EndPrint;
            block = false;
            if (InvokeRequired)
                Invoke((MethodInvoker)(delegate
                                      {
                                          panelReport.Visible = false;
                                          StartSwitchPrintEform();
                                      }));
            else
            {
                panelReport.Visible = false;
                StartSwitchPrintEform();
            }
        }

        private void PrintAllDialog_Load(object sender, EventArgs e)
        {
            if (values != null && values.Length > 0)
            {
                row = 0;
                if (timer == null)
                {
                    timer = new System.Timers.Timer(200);
                    timer.Elapsed += timer_Elapsed;
                    timer.AutoReset = false;
                }
                else
                {
                    timer.Interval = 200;
                }
                timer.Start();
            }
            else
                Close();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            timer.Elapsed -= timer_Elapsed;
            if (Disposing || IsDisposed)
                return;
            OnPrintDoc();
        }

        private void OnPrintDoc()
        {
            Application.DoEvents();
            if (InvokeRequired)
                BeginInvoke((MethodInvoker)(OnPrintDoc));
            else
            {
                if (cancel || (!printImage && !eForm))
                    Close();
                if (row < values.Length)
                {
                    Console.WriteLine("{0}: row {1} docID {2}", DateTime.Now.ToString("HH:mm:ss fff"), row, values[row]);
                    row++;
                    if (InvokeRequired)
                        Invoke(new Action<int>(PrintDoc), new object[] {values[row - 1]});
                    else
                        PrintDoc(values[row - 1]);
                }
                else if (!block)
                    Close();
            }
        }

        private void PrintAllDialog_Closed(object sender, EventArgs e)
        {
            if (formLayout == null)
                return;
            formLayout.Option("Width").Value = Width;
            formLayout.Option("Height").Value = Height;
            formLayout.Save();
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Web.ExtendedBrowserControl browser = sender as Web.ExtendedBrowserControl;
            if (browser == null)
                return;
			if(browser.Tag is Classes.Tag)
            {
				var tag = (Classes.Tag)browser.Tag;
                browser.Print(DocPrintDialog.Printer, Environment.ShowHelpString + "printtemplate" + ((tag.TypeID == 3) ? "landscape" : "") +  ".html" + ((copiescount > 1) ? ("?numcopies=" + copiescount.ToString()) : ""));
                syncPrintEForm.Set();
				if(browser.Version.Major < 7)
				{
					if(browser == webBrowser)
					{
						panelBrowser.Controls.Remove(webBrowser);
						webBrowser = null;
					}
					browser.DocumentCompleted -= webBrowser_DocumentCompleted;
					browser.PrintCompleted -= Browser_PrintCompleted;
					browser.Dispose();
				}
			}
            else
            {
                browser.Print(DocPrintDialog.Printer, Environment.ShowHelpString + "printtemplate.html");
				
                syncPrintEForm.Set();
				if(browser.Version.Major < 7)
				{
					if(browser == webBrowser)
					{
						panelBrowser.Controls.Remove(webBrowser);
						webBrowser = null;
					}
					browser.DocumentCompleted -= webBrowser_DocumentCompleted;
					browser.PrintCompleted -= Browser_PrintCompleted;
					browser.Dispose();
				}
			}
        }

		private void Browser_PrintCompleted(object sender, EventArgs e)
		{
			block = false;
			StartSwitchPrintEform();
		}
	}
}