using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.PropertiesDialogs
{
    /// <summary>
    /// Диалог свойств файла изображени
    /// </summary>
    public partial class PropertiesImageDialog : Form
    {
        private string fullFileName;
        private string name;
        private int page;
        private bool filled;

        /// <summary>
        ///   Диалог свойств файла изображения
        /// </summary>
        /// <param name="fullFileName"> полный путь до файла </param>
        /// <param name="page"> Номер открытой страницы </param>
        public PropertiesImageDialog(string fullFileName, int page)
        {
            InitializeComponent();

            this.fullFileName = fullFileName;
            if (Environment.TmpFilesContains(fullFileName))
                this.fullFileName = Environment.GetTmpFileKey(fullFileName);
			if(string.IsNullOrEmpty(this.fullFileName))
				this.fullFileName = fullFileName;

            this.page = page;
        }

        /// <summary>
        ///   Диалог свойств файла изображения
        /// </summary>
        /// <param name="name"> замена теста имени файла </param>
        /// <param name="fullFileName"> полный путь до файла </param>
        /// <param name="page"> Номер открытой страницы </param>
        public PropertiesImageDialog(string name, string fullFileName, int page) : this(fullFileName, page)
        {
            this.name = name;
        }

        /// <summary>
        ///   Диалог свойств файла изображения
        /// </summary>
        /// <param name="fullFileName"> полный путь до файла </param>
        /// <param name="page"> Номер открытой страницы </param>
        /// <param name="resolution"> Разрешение изображения в формате "dpi x dpi" </param>
        /// <param name="size"> размер страницы в миллиметрах </param>
        /// <param name="format"> формат изображения </param>
        public PropertiesImageDialog(string fullFileName, int page, string resolution, string size, string format)
            : this(fullFileName, page)
        {
            filled = true;
            labelFormat.Text = "Tif";
            labelPageSize.Text = size;
            labelPageFormat.Text = format;
            labelResolution.Text = resolution;
        }

        /// <summary>
        ///   Диалог свойств файла изображения
        /// </summary>
        /// <param name="name"> замена теста имени файла </param>
        /// <param name="fullFileName"> полный путь до файла </param>
        /// <param name="page"> Номер открытой страницы </param>
        /// <param name="resolution"> Разрешение изображения в формате "dpi x dpi" </param>
        /// <param name="size"> размер страницы в миллиметрах </param>
        /// <param name="format"> формат изображения </param>
        public PropertiesImageDialog(string name, string fullFileName, int page, string resolution, string size,
                                     string format)
            : this(fullFileName, page, resolution, size, format)
        {
            this.name = name;
        }

        private void PropertiesImageDialog_Load(object sender, EventArgs e)
        {
            if (File.Exists(fullFileName))
            {
                var fi = new FileInfo(fullFileName);

                labelName.Text = string.IsNullOrEmpty(name) ? fi.Name : name;
                labelSize.Text = fi.Length.ToString();
                if (filled)
                    return;
                var bmp = new Bitmap(fullFileName);
                labelFormat.Text = bmp.RawFormat.ToString();
                if (bmp.RawFormat.Guid == ImageFormat.Tiff.Guid)
                {
                    labelFormat.Text = "Tiff";
                    Multupage(bmp);
                }
                else if (bmp.RawFormat.Guid == ImageFormat.Gif.Guid)
                {
                    labelFormat.Text = "Gif";
                    Multupage(bmp);
                }
                else if (bmp.RawFormat.Guid == ImageFormat.Bmp.Guid)
                {
                    labelFormat.Text = "Bmp";
                    Singlepage(bmp);
                }
                else if (bmp.RawFormat.Guid == ImageFormat.Emf.Guid)
                {
                    labelFormat.Text = "Emf";
                    Singlepage(bmp);
                }
                else if (bmp.RawFormat.Guid == ImageFormat.Exif.Guid)
                {
                    labelFormat.Text = "Exif";
                    Singlepage(bmp);
                }
                else if (bmp.RawFormat.Guid == ImageFormat.Icon.Guid)
                {
                    labelFormat.Text = "Icon";
                    Singlepage(bmp);
                }
                else if (bmp.RawFormat.Guid == ImageFormat.Jpeg.Guid)
                {
                    labelFormat.Text = "Jpeg";
                    Singlepage(bmp);
                }
                else if (bmp.RawFormat.Guid == ImageFormat.MemoryBmp.Guid)
                {
                    labelFormat.Text = "MemoryBmp";
                    Singlepage(bmp);
                }
                else if (bmp.RawFormat.Guid == ImageFormat.Png.Guid)
                {
                    labelFormat.Text = "Png";
                    Singlepage(bmp);
                }
                else if (bmp.RawFormat.Guid == ImageFormat.Wmf.Guid)
                {
                    labelFormat.Text = "Wmf";
                    Singlepage(bmp);
                }
                else
                {
                    labelFormat.Text = "Unknown";
                    Singlepage(bmp);
                }
            }
            else
                Close();
        }

        private void Singlepage(Bitmap bmp)
        {
            labelPageSize.Text = ((bmp.Height*2.54)/bmp.HorizontalResolution).ToString("N2") + "cm x " +
                                 ((bmp.Width*2.54)/bmp.VerticalResolution).ToString("N2") + "cm";
            labelPageFormat.Text = bmp.PixelFormat.ToString();
            labelResolution.Text = bmp.HorizontalResolution.ToString() + "x" + bmp.VerticalResolution.ToString();
        }

        private void Multupage(Bitmap bmp)
        {
            if (page > 0 && bmp.FrameDimensionsList != null)
            {
                var fd = new FrameDimension(bmp.FrameDimensionsList[0]);
                bmp.SelectActiveFrame(fd, page - 1);
            }
            labelPageSize.Text = ((bmp.Height*2.54)/bmp.HorizontalResolution).ToString("N2") + "cm x " +
                                 ((bmp.Width*2.54)/bmp.VerticalResolution).ToString("N2") + "cm";
            labelPageFormat.Text = bmp.PixelFormat.ToString();
            labelResolution.Text = bmp.HorizontalResolution.ToString() + "x" + bmp.VerticalResolution.ToString();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}