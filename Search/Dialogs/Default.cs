using System.ComponentModel;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class Default : Base
    {
        private Container components;

        public Default()
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

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // Default
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(378, 287);
            this.Name = "Default";
            this.Text = "Default";
        }

        #endregion

        protected override void FillElement()
        {
        }

        protected override void FillForm()
        {
        }
    }
}