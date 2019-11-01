using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.ImageControl;

namespace Kesco.Lib.Win.Document.Classes
{
    internal class PrintImage : ImageControl.PrintImage
    {
        protected override void Print_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (printingPage == -1)
                printingPage = startPage;
            else
                printingPage++;
            int rx = e.PageSettings.PrinterResolution.X, ry = e.PageSettings.PrinterResolution.Y;
            const int rdef = 300;

            OnPagePrinded(new PageEventArgs(printingPage - startPage));
            var pi = images[printingPage - startPage] as Tiff.PageInfo;
			if(pi != null && pi.Image != null)
			{
				Image img = pi.Image;
				if(annotations)
				{
					var renderAnnotations = new RenderAnnotations(0);
					byte[] notes = pi.Annotation;
					if(notes != null)
					{
						Bitmap bitmapImg;
						using(TiffAnnotation tiffAnnotation = new TiffAnnotation(null))
						{
							tiffAnnotation.Parse(notes);
							bitmapImg = new Bitmap(img);

							if(img.HorizontalResolution == 0)
								bitmapImg.SetResolution(200, 200);
							else
								bitmapImg.SetResolution(img.HorizontalResolution, img.VerticalResolution);

							using(Graphics g = Graphics.FromImage(bitmapImg))
							{
								g.InterpolationMode = InterpolationMode.High;
								Rectangle[] rects = null;
								renderAnnotations.DrawAnnotation(g, tiffAnnotation,
																 ImageControl.ImageControl.AllMarkGroupsVisibleList,
																 bitmapImg, InterpolationMode.High, ref rects, null);
							}
						}
						img = bitmapImg;
					}
				}

				float hr = img.HorizontalResolution > 0 ? img.HorizontalResolution : 200;
				float vr = img.VerticalResolution > 0 ? img.VerticalResolution : 200;
				if(scaleMode == 1)
				{
					e.Graphics.DrawImage(img, 0f, 0f, img.Width * 100f / hr, img.Height * 100f / vr);
					if(images[printingPage - startPage] is PrintImageInfo)
					{
						PrintImageInfo ii = images[printingPage - startPage] as PrintImageInfo;
						if(ii != null && ii.StampsItems != null && ii.StampsItems.Length > 0)
						{
							float drez = img.VerticalResolution / img.HorizontalResolution;
							foreach(StampItem si in ii.StampsItems)
							{
								if(si.TypeID == 101)
									continue;
								Image simg = Environment.GetStamp(si.StampID, si.ImageID);
								if(!si.Rotate.Equals(0))
								{
									Matrix mx = new Matrix();
									mx.RotateAt(si.Rotate,
												new PointF(si.X * 100 / hr + (simg.Width * si.Zoom) / 2 / hr,
														   si.Y * 100 / vr + (simg.Height * si.Zoom * drez) / 2 / vr));
									e.Graphics.Transform = mx;
								}
								e.Graphics.DrawImage(simg, si.X * 100f / hr, si.Y * 100f / vr, (simg.Width * si.Zoom) / hr,
													 (simg.Height * si.Zoom * drez) / vr);
								if(!si.Rotate.Equals(0))
									e.Graphics.ResetTransform();
							}
                        } 
                        if (ii != null && ii.IsAnnuled)
                            DrowAnnuledString(e);
					}
				}
				else
				{
					IntPtr hdc = e.Graphics.GetHdc();
					int realwidth = GetDeviceCaps(hdc, HORZRES);
					int realheight = GetDeviceCaps(hdc, VERTRES);
					e.Graphics.ReleaseHdc(hdc);

					if(rx < 1)
						rx = rdef;
					if(ry < 1)
						ry = rx;
					if(realwidth > 0)
						realwidth = realwidth * 100 / rx;
					else
						realwidth = e.PageBounds.Width * 100 / ry;
					if(realheight > 0)
						realheight = realheight * 100 / ry;
					else
						realheight = e.PageBounds.Height * 100 / ry;
					bool isScale = img.Width > (realwidth) * vr / 100f || img.Height > realheight * hr / 100f;
					bool isWidth = img.Width / hr / realwidth > img.Height / vr / realheight;
					if(isScale)
					{
						int swidth;
						int sheight;
						if(isWidth)
						{
							swidth = realwidth;
							sheight = (int)(swidth / (float)img.Width * hr / vr * img.Height);
						}
						else
						{
							sheight = realheight;
							swidth = (int)(sheight / (float)img.Height * vr / hr * img.Width);
						}
						e.Graphics.DrawImage(img, 0, 0, swidth, sheight);
						float scale = realwidth * vr / 100f / img.Width;
						if(!isWidth)
							scale = realheight * vr / 100f / img.Height;
						if(images[printingPage - startPage] is PrintImageInfo)
						{
							var ii =
								images[printingPage - startPage] as PrintImageInfo;
							if(ii != null && ii.StampsItems != null && ii.StampsItems.Length > 0)
							{
								float drez = img.VerticalResolution / img.HorizontalResolution;

								foreach(StampItem si in ii.StampsItems)
								{
									if(si.TypeID == 101)
										continue;
									Image simg = Environment.GetStamp(si.StampID, si.ImageID);
									if(!si.Rotate.Equals(0))
									{
										Matrix mx = new Matrix();
										mx.RotateAt(si.Rotate,
													new PointF(si.X * 100 * scale / hr + (simg.Width * si.Zoom * scale) / 2 / hr,
															   si.Y * 100 * scale / vr + (simg.Height * si.Zoom * scale * drez) / 2 / vr));
										e.Graphics.Transform = mx;
									}
									e.Graphics.DrawImage(simg, si.X * 100 * scale / hr, si.Y * 100 * scale / vr,
														 (simg.Width * scale * si.Zoom) / hr,
														 (simg.Height * scale * si.Zoom * drez) / vr);
									if(!si.Rotate.Equals(0))
										e.Graphics.ResetTransform();
								}
                            }
                            if (ii != null && ii.IsAnnuled)
                                DrowAnnuledString(e);
						}
					}
					else
					{
						e.Graphics.DrawImage(img, 0f, 0f, img.Width * 100f / hr, img.Height * 100f / vr);
						if(images[printingPage - startPage] is PrintImageInfo)
						{
							var ii = images[printingPage - startPage] as PrintImageInfo;
							if(ii != null && ii.StampsItems != null && ii.StampsItems.Length > 0)
							{
								float drez = img.VerticalResolution / img.HorizontalResolution;
								foreach(StampItem si in ii.StampsItems)
								{
									if(si.TypeID == 101)
										continue;

									Image simg = Environment.GetStamp(si.StampID, si.ImageID);
									if(!si.Rotate.Equals(0))
									{
										Matrix mx = new Matrix();
										mx.RotateAt(si.Rotate,
													new PointF(si.X * 100 / hr + (simg.Width * si.Zoom) / 2 / hr,
															   si.Y * 100 / vr + (simg.Height * si.Zoom * drez) / 2 / vr));
										e.Graphics.Transform = mx;
									}
									e.Graphics.DrawImage(simg, si.X * 100f / hr, si.Y * 100f / vr, (simg.Width * si.Zoom) / hr,
														 (simg.Height * si.Zoom * drez) / vr);
									if(!si.Rotate.Equals(0))
										e.Graphics.ResetTransform();
								}
							}
                            if (ii != null && ii.IsAnnuled)
                                DrowAnnuledString(e);
						}
					}
				}
			}
			else
			{
				float hr = rx, vr = ry;
				var pii = images[printingPage - startPage] as PrintImageInfo;
				if(pii == null)
				{
					e.HasMorePages = false;
					OnEndPrint();
					return;
				}
				if(scaleMode == 1)
				{
					pii.ResX = rx;
					pii.ResY = ry;
					Bitmap[] bml = pii.GetMatrix();
					if(bml != null)
					{
						for(int i = 0; i < bml.Length; i++)
						{
							e.Graphics.DrawImage(bml[i], 0f, (bml[0].Height * 100f / ry) * i, (bml[i].Width * 100f / rx), (bml[i].Height * 100f / ry));
							if(i > 0)
								bml[i].Dispose();
						}
						if(bml.Length > 0)
							bml[0].Dispose();
						bml = null;
					}
					if(pii.StampsItems != null && pii.StampsItems.Length > 0)
					{
						float drez = ry / rx;
						foreach(StampItem si in pii.StampsItems)
						{
							if(si.TypeID == 101)
								continue;
							Image simg = Environment.GetStamp(si.StampID, si.ImageID);
							if(!si.Rotate.Equals(0))
							{
								Matrix mx = new Matrix();
								mx.RotateAt(si.Rotate,
											new PointF(si.X * 100 / rx + (simg.Width * si.Zoom) / 2 / rx,
													   si.Y * 100 / ry + (simg.Height * si.Zoom * drez) / 2 / ry));
								e.Graphics.Transform = mx;
							}
							e.Graphics.DrawImage(simg, si.X * 100f / rx, si.Y * 100f / ry, (simg.Width * si.Zoom) / ry,
												 (simg.Height * si.Zoom * drez) / ry);
							if(!si.Rotate.Equals(0))
								e.Graphics.ResetTransform();
						}
					}
                    if (pii.IsAnnuled)
                        DrowAnnuledString(e);
				}
				else
				{
					IntPtr hdc = e.Graphics.GetHdc();
					int realwidth = GetDeviceCaps(hdc, HORZRES);
					int realheight = GetDeviceCaps(hdc, VERTRES);
					int wid = pii.GetWidth();
					int hei = pii.GetHeight();
					e.Graphics.ReleaseHdc(hdc);

					if(rx < 1)
						rx = rdef;
					if(ry < 1)
						ry = rx;

					if(realwidth > 0)
						realwidth = realwidth * 100 / rx;
					else
						realwidth = e.PageBounds.Width * 100 / ry;

					if(realheight > 0)
						realheight = realheight * 100 / ry;
					else
						realheight = e.PageBounds.Height * 100 / ry;

					bool isScale = wid * 100f > realwidth * 72f || hei * 100f > realheight * 72f;
					bool isWidth = wid * 1f / realwidth > hei * 1f / realheight;
					if(isScale)
					{
						int swidth;
						int sheight;
						if(isWidth)
						{
							swidth = realwidth;
							sheight = (int)(swidth * hr / vr / wid * hei);
						}
						else
						{
							sheight = realheight;
							swidth = (int)(sheight * vr / hr / hei * wid);
						}
						hr *= swidth * 72 / 100f / wid;
						vr *= sheight * 72 / 100f / hei;
						float scale = vr / ry;
						if(!isWidth)
							scale = hr / rx;
						pii.ResX = (int)hr;
						pii.ResY = (int)vr;
						Bitmap[] bml = pii.GetMatrix();
						if(bml != null)
						{
							for(int i = 0; i < bml.Length; i++)
							{
								e.Graphics.DrawImage(bml[i], 0f, (bml[0].Height * 100f / ry) * i, swidth,
													 (bml[i].Height * 100f / ry));
								if(i > 0)
									bml[i].Dispose();
							}
							if(bml.Length > 0)
								bml[0].Dispose();
							bml = null;
						}

						if(pii.StampsItems != null && pii.StampsItems.Length > 0)
						{
							float drez = ry / rx;
							foreach(StampItem si in pii.StampsItems)
							{
								if(si.TypeID == 101)
									continue;
								Image simg = Environment.GetStamp(si.StampID, si.ImageID);
								if(!si.Rotate.Equals(0))
								{
									Matrix mx = new Matrix();
									mx.RotateAt(si.Rotate,
												new PointF(si.X * scale * 100 / rx + (simg.Width * scale * si.Zoom) / 2 / rx,
														   si.Y * scale * 100 / ry + (simg.Height * scale * si.Zoom * drez) / 2 / ry));
									e.Graphics.Transform = mx;
								}
								e.Graphics.DrawImage(simg, si.X * scale * 100f / rx, si.Y * scale * 100f / ry,
													 (simg.Width * scale * si.Zoom) / ry, (simg.Height * scale * si.Zoom * drez) / ry);
								if(!si.Rotate.Equals(0))
									e.Graphics.ResetTransform();
							}
						}
                        if (pii.IsAnnuled)
                            DrowAnnuledString(e);
					}
					else
					{
						pii.ResX = rx;
						pii.ResY = ry;
						Bitmap[] bml = pii.GetMatrix();
						if(bml != null)
						{
							for(int i = 0; i < bml.Length; i++)
							{
								e.Graphics.DrawImage(bml[i], 0f, (bml[0].Height * 100f / ry) * i, realwidth,
													 (bml[i].Height * 100f / ry));
								if(i > 0)
									bml[i].Dispose();
							}
							if(bml.Length > 0)
								bml[0].Dispose();
							bml = null;
						}
						if(pii.StampsItems != null && pii.StampsItems.Length > 0)
						{
							float drez = ry / rx;

							foreach(StampItem si in pii.StampsItems)
							{
								if(si.TypeID == 101)
									continue;
								Image simg = Environment.GetStamp(si.StampID, si.ImageID);
								if(!si.Rotate.Equals(0))
								{
									Matrix mx = new Matrix();
									mx.RotateAt(si.Rotate,
												new PointF(si.X * 100 / rx + (simg.Width * si.Zoom) / 2 / rx,
														   si.Y * 100 / ry + (simg.Height * si.Zoom * drez) / 2 / ry));
									e.Graphics.Transform = mx;
								}
								e.Graphics.DrawImage(simg, si.X * 100f / rx, si.Y * 100f / ry, (simg.Width * si.Zoom) / ry,
													 (simg.Height * si.Zoom * drez) / ry);
								if(!si.Rotate.Equals(0))
									e.Graphics.ResetTransform();
							}
                        }
                        if (pii.IsAnnuled)
                            DrowAnnuledString(e);
					}
				}
			}

            if (printingPage < endPage)
            {
                SetOrientation(IsLand(images[printingPage - startPage + 1] as Tiff.PageInfo), e.PageSettings);
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
                OnEndPrint();
            }
        }

        protected override bool IsLand(Tiff.PageInfo info)
        {
            var pInfo = info as PrintImageInfo;
            return pInfo != null && pInfo.Image == null ? pInfo.GetWidth() > pInfo.GetHeight() : base.IsLand(info);
        }

        private void DrowAnnuledString(PrintPageEventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int w = (int)(System.Math.Sqrt(System.Math.Pow(e.PageBounds.Width, 2) + System.Math.Pow(e.PageBounds.Height, 2)));
            int h = (int)System.Math.Round((e.PageBounds.Height > e.PageBounds.Width ? e.PageBounds.Width : e.PageBounds.Height) / 30M);
            float sz = 18;
            Font ft = new Font("Arial", (float) sz);
            Size tl = System.Windows.Forms.TextRenderer.MeasureText("А", ft);
            while (tl.Height > h)
            {
                if (sz <= 1)
                {
                    ft = new Font("Arial", 2F);
                    break;
                }

                ft = new Font("Arial", --sz);
                tl = System.Windows.Forms.TextRenderer.MeasureText("А", ft);
            }
            while (tl.Height < h)
            {
                ft = new Font("Arial", (float) ++sz);
                tl = System.Windows.Forms.TextRenderer.MeasureText("А", ft);
            }

            sb.Append(Environment.StringResources.GetString("AnnulledImage"));
            while (System.Windows.Forms.TextRenderer.MeasureText(sb.ToString(), ft).Width <= w)
            {
                sb.Append(Environment.StringResources.GetString("AnnulledImage"));
            }

            PointF p = new PointF(ft.GetHeight(), 0);
            Matrix mx = new Matrix();
            mx.RotateAt((float)(System.Math.Atan((double)e.PageBounds.Height / (double)e.PageBounds.Width) * (180 / System.Math.PI)), p);
            e.Graphics.Transform = mx;
            e.Graphics.DrawString(sb.ToString(), ft, new SolidBrush(Color.Red), p.X, 0);
        }
    }
}