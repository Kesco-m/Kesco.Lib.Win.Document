using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Objects;

namespace Kesco.Lib.Win.Document.Components
{
	/// <summary>
	/// Базовый класс для сохранения в архив.
	/// </summary>
	public class SaveToArchiveComponent : DocControlComponent
	{
		private bool sendMail;
		private bool force;
		private SynchronizedCollection<int> parentDocIDs;
		private SynchronizedCollection<int> childDocIDs;
		private string sendString;
		protected SaveFaxInfo saveFaxInfo;

		public SaveToArchiveComponent()
		{
		}

		public SaveToArchiveComponent(IContainer container, Controls.DocControl control) : base(container, control)
		{
		}

		public SaveToArchiveComponent(IContainer container) : base(container)
		{
		}

		public override void Save()
		{
			if(docControl == null || !docControl.ImageDisplayed || ServerInfo == null)
				return;

			string oldFileName;
			TmpFile tf = null;
			// Добавил сохранения изменений изображения из факса.
			if(IsFax() && docControl.Modified)
			{
				oldFileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
				if(!docControl.SaveAs(oldFileName))
					return;
			}
			else
			{
				tf = Environment.GetTmpFileByKey(docControl.FileName);
				oldFileName = docControl.FileName;
				if(tf != null && tf.CurAct == Environment.ActionBefore.Save)
				{
					if(!File.Exists(tf.TmpFullName))
						docControl.SaveToTmpCopy(docControl.FileName, tf.TmpFullName, docControl.IsPDFMode);

					oldFileName = tf.TmpFullName;
				}
			}

			if(Environment.DocToSave.Contains(oldFileName))
			{
				Dialogs.AddDBDocDialog dialog = Environment.DocToSave[oldFileName] as Dialogs.AddDBDocDialog;
				if(dialog != null)
				{
					dialog.BringToFront();
					dialog.Activate();
				}
			}
			else
			{
				if(File.Exists(oldFileName))
				{
					Environment.DocToSave.Add(oldFileName, null);
					Form findForm = docControl.FindForm();
					if(findForm != null && findForm.InvokeRequired)
					{
						findForm.Invoke((MethodInvoker)(delegate
							{
								Dialogs.AddDBDocDialog dialog = new Dialogs.AddDBDocDialog(serverInfo, Dialogs.AddDBDocDialog.TypeSave.SaveAll,
																RetrieveScanDate(), IsFax(), ID, docControl.CurDocString,//GetStringToSave(),
																IsFax(), IsScaner(), oldFileName, saveFaxInfo, null,
																IsPdf, docControl.PageCount);
								Environment.DocToSave[oldFileName] = dialog;

								if(tf == null || tf.CurAct == Environment.ActionBefore.Save)
								{
									if(tf == null)
										tf = Environment.GetTmpFile(oldFileName);
									if(tf != null)
										tf.LinkCnt++;
								}

								dialog.DialogEvent += dialog_DialogEvent;
								dialog.Show();
							}));
					}
					else
					{
						Dialogs.AddDBDocDialog dialog = new Dialogs.AddDBDocDialog(serverInfo, Dialogs.AddDBDocDialog.TypeSave.SaveAll,
											   RetrieveScanDate(), IsFax(), ID,
											   docControl.CurDocString,//GetStringToSave(), 
											   IsFax(), IsScaner(),
											   oldFileName, saveFaxInfo, null, IsPdf,
											   docControl.PageCount);
						Environment.DocToSave[oldFileName] = dialog;

						if(tf == null || tf.CurAct == Environment.ActionBefore.Save)
						{
							if(tf == null)
								tf = Environment.GetTmpFile(oldFileName);
							if(tf != null)
								tf.LinkCnt++;
						}

						dialog.DialogEvent += dialog_DialogEvent;
						dialog.Show();
					}
				}
			}
		}

		public override ServerInfo ServerInfo
		{
			get { return serverInfo; }
			set { serverInfo = value; }
		}

		public override DateTime RetrieveScanDate()
		{
			string realFileName = docControl.FileName;

			if(File.Exists(realFileName))
				return File.GetCreationTimeUtc(realFileName);

			throw new Exception("RetrieveScanDate() Error");
		}

		private void dialog_DialogEvent(object source, DialogEventArgs e)
		{
			Dialogs.AddDBDocDialog dialog = e.Dialog as Dialogs.AddDBDocDialog;
			if(dialog == null)
				return;

			Document.Objects.TmpFile tf = null;
			if(!string.IsNullOrEmpty(dialog.OldFileName))
			{
				if(Environment.DocToSave.Contains(dialog.OldFileName))
					Environment.DocToSave.Remove(dialog.OldFileName);

				tf = Environment.GetTmpFileByValue(dialog.OldFileName);
			}
			switch(dialog.DialogResult)
			{
				case DialogResult.Yes:
				case DialogResult.Retry:
				case DialogResult.OK:
					{
						int docID = dialog.DocID;
						int imageID = dialog.ImageID;

                        Console.WriteLine("{0}: DocID: {1} ImageID: {2}", DateTime.Now.ToString("HH:mm:ss fff"), docID, imageID);
						if(docID > 0 && imageID > 0)
						{
							bool work = dialog.DialogResult != DialogResult.Yes;
							if(work && dialog.AddToWork)
							{
                                Console.WriteLine("{0}: Add to Work", DateTime.Now.ToString("HH:mm:ss fff"));
								Environment.WorkDocData.AddDocToEmployee(docID, Environment.CurEmp.ID);
							}
							DocumentSavedEventArgs doc = new DocumentSavedEventArgs(docID, imageID, work && dialog.GotoDoc, work && dialog.CreateEForm, dialog.CreateSlaveEForm);

							if(work)
							{
								force = !dialog.AddToWork;
								sendMail = dialog.SendMessage && !dialog.CreateSlaveEForm;
								parentDocIDs = dialog.ParentDocIDs;
								childDocIDs = dialog.ChildDocIDs;
								sendString = GetStringToSave();
								if(dialog.NeedOpenDoc)
									Environment.OnNewWindow(this, doc);
								Send(dialog.DocID);
							}
                            Console.WriteLine("{0}: Save event", DateTime.Now.ToString("HH:mm:ss fff"));
							OnDocumentSaved(doc);
                            Console.WriteLine("{0}: event", DateTime.Now.ToString("HH:mm:ss fff"));
						}
					}
					if(tf != null && tf.Window != null)
					{
						tf.AscBeforeClose = false;
						tf.Window.Close();
					}
					break;
			}

			if(tf != null)
			{
				tf.CurAct = Environment.ActionBefore.None;
				tf.LinkCnt--;
			}
		}

		private void SendMessageAfterSave_DialogEvent(object source, DialogEventArgs e)
		{
			if(e.Dialog.DialogResult == DialogResult.OK)
				return;
			var dialog = e.Dialog as Dialogs.SendMessageDialog;
			if(dialog != null && dialog.Forced)
				foreach(int docID in dialog.DocIDs)
					Environment.WorkDocData.AddDocToEmployee(docID, Environment.CurEmp.ID);
		}

		private void SendFromChild(int docID)
		{
			if(childDocIDs.Count > 0)
			{
				Console.WriteLine("{0}: Start link form", DateTime.Now.ToString("HH:mm:ss fff"));
				if(!Environment.DocLinksData.CheckDocLinkExists(docID, childDocIDs[0]))
					Environment.DocLinksData.AddDocLink(docID, childDocIDs[0]);
                Console.WriteLine("{0}: Parent: {1}, Child: {2}", DateTime.Now.ToString("HH:mm:ss fff"), docID, childDocIDs[0]);
				childDocIDs.RemoveAt(0);
				SendFromChild(docID);
			}
			else
			{
				if(sendMail)
				{
                    Console.WriteLine("{0}: Send Message", DateTime.Now.ToString("HH:mm:ss fff"));
					var senddialog = new Dialogs.SendMessageDialog(docID, sendString, force);
					senddialog.DialogEvent += SendMessageAfterSave_DialogEvent;
					senddialog.Show();
				}
			}
		}

		private void Send(int docID)
		{
			if(parentDocIDs.Count > 0)
			{
                Console.WriteLine("{0}: Start link form", DateTime.Now.ToString("HH:mm:ss fff"));
				if(!Environment.DocLinksData.CheckDocLinkExists(parentDocIDs[0], docID))
					Environment.DocLinksData.AddDocLink(parentDocIDs[0], docID);
                Console.WriteLine("{0}: Parent: {1}, Child: {2}", DateTime.Now.ToString("HH:mm:ss fff"), parentDocIDs[0], docID);
				parentDocIDs.RemoveAt(0);
				Send(docID);
			}
			else
			{
				SendFromChild(docID);
			}
		}
	}
}