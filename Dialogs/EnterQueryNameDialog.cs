using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public class EnterQueryNameDialog : FreeDialog
    {
        private readonly string oldName;
        private readonly string genName;
        private string lastName;

        private Button buttonCancel;
        private Button buttonOK;
        private Label label;
        private TextBox name;
        private CheckBox check;

        private Container components;

        public EnterQueryNameDialog(string oldName, string genName)
        {
            InitializeComponent();

            this.oldName = oldName;
            this.genName = genName;

            name.Text = genName;
        }

        #region Accessors

        public string QueryName
        {
            get { return name.Text; }
        }

        #endregion

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnterQueryNameDialog));
			this.label = new System.Windows.Forms.Label();
			this.name = new System.Windows.Forms.TextBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.check = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label
			// 
			resources.ApplyResources(this.label, "label");
			this.label.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label.Name = "label";
			// 
			// name
			// 
			resources.ApplyResources(this.name, "name");
			this.name.Name = "name";
			this.name.TextChanged += new System.EventHandler(this.textBox_TextChanged);
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// check
			// 
			resources.ApplyResources(this.check, "check");
			this.check.Name = "check";
			this.check.CheckedChanged += new System.EventHandler(this.check_CheckedChanged);
			// 
			// EnterQueryNameDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.check);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.name);
			this.Controls.Add(this.label);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EnterQueryNameDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = (name.Text.Length > 0);
        }

        private void check_CheckedChanged(object sender, EventArgs e)
        {
            string newName = (check.Checked) ? oldName : genName;
            if (lastName != null)
                newName = lastName;

            lastName = name.Text;
            name.Text = newName;
        }
    }
}