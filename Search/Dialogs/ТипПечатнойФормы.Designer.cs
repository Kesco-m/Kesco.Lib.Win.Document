namespace Kesco.Lib.Win.Document.Search.Dialogs
{
	partial class ТипПечатнойФормы
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ТипПечатнойФормы));
			this.bDelete = new System.Windows.Forms.Button();
			this.lv = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.bSelect = new System.Windows.Forms.Button();
			this.rbOR = new System.Windows.Forms.RadioButton();
			this.rbAND = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// bDelete
			// 
			resources.ApplyResources(this.bDelete, "bDelete");
			this.bDelete.Name = "bDelete";
			this.bDelete.Click += new System.EventHandler(this.bDelete_Click);
			// 
			// lv
			// 
			resources.ApplyResources(this.lv, "lv");
			this.lv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.lv.FullRowSelect = true;
			this.lv.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lv.Name = "lv";
			this.lv.UseCompatibleStateImageBehavior = false;
			this.lv.View = System.Windows.Forms.View.Details;
			this.lv.SelectedIndexChanged += new System.EventHandler(this.lv_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			resources.ApplyResources(this.columnHeader1, "columnHeader1");
			// 
			// bSelect
			// 
			resources.ApplyResources(this.bSelect, "bSelect");
			this.bSelect.Name = "bSelect";
			this.bSelect.Click += new System.EventHandler(this.bSelect_Click);
			// 
			// rbOR
			// 
			resources.ApplyResources(this.rbOR, "rbOR");
			this.rbOR.Name = "rbOR";
			// 
			// rbAND
			// 
			resources.ApplyResources(this.rbAND, "rbAND");
			this.rbAND.Name = "rbAND";
			// 
			// ТипПечатнойФормы
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.rbOR);
			this.Controls.Add(this.rbAND);
			this.Controls.Add(this.bDelete);
			this.Controls.Add(this.lv);
			this.Controls.Add(this.bSelect);
			this.Name = "ТипПечатнойФормы";
			this.Controls.SetChildIndex(this.bSelect, 0);
			this.Controls.SetChildIndex(this.lv, 0);
			this.Controls.SetChildIndex(this.bDelete, 0);
			this.Controls.SetChildIndex(this.rbAND, 0);
			this.Controls.SetChildIndex(this.rbOR, 0);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bDelete;
		private System.Windows.Forms.ListView lv;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Button bSelect;
		private System.Windows.Forms.RadioButton rbOR;
		private System.Windows.Forms.RadioButton rbAND;
	}
}
