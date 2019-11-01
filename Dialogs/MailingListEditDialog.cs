using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public class MailingListEditDialog : FreeDialog
    {
        private MailingListItem ml;

        private ListViews.DelableListView list;
        private Button buttonDelete;
        private Label labelMailingListName;
        private TextBox textMailingListName;
        private Label labelEmployeeList;
        private Button buttonSave;
        private Button buttonCancel;
        private Blocks.EmployeeBlock employeeBlock;
        private Label labelAddEmployees;

        private Container components;

        public MailingListEditDialog(MailingListItem ml) : this()
        {
            this.ml = ml;
        }

        public MailingListEditDialog(List<Employee> emps)  : this()
        {
            LoadEmployees(emps);
        }

        public MailingListEditDialog()
        {
            InitializeComponent();

            list.Columns.Add(Environment.StringResources.GetString("Employee"), list.Width - 20,
                             HorizontalAlignment.Left);

            employeeBlock.Parser = new Blocks.Parsers.EmployeeParser(Environment.EmpData, false);
            employeeBlock.Label = Environment.StringResources.GetString("Search");
        }

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Kesco.Lib.Win.Document.Dialogs.MailingListEditDialog));
            this.list = new Kesco.Lib.Win.Document.ListViews.DelableListView();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.labelMailingListName = new System.Windows.Forms.Label();
            this.textMailingListName = new System.Windows.Forms.TextBox();
            this.labelEmployeeList = new System.Windows.Forms.Label();
            this.employeeBlock = new Kesco.Lib.Win.Document.Blocks.EmployeeBlock();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelAddEmployees = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // list
            // 
            resources.ApplyResources(this.list, "list");
            this.list.FullRowSelect = true;
            this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.list.HideSelection = false;
            this.list.MultiSelect = false;
            this.list.Name = "list";
            this.list.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.Details;
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // labelMailingListName
            // 
            resources.ApplyResources(this.labelMailingListName, "labelMailingListName");
            this.labelMailingListName.Name = "labelMailingListName";
            // 
            // textMailingListName
            // 
            resources.ApplyResources(this.textMailingListName, "textMailingListName");
            this.textMailingListName.Name = "textMailingListName";
            // 
            // labelEmployeeList
            // 
            resources.ApplyResources(this.labelEmployeeList, "labelEmployeeList");
            this.labelEmployeeList.Name = "labelEmployeeList";
            // 
            // employeeBlock
            // 
            this.employeeBlock.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.employeeBlock, "employeeBlock");
            this.employeeBlock.Name = "employeeBlock";
            this.employeeBlock.EmployeeSelected += new Kesco.Lib.Win.Document.Blocks.EmployeeBlockEventHandler(this.employeeBlock_EmployeeSelected);
            // 
            // buttonSave
            // 
            resources.ApplyResources(this.buttonSave, "buttonSave");
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelAddEmployees
            // 
            resources.ApplyResources(this.labelAddEmployees, "labelAddEmployees");
            this.labelAddEmployees.Name = "labelAddEmployees";
            // 
            // MailingListEditDialog
            // 
            this.AcceptButton = this.buttonSave;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.employeeBlock);
            this.Controls.Add(this.textMailingListName);
            this.Controls.Add(this.labelMailingListName);
            this.Controls.Add(this.labelEmployeeList);
            this.Controls.Add(this.list);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.labelAddEmployees);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MailingListEditDialog";
            this.Load += new System.EventHandler(this.MailingListEditDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (textMailingListName.Text.Length == 0)
            {
                MessageForm.Show(
                    Environment.StringResources.GetString("MailingListEditDialog_buttonSave_Click_Error1"),
                    Environment.StringResources.GetString("InputError"));
                textMailingListName.Focus();
                return;
            }

            int count = list.Items.Count;
            if (count == 0)
            {
                MessageForm.Show(
                    Environment.StringResources.GetString("MailingListEditDialog_buttonSave_Click_Error2"),
                    Environment.StringResources.GetString("InputError"));
                employeeBlock.Focus();
                return;
            }

            var emps = new List<Employee>(count);
            for (int i = 0; i < count; i++)
                emps.Add(list.Items[i].Tag as Employee);

            try
            {
                if (ml == null)
                    Environment.MailingListData.SaveMailingList(textMailingListName.Text, emps);
                else
                {
                    ml.Name = textMailingListName.Text;
                    ml.Employees = emps;

                    Environment.MailingListData.SaveMailingList(ml);
                }

                End(DialogResult.OK);
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
                MessageForm.Show(ex.Message);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
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

        private void MailingListEditDialog_Load(object sender, EventArgs e)
        {
            if (ml != null)
            {
                textMailingListName.Text = ml.Name;
                LoadEmployees(ml.Employees);
            }

            UpdateControls();
        }

        private void LoadEmployees(List<Employee> emps)
        {
            try
            {
                foreach (Employee t in emps)
                    list.Items.Add(Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
                                       ? new ListViewItem(t.LongName) {Tag = t}
                                       : new ListViewItem(t.LongEngName) {Tag = t});
                list.Sort();
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
            if (InvokeRequired)
                BeginInvoke((MethodInvoker)(UpdateControls));
            else
                buttonDelete.Enabled = (list.SelectedItems.Count > 0);
        }

        private void employeeBlock_EmployeeSelected(object source, Blocks.EmployeeBlockEventArgs e)
        {
            if (e.Emps == null)
                return;
            for (int i = 0; i < e.Emps.Length; i++)
                AddEmployee(e.Emps[i]);
        }

        private void AddEmployee(Employee emp)
        {
            for (int i = 0; i < list.Items.Count; i++)
            {
                var old = list.Items[i].Tag as Employee;
                if (old != null && old.ID == emp.ID)
                    return;
            }

            list.Items.Add(new ListViewItem((Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
                                                 ? emp.LongName
                                                 : emp.LongEngName)) {Tag = emp});
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }
    }
}