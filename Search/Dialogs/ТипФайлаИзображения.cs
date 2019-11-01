using System;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public partial class ТипФайлаИзображения : Base
    {
		public ТипФайлаИзображения()
        {
            InitializeComponent();
        }

		private void checkBoxPDF_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxPDF.Checked)
            {
                checkBoxTIFF.Checked = true;
            }
        }

        private void checkBoxTIFF_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxTIFF.Checked)
            {
                checkBoxPDF.Checked = true;
            }
        }

        protected override void FillElement()
        {
            int val = 0;
            if (checkBoxTIFF.Checked)
                val += 1;
            if (checkBoxPDF.Checked)
                val += 2;
            elOption.SetAttribute("value", val.ToString());
        }

        protected override void FillForm()
        {
            int val = 0;
            if (!int.TryParse(elOption.GetAttribute("value"), out val))
                val = 0;
            checkBoxPDF.Checked = val > 1;
            checkBoxTIFF.Checked = val%2 == 1;
        }
    }
}