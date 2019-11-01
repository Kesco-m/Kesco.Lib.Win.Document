using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Select
{
    public class SelectSenderDialog : FreeDialog
    {
        private TextBox textBoxName;
        private Button buttonOK;
        private Button buttonCancel;
        private RadioButton radioButtonYes;
        private Label label1;
        private RadioButton radioButtonNo;
        private IContainer components;

        public SelectSenderDialog(string contactString)
        {
            ContactString = contactString;

            InitializeComponent();
            label1.Text = label1.Text.Replace("$", contactString);
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

        #region Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectSenderDialog));
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.radioButtonYes = new System.Windows.Forms.RadioButton();
			this.radioButtonNo = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBoxName
			// 
			resources.ApplyResources(this.textBoxName, "textBoxName");
			this.textBoxName.Name = "textBoxName";
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.button_Click);
			// 
			// radioButtonYes
			// 
			resources.ApplyResources(this.radioButtonYes, "radioButtonYes");
			this.radioButtonYes.Checked = true;
			this.radioButtonYes.Name = "radioButtonYes";
			this.radioButtonYes.TabStop = true;
			this.radioButtonYes.CheckedChanged += new System.EventHandler(this.radioButtonYes_CheckedChanged);
			// 
			// radioButtonNo
			// 
			resources.ApplyResources(this.radioButtonNo, "radioButtonNo");
			this.radioButtonNo.Name = "radioButtonNo";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Name = "label1";
			// 
			// SelectSenderDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.radioButtonNo);
			this.Controls.Add(this.radioButtonYes);
			this.Controls.Add(this.textBoxName);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectSenderDialog";
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        public string SenderText
        {
            get { return textBoxName.Text; }
        }

        public string ContactString { get; private set; }

        private void button_Click(object sender, EventArgs e)
        {
            if (!Owner.Enabled)
                Owner.Enabled = true;
            Close();
        }

        private void radioButtonYes_CheckedChanged(object sender, EventArgs e)
        {
            textBoxName.Text = "";
            textBoxName.Enabled = radioButtonNo.Checked;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!Owner.Enabled)
                Owner.Enabled = true;
            DialogResult = ((radioButtonNo.Checked) ? DialogResult.Yes : DialogResult.OK);
            Close();
        }
    }
}