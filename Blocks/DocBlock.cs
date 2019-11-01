using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Kesco.Lib.Win.Data.Business.V2.Docs.DomainObjects;
using Kesco.Lib.Win.Document.Search;
using Kesco.Lib.Win.Document.Select;
using Kesco.Lib.Win.Error;

namespace Kesco.Lib.Win.Document.Blocks
{
    public class DocBlock : UserControl
    {
        private SynchronizedCollection<Keys> keyLocker = new SynchronizedCollection<Keys>();
        private string lastText = "";
        private const int maxResult = 21;

        public event BlockEventHandler Selected;
        private Button buttonSelect;
        private TextBox text;

        private Container components;

        public DocBlock()
        {
            UsedDocsIDs = new SynchronizedCollection<int>();
            InitializeComponent();
        }

        #region Accessors

        public override string Text
        {
            get { return text.Text; }
            set { text.Text = value; }
        }

        public int ID { get; private set; }
        public int[] PersonIDs { get; set; }
        public SynchronizedCollection<int> UsedDocsIDs { get; private set; }

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
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (DocBlock));
            this.text = new System.Windows.Forms.TextBox();
            this.buttonSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // text
            // 
            resources.ApplyResources(this.text, "text");
            this.text.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
            // DocBlock
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.text);
            this.Controls.Add(this.buttonSelect);
            this.DoubleBuffered = true;
            this.Name = "DocBlock";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private void text_KeyUp(object sender, KeyEventArgs e)
        {
            if (!keyLocker.Contains(e.KeyData))
            {
                keyLocker.Add(e.KeyData);
                try
                {
                    if (e.KeyData == Keys.Enter)
                    {
                        Parse();
                        if (ID > 0)
                            text.SelectAll();
                    }
                    else if (text.Text != lastText)
                        ID = 0;

                    lastText = text.Text;
                }
                catch (Exception ex)
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
            Parse();
        }

        private string FormPersonIDLine()
        {
            return string.Join(",", PersonIDs.Select(p => p.ToString()).ToArray());
        }

        private string FormPersonLine()
        {
            string line = FormPersonIDLine();
            if (line.Length > 0)
                line = "persons=" + line;

            return line;
        }

        public XmlDocument FormXml(bool usePerson, bool force)
        {
            var xml = new XmlDocument();

            XmlElement root = xml.CreateElement("Options");
            xml.AppendChild(root);

            XmlElement child;

            string txt = text.Text.Trim();

            // документ
            if (txt.Length > 0)
            {
                child = xml.CreateElement("Option");
                child.SetAttribute("name", "СтрокаПоиска");
                child.SetAttribute("value", txt);
                if (xml.DocumentElement != null) 
                    xml.DocumentElement.AppendChild(child);
            }

            // лица
            if (usePerson)
            {
                string line = FormPersonIDLine();
                if (line.Length > 0)
                {
                    child = xml.CreateElement("Option");
                    child.SetAttribute("name", "ЛицаКонтрагенты");
                    child.SetAttribute("value", FormPersonIDLine());
                    if (force)
                        child.SetAttribute("mode", "and");
                    if (xml.DocumentElement != null) 
                        xml.DocumentElement.AppendChild(child);
                }
            }

            return xml;
        }

		private void Parse()
		{
			string txt = text.Text.Trim();

			try
			{
				ID = 0;

				if(PersonIDs == null || PersonIDs.Length == 0)
				{
					SelectDialog(FormXml(false, false).OuterXml);
				}
				else
				{
					if(txt.Length == 0)
					{
						SelectDialog(FormXml(true, true).OuterXml);
					}
					else
					{
						var findForm = FindForm();
						if(findForm != null)
							findForm.Cursor = Cursors.WaitCursor;

						var filter = new DsoDoc();
						filter.SearchText.Text = txt;
						filter.SearchText.Enabled = true;
						if(PersonIDs != null)
						{
							for(int i = 0; i < PersonIDs.Length; i++)
								filter.Person.Add(PersonIDs[i]);
							filter.Person.MatchAnyItem = true;
							filter.Person.Enabled = true;
						}
						int result = 0;
						int maxRecords = 0; //поскольку не надо ничего возвращать
						var ids = new int[maxRecords];

						Data.Env.Docs.FindДокументы(filter, ids, ref result, ref maxRecords);

						if(result > 0)
						{
							if(result < maxResult)
							{
								string xml = FormXml(true, true).OuterXml;
								SelectUniversal(Data.DALC.Documents.Search.Options.GetSQL(xml), xml);
							}
							else
								SelectDialog(FormXml(true, true).OuterXml);
						}
						else
						{
							if(PersonIDs.Length > 1)
							{
								filter.Person.MatchAnyItem = false;
								Data.Env.Docs.FindДокументы(filter, ids, ref result, ref maxRecords);
								if(result > 0)
								{
									if(result < maxResult)
									{
										string xml = FormXml(true, false).OuterXml;
										SelectUniversal(Data.DALC.Documents.Search.Options.GetSQL(xml), xml);
									}
									else
										SelectDialog(FormXml(true, false).OuterXml);
								}
								else
									SelectDialog(FormXml(false, false).OuterXml);
							}
							SelectDialog(FormXml(false, false).OuterXml);
						}
						if(findForm != null)
							findForm.Cursor = Cursors.Default;
					}
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				ErrorShower.OnShowError(null, ex.Message, "");
			}
		}
        
        private void ThrowEvent(int id, string name)
        {
            ID = id;

            if (Selected != null)
                Selected(this, new BlockEventArgs(id, name));
        }

        private void ThrowEvent(int id)
        {
            ThrowEvent(id, DBDocString.Format(id));
        }

        public void SelectDialog(string searchString)
        {
            var dialog = new OptionsDialog(searchString);
            dialog.DialogEvent += SelectDialog_DialogEvent;
            dialog.Owner = FindForm();
            dialog.Show(); // вызова из основной формы не производится!
            var findForm = FindForm();
            if (findForm != null)
                findForm.Enabled = false;
        }

        private void SelectDialog_DialogEvent(object source, DialogEventArgs e)
        {
            Form form = FindForm();
            if (form != null)
            {
                form.Enabled = true;
                form.Focus();
            }

            var search = e.Dialog as OptionsDialog;
            if (search != null && search.DialogResult == DialogResult.OK)
            {
                string sql = Data.DALC.Documents.Search.Options.GetSQL(search.GetXML());
                int count = Environment.DocData.GetDocCount(
                    "(" + sql + ") Kesco_Document_Blocks_DocBlock_SelectDialog_DialogEvent");

                if (count > 0)
                {
                    SelectUniversal(sql, search.GetXML());
                    return;
                }
                else
                {
                    if (
                        MessageBox.Show(
                            Environment.StringResources.GetString("DocumentsNotFound") + System.Environment.NewLine +
                            System.Environment.NewLine +
                            Environment.StringResources.GetString("DocBlock_SelectDialog_DialogEvent_Message1"),
                            Environment.StringResources.GetString("NothingFound"), MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        var searchDialog = new OptionsDialog(search.GetXML());

                        searchDialog.DialogEvent += SelectDialog_DialogEvent;
                        searchDialog.Show();
                    }
                }
            }
        }

        private void SelectUniversal(string sql, string xml)
        {
            var dialog =
                new SelectDocUniversalDialog(Environment.DocData.GetFoundDocsIDQuery(sql, true), 0, xml, true);
            dialog.DialogEvent += SelectDocUniversalDialog_DialogEvent;
            dialog.Owner = FindForm();
            dialog.Show();
        }

        private void SelectDocUniversalDialog_DialogEvent(object source, DialogEventArgs e)
        {
            var dialog = e.Dialog as SelectDocUniversalDialog;
            if (dialog != null && dialog.DialogResult == DialogResult.OK)
            {
                if (dialog.DocIDs != null && dialog.DocIDs[0] != -1)
                {
                    foreach (int id in dialog.DocIDs)
                        ThrowEvent(id);
                }
                else if (dialog.DocID > 0)
                {
                    ThrowEvent(dialog.DocID);
                }
            }
            else
            {
                if (dialog != null && dialog.DialogResult == DialogResult.Retry)
                {
                    SelectDialog(dialog.XML);
                }
            }
        }
    }
}