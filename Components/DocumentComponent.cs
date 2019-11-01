using System;
using System.ComponentModel;

namespace Kesco.Lib.Win.Document.Components
{
    public class DocumentComponent : DocControlComponent
    {
        private int imageID;

        public DocumentComponent()
        {
        }

        public DocumentComponent(IContainer container, Controls.DocControl control)
            : base(container, control)
        {
        }

        public DocumentComponent(IContainer container) : base(container)
        {
        }

        public override bool IsDocument()
        {
            return true;
        }

        public override int ID { get; set; }

        public override int ImageID
        {
            get { return imageID; }
            set { imageID = value; }
        }

        public override ServerInfo ServerInfo
        {
            get { return serverInfo; }
            set { serverInfo = value; }
        }

        public override void Save()
        {
            if (docControl.ImageDisplayed)
            {
//				if(System.Windows.Forms.MessageBox.Show("Сохранить изменения документа?","Подтверждение",System.Windows.Forms.MessageBoxButtons.YesNoCancel)==System.Windows.Forms.DialogResult.Yes)
//				{
////					control.Save();
//					return true;
//				}
            }
        }

        public override DateTime RetrieveScanDate()
        {
            return (DateTime) Environment.DocImageData.GetField(Environment.DocImageData.CreateDateField, imageID);
        }
    }
}