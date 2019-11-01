using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Grid
{
	public class SelectDataGrid : DataGridView
	{
		internal bool block;
		internal bool blocksel;
		static readonly object _object = new object();
		private const long waitTick = 2000;
		private CancellationTokenSource cancel;
		private Task task;
		//internal volatile Semaphore lockS;
			
		public SelectDataGrid()
		{
			//lockS = new Semaphore(0, 1);
			InitializeComponent();
			DoubleBuffered = true;
			RowTemplate.Height = FontHeight + 5;
			//BindingContextChanged += new EventHandler(Semaphore_DataBindingComplete);
			ColumnAdded += Grid_ColumnAdded;
			ColumnHeaderMouseClick += SelectDataGrid_ColumnHeaderMouseClick;
			CurrentCellChanged += Grid_CurrentCellChanged;
			RowStateChanged += new DataGridViewRowStateChangedEventHandler(SelectDataGrid_RowStateChanged);
		}

		void SelectDataGrid_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
		{
				if(e.StateChanged == DataGridViewElementStates.Selected && Rows.Count > 0 && e.Row.Index > -1 && e.Row.Selected && e.Row.Index == CurrentRowIndex && KeyObject == null)
				OnSelectionChanged(EventArgs.Empty);
		}

		public void Init(Options.Folder layout)
		{
			OptionFolder = layout.Folders.Add(GetType().Name);
		}

		#region ColumnAdded

		protected virtual void Grid_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
		{
			e.Column.HeaderCell = new DocsDataGridColumnHeaderCell(e.Column.HeaderText);
			e.Column.Width = OptionFolder.LoadIntOption(e.Column.Name, 100);

			FormatColumn(e.Column);
		}

		protected void FormatColumn(DataGridViewColumn column)
		{
			column.Resizable = DataGridViewTriState.True;
			column.Frozen = false;
			column.DataPropertyName = column.Name;
			column.SortMode = DataGridViewColumnSortMode.Programmatic;

			if(column is DataGridViewCheckBoxColumn)
			{
				column.DefaultCellStyle.NullValue = false;
				column.CellTemplate.Style.Alignment = DataGridViewContentAlignment.TopCenter;
			}
			else
			{
				column.DefaultCellStyle.NullValue = "";
				column.CellTemplate.Style.Alignment = DataGridViewContentAlignment.TopLeft;
			}
		}

		#endregion

		#region Accessors

		private object keyObject = null;
		public object KeyObject 
		{
			get { return keyObject; }
			protected set {keyObject = value;}
		}
		public int KeyIndex { get; protected set; }

		protected virtual string KeyField
		{
			get
			{
				return IsFine ? Columns[0].Name : string.Empty;
			}
		}

		protected virtual string IDField
		{
			get
			{
				return IsFine ? Columns[0].Name : string.Empty;
			}
		}

		public Options.Folder OptionFolder { get; protected set; }

		public int CurrentRowIndex
		{
		    get
		    {
                // Заявка №27393
                // FIX Ошибки nullReferenceException
		        var currentRow = CurrentRow;
				return currentRow != null ? currentRow.Index : -1;
		    }
		}

		public int CurrentColumnIndex
		{
			get { return CurrentCell != null ? CurrentCell.ColumnIndex : FirstDisplayedCell.ColumnIndex; }
		}

		public bool CurrentRowSelected
		{
			get
			{
				try
				{
					return CurrentRow != null && CurrentRow.Selected;
				}
				catch
				{
					return false;
				}
			}
		}

		public bool CurrentRowDisplayed
		{
			get
			{
				try
				{
					return CurrentRow != null && CurrentRow.Displayed;
				}
				catch
				{
					return false;
				}
			}
		}

		public int SelectedRowsMinIndex
		{
			get
			{
				if(SelectedRows.Count == 0)
					return -1;

				return (from DataGridViewRow row in SelectedRows select row.Index).Min();
			}
		}

		public int SelectedRowsCount
		{//патамушта так быстрее
			get { return Rows.GetRowCount(DataGridViewElementStates.Selected); }
		}

		public new DataGridViewCell CurrentCell
		{
			get { return base.CurrentCell; }
			set 
			{
				if(!block)
					base.CurrentCell = value;
			}
		}

		#endregion

		#region GetValue + SetValue

		public object GetCurValue(string colName)
		{
			try
			{
			    var currentRow = CurrentRow;

                return currentRow != null ? GetValue(currentRow.Index, colName) : null;
			}
			catch
			{
				return null;
			}
		}

		public object GetValue(int row, string colName)
		{
		    if (-1 < row && row < Rows.Count && !string.IsNullOrEmpty(colName))
		    {
		        if (Columns.Contains(colName))
		        {
                    var dataGridViewColumn = Columns[colName];

                    if (dataGridViewColumn != null)
                    {
                        var cell = this[dataGridViewColumn.Index, row];

                        if (cell != null)
                            return cell.Value;
                    }
		        }
		    }

		    return null;
		}

		public bool SetValue(int row, string colName, object val)
		{
			return row > -1 && row < Rows.Count && !string.IsNullOrEmpty(colName) && Columns.Contains(colName) &&
				   SetValue(row, Columns[colName].Index, val);
		}

		public void SetValueByID(int id, string colName, object val)
		{
			int row = GetIndexConditional(Environment.DocData.IDField, id);
			if(row > -1 && row < Rows.Count && !string.IsNullOrEmpty(colName) && Columns.Contains(colName))
			{
				if(this[colName, row].Value.Equals(val))
					return;
				try
				{
					SuspendLayout();
					this[colName, row].Value = val;
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
				}
				finally
				{
					ResumeLayout();
					Invalidate();
				}
			}
		}

		public bool SetValue(int row, int col, object val)
		{
			if(this[col, row].Value.Equals(val))
				return true;
			try
			{
				SuspendLayout();
				this[col, row].Value = val;
				return true;
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				return false;
			}
			finally
			{
				ResumeLayout();
				Invalidate();
			}
		}

		public int GetCurID()
		{
			object obj = GetCurValue(IDField);
			return obj != null && obj is int ? (int)obj : 0;
		}

		public int GetIndex(object key)
		{
			return GetIndexConditional(KeyField, key);
		}

		public int GetIndexConditional(string field, object key)
		{
			if(key != null)
				for(int row = 0; row < Rows.Count; row++)
				{
					if(key.Equals(GetValue(row, field)))
						return row;
				}

			return -1;
		}

		#endregion

		#region IS

		/// <summary>
		/// Показывает, что грид в нормальном состоянии и из него можно читать данные.
		/// </summary>
		public bool IsFine
		{
			get { return DataSource != null && ColumnCount > 0 && !Disposing && !IsDisposed; }
		}

		/// <summary>
		/// Показывает, что выделен только один элемент списка.
		/// </summary>
		public bool IsSingle
		{
			get { return SelectedRows.Count == 1; }
		}

		/// <summary>
		/// Показывает, что выделено несколько элементов списка.
		/// </summary>
		public bool IsMultiple
		{
			get { return SelectedRows.Count > 1 && SelectedRows.Count < 501; }
		}

		/// <summary>
		/// Показывает, что выделено не более 100 элементов списка.
		/// </summary>
		public bool IsMultipleSmall
		{
			get { return SelectedRows.Count > 1 && SelectedRows.Count < 101; }
		}

		#endregion

		#region Select

		public virtual bool SelectConditional(string field, object key, bool soft)
		{
            Console.WriteLine("{0}: SelectConditional", DateTime.Now.ToString("HH:mm:ss fff"));
			if(!IsFine)
				return false;
			int index = GetIndexConditional(field, key);
			if(index > -1)
			KeyIndex = index;
			if(index > -1 && index < Rows.Count)
			{
				var cur = CurrentRowIndex;

				if(cur == index)
				{
					OnCurrentCellChanged(EventArgs.Empty);
					return true;
				}
				try
				{
					for(int k = this.SelectedRows.Count; k > 0; k--)
						this.SelectedRows[k - 1].Selected = false;
				}
				catch{}
				if(soft)
				{
					SelectRow(index);
					return true;
				}
				return SelectRow(index);
			}

			return false;
		}

		public virtual bool SelectRow(object key)
		{
			Console.WriteLine("{0}: SelectRow {1}", DateTime.Now.ToString("HH:mm:ss fff"), key);
			return SelectConditional(KeyField, key, true);
		}

		public virtual bool SelectRow(int row)
		{
			if(!IsFine || InvokeRequired)
				return false;

			try
			{
				if(row > Rows.Count - 1)
					row = Rows.Count - 1;

				if(row > -1)
				{
					CurrentCell = this[CurrentColumnIndex, row];
					Rows[row].Selected = true;
					Console.WriteLine("{0}: Select row {1} keyObject {2}", DateTime.Now.ToString("HH:mm:ss fff"), row, KeyObject);
					DisplayCurrentRow();
					OnCurrentCellChanged(null);

					return true;
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, string.Format("Select row {0} keyObject {1}", row, KeyObject));
			}
			return false;
		}

		#endregion

		#region SelectedIDs

		public int[] GetCurIDs()
		{
			if(IDField != null && IsFine && Columns.Contains(IDField))
			{
				var ids = new int[SelectedRows.Count];
				for(int i = 0; i < SelectedRows.Count; i++)
					ids[i] = (int)SelectedRows[i].Cells[IDField].Value;

				return ids.Length > 0 ? ids : new[] { -1 };
			}
			return new[] { -1 };
		}

		#endregion

		#region Misc

		public DataView GetDataView()
		{
			if(DataSource != null && BindingContext != null)
			{
				var cm = (CurrencyManager)BindingContext[DataSource, DataMember];
				var dv = (DataView)cm.List;
				return dv;
			}
			return null;
		}

		/// <summary>
		/// перегрузка метода выделения ячейки
		/// </summary>
		/// <param name="e"></param>
		protected override void OnCurrentCellChanged(EventArgs e)
		{
			if(!block)  //&& !(bool)typeof(Control).GetProperty("IsLayoutSuspended", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this, null))
				//if(CurrentCell != null && CurrentCell.ColumnIndex != 0)
				//    CurrentCell = this[0, CurrentCell.RowIndex];
				//else
					base.OnCurrentCellChanged(e);
		}

		protected override void OnSelectionChanged(EventArgs e)
		{
			if(!block && !(bool)typeof(Control).GetProperty("IsLayoutSuspended", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this, null))
				if(CurrentCell != null && CurrentCell.ColumnIndex != 0 && !IsMultiple && !blocksel)
				{
					blocksel = true;

					CurrentCell = this[0, CurrentCell.RowIndex];
				}
				else
				{
					if(cancel != null)
					{
						cancel.Cancel();
						cancel.Dispose();
						cancel = null;
					}
					blocksel = false;
					base.OnSelectionChanged(e);
				}
			else
				if(CurrentCell != null)
				{
					if(cancel != null)
					{
						cancel.Cancel();
						cancel.Dispose();
					}
					cancel = new CancellationTokenSource();

					var cancellationToken = cancel.Token;
					task = Task.Factory.StartNew(() =>
					{
						Thread.Sleep(new TimeSpan(waitTick));
						if(cancellationToken.IsCancellationRequested)
							cancellationToken.ThrowIfCancellationRequested();
						if(this.InvokeRequired)
							this.BeginInvoke( (MethodInvoker)(()=>{base.OnSelectionChanged(e);}));
						else
							base.OnSelectionChanged(e);
					}, cancellationToken);
				}
		}
		
		private void Grid_CurrentCellChanged(object sender, EventArgs e)
		{
            Console.WriteLine("{0}: keyvalue changed", DateTime.Now.ToString("HH:mm:ss fff"));
			if(!block && !(bool)typeof(Control).GetProperty("IsLayoutSuspended", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this, null))
			{
				SetKeys();
			}
		}

		public void SetKeys()
		{
			KeyObject = CurrentCell != null && IsSingle && !string.IsNullOrEmpty(KeyField) ? GetCurValue(KeyField) : null;
			KeyIndex = CurrentCell != null ? CurrentCell.RowIndex : -1;
		}

		public void DropKeys()
		{
			KeyObject = null;
			KeyIndex = -1;
		}

		public virtual void DisplayCurrentRow()
		{
			if(!VerticalScrollBar.Visible || SelectedRows.Count < 1)
				return;

			for(int i = 0; i < Rows.Count && !SelectedRows[0].Displayed; i++)//чтобы не бегал до бесконечности
				try
				{
					if(FirstDisplayedScrollingRowIndex < SelectedRows[0].Index)
						FirstDisplayedScrollingRowIndex++;
					else if(FirstDisplayedScrollingRowIndex > SelectedRows[0].Index)
						FirstDisplayedScrollingRowIndex--;
					else
						return;
				}
				catch { return; }
		}

		protected void SelectDataGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			try
			{
				this.DataBindingComplete -= SelectDataGrid_DataBindingComplete;
				if(e.ListChangedType != System.ComponentModel.ListChangedType.Reset)
					return;
				RestoreSelect();
			}
			catch { }
		}

		protected new object DataSource
		{
			get { return base.DataSource; }
			set
			{
				//lockS.WaitOne();
				base.DataSource = value;
			}
		}

		protected internal void RestoreSelect()
		{
			lock(_object)
			{
				block = false;
				//this.DataBindingComplete -= SelectDataGrid_DataBindingComplete;
				//if(e.ListChangedType != System.ComponentModel.ListChangedType.Reset)
				//    return;
				if(KeyObject == null)
				{
					//this.block = false;
					if(KeyIndex > -1)
						SelectRow(KeyIndex);
					else
						ClearSelection();
				}
				else
					if(!SelectRow(KeyObject)&& KeyIndex > -1) SelectRow(KeyIndex);
				
			}
            Console.WriteLine("{0}: RestoreSelect", DateTime.Now.ToString("HH:mm:ss fff"));
		}

		#endregion

		#region ReturnCelltoNull

		#endregion

		#region Sorting

		/// <summary>
		/// Метод установки порядка сортировки данных в гриде по нескольким столбцам.
		/// </summary>
		public virtual void SetSort(string value)
		{
			if(!IsFine || Disposing || IsDisposed)
				return;
			if(ColumnCount <= 0)
				return;
			try
			{
				string[] _s;
				if(string.IsNullOrEmpty(value))
				{
					if(string.IsNullOrEmpty(KeyField) || !Columns.Contains(KeyField))
						return;
					_s = new[] { KeyField, "desc" };
				}
				else
					_s = value.ToLower().Replace("[", "").Replace("]", "").Split(new[] { ',', ' ' });

				byte _sortOrder = 1;
				for(int i = 0; i < _s.Length - 1 && _sortOrder < 6 && _sortOrder < ColumnCount; i++)
				{
					if(Columns.Contains(_s[i]) && Columns[_s[i]].Visible)
					{
						((DocsDataGridColumnHeaderCell)Columns[_s[i]].HeaderCell).SortOrder = _sortOrder;
						_sortOrder++;
						Columns[_s[i]].HeaderCell.SortGlyphDirection = _s[i + 1].Equals("asc")
																		   ? SortOrder.Ascending
																		   : SortOrder.Descending;
					}
				}

				//this.DataBindingComplete -= SelectDataGrid_DataBindingComplete;
				//this.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(SelectDataGrid_DataBindingComplete);
				MakeSortOrder();
				OnSorted(EventArgs.Empty);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
			Console.WriteLine("{0}: SetSort", DateTime.Now.ToString("HH:mm:ss fff"));
		}


		/// <summary>
		/// Получает текущий порядок сортировки данных в гриде по нескольким столбцам для сохранения в БД.
		/// </summary>
		public virtual string GetSort()
		{
			try
			{
				return GetDataView().Sort;
			}
			catch
			{
				return String.Empty;
			}
		}

		/// <summary>
		/// создание строки сортировки столбцов
		/// </summary>
		protected virtual void MakeSortOrder( bool save = false)
		{
			Console.WriteLine("{0}: MakeSortOrder  start", DateTime.Now.ToString("HH:mm:ss fff"));
			if(!IsFine || Disposing || IsDisposed || ColumnCount == 0)
				return;
			var dv = GetDataView();
			if(dv == null)
				return;
			StringBuilder _sb = new StringBuilder();
			try
			{
				for(byte _sortOrder = 1; _sortOrder < 6; _sortOrder++)
				{
					for(byte i = 0; i < Columns.Count; i++)
						if(Columns[i].Visible && ((DocsDataGridColumnHeaderCell)Columns[i].HeaderCell).SortOrder == _sortOrder)
						{
							if(_sb.Length > 0)
								_sb.Append(", ");

							_sb.Append(Columns[i].Name);
							_sb.Append(" ");
							_sb.Append(Columns[i].HeaderCell.SortGlyphDirection == SortOrder.Ascending ? "asc" : "desc");
							break;
						}
				}
				lock(_object)
				{
					dv.Sort = _sb.ToString();
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, string.Format("Текущая сортировка {0}, новая сортировка {1}", dv.Sort, _sb));
			}
			Console.WriteLine("{0}: MakeSortOrder  end", DateTime.Now.ToString("HH:mm:ss fff"));
		}

		protected virtual void SelectDataGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if(!IsFine || e.Button != MouseButtons.Left || e.Clicks != 1 || Disposing || IsDisposed)
				return;
			object keyObject = KeyObject;
			try
			{
				this.block = true;
				
				DocsDataGridColumnHeaderCell currentHeaderCell = (DocsDataGridColumnHeaderCell)Columns[e.ColumnIndex].HeaderCell;

				switch(currentHeaderCell.SortOrder)
				{
					case 0: // стобец в сортировке не участвовал ранее
						for(byte i = 0; i < Columns.Count; i++)
							if(Columns[i].Visible &&
									((DocsDataGridColumnHeaderCell)Columns[i].HeaderCell).SortOrder < 6
									&& ((DocsDataGridColumnHeaderCell)Columns[i].HeaderCell).SortOrder > 0)
								if(((DocsDataGridColumnHeaderCell)Columns[i].HeaderCell).SortOrder == 5)
								{
									((DocsDataGridColumnHeaderCell)Columns[i].HeaderCell).SortOrder = 0;
									Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
								}
								else
									((DocsDataGridColumnHeaderCell)Columns[i].HeaderCell).SortOrder++;
						currentHeaderCell.SortOrder = 1;
						currentHeaderCell.SortGlyphDirection = SortOrder.Descending;
						break;
					case 1: // поменяем направление сортировки
						currentHeaderCell.SortGlyphDirection = currentHeaderCell.SortGlyphDirection == SortOrder.Ascending
								? SortOrder.Descending
								: SortOrder.Ascending;
						break;
					default: // попали в столбец, который ранее был в очереди, но не первый
						byte _sortOrder = currentHeaderCell.SortOrder;
						for(byte i = 0; i < Columns.Count; i++)
							if(Columns[i].Visible
								&& ((DocsDataGridColumnHeaderCell)Columns[i].HeaderCell).SortOrder < _sortOrder
								&& ((DocsDataGridColumnHeaderCell)Columns[i].HeaderCell).SortOrder > 0)
							{
								((DocsDataGridColumnHeaderCell)Columns[i].HeaderCell).SortOrder++;
							}

						currentHeaderCell.SortOrder = 1;
						break;
				}
				this.DataBindingComplete -= SelectDataGrid_DataBindingComplete;
				this.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(SelectDataGrid_DataBindingComplete);

				MakeSortOrder( true);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
			finally
			{
				//if(this.DataBindings != null)
				//    this.GetDataView().EndInit();
				//if(keyObject == null)
				//{
				//    this.block = false;
				//    ClearSelection();
				//}
				//else
				//    SelectRow(keyObject);
				//this.block = false;

				if(VerticalScrollBar.Visible)
					VerticalScrollBar.Invalidate();
				if(HorizontalScrollBar.Visible)
					HorizontalScrollBar.Invalidate();
			}
		}

		#endregion

		public void SetSilent()
		{
			lock(_object)
			{
				if(!block)
					block = true;
			}
		}

		public void RemoveSilent()
		{
			lock(_object)
			{
				block = false;
			}
		}

		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			this.SuspendLayout();
			// 
			// SelectDataGrid
			// 
			this.AllowUserToAddRows = false;
			this.AllowUserToDeleteRows = false;
			this.AllowUserToResizeRows = false;
			this.AutoGenerateColumns = false;
			this.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.Dock = System.Windows.Forms.DockStyle.Fill;
			this.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.RowHeadersVisible = false;
			this.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.ShowCellErrors = false;
			this.ShowCellToolTips = false;
			this.ShowEditingIcon = false;
			this.ShowRowErrors = false;
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
			this.ResumeLayout(false);

		}
	}
}