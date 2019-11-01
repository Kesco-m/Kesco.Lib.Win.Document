using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Blocks;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class Ëèöà : Base
    {
        private RadioButton rbAND;
        private RadioButton rbOR;
        private PersonBlock pb;
        private Container components;

        public Ëèöà()
        {
            InitializeComponent();
            InitializePersonBlock();
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
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (Ëèöà));
            this.rbAND = new System.Windows.Forms.RadioButton();
            this.rbOR = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // rbAND
            // 
            resources.ApplyResources(this.rbAND, "rbAND");
            this.rbAND.Name = "rbAND";
            // 
            // rbOR
            // 
            resources.ApplyResources(this.rbOR, "rbOR");
            this.rbOR.Name = "rbOR";
            // 
            // Ëèöà
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.rbOR);
            this.Controls.Add(this.rbAND);
            this.Name = "Ëèöà";
            this.Controls.SetChildIndex(this.rbAND, 0);
            this.Controls.SetChildIndex(this.rbOR, 0);
            this.ResumeLayout(false);
        }

        #endregion

        private void InitializePersonBlock()
        {
            pb = new PersonBlock {Location = new Point(10, 10), TabIndex = 0};
            Controls.Add(pb);
        }

        public static string GetText(int id)
        {
            object ret = Environment.PersonData.GetField(Environment.PersonData.NameField, id);
            return ret != null ? ret.ToString() : "#" + id.ToString();
        }

        protected override void FillElement()
        {
            string val = "";
            int[] IDs = pb.PersonIDs;
            for (int i = 0; i < IDs.Length; i++)
                val += (val.Equals("") ? "" : ",") + IDs[i].ToString();

            elOption.SetAttribute("value", val);
            elOption.SetAttribute("mode", rbOR.Checked ? "or" : "and");

            base.FillElement();
        }

        protected override void FillForm()
        {
            AcceptButton = null;
            var r = new Regex("^\\d{1,9}$");
            string[] val = elOption.GetAttribute("value").Split(',');

            foreach (string t in val.Where(t => r.IsMatch(t)))
            {
                int id = int.Parse(t);
                pb.AddPerson(id, GetText(id));
            }

            switch (elOption.GetAttribute("mode").ToLower())
            {
                case "or":
                    rbOR.Checked = true;
                    break;
                default:
                    rbAND.Checked = true;
                    break;
            }

            base.FillForm();
        }
    }
}