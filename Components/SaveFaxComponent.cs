using System.ComponentModel;

namespace Kesco.Lib.Win.Document.Components
{
    public class SaveFaxComponent : SaveToArchiveComponent
    {
        internal int faxID;

        public SaveFaxComponent()
        {
        }

        public SaveFaxComponent(IContainer container, Controls.DocControl control)
            : base(container, control)
        {
        }

        public SaveFaxComponent(IContainer container) : base(container)
        {
        }

        internal override void OnDocumentSaved(DocumentSavedEventArgs doc)
        {
            if (doc.ImageID > 0 && faxID > 0)
                Environment.FaxData.CheckFax(faxID, doc.ImageID);

            doc.IsFax = true;
            base.OnDocumentSaved(doc);
        }


        public override bool IsFax()
        {
            return true;
        }

        public override int ID
        {
            get { return faxID; }
            set { faxID = value; }
        }
    }
}