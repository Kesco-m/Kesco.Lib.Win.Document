using System;
using System.ComponentModel;
using System.IO;

namespace Kesco.Lib.Win.Document.Components
{
    public class SaveScanerComponent : SaveToArchiveComponent
    {
        private string filename;

        public SaveScanerComponent()
        {
        }

        public SaveScanerComponent(IContainer container, Controls.DocControl control)
            : base(container, control)
        {
        }

        public SaveScanerComponent(IContainer container) : base(container)
        {
        }

        public override void Save()
        {
            filename = docControl.FileName;
            base.Save();
        }

        public override bool IsScaner()
        {
            return true;
        }

        public override string GetStringToSave()
        {
            var f = new FileInfo(filename);
            ScanInfo info = TextProcessor.ParseScanInfo(f);
            if (info == null)
            {
                DateTime date = f.CreationTime;
                info = new ScanInfo(date, "");
            }

            // doc type
            string s = Environment.StringResources.GetString("ScanDocument");

            // date
            s += " " + info.Date.ToString("dd.MM.yyyy");

            // description
            if (info.Descr.Length > 0)
                s = TextProcessor.StuffNewLine(s) + Environment.StringResources.GetString("Description") + ": " +
                    info.Descr;

            return s;
        }

        public override DateTime RetrieveScanDate()
        {
            if (File.Exists(docControl.FileName))
            {
                var f = new FileInfo(docControl.FileName);
                ScanInfo si = TextProcessor.ParseScanInfo(f);
                if (si == null)
                {
                    DateTime date = f.CreationTimeUtc;
                    return date;
                }
                return si.Date.ToUniversalTime();
            }
            throw new Exception("RetrieveScanDate() Error");
        }

        internal override void OnDocumentSaved(DocumentSavedEventArgs doc)
        {
            Controls.DocControl doccont = null;
            if (docControl != null)
            {
                doccont = docControl;
                docControl.FileName = "";
            }
            Slave.DeleteFile(filename);
            base.OnDocumentSaved(doccont, doc);
        }
    }
}