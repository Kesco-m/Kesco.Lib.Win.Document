using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Components;

namespace Kesco.Lib.Win.Document.Controls
{
    public partial class NewWindowDocumentButton : Button
    {
        private SynchronizedCollection<KeyValuePair<int, string>> docIDs =
            new SynchronizedCollection<KeyValuePair<int, string>>();

        private SynchronizedCollection<KeyValuePair<string, string>> fileNames =
            new SynchronizedCollection<KeyValuePair<string, string>>();

        private ToolTip tooltip = new ToolTip();

        public NewWindowDocumentButton()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public NewWindowDocumentButton(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        public void Set(int docID)
        {
            if (docID > 0)
            {
                docIDs.Add(new KeyValuePair<int, string>(docID, DBDocString.Format(docID)));
                Verify();
            }
        }

        public void Set(int docID, DataRow row)
        {
            if (docID > 0)
            {
                docIDs.Add(new KeyValuePair<int, string>(docID, DBDocString.Format(row)));
                Verify();
            }
        }

        public void Set(string fileName, string docString)
        {
            if (!string.IsNullOrEmpty(fileName))
                fileNames.Add(new KeyValuePair<string, string>(fileName, docString));
            Verify();
        }

        public void LoopSet(int docID, string docString)
        {
            if (docID > 0)
                docIDs.Add(new KeyValuePair<int, string>(docID, docString));
        }

        public void LoopSet(string fileName, string docString)
        {
            fileNames.Add(new KeyValuePair<string, string>(fileName, docString));
        }

        public void Set(int[] docID)
        {
            foreach (int t in docID.Where(t => t > 0))
                docIDs.Add(new KeyValuePair<int, string>(t, DBDocString.Format(t)));
            Verify();
        }

        public void UnSet()
        {
            docIDs.Clear();
            tooltip.RemoveAll();
            Enabled = false;
        }

        public void Verify()
        {
            if (docIDs.Count > 0 || fileNames.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append(Environment.StringResources.GetString("NewWindowDefaultToolTip"));
                foreach (KeyValuePair<int, string> t in docIDs.Where(t => t.Key > 0))
                {
                    sb.Append("\n");
                    sb.Append(t.Value);
                }
                foreach (KeyValuePair<string, string> t in fileNames)
                {
                    sb.Append("\n");
                    sb.Append(t.Value);
                }
                tooltip.SetToolTip(this, sb.ToString());
                Text = string.Empty;
                Visible = true;
                Enabled = true;
            }
            else
            {
                tooltip.RemoveAll();
                Visible = false;
                Enabled = false;
            }
        }

        private void NewWindowDocumentButton_Click(object sender, EventArgs e)
        {
            OpenDocs();
        }

        public void OpenDocs()
        {
            Enabled = false;

            foreach (KeyValuePair<int, string> t in docIDs)
                Environment.OnNewWindow(null, new DocumentSavedEventArgs(t.Key, -1));

            bool fileNotExist = false;
            var sb = new StringBuilder();
            sb.Append(Environment.StringResources.GetString("NewWindowFileNotFound"));
            foreach (KeyValuePair<string, string> t in fileNames)
                if (File.Exists(t.Key))
                    Environment.OnNewWindow(null,
                                           new DocumentSavedEventArgs(t.Key, t.Value));
                else
                {
                    fileNotExist = true;
                    sb.Append("\n");
                    sb.Append(t.Value);
                }

            if (fileNotExist)
                MessageBox.Show(sb.ToString());

            Enabled = true;
        }

        public void ProcessKey(Keys keyData)
        {
            if (keyData.Equals(Keys.Control | Keys.W))
            {
                OpenDocs();
            }
        }
    }
}