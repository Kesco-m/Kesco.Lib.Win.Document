using System;
using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Grid
{
    public class DocsDataGridColumnHeaderCell : DataGridViewColumnHeaderCell
    {
        public byte SortOrder;

        public DocsDataGridColumnHeaderCell(string text)
        {
            Value = text;
            SortOrder = 0;
            Style.WrapMode = DataGridViewTriState.False;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                                      DataGridViewElementStates dataGridViewElementState, object value,
                                      object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
                                      DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            try
            {
                if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
                    graphics.FillRectangle(new SolidBrush(cellStyle.BackColor), cellBounds);

                if ((paintParts & DataGridViewPaintParts.Border) == DataGridViewPaintParts.Border)
                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);

                int w = cellBounds.Height/2;
                var textbrush = new SolidBrush(cellStyle.ForeColor);

                if (formattedValue is String)
                {
                    var textBounds = new Rectangle(cellBounds.X + 1,
                                                   cellBounds.Y + (cellBounds.Height - cellStyle.Font.Height)/2,
                                                   cellBounds.Width - w, cellStyle.Font.Height);
                    graphics.DrawString((value as string ?? string.Empty), cellStyle.Font, textbrush, textBounds);
                }

                if (SortOrder != 1 || SortGlyphDirection == System.Windows.Forms.SortOrder.None)
                    return;

                int h = (cellBounds.Height/3)*2;
                var glyphBounds = new Rectangle(cellBounds.X + cellBounds.Width - w,
                                                cellBounds.Y + cellBounds.Height - h + 2, w, h - 2);

                int i = h/4;
                var points = new Point[3];
                switch (SortGlyphDirection)
                {
                    case System.Windows.Forms.SortOrder.Descending:
                        points[0].X = glyphBounds.X + i;
                        points[0].Y = glyphBounds.Y + i;
                        points[1].X = glyphBounds.X + (i*3);
                        points[1].Y = glyphBounds.Y + i;
                        points[2].X = glyphBounds.X + (i*2);
                        points[2].Y = glyphBounds.Y + (i*3);
                        break;
                    default:
                        points[0].X = glyphBounds.X + i;
                        points[0].Y = glyphBounds.Y + (i*3);
                        points[1].X = glyphBounds.X + (i*2);
                        points[1].Y = glyphBounds.Y + i;
                        points[2].X = glyphBounds.X + (i*3);
                        points[2].Y = glyphBounds.Y + (i*3);
                        break;
                }

                graphics.FillPolygon(textbrush, points);
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex, "Error while painting a header cell");
            }
        }
    }
}
