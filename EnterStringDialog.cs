using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document
{
    public class EnterStringDialog : FreeDialog
    {
        private readonly bool allowBlank;

        private Button buttonCancel;

        private Button buttonOK;
        private Label label;
        private TextBox textBox;
        private ErrorProvider errorProvider;
        private IContainer components;

        public EnterStringDialog(string caption, string description, string input) :
            this(caption, description, input, false)
        {
        }

        public EnterStringDialog(string caption, string description, string input, bool allowBlank) :
            this(caption, description, input, allowBlank, null)
        {
        }

        public EnterStringDialog(string caption, string description, string input, bool allowBlank, object data)
        {
            InitializeComponent();

            Text = caption;
            label.Text = description;
            textBox.Text = input;

            this.allowBlank = allowBlank;
            Data = data;
        }

        #region Accessors

        public string Input
        {
            get { return textBox.Text; }
        }

        public object Data { get; set; }

        #endregion

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
            this.components = new System.ComponentModel.Container();
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (EnterStringDialog));
            this.label = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AccessibleDescription = null;
            this.label.AccessibleName = null;
            resources.ApplyResources(this.label, "label");
            this.errorProvider.SetError(this.label, resources.GetString("label.Error"));
            this.label.Font = null;
            this.errorProvider.SetIconAlignment(this.label,
                                                ((System.Windows.Forms.ErrorIconAlignment)
                                                 (resources.GetObject("label.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label, ((int) (resources.GetObject("label.IconPadding"))));
            this.label.Name = "label";
            // 
            // textBox
            // 
            this.textBox.AccessibleDescription = null;
            this.textBox.AccessibleName = null;
            resources.ApplyResources(this.textBox, "textBox");
            this.textBox.BackgroundImage = null;
            this.errorProvider.SetError(this.textBox, resources.GetString("textBox.Error"));
            this.textBox.Font = null;
            this.errorProvider.SetIconAlignment(this.textBox,
                                                ((System.Windows.Forms.ErrorIconAlignment)
                                                 (resources.GetObject("textBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBox, ((int) (resources.GetObject("textBox.IconPadding"))));
            this.textBox.Name = "textBox";
            this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleDescription = null;
            this.buttonOK.AccessibleName = null;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.BackgroundImage = null;
            this.errorProvider.SetError(this.buttonOK, resources.GetString("buttonOK.Error"));
            this.buttonOK.Font = null;
            this.errorProvider.SetIconAlignment(this.buttonOK,
                                                ((System.Windows.Forms.ErrorIconAlignment)
                                                 (resources.GetObject("buttonOK.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.buttonOK, ((int) (resources.GetObject("buttonOK.IconPadding"))));
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleDescription = null;
            this.buttonCancel.AccessibleName = null;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackgroundImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.errorProvider.SetError(this.buttonCancel, resources.GetString("buttonCancel.Error"));
            this.buttonCancel.Font = null;
            this.errorProvider.SetIconAlignment(this.buttonCancel,
                                                ((System.Windows.Forms.ErrorIconAlignment)
                                                 (resources.GetObject("buttonCancel.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.buttonCancel,
                                              ((int) (resources.GetObject("buttonCancel.IconPadding"))));
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // EnterStringDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.label);
            this.Controls.Add(this.buttonCancel);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterStringDialog";
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            textBox.Text = Replacer.ReplaceNonPrintableCharacters(textBox.Text);
            if (allowBlank || (!allowBlank && (textBox.Text.Length > 0)))
                End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = Replacer.ReplaceNonPrintableCharacters(textBox.Text).Length > 0;
        }
    }
}