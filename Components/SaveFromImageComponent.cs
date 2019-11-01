using System;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Components
{
    public class SaveFromImageComponent : DocControlComponent
    {
        public void Save(int imageID)
        {
            if (imageID > 0)
            {
                //if(Environment.DocImageData.ArchiveIDField()
                /*Dialog.AddDBDocDialog dialog = new Kesco.Document.Dialog.AddDBDocDialog(imageID);
				dialog.DialogEvent += new Kesco.Win.DialogEventHandler(dialog_DialogEvent);
				dialog.Show();*/
            }
        }

        private void dialog_DialogEvent(object source, DialogEventArgs e)
        {
            var dialog = e.Dialog as Dialogs.AddDBDocDialog;
            if (dialog != null)
                switch (dialog.DialogResult)
                {
                    case DialogResult.Yes:
                    case DialogResult.Retry:
                    case DialogResult.OK:
                        {
                            Console.Write("Document: " + dialog.DocID.ToString() + " Image: " + dialog.ImageID.ToString());
                            int docID = dialog.DocID;
                            int imageID = dialog.ImageID;
                            if (docID == 0 || imageID == 0)
                                return;
                            bool work = dialog.DialogResult != DialogResult.Yes;
                            if (work && dialog.AddToWork)
                            {
                                Console.WriteLine("{0}: Add to Work", DateTime.Now.ToString("HH:mm:ss fff"));
                                Environment.WorkDocData.AddDocToEmployee(docID, Environment.CurEmp.ID);
                            }
                            Console.WriteLine("{0}: event", DateTime.Now.ToString("HH:mm:ss fff"));
                            var doc = new DocumentSavedEventArgs(docID, imageID, work && dialog.GotoDoc,
                                                                 work && dialog.CreateEForm, dialog.CreateSlaveEForm);
                            Console.WriteLine("{0}: Send Message", DateTime.Now.ToString("HH:mm:ss fff"));
							if(work && dialog.SendMessage && !dialog.CreateSlaveEForm)
                            {
                                var senddialog = new Dialogs.SendMessageDialog(dialog.DocID, GetStringToSave(),
                                                                       !dialog.AddToWork)
                                                     {Owner = docControl.ParentForm};
                                senddialog.DialogEvent += SendMessageAfterSave_DialogEvent;
                                senddialog.Show();
                            }
                            else
                            {
                                Console.WriteLine("{0}: link", DateTime.Now.ToString("HH:mm:ss fff"));
                                docControl.OnLinkDoc(docID);
                            }
                            Console.WriteLine("{0}: Save event", DateTime.Now.ToString("HH:mm:ss fff"));
                            OnDocumentSaved(doc);
                        }
                        break;
                }
        }

        private void SendMessageAfterSave_DialogEvent(object source, DialogEventArgs e)
        {
            if (e.Dialog.DialogResult == DialogResult.OK)
                return;
            var dialog = e.Dialog as Dialogs.SendMessageDialog;
            if (dialog != null)
            {
                if (dialog.Forced)
                    foreach (int docID in dialog.DocIDs)
                        Environment.WorkDocData.AddDocToEmployee(docID, Environment.CurEmp.ID);

                foreach (int docID in dialog.DocIDs)
                    docControl.OnLinkDoc(docID);
            }
        }
    }
}