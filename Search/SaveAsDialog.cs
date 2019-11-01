using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search
{
	public class SaveAsDialog : FreeDialog
	{
		private Button bSave;
		private Button bCancel;
		private Label label1;
		private ListBox lb;
		private Label label2;
		private TextBox tbSaveAs;

		private Container components;

		public SaveAsDialog()
		{
			InitializeComponent();
		}

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
			var resources =
				new System.ComponentModel.ComponentResourceManager(typeof(SaveAsDialog));
			this.bSave = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lb = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbSaveAs = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// bSave
			// 
			resources.ApplyResources(this.bSave, "bSave");
			this.bSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bSave.Name = "bSave";
			this.bSave.Click += new System.EventHandler(this.bSave_Click);
			// 
			// bCancel
			// 
			resources.ApplyResources(this.bCancel, "bCancel");
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Name = "bCancel";
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Name = "label1";
			// 
			// lb
			// 
			resources.ApplyResources(this.lb, "lb");
			this.lb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lb.Name = "lb";
			this.lb.SelectedIndexChanged += new System.EventHandler(this.lb_SelectedIndexChanged);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Name = "label2";
			// 
			// tbSaveAs
			// 
			resources.ApplyResources(this.tbSaveAs, "tbSaveAs");
			this.tbSaveAs.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tbSaveAs.Name = "tbSaveAs";
			this.tbSaveAs.TextChanged += new System.EventHandler(this.tbSaveAs_TextChanged);
			// 
			// SaveAsDialog
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tbSaveAs);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lb);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bSave);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SaveAsDialog";
			this.ShowInTaskbar = false;

			this.Load += new System.EventHandler(this.SaveAsDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private void SaveAsDialog_Load(object sender, EventArgs e)
		{
			List<string> ls = Environment.QueryData.GetSearchParameteries();
			if(ls.Count > 0)
				lb.Items.AddRange(ls.ToArray());
			bSave.Enabled = false;
		}

		private void lb_SelectedIndexChanged(object sender, EventArgs e)
		{
			tbSaveAs.Text = (string)lb.SelectedItem;
			bSave.Focus();
		}

		private void tbSaveAs_TextChanged(object sender, EventArgs e)
		{
			bSave.Enabled = tbSaveAs.Text.Trim().Length > 0;
		}

		private void bSave_Click(object sender, EventArgs e)
		{
			End(DialogResult.OK);
		}

		private void bCancel_Click(object sender, EventArgs e)
		{
			End(DialogResult.Cancel);
		}

		public string Input
		{
			get { return tbSaveAs.Text; }
		}
	}
}