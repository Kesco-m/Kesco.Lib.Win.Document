using System;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public partial class NumberConfirmDialog : FreeDialog
    {
        public NumberConfirmDialog(string numberString)
        {
            InitializeComponent();
            labelText.Text = String.Format(labelText.Text, numberString);
            buttonWithNumber.Text = numberString.Length > 8
                                        ? String.Format(buttonWithNumber.Text,
                                                        numberString.Substring(0, 5).Trim() + "...")
                                        : String.Format(buttonWithNumber.Text, numberString);
        }

        private void buttonNoNumber_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void buttonWithNumber_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}