using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
	public class DateInterval : Base
	{
		private Label label1;
		private RadioButton rb3;
		private RadioButton rb0;
		private Label label4;
		public DateTimePicker tbMin3;
		public DateTimePicker tbMax3;
		public DateTimePicker tbMin0;
		private RadioButton rb1;
		private RadioButton rb2;
		public DateTimePicker tbMax1;
		public DateTimePicker tbMin2;
		private CheckBox checkBoxTodayMore;
		private CheckBox checkBoxTodayLess;
		private CheckBox checkBoxTodayEqual;
		private RadioButton radioButtonNone;

		private Container components;

		public DateInterval()
		{
			InitializeComponent();
		}

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DateInterval));
			this.tbMin3 = new System.Windows.Forms.DateTimePicker();
			this.tbMax3 = new System.Windows.Forms.DateTimePicker();
			this.label1 = new System.Windows.Forms.Label();
			this.rb3 = new System.Windows.Forms.RadioButton();
			this.rb1 = new System.Windows.Forms.RadioButton();
			this.rb2 = new System.Windows.Forms.RadioButton();
			this.rb0 = new System.Windows.Forms.RadioButton();
			this.tbMin0 = new System.Windows.Forms.DateTimePicker();
			this.tbMax1 = new System.Windows.Forms.DateTimePicker();
			this.tbMin2 = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.checkBoxTodayMore = new System.Windows.Forms.CheckBox();
			this.checkBoxTodayLess = new System.Windows.Forms.CheckBox();
			this.checkBoxTodayEqual = new System.Windows.Forms.CheckBox();
			this.radioButtonNone = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// tbMin3
			// 
			resources.ApplyResources(this.tbMin3, "tbMin3");
			this.tbMin3.Name = "tbMin3";
			// 
			// tbMax3
			// 
			resources.ApplyResources(this.tbMax3, "tbMax3");
			this.tbMax3.Name = "tbMax3";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// rb3
			// 
			resources.ApplyResources(this.rb3, "rb3");
			this.rb3.Name = "rb3";
			this.rb3.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// rb1
			// 
			resources.ApplyResources(this.rb1, "rb1");
			this.rb1.Name = "rb1";
			this.rb1.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// rb2
			// 
			resources.ApplyResources(this.rb2, "rb2");
			this.rb2.Name = "rb2";
			this.rb2.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// rb0
			// 
			resources.ApplyResources(this.rb0, "rb0");
			this.rb0.Name = "rb0";
			this.rb0.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// tbMin0
			// 
			resources.ApplyResources(this.tbMin0, "tbMin0");
			this.tbMin0.Name = "tbMin0";
			// 
			// tbMax1
			// 
			resources.ApplyResources(this.tbMax1, "tbMax1");
			this.tbMax1.Name = "tbMax1";
			// 
			// tbMin2
			// 
			resources.ApplyResources(this.tbMin2, "tbMin2");
			this.tbMin2.Name = "tbMin2";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// checkBoxTodayMore
			// 
			resources.ApplyResources(this.checkBoxTodayMore, "checkBoxTodayMore");
			this.checkBoxTodayMore.Name = "checkBoxTodayMore";
			this.checkBoxTodayMore.UseVisualStyleBackColor = true;
			this.checkBoxTodayMore.CheckedChanged += new System.EventHandler(this.checkBoxToday_CheckedChanged);
			// 
			// checkBoxTodayLess
			// 
			resources.ApplyResources(this.checkBoxTodayLess, "checkBoxTodayLess");
			this.checkBoxTodayLess.Name = "checkBoxTodayLess";
			this.checkBoxTodayLess.UseVisualStyleBackColor = true;
			this.checkBoxTodayLess.CheckedChanged += new System.EventHandler(this.checkBoxToday_CheckedChanged);
			// 
			// checkBoxTodayEqual
			// 
			resources.ApplyResources(this.checkBoxTodayEqual, "checkBoxTodayEqual");
			this.checkBoxTodayEqual.Name = "checkBoxTodayEqual";
			this.checkBoxTodayEqual.UseVisualStyleBackColor = true;
			this.checkBoxTodayEqual.CheckedChanged += new System.EventHandler(this.checkBoxToday_CheckedChanged);
			// 
			// radioButtonNone
			// 
			resources.ApplyResources(this.radioButtonNone, "radioButtonNone");
			this.radioButtonNone.Name = "radioButtonNone";
			this.radioButtonNone.TabStop = true;
			this.radioButtonNone.UseVisualStyleBackColor = true;
			this.radioButtonNone.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// DateInterval
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.radioButtonNone);
			this.Controls.Add(this.checkBoxTodayEqual);
			this.Controls.Add(this.checkBoxTodayLess);
			this.Controls.Add(this.checkBoxTodayMore);
			this.Controls.Add(this.tbMin3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbMin2);
			this.Controls.Add(this.tbMax1);
			this.Controls.Add(this.tbMin0);
			this.Controls.Add(this.rb0);
			this.Controls.Add(this.rb2);
			this.Controls.Add(this.rb1);
			this.Controls.Add(this.rb3);
			this.Controls.Add(this.tbMax3);
			this.Name = "DateInterval";
			this.Controls.SetChildIndex(this.tbMax3, 0);
			this.Controls.SetChildIndex(this.rb3, 0);
			this.Controls.SetChildIndex(this.rb1, 0);
			this.Controls.SetChildIndex(this.rb2, 0);
			this.Controls.SetChildIndex(this.rb0, 0);
			this.Controls.SetChildIndex(this.tbMin0, 0);
			this.Controls.SetChildIndex(this.tbMax1, 0);
			this.Controls.SetChildIndex(this.tbMin2, 0);
			this.Controls.SetChildIndex(this.label4, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.tbMin3, 0);
			this.Controls.SetChildIndex(this.checkBoxTodayMore, 0);
			this.Controls.SetChildIndex(this.checkBoxTodayLess, 0);
			this.Controls.SetChildIndex(this.checkBoxTodayEqual, 0);
			this.Controls.SetChildIndex(this.radioButtonNone, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected override void FillForm()
		{
			radioButtonNone.Visible = option.HasEmpty();

			bool today = false;
			string min = elOption.GetAttribute("min");
			string max = elOption.GetAttribute("max");
			if(min.Length > 0)
			{
				if(min.Equals("Today"))
				{
					tbMin3.Value =
						tbMin2.Value =
						tbMin0.Value =
						DateTime.Today;
					today = true;
				}
				else
					tbMin3.Value =
						tbMin2.Value =
						tbMin0.Value =
						DateTime.Parse(min);
			}
			if(max.Length > 0)
				if(max.Equals("Today"))
				{
					tbMax3.Value =
						tbMax1.Value =
						DateTime.Today;
					today = true;
				}
				else
					tbMax3.Value =
						tbMax1.Value =
						DateTime.Parse(max);

			checkBoxTodayMore.Checked = checkBoxTodayLess.Checked = checkBoxTodayEqual.Checked = today;

			int flags = (min.Length > 0 ? 0x0001 : 0) +
						(max.Length > 0 ? 0x0010 : 0) +
						(min.Equals(max) ? 0x0100 : 0);
			switch(flags)
			{
				case 0x0001:
					rb2.Checked = true;
					break;
				case 0x0010:
					rb1.Checked = true;
					break;
				case 0x0011:
					rb3.Checked = true;
					break;
				case 0x0111:

					break;
				default:
					if(option.HasEmpty())
						radioButtonNone.Checked = true;
					else
						rb0.Checked = true;
					break;
			}
		}

		protected override void FillElement()
		{
			if(rb0.Checked)
			{
				if(!checkBoxTodayEqual.Checked)
				{
					elOption.SetAttribute("min", tbMin0.Value.ToString("dd.MM.yyyy"));
					elOption.SetAttribute("max", tbMin0.Value.ToString("dd.MM.yyyy"));
				}
				else
				{
					elOption.SetAttribute("min", ("Today"));
					elOption.SetAttribute("max", ("Today"));
				}
			}
			if(rb1.Checked)
			{
				elOption.RemoveAttribute("min");
				elOption.SetAttribute("max",
									  !checkBoxTodayLess.Checked ? tbMax1.Value.ToString("dd.MM.yyyy") : ("Today"));
			}
			if(rb2.Checked)
			{
				elOption.RemoveAttribute("max");
				elOption.SetAttribute("min",
									  !checkBoxTodayMore.Checked ? tbMin2.Value.ToString("dd.MM.yyyy") : ("Today"));
			}
			if(rb3.Checked)
			{
				elOption.SetAttribute("min", tbMin3.Value.ToString("dd.MM.yyyy"));
				elOption.SetAttribute("max", tbMax3.Value.ToString("dd.MM.yyyy"));
			}

			if(radioButtonNone.Checked)
			{
				elOption.SetAttribute("min", "");
				elOption.SetAttribute("max", "");
			}
		}

		private void rb_CheckedChanged(object sender, EventArgs e)
		{
			tbMin0.Enabled = rb0.Checked && !checkBoxTodayEqual.Checked;
			checkBoxTodayEqual.Enabled = rb0.Checked;
			tbMax1.Enabled = rb1.Checked && !checkBoxTodayLess.Checked;
			checkBoxTodayLess.Enabled = rb1.Checked;
			tbMin2.Enabled = rb2.Checked && !checkBoxTodayMore.Checked;
			checkBoxTodayMore.Enabled = rb2.Checked;
			tbMax3.Enabled = tbMin3.Enabled = rb3.Checked;
		}

		private void checkBoxToday_CheckedChanged(object sender, EventArgs e)
		{
			bool today = (sender as CheckBox).Checked;
			checkBoxTodayEqual.Checked = checkBoxTodayLess.Checked = checkBoxTodayMore.Checked = today;
			rb_CheckedChanged(sender, e);
		}
	}
}