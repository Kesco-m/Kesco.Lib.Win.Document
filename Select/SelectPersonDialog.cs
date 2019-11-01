using System;
using System.ComponentModel;
using System.Web;
using System.Windows.Forms;
using Kesco.Lib.Win.Web;

namespace Kesco.Lib.Win.Document.Select
{
    public class SelectPersonDialog : FreeDialog
    {
        private int personID;
        private string personText;

        private Button buttonOK;
        private Button buttonCancel;
        private Button buttonFind;
        private TextBox textBoxPerson;
        private ComboBox comboBoxPerson;
        private IContainer components;

        public SelectPersonDialog(int faxID)
        {
            FaxID = faxID;
            InitializeComponent();
            comboBoxPerson.Enabled = false;
        }

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

        #region Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectPersonDialog));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonFind = new System.Windows.Forms.Button();
			this.textBoxPerson = new System.Windows.Forms.TextBox();
			this.comboBoxPerson = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
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
			this.buttonCancel.Click += new System.EventHandler(this.buttonCansel_Click);
			// 
			// buttonFind
			// 
			resources.ApplyResources(this.buttonFind, "buttonFind");
			this.buttonFind.Name = "buttonFind";
			this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
			// 
			// textBoxPerson
			// 
			resources.ApplyResources(this.textBoxPerson, "textBoxPerson");
			this.textBoxPerson.Name = "textBoxPerson";
			this.textBoxPerson.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxPerson_KeyUp);
			// 
			// comboBoxPerson
			// 
			resources.ApplyResources(this.comboBoxPerson, "comboBoxPerson");
			this.comboBoxPerson.Name = "comboBoxPerson";
			this.comboBoxPerson.TabStop = false;
			this.comboBoxPerson.DropDown += new System.EventHandler(this.comboBoxPerson_DropDown);
			this.comboBoxPerson.SelectedIndexChanged += new System.EventHandler(this.comboBoxPerson_SelectedIndexChanged);
			this.comboBoxPerson.SelectionChangeCommitted += new System.EventHandler(this.comboBoxPerson_SelectionChangeCommitted);
			// 
			// SelectPersonDialog
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonFind);
			this.Controls.Add(this.textBoxPerson);
			this.Controls.Add(this.comboBoxPerson);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectPersonDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        #region Accessors

        public int ID
        {
            get { return personID; }
        }

        public int FaxID { get; set; }

        public string PersonText
        {
            get { return textBoxPerson.Text; }
        }

        #endregion

        private void textBoxPerson_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Enter)
                return;
            FindPerson();
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            FindPersonWeb();
        }

        private void FindPerson()
        {
            var findForm = FindForm();
            if (findForm != null)
                findForm.Cursor = Cursors.WaitCursor;
            string perText = textBoxPerson.Text.Trim();
            using (var dt = Environment.PersonData.FindPersons(perText))
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count == 1)
                    {
                        AddPersonContact((int) dt.Rows[0][Environment.PersonData.IDField],
                                         dt.Rows[0][Environment.PersonData.NameField].ToString());
                    }
                    else
                    {
                        if (dt.Rows.Count < 9)
                        {
                            comboBoxPerson.Enabled = true;
                            comboBoxPerson.SelectedIndexChanged -= comboBoxPerson_SelectedIndexChanged;
                            comboBoxPerson.DisplayMember = Environment.PersonData.NameField;
                            comboBoxPerson.ValueMember = Environment.PersonData.IDField;
                            comboBoxPerson.DataSource = dt;
                            comboBoxPerson.SelectedValue = 0;
                            comboBoxPerson.Focus();
                            comboBoxPerson.DroppedDown = true;
                            comboBoxPerson.SelectedIndexChanged += comboBoxPerson_SelectedIndexChanged;
                        }
                        else
                        {
                            FindPersonWeb();
                        }
                    }
                }
                else
                    FindPersonWeb();
            }
            if (findForm != null)
                findForm.Cursor = Cursors.Default;
        }

        private void FindPersonWeb()
        {
            string perText = textBoxPerson.Text.Trim();
            string paramStr = Environment.PersonParamStr;
            if (perText.Length > 0)
                paramStr += "&search=" + HttpUtility.UrlEncode(perText);

            paramStr = paramStr.Replace("+", "%20");
            var dialog = new PersonDialog(Environment.PersonSearchString, paramStr);
            dialog.DialogEvent += PersonDialog_DialogEvent;
            ShowSubForm(dialog);
        }

        private void PersonDialog_DialogEvent(object source, DialogEventArgs e)
        {
            if (e.Dialog.DialogResult != DialogResult.OK)
                return;
            var dialog = e.Dialog as PersonDialog;
            var info = dialog.Persons[0] as PersonInfo;
            AddPersonContact(info.ID, info.Name);
        }

        private void comboBoxPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (personID > 0)
                DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCansel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AddPersonContact(int id, string name)
        {
            if (id <= 0)
                return;
            personID = id;
            personText = name;
            textBoxPerson.Text = name;
            textBoxPerson.TextChanged += textBoxPerson_TextChanged;
            buttonOK.Enabled = true;
        }

        private void textBoxPerson_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPerson.Text.Trim() == personText) 
                return;
            textBoxPerson.TextChanged -= textBoxPerson_TextChanged;
            personID = 0;
            personText = "";
            buttonOK.Enabled = false;
        }

        private void comboBoxPerson_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!(comboBoxPerson.SelectedValue is int) || (int) comboBoxPerson.SelectedValue <= 0) 
                return;
            AddPersonContact((int) comboBoxPerson.SelectedValue, comboBoxPerson.Text);
            comboBoxPerson.Enabled = false;
        }

        private void comboBoxPerson_DropDown(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }
    }
}