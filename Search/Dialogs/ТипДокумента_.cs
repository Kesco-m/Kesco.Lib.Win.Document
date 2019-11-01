using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Select;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class ТипДокумента_ : SelectTypeDialog
    {
        public Panel panel;
        public CheckBox cbSimilar;
        public CheckBox cbSubtypes;
        private Container components;

        public ТипДокумента_(int typeID) : this(typeID, false)
        {
        }

        public ТипДокумента_(int typeID, bool formNeeded) : base(typeID, formNeeded)
        {
            MyInitializeComponent();
        }

        private void MyInitializeComponent()
        {
            DoubleBuffered = true;
            panel = new Panel();
            cbSimilar = new CheckBox();
            cbSubtypes = new CheckBox();

            panel.SuspendLayout();
            SuspendLayout();

            panel.Location = new Point(270, 0);
            panel.Size = new Size(250, 42);

            cbSimilar.Location = new Point(4, 20);
            cbSimilar.Name = "cbSimilar";
            cbSimilar.AutoSize = true;
            cbSimilar.TabIndex = 0;
            cbSimilar.Text = Environment.StringResources.GetString("Search_Dialogs_ТипДокумента_Message1");
            // 
            // cbSubtypes
            // 
            cbSubtypes.Location = new Point(4, 4);
            cbSubtypes.Name = "cbSubtypes";
            cbSubtypes.AutoSize = true;
            cbSubtypes.TabIndex = 1;
            cbSubtypes.Checked = true;
            cbSubtypes.Text = Environment.StringResources.GetString("Search_Dialogs_ТипДокумента_Message2");

            panel.Controls.Add(cbSimilar);
            panel.Controls.Add(cbSubtypes);
            Controls.Add(panel);


            ResumeLayout();
            panel.ResumeLayout();
            panel.BringToFront();
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
    }
}