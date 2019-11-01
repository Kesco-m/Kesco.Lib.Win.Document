using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Timers;
using System.Web;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document.Blocks.Parsers;
using Kesco.Lib.Win.Document.Items;
using Kesco.Lib.Win.Error;
using Kesco.Lib.Win.Web;

namespace Kesco.Lib.Win.Document.Blocks
{
	public class EmployeeBlock : UserControl
	{
		private SynchronizedCollection<Keys> keyLocker;

		public string label;
		public string singleLabel;
		private Keys lastKeyData = Keys.None;
		private string lastEmployeeText;
		private string paramStr;

		private ButtonSideEnum buttonSide = ButtonSideEnum.Left;

		private Parser parser;

		public event EmployeeBlockEventHandler EmployeeSelected;

		public event EventHandler ButtonTextChanged;
		public bool ClearAfterSelect = true;

		private UpDownTextBox textEmp;
		private Button buttonSelect;
		private ComboBox comboBoxEmp;
		private Container components;

		private EventHandler tEH;

		public EmployeeBlock()
		{
			InitializeComponent();
			label = Environment.StringResources.GetString("Whom");
			singleLabel = Environment.StringResources.GetString("Add");
			keyLocker = new SynchronizedCollection<Keys>();
			parser = new Parser();
			paramStr = "clid=3&UserAccountDisabled=0&return=2";

			tEH = (object sender, EventArgs e) =>
					  {
						  if(EmployerTextChanged != null)
							  EmployerTextChanged(sender, e);
					  };
			textEmp.TextChanged += tEH;
		}

		public EmployeeBlock(string label) : this()
		{
			this.label = label;
		}

		#region Accessors

		public Parser Parser
		{
			set { parser = value; }
		}

		public string Label
		{
			set
			{
				label = value;
				SetLabel();
			}
		}

		public string ParamStr
		{
			get { return paramStr; }
			set { paramStr = value; }
		}

		public ButtonSideEnum ButtonSide
		{
			get { return buttonSide; }
			set
			{
				buttonSide = value;
				if(value == ButtonSideEnum.Left)
				{
					buttonSelect.Location = new Point(0, 0);
					buttonSelect.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					textEmp.Location = new Point(80, 2);
					comboBoxEmp.Location = new Point(80, 1);
				}
				else
				{
					textEmp.Location = new Point(0, 2);
					comboBoxEmp.Location = new Point(0, 1);
					buttonSelect.Location = new Point(textEmp.Width + 8, 0);
					buttonSelect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				}
			}
		}

		[Localizable(true)]
		public string ButtonText
		{
			get { return buttonSelect.Text; }
			set
			{
				buttonSelect.Text = value;
				label = value;
			}
		}

		public string FullText
		{
			get { return textEmp.Text.Trim(); }
			set
			{
				textEmp.TextChanged -= tEH;
				textEmp.Text = (string.IsNullOrEmpty(value) ? "" : value.Trim());
				textEmp.SelectAll();
				textEmp.TextChanged += tEH;
			}
		}

		#endregion

		public event EventHandler EmployerTextChanged;

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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmployeeBlock));
			this.buttonSelect = new System.Windows.Forms.Button();
			this.comboBoxEmp = new System.Windows.Forms.ComboBox();
			this.textEmp = new Kesco.Lib.Win.UpDownTextBox();
			this.SuspendLayout();
			// 
			// buttonSelect
			// 
			this.buttonSelect.BackColor = System.Drawing.SystemColors.Control;
			resources.ApplyResources(this.buttonSelect, "buttonSelect");
			this.buttonSelect.Name = "buttonSelect";
			this.buttonSelect.TabStop = false;
			this.buttonSelect.UseVisualStyleBackColor = false;
			this.buttonSelect.TextChanged += new System.EventHandler(this.buttonSelect_TextChanged);
			this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
			// 
			// comboBoxEmp
			// 
			resources.ApplyResources(this.comboBoxEmp, "comboBoxEmp");
			this.comboBoxEmp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxEmp.Name = "comboBoxEmp";
			this.comboBoxEmp.TabStop = false;
			this.comboBoxEmp.DropDown += new System.EventHandler(this.comboBoxEmp_DropDown);
			this.comboBoxEmp.SelectionChangeCommitted += new System.EventHandler(this.comboBoxEmp_SelectionChangeCommitted);
			this.comboBoxEmp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxEmp_KeyDown);
			this.comboBoxEmp.Leave += new System.EventHandler(this.comboBoxEmp_Leave);
			// 
			// textEmp
			// 
			resources.ApplyResources(this.textEmp, "textEmp");
			this.textEmp.Name = "textEmp";
			this.textEmp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textEmp_KeyDown);
			this.textEmp.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textEmp_KeyUp);
			this.textEmp.Leave += new System.EventHandler(this.textEmp_Leave);
			// 
			// EmployeeBlock
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.textEmp);
			this.Controls.Add(this.buttonSelect);
			this.Controls.Add(this.comboBoxEmp);
			this.Name = "EmployeeBlock";
			resources.ApplyResources(this, "$this");
			this.Load += new System.EventHandler(this.EmployeeBlock_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private void EmployeeBlock_Load(object sender, EventArgs e)
		{
			if(DesignerDetector.IsComponentInDesignMode(this))
				return;

			SetLabel();
			System.Timers.Timer timer = new System.Timers.Timer(1000) { AutoReset = false };
			timer.Elapsed += timer_Elapsed;
			timer.Start();
		}

		private void SetLabel()
		{
			if(ClearAfterSelect)
				buttonSelect.Text = label + "...";
		}

		private void textEmp_KeyUp(object sender, KeyEventArgs e)
		{
			if(!keyLocker.Contains(e.KeyData))
			{
				keyLocker.Add(e.KeyData);
				try
				{
					lastKeyData = e.KeyData;
					if(lastEmployeeText != textEmp.Text &&
						!(textEmp.Text.Length == 1 && Regex.IsMatch(textEmp.Text, "[A-Za-z]")))
					{
						buttonSelect.Text = label + "...";
						if((lastKeyData != Keys.Delete) && (lastKeyData != Keys.Back) && lastKeyData != Keys.Down &&
							lastKeyData != Keys.Up)
							ParseEmployee(false);
					}
					else if((textEmp.Text.Length == 1 && Regex.IsMatch(textEmp.Text, "[A-Za-z]")) ||
							 (textEmp.Text.Length == 0))
					{
						buttonSelect.Text = label + "...";
					}
					lastEmployeeText = textEmp.Text;
				}
				catch(Exception ex)
				{
					ErrorShower.OnShowError(null, ex.Message, "");
				}
				finally
				{
					keyLocker.Remove(e.KeyData);
				}
			}
		}

		private void buttonSelect_Click(object sender, EventArgs e)
		{
			SelectEmployee();
		}

		private void SelectEmployee()
		{
			var dialog = new UserDialog(Environment.EmployeeSearchString, "search=" + HttpUtility.UrlEncode(textEmp.Text).Replace("+", "%20") + "&" + paramStr);

			dialog.DialogEvent += UserDialog_DialogEvent;
			dialog.Show();
			var findForm = FindForm();
			if(findForm != null)
				findForm.Enabled = false;
		}

		private void UserDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Form form = FindForm();
			if(form != null)
			{
				form.Enabled = true;
				form.Focus();

				var dialog = e.Dialog as UserDialog;
				if(dialog != null && dialog.DialogResult == DialogResult.OK && dialog.Users != null)
				{
					var emps = new Employee[dialog.Users.Count];
					for(int i = 0; i < dialog.Users.Count; i++)
					{
						var newUser = (UserInfo)dialog.Users[i];
						emps[i] = new Employee(newUser.ID, Environment.EmpData);
					}

					ThrowEvent(emps);
				}

				Clear();
				SetLabel();
			}
		}

		private void ParseEmployee(bool skip)
		{
			string txt = textEmp.Text.Trim();
			Employee[] emps = parser.Parse(ref txt);
			if((emps.Length == 1) && (textEmp.Text != txt))
			{
				textEmp.Text = txt;
				textEmp.EnableForInput();
				textEmp.SelectAll();
			}
			else
				for(int i = 0; i < emps.Length; i++)
					textEmp.Text =
						textEmp.Text.Replace(
							(Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
								 ? emps[i].ShortName
								 : emps[i].ShortEngName), "").Trim(new[] { ' ', ',', ';' });

			if(!skip)
				comboBoxEmp.DroppedDown = false;
			if(parser.CandidateCount == 0)
				buttonSelect.Text = label + "...";
			else if(parser.CandidateCount == 1)
				buttonSelect.Text = singleLabel;
			else
			{
				buttonSelect.Text = label + "..." + parser.CandidateCount;
				if(parser.CandidateCount < 9)
				{
					ShowEmployees(parser.CandidateEmployees);
					var findForm = FindForm();
					if(findForm != null)
						findForm.Cursor = Cursors.Default;
				}
				else
					ShowEmployees(null);
			}

			if(parser.SplitToBits(textEmp.Text.Trim()).Length > 1)
				textEmp.SelectAll();

			ThrowEvent(emps);
		}

		private void ShowEmployees(DataRow[] drs)
		{
			comboBoxEmp.Items.Clear();
			var findForm = comboBoxEmp.FindForm();
			if(findForm != null)
				findForm.Cursor = Cursors.Default;
			if(drs != null)
			{
				for(int i = 0; i < drs.Length; i++)
				{
					comboBoxEmp.Items.Add(new ListItem((int)drs[i][Environment.EmpData.IDField],
											(Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
												 ? drs[i][Environment.EmpData.NameField].ToString()
												 : drs[i][Environment.EmpData.EmloyeeField].ToString())));
				}
				comboBoxEmp.DroppedDown = true;
				if(findForm != null)
					findForm.Cursor = Cursors.Default;
			}
		}

		private void ThrowEvent(Employee[] emps)
		{
			if((emps != null) && (emps.Length > 0) && (EmployeeSelected != null))
				EmployeeSelected(this, new EmployeeBlockEventArgs(emps));
		}

		public void Clear()
		{
			if(ClearAfterSelect)
				textEmp.Clear();
			lastEmployeeText = "";
		}

		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if(!keyLocker.Contains(Keys.Enter))
			{
				keyLocker.Add(Keys.Enter);
				try
				{
					textEmp.KeyUp -= textEmp_KeyUp;
					textEmp.KeyUp += textEmp_KeyUp;
					keyLocker.Remove(Keys.Enter);
					System.Timers.Timer timer = sender as System.Timers.Timer;
					if(timer != null)
					{
						timer.Elapsed -= timer_Elapsed;
						timer.Stop();
						timer.Dispose();
						timer = null;
					}
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
				}
				finally
				{
					keyLocker.Remove(Keys.Enter);
				}
			}
		}

		private void textEmp_Leave(object sender, EventArgs e)
		{
			comboBoxEmp.DroppedDown = false;
		}

		private void textEmp_KeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
				case Keys.Enter:
					SelectEmployee();
					break;
				case Keys.Up:
					if(comboBoxEmp.DroppedDown)
					{
						textEmp.Leave -= textEmp_Leave;
						comboBoxEmp.Select();
						comboBoxEmp.SelectedItem = comboBoxEmp.Items[comboBoxEmp.Items.Count - 1];
						comboBoxEmp.FindForm().Cursor = Cursors.Default;
						textEmp.Leave += textEmp_Leave;
					}
					break;
				case Keys.Down:
					if(comboBoxEmp.DroppedDown)
					{
						textEmp.Leave -= textEmp_Leave;
						comboBoxEmp.Select();
						comboBoxEmp.SelectedItem = comboBoxEmp.Items[0];
						comboBoxEmp.FindForm().Cursor = Cursors.Default;
						textEmp.Leave += textEmp_Leave;
					}
					break;
			}
		}

		private void comboBoxEmp_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyData != Keys.Enter && e.KeyData != Keys.Up && e.KeyData != Keys.Down && e.KeyData != Keys.PageUp &&
				e.KeyData != Keys.PageDown)
				textEmp.Select();
		}

		private void comboBoxEmp_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if(comboBoxEmp.SelectedItem != null)
			{
				var item = comboBoxEmp.SelectedItem as ListItem;
				if(item != null && item.ID > 0)
				{
					var emps = new Employee[1];
					emps[0] = new Employee(item.ID, Environment.EmpData);
					ThrowEvent(emps);
					Clear();
					SetLabel();
				}
				textEmp.Select();

			}
		}

		private void comboBoxEmp_Leave(object sender, EventArgs e)
		{
			comboBoxEmp.DroppedDown = false;
		}

		private void comboBoxEmp_DropDown(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.Default;
		}

		private void buttonSelect_TextChanged(object sender, EventArgs e)
		{
			if(ButtonTextChanged != null)
				ButtonTextChanged(sender, e);
		}

		/// <summary>
		/// перечислитель положения кнопки 
		/// </summary>
		public enum ButtonSideEnum
		{
			Left,
			Right
		};
	}

	/// <summary>
	/// Событие блока (аргументы?)
	/// </summary>
	public class EmployeeBlockEventArgs : EventArgs
	{
		public EmployeeBlockEventArgs(Employee[] emps)
		{
			Emps = emps;
		}

		public Employee[] Emps { get; private set; }
	}

	/// <summary>
	/// Делегат для обработки события
	/// </summary>
	public delegate void EmployeeBlockEventHandler(object source, EmployeeBlockEventArgs e);
}