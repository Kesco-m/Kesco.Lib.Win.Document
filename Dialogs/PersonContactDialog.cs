using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Controls;
using Kesco.Lib.Win.Options;
using Kesco.Lib.Win.Web;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public class PersonContactDialog : FreeDialog
    {
        private Folder subLayout;

        private Panel panel2;
        private Button buttonCancel;
        private Button buttonAdd;
        private TableLayoutPanel panelControl;
        private Panel panel3;
        private Label label1;
        private Label label2;
        private Button buttonOK;

        private Container components;

        public PersonContactDialog(int personID, string personName) : this(personID, personName, null)
        {
        }

        public PersonContactDialog(int personID, string personName, string phoneNumber)
        {
            InitializeComponent();

            subLayout = Environment.Layout.Folders.Add(Name);
            Width = subLayout.LoadIntOption("Width", Width);
            Height = subLayout.LoadIntOption("Heigth", Height);

            Collection = new CheckedControlCollection(panelControl);
            Collection.Checked += collection_Checked;

            this.PersonID = personID;
            this.PersonName = personName;

            Text += " " + personName;

            AddPersonContact(personID, phoneNumber);
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

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (PersonContactDialog));
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panelControl = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.panelControl.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.buttonAdd);
            this.panel2.Controls.Add(this.buttonOK);
            this.panel2.Controls.Add(this.buttonCancel);
            this.panel2.Name = "panel2";
            // 
            // buttonAdd
            // 
            resources.ApplyResources(this.buttonAdd, "buttonAdd");
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
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
            // panelControl
            // 
            resources.ApplyResources(this.panelControl, "panelControl");
            this.panelControl.Controls.Add(this.label2);
            this.panelControl.Name = "panelControl";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.label1);
            this.panel3.Name = "panel3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // PersonContactDialog
            // 
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panelControl);
            this.Controls.Add(this.panel2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PersonContactDialog";
            this.ShowInTaskbar = false;
            this.Closed += new System.EventHandler(this.PersonContactDialog_Closed);
            this.panel2.ResumeLayout(false);
            this.panelControl.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        #region Accessesors

        public CheckedControlCollection Collection { get; private set; }

        public int PersonID { get; private set; }

        public string PersonName { get; private set; }

        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region Load Contact

        /// <summary>
        ///   полная перезагрузка контактов от определенного лица
        /// </summary>
        /// <param name="id"> код лица </param>
        /// <param name="phoneNumber"> номер телефона </param>
        private void AddPersonContact(int id, string phoneNumber)
        {
            if (id <= 0)
                return;
            Collection.Clear();

            using (DataTable dt = Environment.FaxRecipientData.GetPersonRecipients(id))
            {
                foreach (DataRow dr in dt.Rows)
                    AddContact(dr, phoneNumber);
                dt.Dispose();
            }
            UpdateButton();
        }

        /// <summary>
        ///   перезагрузка контактов текущего лица с полной проверкой на удаление и перегрузкой измененых
        /// </summary>
        private void ReloadContact()
        {
            if (PersonID <= 0)
                return;

            using (DataTable dt = Environment.FaxRecipientData.GetPersonRecipients(PersonID))
            {
                var elementNotRemoved = new ArrayList(Collection.Count);
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[Environment.FaxRecipientData.IDField] is int)
                    {
                        var id = (int) dr[Environment.FaxRecipientData.IDField];
                        bool reload = false;
                        for (int j = 0; j < Collection.Count; j++)
                        {
                            if (Collection[j].ID != id)
                                continue;
                            var oldDR = Collection[j].Tag as DataRow;
                            if (
                                !dr[Environment.FaxRecipientData.ContactTypeField].Equals(
                                    oldDR[Environment.FaxRecipientData.ContactTypeField]) ||
                                !dr[Environment.FaxRecipientData.ContactField].Equals(
                                    oldDR[Environment.FaxRecipientData.ContactField]) ||
                                !dr[Environment.FaxRecipientData.DescriptionField].Equals(
                                    oldDR[Environment.FaxRecipientData.DescriptionField]) ||
                                !dr[Environment.FaxRecipientData.PersonLinkIDField].Equals(
                                    oldDR[Environment.FaxRecipientData.PersonLinkIDField]))
                            {
                                Collection[j].Text = "";
                                if (!(dr[Environment.FaxRecipientData.PersonLinkIDField] is DBNull))
                                {
                                    KeyValuePair<int, string> res =
                                        Environment.PersonLinkData.GetFormatedLink(
                                            (int) dr[Environment.FaxRecipientData.PersonLinkIDField], PersonID);
                                    Collection[j].InsertLink(res.Value,
                                                             Environment.PersonURL + res.Key.ToString());
                                    Collection[j].SelectedText = ": ";
                                }
                                if (!(dr[Environment.FaxRecipientData.DescriptionField] is DBNull) &&
                                    !string.IsNullOrEmpty(
                                        dr[Environment.FaxRecipientData.DescriptionField].ToString()))
                                    Collection[j].SelectedText =
                                        dr[Environment.FaxRecipientData.DescriptionField] + " ";
                                Collection[j].InsertLink(
                                    dr[Environment.FaxRecipientData.ContactField].ToString(), "#Edit");
                            }
                            Collection[j].Tag = dr;
                            Collection[j].UpdateSize();
                            reload = true;
                            elementNotRemoved.Add(id);
                        }
                        if (!reload)
                        {
                            elementNotRemoved.Add(id);
                            AddContact(dr);
                        }
                    }
                }
                dt.Dispose();

                for (int j = Collection.Count; j > 0; j--)
                {
                    if (!elementNotRemoved.Contains(Collection[j - 1].ID))
                        Collection.Remove(Collection[j - 1]);
                }
            }
            UpdateButton();
        }

        #endregion

        #region Contact Work

        /// <summary>
        ///   Добавление контакта из строки контакта
        /// </summary>
        /// <param name="dr"> строка таблицы контакта </param>
        private void AddContact(DataRow dr, string phoneNumber)
        {
            AddContact(dr, false, phoneNumber);
        }

        /// <summary>
        ///   Добавление контакта из строки контакта
        /// </summary>
        /// <param name="dr"> строка таблицы контакта </param>
        /// <param name="check"> Помечать или нет </param>
		private void AddContact(DataRow dr, bool check = false, string phoneNumber = null)
		{
			var cont = new CheckControl();
			var id = (int)dr[Environment.FaxRecipientData.IDField];
			cont.Name += id.ToString();
			cont.Collection = Collection;
			cont.EditPush += cont_EditPush;
			cont.CheckedChanged += new EventHandler(cont_CheckedChanged);
			if(!(dr[Environment.FaxRecipientData.PersonLinkIDField] is DBNull))
			{
				KeyValuePair<int, string> res = Environment.PersonLinkData.GetFormatedLink((int)dr[Environment.FaxRecipientData.PersonLinkIDField], PersonID);
				cont.InsertLink(res.Value, Environment.PersonURL + res.Key.ToString());
				cont.SelectedText = ": ";
			}
			if(!(dr[Environment.FaxRecipientData.DescriptionField] is DBNull) &&
				!string.IsNullOrEmpty(dr[Environment.FaxRecipientData.DescriptionField].ToString()))
				cont.SelectedText = dr[Environment.FaxRecipientData.DescriptionField] + " ";
			cont.InsertLink(dr[Environment.FaxRecipientData.ContactField].ToString(), "#Edit", cont.Text.Length);


			cont.Tag = dr;
			cont.ID = id;
			Collection.Add(cont);
			if(!check && dr[Environment.FaxRecipientData.ContactField].ToString().Equals(phoneNumber))
				cont.Checked = true;
			else
				cont.Checked = check;
			cont.ShowEdit = true;
		}

		void cont_CheckedChanged(object sender, EventArgs e)
		{
			var cont = sender as CheckControl;
			if(cont == null)
				return;
			if(cont.Checked)
			{
				var conts = Collection.Where(x => x.Checked && x != cont);
				if(conts.Count() > 0)
					foreach(var con in conts)
						con.Checked = false;
			}
		}

        private void cont_EditPush(object sender, EventArgs e)
        {
            if (!(sender is CheckControl))
                return;
            var cont = sender as CheckControl;
            var ccDialog = new ContactDialog(Environment.CreateContactString, "docview=yes&id=" + cont.ID.ToString() + "&idclient=" + PersonID.ToString(),
                                             Environment.StringResources.GetString("ContactEdit"));
            ccDialog.DialogEvent += ccDialog_DialogEvent;
            ccDialog.Show();
        }

        private void ccDialog_DialogEvent(object source, DialogEventArgs e)
        {
            var ccDialog = e.Dialog as ContactDialog;
            ccDialog.DialogEvent -= ccDialog_DialogEvent;
            ReloadContact();
        }

        #endregion

        private void UpdateButton()
        {
            bool exist = PersonID > 0;
            label2.Visible = (Collection.Count == 0);
            buttonAdd.Enabled = exist;
            buttonOK.Enabled = exist && Collection.CheckedCount > 0;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (Environment.PersonData.CanAddContact(PersonID))
            {
                var dialog = new ContactDialog(Environment.CreateContactString,"idClient=" + PersonID.ToString() + "&personContactCategor=3,4&docview=yes");
                dialog.DialogEvent += createContactDialog_DialogEvent;
                ShowSubForm(dialog);
            }
            else
            {
                MessageForm.Show(
                    Environment.StringResources.GetString("Dialog_PersonContactDialog_buttonAdd_Click_Warning1") +
                    System.Environment.NewLine + PersonName + System.Environment.NewLine +
                    Environment.StringResources.GetString("Dialog_PersonContactDialog_buttonAdd_Click_Warning2"),
                    Environment.StringResources.GetString("Warning"));
            }
        }

        private void collection_Checked(object sender, EventArgs e)
        {
            UpdateButton();
        }

        private void createContactDialog_DialogEvent(object source, DialogEventArgs e)
        {
            if (e.Dialog.DialogResult == DialogResult.OK)
            {
                var dialog = e.Dialog as ContactDialog;
                if (dialog != null) 
                    dialog.DialogEvent -= createContactDialog_DialogEvent;
                ReloadContact();
            }
        }

        private void PersonContactDialog_Closed(object sender, EventArgs e)
        {
            if (subLayout != null)
            {
                subLayout.Option("Width").Value = Width.ToString();
                subLayout.Option("Heigth").Value = Height.ToString();
                subLayout.Save();
            }
        }
    }
}