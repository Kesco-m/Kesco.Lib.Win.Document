using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml;

namespace Kesco.Lib.Win.Document.Select
{
	/// <summary>
	///   Summary description for SelectDocUniversalDialog.
	/// </summary>
	public class SelectDocUniversalDialog : FreeDialog
	{
		private readonly int curDocID;

		private readonly string source;
		private readonly int stringCount;

		private System.Timers.Timer timer;

		private Options.Folder options;

		/// <summary>
		///   Required designer variable.
		/// </summary>
		private Container components;
		private GroupBox groupDocs;
		private Panel panelTop;
		private Panel panelButtons;
		private Button buttonOK;
		private Button buttonCancel;
		private Panel panelMain;
		private Panel panelBottom;
		private Splitter splitterTopBottom;
		private GroupBox groupLinks;
		private Splitter splitterLeftImage;
		private GroupBox groupPersons;
		private ListView listPersons;
		private Panel panelLeft;
		private Splitter splitterPersonsLinks;
		private Controls.DocControl docControl;
		private Kesco.Lib.Win.Document.Controls.MenuList linkList;
		private Panel panelText;
		private Button buttonChange;
		private Button buttonOpenDoc;
		private Panel panelInfo;
		private Label labelInfo;
		private Kesco.Lib.Win.Document.Grid.SelectDataGrid docGrid;
		private DataGridViewTextBoxColumn dataGridTextBoxColumnDocID;
		private DataGridViewTextBoxColumn dataGridTextBoxColumnType;
		private DataGridViewTextBoxColumn dataGridTextBoxColumnDate;
		private DataGridViewTextBoxColumn dataGridTextBoxColumnNumber;
		private DataGridViewTextBoxColumn dataGridTextBoxColumnDecr;

		public SelectDocUniversalDialog(string source, int curDocID)
		{
			Thread.CurrentThread.CurrentUICulture = Environment.CurCultureInfo;
			InitializeComponent();
			this.KeyPreview = true;
			docGrid.MultiSelect = false;

			this.source = source;

			this.curDocID = curDocID;
			buttonOpenDoc.Enabled = false;
		}

		public SelectDocUniversalDialog(string source, string xml) : this(source, 0, xml, false)
		{
		}

		public SelectDocUniversalDialog(string source, int curDocID, string xml, bool multiselect) : this(source, curDocID)
		{
			XML = xml;
			if(XML != null)
			{
				string text = Data.DALC.Documents.Search.Options.GetText(XML).Trim();
				stringCount = Regex.Matches(text, "[\n]").Count + 1;
				int test = stringCount * (int)(labelInfo.Font.GetHeight() + 1.5) + 20;
				panelText.Height = (test > 38) ? test : 38;
				panelInfo.AutoScrollMinSize = new Size(0,
													   (stringCount > 4)
														   ? ((stringCount) *
															  (int)(labelInfo.Font.GetHeight() + 1.5) + 4)
														   : 0);
				labelInfo.Text = text;
			}

			labelInfo.AutoSize = false;
			labelInfo.Width = labelInfo.PreferredWidth;
			labelInfo.Height = (stringCount) * (int)(labelInfo.Font.GetHeight() + 1.5) + 4;
			panelText.Visible = true;
			panelText.Enabled = true;
			if(XML == null)
				buttonChange.Visible = false;

			docGrid.MultiSelect = multiselect;
		}

		#region Accessors

		public int DocID { get; private set; }

		public int[] DocIDs
		{
			get { return docGrid.GetCurIDs(); }
		}

		public int CurDocID
		{
			get { return curDocID; }
		}

		public string XML { get; private set; }

		#endregion

		/// <summary>
		///   Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
            Console.WriteLine("{0}: Dispose", DateTime.Now.ToString("HH:mm:ss fff"));
			if(timer != null)
			{
				timer.Elapsed -= timer_Elapsed;
				timer.Stop();
			}
			if(disposing)
			{
				foreach(System.Windows.Forms.Control control in panelMain.Controls)
				{
					if(control != null)
					{
						foreach(System.Windows.Forms.Control childControl in control.Controls)
						{
							if(childControl != null)
							{
								Console.WriteLine("{0}: {1} dispose begin", DateTime.Now.ToString("HH:mm:ss fff"), childControl.Name);
								control.Controls.Remove(childControl);
								childControl.Dispose();
								Console.WriteLine("{0}: dispose end", DateTime.Now.ToString("HH:mm:ss fff"));
							}
						}
                        Console.WriteLine("{0}: {1} dispose begin", DateTime.Now.ToString("HH:mm:ss fff"), control.Name);
						
						panelMain.Controls.Remove(control);
						control.Dispose();
                        Console.WriteLine("{0}: dispose end", DateTime.Now.ToString("HH:mm:ss fff"));
					}
				}
				foreach(System.Windows.Forms.Control control in Controls)
				{
					if(control != null)
					{
                        Console.WriteLine("{0}: {1} dispose begin", DateTime.Now.ToString("HH:mm:ss fff"), control.Name);
						Controls.Remove(control);
						control.Dispose();
                        Console.WriteLine("{0}: dispose end", DateTime.Now.ToString("HH:mm:ss fff"));
					}
				}
				Console.WriteLine("{0}: components dispose begin", DateTime.Now.ToString("HH:mm:ss fff"));

				if(components != null)
				{
					components.Dispose();
				}
                Console.WriteLine("{0}: components dispose end", DateTime.Now.ToString("HH:mm:ss fff"));
			}
            Console.WriteLine("{0}: SubSelect dispose begin", DateTime.Now.ToString("HH:mm:ss fff"));
			base.Dispose(disposing);
            Console.WriteLine("{0}: SubSelect dispose end", DateTime.Now.ToString("HH:mm:ss fff"));
		}

		#region Windows Form Designer generated code

		/// <summary>
		///   Required method for Designer support - do not modify the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			var resources =
				new System.ComponentModel.ComponentResourceManager(typeof(SelectDocUniversalDialog));
			this.groupDocs = new System.Windows.Forms.GroupBox();
			this.docGrid = new Grid.SelectDataGrid();
			this.dataGridTextBoxColumnDocID = new DataGridViewTextBoxColumn();
			this.dataGridTextBoxColumnType = new DataGridViewTextBoxColumn();
			this.dataGridTextBoxColumnDate = new DataGridViewTextBoxColumn();
			this.dataGridTextBoxColumnNumber = new DataGridViewTextBoxColumn();
			this.dataGridTextBoxColumnDecr = new DataGridViewTextBoxColumn();
			this.panelTop = new System.Windows.Forms.Panel();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panelMain = new System.Windows.Forms.Panel();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.docControl = new Kesco.Lib.Win.Document.Controls.DocControl();
			this.splitterLeftImage = new System.Windows.Forms.Splitter();
			this.panelLeft = new System.Windows.Forms.Panel();
			this.groupLinks = new System.Windows.Forms.GroupBox();
			this.linkList = new Kesco.Lib.Win.Document.Controls.MenuList();
			this.splitterPersonsLinks = new System.Windows.Forms.Splitter();
			this.groupPersons = new System.Windows.Forms.GroupBox();
			this.listPersons = new System.Windows.Forms.ListView();
			this.splitterTopBottom = new System.Windows.Forms.Splitter();
			this.panelText = new System.Windows.Forms.Panel();
			this.panelInfo = new System.Windows.Forms.Panel();
			this.labelInfo = new System.Windows.Forms.Label();
			this.buttonChange = new System.Windows.Forms.Button();
			this.buttonOpenDoc = new System.Windows.Forms.Button();
			this.groupDocs.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.docGrid)).BeginInit();
			this.panelTop.SuspendLayout();
			this.panelButtons.SuspendLayout();
			this.panelMain.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.panelLeft.SuspendLayout();
			this.groupLinks.SuspendLayout();
			this.groupPersons.SuspendLayout();
			this.panelText.SuspendLayout();
			this.panelInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupDocs
			// 
			this.groupDocs.Controls.Add(this.docGrid);
			resources.ApplyResources(this.groupDocs, "groupDocs");
			this.groupDocs.Name = "groupDocs";
			this.groupDocs.TabStop = false;
			// 
			// dataGridTextBoxColumnDocID
			// 
			resources.ApplyResources(this.dataGridTextBoxColumnDocID, "dataGridTextBoxColumnDocID");
			this.dataGridTextBoxColumnDocID.Name = "КодДокумента";
			// 
			// dataGridTextBoxColumnType
			// 
			resources.ApplyResources(this.dataGridTextBoxColumnType, "dataGridTextBoxColumnType");
			this.dataGridTextBoxColumnType.Name = "НазваниеДокумента";
			// 
			// dataGridTextBoxColumnDate
			// 
			resources.ApplyResources(this.dataGridTextBoxColumnDate, "dataGridTextBoxColumnDate");
			this.dataGridTextBoxColumnDate.Name = "ДатаДокумента";
			// 
			// dataGridTextBoxColumnNumber
			// 
			resources.ApplyResources(this.dataGridTextBoxColumnNumber, "dataGridTextBoxColumnNumber");
			this.dataGridTextBoxColumnNumber.Name = "НомерДокумента";
			// 
			// dataGridTextBoxColumnDecr
			// 
			resources.ApplyResources(this.dataGridTextBoxColumnDecr, "dataGridTextBoxColumnDecr");
			this.dataGridTextBoxColumnDecr.Name = "Описание";
			// 
			// docGrid
			// 
			resources.ApplyResources(this.docGrid, "docGrid");
			this.docGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this.docGrid.DataMember = "";
			this.docGrid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;
			this.docGrid.Name = "docGrid";
			this.docGrid.RowHeadersVisible = false;
			// 
			// panelTop
			// 
			this.panelTop.Controls.Add(this.groupDocs);
			resources.ApplyResources(this.panelTop, "panelTop");
			this.panelTop.Name = "panelTop";
			// 
			// panelButtons
			// 
			this.panelButtons.Controls.Add(this.buttonOK);
			this.panelButtons.Controls.Add(this.buttonCancel);
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Name = "panelButtons";
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
			// panelMain
			// 
			this.panelMain.Controls.Add(this.panelBottom);
			this.panelMain.Controls.Add(this.splitterTopBottom);
			this.panelMain.Controls.Add(this.panelTop);
			resources.ApplyResources(this.panelMain, "panelMain");
			this.panelMain.Name = "panelMain";
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.docControl);
			this.panelBottom.Controls.Add(this.splitterLeftImage);
			this.panelBottom.Controls.Add(this.panelLeft);
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Name = "panelBottom";
			// 
			// docControl
			// 
			this.docControl.AnnotationDraw = false;
			this.docControl.CurDocString = null;
			resources.ApplyResources(this.docControl, "docControl");
			this.docControl.DocumentID = 0;
			this.docControl.EmpName = null;
			this.docControl.ForceRelicate = false;
			this.docControl.ImageID = -1;
			this.docControl.ImagesPanelOrientation =
				Lib.Win.ImageControl.ImageControl.TypeThumbnailPanelOrientation.Left;
			this.docControl.IsEditNotes = false;
			this.docControl.IsMain = false;
			this.docControl.IsMoveImage = true;
			this.docControl.Name = "docControl";
			this.docControl.Page = 1;
			this.docControl.PersonParamStr = "clid=4&return=1";
			this.docControl.SelectionMode = false;
			this.docControl.ShowThumbPanel = true;
			this.docControl.ShowToolBar = true;
			this.docControl.ShowWebPanel = false;
			this.docControl.SplinterPlace = new System.Drawing.Point(200, 50);
			this.docControl.Subscribe = new System.Guid("00000000-0000-0000-0000-000000000000");
			this.docControl.Zoom = 100;
			this.docControl.ZoomText = "";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SelectDocUniversalDialog_KeyUp);
			// 
			// splitterLeftImage
			// 
			resources.ApplyResources(this.splitterLeftImage, "splitterLeftImage");
			this.splitterLeftImage.Name = "splitterLeftImage";
			this.splitterLeftImage.TabStop = false;
			// 
			// panelLeft
			// 
			this.panelLeft.Controls.Add(this.groupLinks);
			this.panelLeft.Controls.Add(this.splitterPersonsLinks);
			this.panelLeft.Controls.Add(this.groupPersons);
			resources.ApplyResources(this.panelLeft, "panelLeft");
			this.panelLeft.Name = "panelLeft";
			// 
			// groupLinks
			// 
			this.groupLinks.Controls.Add(this.linkList);
			resources.ApplyResources(this.groupLinks, "groupLinks");
			this.groupLinks.Name = "groupLinks";
			this.groupLinks.TabStop = false;
			// 
			// linkList
			// 
			this.linkList.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuPopup;
			resources.ApplyResources(this.linkList, "linkList");
			this.linkList.BackColor = System.Drawing.SystemColors.ControlLight;
			this.linkList.CheckLinkDoc = null;
			this.linkList.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			this.linkList.DocID = 0;
			this.linkList.Name = "linkList";
			this.linkList.NoClose = false;
			this.linkList.Zoom = "100";
			this.linkList.TreeMouseUp += new System.Windows.Forms.MouseEventHandler(this.linkList_TreeMouseUp);
			// 
			// splitterPersonsLinks
			// 
			resources.ApplyResources(this.splitterPersonsLinks, "splitterPersonsLinks");
			this.splitterPersonsLinks.Name = "splitterPersonsLinks";
			this.splitterPersonsLinks.TabStop = false;
			// 
			// groupPersons
			// 
			this.groupPersons.Controls.Add(this.listPersons);
			resources.ApplyResources(this.groupPersons, "groupPersons");
			this.groupPersons.Name = "groupPersons";
			this.groupPersons.TabStop = false;
			// 
			// listPersons
			// 
			resources.ApplyResources(this.listPersons, "listPersons");
			this.listPersons.FullRowSelect = true;
			this.listPersons.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listPersons.HideSelection = false;
			this.listPersons.MultiSelect = false;
			this.listPersons.Name = "listPersons";
			this.listPersons.ShowItemToolTips = true;
			this.listPersons.UseCompatibleStateImageBehavior = false;
			this.listPersons.View = System.Windows.Forms.View.Details;
			this.listPersons.LostFocus += new System.EventHandler(listPersons_LostFocus);
			// 
			// splitterTopBottom
			// 
			resources.ApplyResources(this.splitterTopBottom, "splitterTopBottom");
			this.splitterTopBottom.Name = "splitterTopBottom";
			this.splitterTopBottom.TabStop = false;
			// 
			// panelText
			// 
			this.panelText.Controls.Add(this.panelInfo);
			this.panelText.Controls.Add(this.buttonChange);
			this.panelText.Controls.Add(this.buttonOpenDoc);
			resources.ApplyResources(this.panelText, "panelText");
			this.panelText.Name = "panelText";
			// 
			// panelInfo
			// 
			resources.ApplyResources(this.panelInfo, "panelInfo");
			this.panelInfo.Controls.Add(this.labelInfo);
			this.panelInfo.Name = "panelInfo";
			// 
			// labelInfo
			// 
			resources.ApplyResources(this.labelInfo, "labelInfo");
			this.labelInfo.Name = "labelInfo";
			// 
			// buttonChange
			// 
			resources.ApplyResources(this.buttonChange, "buttonChange");
			this.buttonChange.Name = "buttonChange";
			this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
			// 
			// buttonOpenDoc
			// 
			resources.ApplyResources(this.buttonOpenDoc, "buttonOpenDoc");
			this.buttonOpenDoc.Name = "buttonOpenDoc";
			this.buttonOpenDoc.Click += new System.EventHandler(this.buttonOpenDoc_Click);
			// 
			// SelectDocUniversalDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.panelMain);
			this.Controls.Add(this.panelText);
			this.Controls.Add(this.panelButtons);
			this.MinimizeBox = false;
			this.Name = "SelectDocUniversalDialog";
			this.Load += new System.EventHandler(this.SelectDocUniversalDialog_Load);
			this.Closed += new System.EventHandler(this.SelectDocUniversalDialog_Closed);
			this.groupDocs.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.docGrid)).EndInit();
			this.panelTop.ResumeLayout(false);
			this.panelButtons.ResumeLayout(false);
			this.panelMain.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.panelLeft.ResumeLayout(false);
			this.groupLinks.ResumeLayout(false);
			this.groupPersons.ResumeLayout(false);
			this.panelText.ResumeLayout(false);
			this.panelInfo.ResumeLayout(false);
			this.panelInfo.PerformLayout();
			this.ResumeLayout(false);
		}

		#endregion

		private void SelectDocUniversalDialog_Load(object sender, EventArgs e)
		{
			// загружаем настройки
			options = Environment.Layout.Folders.Add(Name);
			bool maximized = (WindowState == FormWindowState.Maximized);
			if(options.LoadStringOption("Maximized", maximized.ToString()) == "True")
				WindowState = FormWindowState.Maximized;
			else
			{
				Left = options.LoadIntOption("Left", Left);
				Top = options.LoadIntOption("Top", Top);

				Width = options.LoadIntOption("Width", Width);
				Height = options.LoadIntOption("Height", Height);
			}

			int width = labelInfo.Width;
			if(width < panelInfo.Width)
			{
				panelInfo.Width = width + SystemInformation.VerticalScrollBarWidth;
				buttonChange.Location = new Point(width + 16, buttonChange.Location.Y);
				buttonOpenDoc.Location = new Point(buttonChange.Size.Width + buttonChange.Location.X + SystemInformation.VerticalScrollBarWidth,
						buttonChange.Location.Y);
			}
			else if(width > panelInfo.Width)
			{
				if(stringCount < 3)
					panelText.Height += SystemInformation.VerticalScrollBarWidth + 1;
				buttonChange.Location = new Point(SystemInformation.VerticalScrollBarWidth + panelInfo.Width, 8);
				buttonOpenDoc.Location = new Point(buttonChange.Size.Width + buttonChange.Location.X + SystemInformation.VerticalScrollBarWidth,
						buttonChange.Location.Y);
			}
			panelInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;


			buttonChange.BringToFront();
			buttonOpenDoc.BringToFront();
			panelTop.Height = options.LoadIntOption("PanelTopHeight", panelTop.Height);
			panelLeft.Width = options.LoadIntOption("PanelLeftWidth", panelLeft.Width);
			groupPersons.Height = options.LoadIntOption("GroupPersonsHeight", groupPersons.Height);

			// загружаем документы
			listPersons.Columns.Add(Environment.PersonWord.GetForm(Cases.I, true, true), listPersons.Width - SystemInformation.VerticalScrollBarWidth - 4,
									HorizontalAlignment.Left);
			groupPersons.Text = Environment.PersonWord.GetForm(Cases.I, true, true);
			docControl.ZoomText = options.LoadStringOption("ZoomString", Environment.StringResources.GetString("ToWidth"));
			docControl.ShowThumbPanel = Convert.ToBoolean(options.LoadStringOption("ShowThumb", false.ToString()));

			docGrid.Init(Environment.Layout);
			docGrid.Columns.Add(dataGridTextBoxColumnDocID);
			docGrid.Columns.Add(dataGridTextBoxColumnType);
			docGrid.Columns.Add(dataGridTextBoxColumnDate);
			docGrid.Columns.Add(dataGridTextBoxColumnNumber);
			docGrid.Columns.Add(dataGridTextBoxColumnDecr);
			docGrid.DoubleClick += docGrid_DoubleClick;
			docGrid.CurrentCellChanged += docGrid_CurrentCellChanged;

			BeginInvoke((MethodInvoker)(FillAll));
		}

		private void FillAll()
		{
			Thread.CurrentThread.CurrentUICulture = Environment.CurCultureInfo;

			docGrid.DataSource = Environment.DocData.GetDocsByIDQuery(source, Environment.CurCultureInfo.Name);
			docGrid.SetSort(docGrid.OptionFolder.LoadStringOption("Сортировка", string.Empty));

			docGrid.CurrentCell = null;
		}

		private bool CheckDoc()
		{
			if(!docGrid.IsMultiple && DocID < 1)
				return false;
			if(docGrid.GetCurIDs().Any(id => curDocID == id))
			{
				var form =
					new MessageForm(
						Environment.StringResources.GetString("Select_SelectDocUniversalDialog_Message1") +
						System.Environment.NewLine +
						Environment.StringResources.GetString("Select_SelectDocUniversalDialog_Message2"),
						Environment.StringResources.GetString("Warning"));
				ShowSubForm(form);
				return false;
			}

			foreach(int id in docGrid.GetCurIDs().Where(id => XML != null))
			{
                Console.WriteLine("{0}: Begin check compliance", DateTime.Now.ToString("HH:mm:ss fff"));
				var doc = new XmlDocument();
				var newDoc = new XmlDocument();
				XmlElement op = newDoc.CreateElement("Options");
				doc.LoadXml(XML);
				foreach(XmlElement elem in doc.SelectNodes("Options/Option[@fixed=\"true\"]"))
					op.AppendChild(newDoc.ImportNode(elem, true));
				newDoc.AppendChild(op);

				if(op.InnerXml.Length <= 0)
					continue;
				XmlElement el = newDoc.CreateElement("Option");
				XmlAttribute atr = newDoc.CreateAttribute("name");
				atr.InnerText = "КодДокумента";
				el.Attributes.Append(atr);
				atr = newDoc.CreateAttribute("value");
				atr.InnerText = id.ToString();
				el.Attributes.Append(atr);
				op.AppendChild(el);

				try
				{
					string sql = Data.DALC.Documents.Search.Options.GetSQL(newDoc.InnerXml);
                    Console.WriteLine("{0}: Created sql compliance", DateTime.Now.ToString("HH:mm:ss fff"));
					bool ret = Environment.DocData.GetDocCount("( " + sql + " ) SearchTable") > 0;
                    Console.WriteLine("{0}: End Sql. compliance = {1}", DateTime.Now.ToString("HH:mm:ss fff"), ret);
					if(!ret)
					{
						var form =
							new MessageForm( Environment.StringResources.GetString("Select_SelectDocUniversalDialog_Message3"),
									Environment.StringResources.GetString("Warning"));
						form.ShowDialog();
					}
					return ret;
				}
				catch
				{
					return false;
				}
			}
			return true;
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if(!CheckDoc())
				return;
			if(docGrid.IsSingle)
			{
				docGrid.ClearSelection();
				End(DialogResult.OK, new object[] { curDocID, DocID, XML });
			}
			else if(docGrid.IsMultiple)
				End(DialogResult.OK, new object[] { curDocID, docGrid.GetCurIDs(), XML });
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			End(DialogResult.Cancel);
		}

		private void buttonChange_Click(object sender, EventArgs e)
		{
			End(DialogResult.Retry, new object[] { curDocID, DocID, XML });
		}

		private void SelectDocUniversalDialog_Closed(object sender, EventArgs e)
		{
			try
			{
				if(docGrid != null && docGrid.IsFine)
				{
					foreach(DataGridViewColumn column in docGrid.Columns)
						docGrid.OptionFolder.OptionForced<int>(column.Name).Value = column.Width;

					docGrid.OptionFolder.OptionForced<string>("Сортировка").Value = docGrid.GetDataView().Sort;
					docGrid.OptionFolder.Save();
				}

				if(options != null)
				{
					bool maximized = (WindowState == FormWindowState.Maximized);
					options.OptionForced<string>("Maximized").Value = maximized.ToString();

					if(!maximized)
					{
						options.OptionForced<int>("Left").Value = this.Left;
						options.OptionForced<int>("Top").Value = this.Top;
						options.OptionForced<int>("Width").Value = this.Width;
						options.OptionForced<int>("Height").Value = this.Height;
					}

					options.OptionForced<int>("PanelTopHeight").Value = panelTop.Height;
					options.OptionForced<int>("PanelLeftWidth").Value = panelLeft.Width;
					options.OptionForced<int>("GroupPersonsHeight").Value = groupPersons.Height;
					options.OptionForced<string>("ZoomString").Value = docControl.ZoomText;
					options.OptionForced<string>("ShowThumb").Value = docControl.ShowThumbPanel.ToString();

					options.Save();
				}
			}
			catch
			{
			}
		}

		private void linkList_TreeMouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Left)
				return;

			int dID = linkList.GetLinkedDocID();
			if(dID > 0)
			{
				DocID = dID;
				docControl.DocumentID = DocID;
			}
			else
			{
				object obj = docGrid[0, docGrid.CurrentRow.Index].Value;
				if(obj is int)
				{
					var testdocID = (int)obj;
					if(docControl.DocumentID != testdocID)
						docControl.DocumentID = testdocID;
				}
			}
		}

		private void docGrid_CurrentCellChanged(object sender, EventArgs e)
		{
			if(!docGrid.IsFine)
				return;

			buttonOpenDoc.Enabled = false;// ждём пока полностью не прогрузится документ
			buttonOK.Enabled = false;

			if(timer == null)
			{
				timer = new System.Timers.Timer(200);
				timer.Elapsed += timer_Elapsed;
			}
			else
			{
				timer.Stop();
				timer.Interval = 200;
			}
			timer.Elapsed += timer_Elapsed;
			timer.AutoReset = false;
			timer.Enabled = true;
			timer.Start();
		}

		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if(Disposing || IsDisposed)
				return;
			if(timer != null)
				timer.Elapsed -= timer_Elapsed;

			if(InvokeRequired)
			{
				this.BeginInvoke((MethodInvoker)(OpenDoc));
                Console.WriteLine("{0}: {1} correct thread", DateTime.Now.ToString("HH:mm:ss fff"), Name);
			}
			else
				OpenDoc();
		}

		private void OpenDoc()
		{
			if(!docGrid.IsFine)
				return;

			buttonOpenDoc.Enabled = false;
			buttonOK.Enabled = false;

			Thread.CurrentThread.CurrentUICulture = Environment.CurCultureInfo;
			if(docGrid.IsSingle && docGrid.CurrentRow != null)
			{
				object obj = docGrid.GetCurID();
				if(obj is int)
				{
					var testdocID = (int)obj;
					if(testdocID >= 0 && testdocID != DocID)
					{
						DocID = testdocID;
						docControl.DocumentID = DocID;
						buttonOK.Enabled = (DocID > 0) && (DocID != curDocID);
						buttonOpenDoc.Enabled = (DocID > 0) && (DocID != curDocID);
						listPersons.Items.Clear();
						using(DataTable dt = Environment.DocData.GetDocPersons(DocID))
						using(DataTableReader dr = dt.CreateDataReader())
						{
							while(dr.Read())
							{
								var personID = (int)dr[Environment.PersonData.IDField];
								string person = dr[Environment.PersonData.NameField].ToString();
								int position = (byte)dr[Environment.DocData.PersonPositionField];
								bool isValid = 0 < (int)dr[Environment.DocData.PersonIsValidField];

								if(person != null)
								{
									var item = new Document.Items.PersonListItem(personID, person, Environment.PersonURL + personID.ToString(), position, isValid);
									listPersons.Items.Add(item);
								}
							}
							dr.Close();
							dr.Dispose();
							dt.Dispose();
						}

						// update link tree
						linkList.DocID = DocID;
						return;
					}
					else
					{
						if(testdocID == DocID)
						{
							buttonOK.Enabled = (DocID > 0) && (DocID != curDocID);
							buttonOpenDoc.Enabled = (DocID > 0) && (DocID != curDocID);
							return;
						}
					}
				}
				buttonOpenDoc.Enabled = false;
				buttonOK.Enabled = false;
			}
			else if(docGrid.IsMultiple)
			{
				buttonOK.Enabled = true;
				buttonOpenDoc.Enabled = true;
			}
			docControl.FileName = null;
			DocID = 0;
			listPersons.Items.Clear();
			linkList.DocID = DocID;
			docControl.DocumentID = DocID;
		}

		private void docGrid_DoubleClick(object sender, EventArgs e)
		{
			if(docGrid.IsSingle && buttonOK.Enabled && CheckDoc())
				End(DialogResult.OK, new object[] { curDocID, DocID, XML });
		}

		private void buttonOpenDoc_Click(object sender, EventArgs e)
		{
			if(DocID > 0)
				Environment.OnNewWindow(this, new Components.DocumentSavedEventArgs(DocID, -1));
			else if(docGrid.IsMultiple)
				foreach(int id in docGrid.GetCurIDs())
					Environment.OnNewWindow(this, new Components.DocumentSavedEventArgs(id, -1));
		}

		private void listPersons_LostFocus(object sender, EventArgs e)
		{
			for(int i = 0; i < listPersons.Items.Count; i++)
				listPersons.Items[i].Selected = false;
		}

		private void SelectDocUniversalDialog_KeyUp(object sender, KeyEventArgs e)
		{
			try
			{
				switch(e.KeyData)
				{
					case Keys.Left:
						if(docControl.ImageDisplayed && docControl.Page > 1)
							docControl.Page--;
						break;
					case Keys.Right:
						if(docControl.ImageDisplayed && docControl.Page < docControl.PageCount)
							docControl.Page++;
						break;
				}
			}
			catch
			{
			}
		}
	}
}