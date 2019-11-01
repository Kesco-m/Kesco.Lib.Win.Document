using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document.Blocks;

namespace Kesco.Lib.Win.Document.Select
{
    public class SelectEmployeeDialog : FreeDialog
    {
        private SynchronizedCollection<Keys> keyLocker;
        private EmployeeBlock employeeBlock;
        private Label label;

        /// <summary>
        ///   Required designer variable.
        /// </summary>
        private Container components;

        #region Accessors

        public Employee Employee { get; private set; }

        #endregion

        public SelectEmployeeDialog(string buttonLabel)
        {
            InitializeComponent();

            keyLocker = new SynchronizedCollection<Keys>();

            CreateEmpBlock(buttonLabel);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectEmployeeDialog));
			this.label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label
			// 
			resources.ApplyResources(this.label, "label");
			this.label.Name = "label";
			// 
			// SelectEmployeeDialog
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.label);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectEmployeeDialog";
			this.ShowInTaskbar = false;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SelectEmployeeDialog_KeyDown);
			this.ResumeLayout(false);

        }

        #endregion

        private void CreateEmpBlock(string LabelText)
        {
            label.Text = LabelText;
            employeeBlock = new EmployeeBlock(Environment.StringResources.GetString("Search"));

            employeeBlock.EmployeeSelected += EmployeeBlockEmployeeSelected;

            employeeBlock.Parser = new Blocks.Parsers.EmployeeParser(Environment.EmpData, false);

            employeeBlock.Location = new Point(16, 32);
            employeeBlock.Name = "employeeBlock";
            employeeBlock.Size = new Size(312, 32);
            employeeBlock.TabIndex = 0;

            Controls.Add(employeeBlock);
        }

        private void EmployeeBlockEmployeeSelected(object source, EmployeeBlockEventArgs e)
        {
            if (e.Emps != null && e.Emps.Length > 0)
            {
                Employee = e.Emps[0];
                End(DialogResult.OK);
            }
            else
                End(DialogResult.Cancel);
        }

        private void SelectEmployeeDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (keyLocker.Contains(e.KeyData))
                return;
            keyLocker.Add(e.KeyData);
            try
            {
                if (e.KeyData == Keys.Escape)
                {
                    e.Handled = true;
                    End(DialogResult.Cancel);
                }
            }
            catch
            {
            }
            finally
            {
                keyLocker.Remove(e.KeyData);
            }
        }
    }
}