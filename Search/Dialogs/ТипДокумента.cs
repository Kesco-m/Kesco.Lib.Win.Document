using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Select;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class ТипДокумента : Base
    {
        private string src;
        private Button bSelect;
        private ListView lv;
        private ColumnHeader columnHeader1;
        private Button bDelete;

        private Container components;

        public ТипДокумента()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Clean up any resources being used.
        /// </summary>
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

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (ТипДокумента));
            this.bSelect = new System.Windows.Forms.Button();
            this.lv = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.bDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bSelect
            // 
            resources.ApplyResources(this.bSelect, "bSelect");
            this.bSelect.Name = "bSelect";
            this.bSelect.Click += new System.EventHandler(this.bSelect_Click);
            // 
            // lv
            // 
            resources.ApplyResources(this.lv, "lv");
            this.lv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                                         {
                                             this.columnHeader1
                                         });
            this.lv.FullRowSelect = true;
            this.lv.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lv.Name = "lv";
            this.lv.UseCompatibleStateImageBehavior = false;
            this.lv.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // bDelete
            // 
            resources.ApplyResources(this.bDelete, "bDelete");
            this.bDelete.Name = "bDelete";
            this.bDelete.Click += new System.EventHandler(this.bDelete_Click);
            // 
            // ТипДокумента
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.bDelete);
            this.Controls.Add(this.lv);
            this.Controls.Add(this.bSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "ТипДокумента";
            this.Controls.SetChildIndex(this.bSelect, 0);
            this.Controls.SetChildIndex(this.lv, 0);
            this.Controls.SetChildIndex(this.bDelete, 0);
            this.ResumeLayout(false);
        }

        #endregion

        private void bSelect_Click(object sender, EventArgs e)
        {
            SelectTypeDialog();
        }

        private void SelectTypeDialog()
        {
            var dialog = new SelectTypeDialog(0, false, true) {ShowInTaskbar = false};
            dialog.DialogEvent += SelectTypeDialog_DialogEvent;
            dialog.Activated += dialog_Load;
            Hide();
            ShowSubForm(dialog);
            Enabled = false;
        }

        private void dialog_Load(object sender, EventArgs e)
        {
            var dialog = sender as Form;
            if (dialog != null)
                dialog.Activated -= dialog_Load;
            Hide();
        }

        private void SelectTypeDialog_DialogEvent(object source, DialogEventArgs e)
        {
            Show();
            Enabled = true;
            var dialog = (SelectTypeDialog) e.Dialog;
            if (dialog.DialogResult != DialogResult.OK || dialog.TypeID <= 0)
                return;

            string id = dialog.TypeID.ToString() +
                        (dialog.SimilarChecked ? "S" : "") +
                        (dialog.SubTypesChecked ? "C" : "");
            id = Data.DALC.Documents.Search.Document.ТипДокумента.NormalizeS(id);

            foreach (string x in Regex.Replace(id, "C", "", RegexOptions.IgnoreCase).Split(','))
                src = Regex.Replace(src, x + "[C]{0,1}", "");
            src = Regex.Replace(src, "[,]+", ",");
            src = Regex.Replace(src, "^,|,$", "");
            src += (src.Length > 0 ? "," : "") + id;
            lv_Refresh();
        }

        private void bDelete_Click(object sender, EventArgs e)
        {
            for (int i = lv.Items.Count - 1; i >= 0; i--)
                if (lv.Items[i].Selected)
                {
                    src = Regex.Replace(src, lv.Items[i].Tag + "[C]{0,1}", "");
                    src = Regex.Replace(src, "[,]+", ",");
                    src = Regex.Replace(src, "^,|,$", "");
                    lv.Items.Remove(lv.Items[i]);
                }
        }

        private void lv_Refresh()
        {
            Match m;
            lv.Items.Clear();
            if (src == null)
                return;
            foreach (string id in src.Split(','))
            {
                if (!(m = Regex.Match(id, "^(\\d{1,9})([C]{0,1})$", RegexOptions.IgnoreCase)).Success)
                    continue;
                lv.Items.Add(new ListViewItem
                         {
                             Text = Data.DALC.Documents.Search.Document.ТипДокумента.GetText(m.Groups[1].Value) +
                                    ((m.Groups[2].Value.Length > 0)
                                         ? " " + Environment.StringResources.GetString("WithSpecifications")
                                         : ""),
                             Tag = m.Groups[1].Value
                         });
            }
        }

        protected override void FillElement()
        {
            elOption.SetAttribute("value", src);
        }

        protected override void FillForm()
        {
            src = elOption.GetAttribute("value");
            lv_Refresh();
            if (lv.Items.Count == 0)
            {
                Load += ТипДокумента_Load;
                SelectTypeDialog();
            }
        }

        private void ТипДокумента_Load(object sender, EventArgs e)
        {
            Hide();
        }
    }
}