using System;
using iTextSharp.text;

namespace Kesco.Lib.Win.Document.Controls.PdfViewControl
{
    internal class PageInfo
    {
        internal PageInfo(Rectangle page_size, Rectangle page_size_with_rotation, int origin_rotate)
        {
            PageSize = OriginalSize(page_size);
            PageSizeWithRotation = OriginalSize(page_size_with_rotation);
            OriginalPdfSize = new System.Drawing.Rectangle(0, 0, (int) page_size_with_rotation.Width,
                                                 (int) page_size_with_rotation.Height);
            OriginalRotation = origin_rotate;
            CurrentRotate = 0;
            BeforeSaveRotate = 0;
        }

        internal int CurrentRotate { get; set; }
        internal System.Drawing.Rectangle PageSize { get; set; }
        internal System.Drawing.Rectangle PageSizeWithRotation { get; set; }
        internal System.Drawing.Rectangle OriginalPdfSize { get; set; }
        internal int OriginalRotation { get; set; }
        internal int BeforeSaveRotate { get; set; }
		internal bool Visible;

        private static System.Drawing.Rectangle OriginalSize(Rectangle size)
        {
            return new System.Drawing.Rectangle(0, 0, (int) Common.ScaleX(Math.Round((size.Width*Common.InchSm)/Common.PdfDpi)),
                                 (int) Common.ScaleY(Math.Round((size.Height*Common.InchSm)/Common.PdfDpi)));
        }
    }
}