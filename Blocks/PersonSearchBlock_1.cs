using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Items;

namespace Kesco.Lib.Win.Document.Blocks
{
	public partial class PersonSearchBlock_1 : UserControl
	{
		public event EventHandler FindPerson;
		public event EventHandler PersonTextChanged;

		private void OnFindPerson()
		{
			if(FindPerson != null)
				FindPerson(this, EventArgs.Empty);
		}

		private void OnTextChanged()
		{
			base.OnTextChanged(EventArgs.Empty);
			if(PersonTextChanged != null)
				PersonTextChanged(null, null);
		}

		public string FullText
		{
			get { return textBoxPerson.Text.Trim(); }
			set
			{
				textBoxPerson.TextChanged -= textBoxPerson_TextChanged;
				textBoxPerson.Text = (string.IsNullOrEmpty(value) ? "" : value.Trim());
				textBoxPerson.SelectAll();
				textBoxPerson.TextChanged += textBoxPerson_TextChanged;
			}
		}

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

		private SynchronizedCollection<Keys> keyLocker;
		private SynchronizedCollection<DataRow> persons;

		private Keys lastKeyData = Keys.None;
		private string lastPersonText;

		public PersonSearchBlock_1()
		{
			InitializeComponent();

			keyLocker = new SynchronizedCollection<Keys>();
		}

		#region Accessors

		public int PersonID { get; private set; }
		public string PersonName { get; private set; }

		#endregion

		private void PersonBlock_Load(object sender, EventArgs e)
		{
			if(DesignerDetector.IsComponentInDesignMode(this))
				return;
			LoadUsedPersons();
		}

		private bool LoadUsedPersons()
		{
			if(Environment.UsedPersonsNames == null)
			{
				if(Environment.UsedPersonsLoader.IsBusy)
				{
					Environment.UsedPersonsLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UsedPersonsLoader_RunWorkerCompleted);
					this.Enabled = false;
				}
			}
			if(!Environment.UsedPersonsLoader.IsBusy)
				LoaderCompleted();
			return true;
		}

		void UsedPersonsLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			LoaderCompleted();
		}

		private void LoaderCompleted()
		{
			Environment.UsedPersonsLoader.RunWorkerCompleted -= new System.ComponentModel.RunWorkerCompletedEventHandler(UsedPersonsLoader_RunWorkerCompleted);
			this.Enabled = true;
			persons = Environment.UsedPersonsNames;
		}

		private void person_KeyUp(object sender, KeyEventArgs e)
		{
			if(!keyLocker.Contains(e.KeyData))
			{
				keyLocker.Add(e.KeyData);
				try
				{
					lastKeyData = e.KeyData;

					if(e.KeyData != Keys.Enter)
					{
						if(lastPersonText != textBoxPerson.Text.Trim())
						{
							if(comboBoxPerson.DroppedDown)
								comboBoxPerson.DroppedDown = false;
							if(textBoxPerson.Text.Trim().Length > 0)
								ParsePerson(lastKeyData == Keys.Delete || lastKeyData == Keys.Back ||
											lastKeyData == Keys.OemMinus || lastKeyData == Keys.Space);
						}
					}

					lastPersonText = textBoxPerson.Text;

					if(e.KeyData == Keys.Enter)
						ChoosePerson();
				}
				catch(Exception ex) { Data.Env.WriteToLog(ex); }
				finally
				{
					keyLocker.Remove(e.KeyData);
				}
			}
		}

		private void person_KeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
				case Keys.Up:
					if(comboBoxPerson.DroppedDown)
					{
						textBoxPerson.Leave -= person_Leave;
						comboBoxPerson.Select();
						comboBoxPerson.SelectedItem = comboBoxPerson.Items[comboBoxPerson.Items.Count - 1];
						textBoxPerson.Leave += person_Leave;
					}
					break;
				case Keys.Down:
					if(comboBoxPerson.DroppedDown)
					{
						textBoxPerson.Leave -= person_Leave;
						comboBoxPerson.Select();
						comboBoxPerson.SelectedItem = comboBoxPerson.Items[0];
						textBoxPerson.Leave += person_Leave;
					}
					break;
			}
		}

		private void person_Leave(object sender, EventArgs e)
		{
			comboBoxPerson.DroppedDown = false;
		}

		private void buttonSelect_Click(object sender, EventArgs e)
		{
			ChoosePerson();
		}

		private void ChoosePerson()
		{
			if(PersonID > 0)
			{
				comboBoxPerson.DroppedDown = false;
				AddPersonToList(PersonID, PersonName);
			}
			else
				SelectUsedPerson();
		}

		private void ChooseUsedPersonFromList(DataRow[] drs, int index)
		{
			if(index >= 0)
			{
				DataRow dr = drs[index];
				var personID = (int)dr[Environment.PersonsUsedData.IDField];
				var personName = (string)dr[Environment.PersonsUsedData.NameField];

				AddPersonToList(personID, personName);
			}
		}

		private void ShowUsedPerson(DataRow[] drs)
		{
			comboBoxPerson.Items.Clear();
			comboBoxPerson.Cursor = Cursors.Default;
			foreach(DataRow dr in drs)
				comboBoxPerson.Items.Add(new ListItem((int)dr[Environment.PersonData.IDField],
													  dr[Environment.PersonData.NameField].ToString()));

			comboBoxPerson.Items.Add(new ListItem(0, Environment.StringResources.GetString("FindAll") + "..."));
			comboBoxPerson.DroppedDown = true;
		}

		private void comboPerson_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if(comboBoxPerson.SelectedItem == null)
				return;
			var item = comboBoxPerson.SelectedItem as ListItem;
			if(item != null && item.ID > 0)
				AddPersonToList(item.ID, item.Text);
			else
				SelectPerson();
			textBoxPerson.Select();
		}

		private void comboPerson_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyData != Keys.Enter && e.KeyData != Keys.Up && e.KeyData != Keys.Down && e.KeyData != Keys.PageUp &&
				e.KeyData != Keys.PageDown)
			{
				textBoxPerson.Select();
			}
		}

		private void comboPerson_Leave(object sender, EventArgs e)
		{
			comboBoxPerson.DroppedDown = false;
		}

		private void comboPerson_DropDown(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.Default;
		}

		private void ParsePerson(bool skip)
		{
			if(!textBoxPerson.Lock())
				return;

			string txt = textBoxPerson.Text.Trim();
			PersonID = 0;
			DataRow[] drs = ParseQuery(txt);

			if(drs != null)
			{
				int count = drs.Length;
				if(count == 1 && !skip)
				{
					DataRow dr = drs[0];
					int personID = (int)dr[Environment.PersonsUsedData.IDField];
					string personName = (string)dr[Environment.PersonsUsedData.NameField];
					string personNameRL = (string)dr[Environment.PersonsUsedData.NameRLField];

					PersonID = personID;
					PersonName = personName;

					textBoxPerson.Text = personName;

					string regTxt = Regex.Replace(Replacer.ReplaceRusLat(Regex.Escape(txt)), @"\s+", @".*");
					regTxt = "^(?<text>" + regTxt + ").*$";

					Match m = Regex.Match(personNameRL, regTxt, RegexOptions.IgnoreCase);
					if(m.Success)
					{
						int len = m.Groups["text"].Value.Length;
						textBoxPerson.Select(len, textBoxPerson.Text.Length - len);
					}

					OnFindPerson();
				}
				else if(count < 11 && count > 0)
				{
					ShowUsedPerson(drs);
				}
			}
			textBoxPerson.Unlock();
		}

		private DataRow[] ParseQuery(string txt)
		{
			if(txt.Length > 0 && persons != null)
			{
				txt = Replacer.ReplaceRusLat(txt);
				string[] w = Regex.Split(txt, @"\s+");

				if(w.Length > 0)
				{
					List<DataRow> result = persons.Where(x =>
						x[Environment.PersonsUsedData.NameRLField].ToString().ToLower().StartsWith(w[0].ToLower())).ToList();
					for(int i = 1; i < w.Length && result.Count > 0; i++)
					{
						result = result.Where(x =>
							x[Environment.PersonsUsedData.NameRLField].ToString().ToLower().Contains(" " + w[i].ToLower())).ToList();
					}

					if(result.Count > 0)
						return result.ToArray();
				}
			}

			return null;
		}

		private void SelectPerson()
		{
			string paramStr = Environment.PersonParamStr;
			if(textBoxPerson.Text.Length > 0)
				paramStr += "&search=" + HttpUtility.UrlEncode(textBoxPerson.Text);

			paramStr = paramStr.Replace("+", "%20");

			Web.PersonDialog dialog = new Web.PersonDialog(Environment.PersonSearchString, paramStr);
			dialog.DialogEvent += PersonDialog_DialogEvent;
			dialog.Owner = FindForm();
			dialog.Show();
		}

		private void PersonDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Form findForm = FindForm();
			if(findForm != null)
				findForm.Focus();

			Web.PersonDialog dialog = e.Dialog as Web.PersonDialog;
			if(dialog != null && dialog.DialogResult == DialogResult.OK &&
				(dialog.Persons != null && dialog.Persons.Count > 0))
			{
				var info = dialog.Persons[0] as Web.PersonInfo;
				if(info != null)
					AddPersonToList(info.ID, info.Name);
			}
		}

		private void SelectUsedPerson()
		{
			DataRow[] drs = ParseQuery(textBoxPerson.Text.Trim());

			if(drs != null && drs.Length > 0)
			{
				if(drs.Length == 1)
					ChooseUsedPersonFromList(drs, 0);
				else
				{
					if(drs.Length < 11)
						ShowUsedPerson(drs);
					else
						SelectPerson();
				}
			}
			else
				SelectPerson();
		}


		public virtual bool AddPersonToList(int id, string name)
		{
			textBoxPerson.Clear();
			ParsePerson(false);

			if(Environment.PersonsUsedData.UsePerson(id) && LoadUsedPersons())
			{
				PersonID = id;
				PersonName = name;

				textBoxPerson.Text = name;
				textBoxPerson.SelectAll();

				OnFindPerson();
				return true;
			}

			return false;
		}

		private void textBoxPerson_TextChanged(object sender, EventArgs e)
		{
			OnTextChanged();
		}
	}
}