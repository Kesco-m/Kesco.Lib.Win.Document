using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Links;
using Kesco.Lib.Win.Document.Blocks;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class SvyazannyiDokument : Base
    {
        private int docID;

        private DocBlock block;
        private CheckBox cbVytekayuschie;
        private CheckBox cbOsnovaniya;
        private Panel pnlOsnovaniya;
        private RadioButton rbOsnovaniyaNeposredstvennye;
        private RadioButton rbOsnovaniyaVse;
        private Panel pnlVytekayuschie;
        private RadioButton rbVytekayuschieNeposredstvennye;
        private RadioButton rbVytekayuschieVse;

        private Container components;

        public SvyazannyiDokument()
        {
            InitializeComponent();
            MyInitializeComponent();
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
                new System.ComponentModel.ComponentResourceManager(typeof (SvyazannyiDokument));
            this.cbVytekayuschie = new System.Windows.Forms.CheckBox();
            this.cbOsnovaniya = new System.Windows.Forms.CheckBox();
            this.pnlOsnovaniya = new System.Windows.Forms.Panel();
            this.rbOsnovaniyaNeposredstvennye = new System.Windows.Forms.RadioButton();
            this.rbOsnovaniyaVse = new System.Windows.Forms.RadioButton();
            this.pnlVytekayuschie = new System.Windows.Forms.Panel();
            this.rbVytekayuschieNeposredstvennye = new System.Windows.Forms.RadioButton();
            this.rbVytekayuschieVse = new System.Windows.Forms.RadioButton();
            this.pnlOsnovaniya.SuspendLayout();
            this.pnlVytekayuschie.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbVytekayuschie
            // 
            resources.ApplyResources(this.cbVytekayuschie, "cbVytekayuschie");
            this.cbVytekayuschie.Name = "cbVytekayuschie";
            this.cbVytekayuschie.CheckedChanged += new System.EventHandler(this.MyRefresh);
            // 
            // cbOsnovaniya
            // 
            resources.ApplyResources(this.cbOsnovaniya, "cbOsnovaniya");
            this.cbOsnovaniya.Name = "cbOsnovaniya";
            this.cbOsnovaniya.CheckedChanged += new System.EventHandler(this.MyRefresh);
            // 
            // pnlOsnovaniya
            // 
            resources.ApplyResources(this.pnlOsnovaniya, "pnlOsnovaniya");
            this.pnlOsnovaniya.Controls.Add(this.rbOsnovaniyaNeposredstvennye);
            this.pnlOsnovaniya.Controls.Add(this.rbOsnovaniyaVse);
            this.pnlOsnovaniya.Name = "pnlOsnovaniya";
            // 
            // rbOsnovaniyaNeposredstvennye
            // 
            resources.ApplyResources(this.rbOsnovaniyaNeposredstvennye, "rbOsnovaniyaNeposredstvennye");
            this.rbOsnovaniyaNeposredstvennye.Name = "rbOsnovaniyaNeposredstvennye";
            // 
            // rbOsnovaniyaVse
            // 
            resources.ApplyResources(this.rbOsnovaniyaVse, "rbOsnovaniyaVse");
            this.rbOsnovaniyaVse.Name = "rbOsnovaniyaVse";
            // 
            // pnlVytekayuschie
            // 
            resources.ApplyResources(this.pnlVytekayuschie, "pnlVytekayuschie");
            this.pnlVytekayuschie.Controls.Add(this.rbVytekayuschieNeposredstvennye);
            this.pnlVytekayuschie.Controls.Add(this.rbVytekayuschieVse);
            this.pnlVytekayuschie.Name = "pnlVytekayuschie";
            // 
            // rbVytekayuschieNeposredstvennye
            // 
            resources.ApplyResources(this.rbVytekayuschieNeposredstvennye, "rbVytekayuschieNeposredstvennye");
            this.rbVytekayuschieNeposredstvennye.Name = "rbVytekayuschieNeposredstvennye";
            // 
            // rbVytekayuschieVse
            // 
            resources.ApplyResources(this.rbVytekayuschieVse, "rbVytekayuschieVse");
            this.rbVytekayuschieVse.Name = "rbVytekayuschieVse";
            // 
            // SvyazannyiDokument
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.pnlVytekayuschie);
            this.Controls.Add(this.pnlOsnovaniya);
            this.Controls.Add(this.cbOsnovaniya);
            this.Controls.Add(this.cbVytekayuschie);
            this.Name = "SvyazannyiDokument";

            this.Controls.SetChildIndex(this.cbVytekayuschie, 0);
            this.Controls.SetChildIndex(this.cbOsnovaniya, 0);
            this.Controls.SetChildIndex(this.pnlOsnovaniya, 0);
            this.Controls.SetChildIndex(this.pnlVytekayuschie, 0);
            this.pnlOsnovaniya.ResumeLayout(false);
            this.pnlVytekayuschie.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private void MyInitializeComponent()
        {
            CloseOnEnter = false;
            block = new DocBlock {Location = new Point(2, 4), TabIndex = 0, Dock = DockStyle.Top};
            block.Selected += block_Selected;
            Controls.Add(block);
        }

        private void block_Selected(object source, BlockEventArgs e)
        {
            docID = e.ID;
            block.Text = e.Name;
        }

        private void MyRefresh(object sender, EventArgs e)
        {
            rbVytekayuschieNeposredstvennye.Enabled = rbVytekayuschieVse.Enabled = cbVytekayuschie.Checked;
            rbOsnovaniyaNeposredstvennye.Enabled = rbOsnovaniyaVse.Enabled = cbOsnovaniya.Checked;
        }

        protected override void FillElement()
        {
            base.FillElement();

            var opt = (СвязанСДокументом) option;

            if (docID <= 0)
                throw new Exception(
                    Environment.StringResources.GetString("Search_Dialogs_SvyazannyiDokument_FillElement_Error1"));
            if ((!cbOsnovaniya.Checked) && (!cbVytekayuschie.Checked))
                throw new Exception(
                    Environment.StringResources.GetString("Search_Dialogs_SvyazannyiDokument_FillElement_Error2"));

            opt.DocumentID = docID;
            opt.Osnovaniya = cbOsnovaniya.Checked;
            opt.OsnovaniyaAll = rbOsnovaniyaVse.Checked;
            opt.Vytekayuschie = cbVytekayuschie.Checked;
            opt.VytekayuschieAll = rbVytekayuschieVse.Checked;
        }

        protected override void FillForm()
        {
            base.FillForm();
            var opt = (СвязанСДокументом) option;

            cbOsnovaniya.Checked = opt.Osnovaniya;
            rbOsnovaniyaVse.Checked = opt.OsnovaniyaAll;
            rbOsnovaniyaNeposredstvennye.Checked = !opt.OsnovaniyaAll;

            cbVytekayuschie.Checked = opt.Vytekayuschie;
            rbVytekayuschieVse.Checked = opt.VytekayuschieAll;
            rbVytekayuschieNeposredstvennye.Checked = !opt.VytekayuschieAll;

            docID = opt.DocumentID;
            block.Text = (docID > 0 ? DBDocString.Format(docID) : "");
            rbVytekayuschieNeposredstvennye.Enabled = rbVytekayuschieVse.Enabled = cbVytekayuschie.Checked;
            rbOsnovaniyaNeposredstvennye.Enabled = rbOsnovaniyaVse.Enabled = cbOsnovaniya.Checked;
        }
    }
}