using System;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Select;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public partial class ТипПечатнойФормы : Base
    {
        private string src;

        public ТипПечатнойФормы()
        {
            InitializeComponent();
        }

        private void lv_Refresh()
        {
            lv.Items.Clear();
            if (string.IsNullOrEmpty(src)) 
                return;
            foreach (
                ListViewItem li in
                    from id in src.Split(';') where !string.IsNullOrEmpty(id) select new ListViewItem {Text = id})
                lv.Items.Add(li);
        }

        private void bSelect_Click(object sender, EventArgs e)
        {
            SelectPrintFormDialog();
        }

        private void SelectPrintFormDialog()
        {
            var spfd = new SelectPrintingFormDialog();
            spfd.DialogEvent += spfd_DialogEvent;
            ShowSubForm(spfd);
            Enabled = false;
        }

        private void spfd_DialogEvent(object source, DialogEventArgs e)
        {
            Enabled = true;
            var dialog = (SelectPrintingFormDialog) e.Dialog;
            if (dialog.DialogResult != DialogResult.OK || dialog.PrintTypes == null || dialog.PrintTypes.Length <= 0)
                return;
            foreach (string val in dialog.PrintTypes.Select(t => t.Trim()))
            {
                if (src.Length > 0)
                    src += ";";
                src += val;
            }
            src = src.Replace(";;", ";");
            src = src.TrimStart(";".ToCharArray()).TrimEnd(",".ToCharArray());
            lv_Refresh();
        }

        protected override void FillElement()
        {
            elOption.SetAttribute("value", src);
            elOption.SetAttribute("mode", rbAND.Checked ? "and" : "or");
        }

        protected override void FillForm()
        {
            src = elOption.GetAttribute("value");
            lv_Refresh();
            if (lv.Items.Count == 0)
                SelectPrintFormDialog();
            string mode = elOption.GetAttribute("mode");
            if (!string.IsNullOrEmpty(mode) && mode.ToLower().Equals("and"))
                rbAND.Checked = true;
            else
                rbOR.Checked = true;
        }

        private void bDelete_Click(object sender, EventArgs e)
        {
            for (int i = lv.SelectedItems.Count - 1; i >= 0; i--)
            {
                int len = src.Length;
                string val = lv.SelectedItems[i].Text;
                src = src.Replace(";" + val + ";", ";");
                if (len == src.Length)
                    src = src.Replace(val + ";", "");
                if (len == src.Length)
                    src = src.Replace(";" + val, "");
                if (len == src.Length)
                    src = src.Replace(val, "");
                len = 0;
                while (len != src.Length)
                {
                    len = src.Length;
                    src = src.Replace(";;", ";");
                }

                src = src.TrimStart(";".ToCharArray()).TrimEnd(";".ToCharArray());
                lv.Items.Remove(lv.SelectedItems[i]);
            }
        }

        private void lv_SelectedIndexChanged(object sender, EventArgs e)
        {
            bDelete.Enabled = lv.SelectedItems.Count > 0;
        }
    }
}