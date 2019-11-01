using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Items;

namespace Kesco.Lib.Win.Document.Blocks
{
    public delegate void PersonEventHandler(object source, PersonEventArgs e);

    public partial class PersonBaseBlock : UserControl
    {
        private SynchronizedCollection<Keys> keyLocker;
        private Keys lastKeyData;
        private string lastPersonText;

        private int parsedID;
        private string parsedName;

        private string buttonMainText;

        private SynchronizedCollection<DataRow> persons;

        public event PersonEventHandler PersonFound;
        public event EventHandler CountChanged;

        protected internal void OnPersonFound(int id)
        {
            if (PersonFound != null)
                PersonFound(this, new PersonEventArgs(id));
        }

        protected internal void OnCountChanged()
        {
            if (CountChanged != null)
                CountChanged(this, EventArgs.Empty);
        }

        public PersonBaseBlock()
        {
            InitializeComponent();
            keyLocker = new SynchronizedCollection<Keys>();
        }

        #region Accessories

        public string ButtonMainText
        {
            get { return buttonMainText; }
            set
            {
                buttonMainText = value;
                if (DesignerDetector.IsComponentInDesignMode(this))
                    buttonSearch.Text = value;
            }
        }

        public string ButtonFindText { get; set; }

        public bool ClearFound { get; set; }

        #endregion

        private void PersonBaseBlock_Load(object sender, EventArgs e)
        {
            if (Environment.UsedPersonsNames == null)
            {
				if(Environment.UsedPersonsLoader.IsBusy)
				{
					Environment.UsedPersonsLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(UsedPersonsLoader_RunWorkerCompleted);
					this.Enabled = false;
				}
				if(!Environment.UsedPersonsLoader.IsBusy)
					LoaderCompleted();
            }
        }

		void UsedPersonsLoader_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			LoaderCompleted();
		}

		private void LoaderCompleted()
		{
			Environment.UsedPersonsLoader.RunWorkerCompleted -= new System.ComponentModel.RunWorkerCompletedEventHandler(UsedPersonsLoader_RunWorkerCompleted);
			this.Enabled = true;
			persons = Environment.UsedPersonsNames;
		}

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    ChoosePerson();
                    break;
                case Keys.Up:
                    if (showBox.DroppedDown)
                    {
                        searchBox.Leave -= searchBox_Leave;
                        showBox.Select();
                        showBox.SelectedItem = showBox.Items[showBox.Items.Count - 1];
                        searchBox.Leave += searchBox_Leave;
                    }
                    break;
                case Keys.Down:
                    if (showBox.DroppedDown)
                    {
                        searchBox.Leave -= searchBox_Leave;
                        showBox.Select();
                        showBox.SelectedItem = showBox.Items[0];
                        searchBox.Leave += searchBox_Leave;
                    }
                    break;
            }
        }

        private void ChoosePerson()
        {
            throw new NotImplementedException();
        }

        private void searchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!keyLocker.Contains(e.KeyData))
                try
                {
                    keyLocker.Add(e.KeyData);

                    lastKeyData = e.KeyData;

                    if (e.KeyData != Keys.Enter)
                    {
                        if (lastPersonText != searchBox.Text.Trim())
                        {
                            if (showBox.DroppedDown)
                                showBox.DroppedDown = false;
                            buttonSearch.Text = buttonMainText + "...";
                            if (searchBox.Text.Trim().Length > 0)
                                ParsePerson(lastKeyData == Keys.Delete || lastKeyData == Keys.Back ||
                                            lastKeyData == Keys.OemMinus || lastKeyData == Keys.Space);
                        }
                    }

                    lastPersonText = searchBox.Text;
                }
                catch (Exception ex)
                { Data.Env.WriteToLog(ex); }
                finally
                {
                    keyLocker.Remove(e.KeyData);
                }
        }

        private void ParsePerson(bool skip)
        {
            if (!searchBox.Lock())
                return;

            string txt = searchBox.Text.Trim();
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

                    searchBox.Text = personName;

                    string regTxt = Regex.Replace(Replacer.ReplaceRusLat(txt), @"\s+", @".*");
                    regTxt = "^(?<text>" + regTxt + ").*$";

                    Match m = Regex.Match(personNameRL, regTxt.Replace("-", " "), RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        int len = m.Groups["text"].Value.Length;
                        searchBox.Select(len, searchBox.Text.Length - len);
                    }
                }
                if (count < 11 && count > 0)
                    ShowUsedPerson(drs);
            }

            switch (count)
            {
                case 0:
                    buttonSearch.Text = buttonMainText + "...";
                    break;
                case 1:
                    buttonSearch.Text = ButtonFindText;
                    break;
                default:
                    buttonSearch.Text = buttonMainText + "..." + count;
                    break;
            }

            searchBox.Unlock();
        }

        private void ShowUsedPerson(DataRow[] drs)
        {
            showBox.Items.Clear();
            showBox.Cursor = Cursors.Default;
            foreach (DataRow t in drs)
                showBox.Items.Add(new ListItem((int) t[Environment.PersonData.IDField],
                                               t[Environment.PersonData.NameField].ToString()));

            var fItem = new ListItem(0, Environment.StringResources.GetString("FindAll") + "...");
            showBox.Items.Add(fItem);
            showBox.DroppedDown = true;
        }

        private DataRow[] ParseQuery(string txt)
        {
            if (txt.Length > 0)
            {
                txt = Replacer.ReplaceRusLat(txt);
                txt = Regex.Replace(txt, "-", " ").Trim();
                string[] w = Regex.Split(txt, @"\s+");

                if (w.Length > 0)
                {
                    List<DataRow> result =persons.Where(x =>x[Environment.PersonsUsedData.NameRLField].ToString().ToLower().StartsWith(w[0].ToLower())).ToList();
                    for (int i = 1; i < w.Length && result.Count > 0; i++)
                        result =result.Where(x =>x[Environment.PersonsUsedData.NameRLField].ToString().ToLower().Contains(" " +w[i].ToLower())).ToList();

                    if (result.Count > 0)
                        return result.ToArray();
                }
            }

            return null;
        }

        private void searchBox_Leave(object sender, EventArgs e)
        {
            showBox.DroppedDown = false;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
        }

        private void showBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Enter && e.KeyData != Keys.Up && e.KeyData != Keys.Down && e.KeyData != Keys.PageUp &&
                e.KeyData != Keys.PageDown)
            {
                searchBox.Select();
            }
        }

        private void showBox_Leave(object sender, EventArgs e)
        {
            showBox.DroppedDown = false;
        }

        private void showBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (showBox.SelectedItem != null)
            {
                var item = showBox.SelectedItem as ListItem;
                if (item != null && item.ID > 0)
                    AddPersonToList(item.ID, item.Text);
                else
                    SelectPerson();
                searchBox.Select();
            }
        }

        private void AddPersonToList(int personID, string personName)
        {
            if (ClearFound)
                searchBox.Clear();
            else
                searchBox.Text = personName;
            ParsePerson(false);

            OnPersonFound(personID);
            OnCountChanged();
        }

        private void SelectPerson()
        {
            throw new NotImplementedException();
        }

        private void showBox_DropDown(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }
    }

    public enum SearchType
    {
        NoSearch,
        SearchEnter,
        SearchTyped,
        SearchTypedAndEnter
    }

    public class PersonEventArgs : EventArgs
    {
        public PersonEventArgs(int personID)
        {
            PersonID = personID;
        }

        public int PersonID { get; private set; }
    }
}