using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using Kesco.Lib.Log;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Error;
using Kesco.Lib.Win.ImageControl;
using Kesco.Lib.Win.MuPDFLib;
using Kesco.Lib.Win.Tiff;

namespace Kesco.Lib.Win.Document.Controls.PdfViewControl
{
	public partial class PDFView : UserControl
	{
		private const int zoom_max = 1200, zoom_min = 2;
		private const int fake_zoom_max = ImageControl.ImageControl.MaxZoom, fake_zoom_min = ImageControl.ImageControl.MinZoom;

		private bool stampMode;

		public bool DSP { get; private set; }

		private static Dictionary<uint, string> Instances = new Dictionary<uint, string>();

		internal const int RenderLimit = 200;

		private Classes.PDFHelper pdfHelper;

		public bool CanSave;

		/// <summary>
		/// Словарь углов поворотов страниц изображения
		/// </summary>
		private readonly Classes.ImageVirtualRotationDictionary _pageVirtualRotationDictionary = new Classes.ImageVirtualRotationDictionary();

		public uint ID;

		public PDFView()
		{
			InitializeComponent();
			DoubleBuffered = true;

			NeedPreview = true;

			ID = Instances.Count > 0 ? Instances.Keys.Max() : 1;

			Load += PDFView_Load;
			pdfHelper = new Classes.PDFHelper();

			if(PageView != null)
				PageView.Zoom = _zoom;
		}

		void PDFView_Load(object sender, EventArgs e)
		{
			PagePreview.Scroll += PagePreview_Scroll;
			PagePreview.KeyUp += PagePreview_KeyUp;
			PagePreview.KeyDown += PagePreview_KeyDown;

			PagePreviewClientSizeChanged = PagePreview_ClientSizeChanged;
			PagePreview.ClientSizeChanged += PagePreviewClientSizeChanged;

			PageViewClientSizeChanged = PageView_ClientSizeChanged;
			Subscribe_PageView_ClientSizeChanged(true);
			PagePreviewSelectedIndexChanged = PagePreview_SelectedIndexChanged;
			PagePreview.SelectedIndexChanged += PagePreviewSelectedIndexChanged;

			PageView.AnnotationCreated += PageView_AnnotationCreated;
			PageView.NeedToReloadStamps += PageView_NeedToReloadStamps;
			PageView.StampsModified += new EventHandler(PageView_StampsModified);

			PagePreview.MouseDown += PagePreview_MouseDown;
			
			PageView.MouseHover += PageView_MouseHover;
			PageView._picture.MouseHover += _picture_MouseHover;
			PagePreview.MouseHover += PagePreview_MouseHover;
		}

		void PageView_StampsModified(object sender, EventArgs e)
		{
			modifiedStamp = PageView.ISStampModified();
		}

		#region Focus_in_StampMode

		private void _picture_MouseHover(object sender, EventArgs e)
		{
			if(!PageView.Focused && stampMode)
				PageView.Focus();
		}

		private void PagePreview_MouseHover(object sender, EventArgs e)
		{
			if(!PagePreview.Focused && stampMode)
				PagePreview.Focus();
		}

		private void PageView_MouseHover(object sender, EventArgs e)
		{
			if(!PageView.Focused && stampMode)
				PageView.Focus();
		}

		public void ReturnFocusToView()
		{
			if(!PageView.Focused)
				PageView.Focus();
		}

		#endregion

		private void PagePreview_MouseDown(object sender, MouseEventArgs e)
		{
			lastSelected = (PagePreview.SelectedItems.Count > 0) ? PagePreview.SelectedItems[0].Index : -1;
		}

		#region Объявление констант, делегатов, событий

		private const string tempFileExt = "_temp";

		public event ImageControl.ImageControl.FileNameChangedHandler FileNameChanged;
		public event EventHandler PageChanged;

		EventHandler PageViewClientSizeChanged;
		EventHandler PagePreviewSelectedIndexChanged;
		EventHandler PagePreviewClientSizeChanged;

		public event SaveEventHandler NeedSave;

		public event EventHandler ImageLoad;
		internal void OnImageLoad()
		{
			try
			{
				if(ImageLoad != null && ImageDisplayed)
					ImageLoad(this, new EventArgs());
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		public void OnFileNameChanged()
		{
			if(FileNameChanged != null)
			{
				FileNameChangedArgs arg = new FileNameChangedArgs { FileName = _file_name };
				FileNameChanged(this, arg);
			}
		}

		public event ImageControl.ImageControl.ToolSelectedHandler ToolSelected;
		#endregion

		#region Объявление структур, переменных

		private string _file_name = String.Empty;
		private bool _lock_item_selection_changed;
		private bool _lock_on_save_item_changing;
		private int _page = 1;
		private int _zoom = 100;

		private short _fitType = -1;

		private bool modified;
		private bool modifiedStamp;
		private bool pageMovied;
		private bool pageMoviedExtern;

		private Dictionary<int, PageInfo> DocPages;
		/// <summary>
		/// Обертка либтифа
		/// </summary>
		//internal static LibTiffHelper libTiff;

		public bool NeedPreview { get; set; }

		#endregion

		#region Accessors

		public int ImageResolutionX
		{
			get { return (int)Common.PdfDpi; }
		}
		public int ImageResolutionY
		{
			get { return (int)Common.PdfDpi; }
		}
		public int ImageHeight
		{
			get
			{
				if(DocPages != null && DocPages.Count > 0 && DocPages.ContainsKey(_page))
					switch((RotateFlipType)DocPages[_page].CurrentRotate)
					{
						case RotateFlipType.Rotate90FlipNone:
						case RotateFlipType.Rotate270FlipNone:
							return DocPages[_page].OriginalPdfSize.Width;
						default:
							return DocPages[_page].OriginalPdfSize.Height;
					}

				return 0;
			}
		}
		public int ImageWidth
		{
			get
			{
				if(DocPages != null && DocPages.Count > 0 && DocPages.ContainsKey(_page))
					switch((RotateFlipType)DocPages[_page].CurrentRotate)
					{
						case RotateFlipType.Rotate90FlipNone:
						case RotateFlipType.Rotate270FlipNone:
							return DocPages[_page].OriginalPdfSize.Height;
						default:
							return DocPages[_page].OriginalPdfSize.Width;
					}

				return 0;
			}
		}

		public int SplinterPlace
		{
			get { return Splitter.SplitterDistance; }
			set { Splitter.SplitterDistance = value; }
		}

		public Orientation Orientation
		{
			get { return Splitter.Orientation; }
			set { Splitter.Orientation = value; }
		}

		[Browsable(false)]
		public int ScrollPositionX
		{
			get { return (PageView != null ? PageView.ScrollHorizontal : 0); }
			set
			{
				if(PageView != null)
					PageView.ScrollHorizontal = (value < PageView.HorizontalScroll.Minimum ? PageView.HorizontalScroll.Minimum :
						(value > PageView.HorizontalScroll.Maximum ? PageView.HorizontalScroll.Maximum : value));
			}
		}

		[Browsable(false)]
		public int ScrollPositionY
		{
			get { return (PageView != null ? PageView.ScrollVertical : 0); }
			set
			{
				if(PageView != null)
					PageView.ScrollVertical = (value < PageView.VerticalScroll.Minimum ? PageView.VerticalScroll.Minimum :
							(value > PageView.VerticalScroll.Maximum ? PageView.VerticalScroll.Maximum : value));
			}
		}

		public bool UseLock
		{
			get { return pdfHelper.UseLock; }
			set { pdfHelper.UseLock = value; }
		}

		/// <summary>
		/// Значение Value скролинга относительно Max value (100% - 1, 0% - 0)
		/// </summary>
		public double ActualImageVerticalScrollValue
		{
			get
			{
				var max = ActualImageVerticalScrollMaxValue;

				if(max == 0)
				{
					if(PageView.scrollVertical.Value != 0)
						return 1;

					return 0;
				}

				return (double)PageView.scrollVertical.Value / max;
			}
		}

		/// <summary>
		/// Эффективная максимальная граница значения скролинга Value (min значение 0)
		/// </summary>
		public int ActualImageVerticalScrollMaxValue
		{
			get
			{
				return PageView.scrollVertical.Maximum - PageView.scrollVertical.LargeChange;
			}
		}

		/// <summary>
		/// Значение Value скролинга относительно Max value (100% - 1, 0% - 0)
		/// </summary>
		public double ActualImageHorisontallScrollValue
		{
			get
			{
				var max = ActualImageHorisontalScrollMaxValue;

				if(max == 0)
				{
					if(PageView.scrollHorizontal.Value != 0)
						return 1;

					return 0;
				}

				return (double)PageView.scrollHorizontal.Value / max;
			}
		}

		/// <summary>
		/// Эффективная максимальная граница значения скролинга Value (min значение 0)
		/// </summary>
		public int ActualImageHorisontalScrollMaxValue
		{
			get
			{
				return PageView.scrollHorizontal.Maximum - PageView.scrollHorizontal.LargeChange;
			}
		}

		#endregion

		/// <summary>
		/// Показывается ли изображение
		/// </summary>
		[Browsable(false)]
		public bool ImageDisplayed
		{
			get { return PageView != null && PageView.ImageDisplayed; }
		}

		/// <summary>
		/// Изображение изменено
		/// </summary>
		[Browsable(false)]
		public bool Modified
		{
			get { return modified || modifiedStamp || pageMovied; }
		}

		public bool RectDrawn()
		{
			return PageView.RectDrawn();
		}

		private void Clear()
		{
			if(Instances != null && Instances.ContainsKey(ID))
				Instances.Remove(ID);
			try
			{
				PageView.IsAnnuled = false;
				PagePreview.ClientSizeChanged -= PagePreviewClientSizeChanged;
				PagePreview.Items.Clear();
				if(PagePreviewImages != null)
					PagePreviewImages.Images.Clear();
				if(PageView.Image != null)
				{
					PageView.Image.Dispose();
					PageView.Image = null;
				}
				if(PageView.StampItems != null)
				{
					PageView.StampItems.Clear();
					PageView.StampItems = null;
				}

				PageView.CurrentDocID = 0;
				PageView.Invalidate();
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			buttonLoad.Visible = labelNoImage.Visible = false;

			_fi = null;
			filesData = DateTime.MinValue;
			filesLenght = 0;
			_file_name = "";
			//imageID = 0;
			PageCount = 0;
			_page = 0;
			modified = false;
			modifiedStamp = false;
			pageMovied = false;
			pageMoviedExtern = false;

			Visible = false;

			_filePass = "";
			_userPass = "";

			_lock_item_selection_changed = false;
			_lock_on_save_item_changing = false;
		}

		/// <summary>
		/// Информация о файле
		/// </summary>
		[Browsable(false)]
		private FileInfo _fi;

		[Browsable(false)]
		private DateTime filesData;
		[Browsable(false)]
		private long filesLenght;

		[Browsable(false)]
		private string _filePass = "";
		[Browsable(false)]
		private string _userPass = "";

		/// <summary>
		/// Принудительное обновление
		/// </summary>
		internal bool ForceReload;

		[Browsable(false)]
		public string FileName
		{
			get
			{
				return _file_name;
			}
			set
			{
				SaveSinc.Set();
				_lock_item_selection_changed = false;
				_lock_on_save_item_changing = false;

				pdfHelper.Close();
				if(string.IsNullOrEmpty(value))
				{
					if(string.IsNullOrEmpty(_file_name))
						return;
				}

				if(!string.IsNullOrEmpty(_file_name) && _file_name.Equals(value, StringComparison.CurrentCultureIgnoreCase))
				{
					if(File.Exists(value))
					{
						pdfHelper.Open(_file_name, _filePass);
						FileInfo tempfi = new FileInfo(value);

						if(_fi != null && tempfi.Length == filesLenght && tempfi.LastWriteTime == filesData && !ForceReload)
							return;
					}
				}

				Cursor = Cursors.WaitCursor;
				try
				{
					Clear();

					if(string.IsNullOrEmpty(value))
						_file_name = "";
					else if(File.Exists(value))
						_file_name = value;

					if(_file_name != "")
					{
						Visible = true;
						if(FileOpen(_file_name) != "")
						{
							_fi = new FileInfo(_file_name);
							filesData = _fi.LastWriteTime;
							filesLenght = _fi.Length;

							if(NeedPreview)
							{
								OnFileNameChanged();

								Page = 1;
								SelectTool(stampMode ? 9 : 0);
								OnImageLoad();
							}
							else
								_page = 1;
						}
						else
						{
							_file_name = "";
							OnFileNameChanged();
						}
					}
				}
				finally
				{
					Cursor = Cursors.Default;
				}
			}
		}

		private bool _forcePage;

		public int Page
		{
			get
			{
				return _page;
			}
			set
			{
				PageView.HorizontalScroll.Value = 0;
				PageView.VerticalScroll.Value = 0;

				if(value <= 0)
				{
					_page = 0;
					return;
				}

				if(PagePreview.Items.Count == 0)
					return;
				try
				{
					_lock_item_selection_changed = true;

					Cursor = Cursors.WaitCursor;

					int p = Math.Min(PagePreview.Items.Count, (value > 0 ? value : 1));
					if(_page != p || _forcePage)
					{
						_page = p;

						RefreshPagePreview(true);
						RefreshPageView();

						PageView.Page = _page;
						if(!isFromSelChange)
							PagePreview.Items[_page - 1].Selected = true;

						SelectTool(0);

						PageView.IsRefreshBitmap = true;

						if(PageChanged != null)
							PageChanged(this, EventArgs.Empty);

						pageMovied = pageMoviedExtern;
					}
				}
				finally
				{
					_lock_item_selection_changed = false;

					_forcePage = false;
					Cursor = Cursors.Default;
					PageView.Focus();
				}
			}
		}

		[Browsable(false)]
		public int PageCount { get; private set; }

		#region Print

		public void Print(ImageControl.ImageControl.PrintActionHandler handler, int docID, int id, string docString, short copyCount)
		{
			PrintAction(handler, true, _file_name, docID, id, 1, PageCount, PageCount, docString, copyCount);
		}

		public void PrintPage(ImageControl.ImageControl.PrintActionHandler handler, int docID, int id, int page, string docString, short copyCount)
		{
			PrintAction(handler, true, _file_name, docID, id, page, page, PageCount, docString, copyCount);
		}

		private void PrintSelection(ImageControl.ImageControl.PrintActionHandler handler, string filesName, int id, int page, string docString, short copyCount)
		{
			PrintAction(handler, true, filesName, 0, id, page, page, 1, docString, copyCount);
		}

		public void PrintSelection(ImageControl.ImageControl.PrintActionHandler handler, string docString, short copyCount)
		{
			try
			{
				string tempImage = CreateSelectedImage();
				if(tempImage.Length > 0)
					PrintSelection(handler, tempImage, -1 /*id*/, 1, docString, copyCount);
			}
			catch(Exception ex)
			{
				OnErrorMessage(ex);
			}
		}

		private void PrintAction(ImageControl.ImageControl.PrintActionHandler handler, bool print, string fileName, int docID, int id, int startPage, int endPage, int pCount, string docName, short copyCount)
		{
			if(handler != null)
				handler(print, docID, id, fileName, startPage, endPage, pCount, docName, copyCount);
			else
			{
				List<Tiff.PageInfo> printImages = pdfHelper.GetBitmapsCollectionFromFile(fileName, startPage, endPage, true);
				if(id > 0)
				{
					List<StampItem> stampItems = Environment.DocSignatureData.GetStamps(imageID);

					if(stampItems != null && stampItems.Count > 0)
					{
						for(int i = 0; i < printImages.Count; i++)
						{
							printImages[i] = new Classes.PrintImageInfo { Image = printImages[i].Image, StampsItems = stampItems.FindAll(x => x.Page == startPage + i + 1).ToArray() };
						}
					}
				}
				PrintImage printImageInst = new PrintImage();

				printImageInst.PrintPage(printImages.ToArray(), null, startPage, endPage, 1, PrintOrientation.Auto, true, copyCount, (new PrinterSettings()).PrinterName, null, null);
			}
		}

		private string CreateSelectedImage()
		{
			Bitmap il = null;
			try
			{
				il = CreateImageListFromSelectedImage();
				if(il == null)
					return "";

				string tempImage = Path.GetTempPath() + "~" + Guid.NewGuid().ToString() + ".pdf";
				using(FileStream file_stream = new FileStream(tempImage, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					using(iTextSharp.text.Document document = new iTextSharp.text.Document())
					{
						using(PdfWriter pdfWriter = PdfWriter.GetInstance(document, file_stream))
						{
							document.Open();

							bool needRotate = il.Width > il.Height;
							if(needRotate)
								il.RotateFlip(RotateFlipType.Rotate90FlipNone);

							iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(il, ImageFormat.Png);
							gif.ScaleAbsolute((float)(il.Width * (72.0 / il.HorizontalResolution)), (float)(il.Height * (72.0 / il.VerticalResolution)));
							gif.SetAbsolutePosition(1, 1);

							document.SetPageSize(new iTextSharp.text.Rectangle(gif.ScaledWidth, gif.ScaledHeight));
							document.NewPage();

							pdfWriter.DirectContent.AddImage(gif);

							if(needRotate)
								pdfWriter.AddPageDictEntry(PdfName.ROTATE, new PdfNumber(270));

							document.Close();
						}
					}
				}
				return tempImage;
			}
			catch(Exception ex)
			{
				OnErrorMessage(new Exception("CreateSelectedImage error", ex));
			}
			finally
			{
				if(il != null)
					il.Dispose();
			}

			return "";
		}


		#endregion

		#region Rotate

		/// <summary>
		/// Угол виртуального поворота
		/// </summary>
		public void SetVirtualRotation()
		{
			_pageVirtualRotationDictionary.SaveVirtualRotation(imageID, _page, CurrentRotate);

			DocPages[Page].CurrentRotate = 0;

			_lock_on_save_item_changing = false;
			modified = false;
		}

		public void RotateLeft()
		{
			if(!_lock_on_save_item_changing)
			{
				if(PageView.StampItems != null)
				{
					int angle = 270;
					double cZ = (Zoom > PDFView.RenderLimit && PageView.FitType == -1 ? 1 : (Zoom / 100.0));
					foreach(var stamp in PageView.StampItems)
					{
						// Старая логика DSP if (Page == 1 && stampItems != null && stampItems.Count == 1 && stampItems[0].TypeID == 101)
						// Реализация старой логики
						if(stamp.StmpItem.Page != Page)
							continue;

						Image stampImage;
						bool dsp = false;

						if(stamp.StmpItem.TypeID == 101)
						{
							stampImage = Environment.GetDSP();
							dsp = true;
						}
						else
							stampImage = Environment.GetStamp(stamp.StmpItem.StampID, stamp.StmpItem.ImageID);

						int X = stamp.StmpItem.X;

						stamp.StmpItem.X = (int)(stamp.StmpItem.Y - (stampImage.Width * stamp.StmpItem.Zoom  / 200.0) + (stampImage.Height * stamp.StmpItem.Zoom  / 200.0));
						stamp.StmpItem.Y = (int)(PageView.OriginalBitmap.Width / cZ - X - (stampImage.Height * stamp.StmpItem.Zoom / 200.0) - (stampImage.Width * stamp.StmpItem.Zoom / 200.0));

						if(dsp)
						{
							if(stamp.StmpItem.X < 1)
								stamp.StmpItem.X = 1;
							if(stamp.StmpItem.X > PageView.OriginalBitmap.Height - (int)(stampImage.Width * cZ * PageView.OriginalBitmap.HorizontalResolution / stampImage.HorizontalResolution) - 1)
								stamp.StmpItem.X = PageView.OriginalBitmap.Height - (int)(stampImage.Width * cZ * stamp.StmpItem.Zoom / 200.0) - 1;

							if(stamp.StmpItem.Y < 1)
								stamp.StmpItem.Y = 1;

							if(stamp.StmpItem.Y > PageView.OriginalBitmap.Width - (int)(stampImage.Height * cZ * stamp.StmpItem.Zoom / 200.0) - 1)
								stamp.StmpItem.Y = PageView.OriginalBitmap.Width - (int)(stampImage.Height * cZ * stamp.StmpItem.Zoom / 200.0) - 1;
						}
						// Применяю угол поворота
						if(!dsp)
							stamp.StmpItem.Rotate += angle;

						// Привожу значение в диапазон -359 -- +359 градусов
						stamp.StmpItem.Rotate = stamp.StmpItem.Rotate % 360;
					}
				}

				Rotate(_page, RotateFlipType.Rotate270FlipNone);
			}
		}

		public void RotateRight()
		{
			if(!_lock_on_save_item_changing)
			{
				if(PageView.StampItems != null)
				{
					int angle = 90;
					double cZ = (Zoom > PDFView.RenderLimit && PageView.FitType == -1 ? 1 : (Zoom / 100.0));
					foreach(var stamp in PageView.StampItems)
					{
						// Старая логика DSP if (Page == 1 && stampItems != null && stampItems.Count == 1 && stampItems[0].TypeID == 101)
						// Реализация старой логики
						if(stamp.StmpItem.Page != Page)
							continue;

						Image stampImage;
						bool dsp = false;

						if(stamp.StmpItem.TypeID == 101)
						{
							stampImage = Environment.GetDSP();
							dsp = true;
						}
						else
							stampImage = Environment.GetStamp(stamp.StmpItem.StampID, stamp.StmpItem.ImageID);

						int X = stamp.StmpItem.X;

						stamp.StmpItem.X = (int)((PageView.OriginalBitmap.Height / cZ - stamp.StmpItem.Y  - (stampImage.Height * stamp.StmpItem.Zoom / 200.0) - stampImage.Width * stamp.StmpItem.Zoom / 200.0));
						stamp.StmpItem.Y = (int)(X - (stampImage.Height * stamp.StmpItem.Zoom /  200.0) + stampImage.Width * stamp.StmpItem.Zoom / 200.0);

						if(dsp)
						{
							if(stamp.StmpItem.X < 1)
								stamp.StmpItem.X = 1;
							if(stamp.StmpItem.X > PageView.OriginalBitmap.Height / cZ - (int)(stampImage.Width * PageView.OriginalBitmap.HorizontalResolution / stampImage.HorizontalResolution) - 1)
								stamp.StmpItem.X = (int)(PageView.OriginalBitmap.Height / cZ - stampImage.Width * stamp.StmpItem.Zoom / 200.0 - 1);

							if(stamp.StmpItem.Y < 1)
								stamp.StmpItem.Y = 1;

							if(stamp.StmpItem.Y > PageView.OriginalBitmap.Width / cZ - (int)(stampImage.Height * stamp.StmpItem.Zoom / 200.0) - 1)
								stamp.StmpItem.Y = (int)(PageView.OriginalBitmap.Width / cZ - stampImage.Height * stamp.StmpItem.Zoom / 200.0 - 1);
						}
						// Применяю угол поворота
						if(!dsp)
							stamp.StmpItem.Rotate += angle;

						// Привожу значение в диапазон -359 -- +359 градусов
						stamp.StmpItem.Rotate = stamp.StmpItem.Rotate % 360;
					}
				}

				Rotate(_page, RotateFlipType.Rotate90FlipNone);
			}
		}

		private void Rotate(int pageNum, RotateFlipType rotate_type)
		{
			int number = Math.Max(0, pageNum - 1);
			ItemTag Origin = (ItemTag)PagePreview.Items[number].Tag;
			Image thumbnail = PagePreviewImages.Images[PagePreview.Items[number].ImageIndex];

			RotateImage(thumbnail, rotate_type);

			int angle = DocPages[Origin.OriginNumber /*pageNum*/].CurrentRotate;
			angle += (int)rotate_type;
			angle %= 4;

			DocPages[Origin.OriginNumber /*pageNum*/].CurrentRotate = angle;

			modified = angle != 0;

			if(!modified)
			{
				_pageVirtualRotationDictionary.UpdatePageRotation(imageID, Page - 1);
				var rotation = _pageVirtualRotationDictionary.GetPageRotation(imageID, Page - 1);

				if(rotation != 0)
					modified = true;
			}

			switch(rotate_type)
			{
				case RotateFlipType.Rotate90FlipNone:
				case RotateFlipType.Rotate270FlipNone:
					Size size = Origin.ItemSize;
					((ItemTag)PagePreview.Items[number].Tag).ItemSize = new Size(size.Height, size.Width);
					break;
			}

			PagePreviewImages.Images[PagePreview.Items[number].ImageIndex] = thumbnail;
			if(ShowThumbPanel)
				PagePreview.RedrawItems(number, number, true);

			if(pageNum != _page)
				return;

			if(_fitType != -1)
				FitTo(_fitType, true, true);
			else
			{
				Image page = (Image)PageView.OriginalBitmap.Clone();
				RotateImage(page, rotate_type);
				PageView.Image = page;
			}

			PageView.Zoom = _zoom;
			PageView.IsRefreshBitmap = true;
		}

		private void RotateImage(Image image, RotateFlipType rotate_type)
		{
			switch(rotate_type)
			{
				case RotateFlipType.Rotate90FlipNone:
					image.RotateFlip(RotateFlipType.Rotate90FlipNone);
					break;
				case RotateFlipType.Rotate180FlipNone:
					image.RotateFlip(RotateFlipType.Rotate180FlipNone);
					break;
				case RotateFlipType.Rotate270FlipNone:
					image.RotateFlip(RotateFlipType.Rotate270FlipNone);
					break;
			}
		}

		/// <summary>
		/// Синхронизация виртуальных поворотов
		/// </summary>
		public void SyncImagePageRotations()
		{
			for(int i = 0; i < PageCount; i++)
			{
				var transition = _pageVirtualRotationDictionary.SyncPageRotation(imageID, i);

				if(transition.Changed)
					RefreshPage(i);
			}
		}

		/// <summary>
		/// Обновить страницу изображения
		/// </summary>
		/// <param name="page"></param>
		private void RefreshPage(int page)
		{
			try
			{
				RefreshPagePreviewItem(page);

				if(ShowThumbPanel)
					PagePreview.RedrawItems(page, page, true);

				if(page == Page - 1)
				{
					RefreshPageView();
					PageView.IsRefreshBitmap = true;
					PageView.Refresh();
				}
			}
			catch(Exception ex)
			{
				LibTiffHelper.WriteToLog(ex);
			}
		}

		#endregion

		#region Save

		/// <summary>
		/// Исп-ся для сохранения изм-ий в изобр-ях вне Архива
		/// </summary>
		public void Save()
		{
			FileAttributes fa = FileAttributes.Normal;
			if(File.Exists(_file_name))
			{
				if(((int)File.GetAttributes(_file_name) & (int)FileAttributes.ReadOnly) == (int)FileAttributes.ReadOnly)
				{
					try
					{
						fa = File.GetAttributes(_file_name);
						File.SetAttributes(_file_name, FileAttributes.Normal);
					}
					catch(Exception ex)
					{
						ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("DocControl_Page_Error1"));
						try { ChangePage(new SaveEventArgs { Save = false }); }
						catch { }
						return;
					}
				}

				FileStream fs = null;
				try { fs = File.Open(_file_name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete); }
				catch(IOException)
				{
					ErrorShower.OnShowError(this, string.Format(Environment.StringResources.GetString("DocControl_PDF_FileBusy"), _file_name), Environment.StringResources.GetString("DocControl_Page_Error1"));
					try { ChangePage(new SaveEventArgs { Save = false }); }
					catch { }
					return;
				}
				catch(UnauthorizedAccessException ue)
				{
					ErrorShower.OnShowError(this, ue.Message, Environment.StringResources.GetString("DocControl_Page_Error1"));
					try { ChangePage(new SaveEventArgs { Save = false }); }
					catch { }
					return;
				}
				finally { if(fs != null) fs.Close(); }
			}

			string temp = Path.GetTempFileName();
			try
			{
				int[] pages = new int[PagePreview.Items.Count];
				int i = 1;
				pages = pages.Select(x => x = ((ItemTag)PagePreview.Items[i++ - 1].Tag).OriginNumber).ToArray();

				if(FileSave(_file_name, temp, pages, true))
				{
					try
					{
						File.Copy(temp, _file_name, true);
					}
					catch(IOException)
					{
						ErrorShower.OnShowError(this, string.Format(Environment.StringResources.GetString("DocControl_PDF_FileBusy"), _file_name), Environment.StringResources.GetString("DocControl_Page_Error1"));
						try { ChangePage(new SaveEventArgs { Save = false }); }
						catch { }
						return;
					}

					_fi = new FileInfo(_file_name);
					filesData = _fi.LastWriteTime;
					filesLenght = _fi.Length;

					if(_userPass != "")
					{
						if(_filePass == _userPass) _filePass = "";
						_userPass = "";
						DocPages = pdfHelper.Refresh(_file_name, ref _filePass, ref _userPass);
						pdfHelper.Open(_file_name, _filePass);
						PagePreview.ClientSizeChanged -= PagePreviewClientSizeChanged;
						PagePreview.BeginUpdate();

						PagePreview.Items.Cast<ListViewItem>().Select(x => ((ItemTag)x.Tag).OriginNumber = x.Index + 1).ToArray();
						PagePreview.Items.Cast<ListViewItem>().Select(x => x.Text = (x.Index + 1).ToString()).ToArray();
						PagePreview.Items.Cast<ListViewItem>().Select(x => ((ItemTag)x.Tag).ItemSize = Size.Empty).ToArray();

						RefreshPagePreview(true);
						PagePreview.ClientSizeChanged += PagePreviewClientSizeChanged;
					}
					else
					{
						DocPages = pdfHelper.Refresh(_file_name, ref _filePass, ref _userPass);
						pdfHelper.Open(_file_name, _filePass);
						PagePreview.Items.Cast<ListViewItem>().Select(x => ((ItemTag)x.Tag).OriginNumber = x.Index + 1).ToArray();
					}
				}
				else
				{
					try { ChangePage(new SaveEventArgs { Save = false }); }
					catch { }
				}
			}
			catch(Exception ex)
			{
				ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("DocControl_Page_Error1"));
				try { ChangePage(new SaveEventArgs { Save = false }); }
				catch { }
			}
			finally
			{
				PagePreview.EndUpdate();
				try { if(fa != File.GetAttributes(_file_name)) File.SetAttributes(_file_name, fa); }
				catch { }

				Slave.DeleteFile(temp);
			}
			modifiedStamp = false;
			SetModified(false);
		}

		/// <summary>
		/// Проверка изменений в файле 
		/// </summary>
		private bool VerifyFile(string fileName)
		{
			bool result = false;
			if(File.Exists(fileName))
			{
				FileInfo fiNew = new FileInfo(fileName);
				if(_fi != null && (fiNew.Length != filesLenght || fiNew.LastWriteTime != filesData))
				{
					MessageBox.Show(string.Format(Environment.StringResources.GetString("DocControl_FileChanged"), " "),
						Environment.StringResources.GetString("Error"));

					SetModified(false);
					FileName = fileName;
				}
				else
					result = true;
			}
			else
				MessageBox.Show(string.Format(Environment.StringResources.GetString("DocControl_FileDeleted"), fileName), Environment.StringResources.GetString("Error"));
			return result;
		}

		/// <summary>
		/// Вывод диалогов, если файл уже не существует
		/// </summary>
		private void DialogForSaveUnexistFile(string tempFileName)
		{
			MessageBox.Show(Environment.StringResources.GetString("DocControl_FileDeleted"),
				Environment.StringResources.GetString("Error"));

			string tempFileN = Path.GetTempFileName();
			FileName = _file_name;
		}

		private void OnNeedSave(SaveEventHandler handler, Environment.ActionBefore act = Environment.ActionBefore.None)
		{
			SaveEventArgs args = new SaveEventArgs();
			args.AfterSave += handler;
			args.Action = act;
			if(NeedSave != null)
			{
				_lock_on_save_item_changing = true;
				NeedSave(args);
			}
		}

		/// <summary>
		/// используется только для сохр-я изображения Архива => newFileName - всегда временный файл и насильно обновлять не нужно
		/// </summary>
		/// <param name="newFileName"></param>
		public bool SaveAs(string newFileName)
		{
			try
			{
				return SavePart(newFileName);
			}
			catch
			{
				return false;
			}
			finally
			{
				SetModified(false);
			}
		}

		public bool SavePart(string dPath)
		{
			return SavePart(dPath, -1, -1);
		}

		/// <summary>
		/// используется только для сохр-я изображения в Архив => newFileName - всегда временный файл и насильно обновлять не нужно
		/// </summary>
		/// <param name="dPath"></param>
		/// <param name="page_from"></param>
		/// <param name="page_to"></param>
		/// <returns></returns>
		public bool SavePart(string dPath, int page_from, int page_to)
		{
			if(!VerifyFile(_file_name))
				return false;

			if(page_from <= 0)
				page_from = 1;
			if(page_to <= 0)
				page_to = PagePreview.Items.Count;
			int[] pages = new int[page_to - page_from + 1];
			int i = page_from;
			pages = pages.Select(x => x = ((ItemTag)PagePreview.Items[i++ - 1].Tag).OriginNumber).ToArray();
			return FileSave(_file_name, dPath, pages);
		}

		/// <summary>
		/// получение количества страниц из файла
		/// </summary>
		/// <param name="fileName">имя файла</param>
		/// <returns>количество страниц</returns>
		public int GetFilePagesCount(string fileName)
		{
			int ret = 0;
			if(File.Exists(fileName))
			{
				PdfReader pr = null;
				try
				{
					pr = new PdfReader(fileName);
					if(pr != null)
						ret = pr.NumberOfPages;
				}
				catch
				{ }
				finally
				{
					try
					{
						if(pr != null) pr.Close();
					}
					catch
					{ }
				}
			}
			return ret;
		}

		/// <summary>
		/// Удаления интерваля страниц из файла PDF
		/// </summary>
		/// <param name="fileName">имя фаqла для удаления страниц</param>
		/// <param name="page_from">страница, c которой начинается удаление</param>
		/// <param name="page_to">страница, на которой заканчивается удаление(включительно)</param>
		/// <param name="refreshView"></param>
		internal void DelPart(string fileName, int page_from, int page_to, bool refreshView)
		{
			bool isOrig = fileName.Equals(_file_name, StringComparison.CurrentCultureIgnoreCase);

			try
			{
				PagePreview.BeginUpdate();
				ChangePage(new SaveEventArgs { Save = false });
				PagePreview.EndUpdate();
			}
			catch
			{ }

			if(!VerifyFile(_file_name))
				return;

			FileAttributes fa = FileAttributes.Normal;
			if(isOrig)
			{
				if(((int)File.GetAttributes(_file_name) & (int)FileAttributes.ReadOnly) == (int)FileAttributes.ReadOnly)
				{
					try
					{
						fa = File.GetAttributes(_file_name);
						File.SetAttributes(_file_name, FileAttributes.Normal);
					}
					catch(Exception ex)
					{
						ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("DocControl_Page_Error1"));
						return;
					}
				}
			}

			int p = (page_to < PagePreview.Items.Count) ? page_from : page_from - 1;

			int i = page_from - 1;
			int j = 1;

			int[] pages = new int[PagePreview.Items.Count];

			pages = pages.Select(x => x = j++).ToArray();
			pages = pages.Where(x => x < page_from || x > page_to).ToArray();

			pages = pages.Select(x => x = ((ItemTag)PagePreview.Items[x - 1].Tag).OriginNumber).ToArray();
			string temp = Path.GetTempFileName();
			try
			{
				if(FileSave(_file_name, temp, pages, isOrig))
				{
					try
					{
						File.Copy(temp, fileName, true);
					}
					catch(IOException)   //сюда попадаем только в случае isOrig == true
					{
						ErrorShower.OnShowError(this, string.Format(Environment.StringResources.GetString("DocControl_PDF_FileBusy"), _file_name), Environment.StringResources.GetString("Error"));
						return;
					}
					catch(UnauthorizedAccessException ue)// и сюда тоже
					{
						ErrorShower.OnShowError(null, ue.Message, Environment.StringResources.GetString("DocControl_Page_Error1"));
						return;
					}

					if(isOrig && refreshView && NeedPreview)
					{
						_fi = new FileInfo(_file_name);

						filesData = _fi.LastWriteTime;
						filesLenght = _fi.Length;

						if(_filePass == _userPass) _filePass = "";
						_userPass = "";

						DocPages = pdfHelper.Refresh(_file_name, ref _filePass, ref _userPass);

						PagePreview.ClientSizeChanged -= PagePreviewClientSizeChanged;
						PagePreview.BeginUpdate();

						for(int k = page_from - 1; k < page_to; k++)
						{
							PagePreview.Items.RemoveAt(i);
							PagePreviewImages.Images.RemoveAt(i);
						}

						PagePreview.EndUpdate();
						PagePreview.ClientSizeChanged += PagePreviewClientSizeChanged;

						PagePreview.Items.Cast<ListViewItem>().Select(x => ((ItemTag)x.Tag).OriginNumber = x.Index + 1).ToArray();
						PagePreview.Items.Cast<ListViewItem>().Select(x => x.Text = (x.Index + 1).ToString()).ToArray();
						PagePreview.Items.Cast<ListViewItem>().Select(x => x.ImageIndex = x.Index).ToArray();
					}
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				Error.ErrorShower.OnShowError(this, ex.Message, StringResources.Error);
			}
			finally
			{
				Slave.DeleteFile(temp);

				if(isOrig)
				{

					if(fa != File.GetAttributes(_file_name))
						try { File.SetAttributes(_file_name, fa); }
						catch { }

					if(refreshView && NeedPreview)
					{
						PageCount = DocPages.Count;
						_forcePage = true;
						// Page = p;
					}
				}
			}
		}
		#endregion

		#region SelectTool

		public virtual void SelectTool(short tool, int docID)
		{
			SelectTool(tool);
			stampDocID = docID;
			PageView.CurrentDocID = docID;
		}

		public void SelectTool(int tool)
		{
			stampMode = false;
			if(PageView == null)
				return;
			switch(tool)
			{
				case 0:
					PageView.WorkMode = PicturePanel.WorkingMode.Move;
					break;
				case 1:
					PageView.WorkMode = PicturePanel.WorkingMode.EditNotes;
					break;
				case 9:
					PageView.BeginCreationFigure(ImageControl.ImageControl.Figures.EmbeddedImage);
					stampMode = true;
					PageView.Focus();
					break;
			}

			OnToolSelected(new ImageControl.ImageControl.ToolSelectedEventArgs { EventType = tool });
		}

		private void OnToolSelected(ImageControl.ImageControl.ToolSelectedEventArgs e)
		{
			try
			{
				if(ToolSelected != null)
					ToolSelected.DynamicInvoke(new object[] { this, e });
			}
			catch(Exception ex)
			{
				LibTiffHelper.WriteToLog(ex);
			}
		}

		#endregion

		public bool ShowThumbPanel
		{
			get
			{
				return !Splitter.Panel1Collapsed;
			}
			set
			{
				Splitter.Panel1Collapsed = !value;
				if(value)
					RefreshPagePreview(true);
			}
		}

		private int _fakeZoom = 100;

		[Browsable(false), DefaultValue(100)]
		public int Zoom
		{
			get
			{
				return _fakeZoom;   // _zoom;
			}
			set
			{
				_fakeZoom = (value < fake_zoom_min ? fake_zoom_min : (value > fake_zoom_max ? fake_zoom_max : value));
				value = (value < zoom_min ? zoom_min : (value > zoom_max ? zoom_max : value));
				if(_zoom != value)
				{
					_zoom = value;
					_fitType = -1;
					PageView.FitType = -1;


					if(!string.IsNullOrEmpty(_file_name) && File.Exists(_file_name))
					{
						RefreshPageView();
						PageView.IsRefreshBitmap = true;

						if(PageView != null)
						{
							PageView.Zoom = _zoom;
						}
					}
				}
			}
		}

		/// <summary>
		/// Проверка, на изменения
		/// </summary>
		public void TestImage(Document.Environment.ActionBefore act)
		{
			if(PageView.Image != null && Modified)
				OnNeedSave(new ImageControl.SaveEventHandler(ChangePage), act);
		}

		public void LoadImagePage(int _imageID, int page, bool force = false)
		{
			if(_imageID < 1)
			{
				SetImageValue(0);
				FileName = null;
			}
			else
			{
				if(this.imageID != _imageID || force)
					Clear();
				SetImageValue(_imageID);
				if(PageView != null)
				{
					PageView.ImageID = _imageID;
					if(page > 0)
						ReloadImageData(true, page);
					else
						ReloadImageData(true, 1);
				}
			}
		}

		#region StampAccessors

		public Image CurrentStamp
		{
			get { return PageView.CurrentStamp; }
			set { PageView.CurrentStamp = value; }
		}

		public int CurrentStampID
		{
			get { return PageView.CurrentStampID; }
			set { PageView.CurrentStampID = value; }
		}

		private int stampDocID;

		#endregion

		/// <summary>
		/// Принудительная репликация
		/// </summary>
		public bool ForceReplicate { get; set; }

		/// <summary>
		/// Текущий сервер
		/// </summary>
		public ServerInfo CurrentServer { get; private set; }

		/// <summary>
		/// Текущий поворот
		/// </summary>
		public int CurrentRotate
		{
			get
			{
				return DocPages[Page].CurrentRotate;
			}
		}

		private int imageID;
		/// <summary>
		/// Код изображения
		/// </summary>
		public int ImageID
		{
			get { return imageID; }
			set
			{
				LoadImagePage(value, 0);
			}
		}

		private void SetImageValue(int value)
		{
			buttonLoad.Visible = false;
			labelNoImage.Visible = false;
			imageID = value;
			if(PageView.StampItems != null)
			{
				PageView.StampItems.Clear();
				PageView.StampItems = null;
			}
			OnImageLoad();
		}

		private void ReloadImageData(bool change, int page)
		{
			try
			{
				if(Environment.GetServers().Count <= 0)
					throw new Exception(Environment.StringResources.GetString("Enviroment_Servers_Error1"));

				List<int> serverIDs = Environment.DocImageData.GetLocalDocImageServers(imageID, Environment.GetLocalServersString());

				if(serverIDs != null && serverIDs.Count > 0)
				{
					Random rand = new Random();
					CurrentServer = Environment.GetLocalServer(serverIDs[rand.Next(serverIDs.Count)]);
					string fileName = CurrentServer.Path + "\\" + Environment.GetFileNameFromID(imageID) + ".pdf";
					if(File.Exists(fileName))
					{
						if(change)
							FileName = fileName;

						OnFileNameChanged();
						if(page > 0)
							Page = page;
					}
					else
					{
						bool err = true;
						if(serverIDs.Count > 1)
						{
							foreach(int id in serverIDs)
							{
								CurrentServer = Environment.GetLocalServer(id);
								fileName = CurrentServer.Path + "\\" + Environment.GetFileNameFromID(imageID) + ".pdf";
								if(!File.Exists(fileName))
									continue;
								err = false;
								if(change)
									FileName = fileName;
								OnFileNameChanged();
								if(page > 0)
									Page = page;
								break;
							}
						}
						if(err)
							NeedReplicate();
					}

					int[] curSel = null;

					if(PageView.StampItems != null)
					{
						if(page > 0)
						{
							curSel = PageView.StampItems.Where(x => x.StmpItem.Page == page && x.IsSelected).Select(
									x => PageView.StampItems.IndexOf(x)).ToArray();
						}

						PageView.StampItems.Clear();
						PageView.StampItems = null;
					}

					PageView.StampItems = Environment.DocSignatureData.GetStamps(imageID).OrderBy(x => x.ID).Select(x => new Stamp { StmpItem = x }).ToList();

					if(curSel != null && PageView.StampItems.Count > 0)
						curSel.Where(x => x < PageView.StampItems.Count).Select(
							x => PageView.StampItems[x].IsSelected = true).ToArray();

					if(PageView.StampItems != null && PageView.StampItems.Count > 0)
						try
						{
							ChangePage(new SaveEventArgs { Save = false });
						}
						catch
						{
						}

					PageView.IsAnnuled = Environment.DocSignatureData.IsDocSignedAnnuled(ImageID);
					PageView.IsRefreshBitmap = true;
				}
				else
					NeedReplicate();
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
				ClearImage();
			}
		}

		void PageView_NeedToReloadStamps(object sender, EventArgs e)
		{
			ReloadStamps();
		}

		internal void ReloadStamps()
		{
			if(Disposing)
				return;
			PageView.Hide();

			int sx = PageView.HorizontalScroll.Value;
			int sy = PageView.VerticalScroll.Value;
			ReloadImageData(false, Page);
			ScrollPositionX = sx;
			ScrollPositionY = sy;

			PageView.HorizontalScroll.Value = sx;
			PageView.VerticalScroll.Value = sy;
			PageView.Show();

			PageView.HorizontalScroll.Value = sx;
			PageView.VerticalScroll.Value = sy;
			PageView.Show();

			PageView.Refresh();
			//PageView.Focus();

			if((PageView.WorkMode == PicturePanel.WorkingMode.EditNotes || PageView.WorkMode == PicturePanel.WorkingMode.EditNote)
				 && (PageView.StampItems == null || PageView.StampItems.Count == 0))
				SelectTool(0);
		}

		/// <summary>
		/// Сохранить координаты штампов
		/// </summary>
		internal void SaveStampsPositions()
		{
			if(PageView.StampItems != null)
			{
				List<StampItem> stamps = PageView.StampItems.Where(x => x.StmpItem.CooordinateChanged).Select(x => x.StmpItem).ToList();
				if(stamps.Count > 0)
					Environment.DocSignatureData.StampReplace(stamps);
				modifiedStamp = false;
			}
		}

		private void NeedReplicate()
		{
			Visible = true;
			FileName = "";
			if(ForceReplicate)
				buttonLoad_Click(this, EventArgs.Empty);
			else
			{
				buttonLoad.Visible = true;
				labelNoImage.Visible = true;
			}
		}

		private void buttonLoad_Click(object sender, EventArgs e)
		{
			List<int> serverIDs = Environment.DocImageData.GetLocalDocImageServers(imageID, Environment.GetLocalServersString());
			if(serverIDs != null && serverIDs.Count > 0)
			{
				ReloadImageData(false, 0);
			}
			else
			{
				Environment.DocImageData.DocImageCopy(Environment.GetRandomLocalServer().ID, imageID);

				ReloadImageData(false, 0);
			}
		}

		void PageView_AnnotationCreated(int x, int y, int r)
		{
			if(imageID <= 0)
				return;
			if(Modified)
				OnNeedSave(new Lib.Win.ImageControl.SaveEventHandler(ChangePage));
			if(CurrentStampID == -101)
				if(!Environment.DocSignatureData.AddStampDSP(stampDocID, imageID, Environment.CurEmp.ID, Page, x, y, r, 0))
				{
					PageView.IsRefreshBitmap = true;
					PageView.Refresh();
				}
				else
				{
					CurrentStamp = null;
					SelectTool(0);
				}
			else
				if(!Environment.DocSignatureData.AddStamp(stampDocID, imageID, Environment.CurEmp.ID, CurrentStampID, Page, x, y, r, 0))
				{
					PageView.IsRefreshBitmap = true;
					PageView.Refresh();
				}
				else
				{
					SelectTool(9);
				}
		}

		private void ClearImage()
		{
			CurrentServer = null;
			buttonLoad.Visible = false;
			labelNoImage.Visible = false;
			FileName = "";
		}

		/// <summary>
		/// Обратный вызов после сохранения 
		/// </summary>
		protected void ChangePage(Lib.Win.ImageControl.SaveEventArgs args)
		{
			try
			{
				if(this.Disposing || this.IsDisposed)
					return;
				if(!args.Save && pageMovied)
				{
					var lvItems = PagePreview.Items.Cast<ListViewItem>().Where(x => (x.Index + 1) != ((ItemTag)x.Tag).OriginNumber).ToArray();
					if(lvItems.Length > 0)
					{
						PagePreview.SelectedIndexChanged -= PagePreviewSelectedIndexChanged;

						foreach(ListViewItem lvi in lvItems)
						{
							int index = ((ItemTag)lvi.Tag).OriginNumber - 1;

							PagePreview.Items[lvi.Index].Remove();
							PagePreview.Items.Insert(index, lvi);

							PagePreview.Sort();
						}

						PagePreview.Items.Cast<ListViewItem>().Select(x => x.Text = (x.Index + 1).ToString()).ToArray();
						if(PagePreview.SelectedItems.Count > 0)
						{
							_page = PagePreview.SelectedItems[0].Index + 1;
							PageView.Page = _page;
						}

						PagePreview.SelectedIndexChanged += PagePreviewSelectedIndexChanged;
					}
				}
				if(!args.Save && modified)
				{
					var pages = DocPages.Where(x => x.Value.CurrentRotate != 0);

					if(pages != null)
					{
						foreach(var p in pages)
						{
							RotateFlipType rt = (RotateFlipType)(p.Value.CurrentRotate);

							switch(rt)
							{
								case RotateFlipType.Rotate90FlipNone:
									rt = RotateFlipType.Rotate270FlipNone;
									break;
								case RotateFlipType.Rotate270FlipNone:
									rt = RotateFlipType.Rotate90FlipNone;
									break;
							}
							Rotate(p.Key, rt);
						}
					}
				}
			}
			finally
			{
				SetModified(false);
				SaveSinc.Set();
				_lock_on_save_item_changing = false;
			}
		}

		internal protected void SetModified(bool modified)
		{
			this.modified = modified;

			if(!modified)
			{
				modifiedStamp = false;
				pageMovied = modified;
				pageMoviedExtern = modified;
			}
		}

		Pen dotedPen = new Pen(new SolidBrush(Color.Black), 1);
		Pen redDotedPen = new Pen(new SolidBrush(Color.Red), 1);

		#region Функции вспомогательные

		private string FileOpen(string path)
		{
            Console.WriteLine("{0}: FileOpen. Start", DateTime.Now.ToString("HH:mm:ss fff"));

			if(!File.Exists(path))
				throw new Exception(String.Format(Common.MSG_FILE_NOT_FOUND, path));

			try
			{
				Cursor = Cursors.WaitCursor;
				DocPages = pdfHelper.Refresh(path, ref _filePass, ref _userPass);
				if(DocPages.Count <= 0)
					return "";
				pdfHelper.Open(path, _filePass);
				PageCount = DocPages.Count;

			    Console.WriteLine("{0}: FileOpen. Refresh from TMP", DateTime.Now.ToString("HH:mm:ss fff"));
			}
			finally
			{
				Cursor = Cursors.Default;
			}

			try
			{
				Cursor = Cursors.WaitCursor;

				if(PageCount > 0)
				{
					Bitmap thumbnail = new Bitmap(140, 140);
					using(Graphics g = Graphics.FromImage(thumbnail))
					{
						g.FillRectangle(Brushes.White, 0, 0, thumbnail.Width, thumbnail.Height);
					}
					#region
					if(NeedPreview)
					{
						Bitmap[] bA = new Bitmap[PageCount];
						bA = bA.Select(x => x = new Bitmap(thumbnail)).ToArray();
						PagePreviewImages.Images.AddRange(bA);
					}
					int i = 0;
					ListViewItem[] lviA = new ListViewItem[PageCount];

					PagePreview.ClientSizeChanged -= PagePreviewClientSizeChanged;
					PagePreview.BeginUpdate();
					PagePreview.Items.AddRange(lviA.Select(x => x = new ListViewItem { ImageIndex = i++, /*Tag = null*/ Tag = new ItemTag(i), Text = i.ToString() }).ToArray());
					PagePreview.EndUpdate();
					if(NeedPreview)
						PagePreview.ClientSizeChanged += PagePreviewClientSizeChanged;

                    Console.WriteLine("{0}: FileOpen. PagePreview loaded", DateTime.Now.ToString("HH:mm:ss fff"));
					#endregion
				}
			}
			finally
			{
				Cursor = Cursors.Default;
			}
			return path;
		}

		/// <summary>
		/// пересохранение файла с удалением страниц, без сохраненияя защиты
		/// </summary>
		/// <param name="srcPath">имя файла, из которого читаются данные</param>
		/// <param name="dstPath">имя файла, в который сохраняются данные</param>
		/// <param name="pages">массив страниц, которые остаются</param>
		private bool FileSave(string srcPath, string dstPath, int[] pages)
		{
			return FileSave(srcPath, dstPath, pages, false);
		}

		/// <summary>
		/// пересохранение файла с удалением страниц
		/// </summary>
		/// <param name="srcPath">имя файла, из которого читаются данные</param>
		/// <param name="dstPath">имя файла, в который сохраняются данные</param>
		/// <param name="pages">массив страниц, которые остаются</param>
		/// <param name="savePermission">сохранять защиту</param>
		private bool FileSave(string srcPath, string dstPath, int[] pages, bool savePermission)
		{
			PdfReader pdfReader = null;

			string _usrPass = "";
			string _ownrPass = "";
			try
			{
				Cursor.Current = Cursors.WaitCursor;

				#region авторизация
				try
				{
					pdfReader = new PdfReader(srcPath, Encoding.UTF8.GetBytes(_filePass));

					if(!pdfReader.IsOpenedWithFullPermissions)
					{
						_usrPass = _filePass;
						throw new BadPasswordException("");
					}
					else
					{
						byte[] up = pdfReader.ComputeUserPassword();
						if(up != null)
							_usrPass = Encoding.UTF8.GetString(up);
						if(_usrPass != _filePass)
							_ownrPass = _filePass;
					}
				}
				catch(BadPasswordException)
				{
					string fp = _filePass;
					while(pdfReader == null || !pdfReader.IsOpenedWithFullPermissions)
					{
						if(pdfReader != null) pdfReader.Close();

						if(InputBox.Show(Environment.StringResources.GetString("DocControl_PDF_Encrypted"),
								Environment.StringResources.GetString("DocControl_PDF_EnterPass"), ref _filePass) == DialogResult.Cancel)
						{
							_filePass = fp;
							return false;
						}
						try
						{
							pdfReader = new PdfReader(srcPath, Encoding.UTF8.GetBytes(_filePass));
							_ownrPass = _filePass;
						}
						catch(BadPasswordException)
						{
							_filePass = fp;
						}
					}
				}
				#endregion
				int pagesCount = pdfReader.NumberOfPages;
				pdfReader.RemoveUnusedObjects();
				pdfReader.SelectPages(pages);

				using(FileStream file_stream = new FileStream(dstPath, FileMode.Create, FileAccess.Write, FileShare.None))
				using(PdfStamper stamper = new PdfStamper(pdfReader, file_stream))
				{
					stamper.SetFullCompression();
					if(savePermission && _ownrPass != "")
					{
						stamper.SetEncryption(
							null,
							(_ownrPass.Length > 0) ? Encoding.UTF8.GetBytes(_ownrPass) : null,
							pdfReader.Permissions, PdfWriter.STANDARD_ENCRYPTION_128);
					}

					for(int k = 0; k < pages.Length; k++)
					{
						int numberPage = pages[k];
						if(numberPage <= 0 || numberPage > pagesCount)
							continue;
						PdfDictionary page = pdfReader.GetPageN(k + 1);
						PdfDictionary resources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
						PdfDictionary xobject = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));
						if(xobject != null)
						{
							PdfObject obj;
							foreach(PdfName pdname in xobject.Keys)
							{
								obj = xobject.Get(pdname);
								if(obj.IsIndirect())
								{
									PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj); //resolve indirect reference
									PdfName subType = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
									if(PdfName.IMAGE.Equals(subType))
									{
										int xrefIndex = ((PRIndirectReference)obj).Number;
										PdfObject imgPdfObj = pdfReader.GetPdfObject(xrefIndex);
										PdfStream imgPdfStream = (PdfStream)imgPdfObj;
										PRStream imgPRStream = (PRStream)imgPdfStream;
										byte[] bytes = PdfReader.GetStreamBytesRaw(imgPRStream);

										if(bytes != null && bytes.Length > 0)
										{
											try
											{
												iTextSharp.text.pdf.parser.PdfImageObject pdfImage = new iTextSharp.text.pdf.parser.PdfImageObject(imgPRStream);

												Image img = pdfImage.GetDrawingImage();
												if(img != null)
												{
													PdfName filter = (PdfName)pdfImage.Get(PdfName.FILTER);
													if(filter != null)
														continue;
													ImageFormat format;
													byte[] updatedImgBytes = ConvertImageToBytes(img, 75, out format);

													iTextSharp.text.Image compressedImage = iTextSharp.text.Image.GetInstance(updatedImgBytes);
													if(format == ImageFormat.Png)
														compressedImage.Deflated = true;

													PdfReader.KillIndirect(obj);
													stamper.Writer.AddDirectImageSimple(compressedImage, (PRIndirectReference)obj);
													img.Dispose();
												}
												else
												{

                                                    Console.WriteLine("{0}: {1} image is null", DateTime.Now.ToString("HH:mm:ss fff"), xrefIndex);
												}
											}
											catch(Exception e)
											{
												Console.WriteLine(e.ToString());
												continue;
											}
										}
										imgPdfStream = null;
										imgPRStream = null;
										imgPdfObj = null;
									}
									else
                                        Console.WriteLine("{0}: Skipping subtype {1} with filter {2}", DateTime.Now.ToString("HH:mm:ss fff"), subType, tg.Get(PdfName.FILTER));
								}
							}
						}
						int angle = 0;
						if(DocPages != null && DocPages.Count >= numberPage)
						{
							PageInfo pi = DocPages[numberPage];
							angle = pi.OriginalRotation;

							// Виртуальный поворот
							_pageVirtualRotationDictionary.UpdatePageRotation(ImageID, numberPage - 1);

							var virtualRotation = _pageVirtualRotationDictionary.GetPageRotation(ImageID, numberPage - 1);

							if(virtualRotation != null)
								switch(virtualRotation)
								{
									case 90:
										pi.CurrentRotate += 1;
										break;
									case 180:
										pi.CurrentRotate += 2;
										break;
									case 270:
										pi.CurrentRotate += 3;
										break;
								}

							if(pi.CurrentRotate != 0 || virtualRotation > 0)
							{
								angle += pi.CurrentRotate;
								angle %= 4;
								if(pi.CurrentRotate == (int)RotateFlipType.Rotate90FlipNone || pi.CurrentRotate == (int)RotateFlipType.Rotate270FlipNone)
								{
									int ht = pi.PageSizeWithRotation.Height;
									pi.PageSizeWithRotation = new System.Drawing.Rectangle(0, 0, pi.PageSizeWithRotation.Height, pi.PageSizeWithRotation.Width);
								}
								pi.OriginalRotation = angle;
								pi.BeforeSaveRotate = pi.CurrentRotate;
								pi.CurrentRotate = 0;
							}
						}
						else
							angle = pdfReader.GetPageRotation(k + 1) / 90;

						page.Put(PdfName.ROTATE, new PdfNumber(angle * 90));
					}

					stamper.Close();
					stamper.Dispose();

				}
			}
			finally
			{
				if(pdfReader != null)
					pdfReader.Close();

				Cursor.Current = Cursors.Default;
			}
			return true;
		}

		public static byte[] ConvertImageToBytes(System.Drawing.Image image, long compressionLevel, out ImageFormat format)
		{
			if(compressionLevel < 0)
			{
				compressionLevel = 0;
			}
			else if(compressionLevel > 100)
			{
				compressionLevel = 100;
			}
			format = ImageFormat.Png;
			ImageCodecInfo pngEncoder = GetEncoder(ImageFormat.Png);
			ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
			System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Compression;
			EncoderParameters myEncoderParameters = new EncoderParameters(1);
			EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 9);
			myEncoderParameters.Param[0] = myEncoderParameter;
			using(MemoryStream ms = new MemoryStream())
			{
				image.Save(ms, pngEncoder, myEncoderParameters);
				long l1 = ms.Length;
				if(image.PixelFormat != PixelFormat.Format24bppRgb)
					return ms.ToArray();
				ms.Position = 0;
				ms.SetLength(0);
				myEncoder = System.Drawing.Imaging.Encoder.Quality;
				myEncoderParameter = new EncoderParameter(myEncoder, compressionLevel);
				myEncoderParameters.Param[0] = myEncoderParameter;
				image.Save(ms, jpgEncoder, myEncoderParameters);
				long l2 = ms.Length;
				if(l2 < l1)
				{
					format = ImageFormat.Jpeg;
					return ms.ToArray();
				}
				else
				{
					myEncoder = System.Drawing.Imaging.Encoder.Compression;
					myEncoderParameter = new EncoderParameter(myEncoder, 9);
					myEncoderParameters.Param[0] = myEncoderParameter;
					ms.Position = 0;
					ms.SetLength(0);
					image.Save(ms, pngEncoder, myEncoderParameters);
					return ms.ToArray();
				}
			}
		}

		public static ImageCodecInfo GetEncoder(ImageFormat imageFormat)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
			foreach(ImageCodecInfo codec in codecs)
			{
				if(codec.FormatID == imageFormat.Guid)
				{
					return codec;
				}
			}
			return null;
		}


		private bool FileSave4Mail_Fax(string srcPath, string dstPath, int[] pages)
		{
			PdfReader pdfReader = null;

			string _usrPass = "";
			string _ownrPass = "";
			try
			{
				Cursor.Current = Cursors.WaitCursor;

				#region авторизация
				try
				{
					pdfReader = new PdfReader(srcPath, Encoding.UTF8.GetBytes(_filePass));

					if(!pdfReader.IsOpenedWithFullPermissions)
					{
						_usrPass = _filePass;
						throw new BadPasswordException("");
					}
					else
					{
						byte[] up = pdfReader.ComputeUserPassword();
						if(up != null)
							_usrPass = Encoding.UTF8.GetString(up);
						if(_usrPass != _filePass)
							_ownrPass = _filePass;
					}
				}
				catch(BadPasswordException)
				{
					string fp = _filePass;
					while(pdfReader == null || !pdfReader.IsOpenedWithFullPermissions)
					{
						if(pdfReader != null)
							pdfReader.Close();

						if(InputBox.Show(Environment.StringResources.GetString("DocControl_PDF_Encrypted"),
								Environment.StringResources.GetString("DocControl_PDF_EnterPass"), ref _filePass) == DialogResult.Cancel)
						{
							_filePass = fp;
							return false;
						}
						try
						{
							pdfReader = new PdfReader(srcPath, Encoding.UTF8.GetBytes(_filePass));
							_ownrPass = _filePass;
						}
						catch(BadPasswordException)
						{
							_filePass = fp;
						}
					}
				}
				#endregion

				pdfReader.RemoveUnusedObjects();

				using(FileStream file_stream = new FileStream(dstPath, FileMode.Create, FileAccess.Write, FileShare.None))
				using(iTextSharp.text.Document document = new iTextSharp.text.Document(pdfReader.GetPageSize(1)))
				using(PdfCopy pdfWriter = new PdfCopy(document, file_stream))
				{
					pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;
					pdfWriter.SetFullCompression();

					document.Open();
					for(int i = 0; i < pages.Length; i++)
					{
						int numberPage = pages[i];
						if(numberPage <= 0 || numberPage > pdfReader.NumberOfPages)
							continue;

						// Сохраняем штампы, если нужно
						List<iTextSharp.text.Image> stamps = new List<iTextSharp.text.Image>();
						if(PageView.StampItems != null && PageView.StampItems.Count > 0)
						{
							foreach(Stamp si in PageView.StampItems)
							{
								if(si.StmpItem.Page == numberPage)
								{
									if(si.StmpItem.TypeID == 101)
										continue;
									Image stamp = Environment.GetStamp(si.StmpItem.StampID, si.StmpItem.ImageID);
									if(stamp != null)
									{
										float x = (float)(si.StmpItem.X * (72.0 / 96.0));
										float y = (float)(si.StmpItem.Y * (72.0 / 96.0));
										int w = (int)Math.Round(stamp.Width * (si.StmpItem.Zoom / 100.0) * (72.0 / 96.0));
										int h = (int)Math.Round(stamp.Height * (si.StmpItem.Zoom / 100.0) * (72.0 / 96.0));
										int g = (int)Math.Round(Math.Sqrt(w * w + h * h));

										Bitmap bm = new Bitmap(g, g);
										using(Graphics gr = Graphics.FromImage(bm))
										{
											if(!si.StmpItem.Rotate.Equals(0))
											{
												Matrix mx = new Matrix();
												mx.RotateAt(si.StmpItem.Rotate, new PointF(bm.Width / 2, bm.Height / 2));
												gr.Transform = mx;
											}
											gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
											gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
											gr.DrawImage(stamp, (g - w) / 2, (g - h) / 2, w, h);
										}

										int r = pdfReader.GetPageRotation(numberPage) / 90;

										iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(bm, ImageFormat.Png);

										if(r % 2 == 0)
											gif.SetAbsolutePosition(x - (g - w) / 2, pdfWriter.PageSize.Height - y - (g + h) / 2);
										else
											gif.SetAbsolutePosition(x - (g - w) / 2, pdfWriter.PageSize.Width - y - (g + h) / 2);

										gif.Interpolation = true;
										stamps.Add(gif);
									}
								}
							}
						}

						PdfImportedPage pp = pdfWriter.GetImportedPage(pdfReader, numberPage);
						if(stamps.Count > 0)
						{
							PdfCopy.PageStamp stamp = pdfWriter.CreatePageStamp(pp);
							PdfContentByte cb = stamp.GetOverContent();
							foreach(iTextSharp.text.Image im in stamps)
								cb.AddImage(im);

							stamp.AlterContents();
						}

						pdfWriter.AddPage(pp);
					}
					document.Close();
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, srcPath);
			}
			finally
			{
				if(pdfReader != null)
					pdfReader.Close();


				Cursor.Current = Cursors.Default;
			}
			return true;
		}

		private void PagePreview_ClientSizeChanged(object sender, EventArgs e)
		{
			if(TopLevelControl != null && !TopLevelControl.Disposing)
				RefreshPagePreview(false);
		}

		private short PageView_ClientSizeChanged_Subscriber;

		private void Subscribe_PageView_ClientSizeChanged(bool subscribe)
		{
			if(subscribe && PageView_ClientSizeChanged_Subscriber == 0)
			{
				PageView.ClientSizeChanged += PageViewClientSizeChanged;
				PageView_ClientSizeChanged_Subscriber++;
			}
			else if(!subscribe && PageView_ClientSizeChanged_Subscriber == 1)
			{
				PageView.ClientSizeChanged -= PageViewClientSizeChanged;
				PageView_ClientSizeChanged_Subscriber--;
			}
		}

		private void PageView_ClientSizeChanged(object sender, EventArgs e)
		{
			if(TopLevelControl == null || TopLevelControl.Disposing || PageView.Image == null)
				return;
			if(_fitType != -1)
				FitTo(_fitType, true, true);
			else
			{
				if((PageView.HorizontalScroll.Value != PageView.ScrollHorizontal && PageView.ScrollHorizontal > 0) ||
					(PageView.VerticalScroll.Value != PageView.ScrollVertical && PageView.ScrollVertical > 0))
				{
					Subscribe_PageView_ClientSizeChanged(false);
					PageView.Hide();

					if(PageView.HorizontalScroll.Value != PageView.ScrollHorizontal && PageView.ScrollHorizontal > 0)
					{
						PageView.HorizontalScroll.Value = (PageView.ScrollHorizontal < PageView.HorizontalScroll.Minimum ? PageView.HorizontalScroll.Minimum :
																																								 (PageView.ScrollHorizontal > PageView.HorizontalScroll.Maximum ? PageView.HorizontalScroll.Maximum : PageView.ScrollHorizontal));

						PageView.scrollHorizontal.Maximum = PageView.HorizontalScroll.Maximum;
						PageView.scrollHorizontal.Value = PageView.HorizontalScroll.Value;
					}
					if(PageView.VerticalScroll.Value != PageView.ScrollVertical && PageView.ScrollVertical > 0)
					{
						PageView.VerticalScroll.Value = (PageView.ScrollVertical < PageView.VerticalScroll.Minimum ? PageView.VerticalScroll.Minimum :
																																						 (PageView.ScrollVertical > PageView.VerticalScroll.Maximum ? PageView.VerticalScroll.Maximum : PageView.ScrollVertical));

						PageView.scrollVertical.Maximum = PageView.VerticalScroll.Maximum;
						PageView.scrollVertical.Value = PageView.VerticalScroll.Value;
					}

					PageView.Show();
					Subscribe_PageView_ClientSizeChanged(true);
				}
			}
		}

		AutoResetEvent SaveSinc = new AutoResetEvent(true);

		bool isFromSelChange;
		int lastSelected = -1;

		private void PagePreview_SelectedIndexChanged(object sender, EventArgs e)
		{
			PageView.WorkMode = PicturePanel.WorkingMode.Move;

			if(_lock_item_selection_changed || PagePreview.SelectedItems.Count <= 0)
				return;
			ListViewItem lvi = PagePreview.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
			if(lvi != null && lastSelected != lvi.Index)
			{
				pageMovied |= pageMoviedExtern;
				if(Modified)
				{
					Cursor = Cursors.WaitCursor;
					OnNeedSave(new Lib.Win.ImageControl.SaveEventHandler(ChangePage));
					SaveSinc.WaitOne();
					Cursor = Cursors.Default;
				}
				isFromSelChange = true;
				_forcePage = true;
				if(PagePreview != null)
					Page = (PagePreview.SelectedItems.Count > 0) ? PagePreview.SelectedItems[0].Index + 1 : 1;
				isFromSelChange = false;
				lastSelected = _page - 1;
			}
		}

		private void PagePreview_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode != Keys.PageDown && e.KeyCode != Keys.PageUp)
				return;
			if(e.Shift)
				e.Handled = true;

			if(PagePreview.SelectedItems.Count == 0)
				return;
			if(!CanSave)
				return;

			ListViewItem lvi = PagePreview.SelectedItems[0];
			int index = lvi.Index;
			int index1 = index;

			switch(e.KeyCode)
			{
				case Keys.PageDown:
					if(PagePreview.SelectedItems[0].Index < (PagePreview.Items.Count - 1))
						index++;
					break;
				case Keys.PageUp:
					if(PagePreview.SelectedItems[0].Index > 0)
						index--;
					break;
			}
			if(index == lvi.Index)
				return;
			if(e.Shift || sender == null)
			{
				if(PageView.StampItems != null && PageView.StampItems.Count > 0)
				{
					ErrorShower.OnShowError(this,
											Environment.StringResources.GetString("DocControl_PageMove_Error"),
											Environment.StringResources.GetString("Error"));
					return;
				}

				PagePreview.SelectedIndexChanged -= PagePreviewSelectedIndexChanged;
				PagePreview.ClientSizeChanged -= PagePreviewClientSizeChanged;

				PagePreview.Items[lvi.Index].Remove();
				PagePreview.Items.Insert(index, lvi);

				PagePreview.Items.Cast<ListViewItem>().Select(x => x.Text = (x.Index + 1).ToString()).ToArray();
				PagePreview.Sort();

				int[] ids = new int[4];
				ids = ids.Select(x => -1).ToArray();

				if(index < (PagePreview.Items.Count - 1) &&
					((ItemTag)PagePreview.Items[index + 1].Tag).ItemSize.IsEmpty)
					ids[0] = index + 1;

				if(index > 0 && ((ItemTag)PagePreview.Items[index - 1].Tag).ItemSize.IsEmpty)
					ids[0] = index - 1;


				if(index1 < (PagePreview.Items.Count - 1) &&
					((ItemTag)PagePreview.Items[index1 + 1].Tag).ItemSize.IsEmpty)
					ids[0] = index1 + 1;

				if(index1 > 0 && ((ItemTag)PagePreview.Items[index1 - 1].Tag).ItemSize.IsEmpty)
					ids[0] = index1 + 1;

				if(ids.Count(x => x > -1) > 0)
					foreach(int id in ids.Where(id => id > -1))
						RefreshPagePreviewItem(id);

				PagePreview.Items[index].EnsureVisible();
				PagePreview.Items[index].Selected = true;

				pageMovied = (((ItemTag)PagePreview.Items[index].Tag).OriginNumber != (index + 1));

				if(sender != null)
				{
					_page = index + 1;
					PageView.Page = _page;
				}

				if(PageChanged != null && sender != null)
					PageChanged(this, EventArgs.Empty);


				PagePreview.ClientSizeChanged += PagePreviewClientSizeChanged;
				PagePreview.SelectedIndexChanged += PagePreviewSelectedIndexChanged;
			}
		}

		private void PagePreview_KeyUp(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp)
			{
				e.Handled = true;
				return;
			}

			if(PagePreview.SelectedItems.Count != 1)
				return;
			if(PagePreview.SelectedItems[0].Index + 1 != _page)
			{
				_lock_item_selection_changed = true;
				Page = PagePreview.SelectedItems[0].Index + 1;
				_lock_item_selection_changed = false;
			}
		}
		private void PagePreview_Scroll(object sender, EventArgs e)
		{
			RefreshPagePreview(false);
		}

		private void RefreshPagePreview(bool ensure_visible)
		{
			if(_page <= 0)
				return;

			if(PagePreview.Items.Count > 0)
			{
				if(ensure_visible && _page > 0)
					PagePreview.EnsureVisible(_page - 1);

				ListViewItem[] v = PagePreview.Items.Cast<ListViewItem>().
					Where(x => x != null && ((ItemTag)x.Tag).ItemSize.IsEmpty && !System.Drawing.Rectangle.Intersect(PagePreview.GetItemRect(x.Index), PagePreview.ClientRectangle).IsEmpty).ToArray();

				try
				{
					PagePreview.ClientSizeChanged -= PagePreviewClientSizeChanged;
					_pageVirtualRotationDictionary.UpdatePageRotation(ImageID);
					foreach(ListViewItem lvi in v)
					{
						RefreshPagePreviewItem(lvi.Index);
					}
					if(ShowThumbPanel && v != null && v.Length > 0)
						PagePreview.RedrawItems(v.First().Index, v.Last().Index, true);
				}
				finally
				{
					PagePreview.ClientSizeChanged += PagePreviewClientSizeChanged;
				}
			}
		}

		private void RefreshPagePreviewItem(int i)
		{
			double zW = (double)DocPages[i + 1].PageSizeWithRotation.Width * 3 / 560.0;
			double zH = (double)DocPages[i + 1].PageSizeWithRotation.Height * 3 / 560.0;

			double zoom = Math.Max(zW, zH);

			pdfHelper.Page = i + 1;
			Bitmap bm = pdfHelper.GetBitmap((int)Math.Round(pdfHelper.Width / zoom), (int)Math.Round(pdfHelper.Height / zoom), 96f, 96f, 0, RenderType.RGB, true, false, 0);

			// Виртуальный поворот
			var virtualRotationAngle = _pageVirtualRotationDictionary.GetPageRotation(ImageID, i);

			if(virtualRotationAngle != 0)
			{
				var rotateFlipType = RotateFlipType.RotateNoneFlipNone;

				switch(virtualRotationAngle)
				{
					case 90:
						rotateFlipType = RotateFlipType.Rotate90FlipNone;
						break;

					case 180:
						rotateFlipType = RotateFlipType.Rotate180FlipNone;
						break;

					case 270:
						rotateFlipType = RotateFlipType.Rotate270FlipNone;
						break;
				}

				if(rotateFlipType != RotateFlipType.RotateNoneFlipNone)
				{
					try
					{
						bm.RotateFlip(rotateFlipType);
					}
					catch(Exception ex)
					{
						Logger.WriteEx(new Exception("Не удалось повернуть изображение", ex));
					}
				}
			}

			Image thumbnail = new Bitmap(PagePreviewImages.ImageSize.Width, PagePreviewImages.ImageSize.Height);

			using(Graphics g = Graphics.FromImage(thumbnail))
			{
				if(bm != null)
				{
					g.DrawImage(bm, (thumbnail.Width - bm.Width) / 2, (thumbnail.Height - bm.Height) / 2);
					g.DrawRectangle(dotedPen, (thumbnail.Width - bm.Width) / 2, (thumbnail.Height - bm.Height) / 2, bm.Width, bm.Height);
					((ItemTag)PagePreview.Items[i].Tag).ItemSize = bm.Size;
				}
			}

			PagePreviewImages.Images[i] = thumbnail;

			if(bm != null)
				bm.Dispose();

			thumbnail.Dispose();
		}

		/// <summary>
		/// Обновить картинку
		/// </summary>
		private void RefreshPageView()
		{
			if(string.IsNullOrEmpty(_file_name) || !File.Exists(_file_name))
				return;

			Subscribe_PageView_ClientSizeChanged(false);

			if(_fitType == -1)
			{
				Image page = null;
				try
				{
					pdfHelper.Page = _page;

					if(_zoom <= RenderLimit)
						page = pdfHelper.GetBitmap((int)Math.Round((pdfHelper.Width * 96 * _zoom / 7200.0)), (int)Math.Round((pdfHelper.Height / 72.0 * 96.0) * (_zoom / 100.0)), 96f, 96f, 0, RenderType.RGB, true, false, 0);
					else
						page = pdfHelper.GetBitmap((int)Math.Round(pdfHelper.Width * 96 * RenderLimit / 7200.0), (int)Math.Round(pdfHelper.Height * 96.0 * RenderLimit / 7200.0), 96f, 96f, 0, RenderType.RGB, true, false, 0);

					if(page != null)
					{
						if(DocPages[_page].CurrentRotate != 0)
							RotateImage(page, (RotateFlipType)DocPages[_page].CurrentRotate);

						// Виртуальный поворот
						_pageVirtualRotationDictionary.UpdatePageRotation(ImageID, _page - 1);
						var virtualRotationAngle = _pageVirtualRotationDictionary.GetPageRotation(ImageID, _page - 1);

						if(virtualRotationAngle != 0)
						{
							var rotateFlipType = RotateFlipType.RotateNoneFlipNone;

							switch(virtualRotationAngle)
							{
								case 90:
									rotateFlipType = RotateFlipType.Rotate90FlipNone;
									break;

								case 180:
									rotateFlipType = RotateFlipType.Rotate180FlipNone;
									break;

								case 270:
									rotateFlipType = RotateFlipType.Rotate270FlipNone;
									break;
							}

							if(rotateFlipType != RotateFlipType.RotateNoneFlipNone)
							{
								try
								{
									page.RotateFlip(rotateFlipType);
								}
								catch(Exception ex)
								{
									Logger.WriteEx(new Exception("Не удалось повернуть изображение(виртуальный поворот)", ex));
								}
							}
						}

						PageView.Image = page;
						PageView.ResumeLayout();
					}
				}
				finally
				{
					if(page != null)
						page.Dispose();
				}
			}
			else
				FitTo(_fitType, true, true);

			PageView.Zoom = _zoom;

			Subscribe_PageView_ClientSizeChanged(true);
		}

		#endregion

		public void ZoomToSelection()
		{
			if(PageView.SelectionModeRectangle.Width <= 0 || PageView.SelectionModeRectangle.Height <= 0)
				return;

			double z;

			double cZ = (_zoom > RenderLimit ? Math.Round(PageView.Zoom / 100.0) : 1);
			if(PageView.SelectionModeRectangle.Width > PageView.SelectionModeRectangle.Height)
				z = (PageView.ClientRectangle.Width / (double)PageView.SelectionModeRectangle.Width) / cZ;
			else
				z = (PageView.ClientRectangle.Height / (double)PageView.SelectionModeRectangle.Height) / cZ;

			int xs = (int)Math.Round(PageView.SelectionModeRectangle.X * z * cZ);
			int ys = (int)Math.Round(PageView.SelectionModeRectangle.Y * z * cZ);

			Zoom = (int)(z * _zoom);

			PageView.Visible = false;
			PageView.HorizontalScroll.Value = (PageView.HorizontalScroll.Maximum > xs ? xs : PageView.HorizontalScroll.Maximum);
			PageView.VerticalScroll.Value = (PageView.VerticalScroll.Maximum > ys ? ys : PageView.VerticalScroll.Maximum);
			PageView.Visible = true;
		}

		public void FitTo(short option, bool repaint)
		{
			Subscribe_PageView_ClientSizeChanged(false);
			FitTo(option, repaint, false);
			Subscribe_PageView_ClientSizeChanged(true);
		}

		private void FitTo(short option, bool repaint, bool dopaint)
		{
			short curFit = _fitType;
			_fitType = option;

			if(string.IsNullOrEmpty(_file_name) || !File.Exists(_file_name))
				return;

			if(PageView == null) return;

			if(_page <= 0) return;

			if(!repaint)
				return;

			Bitmap page = null;
			try
			{
				pdfHelper.Page = ((ItemTag)(PagePreview.Items[_page - 1].Tag)).OriginNumber;

				if(pdfHelper.Page == 0)
					return;
				double w = pdfHelper.Width * 4 / 3.0;
				double h = pdfHelper.Height * 4 / 3.0;


				// Виртуальный поворот
				//_pageVirtualRotationDictionary.UpdatePageRotation(ImageID, _page - 1);

				var virtualRotation = _pageVirtualRotationDictionary.GetPageRotation(ImageID, _page - 1);

				var angle = DocPages[pdfHelper.Page].CurrentRotate;

				switch(virtualRotation)
				{
					case 90:
						angle += 1;
						break;
					case 180:
						angle += 2;
						break;
					case 270:
						angle += 3;
						break;
				}

				bool isAlbum = (angle % 2 == 1);

				int z = 0;
				switch(option)
				{
					case 0:
						double cW = (isAlbum ? h : w);
						double cH = (isAlbum ? w : h);

						double zw = PageView.Width / cW;
						double zh = PageView.Height / cH;

						z = (int)Math.Round(100.0 * Math.Min(zw, zh));

						break;
					case 1:
						if(isAlbum)
							z = (int)Math.Round(100.0 * (PageView.Width / h));
						else
							z = (int)Math.Round(100.0 * (PageView.Width / w));

						break;
					case 2:
						if(isAlbum)
							z = (int)Math.Round(100.0 * (PageView.Height / w));
						else
							z = (int)Math.Round(100.0 * (PageView.Height / h));

						break;
				}

				if(_zoom == z && !dopaint && curFit != -1)
				{
					pdfHelper.Flush();
					return;
				}

				_zoom = z;
				_fakeZoom = _zoom;

				page = pdfHelper.GetBitmap((int)Math.Round(pdfHelper.Width * 4 * _zoom / 300.0), (int)Math.Round(pdfHelper.Height * 4 * _zoom / 300.0), 96f, 96f, 0, RenderType.RGB, true, false, 0);

				if(page != null)
				{
					if(angle != 0)
					{
						angle %= 4;

						RotateImage(page, (RotateFlipType)angle);
					}

					int wR, hR;
					if(option == 1)
					{
						int adjustment = (page.Height > PageView.ClientSize.Height)
											 ? SystemInformation.HorizontalScrollBarHeight + 2
											 : 0;
						if(adjustment > 0)
							wR = PageView.Width - adjustment;
						else if(page.Width > PageView.Width)
							wR = PageView.Width - 2;
						else
							wR = page.Width;
						hR = page.Height;
					}
					else if(option == 2)
					{
						int adjustment = (page.Width > PageView.ClientSize.Width)
											 ? SystemInformation.HorizontalScrollBarHeight + 2
											 : 0;
						wR = page.Width;
						if(adjustment > 0)
							hR = PageView.Height - adjustment;
						else if(page.Height > PageView.Height)
							hR = PageView.Height - 2;
						else
							hR = page.Height;
					}
					else
					{
						PageView.Image = page;
						wR = page.Width;
						hR = page.Height;
					}

					page = new Bitmap(page, wR, hR);

					PageView.Image = page.Clone(new System.Drawing.Rectangle(0, 0, wR, hR), PixelFormat.Format24bppRgb);
					PageView.Image = page;
					PageView.FitType = _fitType;
					PageView.Zoom = _zoom;

					PageView.IsRefreshBitmap = true;
					Invalidate(PageView.Bounds, true);
				}
			}
			catch(ArgumentException ex)
			{
				Data.Env.WriteToLog(ex);
			}
			finally
			{
				if(page != null)
					page.Dispose();
				page = null;
			}
		}

		internal string SaveWithBurnAndResolution(int startPage, int endPage, string tempFileName, int horizontalResolution, int verticalResolution, bool saveColor, bool isFax)
		{
			#region Вариант для сохранения в tif

			if(isFax)
			{
				List<Tiff.PageInfo> il = new List<Tiff.PageInfo>();

				List<StampItem> stL = null;
				if(PageView.StampItems != null && PageView.StampItems.Count > 0)
					stL = PageView.StampItems.Select(x => x.StmpItem).ToList();

				il = pdfHelper.GetBitmapsCollectionFromFile(_file_name, startPage, endPage, saveColor);

				for(int i = 0; i < il.Count; i++)
				{
					Tiff.PageInfo info = il[i];
					if(info != null)
					{
						PixelFormat pf;
						Bitmap bmp = info.Image;
						pf = bmp.PixelFormat;
						if(bmp.HorizontalResolution > horizontalResolution)
							if(bmp.VerticalResolution > verticalResolution)
								bmp.SetResolution(horizontalResolution, verticalResolution);
							else
								bmp.SetResolution(horizontalResolution, bmp.VerticalResolution);

						if(bmp.VerticalResolution > verticalResolution)
							bmp.SetResolution(bmp.HorizontalResolution, verticalResolution);
						if(stL != null && stL.Count > 0)
						{
							var pageStamps = stL.Where(x => x.Page == i + startPage);
							if(pageStamps != null && pageStamps.Count() > 0)
							{
								if(bmp.PixelFormat != PixelFormat.Format24bppRgb ||
									bmp.PixelFormat != PixelFormat.Format32bppArgb)
								{
									Bitmap tm = new Bitmap(bmp);
									tm.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
									bmp = tm;
									tm = null;
								}
								float drez = bmp.VerticalResolution / bmp.HorizontalResolution;
								Matrix mx = null;
								using(Graphics g = Graphics.FromImage(bmp))
								{
									foreach(StampItem si in pageStamps)
									{
										if(si.TypeID == 101)
											continue;
										Image img = Environment.GetStamp(si.StampID, imageID);
										if(!si.Rotate.Equals(0))
										{
											mx = new Matrix();
											mx.RotateAt(si.Rotate,
														new PointF(si.X + (img.Width * si.Zoom) / 200,
																   si.Y + (img.Height * si.Zoom * drez) / 200));
											g.Transform = mx;
										}
										g.DrawImage(img, si.X, si.Y, (img.Width * si.Zoom) / 100,
													(img.Height * si.Zoom * drez) / 100);
										if(!si.Rotate.Equals(0))
											g.ResetTransform();
									}
								}
							}
						}

						if(!saveColor && bmp.PixelFormat != PixelFormat.Format1bppIndexed)
							bmp = Environment.LibTiff.ConvertToBitonal(bmp);

						info.Image = bmp;
					}
				}
				if(il.Count > 0)
					Environment.LibTiff.SaveBitmapsCollectionToFile(tempFileName, il, saveColor);
				else
					return "";
				return tempFileName;
			}

			#endregion

			if(string.IsNullOrEmpty(_file_name) || !File.Exists(_file_name))
				return "";

			if(pdfHelper.PageCount > 0)
			{
				int i = startPage;
				int[] pages = new int[endPage - startPage + 1];
				pages = pages.Select(x => x = i++).ToArray();

				if(pages.Length > 0)
				{
					FileSave4Mail_Fax(_file_name, tempFileName, pages);
				}
				else
					return "";

				return tempFileName;
			}

			return "";
		}

		public Bitmap CreateImageListFromSelectedImage()
		{
			try
			{
				int cZ = (_zoom > RenderLimit ? RenderLimit : _zoom);
				Bitmap image = new Bitmap(PageView.SelectionModeRectangle.Width * 100 / cZ, PageView.SelectionModeRectangle.Height * 100 / cZ);
				image.SetResolution(PageView.OriginalBitmap.HorizontalResolution, PageView.OriginalBitmap.VerticalResolution);
				using(Graphics g = Graphics.FromImage(image))
				{
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					Bitmap bm = pdfHelper.GetBitmap((int)Math.Round(pdfHelper.Width * 96 / 72.0), (int)Math.Round(pdfHelper.Height / 72.0 * 96.0), 96f, 96f, 0, RenderType.RGB, true, false, 0);
                    g.DrawImage(bm, new System.Drawing.Rectangle(0, 0, image.Width, image.Height), new Rectangle((int)Math.Round(PageView.SelectionModeRectangle.X * 100.0 / cZ), (int)Math.Round(PageView.SelectionModeRectangle.Y * 100.0 / cZ), image.Width, image.Height), GraphicsUnit.Pixel);
				}

				return image;
			}
			catch(Exception ex)
			{
				OnErrorMessage(new Exception("CreateSelectedImage error", ex));
			}
			return null;
		}

		protected ImageControl.ImageControl.ErrorMessageHandler errorMessageHandler;
		protected void OnErrorMessage(Exception ex)
		{
			if(errorMessageHandler != null)
				errorMessageHandler(ex);
		}

		/// <summary>
		/// Находится-ли контрол в режиме выделения
		/// </summary>
		[Browsable(false)]
		public bool SelectionMode
		{
			get { return IsSelectionMode; }
			set { IsSelectionMode = value; }
		}

		[Browsable(false)]
		public bool IsSelectionMode
		{
			get
			{
				return PageView.WorkMode == PicturePanel.WorkingMode.Select;
			}
			set
			{
				if(value)
				{
					PageView.WorkMode = PicturePanel.WorkingMode.Select;
				}
			}
		}

		/// <summary>
		/// Включение режима редактирования заметок
		/// </summary>
		[Browsable(false)]
		public bool IsEditNotes
		{
			get { return PageView.WorkMode == PicturePanel.WorkingMode.EditNotes; }
			set
			{
				if(value)
				{
					PageView.WorkMode = PicturePanel.WorkingMode.EditNotes;
				}

			}
		}

		/// <summary>
		/// Включение рижима передвижения картинки
		/// </summary>
		[Browsable(false)]
		public bool IsMoveImage
		{
			get { return PageView.WorkMode == PicturePanel.WorkingMode.Move; }
			set
			{
				if(value)
				{
					PageView.WorkMode = PicturePanel.WorkingMode.Move;
				}

			}
		}

		public bool HasAnnotation()
		{
			return PageView != null && PageView.StampItems != null && PageView.StampItems.Count(x => x.StmpItem.Page == Page) > 0;
		}

		internal void MovePage(int page)
		{
			if(!CanSave)
				return;
			if(FileName != null && page > 0 && page < PageCount)
			{
				int index = page == Page ? page : page - 1;
				bool isForward = page == Page;

				KeyEventArgs e = new KeyEventArgs((isForward ? Keys.PageDown : Keys.PageUp));
				PagePreview_KeyDown(null, e);

				pageMovied = false;
				pageMoviedExtern = (((ItemTag)PagePreview.Items[index].Tag).OriginNumber != (index + 1)); //true;
			}
		}

		internal bool SetImagePalette(int type)
		{
			bool result = false;
			if(PageView != null)
			{
				switch(type)
				{
					case 1:
						PageView.CurrentPixelFormat = PixelFormat.Format24bppRgb;
						result = true;
						break;
					case 2:
						PageView.CurrentPixelFormat = PixelFormat.Format8bppIndexed;
						result = true;
						break;
					case 3:
						PageView.CurrentPixelFormat = PixelFormat.Format1bppIndexed;
						result = true;
						break;
				}
			}
			if(result)
			{
				PageView.IsRefreshBitmap = true;
				PageView.Invalidate(true);
			}
			return result;
		}

		internal int GetImagePalette()
		{
			if(PageView != null)
			{
				switch(PageView.CurrentPixelFormat)
				{
					case PixelFormat.Format24bppRgb:
						return 1;

					case PixelFormat.Format8bppIndexed:
						return 2;

					case PixelFormat.Format1bppIndexed:
						return 3;
				}
			}
			return 0;
		}

		public bool SetDisplayScaleAlgorithm(int type)
		{
			bool result = false;
			if(PageView != null)
			{
				switch(type)
				{
					case 1:
						PageView.CurrentInterpolationMode = InterpolationMode.Low;
						result = true;
						break;
					case 2:
						PageView.CurrentInterpolationMode = InterpolationMode.High;
						result = true;
						break;
					case 3:
						PageView.CurrentInterpolationMode = InterpolationMode.NearestNeighbor;
						result = true;
						break;
				}
			}
			if(result)
			{
				PageView.IsRefreshBitmap = true;
				PageView.Invalidate(true);
			}
			return result;
		}

		public int GetDisplayScaleAlgorithm()
		{
			if(PageView != null)
			{
				switch(PageView.CurrentInterpolationMode)
				{
					case InterpolationMode.Low:
						return 1;

					case InterpolationMode.High:
						return 2;

					case InterpolationMode.NearestNeighbor:
						return 3;
				}
			}
			return 0;
		}

		public void ShowProperties()
		{
			(new PropertiesDialogs.PropertiesPDFDialog(_file_name, _filePass, _page)).Show();
		}

		internal StampItem GetEditedDSP()
		{
			if(PageView.StampItems != null && PageView.StampItems.FirstOrDefault(x => x.StmpItem.TypeID == 101) != null)
				return PageView.StampItems.First(x => x.StmpItem.TypeID == 101).StmpItem;
			return null;
		}
	}
}