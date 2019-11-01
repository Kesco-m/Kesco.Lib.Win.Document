using System.ComponentModel;

namespace Kesco.Lib.Win.Document.Components
{
    public class SaveFromScanerComponent : SaveToArchiveComponent
    {
        private string _filename;

        public SaveFromScanerComponent()
        {
        }

        public SaveFromScanerComponent(IContainer container, Controls.DocControl control)
            : base(container, control)
        {
        }

        public SaveFromScanerComponent(IContainer container) : base(container)
        {
        }

        public override bool IsScaner()
        {
            return true;
        }

        public override bool IsFromScaner()
        {
            return true;
        }

        public override void Save()
        {
            _filename = docControl.FileName;
            base.Save();
        }

        internal override void OnDocumentSaved(DocumentSavedEventArgs doc)
        {
            if (doc.DocID > 0 && doc.ImageID > 0)
            {
                docControl.FileName = "";
                Slave.DeleteFile(_filename);
                base.OnDocumentSaved(doc);
            }
        }

        public override string GetStringToSave()
        {
            return Environment.StringResources.GetString("ScanedDocument");
        }
    }
}