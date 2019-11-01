using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Win32;

namespace Kesco.Lib.Win.Document.Controls.PdfViewControl
{
    public class ListViewEx : ListView
    {
        private Brush brush;
        private Pen pen;

        public ListViewEx()
        {
            DoubleBuffered = true;
            OwnerDraw = true;
            MultiSelect = false;

            ListViewItemSorter = new ListViewItemComparer();
            brush = new SolidBrush(ForeColor);
            pen = new Pen(brush, 2);
        }

        #region Объявление констант, делегатов, событий

        private const int SB_ENDSCROLL = 8;

        public event EventHandler Scroll;

        protected void OnScroll()
        {
            if (!InvokeRequired)
            {
                if (Scroll != null)
                    Scroll(this, EventArgs.Empty);
            }
            else
                Invoke((MethodInvoker)(OnScroll));
        }

        #endregion

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            try
            {
                int param = m.WParam.ToInt32() & 0xFFFF;
                if ((m.Msg == (int) Msgs.WM_VSCROLL && param == SB_ENDSCROLL) ||
                    m.Msg == (int) Msgs.WM_MOUSEWHEEL)
                    OnScroll();
            }
            catch
            {
            }
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (LargeImageList == null || LargeImageList.Images.Count <= 0)
                return;
            Size image_size = LargeImageList.Images[e.ItemIndex].Size;
            Size image_size_real = LargeImageList.Images[e.ItemIndex].Size;
            if (!((ItemTag) e.Item.Tag).ItemSize.IsEmpty)
            {
                image_size_real = ((ItemTag) e.Item.Tag).ItemSize;
                e.Graphics.DrawImageUnscaled(LargeImageList.Images[e.Item.ImageIndex],
                                             e.Bounds.Left + (e.Bounds.Width - image_size.Width)/2,
                                             e.Bounds.Top + (e.Bounds.Height - image_size.Height)/2);
                if (e.Item.Selected)
                {
                    e.Graphics.DrawRectangle(pen, e.Bounds.Left + (e.Bounds.Width - image_size_real.Width)/2 - 2,
                                             e.Bounds.Top + (e.Bounds.Height - image_size_real.Height)/2 - 2,
                                             image_size_real.Width + 3, image_size_real.Height + 3);
                }
                e.Graphics.DrawString(e.Item.Text, Font, brush,
                                      e.Bounds.Left + (e.Bounds.Width - image_size_real.Width)/2 + 3,
                                      e.Bounds.Top + (e.Bounds.Height - image_size_real.Height)/2 +
                                      image_size_real.Height + 3);
            }
        }
    }

    internal class ListViewItemComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return ((ListViewItem) x).Index - ((ListViewItem) y).Index;
        }
    }

    public class ItemTag
    {
        public Size ItemSize = Size.Empty;

        private int _originNumber;

        public int OriginNumber
        {
            get { return _originNumber; }
            set { _originNumber = ((value > 0) ? value : 0); }
        }

        public ItemTag(int on)
        {
            _originNumber = (on > 0) ? on : 0;
        }
    }
}