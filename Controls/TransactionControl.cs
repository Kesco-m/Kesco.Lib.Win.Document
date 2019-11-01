using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Controls
{
	public class TransactionControl : AsyncUserControl
	{
		#region My param

		private int docID;
		private int maxWidth;
		private int curHeigth;
		private int curLabelHeigth;

		private ArrayList linkArray;

		private bool formOpen;

		private Options.Folder transFolder;
		private Options.Folder crFolder;

		#endregion

		private Label labelFin;
		private LinkLabel linkCreate;

		private Container components;

		public TransactionControl()
		{
			InitializeComponent();
			linkArray = new ArrayList();
			Height = 0;
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

		#region Component Designer generated code

		/// <summary>
		///   Required method for Designer support - do not modify the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			var resources =
				new System.ComponentModel.ComponentResourceManager(typeof(TransactionControl));
			this.labelFin = new System.Windows.Forms.Label();
			this.linkCreate = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// labelFin
			// 
			resources.ApplyResources(this.labelFin, "labelFin");
			this.labelFin.Name = "labelFin";
			// 
			// linkCreate
			// 
			resources.ApplyResources(this.linkCreate, "linkCreate");
			this.linkCreate.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
			this.linkCreate.Name = "linkCreate";
			this.linkCreate.Click += new System.EventHandler(this.linkCreate_Clicked);
			// 
			// TransactionControl
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.linkCreate);
			this.Controls.Add(this.labelFin);
			this.DoubleBuffered = true;
			this.Name = "TransactionControl";
			this.Load += new System.EventHandler(this.TransactionControl_Load);
			this.SizeChanged += new System.EventHandler(this.TransactionControl_SizeChanged);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		#region Accessors

		public int DocumentID
		{
			get { return docID; }
			set
			{
				if(docID != value)
				{
					formOpen = false;
					docID = value;
					Clear();
				}
				if(docID > 0)
				{
					curHeigth = labelFin.Height;
					curLabelHeigth = curHeigth;
					Height = curHeigth;
					maxWidth = labelFin.Width;
					Task<bool> task = Task.Factory.StartNew<bool>(new Func<object, bool>(CheckDoc), new object[ ] { docID, source.Token });
					Task t1 = Task.Factory.ContinueWhenAny<bool>(new Task<bool>[ ] { task }, new Action<Task<bool>>(EndCheckDoc));
					LoadTransaction(docID);
				}
			}
		}

		#endregion

		private void TransactionControl_Load(object sender, EventArgs e)
		{
		}

		private void EndCheckDoc(Task<bool> ta)
		{
			if(ta.Status != TaskStatus.RanToCompletion)
				return;
			var crTran = ta.Result;
			if(this.InvokeRequired)
				this.BeginInvoke((MethodInvoker)(() =>
					{
						Visible = crTran;
						linkCreate.Visible = crTran;
					}));
			else
			{
				Visible = crTran;
				linkCreate.Visible = crTran;
			}
		}

		private bool CheckDoc(object obj)
		{
			int docID = (int)((object[ ])obj)[0];
			CancellationToken ct = (CancellationToken)((object[ ])obj)[1];
			if(ct.IsCancellationRequested)
			{
				ct.ThrowIfCancellationRequested();
				return false;
			}
			bool createTr = false;
			int rule = Environment.DocData.CanCreateTransaction(docID, ct);
			if(ct.IsCancellationRequested)
			{
				ct.ThrowIfCancellationRequested();
				return false;
			}
			Console.WriteLine("{0}: fin = {1}", DateTime.Now.ToString("HH:mm:ss fff"), rule);
			if(rule > 0)
			{
				string personsIDs =  Environment.DocDataData.GetDocumentPersons(docID, ct);
				if(ct.IsCancellationRequested)
				{
					ct.ThrowIfCancellationRequested();
					return false;
				}
				createTr = Environment.EmpData.IsInRole(rule, personsIDs, true, ct);
			}
			if(ct.IsCancellationRequested)
			{
				ct.ThrowIfCancellationRequested();
				return false;
			}
			return createTr;
		}

		public void LoadTransaction(int docID)
		{
			if(formOpen)
				return;
			DeleteLabel();
			
			Task<DataTable> task = Task.Factory.StartNew<DataTable>(new Func<object, DataTable>(GetTable), new object[]{ docID, source.Token});
			Task t1 = Task.Factory.ContinueWhenAny<DataTable>(new Task<DataTable>[ ] { task }, new Action<Task<DataTable>>(ContinueLoadTransaction));
		}

		private void ContinueLoadTransaction(Task<DataTable> ta)
		{
			if(ta.Status != TaskStatus.RanToCompletion)
				return;
			var dt = ta.Result;
			if(dt != null)
			{
				if(this.InvokeRequired)
					this.BeginInvoke((MethodInvoker)(() => ShowTran(dt)));
				else
					ShowTran(dt);
			}
		}

		private void ShowTran(DataTable dt)
		{
			using(DataTableReader dr = dt.CreateDataReader())
			{
				while(dr.Read())
				{
					if(dr[Environment.TransactionData.IDField] != null &&
						!dr[Environment.TransactionData.IDField].Equals(DBNull.Value))
					{
						Visible = true;
						string labelText = "";

						//получаем коды транзакции 
						var id = (int)dr[Environment.TransactionData.IDField];
						// Проставляем текст транзакции
						labelText = dr[Environment.TransactionData.NameField].ToString();
						// добавляем готовый элемент в коллекцию
						CreateLabel(labelText, id);
					}
				}
				dr.Close();
				dr.Dispose();
				dt.Dispose();
			}
			SuspendLayout();
			AddingLabels((System.Windows.Forms.Control[ ])linkArray.ToArray(typeof(System.Windows.Forms.Control)));
			if(Width < maxWidth)
				Height = curHeigth + 17;
			else
				Height = curHeigth;
			ResumeLayout(false);
			if(curHeigth < curLabelHeigth)
				AutoScroll = true;
			Refresh();
		}

		private DataTable GetTable(object obj)
		{
			int docID = (int)((object[ ])obj)[0];
			CancellationToken ct = (CancellationToken)((object[ ])obj)[1];
		
			if(ct.IsCancellationRequested)
			{
					ct.ThrowIfCancellationRequested();
					return null;
			}
			return Environment.TransactionData.GetData(docID, ct);
		}

		private void AddingLabels(System.Windows.Forms.Control[] labels)
		{
			Controls.AddRange(labels);
		}

		private void TransactionControl_SizeChanged(object sender, EventArgs e)
		{
			SuspendLayout();
			Height = Width < maxWidth ? curHeigth + 17 : curHeigth;
			ResumeLayout(false);
		}

		private void CreateLabel(string labelText, int tranID)
		{
			SuspendLayout();
			var linkLabel = new LinkLabel
								{
									AutoSize = true,
									LinkBehavior = LinkBehavior.AlwaysUnderline,
									Location = new Point(0, curLabelHeigth),
									Name = "linkLabel" + tranID.ToString(),
									TabIndex = linkArray.Count + 2,
									TabStop = true,
									Text = labelText
								};
			// 
			// linkLabel
			// 
			string url = Environment.ShowTransactionString + tranID.ToString();
			linkLabel.Links.Add(0, labelText.Length, url).Tag = tranID;
			linkLabel.LinkClicked += linkLabel_LinkClicked;
			if(maxWidth < linkLabel.Width)
				maxWidth = linkLabel.Width;
			if(linkArray.Count < 7)
			{
				curHeigth += linkLabel.Size.Height;
				curLabelHeigth = curHeigth;
			}
			else
				curLabelHeigth += linkLabel.Size.Height;
			ResumeLayout(false);
			linkArray.Add(linkLabel);
		}

		private void Clear()
		{
			CancelOperation();
			Visible = false;
			Height = 0;
			linkCreate.Visible = false;
			DeleteLabel();
			maxWidth = 0;
			curHeigth = 0;
			curLabelHeigth = 0;
		}

		private void DeleteLabel()
		{
			for(int i = linkArray.Count; i > 0; i--)
			{
				var label = linkArray[i - 1] as LinkLabel;
				if(label == null)
					continue;
				linkArray.RemoveAt(i - 1);
				Controls.Remove(label);
				label.LinkClicked -= linkLabel_LinkClicked;
				label.Dispose();
			}
		}

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if(e.Link.Length <= 0)
				return;
			if(transFolder == null)
				transFolder = Environment.Layout.Folders.Add("Trans");
			int w = transFolder.LoadIntOption("Width", 640);
			int h = transFolder.LoadIntOption("Height", 460);
			Screen sr = null;
			sr = Screen.FromControl(this);
			if(sr != null)
			{
				Console.WriteLine("{0}: {1}",DateTime.Now.ToString("HH:mm:ss fff"), sr);
			
				if(sr.WorkingArea.Width < w)
					w = sr.WorkingArea.Width;
				if(sr.WorkingArea.Height < h)
					h = sr.WorkingArea.Height;
			}
			else
                Console.WriteLine("{0}: No Screen", DateTime.Now.ToString("HH:mm:ss fff"));
			Web.UrlBrowseDialog dialog = new Web.UrlBrowseDialog(e.Link.LinkData.ToString(),
										 Environment.StringResources.GetString("Control_TransactionControl_linkLabel_LinkClicked_Message1") + " ") { Tag = docID };
			dialog.DialogEvent += urlBrowseDialog_DialogEvent;
			formOpen = true;
			dialog.Show();
			dialog.Width = w;
			dialog.Height = h;
			if(sr.WorkingArea.Bottom < dialog.Bottom || dialog.Top < sr.WorkingArea.Top)
				dialog.Top = sr.WorkingArea.Top;
			if(sr.WorkingArea.Right < dialog.Right || dialog.Left < sr.WorkingArea.Left)
				dialog.Left = sr.WorkingArea.Left;
		}

		private void linkCreate_Clicked(object sender, EventArgs e)
		{
			Web.UrlBrowseDialog viewDialog = new Web.UrlBrowseDialog(Environment.CreateTransactionString + docID.ToString(),
									Environment.StringResources.GetString("Control_TransactionControl_Message1"));
			if(crFolder == null)
				crFolder = Environment.Layout.Folders.Add("TransCreate");
			int w = crFolder.LoadIntOption("Width", 640);
			int h = crFolder.LoadIntOption("Height", 460);
			Form fr = this.FindForm();
			Screen sr = null;
			if(fr != null)
			{
				sr = Screen.AllScreens.FirstOrDefault(x => x.Bounds.IntersectsWith(fr.Bounds));
				if(sr != null)
				{
					if(sr.WorkingArea.Width < w)
						w = sr.WorkingArea.Width;
					if(sr.WorkingArea.Height < h)
						h = sr.WorkingArea.Height;
				}
			}
			viewDialog.Tag = docID;
			viewDialog.DialogEvent += urlBrowseDialog_DialogEvent;
			formOpen = true;
			viewDialog.Show();
			viewDialog.Width = w;
			viewDialog.Height = h;
			if(sr.WorkingArea.Bottom < viewDialog.Bottom)
				viewDialog.Top = sr.WorkingArea.Top;
			if(sr.WorkingArea.Right < viewDialog.Right)
				viewDialog.Left = sr.WorkingArea.Left;
		}

		private void urlBrowseDialog_DialogEvent(object source, DialogEventArgs e)
		{
			formOpen = false;
			if(e.Dialog.Text == Environment.StringResources.GetString("Control_TransactionControl_Message1"))
			{
				crFolder.Option("Width").Value = e.Dialog.Width;
				crFolder.Option("Height").Value = e.Dialog.Height;
				crFolder.Save();
			}
			else
			{
				transFolder.Option("Width").Value = e.Dialog.Width;
				transFolder.Option("Height").Value = e.Dialog.Height;
				transFolder.Save();
			}
			if(docID.Equals(e.Dialog.Tag))
				LoadTransaction( docID);
		}
	}
}