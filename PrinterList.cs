using System;
using System.Collections;
using System.Drawing.Printing;

namespace Kesco.Lib.Win.Document
{
    public class PrinterList
    {
        public PrinterList()
        {
            List = new Hashtable();
        }

        #region Accessors

        public Hashtable List { get; private set; }
        public bool LoadStarted { get; private set; }
        public bool LoadComplete { get; private set; }

        #endregion

        public void Load()
        {
            if (List.Count > 0)
                List.Clear();

            // загрузка информации о принтерах
            LoadPrnInfoDelegate loadPrnInfo = LoadPrnInfo;
            loadPrnInfo.BeginInvoke(null, null);
        }

        private delegate void LoadPrnInfoDelegate();

        private void LoadPrnInfo()
        {
            LoadComplete = false;
            LoadStarted = true;

            Console.WriteLine("{0}: --> Printers load begin", DateTime.Now.ToString("HH:mm:ss fff"));

            foreach (string prnName in PrinterSettings.InstalledPrinters)
            {
                Console.WriteLine("{0}: ---> Printers load: {1}", DateTime.Now.ToString("HH:mm:ss fff"), prnName);

                var prn = new Dialogs.PrinterOp(prnName, null);
                Dialogs.PrinterInfo prnInfo = prn.GetInfo();

                List[prnName] = prnInfo;

                if (Updated != null)
                    Updated(this, new InfoArgs(prnInfo));
            }

            LoadComplete = true;
            LoadStarted = false;

            if (Loaded != null)
                Loaded(this, new EventArgs());

            Console.WriteLine("{0}: --> Printers load end", DateTime.Now.ToString("HH:mm:ss fff"));
        }

        public delegate void SimpleEventHandler(object source, EventArgs e);

        public event SimpleEventHandler Loaded;

        public delegate void InfoEventHandler(object source, InfoArgs e);

        public event InfoEventHandler Updated;
    }
}