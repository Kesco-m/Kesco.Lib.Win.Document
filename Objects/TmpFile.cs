using System;
using System.Windows.Forms;
using System.IO;

namespace Kesco.Lib.Win.Document.Objects
{
    public class TmpFile
    {
        public string originalName = "";
        public Int64 DocId = 0;
        public string DocString = "";
        private int linkCnt = 0;
        public string TmpFullName = "";
        public bool AscBeforeClose = true;
        public Form Window = null;
        public int LinkCnt
        {
            get { return linkCnt; }
            set
            {
                linkCnt = value;

                if (linkCnt <= 0)// || (linkCnt == 1 && IsInMain))
                {
                    if (!AscBeforeClose || !Environment.AnalyzeTmpFile((Window == null), this))
                        Environment.RemoveTmpFile(TmpFullName);
                    else
                    {
                        if (Window == null)
                            Environment.OnNewWindow(null, new Components.DocumentSavedEventArgs(TmpFullName, Path.GetFileName(originalName)));
                        else
                        {
                            Window.BringToFront();
                            Window.Activate();
                        }
                    }
                }
            }
        }

        private bool modified = false;
        public bool Modified
        {
            get { return modified; }
            set
            {
                modified = value;
                if (OnModified != null)
                    OnModified(this, null);
            }
        }
        public bool IsInMain = false;
        public Environment.ActionBefore CurAct = Environment.ActionBefore.None;

        public Classes.PDFHelper Phlp;
        public Lib.Win.Tiff.LibTiffHelper Thlp;
        public IntPtr TiffPtr = IntPtr.Zero;

        public int CurPage;
        //public int LastPage;
        public int PageCount;
        public ServerInfo SrvInfo;
        public bool IsPdf;

        public event EventHandler OnModified;

        public TmpFile(string original)
        {
            originalName = original;
        }

        public bool IsReadOnly = false;
    }
}