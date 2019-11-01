using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Components;
using Kesco.Lib.Win.Document.Controls;
using Kesco.Lib.Win.Document.Dialogs;
using Kesco.Lib.Win.Document.Items;

namespace Kesco.Lib.Win.Document.Select
{
    public class SelectDocDialog : FreeDialog
    {
        private DataTable table;
        private int typeID;
        private string typeStr;
        private string numberStr;
        private DateTime date;
        private string personIDs;
        private int docID;
        private bool image;
        private string description = string.Empty;

        private byte searchType;
        private byte internalSearchType = 2;

        private ContextMenu contextMenu;

        private MenuItem openItem;
        private MenuItem rightsItem;
        private ListView list;
        private Button buttonCancel;
        private Label label1;
        private RadioButton radioAdd;
        private RadioButton radioNew;
        private Button buttonOK;
        private Label labelWarning;
        private RichTextBox labelX;
        private Button btnSearchByPerson;
        private Button btnSearchByNumber;
        private Label labelSame;
        private BackgroundWorker backgroundWorker;
        private Label labelWait;
        private RadioButton radioButtonAdd2Image;

        /// <summary>
        ///   Required designer variable.
        /// </summary>
        private Container components;

        public SelectDocDialog(int typeID, string typeStr, string numberStr, DateTime date, string personIDs, int docID,bool image)
            : this(null, typeID, typeStr, numberStr, date, personIDs, docID, image, string.Empty)
        {
        }

        public SelectDocDialog(DataTable table, int typeID, string typeStr, string numberStr, DateTime date,string personIDs, int docID, bool image, string description)
            : this(table, typeID, typeStr, numberStr, date, personIDs, docID, description)
        {
            this.image = image;
            if (!image)
            {
                labelWarning.Text = Environment.StringResources.GetString("Select_SelectDocDialog_Message1");
                radioAdd.Text = radioAdd.Text.Replace(Environment.StringResources.GetString("Image"),Environment.StringResources.GetString("Select_SelectDocDialog_Message2"));
                radioButtonAdd2Image.Enabled = false;
            }
        }

        public SelectDocDialog(DataTable table, int typeID, string typeStr, string numberStr, DateTime date,string personIDs, int docID, string description)
        {
            InitializeComponent();

            this.table = table;
            this.typeID = typeID;
            this.typeStr = typeStr;
            this.numberStr = numberStr;
            this.date = date;
            image = true;
            this.personIDs = personIDs;
            this.docID = docID;
            this.description = description;

            // контекстное меню
            contextMenu = new ContextMenu();

            // пункты меню
            openItem = new MenuItem();
            rightsItem = new MenuItem();

            // открыть
            openItem.Text = Environment.StringResources.GetString("Show");
            openItem.Click += openItem_Click;

            // права
            rightsItem.Text = Environment.StringResources.GetString("Rights");
            rightsItem.Click += rightsItem_Click;
        }

        #region Accessors

        public DocListItem SelectedItem
        {
            get { return list.SelectedItems.Count > 0 ? (DocListItem) list.SelectedItems[0] : null; }
        }

        public bool ReturnEform { get; set; }

        public byte SearchType
        {
            set
            {
                searchType = value;
                //если тип 2, то видимость остаётся у 2х кнопок
                btnSearchByPerson.Visible = true;
                btnSearchByNumber.Visible = true;
                if (value.Equals(1))
                    btnSearchByNumber.Visible = false;
                if (value.Equals(0))
                {
                    btnSearchByPerson.Visible = false;
                    btnSearchByNumber.Visible = false;
                }
            }
            get { return searchType; }
        }

        #endregion

        /// <summary>
        ///   Clean up any resources being used.
        /// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectDocDialog));
			this.list = new System.Windows.Forms.ListView();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.radioAdd = new System.Windows.Forms.RadioButton();
			this.radioNew = new System.Windows.Forms.RadioButton();
			this.labelWarning = new System.Windows.Forms.Label();
			this.labelX = new System.Windows.Forms.RichTextBox();
			this.btnSearchByPerson = new System.Windows.Forms.Button();
			this.btnSearchByNumber = new System.Windows.Forms.Button();
			this.labelSame = new System.Windows.Forms.Label();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.labelWait = new System.Windows.Forms.Label();
			this.radioButtonAdd2Image = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// list
			// 
			resources.ApplyResources(this.list, "list");
			this.list.FullRowSelect = true;
			this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.list.HideSelection = false;
			this.list.MultiSelect = false;
			this.list.Name = "list";
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
			this.list.DoubleClick += new System.EventHandler(this.list_DoubleClick);
			this.list.MouseUp += new System.Windows.Forms.MouseEventHandler(this.list_MouseUp);
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
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// radioAdd
			// 
			resources.ApplyResources(this.radioAdd, "radioAdd");
			this.radioAdd.Checked = true;
			this.radioAdd.Name = "radioAdd";
			this.radioAdd.TabStop = true;
			this.radioAdd.CheckedChanged += new System.EventHandler(this.radioAdd_CheckedChanged);
			// 
			// radioNew
			// 
			resources.ApplyResources(this.radioNew, "radioNew");
			this.radioNew.Name = "radioNew";
			this.radioNew.CheckedChanged += new System.EventHandler(this.radioNew_CheckedChanged);
			// 
			// labelWarning
			// 
			resources.ApplyResources(this.labelWarning, "labelWarning");
			this.labelWarning.Name = "labelWarning";
			// 
			// labelX
			// 
			resources.ApplyResources(this.labelX, "labelX");
			this.labelX.BackColor = System.Drawing.SystemColors.Control;
			this.labelX.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.labelX.Cursor = System.Windows.Forms.Cursors.Default;
			this.labelX.DetectUrls = false;
			this.labelX.Name = "labelX";
			this.labelX.ReadOnly = true;
			this.labelX.TabStop = false;
			// 
			// btnSearchByPerson
			// 
			resources.ApplyResources(this.btnSearchByPerson, "btnSearchByPerson");
			this.btnSearchByPerson.Name = "btnSearchByPerson";
			this.btnSearchByPerson.Click += new System.EventHandler(this.btnSearchByPerson_Click);
			// 
			// btnSearchByNumber
			// 
			resources.ApplyResources(this.btnSearchByNumber, "btnSearchByNumber");
			this.btnSearchByNumber.Name = "btnSearchByNumber";
			this.btnSearchByNumber.Click += new System.EventHandler(this.btSearchByNumber_Click);
			// 
			// labelSame
			// 
			resources.ApplyResources(this.labelSame, "labelSame");
			this.labelSame.Name = "labelSame";
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
			// 
			// labelWait
			// 
			resources.ApplyResources(this.labelWait, "labelWait");
			this.labelWait.ForeColor = System.Drawing.Color.Red;
			this.labelWait.Name = "labelWait";
			// 
			// radioButtonAdd2Image
			// 
			resources.ApplyResources(this.radioButtonAdd2Image, "radioButtonAdd2Image");
			this.radioButtonAdd2Image.Name = "radioButtonAdd2Image";
			// 
			// SelectDocDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.labelWait);
			this.Controls.Add(this.labelSame);
			this.Controls.Add(this.btnSearchByNumber);
			this.Controls.Add(this.btnSearchByPerson);
			this.Controls.Add(this.labelX);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.radioNew);
			this.Controls.Add(this.labelWarning);
			this.Controls.Add(this.list);
			this.Controls.Add(this.radioAdd);
			this.Controls.Add(this.radioButtonAdd2Image);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectDocDialog";
			this.Load += new System.EventHandler(this.ChooseDocDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

        #endregion

        private void UpdateControls()
        {
            list.Visible = !radioNew.Checked;

            if (radioNew.Checked)
            {
                list.SelectedItems.Clear();
                buttonOK.Enabled = true;
            }
            else
            {
                buttonOK.Enabled = (list.SelectedItems.Count > 0);
            }

            if (image && list.SelectedItems.Count == 1)
            {
                var item = list.SelectedItems[0] as DocListItem;
                if (item != null)
                {
                    radioButtonAdd2Image.Enabled = item.Rights && Environment.DocImageData.DocHasImages(item.ID, false);
                    if (!radioButtonAdd2Image.Enabled && radioButtonAdd2Image.Checked)
                        radioAdd.Checked = true;
                }
            }

            btnSearchByPerson.FlatStyle = internalSearchType == 0 ? FlatStyle.Flat : FlatStyle.System;
            btnSearchByNumber.FlatStyle = internalSearchType == 1 ? FlatStyle.Flat : FlatStyle.System;
        }

        private void ChooseDocDialog_Load(object sender, EventArgs e)
        {
            if (table == null)
            {
                btnSearchByPerson.Enabled = btnSearchByNumber.Enabled = radioAdd.Enabled = radioNew.Enabled = false;
                labelWait.Visible = true;
                backgroundWorker.RunWorkerAsync();
            }
            else
                FillLV();
            btnSearchByPerson.Visible = searchType > 0;
            labelSame.Visible = btnSearchByPerson.Visible;
            btnSearchByNumber.Visible = searchType > 1;
            UpdateControls();
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

		private void list_MouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button != MouseButtons.Right || contextMenu == null)
				return;
			contextMenu.MenuItems.Clear();

			DocListItem item = null;
			try
			{
				item = list.GetItemAt(e.X, e.Y) as DocListItem;
			}
			catch
			{
			}
			if(item != null)
			{
				item.Selected = true;

				if(item.Rights)
					contextMenu.MenuItems.AddRange(new MenuItem[] { openItem, rightsItem });
				else
					contextMenu.MenuItems.AddRange(new MenuItem[] { rightsItem });

				if(contextMenu.MenuItems.Count > 0)
					contextMenu.Show(this, new Point(e.X + list.Left, e.Y + list.Top));
			}
		}

        private void openItem_Click(object sender, EventArgs e)
        {
            //send open
            if (list.SelectedItems.Count > 0)
            {
                var item = (DocListItem) list.SelectedItems[0];

                Environment.OnNewWindow(this, new DocumentSavedEventArgs(item.ID, -1));
            }
        }

        private void rightsItem_Click(object sender, EventArgs e)
        {
            if (list.SelectedItems.Count > 0)
            {
                var item = (DocListItem) list.SelectedItems[0];

                var dialog = new DocUserListDialog(item.ID);
                dialog.Show();
            }
        }

        private void radioAdd_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void radioNew_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void list_DoubleClick(object sender, EventArgs e)
        {
            var item = list.SelectedItems[0] as DocListItem;
            if (item != null && item.Rights)
                openItem_Click(sender, e);
        }

		private void buttonOK_Click(object sender, EventArgs e)
		{
			if(!radioNew.Checked)
				if(list.SelectedItems.Count > 0)
				{
					var item = (DocListItem)list.SelectedItems[0];
					if(!item.Rights)
					{
						MessageBox.Show(Environment.StringResources.GetString("Select_SelectDocDialog_buttonOK_Click_Message1") + "\n\n" +
							Environment.StringResources.GetString("Select_SelectDocDialog_buttonOK_Click_Message2"),
							Environment.StringResources.GetString("NotEnoughRights"));
						return;
					}
					if(!image && !ReturnEform && item.HaveData)
					{
						MessageBox.Show(Environment.StringResources.GetString("Select_SelectDocDialog_buttonOK_Click_Message3") + "\n\n" +
							Environment.StringResources.GetString("Select_SelectDocDialog_buttonOK_Click_Message4"),
							Environment.StringResources.GetString("Error"));
						return;
					}
					{
						// Если у документов разное описание, выбрать какое-то одно
						string firstDocDescription = docID == 0 ? description : Environment.DocData.GetField(Environment.DocData.DescriptionField, docID).ToString();
						string secondDocDescription = Environment.DocData.GetField(Environment.DocData.DescriptionField, item.ID).ToString();

						bool firstDescrStr = !string.IsNullOrEmpty(firstDocDescription);
						bool secondDescrStr = !string.IsNullOrEmpty(secondDocDescription);

						if(firstDescrStr && secondDescrStr)
						{
							var dlg = new SimilarDocsDescriptionDialog(
								firstDocDescription, secondDocDescription);
							if(dlg.ShowDialog(this) == DialogResult.OK)
							{
								description = dlg.Description;
							}
							else
								return;
						}
						else if(firstDescrStr && !secondDescrStr)
						{
							description = firstDocDescription;
						}
						else if(!firstDescrStr && secondDescrStr)
						{
							description = secondDocDescription;
						}
						else if(!firstDescrStr && !secondDescrStr)
						{
							description = string.Empty;
						}
					}
					if(image && radioButtonAdd2Image.Checked)
					{
						End(DialogResult.Yes);
						return;
					}
				}

			End(DialogResult.OK);
		}

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void btnSearchByPerson_Click(object sender, EventArgs e)
        {
            if (internalSearchType != 0)
                internalSearchType = 0;
            else
                internalSearchType = 2;
            UpdateControls();
            Search();
        }

        private void btSearchByNumber_Click(object sender, EventArgs e)
        {
            internalSearchType = (byte) (internalSearchType != 1 ? 1 : 2);
            UpdateControls();
            Search();
        }

        private void Search()
        {
            int search = -1;
            if (internalSearchType != 2)
                search = internalSearchType;
            if (table != null)
                table.Dispose();
            table = Environment.DocData.GetSimilarDocs(typeID, numberStr, date, personIDs, docID, search);

            FillLV();
        }

        private void FillLV()
        {
            int count = 0;

            if (table != null)
                count = table.Rows.Count;

            var msg = new StringBuilder();
            msg.Append(Environment.StringResources.GetString("InArchiv"));
            msg.Append(" ");
            msg.Append(count == 1
                           ? Environment.StringResources.GetString("Find")
                           : Environment.StringResources.GetString("FindPlural"));
            msg.Append(" " + count + " ");
            list.Items.Clear();
            if (count > 0)
            {
                radioAdd.Enabled = true;
                radioAdd.Checked = true;

                switch (count%10)
                {
                    case 1:
                        msg.Append(
                            Environment.StringResources.GetString("Select_SelectDocDialog_ChooseDocDialog_Load_Message1"));
                        break;

                    case 2:
                    case 3:
                    case 4:
                        msg.Append(
                            Environment.StringResources.GetString("Select_SelectDocDialog_ChooseDocDialog_Load_Message2"));
                        break;

                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        msg.Append(
                            Environment.StringResources.GetString("Select_SelectDocDialog_ChooseDocDialog_Load_Message3"));
                        break;
                }

                msg.Append(" " + Environment.StringResources.GetString("On") + " " + typeStr + " " +
                           Environment.StringResources.GetString("Num"));

                msg.Append(numberStr != "" ? numberStr : Environment.StringResources.GetString("NoNumber"));

                msg.Append(" " + Environment.StringResources.GetString("Of") + " ");

                msg.Append(date > DateTime.MinValue
                               ? date.ToString("dd.MM.yyyy")
                               : Environment.StringResources.GetString("NoData"));

                if (list.Columns.Count == 0)
                {
                    list.Columns.Add(Environment.StringResources.GetString("ID"), 60, HorizontalAlignment.Left);
                    list.Columns.Add(Environment.StringResources.GetString("DocumentName"), 120,
                                     HorizontalAlignment.Left);
                    list.Columns.Add(Environment.StringResources.GetString("DocumentNumber"), 80,
                                     HorizontalAlignment.Left);
                    list.Columns.Add(Environment.StringResources.GetString("Description"), 180, HorizontalAlignment.Left);
                    list.Columns.Add(Environment.StringResources.GetString("AccessToEdit"), 130,
                                     HorizontalAlignment.Center);
                }

                bool lang = table.Columns.Contains(Environment.DocData.DocTypeEngField);
                for (int i = 0; i < count; i++)
                {
                    DataRow dr = table.Rows[i];

                    var _docID = (int) dr[Environment.DocData.IDField];

                    var values = new string[5];

                    // код документа
                    values[0] = _docID.ToString();

                    // тип документа
                    if (string.IsNullOrEmpty(dr[Environment.DocData.NameField].ToString()))
                        values[1] = lang && !Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
                                        ? dr[Environment.DocData.DocTypeEngField].ToString()
                                        : dr[Environment.DocData.DocTypeField].ToString();
                    else
                        values[1] = dr[Environment.DocData.NameField].ToString();

                    object obj;

                    // номер
                    obj = dr[Environment.DocData.NumberField];
                    if (!obj.Equals(DBNull.Value))
                        values[2] = obj.ToString();

                    // описание
                    obj = dr[Environment.DocData.DescriptionField];
                    if (!obj.Equals(DBNull.Value))
                        values[3] = obj.ToString();

                    // доступен?
                    var avail = (int) dr[Environment.DocData.RightsField];
                    bool rights = (avail == 1);
                    if (rights)
                        values[4] = Environment.StringResources.GetString("Available");
                    else
                        values[4] = Environment.StringResources.GetString("NotAvailable");
                    var hd = (int) dr[Environment.DocData.HaveDataField];
                    bool have = (hd == 1);
                    if (!image && have)
                        values[4] += ", " + Environment.StringResources.GetString("HaveEForm");

                    var item = new DocListItem(_docID, rights, values);
                    if (!image)
                        item.HaveData = have;
                    if (!item.Rights)
                        item.ForeColor = Color.Gray;

                    list.Items.Add(item);
                }
                // сортируем по коду в обратном порядке
                list.Sorting = SortOrder.Descending;
            }
            else
            {
                radioAdd.Enabled = false;
                radioNew.Checked = true;
            }
            labelX.Text = msg.ToString();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = Environment.DocData.GetSimilarDocs(typeID, numberStr, date, personIDs, docID, searchType);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            table = (DataTable) e.Result;
            FillLV();
            btnSearchByPerson.Enabled = btnSearchByNumber.Enabled = radioAdd.Enabled = radioNew.Enabled = true;
            labelWait.Visible = false;
        }
    }
}