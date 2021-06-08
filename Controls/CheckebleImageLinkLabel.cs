using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Controls
{
	public partial class CheckebleImageLinkLabel : FlowLayoutPanel
	{
		public CheckebleImageLinkLabel()
		{
			InitializeComponent();
		}

		public override string Text 
		{ 
			get => linkLabelText.Text; 
			set => linkLabelText.Text=value;
		}

		public Image ImageClose
		{
			get => pictureBoxClose.Image;
			set => pictureBoxClose.Image = value;
		}

		public Image ImageHead
		{
			get => pictureBoxImage.Image;
			set 
			{
				pictureBoxImage.Image = value;
				pictureBoxImage.Visible = value != null;
			}
		}

		//
		// Сводка:
		//     Возвращает или задает значение, указывающее, отображаются ли чекбокс
		//
		// Возврат:
		//     Значение true, если элемент управления и все его дочерние элементы управления
		//     отображаются; в противном случае — значение false. Значение по умолчанию — false.
		[Localizable(true)]
		[Bindable(true)]
		[SettingsBindable(false)]
		[DefaultValue(true)]
		[RefreshProperties(RefreshProperties.All)]
		public bool CheckBoxVisible
		{
			get => checkBox.Visible;
			set => checkBox.Visible = value;
		}

		//
		// Сводка:
		//     Получает или задает значение, определяющее, находится ли System.Windows.Forms.CheckBox
		//     в выбранном состоянии.
		//
		// Возврат:
		//     Значение true, если элемент управления System.Windows.Forms.CheckBox находится
		//     во включенном состоянии; в противном случае — false. Значение по умолчанию —
		//     false. Если для свойства System.Windows.Forms.CheckBox.ThreeState установлено
		//     значение true, свойство System.Windows.Forms.CheckBox.Checked будет возвращать
		//     значение true для Checked или IndeterminateSystem.Windows.Forms.CheckBox.CheckState.
		[Bindable(true)]
		[SettingsBindable(true)]
		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		public bool Checked
		{
			get { return checkBox.Checked; }
			set { checkBox.Checked = value; }
		}

		//
		// Сводка:
		//     Возвращает или задает значение, указывающее, следует ли отображать ссылку как
		//     выбранную ранее.
		//
		// Возврат:
		//     true, если ссылки должны отображаться, как будто они уже были выбраны; в противном
		//     случае — false. Значение по умолчанию — false.
		[DefaultValue(false)]
		public bool LinkVisited 
		{
			get => linkLabelText.LinkVisited;
			set => linkLabelText.LinkVisited = value; 
		}

		protected override void OnClick(EventArgs e)
		{
			return;
		}

		protected void OnMyClick(EventArgs e)
		{
			base.OnClick(e);
		}

		// Сводка:
		//     Происходит при изменении значения свойства System.Windows.Forms.CheckBox.Checked.
		public event EventHandler CheckedChanged;

		//
		// Сводка:
		//     Происходит при щелчке ссылки в элементе управления.
		public event LinkLabelLinkClickedEventHandler LinkClicked;
		
		private void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
		{
			if(LinkClicked!= null)
				LinkClicked(this, e);
		}

		private void linkLabelText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OnLinkClicked(e);
		}

		private void pictureBoxClose_Click(object sender, EventArgs e)
		{
			OnMyClick(e);
		}
	}
}
