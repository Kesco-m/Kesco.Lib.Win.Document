using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Blocks
{
    public class PersonBoolBlock : PersonBlock
    {
        private RadioButton radioAND;
        private RadioButton radioOR;
        private IContainer components;

        public PersonBoolBlock()
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

        #region Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (PersonBoolBlock));
            this.radioAND = new System.Windows.Forms.RadioButton();
            this.radioOR = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // radioAND
            // 
            resources.ApplyResources(this.radioAND, "radioAND");
            this.radioAND.Name = "radioAND";
            // 
            // radioOR
            // 
            resources.ApplyResources(this.radioOR, "radioOR");
            this.radioOR.Name = "radioOR";
            // 
            // PersonBoolBlock
            // 
            this.Controls.Add(this.radioAND);
            this.Controls.Add(this.radioOR);
            this.Name = "PersonBoolBlock";
            resources.ApplyResources(this, "$this");
            this.Controls.SetChildIndex(this.radioOR, 0);
            this.Controls.SetChildIndex(this.radioAND, 0);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}