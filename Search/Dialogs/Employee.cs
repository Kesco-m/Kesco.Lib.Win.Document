using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.Sign;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;
using Kesco.Lib.Win.Document.Blocks;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class Employee : Base
    {
        private int id;

        private EmployeeBlock eb;
        private Label label1;
        private Container components;

        public EmployeeListOption Option
        {
            get { return (EmployeeListOption) option; }
        }

        public Employee()
        {
            InitializeComponent();
            InitializeUserBlock();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Employee));
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Employee
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.label1);
            this.Name = "Employee";
            this.Controls.SetChildIndex(this.label1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private void InitializeUserBlock()
        {
            PanelBottomVisible = false;
            eb = new EmployeeBlock
                     {
                         Location = new Point(10, 10),
                         Parser = new Blocks.Parsers.EmployeeParser(Environment.EmpData, true),
                         TabIndex = 0,
                         Dock = DockStyle.Top,
                         label = Environment.StringResources.GetString("ToSearch"),
                         singleLabel = Environment.StringResources.GetString("Choose"),
                         ParamStr = "&userstate=3&return=2"
                     };
            eb.EmployeeSelected += eb_EmployeeSelected;
            Controls.Add(eb);
        }

        protected override void FillElement()
        {
            elOption.SetAttribute("value", id == 0 ? "" : id.ToString());
        }

        protected override void FillForm()
        {
            var opt = (Подписан) Data.DALC.Documents.Search.Option.CreateOption(elOption);
            string val = elOption.GetAttribute("value");
            id = val.Length == 0 ? 0 : int.Parse(val);
            label1.Text = id == 0
                              ? Environment.StringResources.GetString("Search_Dialogs_Employee_Message1")
                              : opt.GetItemText(id.ToString());
        }

        private void eb_EmployeeSelected(object source, EmployeeBlockEventArgs e)
        {
            if (e.Emps == null || e.Emps.Length == 0)
                return;

            id = e.Emps[0].ID;
            label1.Text = id == 0
                              ? Environment.StringResources.GetString("Search_Dialogs_Employee_Message1")
                              : Option.GetItemText(id.ToString());
            End(DialogResult.OK);
        }
    }
}