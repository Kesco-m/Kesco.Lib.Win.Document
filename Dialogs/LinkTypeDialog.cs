using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Kesco.Lib.Log;

namespace Kesco.Lib.Win.Document.Dialogs
{
    /// <summary>
    ///   ƒиалог создани€ св€зи между двум€ документами.
    /// </summary>
    public class LinkTypeDialog : FreeDialog
    {
        private SynchronizedCollection<Keys> keyLocker;

        private Label label1;
        private Button buttonBase;
        private Button buttonLink;
        private Label labelOne;
        private Label labelTwo;
        private Button buttonCancel;
        private ErrorProvider errorProvider;
        private ToolTip toolTip;
        private Label labelLink;
        private Label labelBase;
        private IContainer components;

        /// <summary>
        ///   ƒиалог создани€ св€зи между двум€ документами.
        /// </summary>
        /// <param name="firstID"> код документа отображаемого сверху </param>
        /// <param name="secondID"> код документа отображаемого снизу </param>
        public LinkTypeDialog(int firstID, int secondID)
        {
            InitializeComponent();

            keyLocker = new SynchronizedCollection<Keys>();

            this.OneID = firstID;
            this.TwoID = secondID;
            if (firstID == secondID)
            {
                Logger.WriteEx(new LogicalException("ќшибочное создание св€зи\n—овпадают коды документов",
                                                            " од документа: " + firstID.ToString(),
                                                            Assembly.GetExecutingAssembly().GetName()));
                Close();
                return;
            }

            Console.WriteLine("{0}: Loading properteis first linked: {1}", DateTime.Now.ToString("HH:mm:ss fff"), firstID);
            labelOne.Text = DBDocString.Format(firstID);
            Console.WriteLine("{0}: Loading properteis second linked: {1}", DateTime.Now.ToString("HH:mm:ss fff"), secondID);
            labelTwo.Text = DBDocString.Format(secondID);

            bool needCheck = false;
            DateTime firstDate = DateTime.MinValue;
            DateTime secondDate = DateTime.MinValue;

            object date = Environment.DocData.GetField(Environment.DocData.DateField, this.OneID);
            if (date != DBNull.Value)
            {
                firstDate = (DateTime) date;
                needCheck = true;
            }

            if (needCheck)
            {
                date = Environment.DocData.GetField(Environment.DocData.DateField, this.TwoID);
                if (date != DBNull.Value)
                    secondDate = (DateTime) date;
                else
                    needCheck = false;
            }

            if (needCheck)
            {
                if (firstDate < secondDate)
                {
                    SetToolTip(labelLink, buttonLink);
                }
                else if (firstDate > secondDate)
                {
                    SetToolTip(labelBase, buttonBase);
                }
            }
        }

        /// <summary>
        ///   ƒиалог создани€ св€зи между двум€ документами. »спользуетс€ дл€ св€зовани€ с еще не созданным документом.
        /// </summary>
        /// <param name="docString"> строка докумета дл€ св€зи </param>
        /// <param name="docID">  од документа дл€ св€зи </param>
        /// <param name="docData"> дата создаваемого документа дл€ блокировки ошибочной по времени св€зи </param>
        public LinkTypeDialog(string docString, int docID, DateTime docData)
        {
            InitializeComponent();

            keyLocker = new SynchronizedCollection<Keys>();

            TwoID = docID;

            labelOne.Text = docString;
            labelTwo.Text = DBDocString.Format(TwoID);

            if (docData.Equals(DateTime.MinValue))
                return;
            object date = Environment.DocData.GetField(Environment.DocData.DateField, TwoID);
            if (date != DBNull.Value)
            {
                var secondDate = (DateTime) date;
                if (docData < secondDate)
                {
                    SetToolTip(labelLink, buttonLink);
                }
                else if (docData > secondDate)
                {
                    SetToolTip(labelBase, buttonBase);
                }
            }
        }

        private void SetToolTip(Label label, Button button)
        {
            label.Text = button.Text;
            label.ForeColor = SystemColors.GrayText;
            label.BringToFront();
            button.Enabled = false;
            errorProvider.SetError(button, Environment.StringResources.GetString("Dialogs_LinkTypeDialog_ToolTip"));
            toolTip.SetToolTip(label, Environment.StringResources.GetString("Dialogs_LinkTypeDialog_ToolTip"));
            toolTip.SetToolTip(button, Environment.StringResources.GetString("Dialogs_LinkTypeDialog_ToolTip"));
            toolTip.Active = true;
        }

        #region Accessors

        public bool Basic { get; private set; }

        public int OneID { get; private set; }

        public string DocumentOneText
        {
            get { return labelOne.Text; }
        }

        public int TwoID { get; private set; }

        public string DocumentTwoText
        {
            get { return labelTwo.Text; }
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
            this.components = new System.ComponentModel.Container();
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (LinkTypeDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonBase = new System.Windows.Forms.Button();
            this.buttonLink = new System.Windows.Forms.Button();
            this.labelOne = new System.Windows.Forms.Label();
            this.labelTwo = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.labelLink = new System.Windows.Forms.Label();
            this.labelBase = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Name = "label1";
            // 
            // buttonBase
            // 
            resources.ApplyResources(this.buttonBase, "buttonBase");
            this.buttonBase.Name = "buttonBase";
            this.buttonBase.Click += new System.EventHandler(this.buttonBase_Click);
            // 
            // buttonLink
            // 
            resources.ApplyResources(this.buttonLink, "buttonLink");
            this.buttonLink.Name = "buttonLink";
            this.buttonLink.Click += new System.EventHandler(this.buttonLink_Click);
            // 
            // labelOne
            // 
            resources.ApplyResources(this.labelOne, "labelOne");
            this.labelOne.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelOne.Name = "labelOne";
            // 
            // labelTwo
            // 
            resources.ApplyResources(this.labelTwo, "labelTwo");
            this.labelTwo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelTwo.Name = "labelTwo";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // labelLink
            // 
            this.labelLink.FlatStyle = System.Windows.Forms.FlatStyle.System;
            resources.ApplyResources(this.labelLink, "labelLink");
            this.labelLink.Name = "labelLink";
            // 
            // labelBase
            // 
            this.labelBase.FlatStyle = System.Windows.Forms.FlatStyle.System;
            resources.ApplyResources(this.labelBase, "labelBase");
            this.labelBase.Name = "labelBase";
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 50;
            this.toolTip.AutoPopDelay = 0;
            this.toolTip.InitialDelay = 50;
            this.toolTip.ReshowDelay = 50;
            this.toolTip.ShowAlways = true;
            // 
            // LinkTypeDialog
            // 
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonBase);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonLink);
            this.Controls.Add(this.labelOne);
            this.Controls.Add(this.labelTwo);
            this.Controls.Add(this.labelLink);
            this.Controls.Add(this.labelBase);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LinkTypeDialog";

            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private void buttonBase_Click(object sender, EventArgs e)
        {
            Basic = true;
            End(DialogResult.OK);
        }

        private void buttonLink_Click(object sender, EventArgs e)
        {
            Basic = false;
            End(DialogResult.OK);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }
    }
}