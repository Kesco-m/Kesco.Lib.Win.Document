using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Kesco.Lib.Win.Web;

namespace Kesco.Lib.Win.Document.Blocks
{
    public class PersonSearchBlock : UserControl
    {
        #region Events

        public event EventHandler AddContact;

        public event EventHandler CreateFaxContact;
        public event EventHandler CreateEmailContact;

        public event EventHandler CreateClient;
        public event EventHandler CreateClientPerson;

        public event EventHandler FindPerson;
        public event EventHandler FindFax;
        public event EventHandler FindEMail;
        public event EventHandler FindNothing;
        public event EventHandler FindInternal;

        public event EventHandler PersonTextChanged;

        private void OnAddContact()
        {
            if (AddContact != null)
                AddContact(this, EventArgs.Empty);
        }

        private void OnCreateFaxContact()
        {
            if (CreateFaxContact != null)
                CreateFaxContact.DynamicInvoke(new object[] {this, EventArgs.Empty});
        }

        private void OnCreateClient()
        {
            if (CreateClient != null)
                CreateClient.DynamicInvoke(new object[] {this, EventArgs.Empty});
        }

        private void OnCreateClientPerson()
        {
            if (CreateClientPerson != null)
                CreateClientPerson.DynamicInvoke(new object[] {this, EventArgs.Empty});
        }

        private void OnCreateEmailContact()
        {
            if (CreateEmailContact != null)
                CreateEmailContact.DynamicInvoke(new object[] {this, EventArgs.Empty});
        }

        private void OnFindPerson()
        {
            if (FindPerson != null)
                FindPerson(this, EventArgs.Empty);
        }

        private void OnFindFax()
        {
            if (FindFax != null)
                FindFax(this, EventArgs.Empty);
        }

        private void OnFindEMail()
        {
            if (FindEMail != null)
                FindEMail(this, EventArgs.Empty);
        }

        private void OnFindNothing()
        {
            if (FindNothing != null)
                FindNothing(this, EventArgs.Empty);
        }

        private void OnFindInternal()
        {
            if (FindInternal != null)
                FindInternal.DynamicInvoke(new object[] {this, EventArgs.Empty});
        }

        private void OnTextChanged()
        {
            base.OnTextChanged(EventArgs.Empty);
            if (PersonTextChanged != null)
                PersonTextChanged(null, null);
        }

        #endregion

        #region My parameters

        private DataTable backDS;
        private SynchronizedCollection<Keys> keyLocker;

        #endregion

        private ComboBox comboBoxPerson;
        private Button buttonFindAdd;
        private TextBox textBoxPerson;

        private Container components;

        public PersonSearchBlock()
        {
            ParamStr = "return=1&clid=27&personwheresearch=5&personforsend=1&personvalidat=" +
                       DateTime.Today.ToString("dd.MM.yyyy");
            SearchType = true;
            PersonName = "";
            keyLocker = new SynchronizedCollection<Keys>();
            InitializeComponent();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonSearchBlock));
            this.comboBoxPerson = new System.Windows.Forms.ComboBox();
            this.buttonFindAdd = new System.Windows.Forms.Button();
            this.textBoxPerson = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // comboBoxPerson
            // 
            resources.ApplyResources(this.comboBoxPerson, "comboBoxPerson");
            this.comboBoxPerson.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPerson.Name = "comboBoxPerson";
            this.comboBoxPerson.TabStop = false;
            this.comboBoxPerson.DropDown += new System.EventHandler(this.comboBoxPerson_DropDown);
            this.comboBoxPerson.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxPerson_KeyDown);
            // 
            // buttonFindAdd
            // 
            resources.ApplyResources(this.buttonFindAdd, "buttonFindAdd");
            this.buttonFindAdd.Name = "buttonFindAdd";
            this.buttonFindAdd.Click += new System.EventHandler(this.buttonFindAdd_Click);
            // 
            // textBoxPerson
            // 
            resources.ApplyResources(this.textBoxPerson, "textBoxPerson");
            this.textBoxPerson.Name = "textBoxPerson";
            this.textBoxPerson.TextChanged += new System.EventHandler(this.textBoxPerson_TextChanged);
            this.textBoxPerson.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxPerson_KeyUp);
            // 
            // PersonSearchBlock
            // 
            this.Controls.Add(this.textBoxPerson);
            this.Controls.Add(this.buttonFindAdd);
            this.Controls.Add(this.comboBoxPerson);
            this.DoubleBuffered = true;
            this.Name = "PersonSearchBlock";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Accessors

        public int PersonID { get; private set; }
        public string PersonName { get; private set; }
        public string SearchText { get; private set; }

        public string FullText
        {
            get { return textBoxPerson.Text.Trim(); }
            set { textBoxPerson.Text = (string.IsNullOrEmpty(value) ? "" : value.Trim()); }
        }

        public bool SearchType { get; set; }
        public bool IsFax { get; private set; }
        public bool IsEmail { get; private set; }

        [Localizable(true)]
        public string ButtonText
        {
            get { return buttonFindAdd.Text; }
            set { buttonFindAdd.Text = value; }
        }

        public int ButtonWight
        {
            get { return buttonFindAdd.Width; }
            set
            {
                int oldWight = buttonFindAdd.Width;
                int dW = oldWight - value;
                buttonFindAdd.Width = value;
                buttonFindAdd.Left += dW;
                textBoxPerson.Width += dW;
                comboBoxPerson.Width += dW;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ParamStr { get; set; }

        #endregion

        private void textBoxPerson_KeyUp(object sender, KeyEventArgs e)
        {
            if (!keyLocker.Contains(e.KeyData))
            {
                keyLocker.Add(e.KeyData);
                try
                {
                    if (e.KeyData == Keys.Enter && textBoxPerson.Text.Length > 0)
                    {
                        if (InvokeRequired)
                            BeginInvoke((MethodInvoker)(Find));
                        else
                            Find();
                    }
                }
                catch (Exception ex) { Data.Env.WriteToLog(ex); }
                finally
                {
                    keyLocker.Remove(e.KeyData);
                }
            }
        }

        #region Find

		private void Find()
		{
			Cursor = Cursors.WaitCursor;

			string testText = textBoxPerson.Text.Trim();

			if(SearchType)
			{
				if(IsFax)
				{
					testText = Regex.Replace(testText, "[^+0-9]", "");
				}
				if(IsEmail)
				{
					Match match = Regex.Match(testText, "([a-z0-9]+([-+._][a-z0-9]+)*@[a-z0-9]+([-._][a-z0-9]+)*)", RegexOptions.IgnoreCase);
					testText = match.Groups[0].Value;
				}
			}
			DataTable dt;
			if(SearchText != testText)
			{
				SearchText = testText;
				if(IsFax)
					testText = SearchText.TrimStart("+".ToCharArray());
				if(backDS != null)
					backDS.Dispose();
				dt = Environment.PersonData.FindPersons(testText, ((SearchType) ? 5 : 1), !SearchType);
				backDS = dt.Copy();
			}
			else
			{
				dt = backDS.Copy();
				if(comboBoxPerson.DroppedDown)
					return;
			}
			Cursor = Cursors.Default;
			Cursor.Current = Cursors.Default;
			if(dt != null && dt.Rows.Count > 1)
			{
				if(dt.Rows.Count < 11)
				{
					comboBoxPerson.SelectedIndexChanged -= comboBoxPerson_SelectedIndexChanged;

					if(SearchType && (IsEmail || IsFax))
					{
						if(Environment.EmpData.IsSecretary())
						{
							AddRow(dt, "<" + Environment.StringResources.GetString("NoContact") + ">", -4);
						}
						AddRow(dt, "<" + Environment.StringResources.GetString("CreateContact") + ">", -1);
					}
					else
					{
						AddRow(dt, "<" + Environment.StringResources.GetString("Blocks_PersonSearchBlock_Find_Message1") + ">", -2);
						AddRow(dt, "<" + Environment.StringResources.GetString("Blocks_PersonSearchBlock_Find_Message2") + ">", -3);
					}
					comboBoxPerson.Enabled = true;
					comboBoxPerson.DisplayMember = Environment.PersonData.NameField;
					comboBoxPerson.ValueMember = Environment.PersonData.IDField;
					comboBoxPerson.DataSource = dt;
					comboBoxPerson.SelectedValue = 0;
					comboBoxPerson.SelectionChangeCommitted -= comboBoxPerson_SelectionChangeCommitted;
					comboBoxPerson.SelectionChangeCommitted += comboBoxPerson_SelectionChangeCommitted;
					comboBoxPerson.SelectedIndexChanged += comboBoxPerson_SelectedIndexChanged;
					Console.WriteLine("{0}: cursor: {1}", DateTime.Now.ToString("HH:mm:ss fff"), comboBoxPerson.Cursor);
					comboBoxPerson.Focus();
					comboBoxPerson.DroppedDown = true;
				}
				else
				{
					FindPersonWeb();
				}
			}
			else
			{
				if(SearchType)
				{
					if(dt != null && dt.Rows.Count == 1)
					{
						PersonID = (int)dt.Rows[0][0];
						PersonName = dt.Rows[0][1].ToString();
						OnFindPerson();
					}
					else if(IsEmail)
					{
						OnFindEMail();
					}
					else
					{
						if(IsFax)
						{
							OnFindFax();
						}
						else
						{
							SearchText = "";
							OnFindNothing();
						}
					}
				}
				else
				{
					if(dt != null && dt.Rows.Count == 1)
					{
						PersonID = (int)dt.Rows[0][0];
						PersonName = dt.Rows[0][1].ToString();
						OnFindPerson();
					}
					else
					{
						SearchText = "";
						OnFindNothing();
					}
				}
			}
		}

        private void AddRow(DataTable table, string name, int id)
        {
            DataRow dr = table.NewRow();
            dr[Environment.PersonData.NameField] = name;
            dr[Environment.PersonData.IDField] = id;
            table.Rows.Add(dr);
        }

        public void FindPersonWeb()
        {
            string searchStr = ParamStr;
            string text = SearchText;
            if (IsFax)
                text = SearchText.TrimStart("+".ToCharArray());
            if (SearchText.Length > 0)
                searchStr += "&search=" + HttpUtility.UrlEncode(text);

            searchStr = searchStr.Replace("+", "%20");
            var dialog = new PersonDialog(Environment.PersonSearchString, searchStr);
            dialog.DialogEvent += personDialog_DialogEvent;
            dialog.Owner = FindForm();
            dialog.Show();
        }

        private void personDialog_DialogEvent(object source, DialogEventArgs e)
        {
            var dialog = e.Dialog as PersonDialog;
            if (dialog == null)
                return;
            dialog.DialogEvent -= personDialog_DialogEvent;
            if (dialog.DialogResult == DialogResult.OK)
            {
                dialog.DialogEvent -= personDialog_DialogEvent;
                var person = (PersonInfo) dialog.Persons[0];
                PersonID = person.ID;
                PersonName = person.Name;
                OnFindPerson();
            }
            else
            {
                PersonID = 0;
                if (IsFax)
                    OnFindFax();
                if (IsEmail)
                    OnFindEMail();
            }
        }

        #endregion

        private void textBoxPerson_TextChanged(object sender, EventArgs e)
        {
            IsEmail = IsFax = false;
            if (textBoxPerson.Text.Trim().Length > 0 && SearchType)
            {
                if (Regex.IsMatch(textBoxPerson.Text.Trim(),
                                  "([A-Za-z0-9]+([-+._][A-Za-z0-9]+)*@[A-Za-z0-9]+([-._][A-Za-z0-9]+)*.[A-Za-z]{2,})"))
                    IsEmail = true;
                else
                {
                    if (Regex.IsMatch(textBoxPerson.Text.Trim().Replace(" ", ""), "^(\\+\\d|\\d).*\\d.*\\d"))
                        IsFax = true;
                }
            }
            OnTextChanged();
        }

        private void comboBoxPerson_SelectionChangeCommitted(object sender, EventArgs e)
        {
            comboBoxPerson.SelectionChangeCommitted -= comboBoxPerson_SelectionChangeCommitted;
            textBoxPerson.Focus();
            comboBoxPerson.Enabled = false;
            if (comboBoxPerson.SelectedValue is int)
            {
                var testID = (int) comboBoxPerson.SelectedValue;
                if (testID > 0)
                {
                    PersonID = testID;
                    PersonName = ((DataRowView) comboBoxPerson.SelectedItem)[1].ToString();
                    OnFindPerson();
                }
                else
                {
                    switch (testID)
                    {
                        case -1:
                            if (IsFax)
                                OnCreateFaxContact();
                            else if (IsEmail)
                                OnCreateEmailContact();
                            break;
                        case -2:
                            OnCreateClient();
                            break;
                        case -3:
                            OnCreateClientPerson();
                            break;
                        case -4:
                            OnAddContact();
                            break;
                        default:
                            FindPersonWeb();
                            break;
                    }
                }
            }
        }

        private void buttonFindAdd_Click(object sender, EventArgs e)
        {
            Find();
        }

        public void Clear()
        {
            IsFax = IsEmail = false;
            textBoxPerson.Text = "";
        }

        private void comboBoxPerson_KeyDown(object sender, KeyEventArgs e)
        {
            if (comboBoxPerson.DroppedDown)
                return;
            comboBoxPerson.SelectionChangeCommitted -= comboBoxPerson_SelectionChangeCommitted;
            e.Handled = false;
            comboBoxPerson.Enabled = false;
            comboBoxPerson.SelectedValue = 0;
            textBoxPerson.Focus();
        }

        private void comboBoxPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPerson.DroppedDown)
                return;
            comboBoxPerson.SelectionChangeCommitted -= comboBoxPerson_SelectionChangeCommitted;
            comboBoxPerson.Enabled = false;
            comboBoxPerson.SelectedValue = 0;
            textBoxPerson.Focus();
        }

        private void comboBoxPerson_DropDown(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }
    }
}