using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Blocks;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class EmployeeList : TemplateList
    {
        private EmployeeBlock eb;
        private Container components;

        public EmployeeList()
        {
            InitializeComponent();
            InitializeUserBlock();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmployeeList));
			this.SuspendLayout();
			// 
			// EmployeeList
			// 
			resources.ApplyResources(this, "$this");
			this.Name = "EmployeeList";
			this.ResumeLayout(false);
		}

        #endregion

        private void InitializeUserBlock()
        {
            CloseOnEnter = false;
            eb = new EmployeeBlock
                     {
                         Location = new Point(8, 8),
                         Parser = new Blocks.Parsers.EmployeeParser(Environment.EmpData, true),
                         Size = new Size(Width - 16, 24),
                         Anchor = (((((AnchorStyles.Top)
                                      | AnchorStyles.Left)
                                     | AnchorStyles.Right))),
                         TabIndex = 0,
                         label = Environment.StringResources.GetString("ToSearch"),
                         singleLabel = Environment.StringResources.GetString("Choose"),
                         ParamStr = "&UserState=3&return=2"
                     };
            eb.EmployeeSelected += eb_EmployeeSelected;
            Controls.Add(eb);
        }

        private void eb_EmployeeSelected(object source, EmployeeBlockEventArgs e)
        {
            if (e.Emps == null)
                return;

            foreach (Data.Temp.Objects.Employee t in e.Emps)
                AddKey(t.ID.ToString());
        }

		protected override void FillForm()
		{
			base.FillForm();
			//if(elOption.
		}
    }
}