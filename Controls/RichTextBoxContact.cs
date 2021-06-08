using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Controls
{
	public partial class RichTextBoxContact : RichTextBoxEx
	{
		public bool SearchType { get; set; }
		public bool IsFax { get; private set; }
		public bool IsEmail { get; private set; }

		private StringDictionary linkText;

        public event EventHandler FindEMail;

		public event EventHandler SearchStart;

		public event EventHandler<RefreshEventArgs> Delete;

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
			else
				if(Pids != null)
					Pids.Clear();

			base.OnTextChanged(e);
		}

		public string CurrentText
		{
			get { return currentText; }
		}

		public List<int> Pids { get; private set; }

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if(!this.SelectionProtected)
				if(e.KeyCode == Keys.Enter)
				{
					Pids = SearchPerson();
					if(Pids != null &&  Pids.Count > 0)
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
				string testText = currentText;
					if(IsFax)
					    testText = currentText.TrimStart("+".ToCharArray());
					var ds = Environment.PersonData.FindPersons(testText, ((SearchType) ? 5 : 1), !SearchType);
					return ds.Rows.Cast<DataRow>().Select<DataRow, int>(x => (int)x[Environment.PersonData.IDField]).ToList();
			}
			return null;
		}

		string contactText;
		string currentText;

		public void InsertLink(string text, string hyperlink, object tag, bool changeColor = false)
		{
			if(linkText == null)
				linkText = new StringDictionary();

			//if(linkText.Count == 0)
			//	linkText.Add(text, hyperlink);
			//else
			//	if(!linkText.ContainsValue(hyperlink))
			//		linkText.Add(text, hyperlink);
			int pos = 0;
			LinkLabel lab = new LinkLabel();
			lab.Text = text;
			lab.Tag = tag;
			lab.Links.Add(0, text.Length, hyperlink);
			lab.LinkClicked += new LinkLabelLinkClickedEventHandler(Lab_LinkClicked);
			
			lab.Image = (System.Drawing.Image)(global::Kesco.Lib.Win.Document.Properties.Resources.ResourceManager.GetObject("ImageRemove"));
			if(lab.Image != null)
			{
				lab.ImageAlign = ContentAlignment.TopRight;
				lab.Click += new EventHandler(Lab_Click);
				int wid = lab.PreferredWidth;
				lab.AutoSize = false;
				lab.Width = wid + lab.Image.Width + 2;
				lab.Cursor = Cursors.Default;
			}
			//Text += lab.Text + ",   ";
			
			this.Controls.Add(lab);

			//if(SelectionStart > 0)
			//	if(SelectionStart < Text.Length)
			//		pos = Text.LastIndexOf(' ', 0, SelectionStart) + 1;
			//	else
			//		pos = Text.Length;

			base.InsertLink(text, hyperlink, pos);
			
		}

		private void Lab_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.OnLinkClicked(new LinkClickedEventArgs(e.Link.LinkData.ToString()));
		}

		private void Lab_Click(object sender, EventArgs e)
		{
			LinkLabel li = sender as LinkLabel;
			MouseEventArgs ar = e as MouseEventArgs;
			if(ar != null && li != null)
			{
				if(ar.Location.X > li.Width - li.Image.Width - 8)
				{
					this.OnDelete(li.Tag);
					this.Controls.Remove(li);
					this.Text= this.Text.Replace(li.Text +",  ", "");
				}
			}
		}

		private void OnDelete(object tag)
		{
			if(Delete != null)
				Delete(this, new RefreshEventArgs (tag)); 
		}
	}
}
