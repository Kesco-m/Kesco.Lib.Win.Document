using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Select
{
	public partial class SelectSignEmployeeDialog : Form
	{
		private Data.Temp.Objects.SignType signType;

		public Data.Temp.Objects.Employee Employee4 { get; private set; }

		public bool SendMessage
		{
			get
			{
				var ckb = tableLayoutPanel.GetControlFromPosition(0, tableLayoutPanel.RowCount - 1) as CheckBox;
				if(ckb != null)
					return ckb.Checked;
				return false;
			}
		}

		public SelectSignEmployeeDialog(List<Data.Temp.Objects.Employee> replacementEmployees, Data.Temp.Objects.SignType sign)
		{
			InitializeComponent();
			signType = sign;

			tableLayoutPanel.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
			tableLayoutPanel.RowCount = replacementEmployees.Count + 2 + (sign == Data.Temp.Objects.SignType.firstSign ? 0 : 1);

			int startButtonIdx = 0;
			if(sign != Data.Temp.Objects.SignType.firstSign)
			{
				Width += 120;
				AddSignLabel(signType);
				startButtonIdx = 1;
			}
			else
			{
				pictureBox.Hide();
				Controls.Remove(pictureBox);
			}

			if(Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru"))
				replacementEmployees = replacementEmployees.OrderBy(x => x.ShortName).ToList();
			else
				replacementEmployees = replacementEmployees.OrderBy(x => x.ShortEngName).ToList();


			AddButton(startButtonIdx, Environment.CurEmp);
			bool fa = false;
			for(int idx = 0; idx < replacementEmployees.Count; idx++)
			{
				if(replacementEmployees[idx].ID != Environment.CurEmp.ID)
					AddButton(idx + startButtonIdx + (fa ? 0 : 1), replacementEmployees[idx]);
				else
					fa = true;
			}

			AddCancelButton(startButtonIdx + replacementEmployees.Count);
			AddCheckBox(startButtonIdx + replacementEmployees.Count + 1);

			ClientSize = new Size(tableLayoutPanel.Width, tableLayoutPanel.Height + 10);
			MinimumSize = Size;
		}

		private void AddButton(int row, Data.Temp.Objects.Employee employee)
		{
			string employeeName = Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru") ? employee.ShortName : employee.ShortEngName;

			Button btn = new Button();
			btn.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			btn.BackColor = SystemColors.Control;
			btn.DialogResult = DialogResult.OK;
			btn.Margin = new Padding(0);
			btn.Tag = employee;
			btn.Click += btn_Click;

			string signText = Environment.StringResources.GetString("msgSign");
			string asText = Environment.StringResources.GetString("msgAs");
			btn.Text = signText + (Environment.CurEmp.Equals(employee) ? "" : string.Format(" {0} [{1}]", asText, employeeName));

			tableLayoutPanel.Controls.Add(btn, 0, row);
		}

		private void btn_Click(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			Employee4 = (Data.Temp.Objects.Employee)btn.Tag;

			if(signType == Data.Temp.Objects.SignType.firstSign)
				return;
			CheckBox ckb = tableLayoutPanel.GetControlFromPosition(0, tableLayoutPanel.RowCount - 1) as CheckBox;
			if(ckb != null)
			if(Environment.UserSettings.MessageOnEndSign != ckb.Checked)
			{
				Environment.UserSettings.MessageOnEndSign = ckb.Checked;
				Environment.UserSettings.Save();
			}
		}

		private void AddSignLabel(Data.Temp.Objects.SignType signtype)
		{
			TableLayoutPanel tlp = new TableLayoutPanel();
			tlp.AutoSize = true;
			tlp.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			tlp.ColumnCount = 2;
			tlp.Dock = DockStyle.Fill;
			tlp.Margin = new Padding(0);
			tlp.RowCount = 1;
			tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			tlp.Controls.Add(pictureBox, 0, 0);

			Controls.Remove(pictureBox);

			Label label = new Label();
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			label.AutoSize = false;
			label.Margin = new Padding(0);
			switch(signType)
			{
				case Data.Temp.Objects.SignType.finalSign:
					label.Text = Environment.StringResources.GetString("msgFinishSign");
					break;
				case Data.Temp.Objects.SignType.cancelSign:
					label.Text = Environment.StringResources.GetString("msgCancelSign");
					break;
			}

			tableLayoutPanel.Controls.Add(tlp, 0, 0);
			tlp.Controls.Add(label, 1, 0);

			// Следующий код проверяет, поместился ли тект в label'е, и, если нет, увеличивает label'у высоту
			using(Graphics g = Graphics.FromHwnd(label.Handle))
			{
				SizeF size = g.MeasureString(label.Text, label.Font, label.Width);
				if(size.Height > label.Height)
					label.Height = (int)size.Height + 1;
			}
		}

		private void AddCancelButton(int row)
		{
			Button cancel = new Button();
			cancel.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			cancel.BackColor = SystemColors.Control;
			cancel.DialogResult = DialogResult.Cancel;
			cancel.Margin = new Padding(0);
			cancel.Text = Environment.StringResources.GetString("Cancel");

			tableLayoutPanel.Controls.Add(cancel, 0, row);
			this.CancelButton = cancel;
		}

		private void AddCheckBox(int row)
		{
			CheckBox sendMessage = new CheckBox();
			sendMessage.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			sendMessage.Checked = Environment.UserSettings.MessageOnEndSign;

			switch(signType)
			{
				case Data.Temp.Objects.SignType.firstSign:
					sendMessage.Text = Environment.StringResources.GetString("msgSend");
					break;
				case Data.Temp.Objects.SignType.finalSign:
					sendMessage.Text = Environment.StringResources.GetString("msgSendFinishMsg");
					break;
				case Data.Temp.Objects.SignType.cancelSign:
					sendMessage.Text = Environment.StringResources.GetString("msgSendCancelMsg");
					break;
			}
			tableLayoutPanel.Controls.Add(sendMessage, 0, row);
		}
	}
}