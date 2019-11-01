using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document.Blocks;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public partial class MailingListShareDialog : FreeDialog
    {
        private MailingListItem ml;

        public MailingListShareDialog()
        {
            InitializeComponent();

            list.Columns.Add(Environment.StringResources.GetString("Employee"), list.ClientSize.Width - 1,
                             HorizontalAlignment.Left);

            employeeBlock.Parser = new Blocks.Parsers.EmployeeParser(Environment.EmpData, false);
            employeeBlock.Label = Environment.StringResources.GetString("Search");
        }

        public MailingListShareDialog(MailingListItem ml) : this()
        {
            this.ml = ml;
        }

        public MailingListShareDialog(List<Employee> emps) : this()
        {
            LoadEmployees(emps);
        }

        private void MailingListShareDialog_Load(object sender, EventArgs e)
        {
            if (ml != null)
            {
                labelMailingList.Text = string.Format(labelMailingList.Text, ml.Name);
                LoadEmployees(ml.SharedEmploees);
            }

            UpdateControls();
        }

        private void LoadEmployees(List<Employee> emps)
        {
            try
            {
                for (int i = 0; i < emps.Count; i++)
                    list.Items.Add(Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
                                       ? new ListViewItem(emps[i].LongName) {Tag = emps[i]}
                                       : new ListViewItem(emps[i].LongEngName) {Tag = emps[i]});

                if (list.Items.Count > 0)
                    list.Items[0].Selected = true;
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
                MessageForm.Show(ex.Message);
            }
        }

        private void UpdateControls()
        {
            buttonDeleteEmployees.Enabled = (list.SelectedItems.Count > 0);
        }

        private void employeeBlock_EmployeeSelected(object source, EmployeeBlockEventArgs e)
        {
            if (e.Emps == null)
                return;
            for (int i = 0; i < e.Emps.Length; i++)
                AddEmployee(e.Emps[i]);
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void AddEmployee(Employee emp)
        {
            bool found = false;
            for (int i = 0; i < list.Items.Count; i++)
            {
                var old = list.Items[i].Tag as Employee;
                if (old != null)
                    found = (old.ID == emp.ID);
                if (found)
                    break;
            }

            if (!found)
                list.Items.Add(new ListViewItem((Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
                                                     ? emp.LongName
                                                     : emp.LongEngName)) {Tag = emp});
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            int count = list.Items.Count;

            var emps = new List<Employee>(count);
            for (int i = 0; i < count; i++)
                emps.Add(list.Items[i].Tag as Employee);

            try
            {
                ml.SharedEmploees = emps;
                Environment.MailingListData.SaveMailingListSharing(ml);

                End(DialogResult.OK);
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
                MessageForm.Show(ex.Message);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void buttonDeleteEmployees_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Environment.StringResources.GetString("DeleteConfirmationEmployees"),
                                Environment.StringResources.GetString("DeleteConfirmation"), MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                != DialogResult.Yes)
                return;

            if (list.SelectedItems.Count > 0)
            {
                list.DeleteSelectedItem();
                UpdateControls();
            }
        }
    }
}