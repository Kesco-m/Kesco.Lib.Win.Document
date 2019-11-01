using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Kesco.Lib.Win.Document.Items
{
    public class ToolStripSplitButtonCheckable : ToolStripSplitButton
    {
        private bool _Checked;
        private VisualStyleRenderer renderer;
        private readonly VisualStyleElement element = VisualStyleElement.ToolBar.Button.Checked;

        public ToolStripSplitButtonCheckable()
        {
            if (Application.RenderWithVisualStyles &&
                VisualStyleRenderer.IsElementDefined(element))
            {
                renderer = new VisualStyleRenderer(element);
            }
        }

        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
#if AdvancedLogging
			try
			{
				Log.Logger.EnterMethod(this, "OnPaint(PaintEventArgs e)");
#endif
			if(_Checked)
			{
				if(renderer != null)
				{
					Rectangle cr = base.ContentRectangle;
					Image img = Image;

					// Compute the center of the item's ContentRectangle.
					var centerY = (int)((cr.Height - img.Height * Environment.Dpi / 96) / 2);

					var fullRect = new Rectangle(0, 0, Width, Height);

					var imageRect = new Rectangle(
						ContentRectangle.Left,
						ContentRectangle.Top + (int)(centerY * Environment.Dpi / 96),
						(int)(base.Image.Width * Environment.Dpi / 96),
						(int)(base.Image.Height * Environment.Dpi / 96));

					var textRect = new Rectangle(
						ContentRectangle.Left + (int)(imageRect.Width * Environment.Dpi / 96),
						ContentRectangle.Top,
						ContentRectangle.Width - (int)((imageRect.Width + 10) * Environment.Dpi / 96),
						ContentRectangle.Height);

					renderer.DrawBackground(e.Graphics, fullRect);
					renderer.DrawText(e.Graphics, textRect, Text);
					renderer.DrawImage(e.Graphics, imageRect, Image);
				}
				else
				{
					base.OnPaint(e);
					e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(40, SystemColors.Control)), 0, 0, Width,
												Height);
					e.Graphics.DrawLines(SystemPens.ControlDark,
											new[]
                                            {
                                                new Point(0, ButtonBounds.Height), new Point(0, 0),
                                                new Point(ButtonBounds.Width, 0)
                                            });
					e.Graphics.DrawLines(SystemPens.ControlLight,
											new[]
                                            {
                                                new Point(ButtonBounds.Width, 0),
                                                new Point(ButtonBounds.Width, ButtonBounds.Height),
                                                new Point(0, ButtonBounds.Height)
                                            });
				}
			}
			else
			{
				base.OnPaint(e);
			}
#if AdvancedLogging
			}
			finally
			{
				Log.Logger.LeaveMethod(this, "OnPaint(PaintEventArgs e)");
			}
#endif
        }
    }
}