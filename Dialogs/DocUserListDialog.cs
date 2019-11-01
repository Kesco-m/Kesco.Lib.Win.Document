using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Items;
using Kesco.Lib.Win.Error;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public class DocUserListDialog : Form
    {
        private readonly int docID;

        private Label label1;
        private ListView empList;
        private Button buttonOK;

        private Container components;

        public DocUserListDialog(int docID)
        {
            InitializeComponent();
            if (ErrorShower.ErrorShown)
                TopMost = false;
            this.docID = docID;
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
                new System.ComponentModel.ComponentResourceManager(typeof (DocUserListDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.empList = new System.Windows.Forms.ListView();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // empList
            // 
            resources.ApplyResources(this.empList, "empList");
            this.empList.FullRowSelect = true;
            this.empList.Name = "empList";
            this.empList.ShowItemToolTips = true;
            this.empList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.empList.UseCompatibleStateImageBehavior = false;
            this.empList.View = System.Windows.Forms.View.Details;
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // DocUserListDialog
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonOK;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.empList);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DocUserListDialog";
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DocUserListDialog_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private void DocUserListDialog_Load(object sender, EventArgs e)
        {
            Text += docID.ToString() + ")";

            empList.Columns.Add(Environment.StringResources.GetString("Employee"), 180, HorizontalAlignment.Left);
            empList.Columns.Add(Environment.StringResources.GetString("InWork"), empList.Width - 200,
                                HorizontalAlignment.Center);

            using (DataTable dt = Environment.DocData.GetDocRights(docID))
            using (DataTableReader dr = dt.CreateDataReader())
            {
                while (dr.Read())
                {
                    var empID = (int) dr[Environment.DocData.EmpIDField];

                    var values = new string[2];

                    values[0] = dr[Environment.DocData.EmpField].ToString();

                    var rights = (int) dr[Environment.DocData.InWorkField];

                    if (rights == 1)
                        values[1] = Environment.StringResources.GetString("Yes");

                    var li = new ListItem(empID, values);
                    li.ToolTipText = li.Text;
                    empList.Items.Add(li);
                }
                dr.Close();
                dr.Dispose();
                dt.Dispose();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}