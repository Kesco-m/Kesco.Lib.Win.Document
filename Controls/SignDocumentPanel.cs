using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;

namespace Kesco.Lib.Win.Document.Controls
{
	public partial class SignDocumentPanel : AsyncUserControl
	{
		#region Constants

		private const int LABEL_COLUMN_IDX = 0;
		private const int SURNAME_COLUMN_IDX = 1;
		private const int DATE_COLUMN_IDX = 2;
		private const int REMOVE_COLUMN_IDX = 3;

		private const string DATE_FORMAT = "dd.MM.yyyy HH:mm:ss";

		private const int linkColor = -0x1000000 + 0x6495ED;

		#endregion

		#region Fields

		private int _docId = -1;
		private int? _imgId;

		private int _rowCount;
		private bool _canSign;
		private bool _canFinalSign;
		private bool _canSignCancel;
		object syncR = new object();
		private SynchronizedCollection<SignItem> items;

		private int _minWidth;

		private bool _droppedDown;
		private bool _updateState;
		private System.Windows.Forms.Control _parent;

		#endregion

		#region Properties

		public int MinWidth
		{
			get { return _minWidth > 0 ? _minWidth : 0; }
		}

		public bool IsFinalSign { get; private set; }

		public bool IsSignInternal
		{
			get { return items != null && items.Count > 0 && items.Any(x => x.SignType == SignType.interanalSign); }
		}

		public bool IsSigned
		{
			get { return items != null && items.Count > 0 && items.Any(x => x.SignType != SignType.noSign && x.SignType != SignType.interanalSign); }
		}

	    public bool IsSignCancel
        {
            get { return items != null && items.Count > 0 && items.Any(x => x.SignType == SignType.cancelSign); }
        }
		private int TotalHeight
		{
			get
			{
				return (tableLayoutPanel.Visible ? tableLayoutPanel.Height : 0) +
					   (llSign.Visible ? llSign.Height : 0);
			}
		}

		private bool NeedExpand
		{
			get { return !_droppedDown && (TotalHeight > ClientSize.Height || MinWidth > Width); }
		}

		private Form TopLevelForm
		{
			get
			{
				if(!_droppedDown)
					return (Form)TopLevelControl;

				return (Form)_parent.TopLevelControl;
			}
		}

		#endregion

		#region Events

		public event Action NeedRefresh;

		private void OnNeedRefresh()
		{
			if(NeedRefresh != null)
				NeedRefresh.DynamicInvoke();
		}

		public event Action<int> MinSizeChanged;

		public event Func<int, int, bool> AddingDocSign;
		public event Action RemovingDocSign;
		public event Components.DocumentSavedEventHandle AddedDocSign;
		public event Components.DocumentSavedEventHandle RemovedDocSign;

		private bool OnAddingDocSign()
		{
			bool ret = true;
			if(AddingDocSign != null)
			{
				return AddingDocSign(_docId, _imgId.HasValue ? _imgId.Value : -3);
			}
			return true;
		}

		private void OnRemovingDocSign()
		{
			if(RemovingDocSign != null)
				RemovingDocSign();
		}

		private void OnAddedDocSign()
		{
			if(AddedDocSign != null)
				AddedDocSign(this, new Components.DocumentSavedEventArgs(_docId, _imgId.HasValue ? _imgId.Value : -3));
		}

		private void OnRemovedDocSign()
		{
			if(RemovedDocSign != null)
				RemovedDocSign(this, new Components.DocumentSavedEventArgs(_docId, _imgId.HasValue ? _imgId.Value : -3));
		}


		public event Action NeedRefreshStamp;

		private void OnNeedRefreshStamp()
		{
			if(NeedRefreshStamp != null)
				NeedRefreshStamp();
		}

		#endregion

		#region Constructor

		public SignDocumentPanel()
		{
			InitializeComponent();

			AddSimpleLabel(0, Environment.StringResources.GetString("labSign"));

			llFinalSign.LinkColor = Color.FromArgb(linkColor);
			lLCancel.LinkColor = Color.FromArgb(linkColor);

			MouseEventMessageFilter.Instance.MouseMove += filter_MouseMove;
			MouseEventMessageFilter.Instance.MouseLeave += filter_MouseLeave;
			MouseEventMessageFilter.Instance.NCMouseLeave += filter_MouseLeave;
		//	llSign.Left = this.Width - llSign.PreferredWidth;
		}

		#endregion

		#region Clear

		public void ClearPanel()
		{
			items = null;
			ClearPanelControls(0);
		}

		private void ClearPanelControls(int startRowIdx)
		{
			tableLayoutPanel.SuspendLayout();
			try
			{
				for(int row = startRowIdx; row < tableLayoutPanel.RowCount; row++)
				{
					for(int column = 0; column < tableLayoutPanel.ColumnCount; column++)
					{
						if(row == 0 && column == 0)
							continue;

						System.Windows.Forms.Control ctrl = tableLayoutPanel.GetControlFromPosition(column, row);
						if(ctrl == null)
							continue;

						if(ctrl is TableLayoutPanel)
							ClearTableLayoutTable(ctrl as TableLayoutPanel);

						if(ctrl is PictureBox)
							ctrl.Click -= RemoveControl_Click;

						tableLayoutPanel.Controls.Remove(ctrl);
						ctrl.Dispose();
					}
				}

				_rowCount = startRowIdx;
				tableLayoutPanel.RowCount = startRowIdx != 0 ? startRowIdx : 1;
				if(startRowIdx == 0)
					tableLayoutPanel.Visible = false;
			}
			finally
			{
				tableLayoutPanel.ResumeLayout();
			}
		}

		private void ClearTableLayoutTable(TableLayoutPanel pn)
		{
			while(pn.Controls.Count > 0)
			{
				System.Windows.Forms.Control subControl = pn.Controls[0];

				pn.Controls.Remove(subControl);
				subControl.Dispose();
			}
		}

		#endregion

		#region Load document info

		public void LoadDocInfo(int docId, int? imgId)
		{
			CancelOperation();

			IsFinalSign = _canSign = _canFinalSign = _canSignCancel = false;

			if(docId == 0)
				return;
			CancellationToken ct = source.Token;
			Task<SynchronizedCollection<SignItem>> task = Task.Factory.StartNew<SynchronizedCollection<SignItem>>(() =>
					{
						return new SynchronizedCollection<SignItem>(syncR, Environment.DocSignatureData.GetDocumentSigns(docId, imgId, out _canSign, out _canFinalSign, out _canSignCancel, ct));
					}, ct);
			Task taskC = Task.Factory.ContinueWhenAny<SynchronizedCollection<SignItem>>(new Task<SynchronizedCollection<SignItem>>[ ] { task }, (taskc) =>
			{
				if(taskc.Status != TaskStatus.RanToCompletion)
					return;
				if(ct.IsCancellationRequested)
					ct.ThrowIfCancellationRequested();
				bool wasSigned = IsSigned;
				lock(this)
				{
					items = taskc.Result;
				}
				bool needRefresh = false;
				if(_docId == docId && !imgId.HasValue)
					if((wasSigned && !IsSigned) || (!wasSigned && IsSigned))
						OnNeedRefresh();
				if(ct.IsCancellationRequested)
					ct.ThrowIfCancellationRequested();
				_docId = docId;
				_imgId = imgId;
				if(ct.IsCancellationRequested)
					ct.ThrowIfCancellationRequested();
				if(this.InvokeRequired)
					this.BeginInvoke((MethodInvoker)(() => { DrawSignsItems(needRefresh, ct); }));
				else
					DrawSignsItems(needRefresh, ct);
			}, source.Token);
		}

		private void DrawSignsItems(bool needRefresh)
		{
			DrawSignsItems(needRefresh, CancellationToken.None);
		}


		private void DrawSignsItems(bool needRefresh, CancellationToken ct)
		{
			lock(this)
			{
				if(ct != CancellationToken.None && ct.IsCancellationRequested)
					return;
				SuspendLayout();
				tableLayoutPanel.SuspendLayout();
				try
				{
					if(items != null && items.Count > 0)
					{
						if(ct != CancellationToken.None && ct.IsCancellationRequested)
							return;
						SignItem last = items[items.Count - 1];
						IsFinalSign = last.SignType != SignType.firstSign;

						llSign.Visible = _canSign;
						llFinalSign.Visible = _canFinalSign;
						lLCancel.Visible = _canSignCancel;

						//Число уменьшилось или осталось темже
						if(items.Count <= _rowCount)
						{
							if(ct != CancellationToken.None && ct.IsCancellationRequested)
								return;
							if(items.Count < _rowCount)
								ClearPanelControls(items.Count);

							for(int row = 0; row < items.Count; row++)
								try
								{
									if(ct != CancellationToken.None && ct.IsCancellationRequested)
										return;
									UpdateRow(items[row], row);
								}
								catch(NullReferenceException)
								{
								}
						}
						//Число увеличилось
						else
						{
							if(ct != CancellationToken.None && ct.IsCancellationRequested)
								return;
							tableLayoutPanel.RowCount = items.Count;

							for(int row = 0; row < _rowCount; row++)
								try
								{
									if(ct.IsCancellationRequested)
										return;
									UpdateRow(items[row], row);
								}
								catch(NullReferenceException)
								{
								}

							for(int row = _rowCount; row < items.Count; row++)
							{
								if(ct != CancellationToken.None && ct.IsCancellationRequested)
									return;
								AddRow(items[row], row);
							}
						}

						_rowCount = items.Count;
					}
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
					Error.ErrorShower.OnShowError(null, ex.Message, "");
				}
				finally
				{
					tableLayoutPanel.ResumeLayout();
					ResumeLayout(true);
					tableLayoutPanel.Left = ClientSize.Width - tableLayoutPanel.Width;
					UpdateLinksLocation();
				}
				if(ct != CancellationToken.None && ct.IsCancellationRequested)
					return;
				if(items != null && items.Count > 0)
				{
					if(!tableLayoutPanel.Visible)
						tableLayoutPanel.Visible = true;
				}
				else
				{
					ClearPanelControls(0);
					tableLayoutPanel.Visible = false;

					llSign.Visible = _canSign;
					llFinalSign.Visible = _canFinalSign;
					lLCancel.Visible = _canSignCancel;
				}
			}
		}

		#endregion

		#region Adding rows

		private void AddRow(SignItem item, int rowIdx)
		{
			if(this.Disposing)
				return;
			Label lb = null;
			if(rowIdx == 0)
				lb = tableLayoutPanel.GetControlFromPosition(LABEL_COLUMN_IDX, 0) as Label;
			if(lb == null)
				lb = new Label();

			SetLabelProperties(lb, item.SignText);
			if(rowIdx > 0)
				tableLayoutPanel.Controls.Add(lb, LABEL_COLUMN_IDX, rowIdx);

			if(item.Employee != item.Employee4)
				AddCustomSurnameRow(item, rowIdx);
			else
				AddSimpleSurnameRow(item, rowIdx);

			var dateLabel = new Label();
			SetLabelProperties(dateLabel, GetLocalDateTime(item.Date));
			tableLayoutPanel.Controls.Add(dateLabel, DATE_COLUMN_IDX, rowIdx);

			if(item.CanRemove)
				AddRemovePicture(item, rowIdx);
		}

		private void AddCustomSurnameRow(SignItem item, int rowIdx)
		{
			if(this.Disposing)
				return;
			var pn = new TableLayoutPanel { AutoSize = true, ColumnCount = 3, Margin = new Padding(0) };

			var hll = new HoverLinkLabel(TopLevelForm);
			SetLabelProperties(hll, item.FIO);
			hll.Caption = string.Format("№{0} {1}", item.Employee, hll.Text);
			hll.Url = Environment.UsersURL + item.Employee;
			pn.Controls.Add(hll, 2, 0);

			var lb = new Label();
			SetLabelProperties(lb, "/");
			lb.TextAlign = ContentAlignment.MiddleCenter;
			lb.Margin = new Padding(0);
			pn.Controls.Add(lb, 1, 0);

			var hll4 = new HoverLinkLabel(TopLevelForm);
			SetLabelProperties(hll4, item.FIO4);
			hll4.Margin = new Padding(hll.Margin.Left, 0, 0, 0);
			hll4.Caption = string.Format("№{0} {1}", item.Employee4, hll4.Text);
			hll4.Url = Environment.UsersURL + item.Employee4;
			pn.Controls.Add(hll4, 0, 0);

			tableLayoutPanel.Controls.Add(pn, SURNAME_COLUMN_IDX, rowIdx);
		}

		private void AddSimpleSurnameRow(SignItem item, int rowIdx)
		{
			if(this.Disposing)
				return;
			var hll = new HoverLinkLabel(TopLevelForm);
			SetLabelProperties(hll, item.FIO);
			hll.Caption = string.Format("№{0} {1}", item.Employee, hll.Text);
			hll.Url = Environment.UsersURL + item.Employee;
			tableLayoutPanel.Controls.Add(hll, SURNAME_COLUMN_IDX, rowIdx);
		}

		private PictureBox AddRemovePicture(SignItem item, int rowIdx)
		{
			if(this.Disposing || imageList.Images.Count < 1)
				return null;
			PictureBox pb = new PictureBox
						 {
							 SizeMode = PictureBoxSizeMode.AutoSize,
							 Cursor = Cursors.Hand,
							 Image = imageList.Images[0],
							 Margin = new Padding(0),
							 Tag = item
						 };
			pb.Click += RemoveControl_Click;

			tableLayoutPanel.Controls.Add(pb, REMOVE_COLUMN_IDX, rowIdx);

			return pb;
		}

		private static void SetLabelProperties(Label control, string text)
		{
			control.AutoSize = true;
			control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			control.MinimumSize = new Size(0, 18);
			control.TextAlign = ContentAlignment.MiddleLeft;
			control.Text = text;
		}

		private void AddSimpleLabel(int row, string text)
		{
			var lb = new Label();
			SetLabelProperties(lb, text);
			tableLayoutPanel.Controls.Add(lb, LABEL_COLUMN_IDX, row);
		}

		#endregion

		#region Update row

		private void UpdateRow(SignItem item, int rowIdx)
		{
			if(item == null || tableLayoutPanel == null || this.Disposing)
				return;
			Label lb;
			if(rowIdx == 0)
			{
				lb = (Label)tableLayoutPanel.GetControlFromPosition(LABEL_COLUMN_IDX, 0);
			}
			else
			{
				lb = (Label)tableLayoutPanel.GetControlFromPosition(LABEL_COLUMN_IDX, rowIdx);
			}
			if(lb == null)
			{
				lb = new Label();

				tableLayoutPanel.Controls.Add(lb, LABEL_COLUMN_IDX, rowIdx);
			}

			SetLabelProperties(lb, item.SignText);

			System.Windows.Forms.Control ctrl = tableLayoutPanel.GetControlFromPosition(SURNAME_COLUMN_IDX, rowIdx);
			if(ctrl is TableLayoutPanel)
			{
				var pn = (TableLayoutPanel)ctrl;
				if(item.Employee != item.Employee4)
				{
					var hll = (HoverLinkLabel)pn.GetControlFromPosition(0, 0);
					hll.Url = Environment.UsersURL + item.Employee4;
					hll.Text = item.FIO4;
					hll.Caption = string.Format("№{0} {1}", item.Employee4, hll.Text);

					var hll2 = (HoverLinkLabel)pn.GetControlFromPosition(2, 0);
					hll2.Url = Environment.UsersURL + item.Employee;
					hll2.Text = item.FIO;
					hll2.Caption = string.Format("№{0} {1}", item.Employee, hll2.Text);
				}
				else
				{
					ClearTableLayoutTable(pn);
					tableLayoutPanel.Controls.Remove(pn);
					AddSimpleSurnameRow(item, rowIdx);
				}
			}
			else
			{
				if(item.Employee != item.Employee4)
				{
					tableLayoutPanel.Controls.Remove(ctrl);
					AddCustomSurnameRow(item, rowIdx);
				}
				else
				{
					var hll = (HoverLinkLabel)ctrl;
					hll.Url = Environment.UsersURL + item.Employee;
					hll.Text = item.FIO;
					hll.Caption = string.Format("№{0} {1}", item.Employee, hll.Text);
				}
			}

			var date = (Label)tableLayoutPanel.GetControlFromPosition(DATE_COLUMN_IDX, rowIdx);
			if(date != null)
				date.Text = GetLocalDateTime(item.Date);

			var pb = (PictureBox)tableLayoutPanel.GetControlFromPosition(REMOVE_COLUMN_IDX, rowIdx);
			if(pb == null)
			{
				if(item.CanRemove)
					AddRemovePicture(item, rowIdx);
			}
			else
			{
				if(item.CanRemove)
				{
					pb.Tag = item;
				}
				else
				{
					pb.Click -= RemoveControl_Click;
					tableLayoutPanel.Controls.Remove(pb);
				}
			}
		}

		#endregion

		#region Remove sign

		private void RemoveControl_Click(object sender, EventArgs args)
		{
			try
			{
				if(_droppedDown)
					CollapsePanel(true);

				OnRemovingDocSign();

				var pbSender = (PictureBox)sender;
				var item = (SignItem)pbSender.Tag;

				string text = string.Format(Environment.StringResources.GetString("msgRemoveSignConfirm"), item.FIO,
					(_imgId > 0 ? Environment.StringResources.GetString("OImage") : Environment.StringResources.GetString("OEForm")),
					(item.Employee == Environment.CurEmp.ID || item.Employee4 == Environment.CurEmp.ID) ? "" : StringResources.msgRemoveSignEmployeeConfirm);

				if(!_imgId.HasValue || _imgId < 1 && item.SignType == SignType.finalSign)
				{
					// проверку на тип сувать сюда
					int count = Environment.TransactionData.GetCount(_docId);
					if(count > 0)
					{
						if(
							MessageBox.Show(String.Format(Environment.StringResources.GetString("DeleteTrans"), count),
											Environment.StringResources.GetString("DeleteConfirmation"),
											MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
											MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						{
							bool delTran = Environment.TransactionData.DeleteTransaction(_docId);
							if(delTran)
								DeleteSignItem(item);
						}
						return;
					}
				}
                if (item.Employee == Environment.CurEmp.ID || 
					MessageBox.Show(text, Environment.StringResources.GetString("Warning"), MessageBoxButtons.OKCancel,
									MessageBoxIcon.Question) == DialogResult.OK)
				{
					DeleteSignItem(item);
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				Error.ErrorShower.OnShowError(null, ex.Message, "");
			}
		}

        /// <summary>
        /// Удаление подписи
        /// </summary>
        /// <param name="item"></param>
		private void DeleteSignItem(SignItem item)
		{
			if(Environment.DocSignatureData.Delete(item.SignId))
			{
                // в стек UNDO помещаем свои подписи, на востанавление чужих подписей может не хватить прав 
                if(Environment.CurEmp.ID == item.Employee)
			        UndoRemoveSign(item.SignId, _docId, true, _imgId, item.Employee, (byte)item.SignType);

				if(item.SignType == SignType.stampSign)
					OnNeedRefreshStamp();
				LoadDocInfo(_docId, _imgId);
				OnRemovedDocSign();
			}
		}

        /// <summary>
        /// Формирование команды стека Undo Remove Sign
        /// </summary>
        /// <param name="кодПодписиДокумента"></param>
        /// <param name="docId"></param>
        /// <param name="isImage"></param>
        /// <param name="imgId"></param>
        /// <param name="employeeId"></param>
        /// <param name="singT"></param>
        private void UndoRemoveSign(int кодПодписиДокумента, int docId, bool isImage, int? imgId, int employeeId, byte singT)
        {
            Func<object[], bool> undoDelegate = UndoRedoCommands.RedoSign;
            Func<object[], bool> redoDelegate = UndoRedoCommands.UndoSign;

            var undoText = Environment.StringResources.GetString("SignDocumentPanel_DeleteSign_Undo");
            var redoText = Environment.StringResources.GetString("SignDocumentPanel_DeleteSign_Redo");

            Environment.UndoredoStack.Add("RemoveSign", "RemoveSign", undoText, redoText, null, new object[] { кодПодписиДокумента, undoDelegate, redoDelegate, docId, isImage, imgId, Environment.CurEmp.ID, employeeId, singT }, Environment.CurEmp.ID);
        }

		#endregion

		#region Add sign

		private void llSign_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			AddSign(SignType.firstSign);
		}

		private void llFinalSign_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			AddSign(SignType.finalSign);
		}

		private void lLCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			AddSign(SignType.cancelSign);
		}

		private void AddSign(SignType sign)
		{
			try
			{
				if(!OnAddingDocSign())
					return;

				if(_droppedDown)
					CollapsePanel(true);

				bool sendMessage = sign == SignType.firstSign || Environment.UserSettings.MessageOnEndSign;
				Employee employee4 = Environment.CurEmp;
				List<Employee> employees = Environment.ReplacementEmployeeData.GetReplacementEmployees();

				if(employees.Count > 1 || sign != SignType.firstSign)
				{
					var dlg = new Select.SelectSignEmployeeDialog(employees, sign);
					if(dlg.ShowDialog() != DialogResult.OK)
						return;

					employee4 = dlg.Employee4;
					sendMessage = dlg.SendMessage;
				}

				byte singT = Convert.ToByte(sign);
			    int? кодПодписиДокумента;

			    bool isImage = _imgId.HasValue;

                if (isImage ? Environment.DocSignatureData.AddSign(_docId, _imgId.Value, Environment.CurEmp.ID, employee4.ID, singT, out кодПодписиДокумента)
                        : Environment.DocSignatureData.AddSign(_docId, Environment.CurEmp.ID, employee4.ID, singT, out кодПодписиДокумента))
				{
					LoadDocInfo(_docId, _imgId);

                    if (кодПодписиДокумента != null) // успешно подписали
                        UndoSign(кодПодписиДокумента.Value, _docId, isImage, (isImage ? _imgId.Value : (int?)null), employee4.ID, singT);

				    if(sendMessage)
					{
						string str =
							Environment.SignTextData.GetSignMessageText(
								Environment.DocData.GetDocIntField(Environment.DocData.DocTypeIDField, _docId), singT,
								Environment.CurCultureInfo.TwoLetterISOLanguageName);
						if(string.IsNullOrEmpty(str))
							switch(sign)
							{
								case SignType.firstSign:
									str = "Документ подписан";
									break;
								case SignType.finalSign:
									str = "Работа завершена";
									break;
								case SignType.cancelSign:
									str = "Изображение аннулировано";
									break;
							}
						foreach(int empID in Environment.EmpData.GetCurrentEmployeeIDs())
							Environment.WorkDocData.MarkAsRead(empID, _docId);
						var smd = new Dialogs.SendMessageDialog(_docId, str) { Check = true };
						// не правильная реализация события
						smd.FormClosed += smd_FormClosed; 
						smd.Show();
					}
					else
						OnAddedDocSign();
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				Error.ErrorShower.OnShowError(null, ex.Message, "");
			}
		}

	    /// <summary>
	    /// Формирование команды  стека Undo Sign
	    /// </summary>
	    /// <param name="кодПодписиДокумента"></param>
	    /// <param name="docId"></param>
	    /// <param name="isImage"></param>
	    /// <param name="imgId"></param>
	    /// <param name="employeeId"></param>
	    /// <param name="singT"></param>
	    private void UndoSign(int кодПодписиДокумента, int docId, bool isImage, int? imgId, int employeeId, byte singT)
        {
            Func<object[], bool> undoDelegate = UndoRedoCommands.UndoSign;
            Func<object[], bool> redoDelegate = UndoRedoCommands.RedoSign;

            var undoText = Environment.StringResources.GetString("SignDocumentPanel_Sign_Undo");
            var redoText = Environment.StringResources.GetString("SignDocumentPanel_Sign_Redo");

            Environment.UndoredoStack.Add("Sign", "Sign", undoText, redoText, null, new object[] { кодПодписиДокумента, undoDelegate, redoDelegate, docId, isImage, imgId, Environment.CurEmp.ID, employeeId, singT }, Environment.CurEmp.ID);
        }

		private void smd_FormClosed(object sender, FormClosedEventArgs e)
		{
			if(e.CloseReason == CloseReason.UserClosing) //&& ((Dialogs.SendMessageDialog)sender).DocIDs.Contains(_docId))
				OnAddedDocSign();
		}

		#endregion

		#region Change link labels location

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			AutoScroll = false;
		}

		private void tableLayoutPanel_SizeChanged(object sender, EventArgs e)
		{
			UpdateLinksLocation();
		}

		private void tableLayoutPanel_VisibleChanged(object sender, EventArgs e)
		{
			UpdateLinksLocation();
		}

		private void ll_VisibleChanged(object sender, EventArgs e)
		{
			UpdateLinksLocation();
		}

		private void SignDocumentPanel_ClientSizeChanged(object sender, EventArgs e)
		{
			if(items != null && items.Count > 1)
			{
				var tr = new Thread(StartDoSizeChange);
				tr.Start();
			}
		}

		private void StartDoSizeChange()
		{
			//  Application.DoEvents();
			if(InvokeRequired)
				Invoke((MethodInvoker)(DoSizeChanged));
			else
				DoSizeChanged();
		}

		public void DoSizeChanged()
		{
			if(!_updateState && !_droppedDown)
			{
				lock(this)
				{
					CheckMinSize();
				}
			}
		}

		private void UpdateLinksLocation()
		{
			if(_updateState)
				return;
			lock(this)
			{
				llSign.Top = tableLayoutPanel.Visible ? tableLayoutPanel.Bottom : Math.Min(tableLayoutPanel.Top, 0);
				llSign.Left = tableLayoutPanel.Right - llSign.PreferredWidth;

				int left = 0;
				if(tableLayoutPanel.Visible)
				{
					left = tableLayoutPanel.Left;
					if(left < 0)
						left = 0;
					System.Windows.Forms.Control ctrl = tableLayoutPanel.GetControlFromPosition(0, 0);
					if(ctrl != null && ctrl is Label)
						left += ctrl.Left;
				}
				else if(llFinalSign.Visible)
					left = llSign.Left - llFinalSign.Width;
				else if(lLCancel.Visible)
					left = lLCancel.Left - lLCancel.Width;

				lLCancel.Left = left;
				llFinalSign.Left = left;

				llFinalSign.Top = llSign.Top;
				lLCancel.Top = llFinalSign.Visible ? llFinalSign.Bottom : llSign.Top;

				CheckMinSize();
			}
		}

		private void CheckMinSize()
		{
			if(MinSizeChanged != null)
			{
				int width = tableLayoutPanel.Visible
								? tableLayoutPanel.Width
								: ((llFinalSign.Visible ? llFinalSign.Width + 2 : lLCancel.Width + 2) + llSign.PreferredWidth);
				width += Size.Width - ClientSize.Width;

				if(_minWidth != width && width > 0)
				{
					_minWidth = width;
					MinSizeChanged(width);
				}
			}
		}

		#endregion

		#region Convert datetime

		private string GetLocalDateTime(DateTime utcDate)
		{
			var date = new DateTime(utcDate.Ticks, DateTimeKind.Utc);
			return date.ToLocalTime().ToString(DATE_FORMAT);
		}

		#endregion

		#region Drop down support

		private void filter_MouseMove(IntPtr wParam)
		{
			if(!NeedExpand)
				return;

			if(Form.ActiveForm != TopLevelControl)
				return;

			if((wParam.ToInt32() & (Int32)Win32.MK.MK_LBUTTON) != 0 ||
				(wParam.ToInt32() & (Int32)Win32.MK.MK_RBUTTON) != 0) //Нажата левая клавиша мыши
				return;

			Point pt = MousePosition;
			if(!RectangleToScreen(ClientRectangle).Contains(pt))
				return;

			if(HoveredByTopWindow())
				return;

			ExpandPanel();
		}

		private bool HoveredByTopWindow()
		{
			try
			{
				IntPtr hChild = Win32.User32.GetTopWindow(IntPtr.Zero);
				if(hChild == IntPtr.Zero)
					return false;

				if(Win32.User32.IsWindowVisible(hChild) == 0)
					return false;

				if(FromChildHandle(hChild) != null)
					return false;

				Win32.User32.RECT rect;
				if(Win32.User32.GetWindowRect(hChild, out rect) != 0)
				{
					var rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left,
														rect.bottom - rect.top);
					return Rectangle.Intersect(rectangle, RectangleToScreen(Bounds)) != Rectangle.Empty;
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			return false;
		}

		private void filter_MouseLeave()
		{
			if(_droppedDown)
			{
				Point pt = MousePosition;
				Rectangle bounds = RectangleToScreen(ClientRectangle);
				if(VerticalScroll.Visible)
					bounds.Width += SystemInformation.VerticalScrollBarWidth;

				if(!bounds.Contains(pt))
					CollapsePanel(true);
			}
		}

		private void ExpandPanel()
		{
			_updateState = true;

			Point pt = PointToScreen(Location);
			pt.Offset(-2, -2);

			if(Width < MinWidth)
				pt.Offset(Width - MinWidth, 0);

			_parent = Parent;

			Parent.Controls.Remove(this);

			AutoScroll = true;
			BorderStyle = BorderStyle.Fixed3D;

			Rectangle screen = Screen.FromPoint(pt).WorkingArea;
			var form = new Form
						   {
							   FormBorderStyle = FormBorderStyle.None,
							   Location = pt,
							   StartPosition = FormStartPosition.Manual,
							   ShowInTaskbar = false,
							   Size = new Size(Math.Min(screen.Width, Math.Max(MinWidth, Width)),
											   Math.Min(Math.Max(TotalHeight + 5, Height), screen.Height))
						   };


			if(!screen.Contains(form.Bounds))
			{
				int x = form.Right > screen.Right
							? screen.Right - form.Width
							: (form.Left < screen.Left ? screen.Left : form.Left);
				int y = form.Bottom > screen.Bottom
							? screen.Bottom - form.Height
							: (form.Top < screen.Top ? screen.Top : form.Top);

				form.Location = new Point(x, y);
			}

			form.Controls.Add(this);

			if(VerticalScroll.Visible)
			{
				if(form.Left > SystemInformation.VerticalScrollBarWidth)
					form.Left -= SystemInformation.VerticalScrollBarWidth;

				form.Width += SystemInformation.VerticalScrollBarWidth;
			}

			form.Show(_parent.FindForm());
			form.Deactivate += form_Deactivate;
			form.FormClosed += form_FormClosed;

			_droppedDown = true;
			_updateState = false;
		}

		private void form_Deactivate(object sender, EventArgs e)
		{
			CollapsePanel(true);
		}

		private void form_FormClosed(object sender, FormClosedEventArgs e)
		{
			if(e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.None)
				CollapsePanel(false);
		}

		private void CollapsePanel(bool needClose)
		{
			_updateState = true;

			var form = (Form)Parent;
			form.Deactivate -= form_Deactivate;
			form.FormClosed -= form_FormClosed;

			form.Controls.Remove(this);
			if(needClose)
				form.Close();

			AutoScroll = false;
			BorderStyle = BorderStyle.Fixed3D;

			Location = new Point(0, 0);
			_parent.Controls.Add(this);

			_droppedDown = false;
			_updateState = false;

			UpdateLinksLocation();
		}

		#endregion

		#region WndProc

		protected override void WndProc(ref Message m)
		{
			try
			{
				if(m.Msg == (Int32)Win32.Msgs.WM_PARENTNOTIFY)
				{
					if(m.WParam.ToInt64() == (Int64)Win32.Msgs.WM_LBUTTONDOWN)
					{
						int x = (ushort)(((uint)m.LParam) & 0xFFFF);
						int y = (ushort)((((uint)m.LParam) & 0xFFFF0000) >> 16);

						System.Windows.Forms.Control ctrl = GetChildAtPoint(new Point(x, y));

						if(!_droppedDown && TotalHeight > Height && !(ctrl is HoverLinkLabel))
							ExpandPanel();
					}
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, m.ToString());
			}
			base.WndProc(ref m);
		}

		#endregion
	}
}