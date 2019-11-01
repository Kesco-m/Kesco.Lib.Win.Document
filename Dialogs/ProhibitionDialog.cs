using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public partial class ProhibitionDialog : Form
    {
        public ProhibitionDialog(bool isDoc, string fileName, string docStr)
        {
            InitializeComponent();

            this.Text = StringResources.Warning;
            label1.Text = string.Format(StringResources.DocControl_CopyExists, (isDoc ? Environment.StringResources.GetString("SaveChangesDialog_Doc1") : Environment.StringResources.GetString("SaveChangesDialog_File1")).ToLower());
            this.Size = new Size(label1.Width + 16, this.Height);
            label1.Location = new Point(8, label1.Location.Y);

            newWindowDocumentButton.Set(fileName, docStr);
            newWindowDocumentButton.Verify();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newWindowDocumentButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
