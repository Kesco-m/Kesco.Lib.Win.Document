using System;
using System.ComponentModel;
using System.Drawing;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;
using Kesco.Lib.Win.Document.Blocks;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class PersonList : TemplateList
    {
        private Container components;

        private PersonBlock pb;

        public PersonList()
        {
            InitializeComponent();
            DoubleBuffered = true;
            InitializePersonBlock();
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
            // 
            // PersonList
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(282, 176);
            this.Name = "PersonList";
            this.Text = "PersonList";
        }

        #endregion

        private void InitializePersonBlock()
        {
            CloseOnEnter = false;
            pb = new PersonBlock {Location = new Point(2, 4), TabIndex = 0};
            pb_CountChangedEnabled = true;
            pb.CountChanged += pb_CountChanged;

            Controls.Add(pb);
        }

        private bool pb_CountChangedEnabled;

        private void pb_CountChanged(object source, EventArgs e)
        {
            if (!pb_CountChangedEnabled)
                return;
            RemoveAllKeys();
            foreach (int id in pb.PersonIDs)
                AddKey(id.ToString());
        }

        protected override void FillForm()
        {
            base.FillForm();

            if (option == null) 
                return;

            var o =
                (ListOption) option;
            foreach (string key in o.GetValues(false))
                pb.AddPerson(int.Parse(key), key);
        }

        public override void RemoveKey(string key)
        {
            base.RemoveKey(key);
            pb_CountChangedEnabled = false;
            pb.Clear();
            pb_CountChangedEnabled = true;
            foreach (string id in GetValues())
                pb.AddPerson(int.Parse(id), id);
        }
    }
}