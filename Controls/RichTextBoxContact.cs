using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Controls
{
	public partial class RichTextBoxContact : RichTextBoxEx
	{
		public bool SearchType { get; set; }
		public bool IsFax { get; private set; }
		public bool IsEmail { get; private set; }

		private Dictionary<string,string> linkText;

        public event EventHandler FindEMail;

		public event EventHandler SearchStart;

		private void OnFindEMail()
		{
			if(FindEMail != null)
				FindEMail(this, EventArgs.Empty);
		}

        void OnSearchStart()
        {
            if (SearchStart != null)
                SearchStart(this, EventArgs.Empty);
        }

		public RichTextBoxContact()
		{
			InitializeComponent();
			PlainTextMode = true;
			currentText = "";
		}

#region override
	
		protected override void OnTextChanged(EventArgs e)
		{
           	if(linkText != null && linkText.Count > 0)
			{
				int p = this.SelectionStart;
				if(p > 0)
					Select(p - 1, 1);
				if(GetSelectionLink() == 1)
					return;
				SelectedRtf = "";
				if(!this.Rtf.Contains("\\v0"))
				{
					currentText = "";
					linkText.Clear();
				}
				if(linkText.Count > 0)// Text.Length > linkText.Count)
					currentText = Text.Substring(Text.LastIndexOf("#Edit") + 5); //Text.Substring(Text.IndexOf(linkText[linkText.Keys[linkText.Count - 1].]) + linkText.Values[linkText.Count - 1].Lenght);
				else
					currentText = "";
			}
			else
				currentText = Text;
			
			IsEmail = IsFax = false;
			if(currentText.Trim().Length > 0 && SearchType)
			{
				if(Regex.IsMatch(currentText.Trim(), "([A-Za-z0-9]+([-+._][A-Za-z0-9]+)*@[A-Za-z0-9]+([-._][A-Za-z0-9]+)*.[A-Za-z]{2,})"))
					IsEmail = true;
				else
				{
					if(Regex.IsMatch(currentText.Trim().Replace(" ", ""), "^(\\+\\d|\\d).*\\d.*\\d"))
						IsFax = true;
				}
			}
			

			base.OnTextChanged(e);
		}

		public string CurrentText
		{
			get { return currentText; }
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if(!this.SelectionProtected)
				if(e.KeyCode == Keys.Enter)
				{
					List<int> pid = SearchPerson();
					if(pid != null && pid.Count > 0)
						OnSearchStart();
					else
						if(IsEmail)
							OnFindEMail();
					e.Handled = true;
				}
			if(e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
			{
				if(this.SelectionProtected)
				{
					this.SelectionProtected = false;
					//e.Handled = true;
				}
				else if(SelectedText.Length == 0)
				{
					if(linkText != null)
					{
						int p = this.SelectionStart;
						if(p > 0)
							Select(p - 1, 1);
						if(GetSelectionLink() == 1)
						{
							e.Handled = true;
							this.SelectionProtected = false;
						}
						Select(p, 0);
					}
				}
			}
			base.OnKeyDown(e);
		}

		protected override void Select(bool directed, bool forward)
		{
			base.Select(directed, forward);
		}


#endregion

		public List<int> SearchPerson()
		{
			if(string.IsNullOrEmpty(currentText))
			{
				OnSearchStart();
			}
			else
			{
					//if(IsFax)
					//    testText = SearchText.TrimStart("+".ToCharArray());
					//var ds = Environment.PersonData.FindPersons(currentText, ((SearchType) ? 5 : 1), !SearchType);
					//return ds.Rows.Cast<DataRow>().Select<DataRow, int>(x => (int)x[Environment.PersonData.IDField]).ToList();
			}
			return null;
		}

		string contactText;
		string currentText;

		public override void InsertLink(string text, string hyperlink, bool changeColor = false)
		{
			if(linkText == null)
				linkText = new Dictionary<string,string>();
			
			//if(linkText.Count == 0)
			//    linkText.Add(text, hyperlink);
			//else
			//    if(!linkText.ContainsValue(hyperlink))
					//linkText.Add(text, hyperlink);
			int pos = 0;
			if(SelectionStart > 0)
				if(SelectionStart < Text.Length)
					pos = Text.LastIndexOf(' ', 0, SelectionStart) + 1;
				else
					pos = Text.Length;
			
			//base.InsertLink(text, hyperlink, pos);
		}
	}
}
