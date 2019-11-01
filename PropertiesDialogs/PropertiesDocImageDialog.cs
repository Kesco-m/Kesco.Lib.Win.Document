using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Controls;
using Kesco.Lib.Win.Trees;

namespace Kesco.Lib.Win.Document.PropertiesDialogs
{
	public class PropertiesDocImageDialog : FreeDialog
	{
		private string fileName = "";
		private int docID;
		private int archiveID;
		private bool mainImage;
		private int printedID;
		private DateTime date;
		private string imageType = "TIF";

		private CheckBox original;
		private Label archiveLabel;
		private Button buttonSelectArchive;
		private TextBox archive;
		private Button buttonOK;
		private Button buttonCancel;
		private Label labelDate;
		private Label labelMain;
		private GroupBox groupBoxMainImage;
		private Button buttonMakeMain;
		private Label labelDoc;
		private GroupBox groupBoxArchives;
		private GroupBox groupBoxServers;
		private ListView servers;
		private Panel panelDoc;
		private Panel panel1;
		private GroupBox groupSend;
		private ListView sended;
		private Label labelEditor;
		private Label labelEdit;
		private Button buttonFileProperties;
		private GroupBox groupBoxCurrentImage;
		private Label labelColorType;
		private Label labelResolution;
		private Label labelSize;
		private Label labelCreator;
		private IContainer components;

		HoverLinkLabel linkCreator;
		private NewWindowDocumentButton newWindowDocumentButton;
		private Label archiveEditLabel;
		private Label archiveEditorLabel;
		HoverLinkLabel linkEditor;
		HoverLinkLabel linkArchiveEditor;

        /// <summary>
        /// Текущее место хранения
        /// </summary>
        private int _originalArchiveId;

		public PropertiesDocImageDialog()
		{
			InitializeComponent();
			buttonFileProperties.Visible = false;

			linkCreator = new HoverLinkLabel(this)
			{
				AutoSize = true,
				Location = new Point(labelCreator.Right, labelCreator.Location.Y)
			};
			linkEditor = new HoverLinkLabel(this)
			{
				AutoSize = true,
				Location = new Point(labelEditor.Right, labelEditor.Location.Y)
			};
			linkArchiveEditor = new HoverLinkLabel(this)
			{
				AutoSize = true,
				Location = new Point(archiveEditorLabel.Right, archiveEditorLabel.Location.Y),
				Anchor = AnchorStyles.Top | AnchorStyles.Right
			};
			Controls.Add(linkCreator);
			Controls.Add(linkEditor);
			groupBoxArchives.Controls.Add(linkArchiveEditor);
		}

		public PropertiesDocImageDialog(int imageID, int docID) : this()
		{
			this.ImageID = imageID;
			this.docID = docID;

			newWindowDocumentButton.Set(this.docID);
		}

		public PropertiesDocImageDialog(string fileName, int docID) : this()
		{
			this.fileName = fileName;
			this.docID = docID;

			newWindowDocumentButton.Set(this.docID);
		}

		#region Accessors

		public int ImageID { get; private set; }

		public int ResolutionX { get; set; }

		public int ResolutionY { get; set; }

		public int PixelWigth { get; set; }

		public int PixelHeight { get; set; }

		public int VerticalSize { get; set; }

		public int HorizontalSize { get; set; }

		public string ColorType;

		#endregion

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesDocImageDialog));
			this.labelDate = new System.Windows.Forms.Label();
			this.groupBoxArchives = new System.Windows.Forms.GroupBox();
			this.archiveEditLabel = new System.Windows.Forms.Label();
			this.archiveEditorLabel = new System.Windows.Forms.Label();
			this.original = new System.Windows.Forms.CheckBox();
			this.archiveLabel = new System.Windows.Forms.Label();
			this.buttonSelectArchive = new System.Windows.Forms.Button();
			this.archive = new System.Windows.Forms.TextBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelMain = new System.Windows.Forms.Label();
			this.groupBoxMainImage = new System.Windows.Forms.GroupBox();
			this.buttonMakeMain = new System.Windows.Forms.Button();
			this.labelDoc = new System.Windows.Forms.Label();
			this.groupBoxServers = new System.Windows.Forms.GroupBox();
			this.servers = new System.Windows.Forms.ListView();
			this.panelDoc = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupSend = new System.Windows.Forms.GroupBox();
			this.sended = new System.Windows.Forms.ListView();
			this.labelEditor = new System.Windows.Forms.Label();
			this.labelEdit = new System.Windows.Forms.Label();
			this.buttonFileProperties = new System.Windows.Forms.Button();
			this.groupBoxCurrentImage = new System.Windows.Forms.GroupBox();
			this.labelResolution = new System.Windows.Forms.Label();
			this.labelSize = new System.Windows.Forms.Label();
			this.labelColorType = new System.Windows.Forms.Label();
			this.labelCreator = new System.Windows.Forms.Label();
			this.newWindowDocumentButton = new NewWindowDocumentButton(this.components);
			this.groupBoxArchives.SuspendLayout();
			this.groupBoxMainImage.SuspendLayout();
			this.groupBoxServers.SuspendLayout();
			this.panelDoc.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupSend.SuspendLayout();
			this.groupBoxCurrentImage.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelDate
			// 
			resources.ApplyResources(this.labelDate, "labelDate");
			this.labelDate.Name = "labelDate";
			// 
			// groupBoxArchives
			// 
			resources.ApplyResources(this.groupBoxArchives, "groupBoxArchives");
			this.groupBoxArchives.Controls.Add(this.archiveEditLabel);
			this.groupBoxArchives.Controls.Add(this.archiveEditorLabel);
			this.groupBoxArchives.Controls.Add(this.original);
			this.groupBoxArchives.Controls.Add(this.archiveLabel);
			this.groupBoxArchives.Controls.Add(this.buttonSelectArchive);
			this.groupBoxArchives.Controls.Add(this.archive);
			this.groupBoxArchives.Name = "groupBoxArchives";
			this.groupBoxArchives.TabStop = false;
			// 
			// archiveEditLabel
			// 
			resources.ApplyResources(this.archiveEditLabel, "archiveEditLabel");
			this.archiveEditLabel.Name = "archiveEditLabel";
			// 
			// archiveEditorLabel
			// 
			resources.ApplyResources(this.archiveEditorLabel, "archiveEditorLabel");
			this.archiveEditorLabel.Name = "archiveEditorLabel";
			// 
			// original
			// 
			resources.ApplyResources(this.original, "original");
			this.original.Name = "original";
			this.original.CheckedChanged += new System.EventHandler(this.original_CheckedChanged);
			// 
			// archiveLabel
			// 
			resources.ApplyResources(this.archiveLabel, "archiveLabel");
			this.archiveLabel.Name = "archiveLabel";
			// 
			// buttonSelectArchive
			// 
			resources.ApplyResources(this.buttonSelectArchive, "buttonSelectArchive");
			this.buttonSelectArchive.Name = "buttonSelectArchive";
			this.buttonSelectArchive.Click += new System.EventHandler(this.buttonSelectArchive_Click);
			// 
			// archive
			// 
			resources.ApplyResources(this.archive, "archive");
			this.archive.Name = "archive";
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
			// labelMain
			// 
			resources.ApplyResources(this.labelMain, "labelMain");
			this.labelMain.Name = "labelMain";
			// 
			// groupBoxMainImage
			// 
			resources.ApplyResources(this.groupBoxMainImage, "groupBoxMainImage");
			this.groupBoxMainImage.Controls.Add(this.buttonMakeMain);
			this.groupBoxMainImage.Controls.Add(this.labelMain);
			this.groupBoxMainImage.Name = "groupBoxMainImage";
			this.groupBoxMainImage.TabStop = false;
			// 
			// buttonMakeMain
			// 
			resources.ApplyResources(this.buttonMakeMain, "buttonMakeMain");
			this.buttonMakeMain.Name = "buttonMakeMain";
			this.buttonMakeMain.Click += new System.EventHandler(this.buttonMakeMain_Click);
			// 
			// labelDoc
			// 
			resources.ApplyResources(this.labelDoc, "labelDoc");
			this.labelDoc.Name = "labelDoc";
			this.labelDoc.AutoSize = true;
			// 
			// groupBoxServers
			// 
			resources.ApplyResources(this.groupBoxServers, "groupBoxServers");
			this.groupBoxServers.Controls.Add(this.servers);
			this.groupBoxServers.Name = "groupBoxServers";
			this.groupBoxServers.TabStop = false;
			// 
			// servers
			// 
			resources.ApplyResources(this.servers, "servers");
			this.servers.FullRowSelect = true;
			this.servers.HideSelection = false;
			this.servers.MultiSelect = false;
			this.servers.Name = "servers";
			this.servers.ShowItemToolTips = true;
			this.servers.UseCompatibleStateImageBehavior = false;
			this.servers.View = System.Windows.Forms.View.Details;
			// 
			// panelDoc
			// 
			resources.ApplyResources(this.panelDoc, "panelDoc");
			this.panelDoc.Controls.Add(this.labelDoc);
			this.panelDoc.Name = "panelDoc";
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this.groupSend);
			this.panel1.Name = "panel1";
			// 
			// groupSend
			// 
			this.groupSend.Controls.Add(this.sended);
			resources.ApplyResources(this.groupSend, "groupSend");
			this.groupSend.Name = "groupSend";
			this.groupSend.TabStop = false;
			// 
			// sended
			// 
			resources.ApplyResources(this.sended, "sended");
			this.sended.HideSelection = false;
			this.sended.MultiSelect = false;
			this.sended.Name = "sended";
			this.sended.ShowItemToolTips = true;
			this.sended.UseCompatibleStateImageBehavior = false;
			this.sended.View = System.Windows.Forms.View.Details;
			// 
			// labelEditor
			// 
			resources.ApplyResources(this.labelEditor, "labelEditor");
			this.labelEditor.Name = "labelEditor";
			// 
			// labelEdit
			// 
			resources.ApplyResources(this.labelEdit, "labelEdit");
			this.labelEdit.Name = "labelEdit";
			// 
			// buttonFileProperties
			// 
			resources.ApplyResources(this.buttonFileProperties, "buttonFileProperties");
			this.buttonFileProperties.Name = "buttonFileProperties";
			// 
			// groupBoxCurrentImage
			// 
			resources.ApplyResources(this.groupBoxCurrentImage, "groupBoxCurrentImage");
			this.groupBoxCurrentImage.Controls.Add(this.labelResolution);
			this.groupBoxCurrentImage.Controls.Add(this.labelSize);
			this.groupBoxCurrentImage.Controls.Add(this.labelColorType);
			this.groupBoxCurrentImage.Name = "groupBoxCurrentImage";
			this.groupBoxCurrentImage.TabStop = false;
			// 
			// labelResolution
			// 
			resources.ApplyResources(this.labelResolution, "labelResolution");
			this.labelResolution.Name = "labelResolution";
			// 
			// labelSize
			// 
			resources.ApplyResources(this.labelSize, "labelSize");
			this.labelSize.Name = "labelSize";
			// 
			// labelColorType
			// 
			resources.ApplyResources(this.labelColorType, "labelColorType");
			this.labelColorType.Name = "labelColorType";
			// 
			// labelCreator
			// 
			resources.ApplyResources(this.labelCreator, "labelCreator");
			this.labelCreator.Name = "labelCreator";
			// 
			// newWindowDocumentButton
			// 
			resources.ApplyResources(this.newWindowDocumentButton, "newWindowDocumentButton");
			this.newWindowDocumentButton.Name = "newWindowDocumentButton";
			this.newWindowDocumentButton.TabStop = false;
			this.newWindowDocumentButton.UseMnemonic = false;
			this.newWindowDocumentButton.UseVisualStyleBackColor = true;
			// 
			// PropertiesDocImageDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.newWindowDocumentButton);
			this.Controls.Add(this.labelCreator);
			this.Controls.Add(this.groupBoxCurrentImage);
			this.Controls.Add(this.buttonFileProperties);
			this.Controls.Add(this.labelEdit);
			this.Controls.Add(this.labelEditor);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panelDoc);
			this.Controls.Add(this.groupBoxServers);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBoxArchives);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.groupBoxMainImage);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PropertiesDocImageDialog";
			this.Load += new System.EventHandler(this.PropertiesDocImageDialog_Load);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PropertiesDocImageDialog_KeyUp);
			this.groupBoxArchives.ResumeLayout(false);
			this.groupBoxArchives.PerformLayout();
			this.groupBoxMainImage.ResumeLayout(false);
			this.groupBoxServers.ResumeLayout(false);
			this.panelDoc.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.groupSend.ResumeLayout(false);
			this.groupBoxCurrentImage.ResumeLayout(false);
			this.groupBoxCurrentImage.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void UpdateControls()
		{
			archiveLabel.Enabled = original.Checked;
			archive.Enabled = original.Checked;
			buttonSelectArchive.Enabled = original.Checked;
			archiveEditLabel.Enabled = linkArchiveEditor.Enabled = archiveEditorLabel.Enabled = original.Checked || !string.IsNullOrEmpty(linkArchiveEditor.Text);
			this.labelDoc.MaximumSize = new Size(panelDoc.ClientSize.Width, 0);
			this.panelDoc.VerticalScroll.Maximum = this.labelDoc.Height;
			this.labelDoc.MaximumSize = new Size(0, 0);
			buttonMakeMain.Enabled = !mainImage;
			if(mainImage)
				if(printedID > 0)
					labelMain.Text = Environment.StringResources.GetString("Properties.PropertiesDocImageDialog.UpdateControls.Message3");
				else
					labelMain.Text = Environment.StringResources.GetString("Properties.PropertiesDocImageDialog.UpdateControls.Message1");
			else
				if(printedID > 0)
					labelMain.Text = Environment.StringResources.GetString("Properties.PropertiesDocImageDialog.UpdateControls.Message4");
				else
					labelMain.Text = Environment.StringResources.GetString("Properties.PropertiesDocImageDialog.UpdateControls.Message2");

			newWindowDocumentButton.UnSet();
			newWindowDocumentButton.Set(docID);
		}

		private void original_CheckedChanged(object sender, EventArgs e)
		{
			if(original.Checked && (archiveID == 0))
				SelectArchiveDialog();

			buttonSelectArchive.Enabled = original.Checked;
			archiveLabel.Enabled = original.Checked;
			archive.Enabled = original.Checked;
			archiveEditLabel.Enabled = linkArchiveEditor.Enabled = archiveEditorLabel.Enabled = original.Checked || !string.IsNullOrEmpty(linkArchiveEditor.Text);
		}

		private void SelectArchiveDialog()
		{
			var dialog = new Select.SelectArchiveDialog(archiveID, false);
			dialog.DialogEvent += SelectArchiveDialog_DialogEvent;
			ShowSubForm(dialog);
		}

		private void SelectArchiveDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Focus();

			var dialog = e.Dialog as Select.SelectArchiveDialog;
			if(dialog.DialogResult == DialogResult.OK)
			{
				DTreeNode node = dialog.ArchiveNode;

				if(node != null)
				{
					archiveID = node.ID;
					archive.Text = Environment.ArchiveData.GetArchive(archiveID);
				}
			}
			if(original.Checked && (archiveID == 0))
				original.Checked = false;
		}

		private void buttonSelectArchive_Click(object sender, EventArgs e)
		{
			SelectArchiveDialog();
		}

		private void PropertiesDocImageDialog_Load(object sender, EventArgs e)
		{
			date = DateTime.Now;
			archive.ReadOnly = true;

			if(ImageID != 0)
			{
				Text += (Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? " (код " : " ( image ID ") + ImageID + ")";

				labelDoc.Text = DBDocString.Format(docID);

				DataRow docImageRow = Environment.DocImageData.GetDocImage(ImageID);
				if(docImageRow == null)
				{
					MessageBox.Show(Environment.StringResources.GetString("Properties_PropertiesDocImageDialog_PropertiesDocImageDialog_Load_Message1") + " " +
						ImageID + " " + Environment.StringResources.GetString("Properties_PropertiesDocImageDialog_PropertiesDocImageDialog_Load_Message2"));
					Close();
				}
				else
				{
					docID = (int)docImageRow[Environment.DocImageData.DocIDField];
					date = (DateTime)docImageRow[Environment.DocImageData.CreateDateField];

					labelDate.Text += " " + date.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");

					labelEdit.Text += " " + ((DateTime)docImageRow[Environment.DocImageData.EditedField]).ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
					linkEditor.Url = Environment.UsersURL + (int)docImageRow[Environment.DocImageData.EditorField];
					linkEditor.Text = Environment.EmpData.GetEmployee((int)docImageRow[Environment.DocImageData.EditorField], false);
					linkEditor.Caption = string.Format("№{0} {1}", (int)docImageRow[Environment.DocImageData.EditorField], linkEditor.Text);

					linkCreator.Url = Environment.UsersURL + (int)docImageRow[Environment.DocImageData.CreatorField];
					linkCreator.Text = Environment.EmpData.GetEmployee((int)docImageRow[Environment.DocImageData.CreatorField], false);
					linkCreator.Caption = string.Format("№{0} {1}", (int)docImageRow[Environment.DocImageData.CreatorField], linkCreator.Text);

					printedID = 0;
					if(docImageRow[Environment.DocImageData.PrintedField] is int)
						printedID = (int)docImageRow[Environment.DocImageData.PrintedField];
					if(printedID > 0)
					{
						object a1 = Environment.PrintData.GetField(Environment.PrintData.NameField, printedID);
						Text = Environment.StringResources.GetString("PrintedImageProperties") + ((a1 != null) ? " \"" + a1 + "\"" : "") + (Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? " (код " : " ( image ID ") + ImageID + ")";
						groupBoxMainImage.Text = Environment.StringResources.GetString("Properties_PropertiesDocImageDialog_PropertiesDocImageDialog_Load_Message7");
					}

					archiveID = (int)docImageRow[Environment.DocImageData.ArchiveIDField];

				    _originalArchiveId = archiveID;

					if(archiveID != 0)
					{
						archive.Text = Environment.ArchiveData.GetArchive(archiveID);
						original.Checked = true;
					}

					var eId = (int)docImageRow[Environment.DocImageData.ArchiveEditorField];
					if(eId > 0)
					{
						archiveEditLabel.Text += " " + ((DateTime)docImageRow[Environment.DocImageData.ArchiveEditField]).ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
						linkArchiveEditor.Url = Environment.UsersURL + eId;
						linkArchiveEditor.Text = Environment.EmpData.GetEmployee(eId, false);
						linkArchiveEditor.Caption = string.Format("№{0} {1}", eId, linkArchiveEditor.Text);
					}

					// основное ли изображение
					mainImage = (Environment.DocData.GetDocIntField(Environment.DocData.MainImageIDField, docID) == ImageID);

					imageType = docImageRow[Environment.DocImageData.ImageTypeField].ToString();

					labelDoc.Text += System.Environment.NewLine + Environment.StringResources.GetString("Properties_PropertiesDocImageDialog_ImageType_Title") +
						(imageType.ToUpper() == "PDF" ? "Adobe Acrobat Document" : "Microsoft Office Document Imaging File") + " (" + imageType.ToUpper() + ")";
					labelDoc.Text += System.Environment.NewLine + Environment.StringResources.GetString("FileSize") + ": " + ((int)docImageRow[Environment.DocImageData.FileSizeField]).ToString("N0") + " " + Environment.StringResources.GetString("Byte")
							+ "      " + Environment.StringResources.GetString("Pages") + ": " + docImageRow[Environment.DocImageData.PageCountField].ToString();
				}

				// loading servers
				servers.Columns.Add(Environment.StringResources.GetString("Archive"), servers.Width - 80, HorizontalAlignment.Left);
				servers.Columns.Add(Environment.StringResources.GetString("Place"), 70, HorizontalAlignment.Left);

				using(DataTable dt = Environment.DocImageData.GetDocImageServers(ImageID))
				using(DataTableReader dr = dt.CreateDataReader())
				{
					var fields = new string[3];
					FileInfo fi = null;

					while(dr.Read())
					{
						fields[0] = (string)dr[Environment.DocImageData.ServerField];
						var local = (int)dr[Environment.DocImageData.LocalField];

						if(local != 0)
						{
							if(string.IsNullOrEmpty(fileName))
								fileName = dr[Environment.DocImageData.NetworkPathField] + "\\" + Environment.GetFileNameFromID(ImageID) +
										   "." + imageType.ToLower();

							fields[1] = Environment.StringResources.GetString("Local");
							fi = new FileInfo(dr[Environment.DocImageData.NetworkPathField] + "\\" + Environment.GetFileNameFromID(ImageID) + "." + imageType.ToLower());
						}
						else
						{
							fields[1] = Environment.StringResources.GetString("External");
							fi = new FileInfo(dr[Environment.DocImageData.NetworkPathField] + "\\" + Environment.GetFileNameFromID(ImageID) + "." + imageType.ToLower());
						}
						var item = new ListViewItem(fields);
						item.ToolTipText = item.Text;
						servers.Items.Add(item);
					}
					dr.Close(); dr.Dispose(); dt.Dispose();
				}

				sended.Columns.Add(Environment.StringResources.GetString("Sender"), 110, HorizontalAlignment.Left);
				sended.Columns.Add(Environment.StringResources.GetString("Receiver"), 110, HorizontalAlignment.Left);
				sended.Columns.Add(Environment.StringResources.GetString("Contact"), sended.Width - 350, HorizontalAlignment.Left);
				sended.Columns.Add(Environment.StringResources.GetString("SendTime"), 110, HorizontalAlignment.Left);

				using(DataTable dt = Environment.DocImageData.GetLogForImage(ImageID))
				using(DataTableReader dr = dt.CreateDataReader())
				{
					while(dr.Read())
					{
						var direction = (int)dr[Environment.DocImageData.DirectionField];

						if(direction == 1)
						{
							var sb = new StringBuilder();
							string testStr = dr[Environment.DocImageData.SenderField].ToString();
							if(testStr.Length > 0)
							{
								sb.Append(System.Environment.NewLine);
								sb.Append(Environment.StringResources.GetString("Properties_PropertiesDocImageDialog_PropertiesDocImageDialog_Load_Message3"));
								sb.Append(" ");
								sb.Append(testStr);
							}
							sb.Append(System.Environment.NewLine);
							testStr = dr[Environment.DocImageData.SenderAddressField].ToString();
							if(testStr.Length > 0)
							{
								sb.Append(Environment.StringResources.GetString("Properties_PropertiesDocImageDialog_PropertiesDocImageDialog_Load_Message4"));
								sb.Append(" ");
								sb.Append(testStr);
								sb.Append("      ");
							}
							testStr = dr[Environment.DocImageData.RecipientField].ToString();
							if(testStr.Length > 0)
							{
								sb.Append(Environment.StringResources.GetString("Properties_PropertiesDocImageDialog_PropertiesDocImageDialog_Load_Message5"));
								sb.Append(" ");
								sb.Append(testStr);
								testStr = dr[Environment.DocImageData.RecipientAddressField].ToString();
								if(testStr.Length > 0)
								{
									sb.Append(" (");
									sb.Append(testStr);
									sb.Append(")");
								}
							}
							else
							{
								testStr = dr[Environment.DocImageData.RecipientAddressField].ToString();
								if(testStr.Length > 0)
									sb.Append(testStr);
							}
							labelDate.AutoSize = true;
							int oldheight = labelDate.Height;
							labelDate.Text += sb.ToString();
							if(labelDate.Height - oldheight > 0)
								Height += labelDate.Height - oldheight;
						}
						else
						{
							var fields = new string[4];
							fields[0] = dr[Environment.DocImageData.SenderField].ToString();
							fields[1] = dr[Environment.DocImageData.RecipientField].ToString();
							fields[2] = dr[Environment.DocImageData.RecipientAddressField].ToString();
							fields[3] = (dr[Environment.DocImageData.SendTimeField] == null) ? "" : ((DateTime)dr[Environment.DocImageData.SendTimeField]).ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");

							sended.Items.Add(new ListViewItem(fields) { ToolTipText = fields[2] });
						}
					}
					dr.Close(); dr.Dispose(); dt.Dispose();
				}
			}
			else
			{
				if(docID == 0)
				{
					MessageBox.Show("Properties.PropertiesDocImageDialog.PropertiesDocImageDialog_Load.Error1");
					Close();
				}
				else
				{
					Text = Environment.StringResources.GetString("Properties_PropertiesDocImageDialog_PropertiesDocImageDialog_Load_Title1");

					if(!string.IsNullOrEmpty(fileName))
					{
						var f = new FileInfo(fileName);
						ScanInfo info = TextProcessor.ParseScanInfo(f);
						if(info != null)
						{
							date = info.Date;
							labelDate.Text += date.ToString();
						}
					}
					else
					{
						MessageBox.Show(Environment.StringResources.GetString("Properties_PropertiesDocImageDialog_PropertiesDocImageDialog_Load_Message6"));
						Close();
					}
				}
			}
			UpdateControls();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if(!original.Checked)
				archiveID = 0;

			if(Environment.DocImageData.SetDocImageProperties(ImageID, docID, archiveID))
			{
                if (_originalArchiveId != archiveID)
                    UndoRedoStackSetDocImageArchive(_originalArchiveId, archiveID);

				End(DialogResult.OK);
			}
		}

		private void buttonMakeMain_Click(object sender, EventArgs e)
		{
		    var origMainImage = Environment.DocData.GetDocIntField(Environment.DocData.MainImageIDField, docID);

		    if (Environment.DocData.SetMainImage(docID, ImageID))
		        UndoRedoStackSetMainImage(origMainImage, ImageID);

		    DataRow dr = Environment.DocImageData.GetDocImage(ImageID);
			if(dr != null)
			{
				mainImage = (Environment.DocData.GetDocIntField(Environment.DocData.MainImageIDField, docID) == ImageID);
			}

			UpdateControls();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			End(DialogResult.Cancel);
		}

	    /// <summary>
	    /// Формирование команды Undo SetDocImageArchive
	    /// </summary>
	    /// <param name="archiveId"></param>
	    /// <param name="newArchiveId"></param>
	    private void UndoRedoStackSetDocImageArchive(int archiveId, int newArchiveId)
        {
            Func<object[], bool> undoDelegate = UndoRedoCommands.UndoSetDocImageArchive;
            Func<object[], bool> redoDelegate = UndoRedoCommands.RedoSetDocImageArchive;

            var undoText = Environment.StringResources.GetString("PropertiesDocImageDialog_SetDocImageArchive_Undo");
            var redoText = Environment.StringResources.GetString("PropertiesDocImageDialog_SetDocImageArchive_Redo");

            Environment.UndoredoStack.Add("SetDocImageArchive", "SetDocImageArchive", undoText, redoText, null, new object[] { ImageID, undoDelegate, redoDelegate, docID, archiveId, newArchiveId }, Environment.CurEmp.ID);
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

            Environment.UndoredoStack.Add("SetMainImage", "SetMainImage", undoText, redoText, null, new object[] { docID, undoDelegate, redoDelegate, origMainImage, newMainImage }, Environment.CurEmp.ID);
        }

		public void FillCurrentPage()
		{
			labelColorType.Text += ColorType;
			labelResolution.Text += (imageType.ToUpper() == "TIF" ? ResolutionY.ToString() + "x" + ResolutionX.ToString() : Environment.StringResources.GetString("PrinterOp_Status2"));
			labelSize.Text += ((PixelHeight * 2.54) / ResolutionY).ToString("N2") + "cm x " + ((PixelWigth * 2.54) / ResolutionX).ToString("N2") + "cm";
			if(PixelHeight > 2950 * ResolutionY / 254 && PixelHeight < 2990 * ResolutionY / 254 && PixelWigth > 2080 * ResolutionY / 254 && PixelWigth < 2120 * ResolutionY / 254)
				labelSize.Text += " (A4)";
			if(PixelWigth > 2950 * ResolutionY / 254 && PixelWigth < 2990 * ResolutionY / 254 && PixelHeight > 2080 * ResolutionY / 254 && PixelHeight < 2120 * ResolutionY / 254)
				labelSize.Text += " (A4 landscape)";
		}

		private void PropertiesDocImageDialog_KeyUp(object sender, KeyEventArgs e)
		{
			newWindowDocumentButton.ProcessKey(e.KeyData);
		}
	}
}