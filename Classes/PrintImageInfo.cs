using System.Drawing;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document.Controls.PdfViewControl;
using Kesco.Lib.Win.MuPDFLib;

namespace Kesco.Lib.Win.Document.Classes
{
    public class PrintImageInfo : Tiff.PageInfo
    {
        private int page;
        private bool extPH;
        private PDFHelper ph = new PDFHelper();

        public PrintImageInfo()
        {
        }

        public PrintImageInfo(int page, string fileName, bool fileType)
        {
            this.page = page;
            this.fileName = fileName;
            this.fileType = fileType;
        }

        public PrintImageInfo(int page, string fileName, bool fileType, PDFHelper ph) : this(page, fileName, fileType)
        {
            if (ph != null)
            {
                extPH = true;
                this.ph = ph;
            }
        }

        public PrintImageInfo(int page, string fileName, bool fileType, StampItem[] stampsItems)
            : this(page, fileName, fileType)
        {
            StampsItems = stampsItems;
        }

        private string fileName;
        private bool fileType;

        public int ResX { get; set; }
        public int ResY { get; set; }
        public string Password { get; set; }

        public bool IsAnnuled { get; set; }

        public override Bitmap Image
        {
            get
            {
                if (image != null)
                    return image;
                if (fileType)
                {
                    if (page < 0)
                        return null;
                    if (!extPH)
                        ph.Open(fileName, Password);
                    ph.Page = page + 1;
                    try
                    {
                        if (ph.Width < 1 || ph.Height < 1 || ph.Width*ResX*ph.Height*ResY/5184.0 > 5000000 || ResX == 0 ||
                            ResY == 0)
                            return null;
                        else
                            return ph.GetBitmap((int) (ph.Width/72.0*ResX), (int) (ph.Height/72.0*ResY), ResX, ResY, 0,
                                                RenderType.RGB, false, false, 0);
                    }
                    finally
                    {
                        if (!extPH)
                            ph.Close();
                    }
                }
                else
                {
                    Tiff.PageInfo pi = Environment.LibTiff.GetImageFromTiff(fileName, page);
                    if (pi.Image != null)
                    {
                        image = pi.Image;
                        annotation = pi.Annotation;
                    }
                    return image;
                }
            }
            set { image = value; }
        }

        public Bitmap[] GetMatrix()
        {
            if (!extPH)
                ph.Open(fileName, Password);
            ph.Page = page + 1;
            try
            {
                return ph.GetBitmapMatrix((int) (ph.Width/72.0*ResX), (int) (ph.Height/72.0*ResY), ResX, ResY, 0,
                                          RenderType.RGB, false, false, 0);
            }
            finally
            {
                if (!extPH)
                    ph.Close();
            }
        }

        public int GetWidth()
        {
            if (image != null)
                return image.Width;
            if (fileType)
            {
                if (!extPH)
                    ph.Open(fileName, Password);
                ph.Page = page + 1;
                try
                {
                    return (int) ph.Width;
                }
                finally
                {
                    if (!extPH)
                        ph.Close();
                }
            }
            
            Tiff.PageInfo pi = Environment.LibTiff.GetImageFromTiff(fileName, page);
            if (pi.Image != null)
            {
                image = pi.Image;
                annotation = pi.Annotation;
            }
            return image != null ? image.Height : 0;
        }

        public int GetHeight()
        {
            if (image != null)
                return image.Height;
            if (fileType)
            {
                if (!extPH)
                    ph.Open(fileName, Password);
                ph.Page = page + 1;
                try
                {
                    return (int) ph.Height;
                }
                finally
                {
                    if (!extPH)
                        ph.Close();
                }
            }
            
            Tiff.PageInfo pi = Environment.LibTiff.GetImageFromTiff(fileName, page);
            if (pi.Image != null)
            {
                image = pi.Image;
                annotation = pi.Annotation;
            }
            return image != null ? image.Height : 0;
        }

        public StampItem[] StampsItems { get; set; }

        public override void Clear()
        {
            base.Clear();
            if (!extPH && ph != null)
            {
                ph.Close();
            }
            ph = null;
        }
    }
}