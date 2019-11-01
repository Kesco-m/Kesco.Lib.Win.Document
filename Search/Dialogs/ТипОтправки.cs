using System;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public partial class ТипОтправки : Base
    {
        public ТипОтправки()
        {
            InitializeComponent();
        }

        private void checkBoxFax_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxFax.Checked)
            {
                checkBoxEmail.Checked = true;
            }
        }

        private void checkBoxEmail_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxEmail.Checked)
            {
                checkBoxFax.Checked = true;
            }
        }

        protected override void FillElement()
        {
            int val = 0;
            if (checkBoxEmail.Checked)
                val += 2;
            if (checkBoxFax.Checked)
                val += 1;
            elOption.SetAttribute("value", val.ToString());
        }

        protected override void FillForm()
        {
            int val = 3;
            if (!int.TryParse(elOption.GetAttribute("value"), out val))
                val = 3;
            checkBoxEmail.Checked = val > 1;
            checkBoxFax.Checked = val%2 == 1;
        }
    }
}