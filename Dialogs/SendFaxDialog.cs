using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Blocks;
using Kesco.Lib.Win.Document.Classes;
using Kesco.Lib.Win.Document.Win32;
using Kesco.Lib.Win.Tiff;
using Kesco.Lib.Win.Web;

namespace Kesco.Lib.Win.Document.Dialogs
{
	/// <summary>
	/// SendFaxDialog. предназначен для отправки факсов
	/// </summary>
	public class SendFaxDialog : FreeDialog
	{
		#region Variables

		private string fileName;
		private SynchronizedCollection<Components.ImageToSend> docs;
		private SynchronizedCollection<int> processingFiles;
		private int docImageID;
		private int faxID;
		private bool close;
		private bool anotherFormat;
		private string description;
		private Options.Folder subLayout;

		private int personID;
		private string phoneNumber;

		private CheckedControlCollection collection;

		private SynchronizedCollection<Keys> keyLocker = new SynchronizedCollection<Keys>();
		private IContainer components;
		private TextBox textBoxText;
		private Button buttonSend;
		private Button buttonCancel;
		private Panel panelButton;
		private Panel panelSearch;
		private Panel panel3;
		private Label label3;
		private TableLayoutPanel panelControl;
		private Label label4;
		private PersonSearchBlock personSearchBlock;
		private Label label8;
		private ToolTip toolTip;
		private Label label9;
		private Kesco.Lib.Win.Document.Controls.ImageShowControl imgEdit;
		private Panel panelImage;
		private GroupBox groupBoxResip;
		private GroupBox groupBox1;
		private TextBox textBoxPageTo;
		private Label label2;
		private TextBox textBoxPageFrom;
		private Label labelImage;
		private Panel panel2;
		private RadioButton pdfRadio;
		private RadioButton tifRadio;
		private bool pdfMode;
		private GroupBox groupBox2;
		private RadioButton radioButtonMainFormat;
		private GroupBox groupBoxColor;
		private RadioButton radioButtonBlackWhite;
		private RadioButton radioButtonColor;
		private CheckBox checkBoxToImage;
		private CheckBox checkBoxWithNotes;
		private Kesco.Lib.Win.Document.Controls.NewWindowDocumentButton newWindowDocumentButton;
		private Kesco.Lib.Win.Document.Controls.PdfViewControl.PDFView pdfEdit;

		#endregion

		public string FileName { get { return fileName; } }

		#region Constructors
		/// <summary>
		/// базовое определение класса отправки факса, скорее всего будет закрытым
		/// </summary>
		/// <param name="fileName"> имя отправляемого изабражения </param>
		public SendFaxDialog(int faxID, int imageID)
		{
			this.faxID = faxID;

			DataRow dr = Environment.FaxData.GetRow(faxID);
			object obj = dr[Environment.FaxInData.FileNameField];
			if(!(obj is string))
			{
				close = true;
				return;
			}

			string fName = (string)obj;
			obj = dr[Environment.FaxInData.FolderFaxIDField];
			if(!(obj is int))
			{
				close = true;
				return;
			}

			var fdr = Environment.FaxFolderData.GetRow((int)obj);
			if(fdr == null)
			{
				close = true;
				return;
			}

			string folderPath = fdr[Environment.FaxFolderData.NetworkPathField] as string;
			if(string.IsNullOrEmpty(folderPath))
			{
				close = true;
				return;
			}

			var server = Environment.GetServers().FirstOrDefault(x => Directory.Exists(Path.Combine(x.FaxPath, folderPath)));
			if(server == null)
			{
				close = true;
				return;
			}

			folderPath = Path.Combine(server.FaxPath, folderPath);
			fName = Path.Combine(folderPath, fName);

			if(!File.Exists(fName))
			{
				close = true;
				return;
			}

			obj = dr[Environment.FaxInData.DescriptionField];
			string descr = string.Empty;
			if(obj != null)
				descr = (string)obj;

			fileName = fName;
			InitializeComponent();
			subLayout = Environment.Layout.Folders.Add(Name);
			Width = subLayout.LoadIntOption("Width", Width);
			Height = subLayout.LoadIntOption("Height", Height);

			description = descr;
			Text += " " + description;
			docImageID = imageID;


			try
			{
				imgEdit.FileName = fName;
			}
			catch
			{
				close = true;
				return;
			}

			textBoxPageFrom.Text = "1";
			textBoxPageTo.Text = imgEdit.PageCount.ToString();
			collection = new CheckedControlCollection(panelControl) { XOrigin = -32 };
			collection.Checked += collection_Checked;
			if((short)dr[Environment.FaxData.DirectionField] == 2 && (short)dr[Environment.FaxData.StatusField] == -1)
				AddContact(1, ((dr[Environment.FaxData.RecipField].Equals(dr[Environment.FaxData.RecvAddressField])) ? string.Empty : dr[Environment.FaxData.RecipField].ToString()),
						   dr[Environment.FaxData.RecvAddressField].ToString());

			UpdateButtons();
		}

		public SendFaxDialog(int[] curDocsIDs, object[] mainImageIDs)
		{
			docs = new SynchronizedCollection<Components.ImageToSend>();

			for(int i = 0; i < curDocsIDs.Length; i++)
				docs.Add(new Components.ImageToSend(curDocsIDs[i], mainImageIDs[i]));

			BunchOfDocsLoaded(true);
		}

		public SendFaxDialog(object[] curFileNames, string[] curDocStrings)
		{
			docs = new SynchronizedCollection<Components.ImageToSend>();
			for(int i = 0; i < curFileNames.Length; i++)
				docs.Add(new Components.ImageToSend((string)curFileNames[i]) { FileNameToSend = curDocStrings[i]});

			BunchOfDocsLoaded(false);
		}

		public SendFaxDialog(object[] curFileNames, string[] curDocStrings, object[] curFaxesIDs)
		{
			docs = new SynchronizedCollection<Components.ImageToSend>();
			for(int i = 0; i < curFileNames.Length && i < curFaxesIDs.Length; i++)
				docs.Add(new Components.ImageToSend((string)curFileNames[i], curFaxesIDs[i]) { FileNameToSend = curDocStrings[i]});

			BunchOfDocsLoaded(false);
		}

		public SendFaxDialog(string fileName, string descr, int docID, int imageID, int personID, string phoneNumber)
			: this(fileName, descr, docID, imageID)
		{
			this.personID = personID;
			this.phoneNumber = phoneNumber;
		}

		public SendFaxDialog(string fileName, string descr, int docID, int imageID) : this(fileName, descr, docID, imageID, false) { }

		public SendFaxDialog(string fileName, string descr, int docID, int imageID, bool isPdf)
		{
			this.fileName = fileName;
			InitializeComponent();

			subLayout = Environment.Layout.Folders.Add(Name);
			Width = subLayout.LoadIntOption("Width", Width);
			Height = subLayout.LoadIntOption("Height", Height);
			description = descr;
			Text += " " + description;
			docImageID = imageID;

			if(docID > 0)
				newWindowDocumentButton.Set(docID);
			else
				newWindowDocumentButton.Set(fileName, descr);

			if(isPdf)
			{
				pdfEdit = new Controls.PdfViewControl.PDFView { NeedPreview = false };
				pdfMode = true;
			}
			else
				pdfMode = false;

			pdfRadio.Checked = pdfMode;
			tifRadio.Checked = !pdfMode;

			if(imageID > 0)
			{
				try
				{
					if(!isPdf)
						imgEdit.ImageID = imageID;
					else
						pdfEdit.ImageID = imageID;
				}
				catch
				{
					close = true;
					return;
				}
			}
			else
				if(File.Exists(fileName))
				{
					try
					{
						if(!isPdf)
						{
							imgEdit.FileName = fileName;
							anotherFormat = imgEdit.AnotherFormat;
						}
						else
							pdfEdit.FileName = fileName;
					}
					catch
					{
						close = true;
						return;
					}
				}

			radioButtonMainFormat.Enabled = anotherFormat;

			if(radioButtonMainFormat.Enabled || pdfMode)
			{
				radioButtonMainFormat.Checked = radioButtonMainFormat.Enabled;
				groupBoxColor.Enabled = false;
			}
			else
				radioButtonColor.Checked = true;

			textBoxPageFrom.Text = "1";
			textBoxPageTo.Text = !isPdf ? imgEdit.PageCount.ToString() : pdfEdit.PageCount.ToString();

			collection = new CheckedControlCollection(panelControl);
			collection.XOrigin = -32;
			collection.Checked += collection_Checked;
		}

		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(pdfEdit != null) { pdfEdit.FileName = string.Empty; pdfEdit.Dispose(); pdfEdit = null; }

			if(disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
			GC.Collect();
			//GC.WaitForPendingFinalizers();
			//GC.Collect();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SendFaxDialog));
			this.textBoxText = new System.Windows.Forms.TextBox();
			this.buttonSend = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panelSearch = new System.Windows.Forms.Panel();
			this.personSearchBlock = new Kesco.Lib.Win.Document.Blocks.PersonSearchBlock();
			this.label8 = new System.Windows.Forms.Label();
			this.panelButton = new System.Windows.Forms.Panel();
			this.newWindowDocumentButton = new Kesco.Lib.Win.Document.Controls.NewWindowDocumentButton(this.components);
			this.panel3 = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.panelControl = new System.Windows.Forms.TableLayoutPanel();
			this.label9 = new System.Windows.Forms.Label();
			this.panelImage = new System.Windows.Forms.Panel();
			this.imgEdit = new Kesco.Lib.Win.Document.Controls.ImageShowControl();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.textBoxPageTo = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxPageFrom = new System.Windows.Forms.TextBox();
			this.labelImage = new System.Windows.Forms.Label();
			this.groupBoxResip = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.checkBoxToImage = new System.Windows.Forms.CheckBox();
			this.checkBoxWithNotes = new System.Windows.Forms.CheckBox();
			this.groupBoxColor = new System.Windows.Forms.GroupBox();
			this.radioButtonBlackWhite = new System.Windows.Forms.RadioButton();
			this.radioButtonColor = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tifRadio = new System.Windows.Forms.RadioButton();
			this.radioButtonMainFormat = new System.Windows.Forms.RadioButton();
			this.pdfRadio = new System.Windows.Forms.RadioButton();
			this.panelSearch.SuspendLayout();
			this.panelButton.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panelControl.SuspendLayout();
			this.panelImage.SuspendLayout();
			this.groupBoxResip.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBoxColor.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxText
			// 
			resources.ApplyResources(this.textBoxText, "textBoxText");
			this.textBoxText.Name = "textBoxText";
			this.toolTip.SetToolTip(this.textBoxText, resources.GetString("textBoxText.ToolTip"));
			this.textBoxText.Leave += new System.EventHandler(this.textBoxText_Leave);
			// 
			// buttonSend
			// 
			resources.ApplyResources(this.buttonSend, "buttonSend");
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// panelSearch
			// 
			resources.ApplyResources(this.panelSearch, "panelSearch");
			this.panelSearch.Controls.Add(this.personSearchBlock);
			this.panelSearch.Controls.Add(this.label8);
			this.panelSearch.Name = "panelSearch";
			// 
			// personSearchBlock
			// 
			resources.ApplyResources(this.personSearchBlock, "personSearchBlock");
			this.personSearchBlock.ButtonWight = 128;
			this.personSearchBlock.FullText = "";
			this.personSearchBlock.Name = "personSearchBlock";
			this.personSearchBlock.SearchType = true;
			this.toolTip.SetToolTip(this.personSearchBlock, resources.GetString("personSearchBlock.ToolTip"));
			this.personSearchBlock.AddContact += new System.EventHandler(this.personSearchBlock_AddContact);
			this.personSearchBlock.CreateFaxContact += new System.EventHandler(this.personSearchBlock_CreateFaxContact);
			this.personSearchBlock.CreateEmailContact += new System.EventHandler(this.personSearchBlock_CreateEmailContact);
			this.personSearchBlock.CreateClient += new System.EventHandler(this.personSearchBlock_CreateClient);
			this.personSearchBlock.CreateClientPerson += new System.EventHandler(this.personSearchBlock_CreateClientPerson);
			this.personSearchBlock.FindPerson += new System.EventHandler(this.personSearchBlock_FindPerson);
			this.personSearchBlock.FindFax += new System.EventHandler(this.personSearchBlock_FindFax);
			this.personSearchBlock.FindEMail += new System.EventHandler(this.personSearchBlock_FindEMail);
			this.personSearchBlock.FindNothing += new System.EventHandler(this.personSearchBlock_FindNothing);
			this.personSearchBlock.FindInternal += new System.EventHandler(this.personSearchBlock_FindInternal);
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			this.toolTip.SetToolTip(this.label8, resources.GetString("label8.ToolTip"));
			// 
			// panelButton
			// 
			this.panelButton.Controls.Add(this.newWindowDocumentButton);
			this.panelButton.Controls.Add(this.buttonSend);
			this.panelButton.Controls.Add(this.buttonCancel);
			resources.ApplyResources(this.panelButton, "panelButton");
			this.panelButton.Name = "panelButton";
			// 
			// newWindowDocumentButton
			// 
			resources.ApplyResources(this.newWindowDocumentButton, "newWindowDocumentButton");
			this.newWindowDocumentButton.Name = "newWindowDocumentButton";
			this.newWindowDocumentButton.TabStop = false;
			this.newWindowDocumentButton.UseMnemonic = false;
			this.newWindowDocumentButton.UseVisualStyleBackColor = true;
			// 
			// panel3
			// 
			resources.ApplyResources(this.panel3, "panel3");
			this.panel3.Controls.Add(this.label4);
			this.panel3.Controls.Add(this.label3);
			this.panel3.Name = "panel3";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// panelControl
			// 
			resources.ApplyResources(this.panelControl, "panelControl");
			this.panelControl.Controls.Add(this.label9, 0, 0);
			this.panelControl.Controls.Add(this.panelImage, 0, 1);
			this.panelControl.Name = "panelControl";
			// 
			// label9
			// 
			resources.ApplyResources(this.label9, "label9");
			this.label9.Name = "label9";
			this.toolTip.SetToolTip(this.label9, resources.GetString("label9.ToolTip"));
			// 
			// panelImage
			// 
			resources.ApplyResources(this.panelImage, "panelImage");
			this.panelImage.Controls.Add(this.imgEdit);
			this.panelImage.Cursor = System.Windows.Forms.Cursors.Default;
			this.panelImage.Name = "panelImage";
			// 
			// imgEdit
			// 
			resources.ApplyResources(this.imgEdit, "imgEdit");
			this.imgEdit.AnnotationDraw = false;
			this.imgEdit.CurrentStamp = null;
			this.imgEdit.CurrentStampID = 0;
			this.imgEdit.Cursor = System.Windows.Forms.Cursors.Default;
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
			this.imgEdit.ShowThumbPanel = false;
			this.imgEdit.SplinterPlace = 4;
			this.imgEdit.ThumbnailPanelOrientation = Kesco.Lib.Win.ImageControl.ImageControl.TypeThumbnailPanelOrientation.Left;
			this.imgEdit.TypeWorkThumbnailImagesPanel = 3;
			this.imgEdit.UseLock = false;
			this.imgEdit.Zoom = 100;
			// 
			// textBoxPageTo
			// 
			resources.ApplyResources(this.textBoxPageTo, "textBoxPageTo");
			this.textBoxPageTo.Name = "textBoxPageTo";
			this.toolTip.SetToolTip(this.textBoxPageTo, resources.GetString("textBoxPageTo.ToolTip"));
			this.textBoxPageTo.Leave += new System.EventHandler(this.textBoxPageTo_Leave);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			this.toolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
			// 
			// textBoxPageFrom
			// 
			resources.ApplyResources(this.textBoxPageFrom, "textBoxPageFrom");
			this.textBoxPageFrom.Name = "textBoxPageFrom";
			this.toolTip.SetToolTip(this.textBoxPageFrom, resources.GetString("textBoxPageFrom.ToolTip"));
			this.textBoxPageFrom.Leave += new System.EventHandler(this.textBoxPageFrom_Leave);
			// 
			// labelImage
			// 
			resources.ApplyResources(this.labelImage, "labelImage");
			this.labelImage.Name = "labelImage";
			this.toolTip.SetToolTip(this.labelImage, resources.GetString("labelImage.ToolTip"));
			// 
			// groupBoxResip
			// 
			resources.ApplyResources(this.groupBoxResip, "groupBoxResip");
			this.groupBoxResip.Controls.Add(this.panelControl);
			this.groupBoxResip.Controls.Add(this.panel3);
			this.groupBoxResip.Controls.Add(this.panelSearch);
			this.groupBoxResip.Name = "groupBoxResip";
			this.groupBoxResip.TabStop = false;
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.textBoxText);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// panel2
			// 
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Controls.Add(this.checkBoxToImage);
			this.panel2.Controls.Add(this.checkBoxWithNotes);
			this.panel2.Controls.Add(this.groupBoxColor);
			this.panel2.Controls.Add(this.groupBox2);
			this.panel2.Controls.Add(this.textBoxPageTo);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.textBoxPageFrom);
			this.panel2.Controls.Add(this.labelImage);
			this.panel2.Name = "panel2";
			// 
			// checkBoxToImage
			// 
			resources.ApplyResources(this.checkBoxToImage, "checkBoxToImage");
			this.checkBoxToImage.Name = "checkBoxToImage";
			this.checkBoxToImage.UseVisualStyleBackColor = true;
			// 
			// checkBoxWithNotes
			// 
			resources.ApplyResources(this.checkBoxWithNotes, "checkBoxWithNotes");
			this.checkBoxWithNotes.Name = "checkBoxWithNotes";
			this.checkBoxWithNotes.UseVisualStyleBackColor = true;
			// 
			// groupBoxColor
			// 
			resources.ApplyResources(this.groupBoxColor, "groupBoxColor");
			this.groupBoxColor.Controls.Add(this.radioButtonBlackWhite);
			this.groupBoxColor.Controls.Add(this.radioButtonColor);
			this.groupBoxColor.Name = "groupBoxColor";
			this.groupBoxColor.TabStop = false;
			// 
			// radioButtonBlackWhite
			// 
			resources.ApplyResources(this.radioButtonBlackWhite, "radioButtonBlackWhite");
			this.radioButtonBlackWhite.Name = "radioButtonBlackWhite";
			this.radioButtonBlackWhite.TabStop = true;
			this.radioButtonBlackWhite.UseVisualStyleBackColor = true;
			// 
			// radioButtonColor
			// 
			resources.ApplyResources(this.radioButtonColor, "radioButtonColor");
			this.radioButtonColor.Name = "radioButtonColor";
			this.radioButtonColor.TabStop = true;
			this.radioButtonColor.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this.tifRadio);
			this.groupBox2.Controls.Add(this.radioButtonMainFormat);
			this.groupBox2.Controls.Add(this.pdfRadio);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// tifRadio
			// 
			resources.ApplyResources(this.tifRadio, "tifRadio");
			this.tifRadio.Checked = true;
			this.tifRadio.Name = "tifRadio";
			this.tifRadio.TabStop = true;
			this.tifRadio.UseVisualStyleBackColor = true;
			this.tifRadio.CheckedChanged += new System.EventHandler(this.tifRadio_CheckedChanged);
			// 
			// radioButtonMainFormat
			// 
			resources.ApplyResources(this.radioButtonMainFormat, "radioButtonMainFormat");
			this.radioButtonMainFormat.Name = "radioButtonMainFormat";
			this.radioButtonMainFormat.UseVisualStyleBackColor = true;
			this.radioButtonMainFormat.CheckedChanged += new System.EventHandler(this.radioButtonMainFormat_CheckedChanged);
			// 
			// pdfRadio
			// 
			resources.ApplyResources(this.pdfRadio, "pdfRadio");
			this.pdfRadio.Name = "pdfRadio";
			this.pdfRadio.UseVisualStyleBackColor = true;
			this.pdfRadio.CheckedChanged += new System.EventHandler(this.pdfRadio_CheckedChanged);
			// 
			// SendFaxDialog
			// 
			this.AcceptButton = this.buttonSend;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.panelButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBoxResip);
			this.Controls.Add(this.panel2);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.Name = "SendFaxDialog";
			this.Closed += new System.EventHandler(this.SendFaxDialog_Closed);
			this.Load += new System.EventHandler(this.SendFaxDialog_Load);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SendFaxDialog_KeyUp);
			this.panelSearch.ResumeLayout(false);
			this.panelButton.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panelControl.ResumeLayout(false);
			this.panelControl.PerformLayout();
			this.panelImage.ResumeLayout(false);
			this.groupBoxResip.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.groupBoxColor.ResumeLayout(false);
			this.groupBoxColor.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// закрытие по отмене
		/// </summary>
		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void SendFaxDialog_Load(object sender, EventArgs e)
		{
			if(close)
			{
				Close();
				return;
			}
			textBoxText.Text = description;
			textBoxText.Select();
			if(personID > 0)
				AddPersonContact(personID, phoneNumber);
			checkBoxWithNotes.Visible = checkBoxToImage.Visible = tifRadio.Checked && !pdfMode;
			checkBoxWithNotes.Checked = checkBoxToImage.Checked = false;
		}

		#region Old add

		private void addPersonContactDialog_DialogEvent(object source, DialogEventArgs e)
		{
			if(!Enabled)
				Enabled = true;
			Focus();
			if(e.Dialog.DialogResult != DialogResult.OK)
				return;
			PersonContactDialog dialog = e.Dialog as PersonContactDialog;
			if(dialog == null)
				return;

			string personName = dialog.PersonName;
			int personID = dialog.PersonID;
			for(int i = 0; i < dialog.Collection.Count; i++)
			{
				if(!dialog.Collection[i].Checked)
					continue;
				DataRow dr = dialog.Collection[i].Tag as DataRow;
				if(dr == null || dr.IsNull(Environment.FaxRecipientData.PersonLinkIDField))
					AddContact(dr, new KeyValuePair<int, string>(personID, personName));
				else
				{
					KeyValuePair<KeyValuePair<int, string>, KeyValuePair<int, string>> res =
						Environment.PersonLinkData.GetFormatedLink(
							(int)dr[Environment.FaxRecipientData.PersonLinkIDField]);
					AddContact(dr, res.Key, res.Value);
				}
			}
		}

		#endregion

		#region button control

		private void UpdateButtons()
		{
			panel3.Visible = collection.Count > 0;
			label9.Visible = collection.Count == 0;
			buttonSend.Enabled = collection.Count > 0;
		}

		private void collection_Checked(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		#endregion

		#region Send

		private void SendMail(List<KeyValuePair<string, string>> mails, string descr)
		{
			bool isPdfFormat = pdfRadio.Checked;
			bool isOriginal = radioButtonMainFormat.Checked;
			string exte = Path.GetExtension(fileName).TrimStart('.');

			bool color = radioButtonColor.Checked;

			int startPage = 1;
			if(!int.TryParse(textBoxPageFrom.Text, out startPage))
				return;
			if(startPage > 1)
			{
				if(isOriginal)
				{
					isOriginal = false;
					isPdfFormat = pdfMode;
				}
			}
			else
				if(startPage < 1)
					return;

			int countPage, endPage = countPage = (!pdfMode ? imgEdit.PageCount : pdfEdit.PageCount);
			if(!int.TryParse(textBoxPageTo.Text, out endPage))
				return;

			if(endPage < countPage)
			{
				if(isOriginal)
				{
					isOriginal = false;
					isPdfFormat = pdfMode;
				}
			}
			else
				if(endPage > countPage)
					return;

			if(!isOriginal)
				exte = isPdfFormat ? "pdf" : "tif";
			string newFileName = Path.Combine(Path.GetTempPath(), Environment.GenerateFileName(exte));
			Console.WriteLine("{0}: new filename: {1}", DateTime.Now.ToString("HH:mm:ss fff"), newFileName);

			Cursor = Cursors.WaitCursor;
			if(isOriginal)
				File.Copy(fileName, newFileName);
			else
				if(!pdfMode)
				{
					if(endPage <= imgEdit.PageCount && startPage > 0)
					{
						imgEdit.SaveWithBurnAndResolution(startPage - 1, endPage - 1, newFileName, 300, 300, checkBoxWithNotes.Checked, color, checkBoxToImage.Checked);

						if(isPdfFormat)
						{

							LibTiffHelper libTiff = new LibTiffHelper();
							List<Tiff.PageInfo> il = libTiff.GetPagesCollectionFromFile(newFileName, 0, -1, false);
							try { File.Delete(newFileName); }
							catch { }
							ConvertAndInsertClass.Convert(il, newFileName);

							libTiff.Dispose();
							il.Clear();
						}
					}
				}
				else
				{
					if(endPage <= pdfEdit.PageCount && startPage > 0)
						pdfEdit.SaveWithBurnAndResolution(startPage, endPage, newFileName, 300, 300, color, !isPdfFormat);
				}
			if(!File.Exists(newFileName))
				return;
			FileInfo fi = new FileInfo(newFileName);
			if(fi == null || fi.Length > 26214400)
			{
				MessageForm mf = new MessageForm("Размер вложения превышает 25 Мегабайт.\nПисьмо может быть не отправлено из-за большого размера вложения\nOтправить сообщение?", Environment.StringResources.GetString("Comformation"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
				if(mf.ShowDialog() != System.Windows.Forms.DialogResult.Yes)
				{
					File.Delete(newFileName);
					return;
				}
			}
			Outlook.Application ol = null;
			Outlook.NameSpace myNameSpace = null;

			try
			{
				ol = new Outlook.Application();
			}
			catch
			{
				ol = null;
			}

			int version = 0;
			if(ol != null && ol.Version.IndexOf('.') > 0 && int.TryParse(ol.Version.Substring(0, ol.Version.IndexOf('.')), out version) && version > 10)
			{
				try
				{
					myNameSpace = ol.GetNamespace("MAPI");
					object myMissing = Missing.Value;
					myNameSpace.Logon(myMissing, myMissing, true, myMissing);

					Outlook.MailItem item;
					object obj = ol.CreateItem(Outlook.OlItemType.olMailItem);
					item = (Outlook.MailItem)obj;

					item.Subject = descr;
					string mail = string.Empty;
					for(int i = 0; i < mails.Count; i++)
					{
						if(mail.Length > 0)
							mail += ";";
						KeyValuePair<string, string> mailIntry = mails[i];
						mail += EmailCreate(mailIntry.Key, mailIntry.Value, ol.Version.StartsWith("9."));
					}
					item.To = mail;
					string sendDocFileName = Path.Combine(Path.GetTempPath(), "Document." + exte);
					if(File.Exists(sendDocFileName))
						File.Delete(sendDocFileName);
					File.Move(newFileName, sendDocFileName);
					item.Attachments.Add(sendDocFileName, 1, 1, "Document." + exte);
					item.Display(false);
					Marshal.ReleaseComObject(item);
					item = null;
					if(File.Exists(sendDocFileName))
						File.Delete(sendDocFileName);
				}
				catch(System.Exception ex)
				{

					MessageBox.Show(ex.Message);
				}
				finally
				{
					if(ol != null)
					{
						Marshal.ReleaseComObject(ol);
						ol = null;
					}
					if(myNameSpace != null)
					{
						myNameSpace.Logoff();
						Marshal.ReleaseComObject(myNameSpace);
						myNameSpace = null;
					}
					Cursor = Cursors.Default;
				}
			}
			else
			{
				if(!string.IsNullOrEmpty(Environment.FullExchangeServerEwsUrl))
				{
					string sendDocFileName = Path.Combine(Path.GetTempPath(), "Document." + exte);
					try
					{
						if(File.Exists(sendDocFileName))
							File.Delete(sendDocFileName);

						File.Move(newFileName, sendDocFileName);
					}
					catch(System.Exception ex)
					{
						Data.Env.WriteToLog(ex);
						Error.ErrorShower.OnShowError(this, ex.Message, "");
					}
					EWSMail.ExchangeServiceBinding esb = new EWSMail.ExchangeServiceBinding();
					esb.Credentials = CredentialCache.DefaultNetworkCredentials;
					esb.Url = Environment.FullExchangeServerEwsUrl;
					esb.RequestServerVersionValue = new EWSMail.RequestServerVersion();
					esb.RequestServerVersionValue.Version = EWSMail.ExchangeVersionType.Exchange2010_SP2;
					EWSMail.TargetFolderIdType tfid = null;
					string folder = null;
					try
					{
						EWSMail.GetFolderResponseType gfo = esb.GetFolder(new EWSMail.GetFolderType
						{
							FolderIds = new EWSMail.DistinguishedFolderIdType[1] 
							{	
								new EWSMail.DistinguishedFolderIdType { Id = EWSMail.DistinguishedFolderIdNameType.drafts, 
									Mailbox = new EWSMail.EmailAddressType {EmailAddress = Environment.MailBoxName}}
								
							},
							FolderShape = new EWSMail.FolderResponseShapeType { BaseShape = EWSMail.DefaultShapeNamesType.Default }

						});
						if(gfo != null && gfo.ResponseMessages.Items.Length > 0 && gfo.ResponseMessages.Items[0].ResponseClass == EWSMail.ResponseClassType.Success)
						{
							EWSMail.FolderInfoResponseMessageType fldr = gfo.ResponseMessages.Items[0] as EWSMail.FolderInfoResponseMessageType;
							if(fldr != null && fldr.Folders.Length > 0)
							{
								tfid = new EWSMail.TargetFolderIdType
										   {
											   Item = fldr.Folders[0].FolderId
										   };
								folder = fldr.Folders[0].DisplayName.Trim();
							}
						}
						if(tfid == null || string.IsNullOrEmpty(folder))
							throw new NullReferenceException("Not find temp folder");
					}
					catch(System.Exception ex)
					{
						Data.Env.WriteToLog(ex);
						Error.ErrorShower.OnShowError(this, ex.Message, "");
						return;
					}

					EWSMail.CreateItemType cit = new EWSMail.CreateItemType
											 {
												 Items = new EWSMail.NonEmptyArrayOfAllItemsType(),
												 MessageDispositionSpecified = true,
												 SavedItemFolderId = tfid,
												 MessageDisposition = EWSMail.MessageDispositionType.SaveOnly
											 };

					EWSMail.MessageType mt = new EWSMail.MessageType();
					mt.From = new EWSMail.SingleRecipientType { Item = new EWSMail.EmailAddressType { EmailAddress = Environment.MailBoxName } };
					mt.ToRecipients = new EWSMail.EmailAddressType[mails.Count];
					for(int j = 0; j < mails.Count; j++)
					{
						KeyValuePair<string, string> mailIntry = mails[j];
						string[ ] emails = Regex.Split(mailIntry.Value.Trim(), "[^-+._a-z0-9@]", RegexOptions.IgnoreCase);
						EWSMail.EmailAddressType mat = new EWSMail.EmailAddressType();
						if(emails.Length == 1 && !string.IsNullOrEmpty(mailIntry.Key))
							mat.Name = mailIntry.Key;
						StringBuilder sb = new StringBuilder();
						for(int k = 0; k < emails.Length; k++)
						{
							if(!string.IsNullOrEmpty(emails[k]))
							{
								sb.Append(emails[k]);
								sb.Append(";");
							}
						}
						mat.EmailAddress = sb.ToString().TrimEnd(';');
						mt.ToRecipients[j] = mat;
					}
					mt.Subject = descr;

					cit.Items.Items = new EWSMail.ItemType[1];
					cit.Items.Items[0] = mt;

					EWSMail.FileAttachmentType fat = new EWSMail.FileAttachmentType();
					byte[ ] binaryData;
					using(FileStream inFile = new FileStream(sendDocFileName, FileMode.Open, FileAccess.Read))
					{
						binaryData = new Byte[inFile.Length];
						inFile.Read(binaryData, 0, (int)inFile.Length);
						inFile.Close();
					}
					fat.Content = binaryData;
					if(exte == "pdf")
						fat.ContentType = "application/pdf";
					else
						fat.ContentType = "image/tiff";
					fat.Name = Name = "Document." + exte;
					try
					{
						EWSMail.CreateItemResponseType crit = esb.CreateItem(cit);
						if(crit == null || crit.ResponseMessages.Items.Length == 0 || crit.ResponseMessages.Items[0].ResponseClass != EWSMail.ResponseClassType.Success)
							throw new System.Exception("Can't save message/n" + ((crit == null)?"no response":"responce length " + crit.ResponseMessages.Items.Length.ToString()) + "\n" + Environment.FullExchangeServerEwsUrl);
						EWSMail.ItemIdType iid = ((EWSMail.ItemInfoResponseMessageType)(((crit)).ResponseMessages.Items[0])).Items.Items[0].ItemId;
						EWSMail.CreateAttachmentResponseType cart = esb.CreateAttachment
						(
							new EWSMail.CreateAttachmentType
								{
									Attachments = new EWSMail.AttachmentType[1] { fat },
									ParentItemId = iid
								});

						if(cart == null || cart.ResponseMessages.Items.Length == 0 || cart.ResponseMessages.Items[0].ResponseClass != EWSMail.ResponseClassType.Success)
						{
							esb.DeleteItem(new EWSMail.DeleteItemType { ItemIds = new EWSMail.BaseItemIdType[1] { iid } });
							throw new System.Exception("Can't create attachment");
						}

						EWSMail.GetItemResponseType gir = esb.GetItem(
						new EWSMail.GetItemType
							{
								ItemIds = new EWSMail.BaseItemIdType[1] { iid },
								ItemShape = new EWSMail.ItemResponseShapeType
												{
													BaseShape = EWSMail.DefaultShapeNamesType.IdOnly,
													AdditionalProperties = new EWSMail.BasePathToElementType[1]
													{
														new EWSMail.PathToUnindexedFieldType { FieldURI = EWSMail.UnindexedFieldURIType.itemWebClientEditFormQueryString }
													}
												}
							});
						if(gir != null && gir.ResponseMessages.Items.Length > 0 && gir.ResponseMessages.Items[0].ResponseClass == EWSMail.ResponseClassType.Success)
							Environment.IEOpenOnURL(Environment.FullExchangeServerOwaUrl + Environment.MailBoxName + "/" + (((EWSMail.ItemInfoResponseMessageType)(((gir)).ResponseMessages.Items[0])).Items.Items[0]).WebClientEditFormQueryString);
					}
					catch(System.Exception ex)
					{

						Slave.DeleteFile(newFileName);
						Cursor = Cursors.Default;
						if(!ex.Message.Contains(" Insufficient Storage"))
						{
							Data.Env.WriteToLog(ex);

							Error.ErrorShower.OnShowError(this, ex.Message, "");
						}
						else
							Error.ErrorShower.OnShowError(this, "Не удалось присоеденить документ. Превышен объём сообщения или не доступен почтовый ящик.", Environment.StringResources.GetString("Error"));
						return;
					}
					finally
					{
						if(File.Exists(sendDocFileName))
							File.Delete(sendDocFileName);
					}
				}
			}

			if(faxID > 0)
				Environment.FaxOutData.MarkResend(faxID);

			Slave.DeleteFile(newFileName);

			close = true;

		}

		private void SendFax(string recipient, string faxMail, string descr)
		{
			ServerInfo server = Environment.GetRandomLocalServer();
			string name = Environment.GenerateFileName("tif");
			string newFileName = server.Path + "\\temp\\" + name;
			Console.WriteLine("{0}: new filename: {1}", DateTime.Now.ToString("HH:mm:ss fff"), newFileName);

			int startPage = 1;
			if(!int.TryParse(textBoxPageFrom.Text, out startPage))
				return;

			int endPage = (!pdfMode ? imgEdit.PageCount : pdfEdit.PageCount);
			if(!int.TryParse(textBoxPageTo.Text, out endPage))
				return;

			if(!pdfMode)
			{
				if(endPage <= imgEdit.PageCount && startPage > 0)
					imgEdit.SaveWithBurnAndResolution(startPage - 1, endPage - 1, newFileName, 204, 196, false, false, false);
			}
			else
			{
				if(endPage <= pdfEdit.PageCount && startPage > 0)
					pdfEdit.SaveWithBurnAndResolution(startPage - 1, endPage - 1, newFileName, 204, 196, false, true);
			}

			// send fax
			if(Environment.FaxData.SendFax(server.ID, recipient, faxMail, descr, name, docImageID))
			{
				if(faxID > 0)
					Environment.FaxOutData.MarkResend(faxID);
				if(File.Exists(fileName))
				{
					try
					{
						if(!pdfMode)
							imgEdit.FileName = fileName;
					}
					catch
					{
						return;
					}
				}
				close = true;
			}
			else
			{
				MessageBox.Show(Environment.StringResources.GetString("Dialog_SendFaxDialog_SendFax_Error1"), Environment.StringResources.GetString("Error"));
				if(File.Exists(fileName))
				{
					try
					{
						if(!pdfMode)
							imgEdit.FileName = fileName;

					}
					catch
					{
						return;
					}
				}
			}

			//using(MailMessage mm = new MailMessage("74957425361@kescom.com", "FAX=" +faxMail+"@kescom.com"))
			//{
			//    mm.Subject = faxMail;
			//    mm.Body = "";
			//    mm.Attachments.Add(new Attachment(newFileName));
			//    mm.IsBodyHtml = false;
			//    SmtpClient sc = new SmtpClient("voice-gw-msk");
			//    {//Здесь должен быть адрес почтового сервера и порт, если требуется
			//        sc.Send(mm);
			//    }
			//}
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			buttonSend.Enabled = false;
			List<KeyValuePair<string, string>> mail = new List<KeyValuePair<string, string>>();
			int fileNamesCount = (string.IsNullOrEmpty(fileName) && docs != null && docs.Count > 0) ? docs.Count : -1;

			if(fileNamesCount == -1)
			{
				for(int i = 0; i < collection.Count; i++)
				{
					Controls.CheckControl cont = collection[i];
					if(cont.SendType > 2)
					{
						DataRow dr = cont.Tag as DataRow;
						if(cont.SendType.Equals(3))
						{
							SendFax(cont.PersonName, "+" + dr[Environment.FaxRecipientData.ContactRLField].ToString().Trim(), textBoxText.Text);
						}
						else
						{
							mail.Add(new KeyValuePair<string, string>(cont.PersonName, dr[Environment.FaxRecipientData.ContactField].ToString()));
							if(docImageID > 0)
								Environment.LogEmailData.LogEmail(docImageID, cont.PersonName, dr[Environment.FaxRecipientData.ContactField].ToString());
						}
					}
					else
					{
						object[] contact = cont.Tag as object[];
						if(cont.SendType.Equals(1))
						{
							SendFax(contact[0].ToString(), contact[1].ToString(), textBoxText.Text);
						}
						else
						{
							mail.Add(new KeyValuePair<string, string>(contact[0].ToString(), contact[1].ToString()));
							if(docImageID > 0)
								Environment.LogEmailData.LogEmail(docImageID, contact[0].ToString(), contact[1].ToString());
						}
					}
				}
				if(mail.Count > 0)
					SendMail(mail, textBoxText.Text);
				buttonSend.Enabled = true;
				if(close)
					End(DialogResult.OK);
			}
			else
			{
				for(int i = 0; i < collection.Count; i++)
				{
					Controls.CheckControl cont = collection[i];
					if(cont.SendType > 2)
					{
						DataRow dr = cont.Tag as DataRow;
						if(cont.SendType.Equals(3))
						{
							SendBunchOfFaxes(cont.PersonName, "+" + dr[Environment.FaxRecipientData.ContactRLField].ToString().Trim(), textBoxText.Text, fileNamesCount);
						}
						else
						{
							mail.Add(new KeyValuePair<string, string>(cont.PersonName, dr[Environment.FaxRecipientData.ContactField].ToString()));

							List<int> docImageIDs = new List<int>();
							for(int j = 0; j < docs.Count; j++)
								docImageIDs.Add(docs[j].ImageID);
							if(docImageIDs.Count >0)
								Environment.LogEmailData.LogEmail(docImageIDs.ToArray(), cont.PersonName, dr[Environment.FaxRecipientData.ContactField].ToString());
						}
					}
					else
					{
						object[] contact = cont.Tag as object[];
						if(cont.SendType.Equals(1))
						{
							SendBunchOfFaxes(contact[0].ToString(), contact[1].ToString(), textBoxText.Text, fileNamesCount);
						}
						else
						{
							mail.Add(new KeyValuePair<string, string>(contact[0].ToString(), contact[1].ToString()));

							List<int> docImageIDs = new List<int>();
							for(int j = 0; j < docs.Count; j++)
								docImageIDs.Add(docs[j].ImageID);
							if(docImageIDs.Count >0)
								Environment.LogEmailData.LogEmail(docImageIDs.ToArray(), contact[0].ToString(), contact[1].ToString());
						}
					}
				}
				SendBunchOfDocsMail(mail, textBoxText.Text);
			}
		}

		public string EmailCreate(string recipient, string faxMail, bool oldVersion)
		{
			string[] faxMails = Regex.Split(faxMail.Trim(), "[^-+._a-z0-9@]", RegexOptions.IgnoreCase);
			StringBuilder retV = new StringBuilder();
			if(faxMails.Length == 1)
			{
				if(string.IsNullOrEmpty(faxMails[0].Trim()))
					return string.Empty;
				retV.Append(recipient);
				retV.Append("[SMTP:");
				retV.Append(faxMails[0].Trim());
				retV.Append("]");

			}
			else
				for(int i = 0; i < faxMails.Length; i++)
				{
					if(string.IsNullOrEmpty(faxMails[i].Trim()))
						continue;
					if(retV.Length > 0)
						retV.Append(";");
					retV.Append(faxMails[i].Trim());
				}
			return retV.ToString();
		}


		#endregion

		#region Page edit

		private void textBoxPageTo_Leave(object sender, EventArgs e)
		{
			if(textBoxPageTo.Text.Length > 0)
			{
				string numText = Regex.Replace(textBoxPageTo.Text, "\\D", string.Empty);
				if(numText.Length > 0)
				{
					int start = int.Parse(textBoxPageFrom.Text);
					int end = 0;
					if(!int.TryParse(numText, out end) || end < 1)
					{
						textBoxPageTo.Text = !pdfMode ? imgEdit.PageCount.ToString() : pdfEdit.PageCount.ToString();
						return;
					}
					if(end > (!pdfMode ? imgEdit.PageCount : pdfEdit.PageCount))
						textBoxPageTo.Text = !pdfMode ? imgEdit.PageCount.ToString() : pdfEdit.PageCount.ToString();
					else
					{
						if(end < (pdfMode ? pdfEdit.PageCount : imgEdit.PageCount))
						{
							if(radioButtonMainFormat.Enabled)
							{
								radioButtonMainFormat.Enabled = false;
								if(radioButtonMainFormat.Checked)
								{
									pdfRadio.Checked = pdfMode;
									tifRadio.Checked = !pdfMode;
								}
								groupBoxColor.Enabled = true;
								if(radioButtonColor.Checked == radioButtonBlackWhite.Checked)
									radioButtonColor.Checked = true;
							}
						}
						else
							if(start == 1 && !radioButtonMainFormat.Enabled)
							{
								radioButtonMainFormat.Enabled = pdfMode;
								groupBoxColor.Enabled = !(!tifRadio.Checked && pdfMode);
							}

						textBoxPageTo.Text = numText;
						if(start > end)
							textBoxPageFrom.Text = numText;
					}
				}
				else
					textBoxPageTo.Text = !pdfMode ? imgEdit.PageCount.ToString() : pdfEdit.PageCount.ToString();
			}
			else
				textBoxPageTo.Text = !pdfMode ? imgEdit.PageCount.ToString() : pdfEdit.PageCount.ToString();
		}

		private void textBoxPageFrom_Leave(object sender, EventArgs e)
		{
			if(textBoxPageFrom.Text.Length > 0)
			{
				string numText = Regex.Replace(textBoxPageFrom.Text, "\\D", string.Empty);
				if(numText.Length > 0)
				{
					int start = 0;
					if(!int.TryParse(numText, out start) || start < 1)
					{
						textBoxPageFrom.Text = "1";
						return;
					}
					int end = int.Parse(textBoxPageTo.Text);
					if(start > (!pdfMode ? imgEdit.PageCount : pdfEdit.PageCount))
						textBoxPageFrom.Text = !pdfMode ? imgEdit.PageCount.ToString() : pdfEdit.PageCount.ToString();
					else
					{
						textBoxPageFrom.Text = numText;
						if(start > end)
							textBoxPageTo.Text = numText;
					}

					if(start != 1)
					{
						if(radioButtonMainFormat.Enabled)
						{
							radioButtonMainFormat.Enabled = false;
							if(radioButtonMainFormat.Checked)
							{
								pdfRadio.Checked = pdfMode;
								tifRadio.Checked = !pdfMode;
							}

							groupBoxColor.Enabled = true;
							if(radioButtonColor.Checked == radioButtonBlackWhite.Checked)
								radioButtonColor.Checked = true;
						}
					}
					else
						if(end == (pdfMode ? pdfEdit.PageCount : imgEdit.PageCount) && !radioButtonMainFormat.Enabled)
						{
							radioButtonMainFormat.Enabled = pdfMode;
							groupBoxColor.Enabled = !(!tifRadio.Checked && pdfMode);
						}
				}
				else
					textBoxPageFrom.Text = "1";
			}
			else
				textBoxPageFrom.Text = "1";
		}

		#endregion

		#region Find

		private void FindPersonWeb()
		{
			string perText = string.IsNullOrEmpty(personSearchBlock.SearchText) ? personSearchBlock.FullText : personSearchBlock.SearchText;
			string paramStr = "return=1&clid=27&_personwheresearch=5&_personforsend=1&_personvalidat=" + DateTime.Today.ToString("dd.MM.yyyy");
			if(perText.Length > 0)
				paramStr += "&search=" + HttpUtility.UrlEncode(perText);

			paramStr = paramStr.Replace("+", "%20");
			PersonDialog dialog = new PersonDialog(Environment.PersonSearchString, paramStr);
			dialog.DialogEvent += personDialog_DialogEvent;
			ShowSubForm(dialog);
			Enabled = false;
		}

		private void personDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Enabled = true;
			if(e.Dialog.DialogResult == DialogResult.OK)
			{
				PersonDialog dialog = e.Dialog as PersonDialog;
				dialog.DialogEvent -= personDialog_DialogEvent;
				PersonInfo person = (PersonInfo)dialog.Persons[0];
				AddPersonContact(person.ID);
			}
		}

		#endregion

		#region Person contact

		private void AddPersonContact(int id, string phoneNumber)
		{
			if(id > 0)
			{
				string name = Environment.PersonData.GetFullPerson(id);
				PersonContactDialog dialog;
				if(phoneNumber != null && phoneNumber.Length > 0)
					dialog = new PersonContactDialog(id, name, phoneNumber);
				else
					dialog = new PersonContactDialog(id, name);
				if((personSearchBlock.SearchText.IndexOf('@') >= 0 && Environment.PersonData.PersonType == 2) ||
					dialog.Collection.Count == 1)
				{
					if(dialog.Collection.Count == 1)
						dialog.Collection[0].Checked = true;
					else
					{
						for(int i = 0; i < dialog.Collection.Count; i++)
							if(dialog.Collection[i].Text.IndexOf(personSearchBlock.SearchText) != -1)
							{
								dialog.Collection[i].Checked = true;
								break;
							}
					}
					dialog.DialogResult = DialogResult.OK;
					addPersonContactDialog_DialogEvent(null, new DialogEventArgs(dialog));
				}
				else
				{
					dialog.DialogEvent += addPersonContactDialog_DialogEvent;
					Enabled = false;
					ShowSubForm(dialog);
				}
			}
		}
		private void AddPersonContact(int id)
		{
			AddPersonContact(id, null);
		}

		private void AddContact(DataRow dr, KeyValuePair<int, string> parent)
		{
			AddContact(dr, parent, new KeyValuePair<int, string>(0, null));
		}

		private void AddContact(DataRow dr, KeyValuePair<int, string> parent, KeyValuePair<int, string> child)
		{
			personSearchBlock.Clear();
			Controls.CheckControl cont = new Controls.CheckControl();
			int id = (int)dr[Environment.FaxRecipientData.IDField];
			cont.Name += id.ToString();
			cont.Collection = collection;
			if(parent.Key > 0)
			{
				cont.InsertLink(parent.Value, Environment.PersonURL + parent.Key.ToString());
				if(child.Key > 0)
				{
					cont.SelectedText = "(";
					cont.InsertLink(child.Value, Environment.PersonURL + child.Key.ToString());
					cont.SelectedText = ")";
				}
				cont.SelectedText = ":";
			}
			if(!dr.IsNull(Environment.FaxRecipientData.DescriptionField))
			{
				string res = dr[Environment.FaxRecipientData.DescriptionField].ToString();
				if(!string.IsNullOrEmpty(res))
					cont.SelectedText = res + " ";
			}

			cont.InsertLink(dr[Environment.FaxRecipientData.ContactField].ToString(), "#Edit");
			cont.Tag = dr;
			cont.PersonName = parent.Key > 0 ? parent.Value + (child.Key > 0 ? "(" + child.Value + ")" : string.Empty) : string.Empty;
			cont.ID = id;
			cont.SendType = (int)dr[Environment.FaxRecipientData.CategoryField];
			cont.ShowDelete = true;
			cont.ShowCheckBox = false;
			collection.Add(cont);
			UpdateButtons();
		}

		private void AddContact(int sendType, string senderName, string contactAddress)
		{
			personSearchBlock.Clear();
			Controls.CheckControl cont = new Controls.CheckControl
									{
										Collection = collection,
										Text =
											senderName + ((sendType == 2) ? " (e-mail): " : " (Fax): ") + contactAddress,
										Tag = new object[] { senderName, contactAddress },
										ID = 0,
										SendType = sendType,
										ShowDelete = true,
										ShowCheckBox = false
									};
			collection.Add(cont);
			cont.Name += contactAddress;
			UpdateButtons();
		}

		private void SelectSenderDialogEmail_DialogEvent(object source, DialogEventArgs e)
		{
			if(!keyLocker.Contains(Keys.Enter))
			{
				keyLocker.Add(Keys.Enter);
				try
				{
					Enabled = true;
					Select.SelectSenderDialog dialog = e.Dialog as Select.SelectSenderDialog;
					if(dialog != null)
					{
						switch(dialog.DialogResult)
						{
							case DialogResult.OK:
								{
									ContactDialog ccDialog = new ContactDialog(Environment.CreateContactString, "personContactCategor=4&personContactText=" + personSearchBlock.SearchText + "&docview=yes");
									ccDialog.Owner = this;
									ccDialog.DialogEvent += CreateContactDialog_DialogEvent;
									ccDialog.Show();
									Enabled = false;
									return;
								}
							default:
								if(dialog.DialogResult != DialogResult.Yes)
								{
									keyLocker.Remove(Keys.Enter);
									return;
								}
								break;
						}
						AddContact(2, dialog.SenderText, dialog.ContactString);
					}
				}
				catch
				{
				}
				finally
				{
					keyLocker.Remove(Keys.Enter);
				}
			}
		}

		private void SelectSenderDialogFax_DialogEvent(object source, DialogEventArgs e)
		{
			if(!keyLocker.Contains(Keys.Enter))
				keyLocker.Add(Keys.Enter);

			Enabled = true;
			Select.SelectSenderDialog dialog = e.Dialog as Select.SelectSenderDialog;
			if(dialog.DialogResult == DialogResult.OK)
			{
				ContactDialog ccDialog = new ContactDialog(Environment.CreateContactString, "personContactCategor=3&personContactText=" + personSearchBlock.SearchText + "&docview=yes");
				ccDialog.Owner = this;
				ccDialog.DialogEvent += CreateContactDialog_DialogEvent;
				ccDialog.Show();
				Enabled = false;
				return;
			}
			else
				if(dialog.DialogResult != DialogResult.Yes)
				{
					keyLocker.Remove(Keys.Enter);
					return;
				}
			AddContact(1, dialog.SenderText, dialog.ContactString);
			keyLocker.Remove(Keys.Enter);
		}

		#endregion

		private void personSearchBlock_FindPerson(object sender, EventArgs e)
		{
			AddPersonContact(personSearchBlock.PersonID);
		}

		private void personSearchBlock_FindEMail(object sender, EventArgs e)
		{
			StartContactWork(4, false);
		}

		private void personSearchBlock_FindFax(object sender, EventArgs e)
		{
			StartContactWork(3, false);
		}

		public void StartContactWork(int typeID, bool create)
		{
			string cont = personSearchBlock.SearchText;
			if(typeID == 3 && !cont.StartsWith("+"))
			{
				String name = null;
				if(SystemInformation.TerminalServerSession)
				{
					uint bytesReturned;

					try
					{
						bool sessionInfo = wtsapi32.GetWTSQuerySessionInformation(IntPtr.Zero, wtsapi32.WTS_CURRENT_SESSION, wtsapi32.WTSInfoClass.WTSClientName, out name, out bytesReturned);
					}
					catch
					{

					}

				}
				else
					name = SystemInformation.ComputerName;
				cont = "+" + Environment.PhoneData.GetInternationalNumber(cont, name);
				if(string.IsNullOrEmpty(cont) || cont == "+")
				{
					FindPersonWeb();
					return;
				}
			}
			if(!create)
			{
				Select.SelectSenderDialog dialog = new Select.SelectSenderDialog(cont);
				if(typeID == 4)
				{
					dialog.DialogEvent += SelectSenderDialogEmail_DialogEvent;
				}
				else
				{
					dialog.DialogEvent += SelectSenderDialogFax_DialogEvent;
				}
				dialog.Owner = this;
				dialog.Show();
				Enabled = false;
			}
			else
			{
				ContactDialog dialog = new ContactDialog(Environment.CreateContactString, "personContactCategor=" + typeID.ToString() + "&personContactText=" + cont + "&docview=yes");
				dialog.DialogEvent += CreateContactDialog_DialogEvent;
				dialog.Show();
				Enabled = false;
			}
		}

		private void CreateContactDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Enabled = true;
			ContactDialog dialog = e.Dialog as ContactDialog;
			dialog.DialogEvent -= CreateContactDialog_DialogEvent;
			if(dialog.DialogResult == DialogResult.OK)
			{
				if(dialog.ContactID > 0)
				{
					DataRow dr = Environment.FaxRecipientData.GetPersonContact(dialog.ContactID);
					if(dr != null)
					{
						if(!dr.IsNull(Environment.PersonData.IDField))
							AddContact(dr, new KeyValuePair<int, string>((int)dr[Environment.PersonData.IDField], Environment.PersonData.GetPerson((int)dr[Environment.PersonData.IDField])));
						else
							AddContact(dr, new KeyValuePair<int, string>());
					}
					else
						MessageForm.Show(Environment.StringResources.GetString("Dialog_SendFaxDialog_SendFax_Error2"), Environment.StringResources.GetString("Error"));
				}
			}
		}

		private void personSearchBlock_FindNothing(object sender, EventArgs e)
		{
			FindPersonWeb();
		}

		private void textBoxText_Leave(object sender, EventArgs e)
		{
			textBoxText.Text = textBoxText.Text.Trim();
			if(textBoxText.Text.Length > 200)
				textBoxText.Text = textBoxText.Text.Remove(200, textBoxText.Text.Length - 200);
		}

		private void personSearchBlock_FindInternal(object sender, EventArgs e)
		{
			AddContact(1, string.Empty, personSearchBlock.SearchText);
		}

		private void personSearchBlock_CreateEmailContact(object sender, EventArgs e)
		{
			StartContactWork(4, true);
		}

		private void personSearchBlock_CreateFaxContact(object sender, EventArgs e)
		{
			StartContactWork(3, true);
		}

		private void personSearchBlock_AddContact(object sender, EventArgs e)
		{
			if(personSearchBlock.IsFax)
				StartContactWork(3, true);
			else
				AddContact(2, string.Empty, personSearchBlock.SearchText);
		}

		private void personSearchBlock_CreateClient(object sender, EventArgs e)
		{
			UrlBrowseDialog urlDialog = new UrlBrowseDialog(Environment.CreateClientString + personSearchBlock.SearchText, Environment.StringResources.GetString("Dialog_SendFaxDialog_SendFax_Error3"));
			urlDialog.DialogEvent += urlCreateClientDialog_DialogEvent;
			urlDialog.Show();
			Enabled = false;
		}

		private void personSearchBlock_CreateClientPerson(object sender, EventArgs e)
		{
			UrlBrowseDialog urlDialog = new UrlBrowseDialog(Environment.CreateClientPersonString + personSearchBlock.SearchText, Environment.StringResources.GetString("Dialog_SendFaxDialog_SendFax_Error4"));
			urlDialog.DialogEvent += urlCreateClientDialog_DialogEvent;
			urlDialog.Show();
			Enabled = false;
		}

		private void urlCreateClientDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Enabled = true;
			if(e.Dialog.DialogResult == DialogResult.OK)
			{
				UrlBrowseDialog urlDialog = e.Dialog as UrlBrowseDialog;
				urlDialog.DialogEvent -= urlCreateClientDialog_DialogEvent;
				Web.Cookie retVal = urlDialog.Collection.GetCookie("RetVal");
				if((retVal != null) && (retVal.Value != null))
				{
					int personID;
					string retstr = retVal.Value;
					try
					{
						personID = int.Parse(retstr.Substring(0, retstr.IndexOf("%")));
					}
					catch
					{
						personID = 0;
					}
					if(personID > 0)
						AddPersonContact(personID);
				}
			}
		}

		private void SendFaxDialog_Closed(object sender, EventArgs e)
		{
			if(subLayout != null)
			{
				subLayout.Option("Width").Value = Width;
				subLayout.Option("Height").Value = Height;
				subLayout.Save();
			}
		}

		private void radioButtonMainFormat_CheckedChanged(object sender, EventArgs e)
		{
			groupBoxColor.Enabled = !radioButtonMainFormat.Checked;
			if(groupBoxColor.Enabled && radioButtonColor.Checked == radioButtonBlackWhite.Checked)
				radioButtonColor.Checked = true;
		}

		private void pdfRadio_CheckedChanged(object sender, EventArgs e)
		{
			groupBoxColor.Enabled = !(pdfRadio.Checked && pdfMode);
			if(groupBoxColor.Enabled && radioButtonColor.Checked == radioButtonBlackWhite.Checked)
				radioButtonColor.Checked = true;
		}

		private void tifRadio_CheckedChanged(object sender, EventArgs e)
		{
			checkBoxWithNotes.Visible = checkBoxToImage.Visible = tifRadio.Checked && !pdfMode;
		}

		#region LoadBunchOfDocs

		private void BunchOfDocsLoaded(bool useOriginalFormat)
		{
			StringBuilder errorDocs = new StringBuilder();
			for(int i = docs.Count - 1; i > -1; --i)
				if(docs[i].Error)
				{
					errorDocs.Append("\n" + docs[i].FileNameToSend);
					docs.RemoveAt(i);
				}
				else
					description += " " + docs[i].FileNameToSend;

			if(docs.Count == 0)
			{
				MessageBox.Show("There is no documents to send!");
				Close();
			}
			else if(docs.Count == 1)
			{
				(new SendFaxDialog(docs[0].SourceFileName, docs[0].FileNameToSend, docs[0].DocID, docs[0].ImageID, Environment.IsPdf(docs[0].SourceFileName))).Show();
				Close();
			}
			else
			{
				useOriginalFormat = false;

				DoubleBuffered = true;
				InitializeComponent();

				subLayout = Environment.Layout.Folders.Add(Name);
				Width = subLayout.LoadIntOption("Width", Width);
				Height = subLayout.LoadIntOption("Height", Height);
				docImageID = -1;

				radioButtonMainFormat.Enabled = false;
				radioButtonMainFormat.Checked = false;
				tifRadio.Checked = true;
				groupBoxColor.Enabled = true;

				textBoxPageFrom.Text = "";
				textBoxPageTo.Text = "";
				textBoxPageFrom.Enabled = false;
				textBoxPageTo.Enabled = false;

				collection = new CheckedControlCollection(panelControl);
				collection.XOrigin = -32;
				collection.Checked += collection_Checked;

				for(int i = 0; i < docs.Count; i++)
				{
					if(docs[i].DocID > 0)
						newWindowDocumentButton.LoopSet(docs[i].DocID, docs[i].FileNameToSend);
					else
						newWindowDocumentButton.LoopSet(docs[i].SourceFileName, docs[i].FileNameToSend);
				}
				newWindowDocumentButton.Verify();

				Show();

				if(errorDocs.Length > 0)
					MessageBox.Show(Environment.StringResources.GetString("Dialogs_SendFaxDialog_ImpossibleForDoc") + errorDocs);
			}
		}

		#endregion

		#region SendBunchOfDocs

		private void SendBunchOfDocsMail(List<KeyValuePair<string, string>> mails, string descr)
		{
			int startPage = 1;
			if(!int.TryParse(textBoxPageFrom.Text, out startPage))
				startPage = 1;

			int endPage = -1;
			if(!int.TryParse(textBoxPageTo.Text, out endPage))
				endPage = -1;

			if(endPage != -1 && endPage < startPage)
				return;

			Cursor = Cursors.WaitCursor;

			if(processingFiles == null)
				processingFiles = new SynchronizedCollection<int>();

			for(int i = docs.Count - 1; i > -1; --i)
			{
				int code = docs[i].SourceFileName.GetHashCode() + i;
				Slave.DoWork(SendBunchOfDocs, new object[] { i, pdfRadio.Checked, radioButtonMainFormat.Checked, startPage, endPage, checkBoxWithNotes.Checked, radioButtonColor.Checked, checkBoxToImage.Checked, code });
				processingFiles.Add(code);
			}

			SendBunchOfDocsOutlook(mails, descr);
		}

		private void SendBunchOfDocs(object sender, DoWorkEventArgs e)
		{
			System.Windows.Forms.Application.DoEvents();

			int _imgID = docs[(int)((object[])e.Argument)[0]].ImageID;
			string _fileName = docs[(int)((object[])e.Argument)[0]].SourceFileName;
			bool _isPdfFormat = (bool)((object[])e.Argument)[1];
			bool _isOriginal = (bool)((object[])e.Argument)[2];
			int _startPage = (int)((object[])e.Argument)[3];
			int _endPage = (int)((object[])e.Argument)[4];
			bool _leaveAnnotation = (bool)((object[])e.Argument)[5];
			bool _color = (bool)((object[])e.Argument)[6];
			bool _burnAnnotation = (bool)((object[])e.Argument)[7];
			int _code = (int)((object[])e.Argument)[8];
			string _extension = Path.GetExtension(_fileName).TrimStart('.');
			bool _pdfMode = Environment.IsPdf(_fileName);

			if(_startPage > 1 || _endPage > -1)
			{
				if(_isOriginal)
				{
					_isOriginal = false;
					_isPdfFormat = Environment.IsPdf(_fileName);
				}
			}

			if(!_isOriginal)
				_extension = _isPdfFormat ? "pdf" : "tif";

			string _newFileName = Path.Combine(Path.GetTempPath(), Environment.GenerateFileName(_extension));
			while(File.Exists(_newFileName))
			{ _newFileName = Path.Combine(Path.GetTempPath(), Environment.GenerateFileName(_extension)); }

			try
			{
				if(_isOriginal)
					File.Copy(_fileName, _newFileName);
				else
				{
					if(_pdfMode)
					{
						using(Controls.PdfViewControl.PDFView _pdfEdit = new Controls.PdfViewControl.PDFView())
						{
							_pdfEdit.NeedPreview = false;

							if(_imgID > 0)
								_pdfEdit.ImageID = _imgID;
							else
								_pdfEdit.FileName = _fileName;

							if(_endPage == -1)
								_endPage = _pdfEdit.PageCount;
							if(_endPage <= _pdfEdit.PageCount && _startPage > 0 && _endPage >= _startPage)
								_pdfEdit.SaveWithBurnAndResolution(_startPage, _endPage, _newFileName, 300, 300, _color, !_isPdfFormat);

							try
							{
								_pdfEdit.Dispose();
							}
							catch { }
						}
					}
					else
					{
						using(Controls.ImageShowControl _imgEdit = new Controls.ImageShowControl())
						{
							_imgEdit.AnnotationDraw = false;
							_imgEdit.IsCorrectScaleDrawThumbnailPanel = true;
							_imgEdit.ShowThumbPanel = false;
							_imgEdit.SplinterPlace = 4;
							_imgEdit.TypeWorkThumbnailImagesPanel = 3;
							_imgEdit.UseLock = false;
							_imgEdit.Zoom = 100;

							if(_imgID > 0)
								_imgEdit.ImageID = _imgID;
							else
								_imgEdit.FileName = _fileName;

							anotherFormat = _imgEdit.AnotherFormat;

							if(_endPage == -1)
								_endPage = _imgEdit.PageCount;
							if(_endPage <= _imgEdit.PageCount && _startPage > 0 && _endPage >= _startPage)
							{
								_imgEdit.SaveWithBurnAndResolution(_startPage - 1, _endPage - 1, _newFileName, 300, 300, _leaveAnnotation, _color, _burnAnnotation);

								if(_isPdfFormat)
								{
									LibTiffHelper libTiff = new LibTiffHelper();
									List<Tiff.PageInfo> il = libTiff.GetPagesCollectionFromFile(_newFileName, 0, -1, false);

									Slave.DeleteFile(_newFileName);

									do { _newFileName = Path.Combine(Path.GetTempPath(), Environment.GenerateFileName(_extension)); }
									while(File.Exists(_newFileName));

									ConvertAndInsertClass.Convert(il, _newFileName);

									libTiff.Dispose();
									il.Clear();
								}
							}
							try
							{
								_imgEdit.Dispose();
							}
							catch { }
						}
					}
				}
			}
			finally
			{
				processingFiles.Remove(_code);
				docs[(int)((object[])e.Argument)[0]].SendFileName = _newFileName;
			}
		}

		private void SendBunchOfDocsOutlook(List<KeyValuePair<string, string>> mails, string descr)
		{
			Outlook.Application ol = null;
			Outlook.NameSpace myNameSpace = null;

			try
			{
				ol = new Outlook.Application();
			}
			catch
			{
				ol = null;
			}

			int version = 0;
			if(ol != null && ol.Version.IndexOf('.') > 0 && int.TryParse(ol.Version.Substring(0, ol.Version.IndexOf('.')), out version) && version > 10)
			{
				try
				{
					myNameSpace = ol.GetNamespace("MAPI");
					object myMissing = Missing.Value;
					myNameSpace.Logon(myMissing, myMissing, true, myMissing);

					Outlook.MailItem item;
					object obj = ol.CreateItem(Outlook.OlItemType.olMailItem);
					item = (Outlook.MailItem)obj;

					item.Subject = descr;
					string mail = string.Empty;
					for(int i = 0; i < mails.Count; i++)
					{
						if(mail.Length > 0)
							mail += ";";
						KeyValuePair<string, string> mailIntry = mails[i];
						mail += EmailCreate(mailIntry.Key, mailIntry.Value, ol.Version.StartsWith("9."));
					}
					item.To = mail;

					while(processingFiles.Count > 0)
					{ System.Windows.Forms.Application.DoEvents(); }

					for(int i = docs.Count - 1; i > -1; --i)
					{
						bool tryUglyName = false;
						try
						{
							string exte = Path.GetExtension(docs[i].SendFileName);
							string sendDocFileName = Path.Combine(Path.GetTempPath(), docs[i].FileNameToSend + exte);
							if(File.Exists(sendDocFileName))
								File.Delete(sendDocFileName);
							File.Move(docs[i].SendFileName, sendDocFileName);
							item.Attachments.Add(sendDocFileName, 1, 1, docs[i].FileNameToSend + exte);
							Slave.DeleteFile(sendDocFileName);
						}
						catch
						{
							tryUglyName = true;
						}

						if(tryUglyName)
							try
							{
								tryUglyName = false;
								string exte = Path.GetExtension(docs[i].SendFileName);
								string sendDocFileName = Path.Combine(Path.GetTempPath(), "Document(" + i.ToString() + ")" + exte);
								if(File.Exists(sendDocFileName))
									File.Delete(sendDocFileName);
								File.Move(docs[i].SendFileName, sendDocFileName);
								item.Attachments.Add(sendDocFileName, 1, 1, docs[i].FileNameToSend + exte);
								Slave.DeleteFile(sendDocFileName);
							}
							catch
							{
								tryUglyName = true;
							}

						if(tryUglyName)
							try
							{
								item.Attachments.Add(docs[i].SendFileName, 1, 1, docs[i].SendFileName);
							}
							catch(System.Exception ex)
							{
								Data.Env.WriteToLog(ex);
								MessageForm.Show(Environment.StringResources.GetString("Dialogs_SendFaxDialog_FileAttachError") + "\n" + docs[i].FileNameToSend);
							}

					}
					item.Display(false);
					Marshal.ReleaseComObject(item);
					item = null;
				}
				catch(System.Exception ex)
				{
					Data.Env.WriteToLog(ex);
					Error.ErrorShower.OnShowError(this, ex.Message, "");
				}
				finally
				{
					if(ol != null)
					{
						Marshal.ReleaseComObject(ol);
						ol = null;
					}
					if(myNameSpace != null)
					{
						myNameSpace.Logoff();
						Marshal.ReleaseComObject(myNameSpace);
						myNameSpace = null;
					}
					Cursor = Cursors.Default;
				}
			}
			else
			{
				if(!string.IsNullOrEmpty(Environment.FullExchangeServerEwsUrl))
				{
					EWSMail.ExchangeServiceBinding esb = new EWSMail.ExchangeServiceBinding();
					esb.Credentials = CredentialCache.DefaultNetworkCredentials;
					esb.Url = Environment.FullExchangeServerEwsUrl;
					esb.RequestServerVersionValue = new EWSMail.RequestServerVersion();
					esb.RequestServerVersionValue.Version = EWSMail.ExchangeVersionType.Exchange2010_SP2;
					EWSMail.TargetFolderIdType tfid = null;
					string folder = null;
					try
					{
						EWSMail.GetFolderResponseType gfo = esb.GetFolder(new EWSMail.GetFolderType
																	  {
																		  FolderIds = new EWSMail.DistinguishedFolderIdType[1] 
															{	
																new EWSMail.DistinguishedFolderIdType
																{
																	Id = EWSMail.DistinguishedFolderIdNameType.drafts, 
																	Mailbox = new EWSMail.EmailAddressType {EmailAddress = Environment.MailBoxName}
																}
															},
																		  FolderShape = new EWSMail.FolderResponseShapeType
																				{
																					BaseShape = EWSMail.DefaultShapeNamesType.Default
																				}

																	  });
						if(gfo != null && gfo.ResponseMessages.Items.Length > 0 && gfo.ResponseMessages.Items[0].ResponseClass == EWSMail.ResponseClassType.Success)
						{
							EWSMail.FolderInfoResponseMessageType fldr = gfo.ResponseMessages.Items[0] as EWSMail.FolderInfoResponseMessageType;
							if(fldr != null && fldr.Folders.Length > 0)
							{
								tfid = new EWSMail.TargetFolderIdType { Item = fldr.Folders[0].FolderId };
								folder = fldr.Folders[0].DisplayName.Trim();
							}
						}
						if(tfid == null || string.IsNullOrEmpty(folder))
							throw new NullReferenceException("Not find temp folder");
					}
					catch(System.Exception ex)
					{
						Data.Env.WriteToLog(ex);
						Error.ErrorShower.OnShowError(this, ex.Message, "");
						return;
					}

					EWSMail.CreateItemType cit = new EWSMail.CreateItemType
							{
								Items = new EWSMail.NonEmptyArrayOfAllItemsType(),
								MessageDispositionSpecified = true,
								SavedItemFolderId = tfid,
								MessageDisposition = EWSMail.MessageDispositionType.SaveOnly
							};

					EWSMail.MessageType mt = new EWSMail.MessageType();
					mt.From = new EWSMail.SingleRecipientType { Item = new EWSMail.EmailAddressType { EmailAddress = Environment.MailBoxName } };
					mt.ToRecipients = new EWSMail.EmailAddressType[mails.Count];
					for(int j = 0; j < mails.Count; j++)
					{
						KeyValuePair<string, string> mailIntry = mails[j];
						string[ ] emails = Regex.Split(mailIntry.Value.Trim(), "[^-+._a-z0-9@]", RegexOptions.IgnoreCase);
						EWSMail.EmailAddressType mat = new EWSMail.EmailAddressType();
						if(emails.Length == 1 && !string.IsNullOrEmpty(mailIntry.Key))
							mat.Name = mailIntry.Key;
						StringBuilder sb = new StringBuilder();
						for(int k = 0; k < emails.Length; k++)
						{
							if(!string.IsNullOrEmpty(emails[k]))
							{
								sb.Append(emails[k]);
								sb.Append(";");
							}
						}
						mat.EmailAddress = sb.ToString().TrimEnd(';');
						mt.ToRecipients[j] = mat;
					}
					mt.Subject = descr;

					cit.Items.Items = new EWSMail.ItemType[1];
					cit.Items.Items[0] = mt;

					EWSMail.AttachmentType[ ] attachments = new EWSMail.AttachmentType[docs.Count];

					while(processingFiles.Count > 0)
					{ System.Windows.Forms.Application.DoEvents(); }
					long size = 0;

					for(int i = docs.Count - 1; i > -1; --i)
					{
						try
						{
							byte[ ] binaryData;
							using(FileStream inFile = new FileStream(docs[i].SendFileName, FileMode.Open, FileAccess.Read))
							{
								if(size > -1)
									size += inFile.Length;
								if(size > 26214400)
								{
									MessageForm mf = new MessageForm("Размер вложения превышает 25 Мегабайт.\nПисьмо может быть не отправлено из-за большого размера вложения\nОтправить сообщение?", Environment.StringResources.GetString("Comformation"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
									if(mf.ShowDialog() != System.Windows.Forms.DialogResult.Yes)
									{
										for(int k = docs.Count - 1; k > -1; --k)
										{
											Slave.DeleteFile(docs[k].SendFileName);
											docs[k].SendFileName = null;
										}
										inFile.Close();
										Cursor = Cursors.Default;
										buttonSend.Enabled = true;
										return;
									}
									else
										size = -1;
								}

								binaryData = new Byte[inFile.Length];
								inFile.Read(binaryData, 0, (int)inFile.Length);
								inFile.Close();
							}
							EWSMail.FileAttachmentType fat = new EWSMail.FileAttachmentType();
							fat.Content = binaryData;
							if(Path.GetExtension(docs[i].SendFileName).ToLower().Equals(".pdf"))
								fat.ContentType = "application/pdf";
							else
								fat.ContentType = "image/tiff";
							fat.Name = docs[i].FileNameToSend + Path.GetExtension(docs[i].SendFileName);

							attachments[i] = fat;
						}
						catch(System.Exception ex)
						{
							Data.Env.WriteToLog(ex);
							Error.ErrorShower.OnShowError(this, Environment.StringResources.GetString("Dialogs_SendFaxDialog_FileAttachError") + "\n" + docs[i].FileNameToSend, "");
						}
					}

					try
					{
						EWSMail.CreateItemResponseType crit = esb.CreateItem(cit);
						if(crit == null || crit.ResponseMessages.Items.Length == 0 || crit.ResponseMessages.Items[0].ResponseClass != EWSMail.ResponseClassType.Success)
							throw new System.Exception("Can't save message");
						EWSMail.ItemIdType iid = ((EWSMail.ItemInfoResponseMessageType)(((crit)).ResponseMessages.Items[0])).Items.Items[0].ItemId;
						EWSMail.CreateAttachmentResponseType cart = esb.CreateAttachment
						(
							new EWSMail.CreateAttachmentType
								{
									Attachments = attachments,
									ParentItemId = iid
								});

						if(cart == null || cart.ResponseMessages.Items.Length == 0 || cart.ResponseMessages.Items[0].ResponseClass != EWSMail.ResponseClassType.Success)
						{
							esb.DeleteItem(new EWSMail.DeleteItemType { ItemIds = new EWSMail.BaseItemIdType[1] { iid } });
							throw new System.Exception("Can't create attachment");
						}
						EWSMail.GetItemResponseType gir = esb.GetItem(
						new EWSMail.GetItemType
							{
								ItemIds = new EWSMail.BaseItemIdType[1] { iid },
								ItemShape = new EWSMail.ItemResponseShapeType
									{
										BaseShape = EWSMail.DefaultShapeNamesType.IdOnly,
										AdditionalProperties = new EWSMail.BasePathToElementType[1]
										{
											new EWSMail.PathToUnindexedFieldType
											{
												FieldURI = EWSMail.UnindexedFieldURIType.itemWebClientEditFormQueryString
											}
										}
									}
							});

						if(gir != null && gir.ResponseMessages.Items.Length > 0 && gir.ResponseMessages.Items[0].ResponseClass == EWSMail.ResponseClassType.Success)
							Environment.IEOpenOnURL(Environment.FullExchangeServerOwaUrl + Environment.MailBoxName + "/" + (((EWSMail.ItemInfoResponseMessageType)(((gir)).ResponseMessages.Items[0])).Items.Items[0]).WebClientEditFormQueryString);
					}
					catch(System.Exception ex)
					{
						for(int k = docs.Count - 1; k > -1; --k)
						{
							Slave.DeleteFile(docs[k].SendFileName);
							docs[k].SendFileName = null;
						}
						if(!ex.Message.Contains(" Insufficient Storage"))
						{
							Data.Env.WriteToLog(ex);
							Error.ErrorShower.OnShowError(this, ex.Message, "");
						}
						else
							Error.ErrorShower.OnShowError(this, "Не удалось присоеденить документ. Превышен объём сообщения или или не доступен почтовый ящик.", Environment.StringResources.GetString("Error"));
						return;
					}
					finally
					{
						Cursor = Cursors.Default;
						buttonSend.Enabled = true;
					}
				}
			}
			for(int i = docs.Count - 1; i > -1; --i)
			{
				Slave.DeleteFile(docs[i].SendFileName);
				docs.RemoveAt(i);
			}

			AllSendsCompleted();
		}

		#endregion

		#region SendBunchOfFaxes

		private void SendBunchOfFaxes(string recipient, string faxMail, string descr, int count)
		{
			int startPage = 1;
			if(!int.TryParse(textBoxPageFrom.Text, out startPage))
				startPage = 1;

			int endPage = -1;
			if(!int.TryParse(textBoxPageTo.Text, out endPage))
				endPage = -1;

			if(endPage != -1 && endPage < startPage)
				return;

			Cursor = Cursors.WaitCursor;

			if(processingFiles == null)
				processingFiles = new SynchronizedCollection<int>();

			for(int i = docs.Count - 1; i > -1; --i)
			{
				int code = string.Concat(recipient, docs[i].SourceFileName).GetHashCode() + i;
				Slave.DoWork(SendBunchOfFaxesDoWork, new object[ ] { docs[i].SourceFileName, docs[i].FaxID, startPage, endPage, recipient, faxMail, descr, code });
				processingFiles.Add(code);
			}

			AllSendsCompleted();
		}

		private void SendBunchOfFaxesDoWork(object sender, DoWorkEventArgs e)
		{
			string _fileName = (string)((object[])e.Argument)[0];
			int _faxID = (int)((object[])e.Argument)[1];
			int _startPage = (int)((object[])e.Argument)[2];
			int _endPage = (int)((object[])e.Argument)[3];
			string _recipient = (string)((object[])e.Argument)[4];
			string _faxMail = (string)((object[])e.Argument)[5];
			string _descr = (string)((object[])e.Argument)[6];
			int _code = (int)((object[])e.Argument)[7];

			ServerInfo _server = Environment.GetRandomLocalServer();
			string _name = Environment.GenerateFileName("tif");
			string _newFileName = _server.Path + "\\temp\\" + _name;
			while(File.Exists(_newFileName))
			{
				_name = Environment.GenerateFileName("tif");
				_newFileName = _server.Path + "\\temp\\" + _name;
			}

			try
			{
				if(Environment.IsPdf(_fileName))
				{
					using(Controls.PdfViewControl.PDFView _pdfEdit = new Controls.PdfViewControl.PDFView())
					{
						_pdfEdit.NeedPreview = false;
						_pdfEdit.FileName = _fileName;

						if(_endPage == -1)
							_endPage = _pdfEdit.PageCount;
						if(_endPage <= _pdfEdit.PageCount && _startPage > 0 && _endPage >= _startPage)
							_pdfEdit.SaveWithBurnAndResolution(_startPage - 1, _endPage - 1, _newFileName, 204, 196, false, true);

						try
						{
							_pdfEdit.Dispose();
						}
						catch { }
					}
				}
				else
				{
					using(Controls.ImageShowControl imgEdit = new Controls.ImageShowControl())
					{
						imgEdit.AnnotationDraw = false;
						imgEdit.IsCorrectScaleDrawThumbnailPanel = true;
						imgEdit.ShowThumbPanel = false;
						imgEdit.SplinterPlace = 4;
						imgEdit.TypeWorkThumbnailImagesPanel = 3;
						imgEdit.UseLock = false;
						imgEdit.Zoom = 100;

						imgEdit.FileName = _fileName;
						anotherFormat = imgEdit.AnotherFormat;

						if(_endPage == -1)
							_endPage = imgEdit.PageCount;
						if(_endPage <= imgEdit.PageCount && _startPage > 0 && _endPage >= _startPage)
							imgEdit.SaveWithBurnAndResolution(_startPage - 1, _endPage - 1, _newFileName, 204, 196, false, false, false);

						try
						{
							imgEdit.Dispose();
						}
						catch { }
					}
				}

				// send fax
				if(Environment.FaxData.SendFax(_server.ID, _recipient, _faxMail, _descr, _name, -1))
				{
					if(_faxID > 0)
						Environment.FaxOutData.MarkResend(faxID);
				}
				else
				{
					MessageBox.Show(Environment.StringResources.GetString("Dialog_SendFaxDialog_SendFax_Error1"), Environment.StringResources.GetString("Error"));
				}
			}
			finally
			{
				processingFiles.Remove(_code);
			}
		}

		private void AllSendsCompleted()
		{
			do
			{
				System.Windows.Forms.Application.DoEvents();
			}
			while(processingFiles.Count > 0);

			if(processingFiles.Count == 0 && (docs == null || docs.Count == 0))
				BeginInvoke(new Action<DialogResult>(End), DialogResult.OK);
		}

		#endregion

		private void SendFaxDialog_KeyUp(object sender, KeyEventArgs e)
		{
			newWindowDocumentButton.ProcessKey(e.KeyData);
		}
	}
}