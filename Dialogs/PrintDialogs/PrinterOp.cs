using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Kesco.Lib.Win.Document.Dialogs
{
    /// <summary>
    ///   Working with printer
    /// </summary>
    public class PrinterOp : IDisposable
    {
        private string printerName;
        private Form form;
        private Hashtable printerStatus;
        private short oldOrientation;
        private short oldPaperSize;
        private short oldPaperLength;
        private short oldPaperWigth;

        private static DevMode dm;

        public PrinterOp(string printerName, Form form)
        {
            this.printerName = printerName;
            this.form = form;

            // инициализация статусов принтера
            printerStatus = new Hashtable
                                {
                                    {"1", Environment.StringResources.GetString("PrinterOp_Status1")},
                                    {"2", Environment.StringResources.GetString("PrinterOp_Status2")},
                                    {"3", Environment.StringResources.GetString("PrinterOp_Status3")},
                                    {"4", Environment.StringResources.GetString("PrinterOp_Status4")},
                                    {"5", Environment.StringResources.GetString("PrinterOp_Status5")},
                                    {"6", Environment.StringResources.GetString("PrinterOp_Status6")},
                                    {"7", Environment.StringResources.GetString("PrinterOp_Status7")}
                                };

            Application.ThreadException += Application_ThreadException;
        }

        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;

        private const int DMORIENT_PORTRAIT = 1;
        private const int DMORIENT_LANDSCAPE = 2;

        private const short DMPAPER_A4 = 9;
        private const short DMPAPER_A4_ROTATED = 77;
        private const short DMPAPER_USER = 256;

        private const int DMDUP_SIMPLEX = 1;
        private const int DMDUP_VERTICAL = 2;
        private const int DMDUP_HORIZONTAL = 3;

        [StructLayout(LayoutKind.Sequential)]
        private struct PrinterDefaults
        {
            public int pDatatype;
            public int pDevMode;
            public int DesiredAccess;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DevMode
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME - 1)] public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;
            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME - 1)] public string dmFormName;
            public short dmUnusedPadding;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
        }

        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const int PRINTER_ACCESS_USE = 0x8;
        private const int PRINTER_ACCESS_ADMINISTER = 0x4;

        private const int PRINTER_ALL_ACCESS =
            (STANDARD_RIGHTS_REQUIRED | PRINTER_ACCESS_ADMINISTER | PRINTER_ACCESS_USE);

        private const int PRINTER_USER_ACCESS = (PRINTER_ACCESS_USE);

        private const int DM_UPDATE = 1;
        private const int DM_COPY = 2;
        private const int DM_PROMPT = 4;
        private const int DM_MODIFY = 8;

        private const int DM_IN_BUFFER = DM_MODIFY;
        private const int DM_IN_PROMPT = DM_PROMPT;
        private const int DM_OUT_BUFFER = DM_COPY;
        private const int DM_OUT_DEFAULT = DM_UPDATE;

        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool OpenPrinter(
            [MarshalAs(UnmanagedType.LPTStr)] string pPrinterName,
            out IntPtr phPrinter,
            ref PrinterDefaults pDefaults
            );

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool ClosePrinter(
            IntPtr hPrinter
            );

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool GetPrinter(
            IntPtr hPrinter,
            int Level,
            IntPtr pPrinter,
            int cbBuf,
            out int pcbNeeded
            );

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool SetPrinter(
            IntPtr hPrinter,
            int Level,
            IntPtr pPrinter,
            int Command
            );

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool PrinterProperties(
            IntPtr hWnd,
            IntPtr hPrinter
            );

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern int AdvancedDocumentProperties(
            IntPtr hWnd,
            IntPtr hPrinter,
            [MarshalAs(UnmanagedType.LPTStr)] string pDeviceName,
            IntPtr pDevModeOutput,
            IntPtr pDevModeInput
            );

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern int DocumentProperties(
            IntPtr hWnd,
            IntPtr hPrinter,
            [MarshalAs(UnmanagedType.LPTStr)] string pDeviceName,
            IntPtr pDevModeOutput,
            IntPtr pDevModeInput,
            short fMode
            );

        [DllImport("printui.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern void PrintUIEntryW(IntPtr hwnd, IntPtr hinst, string lpszCmdLine, int nCmdShow);

        /// <summary>
        /// Открывает окно настроек принтера
        /// </summary>
        /// <param name="prnName">имя принтера</param>
        public void PrinterPropertiesPrintUI(string prnName)
        {
            PrintUIEntryW(form.Handle, IntPtr.Zero, @"/e /n " + "\"" + prnName + "\"", 0);
        }

        /// <summary>
        /// Открывает окно свойств принтера
        /// </summary>
        /// <param name="prnName">имя принтера</param>
        public void PrinterSystemPropertiesPrintUI(string prnName)
        {
            PrintUIEntryW(form.Handle, IntPtr.Zero, @"/p /n " + "\"" + prnName + "\"", 0);
        }

        public bool PrinterProperties()
        {
            var nullPtr = new IntPtr(0);
            IntPtr printerHandle = nullPtr;
            IntPtr bufPtr = nullPtr;

            PrinterDefaults pd;
            pd.pDatatype = 0;
            pd.pDevMode = 0;
            pd.DesiredAccess = PRINTER_USER_ACCESS;


            bool open = false;
            long result = 0;

            try
            {
                open = OpenPrinter(printerName, out printerHandle, ref pd);

                if (open)
                {
                    int nSize = DocumentProperties(form.Handle,
                                                   printerHandle, /* Handle to our printer. */
                                                   printerName, /* Name of the printer. */
                                                   IntPtr.Zero, /* Asking for size, so */
                                                   IntPtr.Zero, /* these are not used. */
                                                   0); /* Zero returns buffer size. */
                    bufPtr = Marshal.AllocHGlobal(nSize);

                    /*
                     * Step 2:
                     * Get the default DevMode for the printer and
                     * modify it for your needs.
                     */
                    result = DocumentProperties(form.Handle,
                                                printerHandle,
                                                printerName,
                                                bufPtr, /* The address of the buffer to fill. */
                                                IntPtr.Zero, /* Not using the input buffer. */
                                                DM_OUT_BUFFER); /* Have the output buffer filled. */
                    if (result < 1)
                    {
                        /* If failure, cleanup and return failure. */
                        Marshal.FreeHGlobal(bufPtr);
                        throw new Exception(
                            Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error1") +
                            "\n" + Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error2") + ": " +
                            Marshal.GetLastWin32Error());
                    }

                    dm = (DevMode) Marshal.PtrToStructure(bufPtr, typeof (DevMode));
                    Marshal.StructureToPtr(dm, bufPtr, true);

                    result = DocumentProperties(form.Handle,
                                                printerHandle,
                                                printerName,
                                                bufPtr, /* The address of the buffer to fill. */
                                                bufPtr, /* Not using the input buffer. */
                                                DM_IN_BUFFER | DM_OUT_BUFFER);

                    dm = (DevMode) Marshal.PtrToStructure(bufPtr, typeof (DevMode));

                    IntPtr tempPtr = Marshal.AllocHGlobal(bufPtr);
                    Marshal.WriteIntPtr(bufPtr, tempPtr);

                    SetPrinter(printerHandle, 9, tempPtr, 0);

                    Marshal.FreeHGlobal(tempPtr);
                    result = AdvancedDocumentProperties(form.Handle, printerHandle, printerName, bufPtr, bufPtr);

                    dm = (DevMode) Marshal.PtrToStructure(bufPtr, typeof (DevMode));
                    Marshal.StructureToPtr(dm, bufPtr, true);
                    Marshal.FreeHGlobal(bufPtr);
                }
                else
                    throw new Exception(
                        Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error3") + " " + printerName +
                        "\n" + Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error2") + ": " +
                        Marshal.GetLastWin32Error());
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
                MessageForm.Show(ex.Message, Environment.StringResources.GetString("Error"));
            }
            finally
            {
                if (open && printerHandle != nullPtr)
                    ClosePrinter(printerHandle);
            }

            return result > 0;
        }

        public PrinterSettings GetPS()
        {
            if (string.IsNullOrEmpty(dm.dmDeviceName))
                return null;
            var ps = new PrinterSettings();
            IntPtr ptr = IntPtr.Zero;
            Marshal.StructureToPtr(dm, ptr, true);
            ps.SetHdevmode(ptr);
            return ps;
        }

        public bool SetOrientation(Image imgEdit, PrintOrientation orientation, bool change,
                                   bool duplex, short copiesCount)
        {
            return SetOrientation(imgEdit, orientation, change, duplex, copiesCount, false, 0);
        }

        public bool SetOrientation(Image imgEdit, PrintOrientation orientation, bool change,
                                   bool duplex, short copiesCount, bool changeSize, short paperSize)
        {
            var nullPtr = new IntPtr(0);
            IntPtr printerHandle = nullPtr;
            IntPtr bufPtr = nullPtr;

            PrinterDefaults pd;
            pd.pDatatype = 0;
            pd.pDevMode = 0;
            pd.DesiredAccess = PRINTER_USER_ACCESS;

            const string preError = "\n\n";

            bool open = false;
            bool result = false;

            try
            {
                open = OpenPrinter(printerName, out printerHandle, ref pd);

                if (open)
                {
                    int nSize;
                    GetPrinter(printerHandle, 9, IntPtr.Zero, 0, out nSize);
                    if (nSize <= 4)
                    {
                        GetPrinter(printerHandle, 8, IntPtr.Zero, 0, out nSize);
                        if (nSize > 4)
                        {
                            bufPtr = Marshal.AllocHGlobal(nSize);
                            if (GetPrinter(printerHandle, 8, bufPtr, nSize, out nSize))
                            {
                                result = SetPrinter(printerHandle, 9, bufPtr, 0);

                                if (!result)
                                    throw new Exception(
                                        preError +
                                        Environment.StringResources.GetString("Error") + " SetPrinter()" +
                                        "\n" +
                                        Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error2") +
                                        ": " + Marshal.GetLastWin32Error());
                            }
                            else
                                throw new Exception(
                                    preError +
                                    Environment.StringResources.GetString("Error") + " GetPrinter()" +
                                    "\n" + Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error2") +
                                    ": " + Marshal.GetLastWin32Error());

                            Marshal.FreeHGlobal(bufPtr);
                            bufPtr = nullPtr;
                            GetPrinter(printerHandle, 9, IntPtr.Zero, 0, out nSize);
                        }
                    }

                    if (nSize > 4)
                    {
                        bufPtr = Marshal.AllocHGlobal(nSize);
                        if (GetPrinter(printerHandle, 9, bufPtr, nSize, out nSize))
                        {
                            IntPtr dmPtr = Marshal.ReadIntPtr(new IntPtr((int) bufPtr));

                            if ((int) dmPtr == 0)
                                throw new Exception(
                                    preError +
                                    Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error1") +
                                    "\n" + Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error2") +
                                    ": " + Marshal.GetLastWin32Error());

                            var _dm = (DevMode) Marshal.PtrToStructure(dmPtr, typeof (DevMode));

                            if (change)
                            {
                                if (changeSize)
                                {
                                    oldPaperSize = _dm.dmPaperSize;
                                    if (_dm.dmPaperSize != paperSize)
                                    {
                                        if (oldPaperSize == DMPAPER_USER)
                                        {
                                            oldPaperLength = _dm.dmPaperLength;
                                            oldPaperWigth = _dm.dmPaperWidth;
                                        }

                                        _dm.dmPaperSize = paperSize;
                                    }
                                }
                                oldOrientation = _dm.dmOrientation;
                                switch (orientation)
                                {
                                    case PrintOrientation.Book:
                                        if (_dm.dmOrientation != DMORIENT_PORTRAIT)
                                            _dm.dmOrientation = DMORIENT_PORTRAIT;
                                        break;

                                    case PrintOrientation.Album:
                                        if (_dm.dmOrientation != DMORIENT_LANDSCAPE)
                                            _dm.dmOrientation = DMORIENT_LANDSCAPE;
                                        break;

                                    case PrintOrientation.Auto:
                                        if (imgEdit.Height/imgEdit.VerticalResolution >=
                                            imgEdit.Width/imgEdit.HorizontalResolution)
                                            goto case PrintOrientation.Book;
                                        else
                                            goto case PrintOrientation.Album;
                                }

                                _dm.dmCopies = copiesCount;
                            }
                            else
                            {
                                if (changeSize)
                                {
                                    if (_dm.dmPaperSize != oldPaperSize)
                                    {
                                        _dm.dmPaperSize = oldPaperSize;
                                        if (oldPaperSize == DMPAPER_USER)
                                        {
                                            _dm.dmPaperLength = oldPaperLength;
                                            _dm.dmPaperWidth = oldPaperWigth;
                                        }
                                    }
                                }
                                if (_dm.dmOrientation != oldOrientation)
                                    _dm.dmOrientation = oldOrientation;
                                _dm.dmCopies = 1;
                            }
                            Marshal.StructureToPtr(_dm, dmPtr, true);
                            result = SetPrinter(printerHandle, 9, bufPtr, 0);
                            Marshal.FreeHGlobal(bufPtr);
                            bufPtr = nullPtr;

                            if (!result)
                                throw new Exception(
                                    preError +
                                    Environment.StringResources.GetString("Error") + " SetPrinter()" +
                                    "\n" + Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error2") +
                                    ": " + Marshal.GetLastWin32Error());
                        }
                        else
                            throw new Exception(
                                preError +
                                Environment.StringResources.GetString("Error") + " GetPrinter()" +
                                "\n" + Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error2") +
                                ": " + Marshal.GetLastWin32Error());
                    }
                }
                else
                    throw new Exception(
                        preError +
                        Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error3") + " " + printerName +
                        "\n" + Environment.StringResources.GetString("PrinterOp_PrinterProperties_Error2") + ": " +
                        Marshal.GetLastWin32Error());
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
                MessageForm.Show(ex.Message, Environment.StringResources.GetString("Error"));
            }
            finally
            {
                if (open && printerHandle != nullPtr)
                    ClosePrinter(printerHandle);
                if (bufPtr != nullPtr)
                    Marshal.FreeHGlobal(bufPtr);
            }

            return result;
        }

        public PrinterInfo GetInfo()
        {
            var info = new PrinterInfo
                           {
                               Name = printerName,
                               Comment = "",
                               DriverName = "",
                               Location = "",
                               PortName = "",
                               PrinterStatus = ""
                           };

            try
            {
                using (
                    var printObject = new ManagementObject("Win32_Printer.DeviceID='" + printerName + "'"))
                {
                    // тип (имя драйвера)
                    PropertyData propData = printObject.Properties["DriverName"];
                    if (propData.Value != null)
                        info.DriverName = propData.Value.ToString();

                    // имя порта
                    propData = printObject.Properties["PortName"];
                    if (propData.Value != null)
                        info.PortName = propData.Value.ToString();

                    // место
                    propData = printObject.Properties["Location"];
                    if (propData.Value != null)
                        info.Location = propData.Value.ToString();

                    if ((System.Environment.OSVersion.Version.Major == 5 &&
                         System.Environment.OSVersion.Version.Minor > 0) ||
                        System.Environment.OSVersion.Version.Major > 5)
                    {
                        // комментарий
                        propData = printObject.Properties["Comment"];
                        if (propData.Value != null)
                            info.Comment = propData.Value.ToString();
                    }

                    int resol = 0;
                    propData = printObject.Properties["HorizontalResolution"];
                    {
                        if (propData.Value != null && int.TryParse(propData.Value.ToString(), out resol))
                            info.HorizontalResolution = resol;
                        else
                            info.HorizontalResolution = 96;
                    }

                    propData = printObject.Properties["VerticalResolution"];
                    {
                        if (propData.Value != null && int.TryParse(propData.Value.ToString(), out resol))
                            info.VerticalResolution = resol;
                        else
                            info.VerticalResolution = 96;
                    }
                }
            }
            catch
            {
            }

            return info;
        }

        public static string GetPrinterName(string printerDriver)
        {
            try
            {
                using (
                    RegistryKey regKey =
                        Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                            "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Print\\Printers"))
                {
                    if (regKey != null)
                    {
                        foreach (string t in regKey.GetSubKeyNames())
                        {
                            using (RegistryKey printerKey = regKey.OpenSubKey(t))
                            {
                                if (printerKey == null)
                                    continue;
                                object obj = printerKey.GetValue("Printer Driver");
                                if (obj == null || !obj.Equals(printerDriver))
                                    continue;
                                obj = printerKey.GetValue("Name");
                                if (obj != null)
                                {
                                    return obj.ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex, printerDriver);
            }

            return string.Empty;
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Data.Env.WriteToLog(e.Exception);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Application.ThreadException -= Application_ThreadException;
            form = null;
        }

        #endregion
    }
}