using System;
using System.IO;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.PropertiesDialogs
{
    /// <summary>
    /// Диалог свойств файла изображения
    /// </summary>
    public partial class PropertiesPDFDialog : Form
    {
        private string fullFileName;
        private string password;
        private int page;

        /// <summary>
        ///   Диалог свойств файла изображения
        /// </summary>
        /// <param name="fullFileName"> полный путь до файла </param>
        /// <param name="page"> Номер открытой страницы </param>
        public PropertiesPDFDialog(string fullFileName, string password, int page)
        {
            InitializeComponent();

            this.fullFileName = fullFileName;
            if (Environment.TmpFilesContains(fullFileName))
                this.fullFileName = Environment.GetTmpFileKey(fullFileName);
            this.page = page;
            this.password = password;
        }

        private void PropertiesImageDialog_Load(object sender, EventArgs e)
        {
            if (File.Exists(fullFileName))
            {
                var fi = new FileInfo(fullFileName);

                labelName.Text = fi.Name;
                labelSize.Text = fi.Length.ToString();
                Environment.PDFHelper.Open(fullFileName, password);
                labelFormat.Text = "PDF ";
                labelPageCount.Text = Environment.PDFHelper.PageCount.ToString();
                Environment.PDFHelper.Page = page;
                labelPageSize.Text = (Environment.PDFHelper.Height*1.27/36).ToString("N2") + "cm x " +
                                     (Environment.PDFHelper.Width*1.27/36).ToString("N2") + "cm";
            }
            else
                Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}