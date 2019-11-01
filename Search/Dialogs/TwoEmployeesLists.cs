using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;
using Kesco.Lib.Win.Document.Blocks;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class TwoEmployeesLists : Base
    {
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private Panel panel6;
        private Button buttonDeleteFirst;
        private Button buttonDeleteSecond;
        private ListBox listBoxSecond;
        private ListBox listBoxFirst;
        private EmployeeBlock employeeBlockSecond;
        private EmployeeBlock employeeBlockFirst;
        private Container components;

        protected ArrayList keys = new ArrayList();
        protected ArrayList keys2 = new ArrayList();

        public TwoEmployeesLists()
        {
            InitializeComponent();
            employeeBlockFirst.Parser = new Blocks.Parsers.EmployeeParser(Environment.EmpData, true);
            employeeBlockSecond.Parser = new Blocks.Parsers.EmployeeParser(Environment.EmpData, true);
            employeeBlockFirst.Label = Environment.StringResources.GetString("From");
            CloseOnEnter = false;
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
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (TwoEmployeesLists));
            this.panel1 = new System.Windows.Forms.Panel();
            this.employeeBlockSecond = new EmployeeBlock();
            this.panel4 = new System.Windows.Forms.Panel();
            this.listBoxSecond = new System.Windows.Forms.ListBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.buttonDeleteSecond = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.employeeBlockFirst = new EmployeeBlock();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listBoxFirst = new System.Windows.Forms.ListBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.buttonDeleteFirst = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.employeeBlockSecond);
            this.panel1.Controls.Add(this.panel4);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // employeeBlockSecond
            // 
            resources.ApplyResources(this.employeeBlockSecond, "employeeBlockSecond");
            this.employeeBlockSecond.BackColor = System.Drawing.SystemColors.Control;
            this.employeeBlockSecond.ButtonSide = EmployeeBlock.ButtonSideEnum.Left;
            this.employeeBlockSecond.FullText = "";
            this.employeeBlockSecond.Name = "employeeBlockSecond";
            this.employeeBlockSecond.ParamStr = "clid=3&UserAccountDisabled=0&return=2";
            this.employeeBlockSecond.EmployeeSelected +=
                new EmployeeBlockEventHandler(this.employeeBlockSecond_EmployeeSelected);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.listBoxSecond);
            this.panel4.Controls.Add(this.panel5);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // listBoxSecond
            // 
            resources.ApplyResources(this.listBoxSecond, "listBoxSecond");
            this.listBoxSecond.Name = "listBoxSecond";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.buttonDeleteSecond);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // buttonDeleteSecond
            // 
            resources.ApplyResources(this.buttonDeleteSecond, "buttonDeleteSecond");
            this.buttonDeleteSecond.Name = "buttonDeleteSecond";
            this.buttonDeleteSecond.Click += new System.EventHandler(this.buttonDeleteSecond_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.employeeBlockFirst);
            this.panel2.Controls.Add(this.panel3);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // employeeBlockFirst
            // 
            resources.ApplyResources(this.employeeBlockFirst, "employeeBlockFirst");
            this.employeeBlockFirst.BackColor = System.Drawing.SystemColors.Control;
            this.employeeBlockFirst.ButtonSide = EmployeeBlock.ButtonSideEnum.Left;
            this.employeeBlockFirst.FullText = "";
            this.employeeBlockFirst.Name = "employeeBlockFirst";
            this.employeeBlockFirst.ParamStr = "clid=3&UserAccountDisabled=0&return=2";
            this.employeeBlockFirst.EmployeeSelected +=
                new EmployeeBlockEventHandler(this.employeeBlockFirst_EmployeeSelected);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.listBoxFirst);
            this.panel3.Controls.Add(this.panel6);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // listBoxFirst
            // 
            resources.ApplyResources(this.listBoxFirst, "listBoxFirst");
            this.listBoxFirst.Name = "listBoxFirst";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.buttonDeleteFirst);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // buttonDeleteFirst
            // 
            resources.ApplyResources(this.buttonDeleteFirst, "buttonDeleteFirst");
            this.buttonDeleteFirst.Name = "buttonDeleteFirst";
            this.buttonDeleteFirst.Click += new System.EventHandler(this.buttonDeleteFirst_Click);
            // 
            // TwoEmployeesLists
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "TwoEmployeesLists";
            this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        protected override void FillElement()
        {
            base.FillElement();
            if (option == null)
                return;

            var o = (EmployeeTwoValueListOption) option;
            o.SetValue(keys.Cast<string>().Aggregate("",
                                                     (current, key) =>
                                                     current + ((current.Length == 0 ? "" : ",") + key)));
            o.SetValue2(keys2.Cast<string>().Aggregate("",
                                                       (current, key) =>
                                                       current + ((current.Length == 0 ? "" : ",") + key)));
        }

        protected override void FillForm()
        {
            base.FillForm();
            if (option == null) return;

            var o = (EmployeeTwoValueListOption) option;
            foreach (string key in o.GetValues(false))
                keys.Add(key);
            foreach (string key in o.GetValues2(false))
                keys2.Add(key);
            lbRefresh();
        }

        protected void lbRefresh()
        {
            var o = (ListOption) option;
            listBoxFirst.Items.Clear();
            string s;
            foreach (string key in keys)
            {
                s = (listBoxFirst.Items.Count > 0 ? Environment.StringResources.GetString("Or") + " " : "");
                listBoxFirst.Items.Add(s + o.GetItemText(key));
            }

            listBoxSecond.Items.Clear();
            foreach (string key in keys2)
            {
                s = (listBoxSecond.Items.Count > 0 ? Environment.StringResources.GetString("Or") + " " : "");
                listBoxSecond.Items.Add(s + o.GetItemText(key));
            }
        }

        private void buttonDeleteFirst_Click(object sender, EventArgs e)
        {
            if (listBoxFirst.SelectedIndex < 0 || listBoxFirst.SelectedIndex >= listBoxFirst.Items.Count)
                return;
            RemoveKey((string) keys[listBoxFirst.SelectedIndex]);
        }

        private void buttonDeleteSecond_Click(object sender, EventArgs e)
        {
            if (listBoxSecond.SelectedIndex < 0 || listBoxSecond.SelectedIndex >= listBoxSecond.Items.Count)
                return;
            RemoveKey2((string) keys2[listBoxSecond.SelectedIndex]);
        }

        public void AddKey(string key)
        {
            if (keys.Contains(key))
                return;
            keys.Add(key);
            lbRefresh();
        }

        public void AddKey2(string key)
        {
            if (keys2.Contains(key))
                return;
            keys2.Add(key);
            lbRefresh();
        }

        public virtual void RemoveKey(string key)
        {
            if (!keys.Contains(key))
                return;
            keys.Remove(key);
            lbRefresh();
        }

        public virtual void RemoveKey2(string key)
        {
            if (!keys2.Contains(key))
                return;
            keys2.Remove(key);
            lbRefresh();
        }

        public void RemoveAllKeys()
        {
            keys.Clear();
            keys2.Clear();
            lbRefresh();
        }

        private void employeeBlockFirst_EmployeeSelected(object source, EmployeeBlockEventArgs e)
        {
            if (e.Emps == null)
                return;

            foreach (var t in e.Emps)
                AddKey(t.ID.ToString());
        }

        private void employeeBlockSecond_EmployeeSelected(object source, EmployeeBlockEventArgs e)
        {
            if (e.Emps == null)
                return;

            foreach (var t in e.Emps)
                AddKey2(t.ID.ToString());
        }
    }
}