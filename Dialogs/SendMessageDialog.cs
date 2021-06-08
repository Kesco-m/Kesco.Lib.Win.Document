using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document.Controls;
using Kesco.Lib.Win.Document.ListViews;
using Kesco.Lib.Win.Error;

namespace Kesco.Lib.Win.Document.Dialogs
{
	/// <summary>
	/// Диалог отправки сообщения по документу
	/// </summary>
	public class SendMessageDialog : FreeDialog
	{
		private bool sendBlock;
		private NewWindowDocumentButton newWindowDocumentButton;
		private Button buttonCancel;
		private MessageSnatcher snatcher;

		private bool removeWork;

		public delegate void SendDelegate(
			int[] ids, string forAllMessage, SynchronizedCollection<int> forAllRecipients, Hashtable personMessages);

		public delegate void MessageSendDelegate(bool delete, int[] ids);

		public static event SendDelegate NeedSendWindow;
		public static event MessageSendDelegate MessageSend;

		public static void OnNeedSendWindow(int[] ids, string forAllMessage, SynchronizedCollection<int> forAllRecipients, Hashtable personMessages)
		{
			if(NeedSendWindow != null)
				NeedSendWindow(ids, forAllMessage, forAllRecipients, personMessages);
		}

		public static void OnMessageSend(bool delete, int[] ids)
		{
			try
			{
				if(MessageSend != null)
					MessageSend(delete, ids);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private SynchronizedCollection<Keys> keyLocker = new SynchronizedCollection<Keys>();

		private string docIDstr;
		private string docString;
		private DateTime addDate = DateTime.Today;
		private int empID;
		private bool personMessage;
		private SynchronizedCollection<int> allMessageEmployee = new SynchronizedCollection<int>();
		private Hashtable personSenders = new Hashtable(new myEqualityComparer());

		private IContainer components;
		private PictureBox pictureBox;
		private PictureBox pictureBox_five;
		private Panel panelRecip;
		private PictureBox pictureBox_two;
		private PictureBox pictureBox_one;
		private Label label_pers;
		private Label labelForAll;
		private PictureBox pictureBox_last;
		private Panel panelIcons;
		private Panel panelMessage;
		private Panel panelButtons;
		private Panel panelSearch;
		private Button buttonOK;
		private ToolTip toolTip;
		private Button buttonCreateList;
		private Button buttonSendAndDel;
		private Blocks.EmployeeBlock employeeBlock;
		private TextBox textBox_forAll;
		private Label label1;
		private CheckBox CheckAll;
		private Label label2;
		private PictureBox pbShowContacts;
		private Button buttonBrowseList;
		private DelableListView lvMailingList;
		private ColumnHeader chName;
		private ColumnHeader chAuthor;

		public SendMessageDialog(int docID) : this(new[] { docID }, null, false)
		{
		}

		public SendMessageDialog(int[] docIDs) : this(docIDs, null, false)
		{
		}

		public SendMessageDialog(int docID, string docString) : this(new[] { docID }, docString, false)
		{
		}

		public SendMessageDialog(int docID, string docString, bool forced) : this(new[] { docID }, docString, forced)
		{
		}

		public SendMessageDialog(int[] docIDs, string docString) : this(docIDs, docString, false)
		{
		}

		public SendMessageDialog(int[] docIDs, string docString, bool forced)
		{
			try
			{
				if(docIDs == null || docIDs.Length == 0)
					return;

				InitializeComponent();

				this.DocIDs = docIDs;
				this.docString = string.IsNullOrEmpty(docString) ? string.Empty : docString;
				this.Forced = forced;

				var sb = new StringBuilder();
				foreach(int docID in docIDs)
				{
					sb.Append(docID.ToString());
					sb.Append(',');
				}
				docIDstr = sb.ToString().TrimStart(new[] { ' ', ',' }).TrimEnd(new[] { ' ', ',' });

				buttonSendAndDel.Enabled = Environment.WorkDocData.IsInWork(docIDstr, Environment.CurEmp.ID);

				if(employeeBlock == null)
					employeeBlock = new Blocks.EmployeeBlock();
				employeeBlock.Parser = new Blocks.Parsers.EmployeeParser(Environment.EmpData, false);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		#region Accessors

		public int[] DocIDs { get; private set; }

		public bool Forced { get; private set; }

		public bool Check { get; set; }

		public int EmpID
		{
			get { return empID; }
			set
			{
				empID = value;
				buttonSendAndDel.Enabled = empID > 0;
			}
		}

		public int FolderID { get; set; }

		public string AllText
		{
			get { return textBox_forAll.Text; }
			set
			{
				if(value != null)
					textBox_forAll.Text = value;
			}
		}

		public SynchronizedCollection<int> AllUsers
		{
			get { return allMessageEmployee; }
			set
			{
				if(value != null)
				{
					allMessageEmployee = value;
					CheckEmployees();
				}
			}
		}

		public Hashtable PersonSenders
		{
			get { return personSenders; }
			set
			{
				if(value == null)
					return;
				personSenders = value;
				CheckPersonMessages();
			}
		}

		#endregion

		public void Uncheck_CheckAll()
		{
			CheckAll.Checked = false;
		}

		public void CheckSelection()
		{
			CheckAll.Checked = true;
			CheckAll.CheckState = CheckState.Checked;

			int UnCheckedCount = 0;
			bool isMe = false;
			for(int i = 0; i < recipList.Count; i++)
			{
				if(recipList[i].Equals(Environment.CurEmp))
					isMe = true;
				if(!recipList[i].CheckCommon.Checked && !recipList[i].Equals(Environment.CurEmp))
				{
					UnCheckedCount++;
					CheckAll.Checked = true;
					CheckAll.CheckState = CheckState.Indeterminate;
				}
			}
			if((isMe && UnCheckedCount == (recipList.Count - 1)) ||
				(!isMe && UnCheckedCount == recipList.Count))
			{
				CheckAll.Checked = false;
				CheckAll.CheckState = CheckState.Unchecked;
			}
		}

		public void AddUsers(List<int> empIDs)
		{
			if(recipList != null)
			for(int i = recipList.Count; i > 0; i--)
			{
				Recipient recip = recipList[i - 1];
				if(recip != null && recip.DelPic != null)
					recipList.RemoveAt(i - 1);
			}

			foreach(int id in empIDs)
			{
				Recipient recip = recipList.Find(id);
				if(recip == null)
				{
					var emp = new Employee(id, Environment.EmpData);
					if(!string.IsNullOrEmpty(emp.LongName))
						recipList.Add(emp, true, personMessage);
				}
				else
				{
					recip.CheckCommon.Checked = !recip.Equals(Environment.CurEmp);
				}
			}
		}

		/// <summary>
		///   Clean up any resources being used.
		/// </summary>
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
		///   Required method for Designer support - do not modify the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SendMessageDialog));
			this.panelRecip = new System.Windows.Forms.Panel();
			this.pictureBox_five = new System.Windows.Forms.PictureBox();
			this.labelForAll = new System.Windows.Forms.Label();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.panelIcons = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.CheckAll = new System.Windows.Forms.CheckBox();
			this.pictureBox_two = new System.Windows.Forms.PictureBox();
			this.pictureBox_one = new System.Windows.Forms.PictureBox();
			this.label_pers = new System.Windows.Forms.Label();
			this.pictureBox_last = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panelMessage = new System.Windows.Forms.Panel();
			this.textBox_forAll = new System.Windows.Forms.TextBox();
			this.pbShowContacts = new System.Windows.Forms.PictureBox();
			this.panelButtons = new System.Windows.Forms.Panel();
			this.newWindowDocumentButton = new Kesco.Lib.Win.Document.Controls.NewWindowDocumentButton(this.components);
			this.buttonCreateList = new System.Windows.Forms.Button();
			this.buttonSendAndDel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.panelSearch = new System.Windows.Forms.Panel();
			this.buttonBrowseList = new System.Windows.Forms.Button();
			this.employeeBlock = new Kesco.Lib.Win.Document.Blocks.EmployeeBlock();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.lvMailingList = new Kesco.Lib.Win.Document.ListViews.DelableListView();
			this.chName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.chAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_five)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.panelIcons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_two)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_one)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_last)).BeginInit();
			this.panelMessage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbShowContacts)).BeginInit();
			this.panelButtons.SuspendLayout();
			this.panelSearch.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelRecip
			// 
			resources.ApplyResources(this.panelRecip, "panelRecip");
			this.panelRecip.Name = "panelRecip";
			// 
			// pictureBox_five
			// 
			resources.ApplyResources(this.pictureBox_five, "pictureBox_five");
			this.pictureBox_five.Name = "pictureBox_five";
			this.pictureBox_five.TabStop = false;
			// 
			// labelForAll
			// 
			this.labelForAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources(this.labelForAll, "labelForAll");
			this.labelForAll.Name = "labelForAll";
			// 
			// pictureBox
			// 
			resources.ApplyResources(this.pictureBox, "pictureBox");
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.TabStop = false;
			// 
			// panelIcons
			// 
			this.panelIcons.Controls.Add(this.label2);
			this.panelIcons.Controls.Add(this.CheckAll);
			this.panelIcons.Controls.Add(this.pictureBox_two);
			this.panelIcons.Controls.Add(this.pictureBox_one);
			this.panelIcons.Controls.Add(this.label_pers);
			this.panelIcons.Controls.Add(this.pictureBox_last);
			this.panelIcons.Controls.Add(this.label1);
			resources.ApplyResources(this.panelIcons, "panelIcons");
			this.panelIcons.Name = "panelIcons";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Name = "label2";
			this.toolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
			this.label2.Click += new System.EventHandler(this.label2_Click);
			// 
			// CheckAll
			// 
			resources.ApplyResources(this.CheckAll, "CheckAll");
			this.CheckAll.Name = "CheckAll";
			this.toolTip.SetToolTip(this.CheckAll, resources.GetString("CheckAll.ToolTip"));
			this.CheckAll.Click += new System.EventHandler(this.CheckAll_Click);
			this.CheckAll.Enter += new System.EventHandler(this.CheckAll_Enter);
			this.CheckAll.Leave += new System.EventHandler(this.CheckAll_Leave);
			// 
			// pictureBox_two
			// 
			resources.ApplyResources(this.pictureBox_two, "pictureBox_two");
			this.pictureBox_two.Name = "pictureBox_two";
			this.pictureBox_two.TabStop = false;
			this.toolTip.SetToolTip(this.pictureBox_two, resources.GetString("pictureBox_two.ToolTip"));
			// 
			// pictureBox_one
			// 
			resources.ApplyResources(this.pictureBox_one, "pictureBox_one");
			this.pictureBox_one.Name = "pictureBox_one";
			this.pictureBox_one.TabStop = false;
			// 
			// label_pers
			// 
			this.label_pers.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources(this.label_pers, "label_pers");
			this.label_pers.Name = "label_pers";
			// 
			// pictureBox_last
			// 
			resources.ApplyResources(this.pictureBox_last, "pictureBox_last");
			this.pictureBox_last.Name = "pictureBox_last";
			this.pictureBox_last.TabStop = false;
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// panelMessage
			// 
			this.panelMessage.Controls.Add(this.textBox_forAll);
			this.panelMessage.Controls.Add(this.labelForAll);
			this.panelMessage.Controls.Add(this.pictureBox_five);
			this.panelMessage.Controls.Add(this.pictureBox);
			this.panelMessage.Controls.Add(this.pbShowContacts);
			resources.ApplyResources(this.panelMessage, "panelMessage");
			this.panelMessage.Name = "panelMessage";
			// 
			// textBox_forAll
			// 
			resources.ApplyResources(this.textBox_forAll, "textBox_forAll");
			this.textBox_forAll.Name = "textBox_forAll";
			// 
			// pbShowContacts
			// 
			resources.ApplyResources(this.pbShowContacts, "pbShowContacts");
			this.pbShowContacts.Name = "pbShowContacts";
			this.pbShowContacts.TabStop = false;
			// 
			// panelButtons
			// 
			this.panelButtons.Controls.Add(this.newWindowDocumentButton);
			this.panelButtons.Controls.Add(this.buttonCreateList);
			resources.ApplyResources(this.panelButtons, "panelButtons");
			this.panelButtons.Name = "panelButtons";
			// 
			// newWindowDocumentButton
			// 
			resources.ApplyResources(this.newWindowDocumentButton, "newWindowDocumentButton");
			this.newWindowDocumentButton.Name = "newWindowDocumentButton";
			this.newWindowDocumentButton.TabStop = false;
			this.newWindowDocumentButton.UseMnemonic = false;
			this.newWindowDocumentButton.UseVisualStyleBackColor = true;
			// 
			// buttonCreateList
			// 
			resources.ApplyResources(this.buttonCreateList, "buttonCreateList");
			this.buttonCreateList.Name = "buttonCreateList";
			this.buttonCreateList.Click += new System.EventHandler(this.buttonCreateList_Click);
			// 
			// buttonSendAndDel
			// 
			resources.ApplyResources(this.buttonSendAndDel, "buttonSendAndDel");
			this.buttonSendAndDel.Name = "buttonSendAndDel";
			this.toolTip.SetToolTip(this.buttonSendAndDel, resources.GetString("buttonSendAndDel.ToolTip"));
			this.buttonSendAndDel.Click += new System.EventHandler(this.buttonSendAndDel_Click);
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.toolTip.SetToolTip(this.buttonOK, resources.GetString("buttonOK.ToolTip"));
			this.buttonOK.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// panelSearch
			// 
			this.panelSearch.Controls.Add(this.buttonBrowseList);
			this.panelSearch.Controls.Add(this.employeeBlock);
			this.panelSearch.Controls.Add(this.buttonSendAndDel);
			this.panelSearch.Controls.Add(this.buttonOK);
			this.panelSearch.Controls.Add(this.buttonCancel);
			resources.ApplyResources(this.panelSearch, "panelSearch");
			this.panelSearch.Name = "panelSearch";
			// 
			// buttonBrowseList
			// 
			resources.ApplyResources(this.buttonBrowseList, "buttonBrowseList");
			this.buttonBrowseList.Name = "buttonBrowseList";
			this.buttonBrowseList.UseVisualStyleBackColor = true;
			this.buttonBrowseList.Click += new System.EventHandler(this.buttonBrowseList_Click);
			// 
			// employeeBlock
			// 
			resources.ApplyResources(this.employeeBlock, "employeeBlock");
			this.employeeBlock.BackColor = System.Drawing.SystemColors.Control;
			this.employeeBlock.ButtonSide = Kesco.Lib.Win.Document.Blocks.EmployeeBlock.ButtonSideEnum.Left;
			this.employeeBlock.FullText = "";
			this.employeeBlock.Name = "employeeBlock";
			this.employeeBlock.ParamStr = "clid=3&UserAccountDisabled=0&return=2";
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// lvMailingList
			// 
			this.lvMailingList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chAuthor});
			this.lvMailingList.Cursor = System.Windows.Forms.Cursors.Default;
			this.lvMailingList.FullRowSelect = true;
			this.lvMailingList.GridLines = true;
			this.lvMailingList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvMailingList.HideSelection = false;
			resources.ApplyResources(this.lvMailingList, "lvMailingList");
			this.lvMailingList.MultiSelect = false;
			this.lvMailingList.Name = "lvMailingList";
			this.lvMailingList.ShowItemToolTips = true;
			this.lvMailingList.TabStop = false;
			this.lvMailingList.UseCompatibleStateImageBehavior = false;
			this.lvMailingList.View = System.Windows.Forms.View.Details;
			this.lvMailingList.Click += new System.EventHandler(this.lvMailingList_Click);
			this.lvMailingList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvMailingList_KeyUp);
			this.lvMailingList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lvMailingList_MouseMove);
			// 
			// chName
			// 
			resources.ApplyResources(this.chName, "chName");
			// 
			// chAuthor
			// 
			resources.ApplyResources(this.chAuthor, "chAuthor");
			// 
			// SendMessageDialog
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.lvMailingList);
			this.Controls.Add(this.panelRecip);
			this.Controls.Add(this.panelIcons);
			this.Controls.Add(this.panelButtons);
			this.Controls.Add(this.panelMessage);
			this.Controls.Add(this.panelSearch);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SendMessageDialog";
			this.Activated += new System.EventHandler(this.SendMessageDialog_Activated);
			this.Load += new System.EventHandler(this.SendMessageDialog_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SendMessageDialog_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_five)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.panelIcons.ResumeLayout(false);
			this.panelIcons.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_two)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_one)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_last)).EndInit();
			this.panelMessage.ResumeLayout(false);
			this.panelMessage.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbShowContacts)).EndInit();
			this.panelButtons.ResumeLayout(false);
			this.panelSearch.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void SendMessageDialog_Load(object sender, EventArgs e)
		{
			try
			{
				{
					if(DocIDs.Length == 1)
					{
						string s = DBDocString.Format(DocIDs[0]);
						Text += " " + Environment.StringResources.GetString("Dialog_SendMessageDialog_SendMessageDialog_Load_Title1") +
														 (string.IsNullOrEmpty(s) ? "" : " " + Regex.Replace(s, @"[\n\r]", " "));
					}
					else
						Text += " " + String.Format(Environment.StringResources.GetString("Dialog_SendMessageDialog_SendMessageDialog_Load_Title2"), DocIDs.Length);
						
				}
				recipList = new RecipientList(panelRecip);
				RecipientList.Parent = this;

				personMessage = Environment.PersonMessage;
				personMessage &= DocIDs.Length == 1;

				pictureBox_one.Visible = personMessage;
				label_pers.Visible = personMessage;

				using(
					DataTable dt = (DocIDs.Length == 1)
									   ? Environment.DocData.GetDocSubscribers(DocIDs[0])
									   : Environment.DocData.GetDocSubscribers(docIDstr))
				using(DataTableReader dr = dt.CreateDataReader())
				{
					while(dr.Read())
					{
						var empID = (int)dr[Environment.DocData.WorkEmpIDField];
						var emp = new Employee(empID, Environment.EmpData);

						if(!string.IsNullOrEmpty(emp.LongName) && (recipList.Find(emp) == null))
							recipList.Add(emp, false, personMessage);
					}
					dr.Close();
					dr.Dispose();
					dt.Dispose();
				}

				bool isFaxReceiver = Environment.IsFaxReceiver();
				foreach(int docID in DocIDs)
					try
					{
						// if document is just added, adding responsible employees
						if(isFaxReceiver && Environment.CurEmp.ID.Equals(Environment.DocData.GetDocIntField(Environment.DocData.EditorField, docID)) &&
							!Environment.DocData.DocHasMessage(docID))
						{
							object obj = Environment.DocData.GetCurator(docID);
							if(obj is int && (int)obj > 0)
							{
								var emp = new Employee((int)obj, Environment.EmpData);

								if((emp.ShortName != null) && (recipList.Find(emp) == null))
									recipList.Add(emp, true, personMessage);
							}
							else
							{
								using(DataTable dt = Environment.DocData.GetDocPersonsLite(docID, true))
								// getting document persons
								using(DataTableReader dr = dt.CreateDataReader())
								{
									while(dr.Read())
									{
										var personID = (int)dr[Environment.DocData.PersonIDField];

										using(DataTable dt2 = Environment.PersonData.GetPersonEmployees(personID))
										// getting person employees
										using(DataTableReader dr2 = dt2.CreateDataReader())
										{
											while(dr2.Read())
											{
												var empID = (int)dr2[Environment.EmpData.IDField];
												var emp =
													new Employee(empID, Environment.EmpData);

												if((emp.ShortName != null) && (recipList.Find(emp) == null))
													recipList.Add(emp, true, personMessage);
											}
											dr2.Close();
											dr2.Dispose();
											dt2.Dispose();
										}
									}
									dr.Close();
									dr.Dispose();
									dt.Dispose();
								}
							}
						}
					}
					catch(Exception ex)
					{
						Log.Logger.WriteEx(new Log.DetailedException("Ошибка подгрузки сотрудников", ex));
					}

				LoadMailingList();
				snatcher = new MessageSnatcher(this);
				snatcher.LeftMouseClickOccured += snatcher_LeftMouseClickOccured;

				// if docString filled, put it into message text
				if(docString != null)
					textBox_forAll.Text = docString;

				if(Check)
				{
					CheckAll.Checked = Check;
					CheckThemAll();
				}
				CheckSelection();
				UpdateControls();
				employeeBlock.Focus();
				newWindowDocumentButton.Set(DocIDs);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, docIDstr);
			}
		}

		private void snatcher_LeftMouseClickOccured(object sender, MouseEventArgs e)
		{
			if(lvMailingList.Visible && !lvMailingList.Bounds.Contains(e.X, e.Y))
			{
				Rectangle rect = buttonBrowseList.Bounds;
				rect.Offset(buttonBrowseList.Parent.Location);

				if(!rect.Contains(e.X, e.Y))
					lvMailingList.Visible = false;
			}
		}

		private void LoadMailingList()
		{
			try
			{
				lvMailingList.BeginUpdate();
				lvMailingList.Items.Clear();

				List<MailingListItem> mls = Environment.MailingListData.GetMailingListsEx(Environment.CurEmp.ID);
				int height = 14;
				foreach(MailingListItem maileng in mls)
				{
					var item = new ListViewItem(maileng.Name);
					item.SubItems.Add(maileng.Author);
					item.Tag = maileng;

					

					lvMailingList.Items.Add(item);
				}
				if(mls.Count > 0)
					height = lvMailingList.GetItemRect(0).Height;
				lvMailingList.Columns[0].Width = lvMailingList.ClientSize.Width / 2 - 2;
				lvMailingList.Columns[1].Width = lvMailingList.ClientSize.Width / 2 - 2;

				lvMailingList.Height = mls.Count * (height + 1) + 5;
				//lvMailingList.ClientSize = new Size(lvMailingList.ClientSize.Width + 15, mls.Count * height + 4);

				if(lvMailingList.Bottom + 1 > ClientSize.Height)
				{
					lvMailingList.Height = (ClientSize.Height - lvMailingList.Top - 1);
					lvMailingList.Columns[1].Width -= 15;
				}

				buttonBrowseList.Enabled = lvMailingList.Items.Count > 0;//mls.Count != 0;

				lvMailingList.SortByColumn(Environment.UserSettings.SortMailingListByAuthor ? 1 : 0, SortOrder.Ascending);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				ErrorShower.OnShowError(this, ex.Message, StringResources.Error);
			}
			finally
			{
				lvMailingList.EndUpdate();
			}
		}

		#region Recipients

		public class Recipient : Employee
		{
			public static SendMessageDialog Parent;
			private static EmpContacts empCon;
			private TextBox label;
			private TextBox textBox;
			private RecipientPicture delPic;
			private CheckBox checkCommon;
			public bool sendPersonMessage;
			private PictureBox pbCommunication;
			private Color fontColor;

			private RecipientList list;

			public Recipient(int id, TextBox label, PictureBox pb, TextBox textBox, RecipientPicture delPic,
							 CheckBox checkCommon, RecipientList list)
				: this(id, label, pb, textBox, delPic, checkCommon, list, true)
			{
			}

			public Recipient(int id, TextBox label, PictureBox pb, TextBox textBox, RecipientPicture delPic,
							 CheckBox checkCommon, RecipientList list, bool sendPersonMessage)
				: base(id, Environment.EmpData)
			{
				this.label = label;
				pbCommunication = pb;
				this.textBox = textBox;
				if(this.textBox != null)
					this.textBox.TextChanged += textBox_TextChanged;
				this.delPic = delPic;
				this.checkCommon = checkCommon;
				this.list = list;
				this.sendPersonMessage = sendPersonMessage;

				this.label.MouseEnter += label_MouseEnter;
				this.label.MouseLeave += label_MouseLeave;
				this.label.Click += label_Click;
				fontColor = this.label.ForeColor;

				pbCommunication.Click += pbCommunication_Click;

				this.checkCommon.CheckedChanged += checkCommon_CheckedChanged;
				this.checkCommon.Click += checkCommon_Click;
				this.checkCommon.Enter += checkCommon_Enter;
				this.checkCommon.Leave += checkCommon_Leave;
			}

			public event EventHandler CheckedChanged;
			public event EventHandler TextChanged;

			#region Accessors

			public TextBox Label
			{
				get { return label; }
			}

			public TextBox TextBox
			{
				get { return textBox; }
			}

			public PictureBox DelPic
			{
				get { return delPic; }
			}

			public CheckBox CheckCommon
			{
				get { return checkCommon; }
			}

			public RecipientList List
			{
				get { return list; }
			}

			public bool SendPersonMessage
			{
				get { return sendPersonMessage; }
				set { sendPersonMessage = value; }
			}

			public PictureBox Communication
			{
				get { return pbCommunication; }
			}

			#endregion

			private void checkCommon_CheckedChanged(object sender, EventArgs e)
			{
				if(CheckedChanged != null)
					CheckedChanged(this, new EventArgs());
			}

			private void textBox_TextChanged(object sender, EventArgs e)
			{
				if(TextChanged != null)
					TextChanged(this, new EventArgs());
			}

			private void checkCommon_Click(object sender, EventArgs e)
			{
				if(!((CheckBox)sender).Checked)
					Parent.Uncheck_CheckAll();
				Parent.CheckSelection();
			}

			private void checkCommon_Enter(object sender, EventArgs e)
			{
				((CheckBox)sender).BackColor = SystemColors.ControlDark;
			}

			private void checkCommon_Leave(object sender, EventArgs e)
			{
				((CheckBox)sender).BackColor = SystemColors.Control;
			}

			private void label_MouseEnter(object sender, EventArgs e)
			{
				((TextBox)sender).ForeColor = Color.Blue;
				((TextBox)sender).Font = new Font(((TextBox)sender).Font, FontStyle.Underline);
			}

			private void label_MouseLeave(object sender, EventArgs e)
			{
				((TextBox)sender).ForeColor = fontColor;
				((TextBox)sender).Font = new Font(((TextBox)sender).Font, FontStyle.Regular);
			}

			private void label_Click(object sender, EventArgs e)
			{
				string caption = "№" + ID.ToString() + " " + label.Text;
				string url = Environment.UsersURL;
				var udb = new Web.UrlBrowseDialog(url + ID.ToString(), caption)
							  {
								  StartPosition = FormStartPosition.CenterScreen,
								  Width = 500,
								  Height = 500,
								  MaximizeBox = false,
								  MinimizeBox = false,
								  Owner = label.FindForm()
							  };
				udb.ShowDialog();
			}

			private void pbCommunication_Click(object sender, EventArgs e)
			{
				if(empCon != null)
					empCon.Close();
				empCon = new EmpContacts(this);
				empCon.Show();
			}
		}

		public class RecipientPicture : PictureBox
		{
			public Recipient Recip { get; set; }
		}

		public class RecipientList
		{
			private int xOrigin;
			private int yOrigin;
			private int yOffset;
			private int panelWidth;

			private Panel panel;
			private int number;

			private List<Recipient> list;

			private bool checks;

			private SendMessageDialog dialog;

			public RecipientList(Panel panel)
			{
				this.panel = panel;

				list = new List<Recipient>();

				xOrigin = 26;
				yOrigin = 0;
				yOffset = 24;

				number = 0;

				panelWidth = panel.Width;

				dialog = (SendMessageDialog)panel.FindForm();
			}

			#region Accessors

			public int Count
			{
				get { return list.Count; }
			}

			public Recipient this[int index]
			{
				get
				{
					if(index >= 0 && index < Count)
						return list[index];

					throw new Exception("RecipientList: " + Environment.StringResources.GetString("Index") + " " +
										index + " " + Environment.StringResources.GetString("Dialog_CheckedControlCollection_Error1") + ": " + Count + ")");
				}
			}

			public int CheckedCount
			{
				get
				{
					int count = 0;
					for(int i = 0; i < Count; i++)
						if(this[i].CheckCommon.Checked)
							count++;

					return count;
				}
			}

			public bool Checks
			{
				set
				{
					if(checks == value)
						return;
					checks = value;
					foreach(Recipient t in list)
						t.CheckCommon.Enabled = checks;
				}
			}

			public static SendMessageDialog Parent
			{
				set { Recipient.Parent = value; }
			}

			#endregion

			public void Add(Employee emp, bool delable)
			{
				Add(emp, delable, true);
			}

			public void Add(Employee emp, bool delable, bool personMessage)
			{
				var resources =
					new ResourceManager(typeof(SendMessageDialog));

				int realCount = list.Count;
				const int tabIndexOffset = 6;
				int scrollY = panel.AutoScrollPosition.Y;

				TextBox textBox = null;
				if(personMessage)
				{
					textBox = new TextBox
								  {
									  Location = new Point(xOrigin + 250, yOrigin + 6 + realCount * yOffset + scrollY),
									  Name = "textBox" + number,
									  Size = new Size(186, 20),
									  TabIndex = tabIndexOffset + number * 3 + 1,
									  Multiline = true,
									  Text = "",
									  MaxLength = 1000,
									  HideSelection = false,
									  WordWrap = true,
									  ScrollBars = ScrollBars.Vertical
								  };


					panel.Controls.Add(textBox);
				}

				var label = new TextBox
								{
									AutoSize = false,
									BackColor = SystemColors.Control,
									BorderStyle = BorderStyle.None,
									ReadOnly = true,
									WordWrap = false,
									Cursor = Cursors.Hand,
									Location = new Point(xOrigin, yOrigin + 8 + realCount * yOffset + scrollY),
									Name = "label" + number,
									Size = new Size(220, 16),
									TabIndex = tabIndexOffset + number * 3,
									Text =
										Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
											? emp.LongName
											: emp.LongEngName
								};

				if(emp.WorkState > 2)
					label.ForeColor = Color.Red;

				panel.Controls.Add(label);

				var contactsPB = new PictureBox();
				RecipientPicture delPic;

				if(delable)
				{
					delPic = new RecipientPicture
								 {
									 Image = ((Bitmap)(resources.GetObject("pictureBox.Image"))),
									 Location = new Point(xOrigin - 20, yOrigin + 8 + realCount * yOffset + scrollY),
									 Name = "pictureBox" + number,
									 Size = new Size(16, 16),
									 TabIndex = 0,
									 TabStop = false,
									 Cursor = Cursors.Hand
								 };

					delPic.Click += pictureBox_Click;

					panel.Controls.Add(delPic);
				}
				else
					delPic = null;

				var checkBox = new CheckBox
								   {
									   Checked = delable,
									   Location = new Point(xOrigin + 480, yOrigin + 8 + realCount * yOffset + scrollY),
									   Name = "checkBox" + number,
									   Size = new Size(16, 16),
									   TabIndex = tabIndexOffset + number * 3 + 2
								   };


				panel.Controls.Add(checkBox);

				var recip = new Recipient(emp.ID, label, contactsPB, textBox, delPic, checkBox, this,
												personMessage);


				if(delPic != null)
					delPic.Recip = recip;

				recip.CheckedChanged += recip_CheckedChanged;
				recip.TextChanged += recip_TextChanged;

				list.Add(recip);

				number++;

				dialog.UpdateControls();
			}

			public void RemoveAt(int index)
			{
				Recipient recip = list[index];
				list.RemoveAt(index);

				dialog.UpdateControls();

				if(recip.DelPic != null)
					panel.Controls.Remove(recip.DelPic);
				panel.Controls.Remove(recip.Label);
				panel.Controls.Remove(recip.TextBox);
				panel.Controls.Remove(recip.CheckCommon);
				panel.Controls.Remove(recip.Communication);
			}

			public Recipient Find(Employee emp)
			{
				return list.FirstOrDefault(t => t.Equals(emp));
			}

			public Recipient Find(int empID)
			{
				return list.FirstOrDefault(t => t.ID.Equals(empID));
			}

			private void pictureBox_Click(object sender, EventArgs e)
			{
				try
				{
					var pic = (RecipientPicture)sender;
					Recipient recip = pic.Recip;
					RecipientList rl = recip.List;

					int foundIndex = -1;

					for(int i = 0; i < rl.Count; i++)
					{
						recip = rl[i];
						if((recip.DelPic != null) && (recip.DelPic.Name == pic.Name))
						{
							foundIndex = i;
							break;
						}
					}

					if(foundIndex != -1)
					{
						rl.RemoveAt(foundIndex);

						for(int i = foundIndex; i < rl.Count; i++)
						{
							recip = rl[i];

							if(recip.DelPic != null)
								recip.DelPic.Top -= yOffset;
							recip.Label.Top -= yOffset;
							if(recip.TextBox != null)
								recip.TextBox.Top -= yOffset;
							recip.CheckCommon.Top -= yOffset;
						}
					}
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
					Error.ErrorShower.OnShowError(dialog, ex.Message, StringResources.Error);
				}
			}

			private void recip_CheckedChanged(object sender, EventArgs e)
			{
				dialog.UpdateControls();
			}

			private void recip_TextChanged(object sender, EventArgs e)
			{
				var recip = sender as Recipient;
				if(recip != null && recip.TextBox.Text.Length > 0)
				{
					if(dialog.personSenders.ContainsKey(recip.ID))
						dialog.personSenders[recip.ID] = recip.TextBox.Text.Trim();
					else
						dialog.personSenders.Add(recip.ID, recip.TextBox.Text);
				}
				else if(dialog.personSenders.ContainsKey(recip.ID))
					dialog.personSenders.Remove(recip.ID);
			}
		}

		private RecipientList recipList;

		#endregion

		private static bool Send(int[] ids, string idstr, string forAllMessage, SynchronizedCollection<int> forAllRecipients,
								 Hashtable personMessages)
		{
			bool badMSend = false;
			var goodPersons = new ArrayList(personMessages.Count);
			Console.WriteLine("{0}: send started. Thread: {1}", DateTime.Now.ToString("HH:mm:ss fff"), Thread.CurrentThread.GetHashCode());

			foreach(object key in personMessages.Keys)
			{
				try
				{
					badMSend = ids.Aggregate(badMSend, (current, id) => current || !Environment.MessageData.SendOneRecipientMessage(id, Environment.CurEmp.ID, personMessages[key].ToString(), (int)key, true));
				}
				catch
				{
					badMSend = true;
				}
				if(!badMSend)
					goodPersons.Add(key);

                Console.WriteLine("{0}: personal message sent (empID = {1})", DateTime.Now.ToString("HH:mm:ss fff"), key);
			}

			foreach(object t in goodPersons)
				personMessages.Remove(t);

			// отправляем общее сообщение
			if(forAllMessage.Length > 0 && forAllRecipients.Count > 0)
			{
				if(forAllRecipients.Count == 1)
				{
					try
					{
						badMSend = ids.Aggregate(badMSend, (current, id) => current || !Environment.MessageData.SendOneRecipientMessage(id, Environment.CurEmp.ID, forAllMessage, (int)forAllRecipients[0], false));
					}
					catch
					{
						badMSend = true;
					}
				}
				else
				{
					var sb = new StringBuilder();
					if(forAllRecipients.Count > 0)
					{
						sb.Append(forAllRecipients[0]);
						for(int i = 1; i < forAllRecipients.Count; i++)
						{
							sb.Append(',');
							sb.Append(forAllRecipients[i]);
						}
					}
					string recipIDsStr = sb.ToString();
					string recipStr = Environment.EmpData.GetEmployeesFIO(recipIDsStr);
					string recipENStr = Environment.EmpData.GetEmployeesIOFEn(recipIDsStr);
                    Console.WriteLine("{0}: start common message sent", DateTime.Now.ToString("HH:mm:ss fff"));
					try
					{
						badMSend = ids.Aggregate(badMSend, (current, id) => current || !Environment.MessageData.SendMessage2(id, Environment.CurEmp.ID, forAllMessage, recipIDsStr, recipStr, recipENStr, false));
					}
					catch
					{
						badMSend = true;
					}
				}
                Console.WriteLine("{0}: common message sent", DateTime.Now.ToString("HH:mm:ss fff"));
			}

			if(badMSend || personMessages.Count > 0)
			{
				int[] alres = new int[forAllRecipients.Count];
				forAllRecipients.CopyTo(alres, 0);
				
				OnNeedSendWindow(ids, ((badMSend) ? forAllMessage : null), ((badMSend) ? new SynchronizedCollection<int> (new object(), alres) : null),
								 personMessages);
				return false;
			}
			return true;
		}

		private bool CheckCanSend()
		{
			if(!((allMessageEmployee.Count != 0 && textBox_forAll.Text.Length != 0) || personSenders.Count != 0))
			{
				if(!Visible)
					Visible = true;
				if((allMessageEmployee.Count > 0 && textBox_forAll.Text.Length == 0) || personSenders.Count > 0)
					MessageBox.Show(
						Environment.StringResources.GetString("Dialog_SendMessageDialog_Send_Message1") +
						System.Environment.NewLine +
						Environment.StringResources.GetString("Dialog_SendMessageDialog_Send_Message2"),
						Environment.StringResources.GetString("InputError"));
				else
					MessageBox.Show(
						Environment.StringResources.GetString("Dialog_SendMessageDialog_Send_Message3") +
						System.Environment.NewLine +
						Environment.StringResources.GetString("Dialog_SendMessageDialog_Send_Message4"),
						Environment.StringResources.GetString("InputError"));
				return false;
			}
			return true;
		}

		private bool Send()
		{
			Visible = false;
			sendBlock = true;
			bool ret = false;
			try
			{
				if(CheckCanSend())
				{
                    Console.WriteLine("{0}: send started. Thread: {1}", DateTime.Now.ToString("HH:mm:ss fff"), Thread.CurrentThread.GetHashCode());
					string allMsg = textBox_forAll.Text.Trim();
					ret = Send(DocIDs, docIDstr, (string)allMsg.Clone(), allMessageEmployee, (Hashtable)personSenders.Clone());
					Hide();
					if(ret)
					OnMessageSend(removeWork, DocIDs);
				}
			}
			catch(Exception ex)
			{
				Visible = true;
				ret =false;
			}
			finally
			{
				if(Visible == false)
					EndWithParent(DialogResult.OK);
				sendBlock = false;
			}
			return ret;
		}

		private void buttonSend_Click(object sender, EventArgs e)
		{
			if(!sendBlock)
			{
				Send();
			}
		}

		public void UpdateControls()
		{
			bool listNotEmpty = (recipList.Count > 0);

			allMessageEmployee.Clear();

			for(int i = 0; i < recipList.Count; i++)
			{
				Recipient recip = recipList[i];
				if(recip.CheckCommon.Enabled && recip.CheckCommon.Checked)
				{
					if(!allMessageEmployee.Contains(recip.ID))
						allMessageEmployee.Add(recip.ID);
				}
			}

			labelForAll.Enabled = listNotEmpty;
			textBox_forAll.Enabled = listNotEmpty;

			buttonCreateList.Enabled = (recipList.CheckedCount > 0);
		}

		private void SendMessageDialog_Activated(object sender, EventArgs e)
		{
			Activated -= SendMessageDialog_Activated;
			employeeBlock.Focus();
		}

		private void SendMessageDialog_KeyDown(object sender, KeyEventArgs e)
		{
			if(!keyLocker.Contains(e.KeyData))
			{
				keyLocker.Add(e.KeyData);
				try
				{
					switch(e.KeyData)
					{
						case Keys.Escape:
							End(DialogResult.Cancel);
							break;
						case Keys.F7 | Keys.Control:
							SendAndDel();
							break;
						case Keys.F7:
						case Keys.Enter | Keys.Control:
							Send();
							break;
						case Keys.F4:
							CheckThemAll();
							break;
						default:
							newWindowDocumentButton.ProcessKey(e.KeyData);
							break;
					}
				}
				catch
				{
				}
				finally
				{
					keyLocker.Remove(e.KeyData);
				}
			}
		}

		/// <summary>
		///   Проверка на возможность отпавить сообщение
		/// </summary>
		private void CheckThemAll()
		{
			recipList.Checks = true;
			for(int i = 0; i < recipList.Count; i++)
			{
				Recipient recip = recipList[i];
				recip.CheckCommon.Checked = !recip.Equals(Environment.CurEmp) && CheckAll.Checked;
			}
		}

		/// <summary>
		///   Отправка и удаление из работы
		/// </summary>
		private void SendAndDel()
		{
			if(sendBlock)
				return;
			removeWork = true;
			if(Send())
			{
				if(DocIDs.Length == 1)
				{
					if(empID > 0)
						Environment.WorkDocData.RemoveDocFromWorkFolder(DocIDs[0], FolderID, empID, true);
					else
						Environment.WorkDocData.RemoveDocFromWork(DocIDs[0], Environment.CurEmp.ID);
				}
				else
				{
					if(empID > 0)
						Environment.WorkDocData.RemoveDocFromWorkFolder(docIDstr, FolderID, empID, true);
					else
						Environment.WorkDocData.RemoveDocFromWork(DocIDs, Environment.CurEmp.ID);
				}
			}
		}

		private void buttonSendAndDel_Click(object sender, EventArgs e)
		{
			SendAndDel();
		}

		private void buttonCreateList_Click(object sender, EventArgs e)
		{
			if(recipList.CheckedCount <= 0)
				return;

			var emps = new List<Employee>(recipList.CheckedCount);
			for(int i = 0; i < recipList.Count; i++)
				if(recipList[i].CheckCommon.Checked)
					emps.Add(recipList[i]);

			var dialog = new MailingListEditDialog(emps);
			dialog.DialogEvent += ML_DialogEvent;
			ShowSubForm(dialog);
			Enabled = false;
		}

		private void mailingListBlock_EmployeeSelected(object source, Blocks.EmployeeBlockEventArgs e)
		{
			if(e.Emps != null)
				OnEmployeeSelected(e.Emps);
		}

		private void OnEmployeeSelected(Employee[] empls)
		{
			foreach(Employee employee in empls)
			{
				Recipient recip = recipList.Find(employee);
				if(recip == null)
					recipList.Add(employee, !employee.Equals(Environment.CurEmp), personMessage);
				else if(!recip.Equals(Environment.CurEmp))
					recip.CheckCommon.Checked = true;
				CheckSelection();
			}
		}

		private void ML_DialogEvent(object source, DialogEventArgs e)
		{
			Enabled = true;
			Focus();

			if(e.Dialog.DialogResult == DialogResult.OK)
				LoadMailingList();
		}

		private void CheckEmployees()
		{
			int[] empIDs = allMessageEmployee.ToArray();
			for(int i = 0; i < empIDs.Length; i++)
			{
				int id = empIDs[i];
				Recipient recip = recipList.Find(id);
				if(recip == null)
					recipList.Add(new Employee(id, Environment.EmpData), true, personMessage);
				else
					recip.CheckCommon.Checked = true;
				CheckSelection();
			}
		}

		private void CheckPersonMessages()
		{
			var keys = new ArrayList(personSenders.Keys);
			foreach(int id in keys)
			{
				Recipient recip = recipList.Find(id);
				if(recip == null)
				{
					var emp = new Employee(id, Environment.EmpData);
					recipList.Add(emp, true, personMessage);
					recip = recipList.Find(emp);
				}
				recip.TextBox.Text = personSenders[id].ToString();
			}
		}

		private void CheckAll_Click(object sender, EventArgs e)
		{
			CheckThemAll();
		}

		private void CheckAll_Enter(object sender, EventArgs e)
		{
			((CheckBox)sender).BackColor = SystemColors.ControlDark;
		}

		private void CheckAll_Leave(object sender, EventArgs e)
		{
			((CheckBox)sender).BackColor = SystemColors.Control;
		}

		private void label2_Click(object sender, EventArgs e)
		{
			CheckAll.Checked = !CheckAll.Checked;
			CheckThemAll();
		}

		private void buttonBrowseList_Click(object sender, EventArgs e)
		{
			lvMailingList.Visible = !lvMailingList.Visible;
			if(lvMailingList.Visible)
				lvMailingList.Focus();
		}

		private void lvMailingList_Click(object sender, EventArgs e)
		{
			if(lvMailingList.SelectedItems.Count != 1)
				return;

			var mailing = (MailingListItem)lvMailingList.SelectedItems[0].Tag;
			OnEmployeeSelected(
				mailing.Employees.OrderBy(
					x => (Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? x.LongName : x.LongEngName))
					.ToArray());

			lvMailingList.Visible = false;
		}

		private void lvMailingList_KeyUp(object sender, KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
				case Keys.Space:
				case Keys.Enter:
					lvMailingList_Click(null, null);
					break;
				case Keys.Escape:
					lvMailingList.Visible = false;
					break;
			}
		}

		private void lvMailingList_MouseMove(object sender, MouseEventArgs e)
		{
			ListViewItem item = lvMailingList.GetItemAt(e.X, e.Y);
			if(item != null && !item.Selected)
				item.Selected = true;
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			End(System.Windows.Forms.DialogResult.Cancel);
		}
	}

	public class myEqualityComparer : IEqualityComparer
	{
		#region IEqualityComparer Members

		bool IEqualityComparer.Equals(object x, object y)
		{
			return (int)x == (int)y;
		}

		int IEqualityComparer.GetHashCode(object obj)
		{
			return (int)obj;
		}

		#endregion
	}


	public class myComperer : IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			return Math.Sign((int)x - (int)y);
		}

		#endregion
	}

	internal class MessageSnatcher : NativeWindow
	{
		public event EventHandler<MouseEventArgs> LeftMouseClickOccured;
		private const int WM_LBUTTONDOWN = 0x201;
		private const int WM_PARENTNOTIFY = 0x210;
		private readonly System.Windows.Forms.Control _control;

		public MessageSnatcher(System.Windows.Forms.Control control)
		{
			if(control.Handle != IntPtr.Zero)
				AssignHandle(control.Handle);
			else
				control.HandleCreated += OnHandleCreated;

			control.HandleDestroyed += OnHandleDestroyed;
			_control = control;
		}

		protected override void WndProc(ref Message m)
		{
			if(m.Msg == WM_PARENTNOTIFY)
			{
				if(m.WParam.ToInt64() == WM_LBUTTONDOWN)
					if(LeftMouseClickOccured != null)
					{
						Int64 lParam = m.LParam.ToInt64();

						int x = (ushort)(((uint)m.LParam) & 0xFFFF);
						int y = (ushort)((((uint)m.LParam) & 0xFFFF0000) >> 16);
						var args = new MouseEventArgs(MouseButtons.None, 0, x, y, 0);
						LeftMouseClickOccured(this, args);
					}
			}
			base.WndProc(ref m);
		}

		private void OnHandleCreated(object sender, EventArgs e)
		{
			AssignHandle(_control.Handle);
		}

		private void OnHandleDestroyed(object sender, EventArgs e)
		{
			ReleaseHandle();
		}
	}
}