using System;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document
{
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class MouseEventMessageFilter : IMessageFilter
    {
        #region Singlton

        private static object locker = new object();
        private static MouseEventMessageFilter _instance;

        private MouseEventMessageFilter()
        {
        }

        public static MouseEventMessageFilter Instance
        {
            get
            {
                lock (locker)
                {
                    if (_instance == null)
                    {
                        _instance = new MouseEventMessageFilter();
                        Application.AddMessageFilter(_instance);
                    }

                    return _instance;
                }
            }
        }

        #endregion

        #region Events

        public event Action<IntPtr> MouseMove;
        public event Action MouseHover;
        public event Action MouseLeave;
        public event Action NCMouseLeave;

        private void OnMouseMove(IntPtr wParam)
        {
            if (MouseMove != null)
                MouseMove(wParam);
        }

        private void OnMouseHover()
        {
            if (MouseHover != null)
                MouseHover();
        }

        private void OnMouseLeave()
        {
            if (MouseLeave != null)
                MouseLeave();
        }

        private void OnNCMouseLeave()
        {
            if (NCMouseLeave != null)
                NCMouseLeave();
        }

        #endregion

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == (Int32) Win32.Msgs.WM_MOUSEMOVE)
            {
                OnMouseMove(m.WParam);
            }
            else if (m.Msg == (Int32) Win32.Msgs.WM_MOUSEHOVER)
            {
                OnMouseHover();
            }
            else if (m.Msg == (Int32) Win32.Msgs.WM_MOUSELEAVE)
            {
                OnMouseLeave();
            }
            else if (m.Msg == (Int32) Win32.Msgs.WM_NCMOUSELEAVE)
            {
                OnNCMouseLeave();
            }

            return false;
        }

        #endregion
    }
}