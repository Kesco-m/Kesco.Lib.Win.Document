using System;
using System.ComponentModel;

namespace Kesco.Lib.Win.Document.Components
{
    public class DocControlComponent : System.ComponentModel.Component
    {
        internal ServerInfo serverInfo;

        public static event DocumentSavedEventHandle DocumentSaved;

        internal virtual void OnDocumentSaved(DocumentSavedEventArgs doc)
        {
            if (DocumentSaved != null)
                DocumentSaved.DynamicInvoke(new object[] {docControl, doc});
        }

        internal virtual void OnDocumentSaved(Controls.DocControl cont, DocumentSavedEventArgs doc)
        {
            if (DocumentSaved != null)
                DocumentSaved.DynamicInvoke(new object[] {cont, doc});
        }

        public static event FaxInContactCreatedEventHandle FaxInContactCreated;

        internal virtual void OnFaxInContactCreated(NogeIdEventArgs faxNodeId)
        {
            if (faxNodeId != null && faxNodeId.NodeID > 0 && FaxInContactCreated != null)
                FaxInContactCreated.DynamicInvoke(new object[] {docControl, faxNodeId});
        }

        internal virtual void CreateContactAfterFaxInSave(int faxId, int recipId, int docId)
        {
        }

        protected Controls.DocControl docControl;
        protected Container components;

        /// <summary>
        ///   Нулевой компонет контрола документа
        /// </summary>
        public DocControlComponent(IContainer container, Controls.DocControl docControl)
            : this(container)
        {
            this.docControl = docControl;
        }

        public DocControlComponent(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        public DocControlComponent()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                docControl = null;
                if (Container != null)
                    Container.Remove(this);
            }
        }

        #region Component Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion

        public bool IsOutlook { get; set; }

        public bool IsPdf { get; set; }

        public virtual bool IsDocument()
        {
            return false;
        }

        public virtual bool IsFax()
        {
            return false;
        }

        public virtual bool IsFaxIn()
        {
            return false;
        }

        public virtual bool IsFaxOut()
        {
            return false;
        }

        public virtual bool IsScaner()
        {
            return false;
        }

        public virtual bool IsFromScaner()
        {
            return false;
        }

        public virtual void Save()
        {
        }

        public virtual int ID
        {
            get { return 0; }
            set { }
        }

        public virtual int ImageID
        {
            get { return 0; }
            set { }
        }

        public virtual ServerInfo ServerInfo
        {
            get { return null; }
            set { }
        }

        public virtual string FileName
        {
            get { return null; }
            set { }
        }

        public virtual string GetStringToSave()
        {
            return null;
        }

        public virtual DateTime RetrieveScanDate()
        {
            return DateTime.MinValue;
        }

        public virtual Controls.DocControl Contr
        {
            get { return docControl; }
            set { docControl = value; }
        }
    }

    public class DocumentSavedEventArgs : EventArgs
    {
        private int docID;
        private int imageID;
        private bool goToDoc;
        private bool createEForm;
		private bool createSlaveEForm;
        private string fileName = string.Empty;
        private string docString = string.Empty;
        private Environment.ActionBefore beforeAct = Environment.ActionBefore.None;

        public DocumentSavedEventArgs(string fileName, string docString)
        {
            this.fileName = fileName;
            this.docString = docString;
        }

        public DocumentSavedEventArgs(int docID, int imageID)
        {
            this.docID = docID;
            this.imageID = imageID;
        }

		/// <summary>
		/// Обработка после создания документа
		/// </summary>
		/// <param name="docID"></param>
		/// <param name="imageID"></param>
		/// <param name="goToDoc"></param>
		/// <param name="createEForm"></param>
		/// <param name="createSlaveEForm"></param>
        public DocumentSavedEventArgs(int docID, int imageID, bool goToDoc, bool createEForm, bool createSlaveEForm) : this(docID, imageID)
        {
            this.goToDoc = goToDoc;
            this.createEForm = createEForm;
			this.createSlaveEForm = createSlaveEForm;
        }

        internal DocumentSavedEventArgs()
        {
        }

        public bool IsFax { get; set; }

        public int DocID
        {
            get { return docID; }
        }

        public string FileName
        {
            get { return fileName; }
        }

        public string DocString
        {
            get { return docString; }
        }

        internal void SetDocID(int docID)
        {
            this.docID = docID;
        }

        public int ImageID
        {
            get { return imageID; }
        }

        internal void SetImageID(int imageID)
        {
            this.imageID = imageID;
        }

        public bool GoToDoc
        {
            get { return goToDoc; }
        }

        public bool CreateEForm
        {
            get { return createEForm; }
        }

		public bool CreateSlaveEForm
		{
			get { return createSlaveEForm; }
		}

        internal void SetGoTo(bool goToDoc)
        {
            this.goToDoc = goToDoc;
        }

        internal void SetCreateEForm(bool createEForm)
        {
            this.createEForm = createEForm;
        }

        public bool IsSystemFolder { get; set; }

        public Environment.ActionBefore BeforeAct { get; set; }
    }

    // Вызываем добавление связи
    public class LinkDocEventArgs : EventArgs
    {
        private int docID;

        public LinkDocEventArgs(int docID)
        {
            this.docID = docID;
        }

        #region Accessors

        public int DocID
        {
            get { return docID; }
        }

        #endregion
    }


    public class NogeIdEventArgs : EventArgs
    {
        private int nodeID;

        public NogeIdEventArgs(int nodeId)
        {
            nodeID = nodeId;
        }

        public int NodeID
        {
            get { return nodeID; }
        }
    }

    // Делегат для добавления связи
    public delegate void LinkDocEventHandler(object source, LinkDocEventArgs e);

    public delegate void DocumentSavedEventHandle(object sender, DocumentSavedEventArgs e);

    public delegate void FaxInContactCreatedEventHandle(object sender, NogeIdEventArgs e);
}