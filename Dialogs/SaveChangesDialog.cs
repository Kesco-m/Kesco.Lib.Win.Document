using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
	public partial class SaveChangesDialog : Lib.Win.FreeDialog
	{
		private string _docstring = "";
		private bool _readOnly;
		private Document.Environment.ActionBefore _act;
		private int _docId;
		private Lib.Win.Options.Folder _options;

		public DelPartDialogResult Result { get; private set; }

		public SaveChangesDialog(string docstring, bool readOnly, Document.Environment.ActionBefore act, int docId, Options.Folder opts)
		{
			InitializeComponent();

			this._docstring = docstring;
			this._readOnly = readOnly;
			this._act = act;
			this._docId = docId;
			this._options = opts;
		}


		private void SaveChangesDialog_Load(object sender, EventArgs e)
		{
			string opt;
			opt = _options.LoadStringOption("ChangeOrig", radioButSave.Checked.ToString());
			radioButSave.Checked = System.Convert.ToBoolean(opt);
			radioButDontSave.Checked = !radioButSave.Checked;
			radioButActOrig.Enabled = radioButActCopy.Enabled = radioButDontSave.Checked;

			opt = _options.LoadStringOption("SaveOrig", radioButActOrig.Checked.ToString());
			radioButActOrig.Checked = System.Convert.ToBoolean(opt);
			radioButActCopy.Checked = !radioButActOrig.Checked;
			chBoxOpenCopy.Enabled = radioButActCopy.Checked;

			if(!_readOnly)
			{
				readOnlyWarnlabel.Visible = false;

				int delta = panel1.Location.Y - readOnlyWarnlabel.Location.Y;
				panel1.Location = new Point(panel1.Location.X, panel1.Location.Y - delta);
				panel2.Location = new Point(panel2.Location.X, panel2.Location.Y - delta);
				chBoxOpenCopy.Location = new Point(chBoxOpenCopy.Location.X, chBoxOpenCopy.Location.Y - delta);
			}
			else
			{
				radioButSave.Checked = radioButSave.Enabled = false;
				radioButDontSave.Checked = true;
				radioButActOrig.Enabled = radioButActCopy.Enabled = radioButDontSave.Checked;
				readOnlyWarnlabel.Text = Environment.StringResources.GetString("DocControl_ReadOnlyFile");
			}

			if(_act == Environment.ActionBefore.None || _act == Environment.ActionBefore.LeaveFile || _act == Environment.ActionBefore.DelPart || _act == Environment.ActionBefore.DelPartAfterSave)
			{
				panel2.Visible = false;

				int delta = chBoxOpenCopy.Location.Y - panel2.Location.Y;
				chBoxOpenCopy.Location = new Point(chBoxOpenCopy.Location.X, chBoxOpenCopy.Location.Y - delta);
				chBoxOpenCopy.Enabled = radioButDontSave.Checked;
			}
			//else if (_act == Environment.ActionBefore.Save)
			//    chBoxOpenCopy.Visible = false;

			opt = _options.LoadStringOption("OpenCopy", chBoxOpenCopy.Checked.ToString());
			chBoxOpenCopy.Checked = System.Convert.ToBoolean(opt) && chBoxOpenCopy.Visible && chBoxOpenCopy.Enabled &&
									(panel2.Visible && radioButActCopy.Checked || !panel2.Visible && radioButDontSave.Checked);

			string file = (_docId > 0 ? Environment.StringResources.GetString("Document") : Environment.StringResources.GetString("File")).ToLower();
			radioButDontSave.Text += file;

			this.Text = StringResources.DocControl_image_NeedSave_Message1;
			switch(_act)
			{
				case Environment.ActionBefore.None:
				case Environment.ActionBefore.LeaveFile:
					radioButSave.Text = StringResources.SaveChangesDialog_Change + StringResources.SaveChangesDialog_Source + file;
					break;
				case Environment.ActionBefore.Save:
				case Environment.ActionBefore.SavePart:
					radioButSave.Text = StringResources.SaveChangesDialog_SaveToArchive + StringResources.SaveChangesDialog_Changed + StringResources.SaveChangesDialog_Source + file;
					radioButActOrig.Text = StringResources.SaveChangesDialog_SaveToArchive + StringResources.SaveChangesDialog_Source + file;
					radioButActCopy.Text = StringResources.SaveChangesDialog_SaveToArchive + StringResources.SaveChangesDialog_ChangesCopy;
					break;
				case Environment.ActionBefore.Print:
					radioButSave.Text = StringResources.SaveChangesDialog_Print + StringResources.SaveChangesDialog_Changed + StringResources.SaveChangesDialog_Source + file;
					radioButActOrig.Text = StringResources.SaveChangesDialog_Print + StringResources.SaveChangesDialog_Source + file;
					radioButActCopy.Text = StringResources.SaveChangesDialog_Print + StringResources.SaveChangesDialog_ChangesCopy;
					break;
				case Environment.ActionBefore.SendFaxAndMail:
					radioButSave.Text = StringResources.SaveChangesDialog_SendMail + StringResources.SaveChangesDialog_Changed + StringResources.SaveChangesDialog_Source + file;
					radioButActOrig.Text = StringResources.SaveChangesDialog_SendMail + StringResources.SaveChangesDialog_Source + file;
					radioButActCopy.Text = StringResources.SaveChangesDialog_SendMail + StringResources.SaveChangesDialog_ChangesCopy;
					break;
				case Environment.ActionBefore.DelPart:
					radioButSave.Text = StringResources.SaveChangesDialog_DeleteInSource + (_docId > 0 ? StringResources.SaveChangesDialog_Doc1 : StringResources.SaveChangesDialog_File1);
					string[] ss = StringResources.DocControl_DelPartQuery.Split(' ');
					this.Text = ss[0] + " " + ss[1];
					break;
				case Environment.ActionBefore.DelPartAfterSave:
					radioButSave.Text = StringResources.SaveChangesDialog_DeleteInSource + (_docId > 0 ? StringResources.SaveChangesDialog_Doc1 : StringResources.SaveChangesDialog_File1);
					radioButActOrig.Text = StringResources.SaveChangesDialog_SaveToArchive + StringResources.SaveChangesDialog_Source + file;
					radioButActCopy.Text = StringResources.SaveChangesDialog_SaveToArchive + StringResources.SaveChangesDialog_ChangesCopy;
					this.Text = StringResources.DocControl_DelPartQuery_1.Substring(0, StringResources.DocControl_DelPartQuery_1.Length - 5) + "?";
					break;
			}

			this.radioButDontSave.CheckedChanged += new System.EventHandler(this.radioButDontSave_CheckedChanged);
			this.radioButActCopy.CheckedChanged += new System.EventHandler(this.radioButActCopy_CheckedChanged);
		}
		private void radioButDontSave_CheckedChanged(object sender, EventArgs e)
		{
			radioButActOrig.Enabled = radioButActCopy.Enabled = radioButDontSave.Checked;
			radioButActOrig.Checked = true;
			if(!panel2.Visible)
			{
				chBoxOpenCopy.Enabled = radioButDontSave.Checked;
				chBoxOpenCopy.Checked = false;
			}
		}

		private void radioButActCopy_CheckedChanged(object sender, EventArgs e)
		{
			chBoxOpenCopy.Enabled = radioButActCopy.Checked;
			chBoxOpenCopy.Checked = false;
		}

		private void SaveChangesDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			//if(e.CloseReason != CloseReason.UserClosing)
			//    return;
			if(DialogResult == DialogResult.Yes)
				SaveOptions();

			if(radioButSave.Checked)
				Result = DelPartDialogResult.Yes;
			else //if(radioButDontSave.Checked)
			{
				Result = DelPartDialogResult.No;
				if(radioButActCopy.Visible && radioButActCopy.Checked)
					Result = DelPartDialogResult.CreateCopy;
				if(chBoxOpenCopy.Visible && chBoxOpenCopy.Checked)
					Result = DelPartDialogResult.ShowCopy;
			}
		}

		public void SaveOptions()
		{
			_options.Option("ChangeOrig").Value = radioButSave.Checked.ToString();
			_options.Option("SaveOrig").Value = radioButActOrig.Checked.ToString();
			_options.Option("OpenCopy").Value = chBoxOpenCopy.Checked.ToString();

			_options.Save();
		}
	}

	public enum DelPartDialogResult
	{
		None,
		Yes,
		No,
		CreateCopy,
		ShowCopy
	}
}