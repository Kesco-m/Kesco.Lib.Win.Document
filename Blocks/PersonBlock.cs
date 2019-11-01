using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Items;
using Kesco.Lib.Win.Web;

namespace Kesco.Lib.Win.Document.Blocks
{
    public class PersonBlock : UserControl
    {
        private readonly SynchronizedCollection<Keys> _keyLocker = new SynchronizedCollection<Keys>();

        // дата документа, по которой следует фильтровать список лиц-контрагентов, если стоит флаг
        private DateTime _personValidAtDate = DateTime.Now;

        private bool _personSelectListFilter; // флаг о том, что следует фильтровать список лиц-контрагентов

        public event EventHandler CountChanged;
        public event PersonEventHandler PersonAdded;
        public event PersonEventHandler PersonRemoved;

        private List<DataRow> _persons;
        // список лиц-контрагентов, который будет выпадать пользователю с учЄтом даты документа

        #region event fired

        private void OnCountChanged()
        {
            if (CountChanged != null)
                CountChanged(this, new EventArgs());
        }

        private void OnPersonAdded(int personID)
        {
            if (PersonAdded != null)
                PersonAdded(this, new PersonEventArgs(personID));
        }

        private void OnPersonRemoved(int personID)
        {
            if (PersonRemoved != null)
                PersonRemoved(this, new PersonEventArgs(personID));
        }

        #endregion

        private bool able = true;

        private int parsedID;
        private string parsedName;
        private Keys lastKeyData = Keys.None;
        private string lastPersonText;

        private Button buttonSelect;
        private Button buttonRemove;
        private ListView list;
        private UpDownTextBox person;
        private ColumnHeader Column1;
        private Button buttonFindAll;
        private ComboBox comboPerson;
        private Container components;

        public PersonBlock()
        {
            InitializeComponent();

            if(DesignerDetector.IsComponentInDesignMode(this))
                return;

            _persons = Environment.UsedPersonsNames.ToList();

        }

        #region Accessors

        public int Count
        {
            get { return list.Items.Count; }
        }

        public ListItem this[int index]
        {
            get
            {
                if (list.Items.Count > 0 && index >= 0 && index < list.Items.Count)
                    return list.Items[index] as ListItem;
                else
                    throw new Exception("PersonBlock: " + Environment.StringResources.GetString("Index") + " " + index +
                                        " " +
                                        Environment.StringResources.GetString("Dialog_CheckedControlCollection_Error1") +
                                        ": " + Count + ")");
            }
        }

        public int[] PersonIDs
        {
            get
            {
                var ids = new int[Count];
                for (int i = 0; i < Count; i++)
                    ids[i] = this[i].ID;

                return ids;
            }
        }

        public bool Able
        {
            get { return able; }
            set
            {
                able = value;

                person.Enabled = able;
                buttonSelect.Enabled = able;
                buttonFindAll.Enabled = able;
                UpdateButtonRemove();

                list.Enabled = true;
            }
        }

        #endregion

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonBlock));
			this.buttonSelect = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.list = new System.Windows.Forms.ListView();
			this.Column1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.person = new Kesco.Lib.Win.UpDownTextBox();
			this.buttonFindAll = new System.Windows.Forms.Button();
			this.comboPerson = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// buttonSelect
			// 
			resources.ApplyResources(this.buttonSelect, "buttonSelect");
			this.buttonSelect.Name = "buttonSelect";
			this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
			// 
			// buttonRemove
			// 
			resources.ApplyResources(this.buttonRemove, "buttonRemove");
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// list
			// 
			resources.ApplyResources(this.list, "list");
			this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Column1});
			this.list.FullRowSelect = true;
			this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.list.HideSelection = false;
			this.list.HotTracking = true;
			this.list.HoverSelection = true;
			this.list.Name = "list";
			this.list.ShowItemToolTips = true;
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
			this.list.LostFocus += new System.EventHandler(this.list_FocusLost);
			this.list.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.list_MouseDoubleClick);
			// 
			// Column1
			// 
			resources.ApplyResources(this.Column1, "Column1");
			// 
			// person
			// 
			resources.ApplyResources(this.person, "person");
            this.person.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.person.HideSelection = false;
			this.person.Name = "person";
			this.person.KeyDown += new System.Windows.Forms.KeyEventHandler(this.person_KeyDown);
			this.person.KeyUp += new System.Windows.Forms.KeyEventHandler(this.person_KeyUp);
			this.person.Leave += new System.EventHandler(this.person_Leave);
			// 
			// buttonFindAll
			// 
			resources.ApplyResources(this.buttonFindAll, "buttonFindAll");
			this.buttonFindAll.Name = "buttonFindAll";
			this.buttonFindAll.Click += new System.EventHandler(this.buttonFindAll_Click);
			// 
			// comboPerson
			// 
			resources.ApplyResources(this.comboPerson, "comboPerson");
			this.comboPerson.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPerson.Name = "comboPerson";
			this.comboPerson.TabStop = false;
			this.comboPerson.DropDown += new System.EventHandler(this.comboPerson_DropDown);
			this.comboPerson.SelectionChangeCommitted += new System.EventHandler(this.comboPerson_SelectionChangeCommitted);
			this.comboPerson.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboPerson_KeyDown);
			this.comboPerson.Leave += new System.EventHandler(this.comboPerson_Leave);
			// 
			// PersonBlock
			// 
			this.Controls.Add(this.person);
			this.Controls.Add(this.list);
			this.Controls.Add(this.buttonSelect);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonFindAll);
			this.Controls.Add(this.comboPerson);
			this.DoubleBuffered = true;
			this.Name = "PersonBlock";
			resources.ApplyResources(this, "$this");
			this.Load += new System.EventHandler(this.PersonBlock_Load);
			this.ResumeLayout(false);
			this.PerformLayout();
        }

        #endregion

        private void PersonBlock_Load(object sender, EventArgs e)
        {
            list.Columns[0].Width = list.Width - 25;
        }

        private DataRow[] ParseQuery(string txt)
        {
            if (txt.Length > 0)
            {
                txt = Replacer.ReplaceRusLat(txt);
               // txt = Regex.Replace(txt, "-", " ").Trim();
                string[] w = Regex.Split(txt, @"\s+");

                if (w.Length > 0)
                {
                    var result = _persons.Where( x => x[Environment.PersonsUsedData.NameRLField].ToString().ToLower().Contains(" " + w[0].ToLower())
						|| x[Environment.PersonsUsedData.NameRLField].ToString().ToLower().StartsWith(w[0].ToLower())).ToList();

                    for (int i = 1; i < w.Length && result.Count > 0; i++)
                    {
                        result =
                            result.Where(x =>
                                x[Environment.PersonsUsedData.NameRLField].ToString().ToLower().Contains(" " +  w[i].ToLower())).ToList();
                    }

                    if (result.Count > 0)
                        return result.ToArray();
                }
            }

            return null;
        }

        public bool AddPerson(int id, string name, int position, bool is_valid)
        {
            if (!_personSelectListFilter)
                is_valid = true;

            for (int i = 0; i < Count; i++)
            {
                var old = this[i] as PersonListItem;
                if (old != null && old.ID == id)
                {
                    old.IsValid = is_valid;
                    old.UpdateStyle();

                    if (old.Removable)
                        if (position != 0)
                            old.Position = position;
                    return true;
                }
            }
            
            list.Items.Add(new PersonListItem(id, name, Environment.PersonURL + id.ToString(), position, is_valid));

            person.Clear();
            ParsePerson(false);

            OnPersonAdded(id);
            OnCountChanged();
            return true;
        }

        public bool AddPerson(int personID, string name)
        {
            bool isValid = true;
            if (_personValidAtDate != null && _personSelectListFilter)
            {
                isValid = Environment.IsPersonValid(personID, _personValidAtDate);
            }
            return AddPerson(personID, name, 0, isValid);
        }

        private void AddPersonToList(int id, string name, bool updateUsedPersons)
        {
            if (!AddPerson(id, name) || !Environment.PersonsUsedData.UsePerson(id))
                return;
            if (updateUsedPersons)
            {
                if (Environment.UsedPersonsLoader.IsBusy)
                {
                    while (Environment.UsedPersonsLoader.IsBusy){}
                    PersonListChanged(null, null);
                }
                else
                {
                    Environment.UsedPersonsLoader.RunWorkerCompleted += PersonListChanged;
                    Environment.UsedPersonsLoader.RunWorkerAsync();
                }
            }
        }

        private void PersonListChanged(object sender, RunWorkerCompletedEventArgs e)
        {
            Environment.UsedPersonsLoader.RunWorkerCompleted -= PersonListChanged;

            _persons = _personSelectListFilter ? Environment.GetUsedPersons(_personValidAtDate) : Environment.UsedPersonsNames.ToList();
        }

        private void UpdateButtonRemove()
        {
            bool flag = false;
            int count = list.SelectedItems.Count;

            if (able && count > 0)
                for (int i = 0; i < count; i++)
                {
                    var item = list.SelectedItems[i] as PersonListItem;
                    if (item != null && item.Removable)
                    {
                        flag = true;
                        break;
                    }
                }

            buttonRemove.Enabled = flag;
        }

        private void list_FocusLost(object sender, EventArgs e)
        {
            if (buttonRemove.Focused)
                return;

            for (int i = 0; i < list.Items.Count; i++)
                list.Items[i].Selected = false;

            UpdateButtonRemove();
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonRemove();
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            ChoosePerson();
        }

        private void ChoosePerson()
        {
            if (parsedID > 0)
            {
                comboPerson.DroppedDown = false;
                AddPersonToList(parsedID, parsedName, false);
            }
            else
                SelectUsedPerson();
        }

        private void SelectUsedPerson()
        {
            DataRow[] drs = ParseQuery(person.Text.Trim());

            if (drs != null && drs.Length > 0)
            {
                if (drs.Length == 1)
                    ChooseUsedPersonFromList(drs, 0);
                else
                {
                    if (drs.Length < 11)
                        ShowUsedPerson(drs);
                    else
                        SelectPerson();
                }
            }
            else
                SelectPerson();
        }

        private void ChooseUsedPersonFromList(DataRow[] drs, int index)
        {
            if (index >= 0)
            {
                DataRow dr = drs[index];
                var personID = (int) dr[Environment.PersonsUsedData.IDField];
                var personName = (string) dr[Environment.PersonsUsedData.NameField];

                AddPersonToList(personID, personName, false);
            }
        }

        private void ShowUsedPerson(DataRow[] drs)
        {
            comboPerson.Items.Clear();
            comboPerson.Cursor = Cursors.Default;
            foreach (DataRow dr in drs)
            {
                comboPerson.Items.Add(new ListItem((int) dr[Environment.PersonData.IDField],
                                        dr[Environment.PersonData.NameField].ToString()));
            }

            var fItem = new ListItem(0, Environment.StringResources.GetString("FindAll") + "...");
            comboPerson.Items.Add(fItem);
            comboPerson.DroppedDown = true;
        }

        private void SelectPerson()
        {
            string paramStr = Environment.PersonParamStr;
            if (person.Text.Length > 0)
                paramStr += "&search=" + HttpUtility.UrlEncode(person.Text);
            if (_personSelectListFilter)
                paramStr += "&personValidAt=" + _personValidAtDate.ToString("yyyyMMdd");

            paramStr = paramStr.Replace("+", "%20");

            var dialog = new PersonDialog(Environment.PersonSearchString, paramStr);
            dialog.DialogEvent += PersonDialog_DialogEvent;
            dialog.Owner = FindForm();
            dialog.Show();
        }

        private void PersonDialog_DialogEvent(object source, DialogEventArgs e)
        {
            var findForm = FindForm();
            if (findForm != null) 
                findForm.Focus();

            var dialog = e.Dialog as PersonDialog;
            if (dialog != null && dialog.DialogResult == DialogResult.OK &&
                (dialog.Persons != null && dialog.Persons.Count > 0))
            {
                var info = dialog.Persons[0] as PersonInfo;
                if (info != null)
                    AddPersonToList(info.ID, info.Name, true);
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            for (int i = list.SelectedItems.Count; i > 0; i--)
            {
                var item = list.SelectedItems[i - 1] as PersonListItem;
                if (item != null && item.Removable)
                {
                    item.Remove();
                    OnPersonRemoved(item.ID);
                }
            }
            OnCountChanged();
        }

        public void Clear()
        {
            list.Items.Clear();

            OnCountChanged();
        }

        private void person_KeyUp(object sender, KeyEventArgs e)
        {
            if (!_keyLocker.Contains(e.KeyData))
                try
                {
                    _keyLocker.Add(e.KeyData);
                    lastKeyData = e.KeyData;

                    if (e.KeyData != Keys.Enter)
                    {
                        if (lastPersonText != person.Text.Trim())
                        {
                            if (comboPerson.DroppedDown)
                                comboPerson.DroppedDown = false;
                            buttonSelect.Text = Environment.StringResources.GetString("Search") + "...";
                            if (person.Text.Trim().Length > 0)
                                ParsePerson(lastKeyData == Keys.Delete || lastKeyData == Keys.Back ||
                                            lastKeyData == Keys.OemMinus || lastKeyData == Keys.Space);
                        }
                    }

                    lastPersonText = person.Text;
                }
                catch (Exception ex)
                { Data.Env.WriteToLog(ex);}
                finally
                {
                    _keyLocker.Remove(e.KeyData);
                }
        }

        private void person_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    ChoosePerson();
                    break;
                case Keys.Up:
                    if (comboPerson.DroppedDown)
                    {
                        person.Leave -= person_Leave;
                        comboPerson.Select();
                        comboPerson.SelectedItem = comboPerson.Items[comboPerson.Items.Count - 1];
                        person.Leave += person_Leave;
                    }
                    break;
                case Keys.Down:
                    if (comboPerson.DroppedDown)
                    {
                        person.Leave -= person_Leave;
                        comboPerson.Select();
                        comboPerson.SelectedItem = comboPerson.Items[0];
                        person.Leave += person_Leave;
                    }
                    break;
            }
        }

        private void ParsePerson(bool skip)
        {
            if (!person.Lock()) 
                return;

            string txt = person.Text.Trim();
            parsedID = 0;
            DataRow[] drs = ParseQuery(txt);
            int count = 0;

            if (drs != null)
            {
                count = drs.Length;
                if (count == 1 && !skip)
                {
                    DataRow dr = drs[0];
                    var personID = (int) dr[Environment.PersonsUsedData.IDField];
                    var personName = (string) dr[Environment.PersonsUsedData.NameField];
                    var personNameRL = (string) dr[Environment.PersonsUsedData.NameRLField];

                    parsedID = personID;
                    parsedName = personName;

                    

                    string regTxt = Regex.Replace(Replacer.ReplaceRusLat(txt), @"\s+", @".*");
                    regTxt = "^(?<text>" + regTxt + ").*$";

                    Match m = Regex.Match(personNameRL, regTxt.Replace("-", " "), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
						person.Text = personName;
                        int len = m.Groups["text"].Value.Length;
                        person.Select(len, person.Text.Length - len);
                    }
                }
                if (count < 11 && count > 0)
                {
                    ShowUsedPerson(drs);
                }
            }

            switch (count)
            {
                case 0:
                    buttonSelect.Text = Environment.StringResources.GetString("Search") + "...";
                    break;
                case 1:
                    buttonSelect.Text = Environment.StringResources.GetString("Add");
                    break;
                default:
                    buttonSelect.Text = Environment.StringResources.GetString("Search") + "..." + count;
                    break;
            }

            person.Unlock();
        }

        private void buttonFindAll_Click(object sender, EventArgs e)
        {
            SelectPerson();
        }

        private void comboPerson_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboPerson.SelectedItem == null) 
                return;
            var item = comboPerson.SelectedItem as ListItem;
            if (item != null && item.ID > 0)
                AddPersonToList(item.ID, item.Text, false);
            else
                SelectPerson();
            person.Select();
        }

        private void comboPerson_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Enter && e.KeyData != Keys.Up && e.KeyData != Keys.Down && e.KeyData != Keys.PageUp &&
                e.KeyData != Keys.PageDown)
            {
                person.Select();
            }
        }

        private void comboPerson_Leave(object sender, EventArgs e)
        {
            comboPerson.DroppedDown = false;
        }

        private void person_Leave(object sender, EventArgs e)
        {
            comboPerson.DroppedDown = false;
        }

        private void comboPerson_DropDown(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        ///   ¬ызываетс€ при изменении даты документа, дл€ ограничени€ и подсветки списка контрагентов
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="d"> Ќова€ дата документа и флаг о необходимости ограничивать списки котрагентов датой </param>
        public void HandleDateBlockEvent(object sender, DateBlockEventArgs d)
        {
            _personValidAtDate = d.Value;
            _personSelectListFilter = d.FilterUsedPersons;

            _persons = _personSelectListFilter ? Environment.GetUsedPersons(_personValidAtDate) : Environment.UsedPersonsNames.ToList();

            for (int i = 0; i < Count; i++)
            {
                var person = this[i] as PersonListItem;
                if (person != null)
                {
                    person.IsValid = !_personSelectListFilter || Environment.IsPersonValid(person.ID, _personValidAtDate);
                    person.UpdateStyle();
                }
            }
        }

		private void list_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ListViewHitTestInfo hti = list.HitTest(e.Location);
			if(hti.Item != null)
				Environment.IEOpenOnURL(hti.Item.SubItems[1].Text);
		}
    }
}