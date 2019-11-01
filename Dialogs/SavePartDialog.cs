using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
	public class SavePartDialog : FreeDialog
	{
		private readonly int numPages;

		private Button buttonOK;
		private Button buttonCancel;
		private GroupBox groupBox1;
		private Label label1;
		private Label label2;
		private NumericUpDown numericFirst;
		private NumericUpDown numericLast;

		private Container components;

		public SavePartDialog(string fileName, int page, int pageCount)
			: this(fileName, false, DateTime.MinValue, null, page, 0, pageCount)
		{
			numericFirst.Value = page;
			Text = Environment.StringResources.GetString("Dialog_SavePartDialog_Title1");
		}

		public SavePartDialog(string fileName, bool messageNeeded, DateTime scanDate, string docStr, int page,
							  int imageID, int pageCount)
		{
			InitializeComponent();

			FileName = fileName;
			MessageNeeded = messageNeeded;
			ScanDate = scanDate;
			DocStr = docStr;
			ImageID = imageID;

			numPages = pageCount;

			numericFirst.Minimum = 1;
			numericFirst.Maximum = numPages;

			numericFirst.Value = page < numPages ? 1 : page;

			numericLast.Minimum = 1;
			numericLast.Maximum = numPages;
			numericLast.Value = page;
		}

		#region Accessors

		public int MinPage
		{
			get { return Convert.ToInt32(numericFirst.Value); }
		}

		public int MaxPage
		{
			get { return Convert.ToInt32(numericLast.Value); }
		}

		public string FileName { get; private set; }
		public bool MessageNeeded { get; private set; }
		public DateTime ScanDate { get; private set; }
		public string DocStr { get; private set; }
		public int ImageID { get; private set; }
		public bool Delete { get; set; }
		public bool IsPdf { get; set; }

		#endregion

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SavePartDialog));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.numericFirst = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numericLast = new System.Windows.Forms.NumericUpDown();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericFirst)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLast)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.numericFirst);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.numericLast);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// numericFirst
			// 
			resources.ApplyResources(this.numericFirst, "numericFirst");
			this.numericFirst.Name = "numericFirst";
			this.numericFirst.TextChanged += new System.EventHandler(this.numericFirst_TextChanged);
			this.numericFirst.ValueChanged += new System.EventHandler(this.numericFirst_ValueChanged);
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// numericLast
			// 
			resources.ApplyResources(this.numericLast, "numericLast");
			this.numericLast.Name = "numericLast";
			this.numericLast.TextChanged += new System.EventHandler(this.numericLast_TextChanged);
			this.numericLast.ValueChanged += new System.EventHandler(this.numericLast_ValueChanged);
			// 
			// SavePartDialog
			// 
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SavePartDialog";
			this.Closed += new System.EventHandler(this.dialog_Closed);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericFirst)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericLast)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private void UpdateControls()
		{
			buttonOK.Enabled = ((numericFirst.Value > 1) || (numericLast.Value < numPages)) &&
							   (numericFirst.Value <= numericLast.Value);
		}

		private void numericFirst_ValueChanged(object sender, EventArgs e)
		{
			UpdateControls();
		}

		private void numericLast_ValueChanged(object sender, EventArgs e)
		{
			UpdateControls();
		}

		private void numericFirst_TextChanged(object sender, EventArgs e)
		{
			UpdateControls();
		}

		private void numericLast_TextChanged(object sender, EventArgs e)
		{
			UpdateControls();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			End(DialogResult.OK);
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			End(DialogResult.Cancel);
		}

		private void dialog_Closed(object sender, EventArgs e)
		{
			Document.Controls.DocControl.RemoveSavePartDialog(FileName);
		}
	}
}