namespace Kesco.Lib.Win.Document.Controls
{
	partial class ContactControl
	{
		/// <summary> 
		/// Требуется переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором компонентов

		/// <summary> 
		/// Обязательный метод для поддержки конструктора - не изменяйте 
		/// содержимое данного метода при помощи редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.richTextBox1 = new Kesco.Lib.Win.Document.Controls.RichTextBoxContact();
			this.comboBoxContact = new System.Windows.Forms.ComboBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.AutoWordSelection = true;
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox1.Location = new System.Drawing.Point(0, 0);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.PlainTextMode = true;
			this.richTextBox1.SearchType = true;
			this.richTextBox1.Size = new System.Drawing.Size(234, 24);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			this.richTextBox1.MouseHover += new System.EventHandler(this.richTextBox1_MouseHover);
			this.richTextBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.richTextBox1_MouseMove);
			// 
			// comboBoxContact
			// 
			this.comboBoxContact.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comboBoxContact.FormattingEnabled = true;
			this.comboBoxContact.Location = new System.Drawing.Point(0, 0);
			this.comboBoxContact.Name = "comboBoxContact";
			this.comboBoxContact.Size = new System.Drawing.Size(234, 21);
			this.comboBoxContact.TabIndex = 1;
			// 
			// ContactControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.comboBoxContact);
			this.Name = "ContactControl";
			this.Size = new System.Drawing.Size(234, 24);
			this.ResumeLayout(false);

		}

		#endregion

		private RichTextBoxContact richTextBox1;
		private System.Windows.Forms.ComboBox comboBoxContact;
		private System.Windows.Forms.ToolTip toolTip;
	}
}
