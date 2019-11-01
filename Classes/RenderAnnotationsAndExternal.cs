using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.ImageControl;

namespace Kesco.Lib.Win.Document.Classes
{
    /// <summary>
    ///   Отрисовка заметок и внешних данных
    /// </summary>
    public class RenderAnnotationsAndExternal : RenderAnnotations
    {
        public bool IsAnnuled;

		public RenderAnnotationsAndExternal(int widthSelectedtRect) : base(widthSelectedtRect)
        {
        }

        public SynchronizedCollection<StampItem> PageSelecedStamps;
        public SynchronizedCollection<StampItem> PageStamps;

        public override void DrawAnnotation(Graphics g, object drawData, Hashtable markGroupsVisibleList, Bitmap bitmapImg,
			InterpolationMode CurrentInterpolationMode, ref Rectangle[] SelectedRectangles, ListDictionary notesToSelectedRectangles)
        {
            if (drawData is TiffAnnotation)
            {
                base.DrawAnnotation(g, drawData, markGroupsVisibleList, bitmapImg, CurrentInterpolationMode,
                                    ref SelectedRectangles, notesToSelectedRectangles);
            }
            DrawStamps(g, ref SelectedRectangles);
        }

		private void DrawStamps(Graphics g, ref Rectangle[] SelectedRectangles)
		{
			if(PageStamps == null && !IsAnnuled)
				return;
            if (PageStamps != null)
			    for(int i = 0; i < PageStamps.Count; i++)
			    {
				    Image img = null;
 				    if(PageStamps[i].TypeID == 100)
					    img = Environment.GetStamp(PageStamps[i].StampID, PageStamps[i].ImageID);
				    else if(PageStamps[i].TypeID == 101)
					    img = Environment.GetDSP();
				    if(img != null)
				    {
					    float drez = img.VerticalResolution / img.HorizontalResolution;
					    if(!PageStamps[i].Rotate.Equals(0))
					    {
						    Matrix mx = new Matrix();
						    mx.RotateAt(PageStamps[i].Rotate,
									    new PointF(PageStamps[i].X + (img.Width * PageStamps[i].Zoom) / 200,
											       PageStamps[i].Y + (img.Height * PageStamps[i].Zoom * drez) / 200));
						    g.Transform = mx;
					    }
					    g.DrawImage(img, PageStamps[i].X, PageStamps[i].Y, (img.Width * PageStamps[i].Zoom) / 100,
								    (img.Height * PageStamps[i].Zoom * drez) / 100);
					    if(!PageStamps[i].Rotate.Equals(0))
						    g.ResetTransform();
					    if(PageSelecedStamps != null && PageSelecedStamps.Contains(PageStamps[i]))
						    DrawSelectedRectangle(g, new Rectangle(PageStamps[i].X, PageStamps[i].Y,
							    (int)((img.Width * PageStamps[i].Zoom) / 100f), (int)((img.Height * PageStamps[i].Zoom * drez) / 100f)),
											      ref SelectedRectangles);
				    }
			    }

            if(IsAnnuled)
            {
                int hMin = (int)((4.0 / 25.4) * 96);
                hMin = hMin < 1 ? 1 : hMin;
                int w = (int)(System.Math.Sqrt(System.Math.Pow(g.VisibleClipBounds.Height * (100 / g.DpiX), 2) + System.Math.Pow(g.VisibleClipBounds.Width * (100 / g.DpiX), 2)) - 20);
                int h = (int)System.Math.Round(g.VisibleClipBounds.Height > g.VisibleClipBounds.Width ? (g.VisibleClipBounds.Width/30)*(100/g.DpiX) : (g.VisibleClipBounds.Height/30)*(100/g.DpiY)); 
                
                w = w < 1 ? 1 : w;
                h = h < hMin ? hMin : h;

                float sz = 18;
                Font ft = new Font("Arial", (float)sz);
                Size tl = System.Windows.Forms.TextRenderer.MeasureText("А", ft);
                while (tl.Height > h)
                {
                    if (sz <= 1)
                    {
                        ft = new Font("Arial", 2F);
                        break;
                    }

                    ft = new Font("Arial", (float)--sz);
                    tl = System.Windows.Forms.TextRenderer.MeasureText("А", ft);
                }
                while (tl.Height < h)
                {
                    ft = new Font("Arial", (float)++sz);
                    tl = System.Windows.Forms.TextRenderer.MeasureText("А", ft);
                }

                int x = (int)System.Math.Round(tl.Width * (g.DpiX/100));
                int y = 0; 
                w -= x;

                System.Text.StringBuilder txt = new System.Text.StringBuilder();
                txt.Append(Environment.StringResources.GetString("AnnulledImage"));
                while (System.Windows.Forms.TextRenderer.MeasureText(txt.ToString(), ft).Width <= w)
                {
                    txt.Append(Environment.StringResources.GetString("AnnulledImage"));
                }

                Matrix mx = new Matrix();
                mx.RotateAt((float)(System.Math.Atan((double)g.VisibleClipBounds.Height / (double)g.VisibleClipBounds.Width) * (180 / System.Math.PI)), new PointF(x, y));
                g.Transform = mx;

                g.DrawString(txt.ToString(), ft, new SolidBrush(Color.Red), x, y);
            }
		}

        public override void Clear()
        {
            if (PageSelecedStamps != null)
            {
                PageSelecedStamps.Clear();
                PageSelecedStamps = null;
            }
            if (PageStamps != null)
            {
                PageStamps.Clear();
                PageStamps = null;
            }
            IsAnnuled = false;
        }
    }
}