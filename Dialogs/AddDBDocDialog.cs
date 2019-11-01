using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Classes;
using Kesco.Lib.Win.Document.Select;
using Kesco.Lib.Win.Error;
using Kesco.Lib.Win.Tiff;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Kesco.Lib.Win.Data.DALC.Documents;
using Kesco.Lib.Win.Document.Blocks;
using Kesco.Lib.Win.Document.Components;
using Kesco.Lib.Win.Document.Items;
using Kesco.Lib.Win.Trees;
using Timer = System.Timers.Timer;

namespace Kesco.Lib.Win.Document.Dialogs
{
    /// <summary>
    /// Диалог сохранения изображения в документ
    /// </summary>
    public partial class AddDBDocDialog : FreeDialog
    {
        private bool saveAfterColor = true;
        private int maxWidth = 592;
        private int maxHeight = 399;

        private int faxID;

        private ServerInfo server;
        private string docString;

        private bool faxMode;
        private bool scanMode;
        private string oldFileName;
        private FileInfo oldFI;

        internal bool isPDF;

        private int docID;
        private int typeID;

        private int archiveID;
        private int imageID;
        private int oldImageID;

        private int minPage;
        private int maxPage;
        private int pagesCount = -1;

        private DateTime createDate;
        private bool printed;

        private Timer timer;
        private bool buttonPush;
        private string pushedButton = "";

        private bool canAddEForm;
		private bool canAddSlaveEForm;

        internal SynchronizedCollection<int> ParentDocIDs;
        internal SynchronizedCollection<int> ChildDocIDs;

        private bool baseEdit = true;

        private string numberStr = "";
        private DateTime docDate = DateTime.MinValue;
        private bool protect;

        private bool messageNeeded;
        private SaveFaxInfo saveFaxInfo;

        private List<Bitmap> images;
        private int addImageID;

        private TypeSave typeSaveImage;

        public AddDBDocDialog(ServerInfo server, TypeSave typeSaveImage, DateTime scanDate, bool messageNeeded, int faxID, string docString, int oldImageID, string oldFileName, int minPage, int maxPage, bool isPdf, int pagesCount) :
            this(server, typeSaveImage, scanDate, messageNeeded, faxID, docString, 0, oldImageID, false, false, oldFileName, minPage, maxPage, pagesCount)
        {
            isPDF = isPdf;
        }

        public AddDBDocDialog(ServerInfo server, TypeSave typeSaveImage, DateTime scanDate, bool messageNeeded, int faxID, string docString, bool faxMode, bool scanMode, string oldFileName, SaveFaxInfo saveFaxInfo, Image images, bool isPdf, int pagesCount) :
            this(server, typeSaveImage, scanDate, messageNeeded, faxID, docString, faxMode, scanMode, oldFileName, saveFaxInfo, images, pagesCount)
        {
            isPDF = isPdf;
        }

        public AddDBDocDialog(ServerInfo server, TypeSave typeSaveImage, DateTime scanDate, bool messageNeeded, int faxID, string docString, bool faxMode, bool scanMode, string oldFileName, SaveFaxInfo saveFaxInfo, Image images, int pagesCount) :
            this(server, typeSaveImage, scanDate, messageNeeded, faxID, docString, 0, 0, faxMode, scanMode, oldFileName, 0, 0, pagesCount)
        {
            this.saveFaxInfo = saveFaxInfo;
            if (!this.saveFaxInfo.Equals(SaveFaxInfo.Empty))
            {
                if (this.saveFaxInfo.SenderId != 0)
                    personBlock.AddPerson(this.saveFaxInfo.SenderId, this.saveFaxInfo.SenderName);
                if (this.saveFaxInfo.RecipId != 0)
                    personBlock.AddPerson(this.saveFaxInfo.RecipId, this.saveFaxInfo.RecipName);
            }
            this.images = new List<Bitmap> { images as Bitmap };
        }

        public AddDBDocDialog(ServerInfo server, int imageID, int pagesCount, bool isPdf) :
            this(server, TypeSave.None, DateTime.MinValue, false, 0, null, imageID, 0, false, false, null, 0, 0, pagesCount)
        {
            isPDF = isPdf;
        }

        private AddDBDocDialog(ServerInfo server, TypeSave typeSaveImage, DateTime createDate, bool messageNeeded, int faxID, string docString, int imageID, int oldImageID, bool faxMode, bool scanMode, string oldFileName, int minPage, int maxPage, int pagesCount)
        {
            InitializeComponent();
            DoubleBuffered = true;

            backgroundWorker1.WorkerSupportsCancellation = true;
            this.typeSaveImage = typeSaveImage;
            checkBoxOpenDoc.Checked = false;

            this.server = server;
            this.faxID = faxID;
            this.docString = docString;

            this.faxMode = faxMode;
            this.scanMode = scanMode;
            this.oldFileName = oldFileName;
            if (!string.IsNullOrEmpty(oldFileName))
                oldFI = new FileInfo(oldFileName);

            this.minPage = minPage;
            this.maxPage = maxPage;
            this.oldImageID = oldImageID;
            this.pagesCount = pagesCount;

            this.createDate = createDate;
            this.messageNeeded = messageNeeded;
			//this.checkBoxName.Checked = true;

            // get settings
            {
                DataRow dr = Environment.SettingsData.GetSettings();
                if (dr != null)
                {
                    object obj = dr[Environment.SettingsData.SaveAddToWorkField];
                    if (obj is bool)
                        checkAddToWork.Checked = (bool)obj;

                    obj = dr[Environment.SettingsData.SaveOpenSavedField];
                    if (obj is bool)
                        checkGotoDoc.Checked = (bool)obj;

                    if (!messageNeeded)
                    {
                        obj = dr[Environment.SettingsData.SaveSendMessageField];
                        if (obj is bool)
                            checkSendMessage.Checked = (bool)obj;
                    }
                }
                dr = null;
            }
            if (TemplateClass.Template())
            {
                typeID = TemplateClass.DocTypeID;
                docDate = TemplateClass.DocDateTime;

                if (docDate > DateTime.MinValue)
                {
                    dateBlock.Value = docDate;
                    personBlock.HandleDateBlockEvent(this, new DateBlockEventArgs(docDate, true));
                }
                else
                    personBlock.HandleDateBlockEvent(this, new DateBlockEventArgs(DateTime.Now, false));

                int[] pids = TemplateClass.DocPersonsIDs;
                if (pids != null && pids.Length > 0)
                    for (int i = 0; i < pids.Length; i++)
                        personBlock.AddPerson(pids[i], Environment.GetPersonName(pids[i]));

                checkTemplate.Checked = true;
                docTypeBlock.ID = typeID;
            }

            if (imageID > 0)
            {
                this.imageID = imageID;
                DataRow dr = Environment.DocImageData.GetDocImage(imageID);
                if (dr != null)
                {
                    object obj = dr[Environment.DocImageData.DocIDField];
                    if (obj is int)
                        docID = (int)obj;

                    // archive
                    obj = dr[Environment.DocImageData.ArchiveIDField];
                    if (obj is int)
                        archiveID = (int)obj;
                    if (archiveID > 0)
                        original.Checked = true; // not enough?

                    // scan date
                    obj = dr[Environment.DocImageData.CreateDateField];
                    if (obj is DateTime)
                        this.createDate = (DateTime)obj;

                    obj = dr[Environment.DocImageData.CreateDateField];
                    if (obj != null)
                        printed = obj.Equals((byte)1);
                }
                dr = null;
            }

            if (this.createDate != null && this.createDate != DateTime.MinValue)
                labelScanDate.Text += this.createDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
            ParentDocIDs = new SynchronizedCollection<int>();
            ChildDocIDs = new SynchronizedCollection<int>();

            docLinksBlock.Added += docLinksBlock_Added;
            dateBlock.RaiseDateBlockEvent += HandleDateBlockEvent;

		    DataRow ddr = Environment.DocImageData.GetDocImage(oldImageID);
		    if (ddr != null)
		    {
                object obj = ddr[Environment.DocImageData.DocIDField];
		        if (obj is int)
		            newWindowDocumentButton.Set((int) obj);
		    }
		    else
                newWindowDocumentButton.Set(oldFileName, docString);
            newWindowDocumentButton.Verify();
        }

        #region Accessors
        public bool NeedOpenDoc
        {
            get { return checkBoxOpenDoc.Checked; }
        }

        public int FaxID
        {
            get { return faxID; }
        }

        public int DocID
        {
            get { return docID; }
        }

        public int ImageID
        {
            get { return imageID; }
        }

        public int OldImageID
        {
            get { return oldImageID; }
        }

        public bool AddToWork
        {
            get { return checkAddToWork.Checked; }
        }

        public bool CreateEForm
        {
            get { return canAddEForm; }
        }

		public bool CreateSlaveEForm
		{
			get { return canAddSlaveEForm; }
		}

        public bool SendMessage
        {
            get { return checkSendMessage.Checked; }
        }

        public bool GotoDoc
        {
            get { return checkGotoDoc.Checked; }
        }

        public string DocString
        {
            get { return docString; }
        }

        public bool FaxMode
        {
            get { return faxMode; }
        }

        public bool ScanMode
        {
            get { return scanMode; }
        }

        public string OldFileName
        {
            get { return oldFileName; }
        }

        public int MinPage
        {
            get { return minPage; }
        }

        public int MaxPage
        {
            get { return maxPage; }
        }

        public ServerInfo Server
        {
            get { return server; }
        }

        #endregion

        public enum TypeSave
        {
            None,
            SavePart,
            SaveAll,
            SaveSelected
        }

        private string CheckSequrity()
        {
            PdfReader basePDFReader = null;

            try
            {
                using (var fio = new FileStream(oldFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    string password = String.Empty;
                    try
                    {
                        basePDFReader = new PdfReader(fio);
                        if (!basePDFReader.IsOpenedWithFullPermissions)
                            throw new BadPasswordException("");
                    }
                    catch (BadPasswordException)
                    {
                        while (basePDFReader == null || !basePDFReader.IsOpenedWithFullPermissions)
                        {
                            if (basePDFReader != null)
                                basePDFReader.Close();

                            password = "";
                            if (Document.Controls.PdfViewControl.InputBox.Show(Environment.StringResources.GetString("DocControl_PDF_Encrypted"), Environment.StringResources.GetString("DocControl_PDF_EnterPass"), ref password) == DialogResult.Cancel)
                                return null;
                            try
                            {
                                fio.Position = 0;
                                basePDFReader = new PdfReader(fio, Encoding.UTF8.GetBytes(password));
                            }
                            catch (BadPasswordException)
                            {
                            }
                        }
                    }

                    return password;
                }
            }
            finally
            {
                if (basePDFReader != null)
                    basePDFReader.Close();
            }
        }

        private string SavePdf(string pass, int startPage, int endPage, string dstFileName)
        {
            PdfReader pdfReader = null;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                pdfReader = new PdfReader(oldFileName, Encoding.UTF8.GetBytes(pass));

                pdfReader.RemoveUnusedObjects();
                if (pdfReader.NumberOfPages == 0)
                    throw new Exception("В документе отсутствуют страницы. Операция сохранения невозможна!");

                startPage = (startPage <= 0 || startPage > pdfReader.NumberOfPages) ? 1 : startPage;
                endPage = (endPage <= 0 || endPage > pdfReader.NumberOfPages) ? pdfReader.NumberOfPages : endPage;

                pdfReader.SelectPages(startPage.ToString() + "-" + endPage.ToString());

                using (var file_stream = new FileStream(dstFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var stamper = new PdfStamper(pdfReader, file_stream))
                {
                    stamper.SetFullCompression();

                    for (int numberPage = startPage; numberPage <= endPage; numberPage++)
                    {
                        if (numberPage <= 0 || numberPage > pdfReader.NumberOfPages)
                            continue;
                        PdfDictionary page = pdfReader.GetPageN(numberPage);
                        var resources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
                        var xobject = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));
                        if (xobject != null)
                        {
                            foreach (PdfName pdname in xobject.Keys)
                            {
                                PdfObject obj = xobject.Get(pdname);
                                if (obj.IsIndirect())
                                {
                                    var tg = (PdfDictionary)PdfReader.GetPdfObject(obj); //resolve indirect reference
                                    var subType = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                                    if (PdfName.IMAGE.Equals(subType))
                                    {
                                        int xrefIndex = ((PRIndirectReference)obj).Number;
                                        PdfObject imgPdfObj = pdfReader.GetPdfObject(xrefIndex);
                                        var imgPdfStream = (PdfStream)imgPdfObj;
                                        var imgPRStream = (PRStream)imgPdfStream;
                                        byte[] bytes = PdfReader.GetStreamBytesRaw(imgPRStream);

                                        if (bytes != null && bytes.Length > 0)
                                        {
                                            try
                                            {
                                                var pdfImage = new PdfImageObject(imgPRStream);

                                                Image img = pdfImage.GetDrawingImage();
                                                if (img != null)
                                                {
                                                    var filter = (PdfName)pdfImage.Get(PdfName.FILTER);
                                                    if (filter != null)
                                                        continue;
                                                    ImageFormat format;
													byte[] updatedImgBytes = Document.Controls.PdfViewControl.PDFView.ConvertImageToBytes(img, 75, out format);

                                                    iTextSharp.text.Image compressedImage = iTextSharp.text.Image.GetInstance(updatedImgBytes);
                                                    if (format == ImageFormat.Png)
                                                        compressedImage.Deflated = true;

                                                    PdfReader.KillIndirect(obj);
                                                    stamper.Writer.AddDirectImageSimple(compressedImage, (PRIndirectReference)obj);
                                                }
                                                else
                                                {

                                                    Console.WriteLine("{0}: {1} image is null", DateTime.Now.ToString("HH:mm:ss fff"), xrefIndex);
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.ToString());
                                                continue;
                                            }
                                        }
                                    }
                                    else
                                        Console.WriteLine("{0}: Skipping subtype {1} with filter {2}", DateTime.Now.ToString("HH:mm:ss fff"),  subType, tg.Get(PdfName.FILTER));
                                }
                            }
                        }
                    }
                    stamper.Close();
                    return dstFileName;
                }
            }
            finally
            {
                if (pdfReader != null)
                    pdfReader.Close();

                Cursor.Current = Cursors.Default;
            }
        }

        public string SavePart(int startPage, int endPage)
        {
            var fi = new FileInfo(oldFileName);
            if (fi.LastWriteTimeUtc != oldFI.LastWriteTimeUtc || oldFI.Length != fi.Length)
                return null;

            string tempFileName = Path.GetTempPath() + Environment.GenerateFileName((isPDF) ? "pdf" : "tif");
            if (isPDF)
            {
                string pass = "";

                pass = CheckSequrity();
                if (pass == null)
                    return null;

                tempFileName = SavePdf(pass, startPage, endPage, tempFileName);
            }
            else
            {
                DialogResult dr = DialogResult.Retry;
                if (Environment.LibTiff.QuickColorCheck(oldFileName, startPage - 1, endPage - 1))
                {
                    dr = MessageBox.Show(Environment.StringResources.GetString("Dialog_ColorCompressionDialog_PreMessage"),
                                         Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes ? new ColorCompressionDialog(oldFileName, tempFileName, startPage - 1, endPage - 1).ShowDialog() : DialogResult.Retry;
                }

                if (dr == DialogResult.Cancel)
                {
                    saveAfterColor = false;
                    Slave.DeleteFile(tempFileName);
                    tempFileName = "";
                }
                else if (dr == DialogResult.Retry)
                    if (!Environment.LibTiff.SavePart(oldFileName, startPage - 1, endPage - startPage + 1, tempFileName, null))
                        tempFileName = null;
            }
            return tempFileName;
        }

		public string Save()
		{
			var fi = new FileInfo(oldFileName);
			if(fi.LastWriteTimeUtc != oldFI.LastWriteTimeUtc || oldFI.Length != fi.Length)
				return null;
			string tempFileName = Path.Combine(Path.GetTempPath(), Environment.GenerateFileName(isPDF ? "pdf" : "tif"));

			if(isPDF)
			{
				string pass = "";

				pass = CheckSequrity();
				if(pass == null)
					return null;

				tempFileName = SavePdf(pass, -1, -1, tempFileName);
			}
			else
			{

				DialogResult dr = DialogResult.Retry;
				if(Environment.LibTiff.QuickColorCheck(oldFileName, -1, -1))
				{
					if(MessageBox.Show(Environment.StringResources.GetString("Dialog_ColorCompressionDialog_PreMessage"),
										 Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
						dr = new ColorCompressionDialog(oldFileName, tempFileName).ShowDialog();
					else
						dr = DialogResult.Retry;
				}
				if(dr == DialogResult.Cancel)
				{
					saveAfterColor = false;
					Slave.DeleteFile(tempFileName);
					tempFileName = "";
				}
				else if(dr == DialogResult.Retry)
					if(!Environment.LibTiff.SavePart(oldFileName, -1, -1, tempFileName, null))
					{
						File.Copy(oldFileName, tempFileName,true);
						File.SetAttributes(tempFileName, FileAttributes.Normal);
					}
			}
			return tempFileName;
		}

        public string SaveSelected()
        {
            string tempFileName = Path.GetTempPath() + Environment.GenerateFileName("tif");
            if (images.Count > 0)
                Environment.LibTiff.SaveBitmapToFile(tempFileName, images[0], true);

            DialogResult dr = MessageBox.Show(Environment.StringResources.GetString("Dialog_ColorCompressionDialog_PreMessage"),
                                 Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes ? 
								 new ColorCompressionDialog(tempFileName, tempFileName).ShowDialog()
								 : DialogResult.Retry;
			if(dr == DialogResult.Retry)
				if(!Environment.LibTiff.SavePart(tempFileName, -1, -1, tempFileName, null) && File.Exists(tempFileName))
				{
					File.Copy(tempFileName, tempFileName, true);
					File.SetAttributes(tempFileName, FileAttributes.Normal);
				}
            return tempFileName;
        }

        private void UpdateControls()
        {
            for (int i = 0; i < groupDocument.Controls.Count; i++)
                groupDocument.Controls[i].Enabled = baseEdit;

            checkBoxName.Enabled = docTypeBlock.Enabled && docTypeBlock.ID > 0 && Environment.DoesDocTypeNameExist(docTypeBlock.ID);
            textBox1.Enabled = checkBoxName.Enabled && checkBoxName.Checked;

            buttonChange.Visible = !baseEdit;
            buttonChange.Enabled = !baseEdit;
            buttonChange.Left = docTypeBlock.Left + docTypeBlock.buttonSelect.Left;
            buttonChange.Size = docTypeBlock.buttonSelect.Size;

            groupImage.Enabled = !baseEdit;
            groupImage.Visible = !baseEdit;

            groupDescription.Enabled = !baseEdit;
            groupDescription.Visible = !baseEdit;

            groupLinks.Enabled = !baseEdit;
            groupLinks.Visible = !baseEdit;

            checkAddToWork.Enabled = !baseEdit;
            checkAddToWork.Visible = !baseEdit;

            checkSendMessage.Enabled = !baseEdit && !messageNeeded;
            checkSendMessage.Visible = checkSendMessage.Enabled;

            if (!checkAddToWork.Checked)
            {
                checkSendMessage.Enabled = false;
                checkSendMessage.Checked = true;
            }

            checkTemplate.Enabled = !baseEdit;
            checkTemplate.Visible = !baseEdit;

            checkGotoDoc.Enabled = !baseEdit;
            checkGotoDoc.Visible = !baseEdit;

            archiveLabel.Enabled = original.Checked;
            archive.Enabled = original.Checked;
            buttonSelectArchive.Enabled = original.Checked;

            if (baseEdit)
            {
                maxWidth = Width;
                maxHeight = Height;
                Width = groupDocument.Width + 10;
                Height = groupPersons.Height + groupPersons.Location.Y + buttonOK.Height + 48;
            }
            else
            {
                Width = maxWidth;
                Height = maxHeight;
            }

            checkMain.Enabled = (docID > 0);
            checkMain.Visible = !baseEdit;

            checkBoxProtected.Enabled = !baseEdit;
            checkBoxProtected.Visible = !baseEdit;
        }

        private void DocProperties_Load(object sender, EventArgs e)
        {
            groupPersons.Text = Environment.PersonWord.GetForm(Cases.I, true, true);
            UpdateControls();
        }

        private void original_CheckedChanged(object sender, EventArgs e)
        {
            if (original.Checked && (archiveID == 0))
                SelectArchiveDialog();

            buttonSelectArchive.Enabled = original.Checked;
            archiveLabel.Enabled = original.Checked;
            archive.Enabled = original.Checked;
        }

        private void SelectArchiveDialog()
        {
            var dialog = new SelectArchiveDialog(archiveID, false);
            dialog.DialogEvent += SelectArchiveDialog_DialogEvent;
            ShowSubForm(dialog);
            Enabled = false;
        }

        private void SelectArchiveDialog_DialogEvent(object source, DialogEventArgs e)
        {
            Enabled = true;
            Focus();

            var dialog = e.Dialog as SelectArchiveDialog;
            if (dialog != null && dialog.DialogResult == DialogResult.OK)
            {
                DTreeNode node = dialog.ArchiveNode;
                if (node != null)
                {
                    archiveID = node.ID;
                    archive.Text = node.Text;
                }
            }

            if (original.Checked && (archiveID == 0))
                original.Checked = false;
        }

        private void buttonSelectArchive_Click(object sender, EventArgs e)
        {
            SelectArchiveDialog();
        }

		private void buttonOK_Click(object sender, EventArgs e)
		{
			buttonOK.Enabled = false;
			string fileName = "";

			try
			{
				if(baseEdit)
				{
					typeID = docTypeBlock.ID;

					canAddEForm = false;

					if(typeID == 0)
					{
						docTypeBlock.SelectDialog();
						return;
					}
					else
					{
						DataRow dr = Environment.GetDocTypeProperties(typeID);
						if(dr != null)
						{
							canAddEForm = dr[Environment.DocTypeData.FormPresentField] is byte && dr[Environment.DocTypeData.FormPresentField].Equals((byte)2);

							if(dr[Environment.DocTypeData.ProtectedField] is byte)
							{
								protect = dr[Environment.DocTypeData.ProtectedField].Equals((byte)1);
								checkBoxProtected.Checked = protect;
							}
						}
					}

					if((!checkBoxName.Checked || textBox1.Text.Trim().Length == 0) && Environment.DoesDocTypeNameExist(typeID))
					{
                        var dialog = new CheckTypeDialog();
                        dialog.SetStartPosition(this, 0, 60);
                        if (dialog.ShowDialog() != DialogResult.Yes)
						{
							checkBoxName.Checked = true;
							textBox1.Focus();
							return;
						}
						else
							textBox1.Text = "";
					}

					if(!checkBoxNoNumber.Checked)
					{
						numberStr = number.Text.TrimStart().TrimEnd();

						if(numberStr == "")
						{
							if(MessageBox.Show(Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning1"), Environment.StringResources.GetString("InputError"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
							{
								number.Focus();
								return;
							}
							else
							{
								checkBoxNoNumber.Checked = true;
								numberStr = "";
								number.Text = "";
							}
						}
						else if(!Regex.IsMatch(numberStr, @"\d+"))
						{
							switch(new NumberConfirmDialog(numberStr).ShowDialog())
							{
								case DialogResult.No:
									number.Text = "";
									numberStr = "";
									checkBoxNoNumber.Checked = true;
									break;
								case DialogResult.Yes:
									break;
								default:
									number.Focus();
									return;
							}
						}
					}
					else
						numberStr = "";

					if(dateBlock.IsEmpty())
					{
						if(MessageBox.Show(Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning3"), Environment.StringResources.GetString("InputError"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
						{
							dateBlock.Focus();
							return;
						}
						else
							docLinksBlock.DocDate = DateTime.MinValue;
					}
					else if(!dateBlock.Check())
					{
						MessageBox.Show(dateBlock.Error + System.Environment.NewLine + Environment.StringResources.GetString("RepeatInput"), Environment.StringResources.GetString("InputError"));
						dateBlock.Focus();
						return;
					}
                    else if(dateBlock.Value > DateTime.Now.Date)
                    {
                        var dialog = new CheckDateDialog();
                        dialog.SetStartPosition(this, 0, 60);
                        if (dialog.ShowDialog() != DialogResult.Yes)
                        {
                            dateBlock.Focus();
                            return;
                        }
                    }
					else
					{
						docDate = dateBlock.Value;
						docLinksBlock.DocDate = docDate;
					}

					string personIDs = "";
					for(int i = 0; i < personBlock.PersonIDs.Length; i++)
						personIDs += (((personIDs.Length > 0 && personBlock.PersonIDs[i] > 0) ? "," : "") + ((personBlock.PersonIDs[i] > 0) ? personBlock.PersonIDs[i].ToString() : ""));

					// Проверка соответствия типов документа контрагентам (резидент/нерезидент -> счет/инвойс)
					if(!string.IsNullOrEmpty(personIDs))
					{
						bool ret1 = typeID == DocTypeDALC.TYPE_ID_SCHET || typeID == DocTypeDALC.TYPE_ID_SCHET_FACTURA;
						bool ret2 = typeID == DocTypeDALC.TYPE_ID_INVOICE || typeID == DocTypeDALC.TYPE_ID_INVOICE_PROFORMA;
						int ret = 0;
						if(ret1 && (ret = Lib.Win.Document.Environment.PersonData.CheckPersonsCountry(personIDs, ret1)) != Lib.Win.Data.DALC.Directory.PersonDALC.AREA_CODE_RUSSIAN_FEDERATION && ret != 0)
						{
							// для типов счёт/счёт-фактура только резиденты
							var checkCountryDialog = new CheckTypeDialogCountry();
							checkCountryDialog.Controls["lblConfirmationDescription"].Text = String.Format("{0}", Lib.Win.Document.Environment.StringResources.GetString("CheckTypeDialogCountry_OnlyRussianCompany"));
							checkCountryDialog.Controls["lblConfirmationQuestion"].Text += String.Format(" {0}", Lib.Win.Document.Environment.DocTypeData.GetDocType(typeID, Lib.Win.Document.Environment.CurCultureInfo.TwoLetterISOLanguageName));
							checkCountryDialog.ShowDialog();
							if(checkCountryDialog.DialogResult == DialogResult.No)
								return;
						}
						else if(ret2 && Lib.Win.Document.Environment.PersonData.CheckPersonsCountry(personIDs, ret2) == 0)
						{
							// для типов инвойс/инвойс-проформа одна из компаний должна быть нерезидентом
							var checkCountryDialog = new CheckTypeDialogCountry();
							checkCountryDialog.Controls["lblConfirmationDescription"].Text = String.Format("{0}", Lib.Win.Document.Environment.StringResources.GetString("CheckTypeDialogCountry_MustHaveNotResident"));
							checkCountryDialog.Controls["lblConfirmationQuestion"].Text += String.Format(" {0}", Lib.Win.Document.Environment.DocTypeData.GetDocType(typeID, Lib.Win.Document.Environment.CurCultureInfo.TwoLetterISOLanguageName));
							checkCountryDialog.ShowDialog();
							if(checkCountryDialog.DialogResult == DialogResult.No)
								return;
						}
					}

				    byte type = docTypeBlock.SearchType;
					// проверяем, есть ли похожий документ уже
					using(DataTable dt = Environment.DocData.GetSimilarDocs(typeID, numberStr, docDate, personIDs, 0, -1))
					{
						if(dt.Rows.Count > 0)
						{
							SelectDocDialog dialog = new SelectDocDialog(dt, typeID, docTypeBlock.Text, numberStr, docDate, personIDs, docID, description.Text);
							dialog.SearchType = type;
							dialog.DialogEvent += SelectDocDialog_DialogEvent;
							ShowSubForm(dialog);
							Enabled = false;
						}
						else
						{
							if(imageID > 0)
								docID = 0;
							EndBaseEdit();
						}
						dt.Dispose();
					}
				}
				else	// not baseEdit
				{
					DialogResult currentResult = DialogResult.OK;
					if(imageID == 0)
						switch(typeSaveImage)
						{
							case TypeSave.SaveAll:
								fileName = Save();
								break;
							case TypeSave.SavePart:
								fileName = SavePart(minPage, maxPage);
								break;
							case TypeSave.SaveSelected:
								fileName = SaveSelected();
								break;
						}

					string docName = "";
					if(checkBoxName.Checked && textBox1.Text.Length > 0)
						docName = textBox1.Text.Trim();

					if(!saveAfterColor)
					{
						saveAfterColor = true;
						End(DialogResult.Cancel);
						return;
					}
					if(string.IsNullOrEmpty(fileName) && imageID == 0)
					{
						MessageBox.Show(Environment.StringResources.GetString("File") + " " + Environment.StringResources.GetString("NotFound") + System.Environment.NewLine +
							Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Error1") + System.Environment.NewLine + Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Error6"),
							Environment.StringResources.GetString("Error"));
						DialogResult = DialogResult.Cancel;
						Close();
						return;
					}
					int personCount = personBlock.Count;

					if(personCount == 0)
					{
						MessageBox.Show(Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning4") + " " +
							Environment.PersonWord.GetForm(Cases.R, false, false), Environment.StringResources.GetString("InputError"));
						return;
					}

					if(!Environment.PersonData.CheckPersons(personBlock.PersonIDs))
					{
						if(MessageBox.Show(
							Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning5") + " " + Environment.PersonWord.GetForm(Cases.R, true, false) + " " + Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning6") + System.Environment.NewLine +
							Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning7") + " " + Environment.PersonWord.GetForm(Cases.R, true, false) + " " + Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning8") + System.Environment.NewLine +
							Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning9"), Environment.PersonWord.GetForm(Cases.I, true, true),
							MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
							return;
					}

                    string personIDs = "";
                    for (int i = 0; i < personBlock.PersonIDs.Length; i++)
                        personIDs += (((personIDs.Length > 0 && personBlock.PersonIDs[i] > 0) ? "," : "") + ((personBlock.PersonIDs[i] > 0) ? personBlock.PersonIDs[i].ToString() : ""));
                 
					//Проверка соответствия типов документа контрагентам (резидент/нерезидент -> счет/инвойс)
					if(!string.IsNullOrEmpty(personIDs))
					{
						if(!string.IsNullOrEmpty(personIDs))
						{
							bool ret1 = typeID == DocTypeDALC.TYPE_ID_SCHET || typeID == DocTypeDALC.TYPE_ID_SCHET_FACTURA;
							bool ret2 = typeID == DocTypeDALC.TYPE_ID_INVOICE || typeID == DocTypeDALC.TYPE_ID_INVOICE_PROFORMA;
							int ret = 0;
							if(ret1 && (ret = Lib.Win.Document.Environment.PersonData.CheckPersonsCountry(personIDs, ret1)) != Lib.Win.Data.DALC.Directory.PersonDALC.AREA_CODE_RUSSIAN_FEDERATION && ret != 0)
							{
								// для типов счёт/счёт-фактура только резиденты
								var checkCountryDialog = new CheckTypeDialogCountry();
								checkCountryDialog.Controls["lblConfirmationDescription"].Text = String.Format("{0}", Lib.Win.Document.Environment.StringResources.GetString("CheckTypeDialogCountry_OnlyRussianCompany"));
								checkCountryDialog.Controls["lblConfirmationQuestion"].Text += String.Format(" {0}", Lib.Win.Document.Environment.DocTypeData.GetDocType(typeID, Lib.Win.Document.Environment.CurCultureInfo.TwoLetterISOLanguageName));
								checkCountryDialog.ShowDialog();
								if(checkCountryDialog.DialogResult == DialogResult.No)
									return;
							}
							else if(ret2 && Lib.Win.Document.Environment.PersonData.CheckPersonsCountry(personIDs, ret2) == 0)
							{
								// для типов инвойс/инвойс-проформа одна из компаний должна быть нерезидентом
								var checkCountryDialog = new CheckTypeDialogCountry();
								checkCountryDialog.Controls["lblConfirmationDescription"].Text = String.Format("{0}", Lib.Win.Document.Environment.StringResources.GetString("CheckTypeDialogCountry_MustHaveNotResident"));
								checkCountryDialog.Controls["lblConfirmationQuestion"].Text += String.Format(" {0}", Lib.Win.Document.Environment.DocTypeData.GetDocType(typeID, Lib.Win.Document.Environment.CurCultureInfo.TwoLetterISOLanguageName));
								checkCountryDialog.ShowDialog();
								if(checkCountryDialog.DialogResult == DialogResult.No)
									return;
							}
						}
					}
				

					// add code
					if(personCount == 1)
					{
						if(MessageBox.Show(
							Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning10") + " " + Environment.PersonWord.GetForm(Cases.V, false, false) + "." + System.Environment.NewLine +
							Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning11") + " " + Environment.PersonWord.GetForm(Cases.R, true, false) + "." + System.Environment.NewLine + System.Environment.NewLine +
							Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Warning12") + " " + Environment.PersonWord.GetForm(Cases.T, false, false) + "?",
							Environment.PersonWord.GetForm(Cases.I, true, true), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
							return;
					}

					if(!string.IsNullOrEmpty(fileName) && imageID == 0)
						if(!File.Exists(fileName))
						{
							MessageBox.Show(Environment.StringResources.GetString("File") + " " + fileName + " " + Environment.StringResources.GetString("NotFound") + System.Environment.NewLine +
								Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Error1"), Environment.StringResources.GetString("Error"));
							Data.Env.WriteToLog(Environment.StringResources.GetString("File") + " " + fileName + " " + Environment.StringResources.GetString("NotFound") + System.Environment.NewLine + Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Error1"));
							currentResult = DialogResult.Retry;
							return;
						}

					// saving image
					bool faxSave = false;
					if(faxID > 0 && faxMode)
						faxSave = Environment.FaxData.FaxHasDocImage(faxID);
					if(!faxSave)
					{
						canAddSlaveEForm = docID == 0;
						bool retValue = false;
						if(imageID == 0)
						{
							if(oldImageID > 0)
								if(Environment.DocSignatureData.IsImageSigned(oldImageID))
								{
									MessageBox.Show(StringResources.Dialog_AddDBDocDialog_buttonOK_Click_Error1 + System.Environment.NewLine +
										StringResources.Dialog_AddDBDocDialog_buttonOK_Click_Error7, StringResources.Warning);
									DialogResult = DialogResult.Cancel;
									Close();
									Owner = null;
								}
							string extension = (isPDF) ? "pdf" : "tif";
							string name = Environment.GenerateFileName(extension);
							string newFileName = server.Path + "\\Temp\\" + name;

							while(File.Exists(newFileName))
							{
								name = Environment.GenerateFileName(extension);
								newFileName = server.Path + "\\Temp\\" + name;
							}
							File.Copy(fileName, newFileName);
							while(!File.Exists(newFileName))
								throw new Exception(Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Error2") + " " + name);
                            Console.WriteLine("{0}: File to save: {1}", DateTime.Now.ToString("HH:mm:ss fff"), newFileName);
							retValue = !isPDF ? Environment.DocImageData.DocImageInsert(server.ID, name, ref imageID, ref docID, typeID, docName, docDate, numberStr, description.Text.TrimStart().TrimEnd(), protect, createDate, archiveID, checkMain.Checked, pagesCount) : Environment.DocImageData.DocImageInsert(server.ID, name, ref imageID, ref docID, typeID, docName, docDate, numberStr, description.Text.TrimStart().TrimEnd(), protect, createDate, archiveID, checkMain.Checked, 0, "PDF", pagesCount);
						}
						else
						{
							retValue = !isPDF ? Environment.DocImageData.DocImageInsert(ref imageID, ref docID, typeID, docName, docDate, numberStr, description.Text.TrimStart().TrimEnd(), protect, createDate, archiveID, checkMain.Checked, pagesCount) :
                                         Environment.DocImageData.DocImageInsert(0, "", ref imageID, ref docID, typeID, docName, docDate, numberStr, description.Text.TrimStart().TrimEnd(), protect, createDate, archiveID, checkMain.Checked, 0, "PDF", pagesCount);
						}

						if(!retValue || imageID == 0 || docID == 0)
						{
							MessageBox.Show(Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Error3") + System.Environment.NewLine +
								Environment.StringResources.GetString("DocumentID") + ": " + docID + System.Environment.NewLine +
								Environment.StringResources.GetString("ImageID") + ": " + imageID, Environment.StringResources.GetString("Error"));
							return;
						}
						else
						{
							Slave.DeleteFile(fileName);
							fileName = string.Empty;
						}

						if(canAddEForm)
							canAddEForm = !Environment.DocDataData.IsDataPresent(docID);

						// saving persons
						if(personCount > 0 && !Environment.DocData.SetDocPersons(docID, personBlock.PersonIDs))
						{
							MessageBox.Show(Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Error4") + " " +
								Environment.PersonWord.GetForm(Cases.V, true, false) + ".",
								Environment.StringResources.GetString("Error"));
							return;
						}
					}
					else
					{
						MessageBox.Show(docString + " " + Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Error5"), Environment.StringResources.GetString("Error"));
						return;
					}

					// добавляем связи к документу

					for(int j = 0; j < docLinksBlock.Count; j++)
						if(docLinksBlock[j].SubItems[1].Text.Equals("True"))
						{
							if(ParentDocIDs == null)
								ParentDocIDs = new SynchronizedCollection<int>();
							ParentDocIDs.Add(docLinksBlock[j].ID);
						}
						else
						{
							if(ChildDocIDs == null)
								ChildDocIDs = new SynchronizedCollection<int>();
							ChildDocIDs.Add(docLinksBlock[j].ID);
						}
					if(ParentDocIDs.Count > 0 && canAddSlaveEForm)
					{
						List<int> dIDs = Environment.DocTypeData.GetContarcts(typeID);
						canAddSlaveEForm = dIDs.Count > 0 && ParentDocIDs.FirstOrDefault(x => dIDs.Contains(x)) > 0;
					}
					else
						canAddSlaveEForm = false;
					// setting options
					if(checkTemplate.Checked)
					{
						TemplateClass.DocTypeID = typeID;
						TemplateClass.DocDateTime = docDate;

						int count = personBlock.Count;
						if(count > 0)
						{
							TemplateClass.DocPersonsIDs = new int[count];
							for(int i = 0; i < count; i++)
								TemplateClass.DocPersonsIDs[i] = personBlock[i].ID;
						}
					}
					else
					{
						TemplateClass.ClearAll();
					}

					// сохраняем настройки
					Environment.SettingsData.SetSaveSettings(new UserSaveSettings
											   {
												   AddToWork = checkAddToWork.Checked,
												   OpenSaved = checkGotoDoc.Checked,
												   SendMessage = checkSendMessage.Checked,
												   SendMessageNeeded = checkSendMessage.Enabled
											   });

					// слава тебе, Господи
					DialogResult = currentResult;
					Close();
					Owner = null;
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, "server: " + (server == null ? "пусто" : server.Name) + " server paht: " + (server == null ? "пусто" : server.Path) + " filename: " + fileName);
				ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
			}
			finally
			{
				if(!IsDisposed)
					buttonOK.Enabled = true;
			}
		}

        private void EndBaseEdit()
        {
            number.Text = number.Text.TrimStart().TrimEnd();
            baseEdit = false;
            UpdateControls();
        }

        private void SelectDocDialog_DialogEvent(object source, DialogEventArgs e)
        {
            Enabled = true;
            Focus();
            docID = 0;
            var dialog = e.Dialog as SelectDocDialog;
            if (dialog != null)
            {
                if (dialog.DialogResult == DialogResult.OK)
                {
                    DocListItem docItem = dialog.SelectedItem;
                    if (docItem != null)
                    {
                        docID = docItem.ID; // добавление нового изображения
                        AddImage();
                    }
                    EndBaseEdit();
                }
                else if (dialog.DialogResult == DialogResult.Yes)
                {
                    ListItem docItem = dialog.SelectedItem;
                    if (docItem != null)
                    {
                        docID = docItem.ID; // добавление нового изображения
                        var cdialog = new SelectImageDialog
                                          {DocumentID = docID, PagesCount = pagesCount, OldImageID = imageID};
                        cdialog.DialogEvent += cdialog_DialogEvent;
                        ShowSubForm(cdialog);
                    }
                }
            }
        }

        private void HandleDateBlockEvent(object sender, DateBlockEventArgs d)
        {
            personBlock.HandleDateBlockEvent(sender, d);
        }

        private void LoadPropertiesPersons()
        {
            using (DataTable dt = Environment.DocData.GetDocPersons(docID, dateBlock.Value))
            using (DataTableReader dr = dt.CreateDataReader())
            {
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        var personID = (int)dr[Environment.PersonData.IDField];
                        string person = Environment.PersonData.GetPerson(personID);
                        bool isValid = 0 < (int)dr[Environment.DocData.PersonIsValidField];

                        int position = 0;
                        if (dr[Environment.DocData.PersonPositionField] is byte)
                            position = (byte)dr[Environment.DocData.PersonPositionField];
                        if (person != null)
                            personBlock.AddPerson(personID, person, position, isValid);
                    }
                }
                dr.Close(); dr.Dispose(); dt.Dispose();
            }
        }

        private void AddImage()
        {
            if (docID != 0)
            {
                DataRow row = Environment.DocData.GetDocProperties(docID, Environment.CurCultureInfo.TwoLetterISOLanguageName);

                // description
                object obj = row[Environment.DocData.DescriptionField];
                description.Text = !obj.Equals(DBNull.Value) ? obj.ToString() : "";

                obj = row[Environment.DocData.ProtectedField];
                if (obj is byte)
                    protect = obj.Equals((byte)1);
                checkBoxProtected.Checked = protect;
                //type
                obj = row[Environment.DocData.DocTypeIDField];
                if (!obj.Equals(DBNull.Value))
                {
                    typeID = (int)obj;
                    string docType = Environment.DocTypeData.GetDocType(typeID, Environment.CurCultureInfo.TwoLetterISOLanguageName);
                    if (docType != null)
                        docTypeBlock.Text = docType;
                }
                else
                    docTypeBlock.Text = "";

                // persons
                personBlock.Clear();
                LoadPropertiesPersons();

                //links
                docLinksBlock.Added -= docLinksBlock_Added;
                docLinksBlock.LoadDocLinks(docID);
                docLinksBlock.Added += docLinksBlock_Added;
            }
        }

        void cdialog_DialogEvent(object source, DialogEventArgs e)
        {
            if (e.Dialog.DialogResult != DialogResult.OK)
                return;
            var cd = e.Dialog as SelectImageDialog;
            if (cd == null)
                return;
            addImageID = cd.ImageID;
            int page = cd.Page - 1;
            int pagesCount = cd.PagesCount;
            int documentID = cd.DocumentID;

            if (addImageID > 0)
            {
                bool? res = AddAndSave(imageID, addImageID, images, oldFileName, page, pagesCount, documentID, minPage,
                                       maxPage, server, typeSaveImage);
                if (res.HasValue)
                {
                    imageID = addImageID;
                    End(DialogResult.Yes);
                }
            }
            else
            {
                docID = documentID;
                AddImage();
                EndBaseEdit();
            }
        }

        public static bool? AddAndSave(int imageID, int addImageID, string oldFileName, int page, int pagesCount, int documentID, int minPage, int maxPage, ServerInfo server, TypeSave typeSaveImage)
        {
            return AddAndSave(imageID, addImageID, null, oldFileName, page, pagesCount, documentID, minPage, maxPage, server, typeSaveImage);
        }

		public static bool? AddAndSave(int imageID, int addImageID, List<Bitmap> images, string oldFileName, int page, int pagesCount, int documentID, int minPage, int maxPage, ServerInfo server, TypeSave typeSaveImage)
		{
			bool isPDF = false;
			string addFileName = "";
			if(imageID > 0)
			{
				isPDF = "pdf".Equals(Environment.DocImageData.GetField(Environment.DocImageData.ImageTypeField, imageID).ToString(), StringComparison.CurrentCultureIgnoreCase);
				addFileName = GetFile(imageID, isPDF ? ".pdf" : ".tif");
				if(string.IsNullOrEmpty(addFileName))
					return null;
			}
			else
			{
				if(typeSaveImage == TypeSave.SaveSelected)
					isPDF = false;
				else
					if(typeSaveImage == TypeSave.SaveAll || typeSaveImage == TypeSave.SavePart)
					{
						addFileName = oldFileName;
						isPDF = Environment.IsPdf(addFileName);
					}
			}

			if(isPDF)
			{
				if("pdf".Equals(Environment.DocImageData.GetField(Environment.DocImageData.ImageTypeField, addImageID).ToString(), StringComparison.CurrentCultureIgnoreCase))
				{
					string tmpFileName = Path.GetTempFileName();
					PdfReader basePDFReader = null;
					PdfReader addPdfReader = null;
					try
					{
						string filename = GetFile(addImageID, ".pdf");
						if(string.IsNullOrEmpty(filename))
							return null;
						using(var fio = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
						{
							string password = String.Empty;
							try
							{
								basePDFReader = new PdfReader(fio);
								if(!basePDFReader.IsOpenedWithFullPermissions)
									throw new BadPasswordException("");
							}
							catch(BadPasswordException)
							{
								while(!basePDFReader.IsOpenedWithFullPermissions)
								{
									basePDFReader.Close();

									if(Document.Controls.PdfViewControl.InputBox.Show(Environment.StringResources.GetString("DocControl_PDF_Encrypted"), Environment.StringResources.GetString("DocControl_PDF_EnterPass"), ref password) == DialogResult.Cancel)
										return null;
									try
									{
										fio.Position = 0;
										basePDFReader = new PdfReader(fio, Encoding.ASCII.GetBytes(password));
									}
									catch(BadPasswordException)
									{
									}
								}
							}

							if(basePDFReader.NumberOfPages == 0)
								throw new Exception("В документе отсутствуют страницы. Операция сохранения невозможна!");
							if(-2 == page)
								page = basePDFReader.NumberOfPages - 1;
							using(var fs = new FileStream(addFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
							{
								password = String.Empty;
								try
								{
									addPdfReader = new PdfReader(fs);
									if(!addPdfReader.IsOpenedWithFullPermissions)
										throw new BadPasswordException("");
								}
								catch(BadPasswordException)
								{
									while(!addPdfReader.IsOpenedWithFullPermissions)
									{
										addPdfReader.Close();
										if(Document.Controls.PdfViewControl.InputBox.Show(Environment.StringResources.GetString("DocControl_PDF_Encrypted"), Environment.StringResources.GetString("DocControl_PDF_EnterPass"), ref password) == DialogResult.Cancel)
											return null;
										try
										{
											fs.Position = 0;
											addPdfReader = new PdfReader(fs, Encoding.ASCII.GetBytes(password));
										}
										catch(BadPasswordException)
										{
										}
									}
								}

								if(basePDFReader.NumberOfPages == 0)
									throw new Exception("В добавляемом документе отсутсвуют страницы. Операция сохранения невозможна!");

								using(var file_stream = new FileStream(tmpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
								using(var document = new iTextSharp.text.Document(basePDFReader.GetPageSize(1)))
								using(var pdfWriter = new PdfCopy(document, file_stream))
								{
									pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;
									pdfWriter.SetFullCompression();
									pdfWriter.RotateContents = true;

									document.Open();
									if(page == -1)
									{
										for(int j = 0; j < addPdfReader.NumberOfPages; j++)
										{
											if(typeSaveImage == TypeSave.SavePart && (j < minPage || j > maxPage))
												continue;
											PdfImportedPage pp = pdfWriter.GetImportedPage(addPdfReader, j + 1);
											pdfWriter.AddPage(pp);
										}
									}
									for(int i = 0; i < basePDFReader.NumberOfPages; i++)
									{
										PdfImportedPage pp = pdfWriter.GetImportedPage(basePDFReader, i + 1);
										pdfWriter.AddPage(pp);

										if(i == page)
										{
											for(int j = 1; j <= addPdfReader.NumberOfPages; j++)
											{
												if(typeSaveImage == TypeSave.SavePart && (j < minPage || j > maxPage))
													continue;
												pp = pdfWriter.GetImportedPage(addPdfReader, j);
												pdfWriter.AddPage(pp);
											}
										}
									}
									document.Close();
								}
							}
						}
					}
					finally
					{
						if(basePDFReader != null)
							basePDFReader.Close();
					}

					basePDFReader = new PdfReader(tmpFileName);
					using(var file_stream = new FileStream(tmpFileName + ".tmp", FileMode.Create, FileAccess.Write, FileShare.None))
					using(var stamper = new PdfStamper(basePDFReader, file_stream))
					{
						stamper.SetFullCompression();

						for(int numberPage = 1; numberPage <= basePDFReader.NumberOfPages; numberPage++)
						{
							PdfDictionary overpage = basePDFReader.GetPageN(numberPage);
							var resources = (PdfDictionary)PdfReader.GetPdfObject(overpage.Get(PdfName.RESOURCES));
							var xobject = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));
							if(xobject != null)
							{
								foreach(PdfName pdname in xobject.Keys)
								{
									PdfObject obj = xobject.Get(pdname);
									if(obj.IsIndirect())
									{
										var tg = (PdfDictionary)PdfReader.GetPdfObject(obj); //resolve indirect reference
										var subType = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
										if(PdfName.IMAGE.Equals(subType))
										{
											int xrefIndex = ((PRIndirectReference)obj).Number;
											PdfObject imgPdfObj = basePDFReader.GetPdfObject(xrefIndex);
											var imgPdfStream = (PdfStream)imgPdfObj;
											var imgPRStream = (PRStream)imgPdfStream;
											byte[] bytes = PdfReader.GetStreamBytesRaw(imgPRStream);

											if(bytes != null && bytes.Length > 0)
											{
												try
												{
													var pdfImage = new PdfImageObject(imgPRStream);

													Image img = pdfImage.GetDrawingImage();
													if(img != null)
													{
														var filter = (PdfName)pdfImage.Get(PdfName.FILTER);
														if(filter != null)
															continue;
														ImageFormat format;
														byte[] updatedImgBytes = Document.Controls.PdfViewControl.PDFView.ConvertImageToBytes(img, 75, out format);

														iTextSharp.text.Image compressedImage = iTextSharp.text.Image.GetInstance(updatedImgBytes);
														if(format == ImageFormat.Png)
															compressedImage.Deflated = true;

														PdfReader.KillIndirect(obj);
														stamper.Writer.AddDirectImageSimple(compressedImage, (PRIndirectReference)obj);
													}
													else
													{

                                                        Console.WriteLine("{0}: {1} image is null", DateTime.Now.ToString("HH:mm:ss fff"), xrefIndex);
													}
												}
												catch(Exception e)
												{
													Console.WriteLine(e.ToString());
													continue;
												}
											}
										}
										else
											Console.WriteLine("{0}: Skipping subtype {1} with filter {2}", DateTime.Now.ToString("HH:mm:ss fff"), subType, tg.Get(PdfName.FILTER));
									}
								}
							}
						}
						stamper.Close();
						basePDFReader.Close();
					}

					if(File.Exists(tmpFileName))
					{
						string filename = GetFile(addImageID, ".pdf");
						// Сохранение копии файла для Undo
						string tmpFileNameUndo = Path.GetTempFileName();
						string tmpFileNameRedo = Path.GetTempFileName();
						File.Copy(filename, tmpFileNameUndo, true);
						File.Copy(tmpFileName, tmpFileNameRedo, true);

						bool isSaveAfterAdd = SaveAfterAdd(documentID, addImageID, imageID, server, tmpFileName, "PDF");

						//if (isSaveAfterAdd)
						//    UndoRedoStackAddImageToDoc(documentID, addImageID, imageID, server, tmpFileName, "PDF", tmpFileNameUndo, tmpFileNameRedo);

						return isSaveAfterAdd;
					}
				}
				else
				{
					string filename = GetFile(addImageID, ".tif");
					if(string.IsNullOrEmpty(filename))
						return null;
					string tmpFileName = Path.GetTempFileName();
					File.Copy(filename, tmpFileName, true);
					ConvertAndInsertClass.AddPagesToTiff(tmpFileName,
														 typeSaveImage == TypeSave.SavePart
															 ? ConvertAndInsertClass.GetPDFImages(oldFileName, minPage,
																								  maxPage)
															 : ConvertAndInsertClass.GetPDFImages(oldFileName, 0, 0),
														 page, 0, 0);

					// Сохранение копии файла для Undo
					string tmpFileNameUndo = Path.GetTempFileName();
					string tmpFileNameRedo = Path.GetTempFileName();
					File.Copy(filename, tmpFileNameUndo, true);
					File.Copy(tmpFileName, tmpFileNameRedo, true);

					bool isSaveAfterAdd = SaveAfterAdd(documentID, addImageID, imageID, server, tmpFileName, "TIF");

					//if (isSaveAfterAdd)
					//    UndoRedoStackAddImageToDoc(documentID, addImageID, imageID, server, tmpFileName, "TIF", tmpFileNameUndo, tmpFileNameRedo);

					return isSaveAfterAdd;
				}
			}
			else
			{
				if("pdf".Equals(Environment.DocImageData.GetField(Environment.DocImageData.ImageTypeField, addImageID).ToString(), StringComparison.CurrentCultureIgnoreCase))
				{
					string filename = GetFile(addImageID, ".pdf");
					if(string.IsNullOrEmpty(filename))
						return null;
					string tmpFileName = Path.GetTempFileName();
					File.Copy(filename, tmpFileName, true);
					var L = new List<Tiff.PageInfo>();
					switch(typeSaveImage)
					{
						case TypeSave.SaveSelected:
							L.AddRange(images.Select(t => new Tiff.PageInfo { Image = t, Compression = new CompressionInfo() }));
							break;
						case TypeSave.SavePart:
							L = Environment.LibTiff.GetPagesCollectionFromFile(addFileName, minPage - 1, maxPage - minPage + 1, true);
							break;
						default:
							L = Environment.LibTiff.GetPagesCollectionFromFile(addFileName, 0, 0, true);
							break;
					}

					// Сохранение копии файла для Undo
					string tmpFileNameUndo = Path.GetTempFileName();
					string tmpFileNameRedo = Path.GetTempFileName();
					File.Copy(tmpFileName, tmpFileNameUndo, true);

					ConvertAndInsertClass.AddPagesToPDF(tmpFileName, L, page, 0, 0);

					File.Copy(tmpFileName, tmpFileNameRedo, true);

					bool isSaveAfterAdd = SaveAfterAdd(documentID, addImageID, imageID, server, tmpFileName, "PDF");

					//if (isSaveAfterAdd)
					//    UndoRedoStackAddImageToDoc(documentID, addImageID, imageID, server, tmpFileName, "PDF", tmpFileNameUndo, tmpFileNameRedo);

					return isSaveAfterAdd;
				}
				else
				{
					string filename = GetFile(addImageID, ".tif");
					if(string.IsNullOrEmpty(filename))
						return false;
					string tmpFileName = Path.GetTempFileName();
					File.Copy(filename, tmpFileName, true);
					var L = new List<Tiff.PageInfo>();
					switch(typeSaveImage)
					{
						case TypeSave.SaveSelected:
							L.AddRange(images.Select(t => new Tiff.PageInfo { Image = t }));
							break;
						case TypeSave.SavePart:
							L = Environment.LibTiff.GetPagesCollectionFromFile(addFileName, minPage - 1, maxPage - minPage + 1, true);
							break;
						default:
							L = Environment.LibTiff.GetPagesCollectionFromFile(addFileName, 0, 0, true);
							break;
					}

					// Сохранение копии файла для Undo
					string tmpFileNameUndo = Path.GetTempFileName();
					string tmpFileNameRedo = Path.GetTempFileName();
					File.Copy(tmpFileName, tmpFileNameUndo, true);

					ConvertAndInsertClass.AddPagesToTiff(tmpFileName, L, page, 0, 0);

					File.Copy(tmpFileName, tmpFileNameRedo, true);

					bool isSaveAfterAdd = SaveAfterAdd(documentID, addImageID, imageID, server, tmpFileName, "TIF");

					//if (isSaveAfterAdd)
					//    UndoRedoStackAddImageToDoc(documentID, addImageID, imageID, server, tmpFileName, "TIF", tmpFileNameUndo, tmpFileNameRedo);

					return isSaveAfterAdd;
				}
			}
			return null;
		}

        /// <summary>
        /// Формирование команды Undo AddImageToDoc
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="addImageId"></param>
        /// <param name="imageId"></param>
        /// <param name="server"></param>
        /// <param name="tmpFileName"></param>
        /// <param name="imageType"></param>
        /// <param name="tmpFileNameUndo"> Копия оригинального файла</param>
        /// <param name="tmpFileNameRedo"> Копия измененного файла</param>
        private static void UndoRedoStackAddImageToDoc(int documentId, int addImageId, int imageId, ServerInfo server, string tmpFileName, string imageType, string tmpFileNameUndo, string tmpFileNameRedo)
        {
            Func<object[], bool> delegate1 = RemoveImageFromDoc;
            Func<object[], bool> delegate2 = AddImageToDoc;

            var undoText = Environment.StringResources.GetString("MainForm.UNDOAddImageToDoc");
            var redoText = Environment.StringResources.GetString("MainForm.REDOAddImageToDoc");

            Environment.UndoredoStack.Add("AddImageToDoc", "AddImageToDoc", undoText, redoText, null, new object[] { documentId, delegate1, delegate2, addImageId, imageId, server, tmpFileName, imageType, tmpFileNameUndo, tmpFileNameRedo }, 0);
        }

        /// <summary>
        /// UNDO AddImageToDoc
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static bool AddImageToDoc(object[] o)
        {
            int documentId = (int)o[0];
            int addImageId = (int)o[3];
            int imageId = (int)o[4];
            var server = (ServerInfo)o[5];
            string imageType = (string)o[7];
            string tmpFileNameRedo = (string)o[9];

            // Сохранение копии файла
            string tmpFileNameRedoTmp = Path.GetTempFileName();
            File.Copy(tmpFileNameRedo, tmpFileNameRedoTmp, true);

            bool isSaveAfterAdd = SaveAfterAdd(documentId, addImageId, imageId, server, tmpFileNameRedoTmp, imageType);

            return isSaveAfterAdd;
        }

        /// <summary>
        /// REDO AddImageToDoc
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static bool RemoveImageFromDoc(object[] o)
        {
            int documentId = (int)o[0];
            int addImageId = (int)o[3];
            int imageId = (int)o[4];
            var server = (ServerInfo)o[5];
            string imageType = (string)o[7];
            string tmpFileNameUndo = (string)o[8];

            // Сохранение копии файла
            string tmpFileNameUndoTmp = Path.GetTempFileName();
            File.Copy(tmpFileNameUndo, tmpFileNameUndoTmp, true);

            bool isSaveAfterAdd = SaveAfterAdd(documentId, addImageId, imageId, server, tmpFileNameUndoTmp, imageType);

            return isSaveAfterAdd;
        }

        public static string GetFile(int imageID, string extension)
        {
            string filename = "";
            List<int> servers = Environment.DocImageData.GetLocalDocImageServers(imageID, Environment.GetLocalServersString());
            foreach (int t in servers)
            {
                filename = Path.Combine(Environment.GetLocalServer(t).Path, Environment.GetFileNameFromID(imageID)) + extension;
                if (File.Exists(filename))
                    break;
                filename = "";
            }
            return filename;
        }

        public static bool SaveAfterAdd(int documentID, int addImageID, int imageID, ServerInfo server, string tmpFileName, string type)
        {
            string name = Environment.GenerateFileName(type.ToLower());
            string newFileName = server.Path + "\\Temp\\" + name;
            while (File.Exists(newFileName))
            {
                name = Environment.GenerateFileName(type.ToLower());
                newFileName = server.Path + "\\Temp\\" + name;
            }
            File.Move(tmpFileName, newFileName);
            if (!File.Exists(newFileName))
                throw new Exception(Environment.StringResources.GetString("Dialog_AddDBDocDialog_buttonOK_Click_Error2") + " " + name);
            Console.WriteLine("{0}: File to save: {1}", DateTime.Now.ToString("HH:mm:ss fff"), newFileName);
            int count = 0;
            if ("TIF".Equals(type.ToUpper()))
                using (var libTiff = new LibTiffHelper())
                    count = libTiff.GetCountPages(newFileName);
            else
            {
                var pr = new PdfReader(newFileName);
                count = pr.NumberOfPages;
                pr.Close();
            }
            if (count < 1)
                throw new Exception("отсутсвуют стрнаницы в сохраняемом файле");
            if (Environment.DocImageData.DocImageFileUpdate(server.ID, name, documentID, addImageID, type, count))
            {
                if (imageID > 0)
                    Environment.DocImageData.Delete(imageID);
                return true;
            }
            else
                return false;
        }

        private void check_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void checkAddToWork_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkAddToWork.Checked)
                checkSendMessage.Checked = true;

            if (!messageNeeded)
                checkSendMessage.Enabled = checkAddToWork.Checked;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            if (!baseEdit)
            {
                baseEdit = true;
                UpdateControls();
            }
        }

        private void personBlock_CountChanged(object source, EventArgs e)
        {
            docLinksBlock.PersonIDs = personBlock.PersonIDs;
        }

        private void docLinksBlock_Added(object source, BlockEventArgs e)
        {
            if (e.ID > 0)
            {
                // loading persons
                using (DataTable dt = Environment.DocData.GetDocPersons(e.ID, dateBlock.Value))
                {
                    var res = dt.Rows.Cast<DataRow>().Where(x => (byte)x[Environment.DocData.PersonPositionField] > 0).Select(x => new KeyValuePair<int, string>((int)x[Environment.PersonData.IDField], x[Environment.PersonData.NameField].ToString()));
                    if (res != null && res.Any())
                        foreach (KeyValuePair<int, string> dr in res)
                        {
                            if (!string.IsNullOrEmpty(dr.Value))
                                personBlock.AddPerson(dr.Key, dr.Value);
                        }
                    else
                        using (DataTableReader dr = dt.CreateDataReader())
                        {
                            while (dr.Read())
                            {
                                var personID = (int)dr[Environment.PersonData.IDField];
                                var person = (string)dr[Environment.PersonData.NameField];

                                if (!string.IsNullOrEmpty(person))
                                    personBlock.AddPerson(personID, person);
                            }
                            dr.Close();
                            dr.Dispose();
                        }
                    dt.Dispose();
                }
            }
        }

        private void AddDBDocDialog_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.OemPeriod:
                    buttonPush = true;
                    if (timer == null)
                    {
                        timer = new Timer(50);
                        timer.Elapsed += timer_Elapsed;
                        timer.Enabled = true;
                    }
                    else
                        timer.Interval = 50;
                    timer.AutoReset = false;
                    timer.Start();
                    break;
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    if (buttonPush)
                    {
                        e.Handled = true;
                        if (timer != null)
                            timer.Interval = 50;
                        pushedButton += e.KeyData.ToString().TrimStart("D".ToCharArray());
                    }
                    break;
                case Keys.Enter:
                    if (buttonPush)
                    {
                        buttonPush = false;
                        e.Handled = true;
                        try
                        {
                            int templateDocID = int.Parse(pushedButton);
                            DataRow dr = Environment.DocData.GetDocProperties(templateDocID, Environment.CurCultureInfo.TwoLetterISOLanguageName);
                            if (dr != null)
                            {
                                typeID = (int)dr[Environment.DocData.DocTypeIDField];
                                docTypeBlock.Text = dr[Environment.DocData.DocTypeField].ToString();
                                docTypeBlock.ID = typeID;
                                number.Text = dr[Environment.DocData.NumberField].ToString();
                                if (!dr.IsNull(Environment.DocData.DateField))
                                    dateBlock.Value = (DateTime)dr[Environment.DocData.DateField];

                                using (DataTable dt = Environment.DocData.GetDocPersons(templateDocID, dateBlock.Value))
                                using (DataTableReader row = dt.CreateDataReader())
                                {
                                    while (row.Read())
                                    {
                                        var personID = (int)row[Environment.PersonData.IDField];
                                        string person = Environment.PersonData.GetPerson(personID);
                                        if (person != null)
                                            personBlock.AddPerson(personID, person);
                                    }
                                    row.Close();
                                    row.Dispose();
                                    dt.Dispose();
                                }
                            }
                            buttonOK.Select();
                        }
                        catch { }
                        pushedButton = "";
                    }
                    break;
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            buttonPush = false;
            pushedButton = "";
            if (sender is Timer)
            {
                var timer = sender as Timer;
                timer.Elapsed -= timer_Elapsed;
                timer.Stop();
                timer.Dispose();
                if (timer == this.timer)
                {
                    this.timer = null;
                    timer = null;
                }
            }
        }

        private void checkBoxProtected_CheckedChanged(object sender, EventArgs e)
        {
            protect = checkBoxProtected.Checked;
        }

        private void checkBoxNoNumber_CheckedChanged(object sender, EventArgs e)
        {
            number.Enabled = !checkBoxNoNumber.Checked && baseEdit;
        }

        private void docTypeBlock_Selected(object source, BlockEventArgs e)
        {
            checkBoxName.Enabled = docTypeBlock.ID > 0 && Environment.DoesDocTypeNameExist(docTypeBlock.ID);
            if (!checkBoxName.Enabled)
            {
                //checkBoxName.Checked = false;
                textBox1.Text = "";
            }
        }

        private void checkBoxName_CheckedChanged(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                backgroundWorker1.CancelAsync();

            textBox1.Enabled = checkBoxName.Checked;
            if (docTypeBlock.ID > 0 && checkBoxName.Checked)
            {
                if (textBox1.AutoCompleteCustomSource.Count > 0)
                    textBox1.AutoCompleteCustomSource.Clear();
                textBox1.AutoCompleteCustomSource.Add(docTypeBlock.Text);

                while (backgroundWorker1.IsBusy) { Application.DoEvents(); }
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                textBox1.Text = "";
                textBox1.AutoCompleteCustomSource.Clear();
            }
        }

        private void docTypeBlock_DocTypeTextChanged(object sender, EventArgs e)
        {
            checkBoxName.Enabled = docTypeBlock.ID > 0 && Environment.DoesDocTypeNameExist(docTypeBlock.ID);
            if (!checkBoxName.Enabled)
            {
                checkBoxName.Checked = false;
                textBox1.Text = "";
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (backgroundWorker1.CancellationPending)
                return;
            var titles = new List<string>();
            using (var cmd = new SqlCommand(Environment.DocData.GetTitliesQuery(), new SqlConnection(Environment.DocData.GetAsyncConnectionString())))
            {
                cmd.Parameters.Add(new SqlParameter("@TypeID", SqlDbType.Int) {Value = docTypeBlock.ID});
                cmd.Parameters.Add(new SqlParameter("@EmpID", SqlDbType.Int) {Value = Environment.CurEmp.ID});

                if (backgroundWorker1.CancellationPending)
                    return;
                try
                {
                    cmd.Connection.Open();

                    IAsyncResult asyncResult = cmd.BeginExecuteReader();
                    while (!asyncResult.IsCompleted && !backgroundWorker1.CancellationPending)
                    {
						asyncResult.AsyncWaitHandle.WaitOne(300);
                    }

                    using (SqlDataReader dr = cmd.EndExecuteReader(asyncResult))
                    {
                        while (dr.Read() && !backgroundWorker1.CancellationPending)
                        {
                            if (dr[0] != DBNull.Value)
                                titles.Add(dr.GetString(0));
                        }
                        dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    Data.Env.WriteToLog(ex);
                }
                finally
                {
                    cmd.Connection.Close();
                    e.Cancel = backgroundWorker1.CancellationPending;
                }
            }
            e.Result = titles;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Result != null && e.Result is List<string>)
            {
                var res = e.Result as List<string>;
                if (res != null && res.Count > 0)
                {
                    try
                    {
                        if (InvokeRequired)
                            Invoke(new Action<string[]>(textBox1.AutoCompleteCustomSource.AddRange), res.ToArray());
                        else
                            textBox1.AutoCompleteCustomSource.AddRange(res.ToArray());
                    }
                    catch (Exception ex)
                    {
                        Data.Env.WriteToLog(ex);
                    }
                }
            }
        }

		private void dateBlock_Leave(object sender, EventArgs e)
		{
			if(!dateBlock.IsEmpty() && dateBlock.Check())
			{
				docDate = dateBlock.Value;
				docLinksBlock.DocDate = docDate;
			}
		}
    }
}