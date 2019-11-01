using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Blocks.Parsers;
using Kesco.Lib.Win.Document.Select;

namespace Kesco.Lib.Win.Document.Blocks
{
	public class DocTypeBlock : UserControl
	{
		private SynchronizedCollection<Keys> keyLocker;

		private string lastText = "";

		private DocTypeParser docTypeParser;

		public event BlockEventHandler Selected;
		public Button buttonSelect;
		private TextBox text;

		private Container components;

		private EventHandler tEH;

		public DocTypeBlock()
		{
			InitializeComponent();

			keyLocker = new SynchronizedCollection<Keys>();

			tEH = (object sender, EventArgs e) => { if(DocTypeTextChanged != null) DocTypeTextChanged(sender, e); };
			text.TextChanged += tEH;
		}

		#region Accessors

		public override string Text
		{
			get { return text.Text; }
			set
			{
				text.TextChanged -= tEH;

				text.Text = value;
				if(value != string.Empty && docTypeParser != null)
				{
					DataRow[] drs = docTypeParser.ParseQuery(text.Text);
					if(drs != null && drs.Length > 0 && drs.Length == 1)
						SearchType = (byte)drs[0][Environment.DocTypeData.SearchTypeField];
				}

				text.TextChanged += tEH;
			}
		}

		public int ID { get; set; }

		public byte SearchType { get; private set; }

		#endregion

		public event EventHandler DocTypeTextChanged;

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocTypeBlock));
			this.text = new System.Windows.Forms.TextBox();
			this.buttonSelect = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// text
			// 
			resources.ApplyResources(this.text, "text");
			this.text.Name = "text";
			this.text.KeyUp += new System.Windows.Forms.KeyEventHandler(this.text_KeyUp);
			// 
			// buttonSelect
			// 
			resources.ApplyResources(this.buttonSelect, "buttonSelect");
			this.buttonSelect.Name = "buttonSelect";
			this.buttonSelect.TabStop = false;
			this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
			// 
			// DocTypeBlock
			// 
			this.Controls.Add(this.text);
			this.Controls.Add(this.buttonSelect);
			this.Name = "DocTypeBlock";
			resources.ApplyResources(this, "$this");
			this.Load += new System.EventHandler(this.OnLoad);
			this.Leave += new System.EventHandler(this.DocTypeBlock_Leave);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private void OnLoad(object sender, EventArgs e)
		{
			if(DesignerDetector.IsComponentInDesignMode(this))
				return;
			// loading doc types
			if(Environment.DocTypeData != null)
			{
				if(Environment.DocTypes != null)
				{
					docTypeParser = new DocTypeParser(Environment.DocTypeData, Environment.DocTypes);
					if(ID > 0)
					{
						bool parsed = false;
						text.Text = Environment.DocTypeData.GetDocType(ID, Environment.CurCultureInfo.TwoLetterISOLanguageName);
						DataRow[] drs = docTypeParser.ParseQuery(text.Text);
						if(drs != null)
							foreach(DataRow t in drs.Where(t => t[Environment.DocTypeData.IDField].Equals(ID)))
							{
								SearchType = (byte)t[Environment.DocTypeData.SearchTypeField];
								Text = t[Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? Environment.DocTypeData.NameField : Environment.DocTypeData.TypeDocField].ToString();
								parsed = true;
								break;
							}
						if(!parsed)
						{
							ID = 0;
							text.Text = "";
						}
					}
				}
				else
					throw new Exception("DocTypeBlock: " +
										Environment.StringResources.GetString("DocTypeBlock_OnLoad_Error1"));
			}
			else
			{
				if(!DesignerDetector.IsComponentInDesignMode(this))
					throw new Exception("DocTypeBlock: " +
										Environment.StringResources.GetString("DocTypeBlock_OnLoad_Error2") + " (Data).");
			}
		}

		private void text_KeyUp(object sender, KeyEventArgs e)
		{
			if(!keyLocker.Contains(e.KeyData))
			{
				try
				{
					if(e.KeyData == Keys.Enter)
					{
						text.Text = text.Text.Replace(System.Environment.NewLine, "");
						DataRow[] drs = Parse();
						if(drs != null && drs.Length == 1)
							text.SelectAll();
						else
							SelectDialog();
					}
					else if(text.Text != lastText)
					{
						if(e.KeyData == Keys.Delete || e.KeyData == Keys.Back)
							Parse(false);
						else
							ParseAndSelectTail();
					}

					lastText = text.Text;
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
				}
				finally
				{
					keyLocker.Remove(e.KeyData);
				}
			}
		}

		private void buttonSelect_Click(object sender, EventArgs e)
		{
			SelectDialog();
		}

		public void SelectDialog()
		{
			var dialog = new SelectTypeDialog(text.Text.Trim());
			dialog.DialogEvent += SelectTypeDialog_DialogEvent;

			Form owner = FindForm();
			if(owner is FreeDialog)
			{
				var fOwner = owner as FreeDialog;
				fOwner.ShowSubForm(dialog);
			}
			else
				dialog.Show(); // будем считать, что вызова из основной формы не производится

			if(owner != null)
				owner.Enabled = false;
		}

		private void SelectTypeDialog_DialogEvent(object source, DialogEventArgs e)
		{
			if(!keyLocker.Contains(Keys.Return))
			{
				keyLocker.Add(Keys.Return);
				try
				{
					Form form = FindForm();
					if(form != null)
					{
						form.Enabled = true;
						form.Focus();
					}

					var dialog = e.Dialog as SelectTypeDialog;
					if(dialog != null && (dialog.DialogResult == DialogResult.OK && dialog.TypeID > 0))
					{
						ThrowEvent(dialog.TypeID, dialog.Type);
						text.SelectAll();
					}
					else
						ID = 0;
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
				}
				finally
				{
					keyLocker.Remove(Keys.Return);
				}
			}
		}

		private object CultureObj(DataRow dr)
		{
			return Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? dr[Environment.DocTypeData.NameField] : dr[Environment.DocTypeData.TypeDocField];
		}

		private DataRow[] Parse(bool throwEvent = true)
		{
			// throwEvent - выбрасывать event или нет, если 1 совпадение
			// возврат - число совпадений

			string txt = text.Text.Trim();
			DataRow[] drs = docTypeParser.ParseQuery(txt);
			int count = 0;

			ID = 0;

			if((drs != null) && (drs.Length > 0))
			{
				count = drs.Length;
				if(count == 1)
				{
					DataRow dr = drs[0];
					var gotID = (int)dr[Environment.DocTypeData.IDField];
					SearchType = (byte)dr[Environment.DocTypeData.SearchTypeField];
					if(throwEvent)
						ThrowEvent(gotID, (string)CultureObj(dr));
					else
						ID = gotID;
				}
			}

			UpdateButtonSelect(count);

			return drs;
		}

		private void SelectTail(string head, string rusLat)
		{
			string reg = Regex.Replace(Regex.Replace(
				Replacer.ReplaceRusLat(Regex.Escape(head)),
				@"\\\s+",
				@" "), @"\s+", ".*?");

			reg = "^(?<text>" + reg + ").*$";

			Match m = Regex.Match(rusLat, reg, RegexOptions.IgnoreCase);
			if(m.Success)
			{
				int len = m.Groups["text"].Value.Length;
				text.Select(len, text.Text.Length - len);
			}
		}

		private DataRow[] ParseAndSelectTail()
		{
			string curText = text.Text.Trim();
			DataRow[] drs = Parse();
			if(drs != null && drs.Length == 1)
			{
				DataRow dr = drs[0];
				var valRL = (string)dr[Environment.DocTypeData.NameRLField];
				SelectTail(curText, valRL);
			}

			return drs;
		}

		private void UpdateButtonSelect(int count)
		{
			switch(count)
			{
				case 0:
					buttonSelect.Text = Environment.StringResources.GetString("Search") + "...";
					break;
				case 1:
					buttonSelect.Text = Environment.StringResources.GetString("Choose");
					break;
				default:
					buttonSelect.Text = Environment.StringResources.GetString("Search") + "..." + count;
					break;
			}
		}

		private void ThrowEvent(int id, string name)
		{
			ID = id;
			text.Text = name;
			DataRow[] drs = docTypeParser.ParseQuery(text.Text);
			if(drs != null && drs.Length > 0 && id > 0)
			for(int i =0 ; i < drs.Length; i++)
				if(drs[i][Environment.DocTypeData.IDField].Equals(id))
				{
					SearchType = (byte)drs[i][Environment.DocTypeData.SearchTypeField];
					break;
				}
			UpdateButtonSelect(1);

			if(Selected != null)
				Selected(this, new BlockEventArgs(id, name));
		}

		private void DocTypeBlock_Leave(object sender, EventArgs e)
		{
			if(ID == 0 && text.Text.Length > 0)
			{
				DataRow[] drs = ParseAndSelectTail();
				if(ID == 0 && (drs == null || drs.Length != 1))
					SelectDialog();
			}
		}
	}
}