using System;

namespace Kesco.Lib.Win.Document
{
    public class InfoArgs : EventArgs
    {
        public InfoArgs(Dialogs.PrinterInfo info)
        {
            Info = info;
        }

        public Dialogs.PrinterInfo Info { get; private set; }
    }
}