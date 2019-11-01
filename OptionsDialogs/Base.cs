using System;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Options;

namespace Kesco.Lib.Win.Document.OptionsDialogs
{
    public class Base : FreeDialog
    {
        protected Option option;

        private Button bOk;
        private Button bCancel;
        private Panel panelBottom;
        private Panel panelOKCancel;
        private Container components;

        public Base(Option option)
        {
            if (option == null)
                throw new Exception(Environment.StringResources.GetString("OptionsDialogs_Base_Error1"));
            this.option = option;

            InitializeComponent();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Base));
			this.panelBottom = new System.Windows.Forms.Panel();
			this.panelOKCancel = new System.Windows.Forms.Panel();
			this.bOk = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.panelBottom.SuspendLayout();
			this.panelOKCancel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelBottom
			// 
			this.panelBottom.Controls.Add(this.panelOKCancel);
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Name = "panelBottom";
			// 
			// panelOKCancel
			// 
			this.panelOKCancel.Controls.Add(this.bOk);
			this.panelOKCancel.Controls.Add(this.bCancel);
			resources.ApplyResources(this.panelOKCancel, "panelOKCancel");
			this.panelOKCancel.Name = "panelOKCancel";
			// 
			// bOk
			// 
			this.bOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.bOk, "bOk");
			this.bOk.Name = "bOk";
			this.bOk.Click += new System.EventHandler(this.bOk_Click);
			// 
			// bCancel
			// 
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.bCancel, "bCancel");
			this.bCancel.Name = "bCancel";
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// Base
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.bCancel;
			this.Controls.Add(this.panelBottom);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Base";
			this.ShowInTaskbar = false;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Base_Closing);
			this.panelBottom.ResumeLayout(false);
			this.panelOKCancel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        protected bool CloseOnEnter
        {
            get { return AcceptButton == bOk; }
            set { AcceptButton = value ? bOk : null; }
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            End(DialogResult.OK);
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }

        private void Base_Closing(object sender, CancelEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
                try
                {
                    option.Validate(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    e.Cancel = true;
                }
        }
    }
}