using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Kesco.Lib.Win.Document.Controls
{
	public partial class ImageShowControl : ImageControl.ImageControl
	{
		private int imageID;
		private int stampID;
		private int stampDocID = 0;
		/// <summary>
		/// Лист со штампами на изображении
		/// </summary>
		private List<Data.Temp.Objects.StampItem> stampItems;
		public bool DSP { get; private set; }

		public override bool IsAnnuled { get; set; }
		/// <summary>
		/// Текущий сервер
		/// </summary>
		private ServerInfo curServer;
		/// <summary>
		/// нажата кнопка "Control"
		/// </summary>
		private bool controlPushed = false;
		/// <summary>
		/// нажата кнопка "Shift"
		/// </summary>
		private bool shiftPushed = false;

		/// <summary>
		/// начальный x мыши при драге картинки
		/// </summary>
		private int startX;
		/// <summary>
		/// начальный y мыши при драге картинки
		/// </summary>
		private int startY;

		private bool forceReplicate = false;

		private bool compliteLoading = false;

		private Timer mouseTimer;

        /// <summary>
        /// Словарь углов поворотов страниц изображения
        /// </summary>
        private readonly Classes.ImageVirtualRotationDictionary  _pageVirtualRotationDictionary = new Classes.ImageVirtualRotationDictionary();

		public event EventHandler ImageChanged;
		public event EventHandler LoadComplete;

		private void OnLoadComplete()
		{
			if(LoadComplete != null)
				LoadComplete(this, EventArgs.Empty);
		}

		private void OnImageChanged()
		{
			if(ImageChanged != null)
				ImageChanged(this, EventArgs.Empty);
		}

		public ImageShowControl()
		{
			InitializeComponent();
			DoubleBuffered = true;
			renderAnnotations = new Classes.RenderAnnotationsAndExternal(WidthSelectedtRect);
		}

		/// <summary>
		/// Кода изображения
		/// </summary>
		public int ImageID
		{
			get { return imageID; }
			set
			{
				if(value < 1)
				{
					SetImageValue(0);
					FileName = null;
				}
				else
				{
					if(imageID != value)
						Clear();
					SetImageValue(value);
					ReloadImageData(true, 0);
				}
			}
		}

		/// <summary>
		/// Кода текущего штампа
		/// </summary>
		public int CurrentStampID
		{
			get { return stampID; }
			set { stampID = value; }
		}

		/// <summary>
		/// Принудительная репликация
		/// </summary>
		public bool ForceReplicate
		{
			get { return forceReplicate; }
			set { forceReplicate = value; }
		}

		/// <summary>
		/// Текущий сервер
		/// </summary>
		public ServerInfo CurrentServer
		{
			get { return curServer; }
		}

		/// <summary>
		/// Оканчание загрузки изображения
		/// </summary>
		public bool CompliteLoading
		{
			get { return compliteLoading; }
		}

        #region Rotate

        /// <summary>
        /// Угол виртуального поворота
        /// </summary>
        public override void SetVirtualRotation()
		{
			_pageVirtualRotationDictionary.SaveVirtualRotation(imageID, Page);
			SetModifiedForNewDocument(false);
		}

        /// <summary>
        /// Одна или более страниц виртуально повернута.
        /// Для подписки на изменение изображения
        /// </summary>
        public override bool HaveVirtualRotation
        {
			get { return _pageVirtualRotationDictionary != null && _pageVirtualRotationDictionary.IsAnyPageRotated; }
        }

        /// <summary>
        /// Одна или более страниц виртуально БЫЛА повернута.
        /// Для подписки на изменение изображения
        /// </summary>
        public override bool RotationChanged
        {
			get { return _pageVirtualRotationDictionary != null && _pageVirtualRotationDictionary.RotationChanged; }
        } 

        #endregion

        /// <summary>
        /// Загрузить изображение документа
        /// </summary>
        /// <param name="imageID"></param>
        /// <param name="page"></param>
        /// <param name="force">Перестроить страницу изображения</param>
		public void LoadImagePage(int imageID, int page, bool force = false)
		{
			if(imageID < 1)
			{
				SetImageValue(0);
				FileName = null;
			}
			else
			{
				if(this.imageID != imageID || force)
					Clear();
				SetImageValue(imageID);
				if(page > 0)
					ReloadImageData(this.imageID != imageID, page);
				else
					ReloadImageData(true, 0);
			}
		}

		private void SetImageValue(int value)
		{
			imageID = value;
			if(stampItems != null)
			{
				stampItems.Clear();
				stampItems = null;
			}
            DSP = false;
            IsAnnuled = false;
			buttonLoad.Visible = false;
			labelNoImage.Visible = false;
			if(renderAnnotations != null)
				renderAnnotations.Clear();
			SaveStampsInternal = imageID == 0;
			OnImageChanged();
		}

		public override string FileName
		{
			get
			{
				return base.FileName;
			}
			set
			{
				if(imageID > 0)
				{
					SetImageValue(0);
				}
				else
				{
					buttonLoad.Visible = false;
					labelNoImage.Visible = false;
				}
				base.FileName = value;
			}
		}

		/// <summary>
		/// Перезагрузка данных изображения
		/// <para>
		/// <note type="caution"> Нумерация <paramref name="page"/> начинается с 0</note>
		/// </para>
		/// </summary>
		/// <param name="change">сброс информации о выбранной странице</param>
		/// <param name="page">номер страницы</param>
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
					curServer = Environment.GetLocalServer(serverIDs[rand.Next(serverIDs.Count)]);
					string fileName = curServer.Path + "\\" + Environment.GetFileNameFromID(imageID) + ".tif";
					if(File.Exists(fileName))
					{
						if(change)
							base.FileName = fileName;
						else
							base.LoadFile(fileName, page);
						OnFileNameChanged();
					}
					else
					{
						bool err = true;
						if(serverIDs.Count > 1)
						{
							for(int k = 0; k < serverIDs.Count; ++k)
							{
								curServer = Environment.GetLocalServer(serverIDs[k]);
								fileName = curServer.Path + "\\" + Environment.GetFileNameFromID(imageID) + ".tif";
								if(File.Exists(fileName))
								{
									err = false;
									if(change)
										base.FileName = fileName;
									else
										base.LoadFile(fileName, page);
									OnFileNameChanged();
									break;
								}
							}
						}
						if(err)
							NeedReplicate();
					}

					if(stampItems != null)
					{
						stampItems.Clear();
						stampItems = null;
					}

					renderAnnotations.Clear();

                    // Загрузка из бд информации о штампах
					stampItems = Environment.DocSignatureData.GetStamps(imageID);

					DSP = stampItems.Any(x => x.TypeID == 101);

				    if(page >= 0)
					{
						((Classes.RenderAnnotationsAndExternal)renderAnnotations).PageStamps = new SynchronizedCollection<Data.Temp.Objects.StampItem>(new object(), stampItems.FindAll(x => x.Page == page + 1).ToList());
					}

                    IsAnnuled = ((Classes.RenderAnnotationsAndExternal)renderAnnotations).IsAnnuled = Environment.DocSignatureData.IsDocSignedAnnuled(imageID);

					IsRefreshBitmap = true;
					Invalidate(rectAnimatedImage);
				}
				else
				{
					NeedReplicate();
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				Error.ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
				ClearImage();
			}
		}

        /// <summary>
        /// Синхронизация виртуальных поворотов
        /// </summary>
	    public void SyncImagePageRotations()
        {
            for (int i = 0; i < PageCount; i++)
            {
                var transition = _pageVirtualRotationDictionary.SyncPageRotation(imageID, i);

                if (transition.Changed)
                    RefreshPage(i);
            }
        }

	    public virtual void SelectTool(short tool, int docID)
		{
			SelectTool(tool);
			stampDocID = docID;
		}

		private void ClearImage()
		{
			buttonLoad.Visible = false;
			labelNoImage.Visible = false;
			curServer = null;
			base.FileName = "";
		}

		private void NeedReplicate()
		{
			base.FileName = "";
			if(forceReplicate)
				buttonLoad_Click(this, EventArgs.Empty);
			else
			{
				SetSizeThumbnailPanel(false);
				buttonLoad.Visible = true;
				labelNoImage.Visible = true;
				Invalidate();
			}
			if(!compliteLoading)
				OnLoadComplete();
		}

		private void buttonLoad_Click(object sender, EventArgs e)
		{
			buttonLoad.Visible = false;
			labelNoImage.Visible = false;
			Invalidate();
			List<int> serverIDs = Environment.DocImageData.GetLocalDocImageServers(imageID, Environment.GetLocalServersString());
			if(serverIDs != null && serverIDs.Count > 0)
			{
				ReloadImageData(true, 0);
			}
			else
			{
				if(!Environment.DocImageData.DocImageCopy(Environment.GetRandomLocalServer().ID, imageID))
					ReloadImageData(true, 0);
			}
		}

		protected override void CreateAnnotation(Point mouseUpPt)
		{
			if(imageID > 0 && UserAction == UsersActionsTypes.DrawImage && AnnotationState == AnnotationsState.Create)
			{
				if(Modified)
					OnNeedSave(ChangePage);
				// коэффициенты для масштабирования штампа под разрешение основной картинки
				float horizRatio = animatedImage.HorizontalResolution / CurrentStamp.HorizontalResolution,
					vertRatio = animatedImage.VerticalResolution / CurrentStamp.VerticalResolution;
				// помещаем изображение штампа таким образом, чтобы центр был в месте отпускания мыши
				if(IsMouseOnRect(new Rectangle(new Point(0, 0), animatedImage.Size), Point.Add(mouseUpPt, new Size(-rectAnimatedImage.X, -rectAnimatedImage.Y))))
				{
					if(stampID == -101)
					{
						if(Environment.DocSignatureData.AddStampDSP(stampDocID, imageID, Environment.CurEmp.ID, Page, oldRect.X, oldRect.Y, (int)(horizRatio * 100), 0))
							ReloadImageData(false, Page - 1);
					}
					else
						if(Environment.DocSignatureData.AddStamp(stampDocID, imageID, Environment.CurEmp.ID, stampID, Page, oldRect.X, oldRect.Y, (int)(horizRatio * 100), 0))
							ReloadImageData(false, Page - 1);
					UserAction = UsersActionsTypes.None;
                    Console.WriteLine("{0}: CreateAnnotation.SelectTool", DateTime.Now.ToString("HH:mm:ss fff"));
					CurrentStamp = null;
					SelectTool(0);
				}
			}
			else
				base.CreateAnnotation(mouseUpPt);
		}

		public override bool HasAnnotation()
		{
			return base.HasAnnotation() || DSP || (((Classes.RenderAnnotationsAndExternal)renderAnnotations).PageStamps != null && ((Classes.RenderAnnotationsAndExternal)renderAnnotations).PageStamps.Count > 0);
		}

		public string SaveWithBurnAndResolution(int startPage, int endPage, string tempFileName, int horizontalResolution, int verticalResolution, bool leaveAnnotation, bool saveColor, bool burnAnnotation)
		{
			IntPtr tifw = IntPtr.Zero;
			try
			{
				if(string.IsNullOrEmpty(tempFileName))
					tempFileName = Path.GetTempFileName();
				Slave.DeleteFile(tempFileName);
				if(ControlTypeWork.ReadWithCloseTiffHandle == (TypeWork & ControlTypeWork.ReadWithCloseTiffHandle))
				{
					tifw = libTiff.ExtLT.TIFOpenW(tempFileName, "w");
					for(int n = 0; n < endPage - startPage + 1; n++)
					{
						Tiff.PageInfo info = libTiff.GetImageFromTiff(fileName, n + startPage);
						if(info != null)
						{
							System.Drawing.Imaging.PixelFormat pf;
							Bitmap bmp = info.Image;
							pf = bmp.PixelFormat;
							if(bmp.HorizontalResolution > horizontalResolution)
								if(bmp.VerticalResolution > verticalResolution)
									bmp.SetResolution(horizontalResolution, verticalResolution);
								else
									bmp.SetResolution(horizontalResolution, bmp.VerticalResolution);

							if(bmp.VerticalResolution > verticalResolution)
								bmp.SetResolution(bmp.HorizontalResolution, verticalResolution);
							if(burnAnnotation || stampItems != null && stampItems.Count > 0)
							{

								IEnumerable<Data.Temp.Objects.StampItem> pageStamps = null;
								if(stampItems != null)
									pageStamps = stampItems.Where(x => x.Page == n + startPage + 1);

								tiffAnnotation = null;


								if(pageStamps != null && pageStamps.Any() || TiffAnnotation.MarkAttributes != null)
								{
									if(bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb || bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
									{
										Bitmap tm = new Bitmap(bmp);
										tm.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
										bmp = tm;
									}
									float drez = bmp.VerticalResolution / bmp.HorizontalResolution;
									System.Drawing.Drawing2D.Matrix mx = null;
									using(Graphics g = Graphics.FromImage(bmp))
									{
										if(burnAnnotation && info.Annotation != null)
										{
											tiffAnnotation = new ImageControl.TiffAnnotation(this);
											tiffAnnotation.Parse(info.Annotation);
											ArrayList figuresList = tiffAnnotation.GetFigures(false);
											foreach(object figure in figuresList)
											{
												ImageControl.TiffAnnotation.IBufferBitmap bb = figure as ImageControl.TiffAnnotation.IBufferBitmap;
												if(bb != null)
												{
													switch(figure.GetType().Name)
													{
														case "ImageEmbedded":
															ImageControl.TiffAnnotation.ImageEmbedded img = (ImageControl.TiffAnnotation.ImageEmbedded)figure;
															g.DrawImage(img.Img, img.LrBounds.Location.X, img.LrBounds.Location.Y, img.LrBounds.Size.Width, img.LrBounds.Size.Height);
															break;
														case "StraightLine":
															ImageControl.TiffAnnotation.StraightLine line = (ImageControl.TiffAnnotation.StraightLine)figure;
															if(line.LinePoints == null)
																continue;
															g.DrawLine(new Pen(new SolidBrush(line.RgbColor1), Convert.ToSingle(line.ULineSize)), line.LinePoints[0], line.LinePoints[1]);
															break;
														case "FreehandLine":
															ImageControl.TiffAnnotation.FreehandLine fline = (ImageControl.TiffAnnotation.FreehandLine)figure;
															if(fline.LinePoints == null)
																continue;
															for(int i = 0; i < fline.LinePoints.Length; i += 2)
															{
																if(i != 0)
																	g.DrawLine(new Pen(new SolidBrush(fline.RgbColor1), Convert.ToSingle(fline.ULineSize)), fline.LinePoints[i - 1], fline.LinePoints[i]);
																g.DrawLine(new Pen(new SolidBrush(fline.RgbColor1), Convert.ToSingle(fline.ULineSize)), fline.LinePoints[i], fline.LinePoints[i + 1]);
															}
															break;
														case "HollowRectangle":
															{
																ImageControl.TiffAnnotation.HollowRectangle rect = (ImageControl.TiffAnnotation.HollowRectangle)figure;
																Bitmap bitmapUp = bb.GetBitmap(bmp, CurrentInterpolationMode);
																g.DrawImage(bitmapUp, rect.LrBounds.X, rect.LrBounds.Y);
															}
															break;
														case "FilledRectangle":
															{
																ImageControl.TiffAnnotation.FilledRectangle frect = (ImageControl.TiffAnnotation.FilledRectangle)figure;
																Bitmap bitmapUp = bb.GetBitmap(bmp, CurrentInterpolationMode);
																g.DrawImage(bitmapUp, frect.LrBounds.X, frect.LrBounds.Y);
															}
															break;
														case "TypedText":
															ImageControl.TiffAnnotation.TypedText tt = (ImageControl.TiffAnnotation.TypedText)figure;
															StringFormat sf = new StringFormat();
															mx = null;
															Rectangle newRect = tt.LrBounds;
															switch(tt.TextPrivateData.NCurrentOrientation)
															{
																case 900:
																	mx = new System.Drawing.Drawing2D.Matrix();
																	newRect = new Rectangle(tt.LrBounds.X, tt.LrBounds.Y + tt.LrBounds.Height, tt.LrBounds.Height, tt.LrBounds.Width);
																	mx.RotateAt(270, new PointF(newRect.X, newRect.Y));
																	g.Transform = mx;
																	break;
																case 1800:
																	mx = new System.Drawing.Drawing2D.Matrix();
																	newRect = tt.LrBounds;
																	mx.RotateAt(180, new PointF(tt.LrBounds.Location.X + tt.LrBounds.Width / 2, tt.LrBounds.Location.Y + tt.LrBounds.Height / 2));
																	g.Transform = mx;
																	break;
																case 2700:
																	mx = new System.Drawing.Drawing2D.Matrix();
																	newRect = new Rectangle(tt.LrBounds.X + tt.LrBounds.Width, tt.LrBounds.Y, tt.LrBounds.Height, tt.LrBounds.Width);
																	mx.RotateAt(90, new PointF(newRect.X, newRect.Y));
																	g.Transform = mx;

																	break;

															}

															g.TextRenderingHint = tt.FontRenderingHint;
															sf.Trimming = StringTrimming.Word;
															using(Font f = new Font(tt.LfFont.FontFamily, tt.LfFont.SizeInPoints, tt.LfFont.Style))
																g.DrawString(tt.TextPrivateData.SzAnoText, f, new SolidBrush(tt.RgbColor1), newRect, sf);

															g.ResetTransform();
															g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
															break;
														case "TextStump":
															ImageControl.TiffAnnotation.TextStump ts = (ImageControl.TiffAnnotation.TextStump)figure;
															StringFormat sf3 = new StringFormat();
															g.TextRenderingHint = ts.FontRenderingHint;

															g.DrawString(ts.TextPrivateData.SzAnoText, ts.LfFont, new SolidBrush(ts.RgbColor1), ts.LrBounds, sf3);
															g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
															break;
														case "TextFromFile":
															ImageControl.TiffAnnotation.TextFromFile tf = (ImageControl.TiffAnnotation.TextFromFile)figure;
															StringFormat sf2 = new StringFormat();
															g.TextRenderingHint = tf.FontRenderingHint;

															g.DrawString(tf.TextPrivateData.SzAnoText, tf.LfFont, new SolidBrush(tf.RgbColor1), tf.LrBounds, sf2);
															g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
															break;
														case "AttachANote":
															ImageControl.TiffAnnotation.AttachANote an = (ImageControl.TiffAnnotation.AttachANote)figure;
															StringFormat sf1 = new StringFormat();

															g.TextRenderingHint = an.FontRenderingHint;

															g.FillRectangle(Brushes.Black, an.LrBounds.X + 2, an.LrBounds.Y + 2, an.LrBounds.Width, an.LrBounds.Height);
															g.FillRectangle(new SolidBrush(an.RgbColor1), an.LrBounds);
															g.DrawRectangle(Pens.Black, an.LrBounds.X, an.LrBounds.Y, an.LrBounds.Width, an.LrBounds.Height);

															System.Drawing.Drawing2D.Matrix mx1 = null;
															Rectangle newRect1 = an.LrBounds;
															switch(an.TextPrivateData.NCurrentOrientation)
															{
																case 900:
																	mx1 = new System.Drawing.Drawing2D.Matrix();
																	newRect1 = new Rectangle(an.LrBounds.X, an.LrBounds.Y + an.LrBounds.Height, an.LrBounds.Height, an.LrBounds.Width);
																	mx1.RotateAt(270, new PointF(newRect1.X, newRect1.Y));
																	g.Transform = mx1;
																	break;
																case 1800:
																	mx1 = new System.Drawing.Drawing2D.Matrix();
																	newRect1 = an.LrBounds;
																	mx1.RotateAt(180, new PointF(an.LrBounds.Location.X + an.LrBounds.Width / 2, an.LrBounds.Location.Y + an.LrBounds.Height / 2));
																	g.Transform = mx1;
																	break;
																case 2700:
																	mx1 = new System.Drawing.Drawing2D.Matrix();
																	newRect1 = new Rectangle(an.LrBounds.X + an.LrBounds.Width, an.LrBounds.Y, an.LrBounds.Height, an.LrBounds.Width);
																	mx1.RotateAt(90, new PointF(newRect1.X, newRect1.Y));
																	g.Transform = mx1;
																	break;
															}
															g.DrawString(an.TextPrivateData.SzAnoText, new Font(an.LfFont.FontFamily, an.LfFont.SizeInPoints, an.LfFont.Style), new SolidBrush(an.RgbColor2), newRect1, sf1);
															g.ResetTransform();
															g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
															break;
													}
												}
											}
											tiffAnnotation.Dispose();
										}
										Image stamp;
										if(pageStamps != null)
											foreach(Data.Temp.Objects.StampItem si in pageStamps)
											{
												if(si.TypeID == 101)
													stamp = Environment.GetDSP();
												else
													stamp = Environment.GetStamp(si.StampID, imageID);
												if(!si.Rotate.Equals(0))
												{
													mx = new System.Drawing.Drawing2D.Matrix();
													mx.RotateAt(si.Rotate, new PointF(si.X + (stamp.Width * si.Zoom) / 200, si.Y + (stamp.Height * si.Zoom * drez) / 200));
													g.Transform = mx;
												}
												g.DrawImage(stamp, si.X, si.Y, (stamp.Width * si.Zoom) / 100, (stamp.Height * si.Zoom * drez) / 100);
												if(!si.Rotate.Equals(0))
													g.ResetTransform();
											}
									}
								}
							}
							if(!saveColor && bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
								libTiff.SetImageToTiff(tifw, libTiff.ConvertToBitonal(bmp), leaveAnnotation && !burnAnnotation ? info.Annotation : null);
							else
								if(bmp.PixelFormat != pf)
									libTiff.SetImageToTiff(tifw, libTiff.ConvertTo(pf, bmp), leaveAnnotation && !burnAnnotation ? info.Annotation : null);
								else
									libTiff.SetImageToTiff(tifw, bmp, leaveAnnotation && !burnAnnotation ? info.Annotation : null);
							libTiff.SavePage(tifw);
						}
					}
					libTiff.TiffCloseWrite(ref tifw);
				}
			}
			catch(Exception ex)
			{
				OnErrorMessage(new Exception("SavePart error", ex));
				if(!IntPtr.Zero.Equals(tifw))
					libTiff.TiffCloseWrite(ref tifw);
				Slave.DeleteFile(tempFileName);
				return null;

			}
			return tempFileName;
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if(TypeWorkAnimatedImage == TypeWorkImage.EditNotes) // редактирование заметок (свойства, удаление, масштабирование, перемещение)
			{
				Classes.RenderAnnotationsAndExternal ra = renderAnnotations as Classes.RenderAnnotationsAndExternal;
				if(ra != null && ra.PageSelecedStamps != null && ra.PageSelecedStamps.Count == 1 && ra.PageSelecedStamps[0].TypeID != 101)
				{
					int indent = WidthSelectedtRect >> 1;
					Data.Temp.Objects.StampItem item = ra.PageSelecedStamps[0];
					item.Rotate += e.Delta * 5 / Win32.User32.WHEEL_DELTA + 360;
					item.Rotate	%= 360;
					Image stamp;
					if(item.TypeID == 101)
						stamp = Environment.GetDSP();
					else
						stamp = Environment.GetStamp(item.StampID, imageID);
					var x = item.X;
					var y = item.Y;
					var w = (int)(stamp.Width * item.Zoom / 100f);
					var h = (int)(stamp.Height * item.Zoom / 100f);

					if(x + w > cachedBitmap.Width)
						x = cachedBitmap.Width - w;
					if(x < 0)
						x = 0;

					if(y + h > cachedBitmap.Height)
						y = cachedBitmap.Height - h;
					if(y < 0)
						y = 0;
					int d = (int)Math.Round(Math.Sqrt((double)(w * w) + (h * h))) + 3 * indent;
					int dx = x + (w - d) / 2;
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
					if(dh + d > cachedBitmap.Height)
						dh = cachedBitmap.Height - dy;

					using(Bitmap bit = new Bitmap(dw, dh))
					{
						bit.SetResolution(animatedImage.VerticalResolution, animatedImage.HorizontalResolution);
						using(Graphics gr = Graphics.FromImage(bit))
						{
							gr.InterpolationMode = CurrentInterpolationMode;
							gr.DrawImage(animatedImage, new Rectangle(0, 0, dw, dh), new Rectangle(dx, dy, dw, dh), GraphicsUnit.Pixel);

							if(item.Rotate != 0)
							{
								System.Drawing.Drawing2D.Matrix mx = new System.Drawing.Drawing2D.Matrix();
								mx.RotateAt(item.Rotate, new PointF((int)Math.Round(x - dx +( stamp.Width * item.Zoom) / 200.0), (int)Math.Round(y - dy +( stamp.Height * item.Zoom) / 200.0)));
								gr.Transform = mx;
							}
							gr.DrawImage(stamp, new Rectangle(x - dx, y - dy, w, h));
							Rectangle[] re = null;
							renderAnnotations.DrawSelectedRectangle(gr, new Rectangle(x - dx, y - dy, w, h), ref re);
						}
						using(Graphics g = Graphics.FromHwnd(Handle))
						{
							g.InterpolationMode = CurrentInterpolationMode;
							int drowX = (int)Math.Round(dx * zoom * ppi / animatedImage.HorizontalResolution + rectAnimatedImage.X + scrollX);
							int drowY = (int)Math.Round(dy * zoom * ppi / animatedImage.VerticalResolution + rectAnimatedImage.Y + scrollY);
							Rectangle drowR = new Rectangle(drowX, drowY, (int)Math.Round(dw * zoom * ppi / animatedImage.HorizontalResolution), (int)Math.Round(dh * zoom * ppi / animatedImage.VerticalResolution));
							Rectangle bRect = new Rectangle(0, 0, bit.Width, bit.Height);
							if(!rectAnimatedImage.Contains(drowR))
							{
								Rectangle intR = new Rectangle(drowR.Location, drowR.Size);
								intR.Intersect(rectAnimatedImage);
								intR.Offset(thicknessFrame,  thicknessFrame);
								intR.Inflate(-thicknessFrame, -thicknessFrame);
								bRect.Offset(-(int)Math.Round((drowR.Left - intR.Left) * animatedImage.HorizontalResolution / ppi / zoom),
										-(int)Math.Round((drowR.Top - intR.Top) * animatedImage.VerticalResolution / ppi / zoom));
								bRect.Width -= (int)Math.Round((drowR.Width - intR.Width) * animatedImage.HorizontalResolution / ppi / zoom);
								bRect.Height -= (int)Math.Round((drowR.Height - intR.Height) * animatedImage.VerticalResolution / ppi / zoom);
								drowR = intR;
							}
							//int rx = 0;
							//if(drowX < rectAnimatedImage.X)
								//rx = rectAnimatedImage.X - drowX;
							g.DrawImage(bit, drowR, bRect, GraphicsUnit.Pixel);
							IsRefreshBitmap = true;
						}
					}
					base.modifiedStamps = item.Rotate != item.OriginalRotate;
					return;
				}
			}
			base.OnMouseWheel(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if(TypeWorkAnimatedImage == TypeWorkImage.EditNotes) // редактирование заметок (свойства, удаление, масштабирование, перемещение)
			{
				Select();
				if(stampItems != null && stampItems.Count != 0)
				{
					if(e.Button == MouseButtons.Left && renderAnnotations is Classes.RenderAnnotationsAndExternal)
					{
						Classes.RenderAnnotationsAndExternal ra = renderAnnotations as Classes.RenderAnnotationsAndExternal;
						if(ra.PageStamps != null)
						{
							List<Data.Temp.Objects.StampItem> pageSelecedStamps = new List<Data.Temp.Objects.StampItem>();
							Point mPoint = new Point(e.X - rectAnimatedImage.X, e.Y - rectAnimatedImage.Y);
							bool selectOne = false;
							bool selected = false;
							if(ra.PageStamps.Count == 0)
								return;
							foreach(Lib.Win.Data.Temp.Objects.StampItem figure in ((Classes.RenderAnnotationsAndExternal)renderAnnotations).PageStamps)
							{
								if(IsStampHit(figure, mPoint) && (figure.TypeID == 100 || figure.TypeID == 101 && figure.Employee.ID == Environment.CurEmp.ID))
								{
									selectOne = !selectOne && !selected;
									selected = true;
									pageSelecedStamps.Add(figure);
								}
							}
							bool hasStamp = ra.PageSelecedStamps != null && ra.PageSelecedStamps.Count > 0;
							if(ra.PageSelecedStamps == null)
								ra.PageSelecedStamps = new SynchronizedCollection<Data.Temp.Objects.StampItem>(new object(), pageSelecedStamps);
							else if(!controlPushed && !shiftPushed)
								if(ra.PageSelecedStamps == null || ra.PageSelecedStamps.Intersect(pageSelecedStamps).Count() == 0)
									ra.PageSelecedStamps = new SynchronizedCollection<Data.Temp.Objects.StampItem>(new object(), pageSelecedStamps);
								else
									if(shiftPushed)
										ra.PageSelecedStamps = new SynchronizedCollection<Lib.Win.Data.Temp.Objects.StampItem>(new object(), ra.PageSelecedStamps.Union(pageSelecedStamps));
									else
										if(controlPushed)
										{
											List<Data.Temp.Objects.StampItem> rs = ra.PageSelecedStamps.Except(pageSelecedStamps).ToList();
											ra.PageSelecedStamps = new SynchronizedCollection<Data.Temp.Objects.StampItem>(new object(), rs.Union(pageSelecedStamps.Except(ra.PageSelecedStamps)));
										}

							if(selected || hasStamp)
							{
								IsRefreshBitmap = true;
								if(selected)
								{
									Rectangle forMove = Rectangle.Empty;
									foreach(Data.Temp.Objects.StampItem item in stampItems)
									{
										Image stamp;
										if(item.TypeID == 101)
											stamp = Environment.GetDSP();
										else
											stamp = Environment.GetStamp(item.StampID, imageID);
										forMove = new Rectangle(item.X + (WidthSelectedtRect >> 1), item.Y + (WidthSelectedtRect >> 1), stamp.Width * item.Zoom / 100 - WidthSelectedtRect, stamp.Height * item.Zoom / 100 - WidthSelectedtRect);
										if(IsMouseOnRect(forMove, mPoint))
										{
											UserAction = UsersActionsTypes.EditNote;
											Cursor = Cursors.NoMove2D;
											startX = mPoint.X - scrollX;
											startY = mPoint.Y - scrollY;
											lastPositionForDrag = mPoint;
											break;
										}
									}
								}
								Invalidate(rectAnimatedImage);
								return;
							}
							else
							{
								UserAction = UsersActionsTypes.SelectionNotes;
								selectionNotesRectangle = Rectangle.Empty;
								lastPositionForDrag = mPoint;
							}

						}
					}
				}
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			if(UserAction == UsersActionsTypes.EditNote && isSizeChanged)
			{
				Classes.RenderAnnotationsAndExternal ra = renderAnnotations as Classes.RenderAnnotationsAndExternal;
				if(ra != null)
				{
					if(ra.PageSelecedStamps != null && ra.PageSelecedStamps.Count > 0)
					{
						List<Data.Temp.Objects.StampItem> selectedStamps = new List<Data.Temp.Objects.StampItem>(ra.PageSelecedStamps.Count);
						Point moved = new Point(startX - e.X + rectAnimatedImage.X + scrollX, startY - e.Y + rectAnimatedImage.Y + scrollY);
						int rightStop = animatedImage.Width - thicknessFrame;
						int bottomStop = animatedImage.Height - thicknessFrame;
						Image stamp = null;
						foreach(Data.Temp.Objects.StampItem item in ra.PageSelecedStamps.OrderBy(y => y.ID))
						{
							if(item.TypeID == 101)
							{
								stamp = Environment.GetDSP();
								item.Rotate = 0;
							}
							else
								stamp = Environment.GetStamp(item.StampID, imageID);
							////размер картинки 
							int width = (stamp.Width * item.Zoom) / 100;
							int height = (stamp.Height * item.Zoom) / 100;
							int x = item.X - (int)(moved.X * animatedImage.HorizontalResolution / ppi / zoom);
							int y = item.Y - (int)(moved.Y * animatedImage.VerticalResolution / ppi / zoom);

							item.X = (thicknessFrame > x) ? thicknessFrame : (rightStop - width < x) ? rightStop - width : x;
							item.Y = (thicknessFrame > y) ? thicknessFrame : (bottomStop - height < y) ? bottomStop - height : y;
							modifiedStamps = true;
						}
						IsRefreshBitmap = true;
						Invalidate();
						isSizeChanged = false;
						return;
					}
				}
			}
			else if(e.Button == MouseButtons.Left && IsMouseOnRectMain(base.rectThumbnailPanel, new Point(e.X, e.Y)))
			{
				base.OnMouseUp(e);
				return;
			}
			else if(UserAction == UsersActionsTypes.SelectionNotes)
			{
				Classes.RenderAnnotationsAndExternal ra = renderAnnotations as Classes.RenderAnnotationsAndExternal;
				if(ra != null)
				{
					if(ra.PageStamps != null && selectionNotesRectangle != Rectangle.Empty)
					{
						ra.PageSelecedStamps = new SynchronizedCollection<Data.Temp.Objects.StampItem>();
						Point mPoint = new Point(e.X - rectAnimatedImage.X, e.Y - rectAnimatedImage.Y);
						bool selectOne = false;
						int lastID = stampItems.Where(x => x.TypeID != 101).DefaultIfEmpty(new Data.Temp.Objects.StampItem(-1)).Max(x => x.ID);
						Image stamp;
						foreach(Data.Temp.Objects.StampItem figure in ra.PageStamps)
						{
							if(figure.TypeID == 101)
								stamp = Environment.GetDSP();
							else
								stamp = Environment.GetStamp(figure.StampID, imageID);
							Rectangle rect = new Rectangle(
										figure.X,
										figure.Y,
										(stamp.Width * figure.Zoom) / 100,
										(stamp.Height * figure.Zoom) / 100);
							if(IsRectOnRect(rect, selectionNotesRectangle) && (lastID == figure.ID || figure.TypeID == 101 && figure.Employee.ID == Environment.CurEmp.ID))
							{
								selectOne = true;
								ra.PageSelecedStamps.Add(figure);
							}
						}
						if(selectOne)
						{
							IsRefreshBitmap = true;
							Invalidate(rectAnimatedImage);
						}
						if(stampItems != null && stampItems.Count > 0)
							return;
					}
				}
			}
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(mouseTimer != null)
			{
				mouseTimer.Stop();
				mouseTimer.Tick -= new EventHandler(mouseTimer_Tick);
			}
			if(UserAction == UsersActionsTypes.EditNote && e.Button == MouseButtons.Left)
			{
				#region Перетаскивание штампов
				Classes.RenderAnnotationsAndExternal ra = renderAnnotations as Classes.RenderAnnotationsAndExternal;
				if(ra != null && ra.PageSelecedStamps != null && ra.PageSelecedStamps.Count > 0)
				{
					//координаты перемещаем в координаты рисунка
					Point mPointOffset = new Point(e.X - rectAnimatedImage.X, e.Y - rectAnimatedImage.Y);

					if(selectedRectangles != null)
					{
						AnnotationState = AnnotationsState.Drag;

						if(Cursor == Cursors.NoMove2D)
						{
							Image stamp;
							foreach(var item in ra.PageSelecedStamps)
							{
								if(item.TypeID == 101)
									stamp = Environment.GetDSP();
								else
									stamp = Environment.GetStamp(item.StampID, imageID);
								ShowMouseMove(stamp, item.Zoom / 100.0, item.Zoom / 100.0, mPointOffset, new Point(item.X - (int)((startX) * animatedImage.HorizontalResolution / ppi / zoom), (item.Y - (int)((startY) * animatedImage.VerticalResolution / ppi / zoom))), true);
							}

							//if(ra.PageSelecedStamps.Count == 1)
							//{
							//    Data.Temp.Objects.StampItem item = ra.PageSelecedStamps[0];
							//    if(item.TypeID == 101)
							//        stamp = Environment.GetDSP();
							//    else
							//        stamp = Environment.GetStamp(item.StampID, imageID);
							//    ShowMouseMove(stamp, item.Zoom / 100.0, item.Zoom / 100.0, mPointOffset, new Point(item.X - (int)((startX) * animatedImage.HorizontalResolution / ppi / zoom), (item.Y - (int)((startY) * animatedImage.VerticalResolution / ppi / zoom))), true);
							//}
							//else
							//    ShowMultiplyMove(ra.PageSelecedStamps, mPointOffset, true);
							
							return;
						}
					}
				}
				#endregion
			}
			else
				if(TypeWorkAnimatedImage == TypeWorkImage.CreateNotes && UserAction == UsersActionsTypes.DrawImage)
				{
					if(CurrentStamp != null)
					{
						if(IsMouseOnRectMain(new Rectangle(rectAnimatedImage.X, rectAnimatedImage.Y, realWidthImage, realHeightImage), new Point(e.X, e.Y)))
						{
							if(Cursor != Cursors.NoMove2D)
								Cursor = Cursors.NoMove2D;
						}
						else
							if(Cursor != Cursors.Default)
								Cursor = Cursors.Default;

						Point mPointOffset = new Point(e.X - rectAnimatedImage.X, e.Y - rectAnimatedImage.Y);
						ShowMouseMove(CurrentStamp, (double)animatedImage.VerticalResolution / CurrentStamp.VerticalResolution, (double)animatedImage.VerticalResolution / CurrentStamp.HorizontalResolution, mPointOffset, true);
						return;
					}
					else
						if(Cursor != Cursors.Default && Cursor != Cursors.WaitCursor)
							Cursor = Cursors.Default;

				}
			base.OnMouseMove(e);
		}

		private bool IsStampHit(Data.Temp.Objects.StampItem figure, Point mPoint)
		{
			if(renderAnnotations is Classes.RenderAnnotationsAndExternal)
			{
				Image image;
				if(figure.TypeID == 101)
					image = Environment.GetDSP();
				else
					image = Environment.GetStamp(figure.StampID, imageID);
				Rectangle rect = new Rectangle(
					figure.X,
					figure.Y,
					(image.Width * figure.Zoom) / 100,
					(image.Height * figure.Zoom) / 100);
				Rectangle rectWithSelected = new Rectangle(
					figure.X - (WidthSelectedtRect >> 1),
					figure.Y - (WidthSelectedtRect >> 1),
					(image.Width * figure.Zoom) / 100 + WidthSelectedtRect,
					(image.Height * figure.Zoom) / 100 + WidthSelectedtRect);
				return IsMouseOnRect(rect, mPoint) || IsMouseOnRect(rectWithSelected, mPoint) && Cursor != System.Windows.Forms.Cursors.Default && Cursor != System.Windows.Forms.Cursors.Hand;
			}
			return false;
		}

		/// <summary>
		/// отображение движения картинке по основной картинке
		/// </summary>
		/// <param name="bmp">движущаяся картинка</param>
		/// <param name="imageZoomV">маштаб картинки относительно основной по горизонтале</param>
		/// <param name="imageZoomH">маштаб картинки относительно основной по вертикале</param>
		/// <param name="mPointOffset">изменение пологжения</param>
		/// <param name="startTimer">таймер</param>
		private void ShowMouseMove(Image bmp, double imageZoomV, double imageZoomH, Point mPointOffset, bool startTimer)
		{
			ShowMouseMove(bmp, imageZoomV, imageZoomH, mPointOffset, Point.Empty, startTimer, true);
		}

		/// <summary>
		/// отображение множественного движения по картинке
		/// </summary>
		/// <param name="stamps"></param>
		/// <param name="mPointOffset"></param>
		/// <param name="startTimer"></param>
		private void ShowMultiplyMove(SynchronizedCollection<Data.Temp.Objects.StampItem> stamps, Point mPointOffset, bool startTimer)
		{
			Rectangle r = Rectangle.Empty;
			
			Image img;
			foreach(var item in stamps)
			{
				if(item.TypeID == 101)
							img = Environment.GetDSP();
				else
							img = Environment.GetStamp(item.StampID, item.ImageID);

				if(!r.Contains(new Rectangle(item.X, item.Y, (img.Width * item.Zoom) / 100, (img.Height * item.Zoom) / 100)))
				r = Rectangle.Union(r, new Rectangle(item.X, item.Y, (img.Width * item.Zoom) / 100, (img.Height * item.Zoom) / 100));
			}
			isSizeChanged = true;
			//координаты верхней левой точки
			float x = (float)(mPointOffset.X * animatedImage.HorizontalResolution / zoom / ppi - scrollX * animatedImage.HorizontalResolution / zoom / ppi);
			float y = (float)(mPointOffset.Y * animatedImage.VerticalResolution / zoom / ppi - scrollY * animatedImage.VerticalResolution / zoom / ppi);

			int leftStop = thicknessFrame - (int)(scrollX * animatedImage.HorizontalResolution / zoom / ppi);
			int topStop = thicknessFrame - (int)(scrollY * animatedImage.HorizontalResolution / zoom / ppi);
			int rightStop = (int)Math.Round(realWidthImage * animatedImage.HorizontalResolution / zoom / ppi - scrollX * animatedImage.HorizontalResolution / zoom / ppi) - thicknessFrame;
			int bottomStop = (int)Math.Round(realHeightImage * animatedImage.VerticalResolution / zoom / ppi - scrollY * animatedImage.VerticalResolution / zoom / ppi) - thicknessFrame;
			bool scrolling = false;

			if(MouseScroll((int)r.Width, (int)r.Height, ref x, ref y, leftStop, topStop, rightStop, bottomStop, mPointOffset.X > rectAnimatedImage.Width || mPointOffset.X < 0 || mPointOffset.Y > rectAnimatedImage.Height || mPointOffset.Y < 0))
			{
				scrolling = true;
				if(startTimer)
				{
					if(mouseTimer == null)
						mouseTimer = new Timer();
					mouseTimer.Interval = 200;
					mouseTimer.Tick += new EventHandler(mouseTimer_Tick);
					mouseTimer.Start();
				}
				leftStop = thicknessFrame - (int)(scrollX * animatedImage.HorizontalResolution / zoom / ppi);
				topStop = thicknessFrame - (int)(scrollY * animatedImage.VerticalResolution / zoom / ppi);
				rightStop = (int)Math.Round(realWidthImage * animatedImage.HorizontalResolution / zoom / ppi - scrollX * animatedImage.HorizontalResolution / zoom / ppi) - thicknessFrame;
				bottomStop = (int)Math.Round(realHeightImage * animatedImage.VerticalResolution / zoom / ppi - scrollY * animatedImage.VerticalResolution / zoom / ppi) - thicknessFrame;
			}
			else
				mouseTimerStop();
			if(x + r.Width > rightStop)
				x = rightStop - r.Width;
			if(x + r.Width > animatedImage.Width)
				x = (float)(animatedImage.Width - r.Width);
			if(x < leftStop)
				x = leftStop;
			if(y + r.Height > bottomStop)
				y = bottomStop - r.Height;
			if(y + r.Height > animatedImage.Height)
				y = (float)(animatedImage.Height - r.Height);
			if(y < topStop)
				y = topStop;
			int reInvalidateX = invalidRect.X;
			int reInvalidateY = invalidRect.Y;
			int reInvalidateWidth = invalidRect.Width;
			int reInvalidateHeight = invalidRect.Height;
			int drowX = reInvalidateX + (int)(scrollX * animatedImage.HorizontalResolution / zoom / ppi);
			int drowY = reInvalidateY + (int)(scrollY * animatedImage.VerticalResolution / zoom / ppi);
			float iwidth = r.Width;
			float iheight = r.Height;
			if(drowX > (int)x)
			{
				int dx = (int)x - drowX;
				drowX += dx;
				reInvalidateWidth -= dx;
				reInvalidateX += dx;
			}
			if(drowX < 0)
			{
				int dx = -drowX;
				drowX = 0;
				reInvalidateWidth -= dx;
				reInvalidateX += dx;
			}

			if(drowX + reInvalidateWidth < x + r.Width)
			{
				reInvalidateWidth = (int)Math.Round(x + r.Width - drowX);
			}

			if((int)Math.Round(reInvalidateWidth * zoom * ppi / animatedImage.HorizontalResolution) > realWidthImage)
			{
				reInvalidateWidth = (int)(realWidthImage * animatedImage.HorizontalResolution / zoom / ppi);
			}

			if(drowY > (int)y)
			{
				int dy = (int)y - drowY;
				drowY += dy;
				reInvalidateHeight -= dy;
				reInvalidateY += dy;
			}
			if(drowY < 0)
			{
				int dy = -drowY;
				drowY = 0;
				reInvalidateHeight -= dy;
				reInvalidateY += dy;
			}
			if(drowY + reInvalidateHeight < y + r.Height)
			{
				reInvalidateHeight = (int)Math.Round(y + r.Height - drowY);
			}
			if((int)Math.Round(reInvalidateHeight * zoom * ppi / animatedImage.VerticalResolution) > realHeightImage)
			{
				reInvalidateHeight = (int)(realHeightImage * animatedImage.VerticalResolution / zoom / ppi);
			}
			drowY = (int)(drowY * zoom * ppi / animatedImage.VerticalResolution);
			drowX = (int)(drowX * zoom * ppi / animatedImage.HorizontalResolution);
			drowX += rectAnimatedImage.X + (thicknessFrame);
			drowY += rectAnimatedImage.Y + (thicknessFrame);
			oldRect = new Rectangle((int)x, (int)y, (int)r.Width, (int)r.Height);
			invalidRect = new Rectangle((int)x - sin, (int)y - sin, (int)r.Width + (sin << 1) - 1, (int)r.Height + (sin << 1) - 1);
			if(scrolling)
			{
				using(Graphics g = Graphics.FromHwnd(Handle))
				{
					foreach(var st in stamps)
					{
						if(st.TypeID == 101)
							img = Environment.GetDSP();
						else
							img = Environment.GetStamp(st.StampID, st.ImageID);
						g.DrawImage(img, (float)(x * zoom * ppi / animatedImage.HorizontalResolution) + scrollX + rectAnimatedImage.X + thicknessFrame, (float)(y * zoom * ppi / animatedImage.VerticalResolution) + scrollY + rectAnimatedImage.Y + (thicknessFrame), (float)(img.Width * zoom * ppi / animatedImage.HorizontalResolution), (float)(img.Height * zoom * ppi / animatedImage.VerticalResolution));
					}
				}
			}
			else
			{
				using(Bitmap bit = new Bitmap(reInvalidateWidth, reInvalidateHeight))
				{
					bit.SetResolution(animatedImage.VerticalResolution, animatedImage.HorizontalResolution);
					using(Graphics gr = Graphics.FromImage(bit))
					{
						gr.InterpolationMode = CurrentInterpolationMode;
						gr.DrawImage(cachedBitmap, 0, 0, new Rectangle(reInvalidateX, reInvalidateY, reInvalidateWidth, reInvalidateHeight), GraphicsUnit.Pixel);
						var st = stamps.First();
						{
							if(st.TypeID == 101)
								img = Environment.GetDSP();
							else
								img = Environment.GetStamp(st.StampID, st.ImageID);
							if(st.Rotate != 0)
							{
								System.Drawing.Drawing2D.Matrix mx = new System.Drawing.Drawing2D.Matrix();
								mx.RotateAt(st.Rotate, new PointF((int)Math.Round(x  +( img.Width * st.Zoom) / 200.0), (int)Math.Round(y  +( img.Height * st.Zoom) / 200.0)));
								gr.Transform = mx;
							}
							gr.DrawImage(img, (float)(x * zoom * ppi / animatedImage.HorizontalResolution) + scrollX + rectAnimatedImage.X + thicknessFrame, (float)(y * zoom * ppi / animatedImage.VerticalResolution) + scrollY + rectAnimatedImage.Y + (thicknessFrame), (float)(img.Width * zoom * ppi / animatedImage.HorizontalResolution), (float)(img.Height * zoom * ppi / animatedImage.VerticalResolution));
							gr.ResetTransform();
						}
					}
					using(Graphics g = Graphics.FromHwnd(Handle))
					{
						g.InterpolationMode = CurrentInterpolationMode;
						g.DrawImage(bit, drowX, drowY, (int)Math.Round(reInvalidateWidth * zoom * ppi / animatedImage.HorizontalResolution), (int)Math.Round(reInvalidateHeight * zoom * ppi / animatedImage.VerticalResolution));
					}
				}
			}
		}

		/// <summary>
		/// отображение движения картинке по основной картинке
		/// </summary>
		/// <param name="bmp">движущаяся картинка</param>
		/// <param name="imageZoomV">маштаб картинки относительно основной по горизонтале</param>
		/// <param name="imageZoomH">маштаб картинки относительно основной по вертикале</param>
		/// <param name="mPointOffset">изменение положения</param>
		/// <param name="startOffset">сдвиг картинки относительно положения мышки</param>
		/// <param name="startTimer">таймер</param>
		private void ShowMouseMove(Image bmp, double imageZoomV, double imageZoomH, Point mPointOffset, Point startOffset, bool startTimer, bool newI = false)
		{
			isSizeChanged = true;
			////размер картинки 
			float width = (float)(bmp.Width * imageZoomV);
			float height = (float)(bmp.Height * imageZoomH);
			if(newI)
				startOffset = new Point(-(int)width / 2, -(int)height / 2);
			//координаты верхней левой точки
			float x = (float)((mPointOffset.X - scrollX) * animatedImage.VerticalResolution / zoom / ppi + startOffset.X);
			float y = (float)((mPointOffset.Y - scrollY) * animatedImage.VerticalResolution / zoom / ppi + startOffset.Y);

			int leftStop = thicknessFrame + scrollX;
			int topStop = thicknessFrame + scrollY;
			int rightStop = (int)Math.Round(realWidthImage * animatedImage.HorizontalResolution / zoom / ppi - scrollX * animatedImage.HorizontalResolution / zoom / ppi) - thicknessFrame;
			int bottomStop = (int)Math.Round(realHeightImage * animatedImage.VerticalResolution / zoom / ppi - scrollY * animatedImage.VerticalResolution / zoom / ppi) - thicknessFrame;
			bool scrolling = false;

			if(MouseScroll((int)width, (int)height, ref x, ref y, leftStop, topStop, rightStop, bottomStop, mPointOffset.X > rectAnimatedImage.Width || mPointOffset.X < 0 || mPointOffset.Y > rectAnimatedImage.Height || mPointOffset.Y < 0))
			{
				scrolling = true;
				if(startTimer)
				{
					if(mouseTimer == null)
						mouseTimer = new Timer();
					mouseTimer.Interval = 200;
					mouseTimer.Tick += new EventHandler(mouseTimer_Tick);
					mouseTimer.Start();
				}
				leftStop = thicknessFrame - (int)(scrollX * animatedImage.HorizontalResolution / zoom / ppi);
				topStop = thicknessFrame - (int)(scrollY * animatedImage.VerticalResolution / zoom / ppi);
				rightStop = (int)Math.Round(realWidthImage * animatedImage.HorizontalResolution / zoom / ppi - scrollX * animatedImage.HorizontalResolution / zoom / ppi) - thicknessFrame;
				bottomStop = (int)Math.Round(realHeightImage * animatedImage.VerticalResolution / zoom / ppi - scrollY * animatedImage.VerticalResolution / zoom / ppi) - thicknessFrame;
			}
			else
				mouseTimerStop();
			if(x + width > rightStop)
				x = rightStop - width;
			if(x + width > animatedImage.Width)
				x = (float)(animatedImage.Width - width);
			if(x < leftStop)
				x = leftStop;
			if(y + height > bottomStop)
				y = bottomStop - height;
			if(y + height > animatedImage.Height)
				y = (float)(animatedImage.Height - height);
			if(y < topStop)
				y = topStop;
			int reInvalidateX = invalidRect.X;
			int reInvalidateY = invalidRect.Y;
			int reInvalidateWidth = invalidRect.Width;
			int reInvalidateHeight = invalidRect.Height;
			int drowX = reInvalidateX + (int)(scrollX * animatedImage.HorizontalResolution / zoom / ppi);
			int drowY = reInvalidateY + (int)(scrollY * animatedImage.VerticalResolution / zoom / ppi);
			float iwidth = width;
			float iheight = height;
			if(drowX > (int)x)
			{
				int dx = (int)x - drowX;
				drowX += dx;
				reInvalidateWidth -= dx;
				reInvalidateX += dx;
			}
			if(drowX < 0)
			{
				int dx = -drowX;
				drowX = 0;
				reInvalidateWidth -= dx;
				reInvalidateX += dx;
			}

			if(drowX + reInvalidateWidth < x + width)
			{
				reInvalidateWidth = (int)Math.Round(x + width - drowX);
			}

			if((int)Math.Round(reInvalidateWidth * zoom * ppi / animatedImage.HorizontalResolution) > realWidthImage)
			{
				reInvalidateWidth = (int)(realWidthImage * animatedImage.HorizontalResolution / zoom / ppi);
			}

			if(drowY > (int)y)
			{
				int dy = (int)y - drowY;
				drowY += dy;
				reInvalidateHeight -= dy;
				reInvalidateY += dy;
			}
			if(drowY < 0)
			{
				int dy = -drowY;
				drowY = 0;
				reInvalidateHeight -= dy;
				reInvalidateY += dy;
			}
			if(drowY + reInvalidateHeight < y + height)
			{
				reInvalidateHeight = (int)Math.Round(y + height - drowY);
			}
			if((int)Math.Round(reInvalidateHeight * zoom * ppi / animatedImage.VerticalResolution) > realHeightImage)
			{
				reInvalidateHeight = (int)(realHeightImage * animatedImage.VerticalResolution / zoom / ppi);
			}
			drowY = (int)(drowY * zoom * ppi / animatedImage.VerticalResolution);
			drowX = (int)(drowX * zoom * ppi / animatedImage.HorizontalResolution);
			drowX += rectAnimatedImage.X + (thicknessFrame);
			drowY += rectAnimatedImage.Y + (thicknessFrame);
			oldRect = new Rectangle((int)x, (int)y, (int)width, (int)height);
			invalidRect = new Rectangle((int)x - sin, (int)y - sin, (int)width + (sin << 1) - 1, (int)height + (sin << 1) - 1);
			if(scrolling)
			{
				using(Graphics g = Graphics.FromHwnd(Handle))
					g.DrawImage(bmp, (float)(x * zoom * ppi / animatedImage.HorizontalResolution) + scrollX + rectAnimatedImage.X + thicknessFrame, (float)(y * zoom * ppi / animatedImage.VerticalResolution) + scrollY + rectAnimatedImage.Y + (thicknessFrame), (float)(width * zoom * ppi / animatedImage.HorizontalResolution), (float)(height * zoom * ppi / animatedImage.VerticalResolution));
			}
			else
			{
				using(Bitmap bit = new Bitmap(reInvalidateWidth, reInvalidateHeight))
				{
					bit.SetResolution(animatedImage.VerticalResolution, animatedImage.HorizontalResolution);
					using(Graphics gr = Graphics.FromImage(bit))
					{
						gr.InterpolationMode = CurrentInterpolationMode;
						gr.DrawImage(cachedBitmap, 0, 0, new Rectangle(reInvalidateX, reInvalidateY, reInvalidateWidth, reInvalidateHeight),
GraphicsUnit.Pixel);
						gr.DrawImage(bmp, x - reInvalidateX, y - reInvalidateY, width, height);
					}
					using(Graphics g = Graphics.FromHwnd(Handle))
					{
						g.InterpolationMode = CurrentInterpolationMode;
						g.DrawImage(bit, drowX, drowY, (int)Math.Round(reInvalidateWidth * zoom * ppi / animatedImage.HorizontalResolution), (int)Math.Round(reInvalidateHeight * zoom * ppi / animatedImage.VerticalResolution));
					}
				}
			}
		}

		protected override void OnRightClick(System.Windows.Forms.MouseEventArgs e)
		{
			Classes.RenderAnnotationsAndExternal ra = renderAnnotations as Classes.RenderAnnotationsAndExternal;
			if(ra != null && ra.PageSelecedStamps != null && ra.PageSelecedStamps.Count > 0)
			{
				new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] { new System.Windows.Forms.MenuItem(Environment.StringResources.GetString("DeleteStamp"), mi_Click, System.Windows.Forms.Shortcut.ShiftDel) }).Show(this, e.Location);
			}
			else
				base.OnRightClick(e);
		}

		void mi_Click(object sender, EventArgs e)
		{
			Classes.RenderAnnotationsAndExternal ra = renderAnnotations as Classes.RenderAnnotationsAndExternal;
			if(ra != null && ra.PageSelecedStamps != null && ra.PageSelecedStamps.Count > 0)
			{
				Lib.Win.MessageForm mf = new Lib.Win.MessageForm(Environment.StringResources.GetString("ImageShowControl_mi_Click_Message1"), Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNo);
				if(mf.ShowDialog() == DialogResult.Yes)
				{
					if(ra != null && ra.PageSelecedStamps != null && ra.PageSelecedStamps.Count > 0)
					{
						foreach(Data.Temp.Objects.StampItem st in ((Classes.RenderAnnotationsAndExternal)renderAnnotations).PageSelecedStamps.OrderByDescending(x => x.ID))
						{
							if(!Environment.DocSignatureData.Delete(st.ID))
								break;
						}
						ReloadImageData(false, Page - 1);
					}
				}
			}
		}

		internal class CompareID : IEqualityComparer<KeyValuePair<Data.Temp.Objects.StampItem, byte[]>>
		{
			public bool Equals(KeyValuePair<Data.Temp.Objects.StampItem, byte[]> x, KeyValuePair<Data.Temp.Objects.StampItem, byte[]> y)
			{
				return x.Key.StampID == y.Key.StampID;
			}

			public int GetHashCode(KeyValuePair<Data.Temp.Objects.StampItem, byte[]> obj)
			{
				return obj.Key.StampID;
			}
		}

		protected override void OnPageChange()
		{
			if(base.Page > 0)
			{
				if(stampItems != null)
				{
					((Classes.RenderAnnotationsAndExternal)renderAnnotations).PageStamps = new SynchronizedCollection<Data.Temp.Objects.StampItem>(new object(), stampItems.FindAll(x => x.Page == Page));
					IsRefreshBitmap = true;
					Invalidate();
				}
				if(CurrentStampID == -101)
				{
					CurrentStamp = null;
					SelectTool(0);
				}
			}
			base.OnPageChange();
		}

		protected override bool ProcessDialogKey(System.Windows.Forms.Keys keyData)
		{
			if(keyData == (System.Windows.Forms.Keys.Delete | System.Windows.Forms.Keys.Shift))
			{
				mi_Click(this, EventArgs.Empty);
				return true;
			}
			else
				return base.ProcessDialogKey(keyData);
		}

		void mouseTimer_Tick(object sender, EventArgs e)
		{
			Point mPointOffset = PointToClient(MousePosition);
			mPointOffset = new Point(mPointOffset.X - rectAnimatedImage.X, mPointOffset.Y - rectAnimatedImage.Y);
			if(UserAction == UsersActionsTypes.EditNote)
			{
				#region Перетаскивание штампов
				Classes.RenderAnnotationsAndExternal ra = renderAnnotations as Classes.RenderAnnotationsAndExternal;
				if(ra != null && ra.PageSelecedStamps != null && ra.PageSelecedStamps.Count > 0)
				{
					if(selectedRectangles != null)
					{
						AnnotationState = AnnotationsState.Drag;

						if(Cursor == Cursors.NoMove2D)
						{
							Image img = null;
							if(ra.PageSelecedStamps[0].TypeID == 101)
								img = Environment.GetDSP();
							else
								img = Environment.GetStamp(ra.PageSelecedStamps[0].StampID, imageID);
							ShowMouseMove(img, ra.PageSelecedStamps[0].Zoom / 100.0, ra.PageSelecedStamps[0].Zoom / 100.0, mPointOffset, false);
							return;
						}
					}
				}
				#endregion
			}
			else
				if(TypeWorkAnimatedImage == TypeWorkImage.CreateNotes && UserAction == UsersActionsTypes.DrawImage)
				{
					if(CurrentStamp != null)
					{
						ShowMouseMove(CurrentStamp, (double)animatedImage.VerticalResolution / CurrentStamp.VerticalResolution, animatedImage.HorizontalResolution / CurrentStamp.HorizontalResolution, mPointOffset, false);
						return;
					}
				}
		}

		private void mouseTimerStop()
		{
			if(mouseTimer != null)
			{
				mouseTimer.Stop();
				mouseTimer.Tick -= new EventHandler(mouseTimer_Tick);
			}
		}

		private bool MouseScroll(float width, float height, ref float x, ref float y, int leftStop, int topStop, int rightStop, int bottomStop, bool largeChange)
		{
			bool retval = false;
			if(2 * width < realWidthImage / zoom)
			{
				if(x + width > rightStop && scrollImageHorizontal.Value < scrollImageHorizontal.Maximum - realWidthImage)
				{
					x += (float)(scrollX / zoom);
					if(scrollImageHorizontal.Value + (largeChange ? scrollImageHorizontal.SmallChange * 15 : scrollImageHorizontal.SmallChange) > scrollImageHorizontal.Maximum - realWidthImage)
						ScrollPositionX = realWidthImage - scrollImageHorizontal.Maximum;
					else
						ScrollPositionX -= (largeChange ? scrollImageHorizontal.SmallChange * 15 : scrollImageHorizontal.SmallChange);
					x -= (float)(scrollX / zoom);
					retval = true;
				}
				else
					if(x < leftStop && scrollX < 0)
					{
						x += (float)(scrollX / zoom);
						if(ScrollPositionX > -(largeChange ? scrollImageHorizontal.SmallChange * 15 : scrollImageHorizontal.SmallChange))
							ScrollPositionX = 0;
						else
							ScrollPositionX += (largeChange ? scrollImageHorizontal.SmallChange * 15 : scrollImageHorizontal.SmallChange);
						x -= (float)(scrollX / zoom);
						retval = true;
					}
			}
			if(2 * height < realHeightImage / zoom)
			{
				if(y + height > bottomStop && scrollImageVertical.Value < scrollImageVertical.Maximum - realHeightImage)
				{
					y += (float)(scrollY / zoom);
					if(scrollImageVertical.Value + (largeChange ? scrollImageVertical.SmallChange * 15 : scrollImageVertical.SmallChange) > scrollImageVertical.Maximum - realHeightImage)
						ScrollPositionY = realHeightImage - scrollImageVertical.Maximum;
					else
						ScrollPositionY -= (largeChange ? scrollImageVertical.SmallChange * 15 : scrollImageVertical.SmallChange);
					y -= (float)(scrollY / zoom);
					retval = true;
				}
				else
					if(y < topStop && scrollY < 0)
					{
						y += (float)(scrollY / zoom);
						if(ScrollPositionY > -(largeChange ? scrollImageVertical.SmallChange * 15 : scrollImageVertical.SmallChange))
							ScrollPositionY = 0;
						else
							ScrollPositionY += (largeChange ? scrollImageVertical.SmallChange * 15 : scrollImageVertical.SmallChange);
						retval = true;
						y -= (float)(scrollY / zoom);
					}
			}
			return retval;
		}

		protected override void WndProc(ref Message m)
		{
			try
			{
				if(m.Msg == (int)Win32.Msgs.WM_LBUTTONDOWN)
				{
					controlPushed = (m.WParam.ToInt32() & (Int32)Win32.MK.MK_CONTROL) == 8;
					shiftPushed = (m.WParam.ToInt32() & (Int32)Win32.MK.MK_SHIFT) == 4;
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, m.ToString());
			}
			base.WndProc(ref m);
		}

		public override void MovePage(int page)
		{
			if(CanSave)
				base.MovePage(page);
		}

        /// <summary>
        /// Поворот изображения
        /// </summary>
        /// <returns></returns>
		public override bool RotateLeft()
		{
			RotateStampsLeft();

			if(!base.RotateLeft())
				return false;
			if(imageID > 0 && !CanSave)
				_pageVirtualRotationDictionary.SaveVirtualRotationLocal(imageID, Page - 1, false);
			return true;
		}

        /// <summary>
        /// Поворот изображения
        /// </summary>
        /// <returns></returns>
		public override bool RotateRight()
		{
			RotateStampsRight();

			if(!base.RotateRight())
				return false;
			if(imageID > 0 && !CanSave)
				_pageVirtualRotationDictionary.SaveVirtualRotationLocal(imageID, Page - 1, true);
			return true;
		}

        /// <summary>
        /// Повернуть штампы
        /// </summary>
        private void RotateStampsLeft(bool swapAnimatedImageDimensions = false)
	    {
            RotateStamps(270, swapAnimatedImageDimensions);
	    }

        /// <summary>
        /// Повернуть штампы
        /// </summary>
        private void RotateStampsRight(bool swapAnimatedImageDimensions = false)
        {
            RotateStamps(90, swapAnimatedImageDimensions);
        }

        /// <summary>
        /// Повернуть штампы
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="swapAnimatedImageDimensions"></param>
	    private void RotateStamps(int angle, bool swapAnimatedImageDimensions = false)
	    {
            if (stampItems != null)
            {
                foreach (var stamp in stampItems)
                {
                    // Старая логика DSP if (Page == 1 && stampItems != null && stampItems.Count == 1 && stampItems[0].TypeID == 101)
                    // Реализация старой логики
                    if (stamp.Page != Page)
                        continue;

                    var newPoint = GetRotatePoint(stamp, angle, swapAnimatedImageDimensions);
                    stamp.X = newPoint.X;
                    stamp.Y = newPoint.Y;
					if(stamp.TypeID != 101)
					{
						// Применяю угол поворота
						stamp.Rotate += angle;

						// Привожу значение в диапазон 0 - +359 градусов
						stamp.Rotate = (stamp.Rotate + 360) % 360;
					}
                }
            }
	    }

	    /// <summary>
	    /// Получить точку на текущей странице изображения применив вращение
	    /// </summary>
	    /// <param name="stamp"></param>
	    /// <param name="angle"></param>
	    /// <param name="swapAnimatedImageDimensions"></param>
	    /// <returns></returns>
	    private Point GetRotatePoint(Data.Temp.Objects.StampItem stamp, int angle, bool swapAnimatedImageDimensions = false)
        {
            var oldPoint = new Point(stamp.X, stamp.Y);

            if (animatedImage == null)
                return oldPoint;

            var newPoint = oldPoint;

            int h = animatedImage.Height;
            int w = animatedImage.Width;

            if (swapAnimatedImageDimensions)
            {
                var t = h;
                h = w;
                w = t;
            }

            Image stampImage;

            if (stamp.TypeID == 101)
                stampImage = Environment.GetDSP();
            else
                stampImage = Environment.GetStamp(stamp.StampID, stamp.ImageID);

            var dspWidth = stampImage.Width;

            switch (angle)
            {
                case 90:

                    newPoint.X = h - oldPoint.Y - (int)(stampImage.Width / 2.0 * animatedImage.VerticalResolution / stampImage.VerticalResolution + stampImage.Height / 2.0 * animatedImage.HorizontalResolution / stampImage.HorizontalResolution);

                    if (newPoint.X < thicknessFrame)
                        newPoint.X = thicknessFrame;

                    if (newPoint.X > h - (int)(stampImage.Width * animatedImage.HorizontalResolution / stampImage.VerticalResolution) - thicknessFrame)
                        newPoint.X = h - (int)(stampImage.Width * animatedImage.HorizontalResolution / stampImage.VerticalResolution) - thicknessFrame;

                    newPoint.Y = oldPoint.X + (int)(stampImage.Width / 2.0 * animatedImage.VerticalResolution / stampImage.VerticalResolution - stampImage.Height / 2.0 * animatedImage.HorizontalResolution / stampImage.HorizontalResolution);

                    if (newPoint.Y < thicknessFrame)
                        newPoint.Y = thicknessFrame;

                    if (newPoint.Y > w - (int)(stampImage.Height * animatedImage.VerticalResolution / stampImage.VerticalResolution) - thicknessFrame)
                        newPoint.Y = w - (int)(stampImage.Height * animatedImage.VerticalResolution / stampImage.VerticalResolution) - thicknessFrame;

                    break;

                case 180:

                    stampItems[0].X = w - oldPoint.X - (int)(stampImage.Width * animatedImage.HorizontalResolution / stampImage.HorizontalResolution);
                    stampItems[0].Y = h - oldPoint.Y - (int)(stampImage.Height * animatedImage.VerticalResolution / stampImage.VerticalResolution);

                    break;

                case 270:

                    newPoint.X = oldPoint.Y - (int)(dspWidth / 2.0 * animatedImage.VerticalResolution / stampImage.VerticalResolution - stampImage.Height / 2.0 * animatedImage.HorizontalResolution / stampImage.HorizontalResolution);

                    if (newPoint.X < thicknessFrame)
                        newPoint.X = thicknessFrame;

                    if (newPoint.X > animatedImage.Height - (int)(stampImage.Width * animatedImage.HorizontalResolution / stampImage.VerticalResolution) - thicknessFrame)
                        newPoint.X = animatedImage.Height - (int)(stampImage.Width * animatedImage.HorizontalResolution / stampImage.VerticalResolution) - thicknessFrame;

                    newPoint.Y = animatedImage.Width - oldPoint.X - (int)(stampImage.Width / 2.0 * animatedImage.HorizontalResolution / stampImage.VerticalResolution + stampImage.Height / 2.0 * animatedImage.HorizontalResolution / stampImage.HorizontalResolution);

                    if (newPoint.Y < thicknessFrame)
                        newPoint.Y = thicknessFrame;

                    if (newPoint.Y > animatedImage.Width - (int)(stampImage.Height * animatedImage.VerticalResolution / stampImage.VerticalResolution) - thicknessFrame)
                        newPoint.Y = animatedImage.Width - (int)(stampImage.Height * animatedImage.VerticalResolution / stampImage.VerticalResolution) - thicknessFrame;

                    break;
            }
            return newPoint;
        }

 		internal void ReloadStamps()
		{
			int sx = scrollX;
			int sy = scrollY;
			ReloadImageData(false, Page - 1);
			ScrollPositionX = sx;
			ScrollPositionY = sy;
		}

        /// <summary>
        /// Сохранить координаты штампов
        /// </summary>
	    internal void SaveStampsPositions()
	    {
			if(stampItems == null)
				return;
			List<Data.Temp.Objects.StampItem> stamps = stampItems.Where(x => x.CooordinateChanged).ToList();
			if(stamps.Count > 0)
				if(Environment.DocSignatureData.StampReplace(stamps))
				{
					Classes.RenderAnnotationsAndExternal ra = renderAnnotations as Classes.RenderAnnotationsAndExternal;
					if(ra != null && ra.PageSelecedStamps != null && ra.PageSelecedStamps.Count > 0)
						ra.PageSelecedStamps.Clear();
					IsRefreshBitmap = true;
					Invalidate();
				}
	    }

	    internal string GetImageType()
		{
			if(ImageDisplayed)
			{
				if(!AnotherFormat)
					return "Tif";
				else if(animatedImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Tiff.Guid)
					return "Tiff";
				else if(animatedImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid)
					return "Gif";
				else if(animatedImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Bmp.Guid)
					return "Bmp";
				else if(animatedImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Emf.Guid)
					return "Emf";
				else if(animatedImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Exif.Guid)
					return "Exif";
				else if(animatedImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Icon.Guid)
					return "Icon";
				else if(animatedImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
					return "Jpeg";
				else if(animatedImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.MemoryBmp.Guid)
					return "MemoryBmp";
				else if(animatedImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Png.Guid)
					return "Png";
				else if(animatedImage.RawFormat.Guid == System.Drawing.Imaging.ImageFormat.Wmf.Guid)
					return "Wmf";
				else
					return "Unknown";

			}
			else
				return null;
		}

		internal void ShowProperties()
		{
			if(ImageDisplayed)
			{
				PropertiesDialogs.PropertiesImageDialog dial;
				if(AnotherFormat)
					dial = new PropertiesDialogs.PropertiesImageDialog(fileName, Page);
				else
					dial = new PropertiesDialogs.PropertiesImageDialog(fileName, Page, string.Format("{0}x{1}", animatedImage.HorizontalResolution, animatedImage.VerticalResolution),
					((animatedImage.Height * 2.54) / animatedImage.HorizontalResolution).ToString("N2") + "cm x " + ((animatedImage.Width * 2.54) / animatedImage.VerticalResolution).ToString("N2") + "cm", animatedImage.PixelFormat.ToString());
				dial.Owner = FindForm();
				dial.Show();
			}
		}

		internal void ShowProperties(string curDocString)
		{
			if(ImageDisplayed)
			{
				PropertiesDialogs.PropertiesImageDialog dial;
				if(AnotherFormat)
					dial = new PropertiesDialogs.PropertiesImageDialog(fileName, Page);
				else
					dial = new PropertiesDialogs.PropertiesImageDialog(fileName, Page, string.Format("{0}x{1}", animatedImage.HorizontalResolution, animatedImage.VerticalResolution),
					((animatedImage.Height * 2.54) / animatedImage.HorizontalResolution).ToString("N2") + "cm x " + ((animatedImage.Width * 2.54) / animatedImage.VerticalResolution).ToString("N2") + "cm", animatedImage.PixelFormat.ToString());
				dial.Owner = FindForm();
				dial.Show();
			}
		}

		public void TestImage(Document.Environment.ActionBefore act)
		{
			if(animatedImage != null && (Modified || ModifiedStamps))
			{
				SaveEventArgs args = new SaveEventArgs();
				args.Action = act;
				OnNeedSave(new ImageControl.SaveEventHandler(ChangePage), args);
			}
		}

		internal Data.Temp.Objects.StampItem GetEditedDSP()
		{
			if(stampItems != null)
				return stampItems.FirstOrDefault(x => x.TypeID == 101);
			return null;
		}

        /// <summary>
        /// Получить угол поворота страницы текущего изображения
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
		protected override int GetPageRotation(int page)
		{
			if(HaveVirtualRotation)
				return _pageVirtualRotationDictionary[page];
			else
				return base.GetPageRotation(page);
		}

        /// <summary>
        /// Обновить угол поворота страницы текущего изображения
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        protected override void UpdatePageRotation(int page)
        {
			if(imageID > 0 && !CanSave)
				_pageVirtualRotationDictionary.UpdatePageRotation(ImageID, page);
        }

        /// <summary>
        /// Сбросить уголы поворота страниц текущего изображения
        /// </summary>
        protected override void CleanRotation()
		{
			_pageVirtualRotationDictionary.CleanRotations();
		}
	}
}