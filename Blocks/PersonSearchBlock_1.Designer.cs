namespace Kesco.Lib.Win.Document.Blocks
{
	partial class PersonSearchBlock_1
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
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonSearchBlock_1));
			this.buttonFindAdd = new System.Windows.Forms.Button();
			this.comboBoxPerson = new System.Windows.Forms.ComboBox();
			this.textBoxPerson = new Kesco.Lib.Win.UpDownTextBox();
			this.SuspendLayout();
			// 
			// buttonFindAdd
			// 
			resources.ApplyResources(this.buttonFindAdd, "buttonFindAdd");
			this.buttonFindAdd.Name = "buttonFindAdd";
			this.buttonFindAdd.Click += new System.EventHandler(this.buttonSelect_Click);
			// 
			// comboBoxPerson
			// 
			resources.ApplyResources(this.comboBoxPerson, "comboBoxPerson");
			this.comboBoxPerson.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPerson.Name = "comboBoxPerson";
			this.comboBoxPerson.TabStop = false;
			this.comboBoxPerson.DropDown += new System.EventHandler(this.comboPerson_DropDown);
			this.comboBoxPerson.SelectionChangeCommitted += new System.EventHandler(this.comboPerson_SelectionChangeCommitted);
			this.comboBoxPerson.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboPerson_KeyDown);
			this.comboBoxPerson.Leave += new System.EventHandler(this.comboPerson_Leave);
			// 
			// textBoxPerson
			// 
			resources.ApplyResources(this.textBoxPerson, "textBoxPerson");
			this.textBoxPerson.Name = "textBoxPerson";
			this.textBoxPerson.TextChanged += new System.EventHandler(this.textBoxPerson_TextChanged);
			this.textBoxPerson.KeyDown += new System.Windows.Forms.KeyEventHandler(this.person_KeyDown);
			this.textBoxPerson.KeyUp += new System.Windows.Forms.KeyEventHandler(this.person_KeyUp);
			this.textBoxPerson.Leave += new System.EventHandler(this.person_Leave);
			// 
			// PersonSearchBlock_1
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBoxPerson);
			this.Controls.Add(this.buttonFindAdd);
			this.Controls.Add(this.comboBoxPerson);
			this.Name = "PersonSearchBlock_1";
			this.Load += new System.EventHandler(this.PersonBlock_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonFindAdd;
		private System.Windows.Forms.ComboBox comboBoxPerson;
		private Kesco.Lib.Win.UpDownTextBox textBoxPerson;
	}
}
