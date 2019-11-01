using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kesco.Lib.Win.ImageControl;

namespace Kesco.Lib.Win.Document.Controls.PdfViewControl
{
    public class PicturePanel : Panel
    {
        private MouseEventHandler MouseMoveHandler, MouseDownHandler;

        protected override void Dispose(bool disposing)
        {
            if (StampItems != null)
            {
                StampItems.Clear();
                StampItems = null;
            }

            if (scrollHorizontal != null)
            {
                scrollHorizontal.Dispose();
                scrollHorizontal = null;
            }
            if (scrollVertical != null)
            {
                scrollVertical.Dispose();
                scrollVertical = null;
            }

            if (_picture != null)
            {
                _picture.Dispose();
                _picture = null;
            }

            base.Dispose(disposing);
        }

        [Browsable(false)]
        public double Zoom
        {
            get { return _picture.Zoom; }
            set
            {
                if (_picture.Zoom == value)
                    return;

                if (FitType != -1)
                {
                    _picture.Zoom = value;
                    return;
                }

                if (value > PDFView.RenderLimit && _picture != null)
                {
                    _picture.SizeMode = PictureBoxSizeMode.Zoom;

                    _picture.Size = new Size((int) (_picture.Width*value/100.0),
                                             (int) (_picture.Height*value/100.0));

                    scrollHorizontal.Maximum = HorizontalScroll.Maximum;
                    scrollVertical.Maximum = VerticalScroll.Maximum;

                    scrollHorizontal.Value = (scrH > scrollHorizontal.Maximum ? scrollHorizontal.Maximum : scrH);
                    scrollVertical.Value = (scrV > scrollVertical.Maximum ? scrollVertical.Maximum : scrV);
                }
                else
                    _picture.SizeMode = PictureBoxSizeMode.Normal;

                _picture.Zoom = value;
            }
        }

        public short FitType = -1;

        public PicturePanel()
        {
            DoubleBuffered = true;
            BackColor = SystemColors.Control; //Window;

            _picture.Left = 0;
            _picture.SizeMode = PictureBoxSizeMode.Normal; // Zoom;
            _picture.Top = 0;
            _picture.TabStop = true;
            _picture.TabIndex = 0;
            _picture.BackColor = SystemColors.Control;

            Controls.Add(_picture);

            MouseMoveHandler = _picture_MouseMove;
            MouseDownHandler = _picture_MouseDown;

            Scroll += PicturePanel_Scroll;
        }

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			//if(false)
			//if(((HandledMouseEventArgs)e).Handled)
			//{
			//    ((HandledMouseEventArgs)e).Handled = false;
				base.OnMouseWheel(e);
			//}
		}

        private void PicturePanel_Scroll(object sender, ScrollEventArgs e)
        {
            scrollHorizontal.Maximum = HorizontalScroll.Maximum;
            scrollHorizontal.Value = HorizontalScroll.Value;

            scrollVertical.Maximum = VerticalScroll.Maximum;
            scrollVertical.Value = VerticalScroll.Value;
        }

        private void _picture_Paint(object sender, PaintEventArgs e)
        {
            HorizontalScroll.Value = scrollHorizontal.Value;
            VerticalScroll.Value = scrollVertical.Value;

            if (HorizontalScroll.Value == scrollHorizontal.Value &&
                VerticalScroll.Value == scrollVertical.Value)
                _picture.Paint -= _picture_Paint;
        }

        internal void mi_Click(object sender, EventArgs e)
        {
			if(StampItems == null)
				return;
            int[] idx = StampItems.Where(x => x.IsSelected).OrderByDescending(x => x.StmpItem.ID).Select(x => x.StmpItem.ID).ToArray();

            if (idx.Length <= 0)
                return;
            var mf =
                new MessageForm(
                    Environment.StringResources.GetString("ImageShowControl_mi_Click_Message1"),
                    Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNo);
            if (mf.ShowDialog() != DialogResult.Yes)
                return;
            foreach (int i in idx.Where(i => !Environment.DocSignatureData.Delete(i)))
                break;

            ClearStampsSelection();

            ReloadStamps();
        }

        public enum WorkingMode
        {
            Move = 0,
            EditNotes = 1,
            Select = 7,
            EditNote = 8,
            DrawImage = 9,
            None = 11
        };

        [Browsable(false)]
        public WorkingMode WorkMode
        {
            get { return _picture.WorkMode; }
            set
            {
                _picture.WorkMode = value;
                _picture.RemoveSelected();

                if (value != WorkingMode.EditNotes && value != WorkingMode.EditNote)
                    ClearStampsSelection();

                switch (value)
                {
                    case WorkingMode.Move:
                        _picture.MouseMove += MouseMoveHandler;
                        _picture.MouseDown += MouseDownHandler;

                        _picture.Cursor = TiffCursors.Hand;
                        break;
                    case WorkingMode.Select:
                        _picture.Cursor = Cursors.Default;

                        break;
                    default:
                        _picture.MouseMove -= MouseMoveHandler;
                        _picture.MouseDown -= MouseDownHandler;
                        _picture.Cursor = Cursors.Default;

                        break;
                }
            }
        }

        #region Объявление структур, переменных

        public MyPictureBox _picture = new MyPictureBox();

        [Browsable(false)]
        public IntPtr PictureBoxHandle
        {
            get { return (_picture != null ? _picture.Handle : IntPtr.Zero); }
        }

        public void SetPictureBoxSize(int w, int h)
        {
            if (_picture == null)
                return;
            _picture.Width = w;
            _picture.Height = h;
        }

        #endregion

        [Browsable(false)]
        public Image Image
        {
            get { return _picture.Image; }
            set
            {
                if (value != null)
                    _picture.RemoveSelected();

                ClearStampsSelection();
                if (_picture.Image != null)
                {
                    _picture.Image.Dispose();
                    _picture.Image = null;
                }

                if (_picture.cachedBitmap != null)
                    _picture.cachedBitmap.Dispose();

                if (_picture.originalBitmap != null)
                    _picture.originalBitmap.Dispose();

                //Zoom = 100;
                _picture.SizeMode = PictureBoxSizeMode.Normal;
                _picture.Width = (int) (_picture.Width*100/_picture.Zoom);
                _picture.Height = (int) (_picture.Height*100/_picture.Zoom);
                _picture.Zoom = 100;

                if (value != null)
                {
                    _picture.Image = new Bitmap(value.Width, value.Height, PixelFormat.Format24bppRgb);
                    using (Graphics g = Graphics.FromImage(_picture.Image))
                    {
                        g.FillRectangle(SystemBrushes.Control, 0, 0, _picture.Image.Width, _picture.Image.Height);
                    }
                    _picture.Size = _picture.Image.Size;

                    _picture.originalBitmap = (Bitmap) value.Clone();

                    scrH = scrollHorizontal.Value;
                    scrV = scrollVertical.Value;

                    scrollHorizontal.Maximum = HorizontalScroll.Maximum;
                    scrollVertical.Maximum = VerticalScroll.Maximum;
                }
                else
                {
                    IsRefreshBitmap = false;

                    HorizontalScroll.Value = 0;
                    VerticalScroll.Value = 0;
                }
            }
        }

        [Browsable(false)]
        public bool ImageDisplayed
        {
            get { return _picture != null && _picture.Image != null; }
        }

        public void ClearStampsSelection()
        {
			CurrentStampID = 0;
            if (StampItems == null || StampItems.Count <= 0) return;
            foreach (Stamp si in StampItems.Where(si => si.IsSelected))
            {
                si.IsSelected = false;
                IsRefreshBitmap = true;
            }
            if (IsRefreshBitmap)
                Invalidate(true);
        }

        /// <summary>
        ///   Горизонтальный скрол картинок
        /// </summary>
        public HScrollBar scrollHorizontal = new HScrollBar();

        private int scrH;
        private bool hScrollSet;

        public int ScrollHorizontal
        {
            get { return scrollHorizontal.Value; }
            set
            {
                scrollHorizontal.Value = value;
                hScrollSet = true;
                if (vScrollSet && (scrollVertical.Value > 0 || value > 0))
                    _picture.Paint += _picture_Paint;
            }
        }

        /// <summary>
        ///   Вертикальный скрол картинок
        /// </summary>
        public VScrollBar scrollVertical = new VScrollBar();

        private int scrV;
        private bool vScrollSet;

        public int ScrollVertical
        {
            get { return scrollVertical.Value; }
            set
            {
                scrollVertical.Value = value;
                vScrollSet = true;
                if (hScrollSet && (scrollHorizontal.Value > 0 || value > 0))
                    _picture.Paint += _picture_Paint;
            }
        }

        private void _picture_MouseDown(object sender, MouseEventArgs e)
        {
            Point mPoint = PointToClient(Cursor.Position);

            if (_picture.WorkMode == WorkingMode.Move)
            {
                if (_picture != null && e.Button == MouseButtons.Left)
                {
                    _picture.Cursor = TiffCursors.HandDrag;

                    scrollHorizontal.Maximum = HorizontalScroll.Maximum;
                    scrollHorizontal.Value = HorizontalScroll.Value;

                    scrollVertical.Maximum = VerticalScroll.Maximum;
                    scrollVertical.Value = VerticalScroll.Value;

                    x0 = mPoint.X;
                    y0 = mPoint.Y;
                }
            }
        }

        private void _picture_MouseMove(object sender, MouseEventArgs e)
        {
            Point mPointOffset = PointToClient(Cursor.Position);

            int pX = mPointOffset.X;
            int pY = mPointOffset.Y;

            if (WorkMode == WorkingMode.Move && e.Button == MouseButtons.Left) // после нажатия для перемещения картики
            {
                #region Перетаскивание картинки

                if (!(pX < 0 || pY < 0 || pX > Width || pY > Height))
                {
                    int delta;
                    int dir;
                    if (x0 != pX)
                    {
                        dir = (x0 > pX ? 1 : -1);
                        if ((dir < 0 && scrollHorizontal.Value > scrollHorizontal.Minimum) ||
                            (dir > 0 && scrollHorizontal.Value < scrollHorizontal.Maximum))
                        {
                            delta = scrollHorizontal.Value + (x0 - pX);
                            scrollHorizontal.Value = (delta < scrollHorizontal.Minimum
                                                          ? scrollHorizontal.Minimum
                                                          : (delta > scrollHorizontal.Maximum
                                                                 ? scrollHorizontal.Maximum
                                                                 : delta));
                            HorizontalScroll.Value = scrollHorizontal.Value;
                        }
                    }

                    if (y0 != pY)
                    {
                        dir = (y0 > pY ? 1 : -1);
                        if ((dir < 0 && scrollVertical.Value > scrollVertical.Minimum) ||
                            (dir > 0 && scrollVertical.Value < scrollVertical.Maximum))
                        {
                            delta = scrollVertical.Value + (y0 - pY);
                            scrollVertical.Value = (delta < scrollVertical.Minimum
                                                        ? scrollVertical.Minimum
                                                        : (delta > scrollVertical.Maximum
                                                               ? scrollVertical.Maximum
                                                               : delta));
                            VerticalScroll.Value = scrollVertical.Value;
                        }
                    }
                }
                x0 = pX;
                y0 = pY;

                #endregion
            }
        }

        /// <summary>
        ///   Последний x мыши при драге картинки
        /// </summary>
        private int x0;

        /// <summary>
        ///   Последний y мыши при драге картинки
        /// </summary>
        private int y0;

        /// <summary>
        ///   Текщая интерполяция
        /// </summary>
        internal InterpolationMode CurrentInterpolationMode =
            InterpolationMode.High;

        [Browsable(false)]
        public Bitmap CachedBitmap
        {
            get { return _picture.cachedBitmap; }
        }

        [Browsable(false)]
        public Bitmap OriginalBitmap
        {
            get { return _picture.originalBitmap; }
        }

        [Browsable(false)]
        public Rectangle SelectionModeRectangle
        {
            get { return _picture.SelectionModeRectangle; }
        }

        [Browsable(false)]
        public Image CurrentStamp
        {
            get { return _picture.CurrentStamp; }
            set { _picture.CurrentStamp = value; }
        }

        /// <summary>
        ///   Код текущего штампа
        /// </summary>
        [Browsable(false)]
        public int CurrentStampID
        {
            get { return _picture.CurrentStampID; }
            set { _picture.CurrentStampID = value; }
        }

        [Browsable(false)]
        public int CurrentDocID { get; set; }

        public bool IsAnnuled;

        public int ImageID;
        public int Page;

        public bool RectDrawn()
        {
            return _picture.WorkMode == WorkingMode.Select && SelectionModeRectangle != Rectangle.Empty;
        }

        public void BeginCreationFigure(ImageControl.ImageControl.Figures figure)
        {
            _picture.BeginCreationFigure(figure);
        }

        private int _thicknessFrame;

        [Browsable(false)]
        public int ThicknessFrame
        {
            get
            {
                if (_thicknessFrame == 0 && BorderStyle != BorderStyle.None)
                    _thicknessFrame = (Size.Width - ClientSize.Width)/2;

                return _thicknessFrame;
            }
        }

        public delegate void AnnotationCreatedHandler(int x, int y, int r);

        public event AnnotationCreatedHandler AnnotationCreated;

        internal void CreateAnnotation(int x, int y, int r)
        {
            if (AnnotationCreated != null)
                AnnotationCreated(x, y, r);
        }

        public event EventHandler NeedToReloadStamps;

        internal void ReloadStamps()
        {
            if (NeedToReloadStamps != null)
				NeedToReloadStamps(this, EventArgs.Empty);
        }

		public event EventHandler StampsModified;

		internal void OnStampsModified()
		{
			if(StampsModified != null)
				StampsModified(this, EventArgs.Empty);
		}

        internal List<Stamp> StampItems;

        /// <summary>
        ///   Надо ли перирисовывать закешированный битмап рисунка
        /// </summary>
        internal bool IsRefreshBitmap = true;

        [Browsable(false)]
        public PixelFormat CurrentPixelFormat
        {
            get { return _picture != null ? _picture.CurrentPixelFormat : PixelFormat.Format32bppArgb; }
            set
            {
                if (_picture != null)
                    _picture.CurrentPixelFormat = value;
            }
        }

		internal bool ISStampModified()
		{
			return StampItems != null && StampItems.Where(x => x.StmpItem.X != x.StmpItem.OriginalX || x.StmpItem.Y != x.StmpItem.OriginalY || x.StmpItem.Rotate != x.StmpItem.OriginalRotate).Count() > 0;
		}
	}

    internal class Stamp
    {
        public Data.Temp.Objects.StampItem StmpItem;

        public int Width;
        public int Height;
        public bool IsSelected;
    }

    public class MyPictureBox : PictureBox
    {
        protected override void Dispose(bool disposing)
        {
            if (CurrentStamp != null)
            {
                CurrentStamp.Dispose();
                CurrentStamp = null;
            }

            if (invalBitmap != null)
            {
                invalBitmap.Dispose();
                invalBitmap = null;
            }

            if (cachedBitmap != null)
            {
                cachedBitmap.Dispose();
                cachedBitmap = null;
            }

            if (originalBitmap != null)
            {
                originalBitmap.Dispose();
                originalBitmap = null;
            }

            if (Image != null)
            {
                Image.Dispose();
                Image = null;
            }

            if (InvokeRequired)
                Invoke(new Action<bool>(base.Dispose), new object[] {disposing});
            else
                base.Dispose(disposing);
        }

        public void BeginCreationFigure(ImageControl.ImageControl.Figures figure)
        {
            if (Image == null)
                return;
            if (figure == ImageControl.ImageControl.Figures.EmbeddedImage)
            {
                ((PicturePanel) Parent).WorkMode = PicturePanel.WorkingMode.DrawImage;
                Cursor = Cursors.NoMove2D;
            }
        }

        private Image _currentStamp;

        /// <summary>
        ///   Изображение текущего выбранного штампа
        /// </summary>
        public Image CurrentStamp
        {
            get { return _currentStamp; }
            set
            {
				//if (_currentStamp != null)
				//    _currentStamp.Dispose();
                _currentStamp = value;
                lastPositionForDrag = new Point(scrollX, scrollY);
            }
        }

        /// <summary>
        ///   Код текущего штампа
        /// </summary>
        public int CurrentStampID 
		{
			get
			{
				return currentStampID;
			}
			set
			{
				currentStampID = value;
				if(currentStampID < 1)
					parent.AutoScroll = true;
			}
		}

		private int currentStampID;
		private int currentSignID;

        private double zoom = 100;

        public double Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }


        /// <summary>
        ///   Ширина и высота выделения для фигуры
        /// </summary>
        protected const int WidthSelectedtRect = 8;

        /// <summary>
        ///   Массив выделений для фигур
        /// </summary>
        protected Rectangle[] selectedRectangles;

        /// <summary>
        ///   Координата последней позиции заметки
        /// </summary>
        protected Point lastPositionForDrag;

        /// <summary>
        ///   Отступ скролла по горизонтали
        /// </summary>
        protected int scrollX
        {
            get { return (((PicturePanel) Parent).HorizontalScroll.Value); }
        }

        //Visible ? : 0); } }
        /// <summary>
        ///   Отступ скролла по вертикали
        /// </summary>
        protected int scrollY
        {
            get { return ((PicturePanel) Parent).HorizontalScroll.Value; }
        }

        public PicturePanel.WorkingMode WorkMode = PicturePanel.WorkingMode.None;
        public Rectangle SelectionModeRectangle = Rectangle.Empty;

        private Rectangle oldRect;

        // Кэш текущего отбражения оригинала(со штампами)
        public Bitmap cachedBitmap;

        // Кэш оригинальной картинки
        public Bitmap originalBitmap;

        /// <summary>
        ///   нажата кнопка "Control"
        /// </summary>
        private bool controlPushed;

        /// <summary>
        ///   нажата кнопка "Shift"
        /// </summary>
        private bool shiftPushed;

        public PixelFormat CurrentPixelFormat = PixelFormat.Format24bppRgb;


        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyData == (Keys.Delete | Keys.Shift))
                parent.mi_Click(this, EventArgs.Empty);
        }

        private Bitmap ConvertTo(PixelFormat format, Bitmap image)
        {
            Bitmap result = image;
            if (format == PixelFormat.Format1bppIndexed && image.PixelFormat != PixelFormat.Format1bppIndexed)
            {
                result = Environment.LibTiff.ConvertToBitonal(image);
                image = null;
            }
            else if (format == PixelFormat.Format8bppIndexed &&
                     (image.PixelFormat != PixelFormat.Format8bppIndexed || !IsPalleteGrayscale(image.Palette)))
            {
				result = Environment.LibTiff.ConvertToGrayscale(image);
            }
            if (result == null)
                result = image;
            else
            {
                if (!result.Equals(image))
                {
                    if (image != null)
                    {
                        image.Dispose();
                        image = null;
                    }
                }
            }
            return result;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
#if AdvancedLogging
			Log.Logger.EnterMethod(this, "OnPaint(PaintEventArgs e)");
#endif
			base.OnPaint(e);

            if (parent.IsRefreshBitmap && originalBitmap != null)
            {
                Bitmap bufferAnimatedImage = originalBitmap;
                if (originalBitmap.PixelFormat != CurrentPixelFormat ||
                    (CurrentPixelFormat == PixelFormat.Format8bppIndexed &&
                     !IsPalleteGrayscale(originalBitmap.Palette)))
                {
                    Bitmap tempImage = ConvertTo(CurrentPixelFormat, bufferAnimatedImage.Clone() as Bitmap);
                    if (tempImage != null)
                        bufferAnimatedImage = tempImage;
                }

                if (cachedBitmap != null)
                    cachedBitmap.Dispose();
				cachedBitmap = bufferAnimatedImage.Clone(new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height),
                        PixelFormat.Format24bppRgb);

                DrawStamps(false);

                if (bufferAnimatedImage != originalBitmap)
                {
                    bufferAnimatedImage.Dispose();
                    bufferAnimatedImage = null;
                }

                parent.IsRefreshBitmap = false;
            }

            if (WorkMode == PicturePanel.WorkingMode.Select)
            {
            }
            else if (showStamp && CurrentStamp != null &&
                     (WorkMode == PicturePanel.WorkingMode.DrawImage ||
                      WorkMode == PicturePanel.WorkingMode.EditNote && Cursor == Cursors.NoMove2D))
            {
                showStamp = false;

                e.Graphics.DrawImage(CurrentStamp, oldRect);

                invalRec.X = (oldRect.X - 1);
                invalRec.Y = (oldRect.Y - 1);
                invalRec.Width = (oldRect.Width + 2);
                invalRec.Height = (oldRect.Height + 2);
			}
#if AdvancedLogging
			Log.Logger.LeaveMethod(this, "OnPaint(PaintEventArgs e)");
#endif
		}

        private void DrawStamps(bool onlySelection, bool clear = false)
        {
			using(Graphics gr = Graphics.FromImage(cachedBitmap))
			{
				if(parent.StampItems != null && parent.StampItems.Count > 0)
				{
					int indent = WidthSelectedtRect >> 1;
					if(clear)
					{
						Rectangle r = Rectangle.Empty;
						foreach(Stamp si in parent.StampItems)
						{
							if(si.StmpItem.Page != parent.Page)
								continue;
							Image img = null;

							if(!onlySelection)
							{
								if(si.StmpItem.TypeID == 101)
									img = Environment.GetDSP();
								else
									img = Environment.GetStamp(si.StmpItem.StampID, si.StmpItem.ImageID);

								si.Width = img.Width;
								si.Height = img.Height;
							}
							double cZ = (Zoom > PDFView.RenderLimit && parent.FitType == -1 ? PDFView.RenderLimit : Zoom);
							var x = (int)Math.Round(si.StmpItem.X * cZ / 100);
							var y = (int)Math.Round(si.StmpItem.Y * cZ / 100);
							var w = (int)(si.Width * (cZ / 100) * si.StmpItem.Zoom / 100);
							var h = (int)(si.Height * (cZ / 100) * si.StmpItem.Zoom / 100);
							int d = (int)Math.Round(Math.Sqrt((double)(w * w) + (h * h))) + 3 * indent;
							int dx = x + (w - d) / 2;
							if(dx < 0)
								dx = 0;
							int dy = y + (h - d) / 2;
							if(dx > cachedBitmap.Width - d / 2)
								dx = cachedBitmap.Width - d / 2;
							if(dx < 0)
								dx = 0;
							if(dy > cachedBitmap.Height - d / 2)
								dy = cachedBitmap.Height - d / 2;
							if(dy < 0)
								dy = 0;
							int dw = d;
							if(dx + d > cachedBitmap.Width)
								dw = cachedBitmap.Width - dx;
							int dh = d;
							if(dy + d > cachedBitmap.Height)
								dh = cachedBitmap.Height - dy;
							if(r == Rectangle.Empty)
								 r= new Rectangle(dx, dy, dw, dh);
							else
								if(!r.Contains(  new Rectangle(dx, dy, dw, dh)))
								 r= Rectangle.Union (r, new Rectangle(dx, dy, dw, dh));
				
						}
						if( r!= Rectangle.Empty )
							gr.DrawImage(originalBitmap, r, r, GraphicsUnit.Pixel);
					}
					foreach(Stamp si in parent.StampItems)
					{
						if(si.StmpItem.Page != parent.Page)
							continue;
						Image img = null;

						if(!onlySelection)
						{
							if(si.StmpItem.TypeID == 101)
								img = Environment.GetDSP();
							else
								img = Environment.GetStamp(si.StmpItem.StampID, si.StmpItem.ImageID);

							si.Width = img.Width;
							si.Height = img.Height;
						}
						double cZ = (Zoom > PDFView.RenderLimit && parent.FitType == -1 ? PDFView.RenderLimit : Zoom);
						var x = (int)Math.Round(si.StmpItem.X * cZ / 100);
						var y = (int)Math.Round(si.StmpItem.Y * cZ / 100);
						var w = (int)(si.Width * (cZ / 100) * si.StmpItem.Zoom / 100);
						var h = (int)(si.Height * (cZ / 100) * si.StmpItem.Zoom / 100);

						if(x + w > cachedBitmap.Width)
							x = cachedBitmap.Width - w;

						if(y + h > cachedBitmap.Height)
							y = cachedBitmap.Height - h;
						
						if(!si.StmpItem.Rotate.Equals(0))
						{
							Matrix mx = new Matrix();
							mx.RotateAt(si.StmpItem.Rotate, new PointF(x + w / 2, y + h / 2));
							gr.Transform = mx;
						}

						gr.InterpolationMode = InterpolationMode.High;
						gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
						if(img != null)
							gr.DrawImage(img, x, y, w, h);

						if(si.IsSelected)
							DrawSelectedRectangle(gr, new Rectangle(x, y, w, h));

						if(!si.StmpItem.Rotate.Equals(0))
							gr.ResetTransform();
					}
				}

                if (parent.IsAnnuled)
			    {
                    double cf = 96.0 / 72.0;

			        double cZ = (Zoom > PDFView.RenderLimit && parent.FitType == -1 ? PDFView.RenderLimit : Zoom);
                    int hMin = (int)(((4.0 / 25.4) * gr.DpiY * cZ / 100)*cf); // 4 мм в дюймах 
                    hMin = hMin < 1 ? 1 : hMin;
                    int w = (int)(Math.Sqrt(Math.Pow(cachedBitmap.Height, 2) + Math.Pow(cachedBitmap.Width, 2))); 
                    int h = (int)(cachedBitmap.Height > cachedBitmap.Width ? cachedBitmap.Width * cf / 30 : cachedBitmap.Height * cf / 30); 

                    w = w < 1 ? 1 : w;
                    h = h < hMin ? hMin : h;

                    float sz = 18;
                    Font ft = new Font("Arial", (float)(sz * cZ / 100.0));
                    Size tl = TextRenderer.MeasureText("А", ft);
                    while (tl.Height > h)
                    {
                        if (sz * cZ / 100 <= 1)
                        {
                            ft = new Font("Arial", 2F);
                            break;
                        }

                        ft = new Font("Arial", (float)(--sz * cZ / 100.0));
                        tl = TextRenderer.MeasureText("А", ft);
                    }
                    while (tl.Height < h)
                    {
                        ft = new Font("Arial", (float)(++sz * cZ / 100.0));
                        tl = TextRenderer.MeasureText("А", ft);
                    }

                    int x = (int)System.Math.Round(tl.Width * gr.DpiX / 100);
			        int y = 0;
			        w -= x;

                    StringBuilder txt = new StringBuilder();
                    txt.Append(Environment.StringResources.GetString("AnnulledImage"));
                    while (TextRenderer.MeasureText(txt.ToString(), ft).Width <= w)
                    {
                        txt.Append(Environment.StringResources.GetString("AnnulledImage"));
                    }
                    
                    Matrix mx = new Matrix();
                    mx.RotateAt((float)(Math.Atan((double)cachedBitmap.Height / (double)cachedBitmap.Width) * (180 / Math.PI)), new PointF(x, y));
                    gr.Transform = mx;

                    gr.DrawString(txt.ToString(), ft, new SolidBrush(Color.Red), x, y);
			    }
			}

            if (Image != null)
            {
                Image.Dispose();
                Image = null;
            }
            Image = (Bitmap) cachedBitmap.Clone();

            parent.IsRefreshBitmap = false;
        }

        private bool IsPalleteGrayscale(ColorPalette colorPalette)
        {
            return (colorPalette.Flags & (int) PaletteFlags.GrayScale) == (int) PaletteFlags.GrayScale ||
                   colorPalette.Entries.All(t => t.B == t.R && t.G == t.R && t.B == t.G);
        }

        /// <summary>
        ///   Рисование выделения для заметки
        /// </summary>
        private void DrawSelectedRectangle(Graphics g, Rectangle rect)
        {
            var w = (int) (WidthSelectedtRect*(Zoom > PDFView.RenderLimit ? PDFView.RenderLimit : Zoom)/100.0);
            w = w < 1 ? 1 : w;
            const int indent = WidthSelectedtRect >> 1;
            selectedRectangles = new Rectangle[8]
                                     {
                                         new Rectangle(rect.X - indent, rect.Y - indent, w, w),
                                         new Rectangle(rect.X + (rect.Width >> 1) - indent, rect.Y - indent, w, w),
                                         new Rectangle(rect.X + rect.Width - indent, rect.Y - indent, w, w),
                                         new Rectangle(rect.X + rect.Width - indent, rect.Y + (rect.Height >> 1) - indent, w, w),
                                         new Rectangle(rect.X + rect.Width - indent, rect.Y + rect.Height - indent, w, w),
                                         new Rectangle(rect.X + (rect.Width >> 1) - indent, rect.Y + rect.Height - indent, w, w),
                                         new Rectangle(rect.X - indent, rect.Y + rect.Height - indent, w, w),
                                         new Rectangle(rect.X - indent, rect.Y + (rect.Height >> 1) - indent, w, w)
                                     };
            g.FillRectangle(Brushes.Black, selectedRectangles[0]);
            g.FillRectangle(Brushes.Black, selectedRectangles[1]);
            g.FillRectangle(Brushes.Black, selectedRectangles[2]);
            g.FillRectangle(Brushes.Black, selectedRectangles[3]);
            g.FillRectangle(Brushes.Black, selectedRectangles[4]);
            g.FillRectangle(Brushes.Black, selectedRectangles[5]);
            g.FillRectangle(Brushes.Black, selectedRectangles[6]);
            g.FillRectangle(Brushes.Black, selectedRectangles[7]);
        }

        private PicturePanel parent;

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            parent = Parent as PicturePanel;
        }

        private bool isStampMoved;

        public void RemoveSelected()
        {
            if (Image != null && SelectionModeRectangle != Rectangle.Empty && cachedBitmap != null)
            {
                using (Graphics gr = Graphics.FromImage(Image))
                {
                    var old = new Rectangle(SelectionModeRectangle.X - 1, SelectionModeRectangle.Y - 1,
                                            SelectionModeRectangle.Width + 2, SelectionModeRectangle.Height + 2);
                    gr.DrawImage(cachedBitmap, old, old, GraphicsUnit.Pixel);
                    Invalidate();
                }
            }

            SelectionModeRectangle = Rectangle.Empty;
            oldRect = Rectangle.Empty;
        }

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			((HandledMouseEventArgs)e).Handled = false;
			if(WorkMode == PicturePanel.WorkingMode.EditNotes) // редактирование заметок (свойства, удаление, масштабирование, перемещение)
			{
				var sts = parent.StampItems.Where(x => x.IsSelected && x.StmpItem.TypeID == 100);
				if(sts != null && sts.Count() == 1)
				{
					Stamp st = sts.First();
					if(st != null)
					{
						st.StmpItem.Rotate += e.Delta * 5 / Win32.User32.WHEEL_DELTA + 360;
						st.StmpItem.Rotate %= 360;
						isStampMoved = true;
						DrawStamps(false, true);
						Invalidate(true);
						parent.OnStampsModified();
						((HandledMouseEventArgs)e).Handled = true;
					}
				}
			}
			base.OnMouseWheel(e);
		}

        protected override void OnMouseUp(MouseEventArgs e)
        {
            LastStampSelected = false;

            if (WorkMode == PicturePanel.WorkingMode.Move && e.Button == MouseButtons.Left)
                Cursor = TiffCursors.Hand;
            else if (WorkMode == PicturePanel.WorkingMode.Select && oldRect != Rectangle.Empty)
                SelectionModeRectangle = new Rectangle(oldRect.X, oldRect.Y, oldRect.Width, oldRect.Height);
            else if (WorkMode == PicturePanel.WorkingMode.DrawImage && e.Button == MouseButtons.Left)
            {
                float horizRatio = Image.HorizontalResolution/CurrentStamp.HorizontalResolution;
                parent.CreateAnnotation((int) ((oldRect.X/Zoom)*100), (int) ((oldRect.Y/Zoom)*100),
                                         (int) Math.Round(horizRatio*100));
                parent.ReloadStamps();
                invalRec = Rectangle.Empty;
            }
			else if(WorkMode == PicturePanel.WorkingMode.EditNote && e.Button == MouseButtons.Left)
			{
				if(currentSignID > 0 && isStampMoved)
				{
					Stamp st = parent.StampItems.FirstOrDefault(x => x.StmpItem.ID == currentSignID);
					if(st != null && st.StmpItem.X != (int)((oldRect.X / Zoom) * 100) && st.StmpItem.Y != (int)((oldRect.Y / Zoom) * 100))
					{
						st.StmpItem.X = (int)((oldRect.X / Zoom) * 100);
						st.StmpItem.Y = (int)((oldRect.Y / Zoom) * 100);
						parent.IsRefreshBitmap = true;
						Invalidate(true);
						parent.OnStampsModified();
					}
				}
				else
				{
					Invalidate(true);
				}

				parent.WorkMode = PicturePanel.WorkingMode.EditNotes;

				LastStampSelected = false;

				if(Cursor != Cursors.Default)
					Cursor = Cursors.Default;
			}
			else if(WorkMode == PicturePanel.WorkingMode.EditNotes)
			{
				if(e.Button == MouseButtons.Left)
				{
					if(parent.StampItems != null && parent.StampItems.Count > 0)
					{
						double cZ = (Zoom > PDFView.RenderLimit ? PDFView.RenderLimit : Zoom);
						var v = parent.StampItems.Where(x => x.StmpItem.Page == parent.Page &&
															  Rectangle.Intersect(oldRect,
																				  new Rectangle(
																					  (int)(x.StmpItem.X * cZ / 100),
																					  (int)(x.StmpItem.Y * cZ / 100),
																					  (int)
																					  (x.Width * (cZ / 100) *
																					   (x.StmpItem.Zoom / 100.0)),
																					  (int)
																					  (x.Height * (cZ / 100) *
																					   (x.StmpItem.Zoom / 100.0)))) !=
															  Rectangle.Empty).
							Select(x => x.IsSelected = true).ToArray();

						if(v.Length > 0)
							DrawStamps(true);
						else
							base.OnMouseUp(e);

						if(oldRect != Rectangle.Empty)
						{
							SelectionModeRectangle = new Rectangle(oldRect.X, oldRect.Y, oldRect.Width, oldRect.Height);
							RemoveSelected();
						}

						return;
					}

					base.OnMouseUp(e);
				}
				else if(e.Button == MouseButtons.Right && parent.StampItems != null && parent.StampItems.Count > 0)
				{
					OnRightClick(e);
				}
			}

            base.OnMouseUp(e);
        }

        private bool LastStampSelected;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            LastStampSelected = false;

            const int cZ = 1;
            var mPoint = new Point(e.X/cZ, e.Y/cZ);

            lastPositionForDrag = mPoint;
            if (WorkMode == PicturePanel.WorkingMode.Select && e.Button == MouseButtons.Left) // создание выделения
            {
                Cursor = Cursors.Default;
                RemoveSelected();
            }
            else if (WorkMode == PicturePanel.WorkingMode.EditNotes && e.Button == MouseButtons.Left)
            {
                isStampMoved = false;

                if (oldRect != Rectangle.Empty)
                    SelectionModeRectangle = new Rectangle(oldRect.X, oldRect.Y, oldRect.Width, oldRect.Height);

                RemoveSelected();

				if(parent.StampItems != null && parent.StampItems.Count > 0)
				{
					if(!controlPushed && !shiftPushed)
						parent.ClearStampsSelection();

					Stamp st = parent.StampItems.LastOrDefault(x => x.StmpItem.Page == parent.Page &&
																	 x.StmpItem.X * Zoom / 100 <= e.X &&
																	 x.StmpItem.Y * Zoom / 100 <= e.Y &&
																	 x.StmpItem.X * Zoom / 100 + x.Width * (Zoom / 100) * (x.StmpItem.Zoom / 100.0) >= e.X &&
																	 x.StmpItem.Y * Zoom / 100 + x.Height * (Zoom / 100) * (x.StmpItem.Zoom / 100.0) >= e.Y);
					if(st != null)
					{
						parent.IsRefreshBitmap = true;
						if(!controlPushed && !shiftPushed)
							st.IsSelected = true;
						else
						{
							if(shiftPushed)
							{
								parent.IsRefreshBitmap = !st.IsSelected;
								st.IsSelected = true;
							}
							else if(controlPushed)
								st.IsSelected = !st.IsSelected;
						}

						if(st.StmpItem.TypeID > 99 && st.StmpItem.Page == parent.Page)
						{
							if(st.StmpItem.TypeID == 101)
								CurrentStamp = Environment.GetDSP();
							else
								CurrentStamp = Environment.GetStamp(st.StmpItem.StampID, st.StmpItem.ImageID);

							CurrentStampID = st.StmpItem.StampID;
							currentSignID = st.StmpItem.ID;
							parent.CurrentDocID = st.StmpItem.DocumentID;
							LastStampSelected = true;

							Cursor = Cursors.NoMove2D;
							WorkMode = PicturePanel.WorkingMode.EditNote;
							//parent.AutoScroll = false;
						}
						else
						{
							CurrentStampID = 0;
							currentSignID = 0;
						}
					}
					else if(!controlPushed && !shiftPushed)
						parent.ClearStampsSelection();

					Invalidate();
				}
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (parent == null)
                return;

            Point mPointOffset = new Point(e.X, e.Y);

            if (WorkMode == PicturePanel.WorkingMode.Move && e.Button == MouseButtons.Left)
                // после нажатия для перемещения картики
            {
                #region Перетаскивание картинки

                #endregion
            }
            else if ((WorkMode == PicturePanel.WorkingMode.Select || WorkMode == PicturePanel.WorkingMode.EditNotes) &&
                     e.Button == MouseButtons.Left)
            {
                #region Создание области выделения

                double cZ = (Zoom > PDFView.RenderLimit && parent.FitType == -1 ? (Zoom / 100.0) : 1);

                int x = (lastPositionForDrag.X);
                int y = (lastPositionForDrag.Y);

                int left = 0, top = 0, rigth = 0, bottom = 0;

                if (x > mPointOffset.X)
                {
                    left = mPointOffset.X <= 0 ? 1 : mPointOffset.X;
                    rigth = mPointOffset.X >= ClientRectangle.Width ? ClientRectangle.Width - 1 : x;
                }
                else
                {
                    left = x;
                    rigth = mPointOffset.X >= ClientRectangle.Width ? ClientRectangle.Width - 1 : mPointOffset.X;
                }

                if (y > mPointOffset.Y)
                {
                    top = mPointOffset.Y <= 0 ? 1 : mPointOffset.Y;
                    bottom = mPointOffset.Y >= ClientRectangle.Height ? ClientRectangle.Height - 1 : y;
                }
                else
                {
                    top = y;
                    bottom = mPointOffset.Y >= ClientRectangle.Height ? ClientRectangle.Height - 1 : mPointOffset.Y;
                }

                using (Graphics gr = Graphics.FromImage(Image))
                {
                    if (!oldRect.Equals(Rectangle.Empty))
                    {
                        var old = new Rectangle(oldRect.X - 1, oldRect.Y - 1, oldRect.Width + 1,
                                                oldRect.Height + 1);

                        gr.DrawImage(cachedBitmap, old, old, GraphicsUnit.Pixel);
                        Invalidate(old);
                    }

                    var m_left = (int) (left/cZ);
                    m_left = (m_left == 0 ? 1 : m_left);
                    var m_top = (int) (top/cZ);
                    m_top = (m_top == 0 ? 1 : m_top);

                    oldRect = new Rectangle(m_left, m_top, (int) ((rigth - left)/cZ), (int) ((bottom - top)/cZ));

                    gr.InterpolationMode = InterpolationMode.High;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var dotedPen = new Pen(new SolidBrush(Color.Black), 1))
                    {
                        dotedPen.DashStyle = DashStyle.Dot;
                        dotedPen.LineJoin = LineJoin.Bevel;

                        gr.DrawRectangle(dotedPen, oldRect);
                    }
					Invalidate();
                }

                #endregion
            }
            else if (WorkMode == PicturePanel.WorkingMode.DrawImage && e.Button == MouseButtons.None ||
                     WorkMode == PicturePanel.WorkingMode.EditNote && e.Button == MouseButtons.Left &&
                     Cursor == Cursors.NoMove2D)
            {
                #region Постановка штампа

                if (CurrentStamp != null)
                {
                    ShowMouseMove(CurrentStamp, mPointOffset);

                    if (WorkMode == PicturePanel.WorkingMode.EditNote)
                        isStampMoved = true;

                    return;
                }
                else if (Cursor != Cursors.Default)
                    Cursor = Cursors.Default;

                #endregion
            }
        }

        private Rectangle pr;
        private bool showStamp;
        private Bitmap invalBitmap;

        private void ShowMouseMove(Image bmp, Point mPointOffset)
        {
            float width;
            float height;
			Stamp st = null;
			double cZ = Zoom < PDFView.RenderLimit ? (Zoom / 100.0) : PDFView.RenderLimit / 100;
            if (st != null && LastStampSelected)
            {
				width = (float)(st.Width * (st.StmpItem.Zoom / 100.0) * cZ);
				height = (float)(st.Height * (st.StmpItem.Zoom / 100.0) * cZ);
            }
            else
            {
				width = (float)(bmp.Width * (Image.HorizontalResolution / bmp.HorizontalResolution) * cZ);
				height = (float)(bmp.Height * (Image.VerticalResolution / bmp.VerticalResolution) * cZ);
            }

            int visW = (Width <= parent.ClientRectangle.Width ? Width : parent.ClientRectangle.Width);
            int visH = (Height <= parent.ClientRectangle.Height ? Height : parent.ClientRectangle.Height);

            float drowX, drowY;

            if ((mPointOffset.X + parent.DisplayRectangle.X + width/2) <= visW &&
                (mPointOffset.X + parent.DisplayRectangle.X - width/2) >= 0)
                drowX = (mPointOffset.X - width/2);
            else
            {
                if ((mPointOffset.X + parent.DisplayRectangle.X + width) > visW)
                    drowX = (mPointOffset.X - (mPointOffset.X + width - (visW - parent.DisplayRectangle.X)));
                else
                    drowX = 1 - parent.DisplayRectangle.X;
            }
            if (drowX < 0) drowX = 1;

            if ((mPointOffset.Y + parent.DisplayRectangle.Y + height/2) <= visH &&
                (mPointOffset.Y + parent.DisplayRectangle.Y - height/2) >= 0)
                drowY = (mPointOffset.Y - height/2);
            else
            {
                if ((mPointOffset.Y + parent.DisplayRectangle.Y + height/2) > visH)
                    drowY = (mPointOffset.Y - (mPointOffset.Y + height - (visH - parent.DisplayRectangle.Y)));
                else
                    drowY = 1 - parent.DisplayRectangle.Y;
            }
            if (drowY < 0) drowY = 1;

            oldRect = new Rectangle((int) drowX, (int) drowY, (int) width, (int) height);

            pr = Rectangle.Union(invalRec, oldRect);

            showStamp = true;
            Invalidate();
        }

        private Rectangle invalRec = Rectangle.Empty;
		private bool startModified = true;

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (WorkMode == PicturePanel.WorkingMode.DrawImage && invalRec != Rectangle.Empty)
            {
                using (Graphics g = Graphics.FromImage(Image))
                {
                    g.InterpolationMode = InterpolationMode.High;
                    g.DrawImage(cachedBitmap, invalRec, invalRec, GraphicsUnit.Pixel);
                    Invalidate(invalRec, true);
                }

                invalRec = Rectangle.Empty;
            }
        }

        private void OnRightClick(MouseEventArgs e)
        {
            Stamp st = parent.StampItems.FirstOrDefault(x => x.IsSelected && x.StmpItem.Page == parent.Page &&
                                                              x.StmpItem.X*Zoom/100 <= e.X &&
                                                              x.StmpItem.Y*Zoom/100 <= e.Y &&
                                                              x.StmpItem.X*Zoom/100 +
                                                              x.Width*(Zoom/100)*(x.StmpItem.Zoom/100.0) >= e.X &&
                                                              x.StmpItem.Y*Zoom/100 +
                                                              x.Height*(Zoom/100)*(x.StmpItem.Zoom/100.0) >= e.Y);

            if (st != null) // попали хотя бы в один элемент
            {
                new ContextMenu(new[]
                                    {
                                        new MenuItem(Environment.StringResources.GetString("DeleteStamp"),
                                                     parent.mi_Click, Shortcut.ShiftDel)
                                    }).Show(this, e.Location);
            }
            else // не попали - снимаем выделение
            {
                parent.StampItems.Select(x => x.IsSelected = false).ToArray();
                parent.IsRefreshBitmap = true;
                Invalidate();
            }
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
				if(m != null)
					switch(m.Msg)
					{
						case (int)Win32.Msgs.WM_LBUTTONDOWN:
							controlPushed = (m.WParam.ToInt32() & (Int32)Win32.MK.MK_CONTROL) == 8;
							shiftPushed = (m.WParam.ToInt32() & (Int32)Win32.MK.MK_SHIFT) == 4;
							Select();

							break;

						case (int)Win32.Msgs.WM_SETFOCUS:
							return;
					}
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex, m.ToString());
            }
            base.WndProc(ref m);
        }
    }
}