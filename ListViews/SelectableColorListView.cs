using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.ListViews
{
    public class SelectableColorListView : ListView
    {
        public SelectableColorListView()
        {
            DoubleBuffered = true;
            SelectedForeGroundColor = Color.Orange;
            SelectedBackGroundColor = Color.LightGray;
            FullRowSelect = true;
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer, true);
            Scrollable = true;
        }

        [Browsable(true), Description("Устанавливает цвет фона выделенного итема")]
        public Color SelectedBackGroundColor { get; set; }

        [Browsable(true), Description("Устанавливает цвет текста выделенного итема")]
        public Color SelectedForeGroundColor { get; set; }

        [Browsable(true), Description("Включает выбор цвет текста выделенного итема")]
        public bool IsSelectedForeGroundColorEnable { get; set; }


        protected override void OnPaint(PaintEventArgs e)
        {
#if AdvancedLogging
			Log.Logger.EnterMethod(this, "OnPaint(PaintEventArgs e)");
#endif
            base.OnPaint(e);

            using (var sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Near;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                for (int n = 0; n < Items.Count; n++)
                {
                    ListViewItem lvitem = Items[n];
                    Rectangle rect = GetItemRect(n);
                    rect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
                    if (SelectedItems.Contains(lvitem))
                    {
                        var brushBg = new SolidBrush(SelectedBackGroundColor);
                        e.Graphics.FillRectangle(brushBg, rect);
                        SolidBrush brush = IsSelectedForeGroundColorEnable ? new SolidBrush(SelectedForeGroundColor) : new SolidBrush(lvitem.ForeColor);

                        e.Graphics.DrawString(lvitem.Text, lvitem.Font, brush, rect, sf);
                        brushBg.Dispose();
                        brush.Dispose();
                    }
                    else
                    {
                        var brush = new SolidBrush(lvitem.ForeColor);
                        e.Graphics.DrawString(lvitem.Text, lvitem.Font, brush, rect, sf);
                        brush.Dispose();
                    }
                }
            }
#if AdvancedLogging
			Log.Logger.LeaveMethod(this, "OnPaint(PaintEventArgs e)");
#endif
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (SelectedItems.Count > 0)
                Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            ListViewItem lvitem = GetItemAt(e.X, e.Y);
            if (SelectedItems.Count == 0)
                Invalidate();
            else if (lvitem != null)
                Invalidate(GetItemRect(Items.IndexOf(lvitem)));
        }
    }
}