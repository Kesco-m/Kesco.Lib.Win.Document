using System;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Dialogs;

namespace Kesco.Lib.Win.Document.Controls
{
    public class CheckControl : UserControl
    {
        private RichTextBoxEx richTextBox;
        private PictureBox pictureBoxEdit;
        private int baseHeight;

        public event EventHandler CheckedChanged;
        public event EventHandler EditPush;

        private void OnCheckedChanged()
        {
            if (CheckedChanged != null)
                CheckedChanged(this, EventArgs.Empty);
        }

        private void OnEditPush()
        {
            try
            {
                if (EditPush != null)
                    EditPush.DynamicInvoke(new object[] {this, EventArgs.Empty});
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        private CheckBox checkBox;
        private PictureBox pictureBox;
        private Container components;

        public CheckControl()
        {
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

        #region Component Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (CheckControl));
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.richTextBox = new RichTextBoxEx();
            this.pictureBoxEdit = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBoxEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBox
            // 
            resources.ApplyResources(this.checkBox, "checkBox");
            this.checkBox.Name = "checkBox";
            this.checkBox.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
            // 
            // pictureBox
            // 
            resources.ApplyResources(this.pictureBox, "pictureBox");
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.TabStop = false;
            this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // richTextBox
            // 
            resources.ApplyResources(this.richTextBox, "richTextBox");
            this.richTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox.HideSelection = false;
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.TabStop = false;
            this.richTextBox.LinkClicked +=
                new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox_LinkClicked);
            // 
            // pictureBoxEdit
            // 
            resources.ApplyResources(this.pictureBoxEdit, "pictureBoxEdit");
            this.pictureBoxEdit.Name = "pictureBoxEdit";
            this.pictureBoxEdit.TabStop = false;
            this.pictureBoxEdit.Click += new System.EventHandler(this.pictureBoxEdit_Click);
            // 
            // CheckControl
            // 
            this.Controls.Add(this.pictureBoxEdit);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.checkBox);
            this.DoubleBuffered = true;
            this.Name = "CheckControl";
            resources.ApplyResources(this, "$this");
            this.Load += new System.EventHandler(this.CheckControl_Load);
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBoxEdit)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private void pictureBox_Click(object sender, EventArgs e)
        {
            Collection.Remove(this);
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            OnCheckedChanged();
        }

        private void pictureBoxEdit_Click(object sender, EventArgs e)
        {
            OnEditPush();
        }

        public int ID { get; set; }

        public string SelectedText
        {
            get { return richTextBox.SelectedText; }
            set { richTextBox.SelectedText = value; }
        }

        public override string Text
        {
            get { return richTextBox.Text; }
            set { richTextBox.Text = value; }
        }

        public void InsertLink(string text, string link)
        {
            richTextBox.InsertLink(text, link);
        }

        public void InsertLink(string text, string link, int position)
        {
            richTextBox.InsertLink(text, link, position);
        }

        public string PersonName { get; set; }

        public int SendType { get; set; }

        public CheckedControlCollection Collection { get; set; }

        public bool Checked
        {
            get { return checkBox.Checked; }
            set { checkBox.Checked = value; }
        }

        public bool ShowEdit
        {
            get { return pictureBoxEdit.Visible; }
            set { pictureBoxEdit.Visible = value; }
        }

        public bool ShowDelete
        {
            get { return pictureBox.Visible; }
            set { pictureBox.Visible = value; }
        }

        public bool ShowCheckBox
        {
            get { return checkBox.Visible; }
            set { checkBox.Visible = value; }
        }

        /// <summary>
        ///   изменение размера контрола
        /// </summary>
        public void UpdateSize()
        {
            if (!richTextBox.PreferredSize.IsEmpty)
                Height = (richTextBox.PreferredSize.Width/richTextBox.Width + 1)*baseHeight*
                              richTextBox.PreferredSize.Height/richTextBox.PreferredHeight;
        }

        private void CheckControl_Load(object sender, EventArgs e)
        {
            baseHeight = Height;
            UpdateSize();
        }

        private void richTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (e.LinkText.EndsWith("#Edit"))
                OnEditPush();
            else
                Environment.IEOpenOnURL(e.LinkText.Substring(e.LinkText.IndexOf('#') + 1));
        }
    }
}