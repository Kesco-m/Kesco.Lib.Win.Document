using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Controls
{
	public partial class ContactControl : AsyncUserControl
	{
		public ContactControl()
		{
			InitializeComponent();
			richTextBox1.LinkClicked += new LinkClickedEventHandler(richTextBox1_LinkClicked);
			richTextBox1.SearchStart += new EventHandler(richTextBox1_SearchStart);
			richTextBox1.FindEMail += new EventHandler(richTextBox_FindEMail);
		}

		void richTextBox1_SearchStart(object sender, EventArgs e)
		{
			OnSearchStart();
		}

		void richTextBox_FindEMail(object sender, EventArgs e)
		{
			OnFindEMail();
		}

		public event EventHandler SearchStart;

		private void OnSearchStart()
		{
			if(SearchStart != null)
				SearchStart(this, EventArgs.Empty);
		}

		public event EventHandler FindEMail;

		private void OnFindEMail()
		{
			if(FindEMail != null)
				FindEMail(this, EventArgs.Empty);
		}

		void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			OnLinkClicked(e);
		}

		public event EventHandler EditPush;

		private void OnEditPush()
		{
			if(EditPush != null)
				EditPush(this, EventArgs.Empty);
		}

		public new string Text
		{
			get { return richTextBox1.Text; }
			set
			{
				richTextBox1.Text = value;
			}
		}

		public string SearchText
		{
			get { return richTextBox1.CurrentText; }
		}

		public event LinkClickedEventHandler LinkClicked;

		void OnLinkClicked(LinkClickedEventArgs e)
		{
			if(LinkClicked != null)
				LinkClicked(this, e);
		}

		internal void InsertLink(string text, string hyperlink)
		{
			richTextBox1.InsertLink(text, hyperlink, true);
		}

		internal void AddContact(DataRow dr, KeyValuePair<int, string> keyValuePair)
		{
			if(!string.IsNullOrEmpty(richTextBox1.CurrentText))
			{
				int str_pos = richTextBox1.Text.IndexOf(string.IsNullOrEmpty(richTextBox1.CurrentText) ? "" : richTextBox1.CurrentText);
				richTextBox1.Text = richTextBox1.Text.Remove(str_pos, richTextBox1.CurrentText.Length);
			}
			richTextBox1.InsertLink(keyValuePair.Value, "http://www.com/" + keyValuePair.Key.ToString() + "#Edit");
		}

		void richTextBox1_MouseHover(object sender, System.EventArgs e)
		{

			if(richTextBox1.Text.Length < 1)
			{
				toolTip.SetToolTip(richTextBox1, null);
				return;
			}

			Point p, p2;
			p = Cursor.Position;
			p2 = richTextBox1.PointToClient(p);

			int i = richTextBox1.GetCharIndexFromPosition(p2);
			int str_pos = richTextBox1.Text.IndexOf(" ", i);
			string str = richTextBox1.Text.Substring(0, str_pos > 0 ? str_pos : richTextBox1.Text.Length - 1);
			str_pos = richTextBox1.Text.LastIndexOf(" ", i);
			string str2 = str.Substring(str_pos < 0 ? 0 : str_pos).Trim();

			toolTip.SetToolTip(richTextBox1, str2);
		}

		void richTextBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			toolTip.SetToolTip(richTextBox1, null);
		}

		internal void AddContact(DataRow dr, KeyValuePair<int, string> main, KeyValuePair<int, string> second)
		{
			int str_pos = richTextBox1.Text.IndexOf(richTextBox1.CurrentText);
			richTextBox1.Text = richTextBox1.Text.Remove(str_pos, richTextBox1.CurrentText.Length);
			richTextBox1.InsertLink(main.Value, "http://www.com/" + main.Key.ToString() + "#Edit");
		}

		internal void AddContact(Tuple<int, string, string> tuple)
		{
			int str_pos = richTextBox1.Text.IndexOf(richTextBox1.CurrentText);
			int len = richTextBox1.CurrentText.Length;
			richTextBox1.Text = richTextBox1.Text.Remove(str_pos, len);
			richTextBox1.InsertLink(tuple.Item2, "http://www.com/" + tuple.Item3 + "#Edit");
		}

		public bool IsEmail { get { return richTextBox1.IsEmail; } }
	}
}
