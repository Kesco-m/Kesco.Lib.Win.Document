using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
	partial class LoopLinkDialog
	{
		/// <summary>
		///   Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoopLinkDialog));
			this.list = new System.Windows.Forms.ListView();
			this.labelMsg = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// list
			// 
			resources.ApplyResources(this.list, "list");
			this.list.FullRowSelect = true;
			this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.list.HideSelection = false;
			this.list.MultiSelect = false;
			this.list.Name = "list";
			this.list.ShowItemToolTips = true;
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			this.list.DoubleClick += new System.EventHandler(this.list_DoubleClick);
			this.list.MouseUp += new System.Windows.Forms.MouseEventHandler(this.list_MouseUp);
			// 
			// labelMsg
			// 
			resources.ApplyResources(this.labelMsg, "labelMsg");
			this.labelMsg.Name = "labelMsg";
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// LoopLinkDialog
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.labelMsg);
			this.Controls.Add(this.list);
			this.Controls.Add(this.buttonCancel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoopLinkDialog";
			this.Load += new System.EventHandler(this.ChooseDocDialog_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private Container components;

		private ListView list;
		private Button buttonCancel;
		private Label labelMsg;
	}
}