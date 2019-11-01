using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
	public partial class ColorCompressionDialog : Form
	{
		private Bitmap currentImage;
		private Bitmap currentBWImage;
		private Hashtable ht = new Hashtable();
		private string fileName;
		private string newFileName;
		private int startPage = -1;
		private int endPage = -1;
		private FileInfo fi;
		private int zoom = 100;

		private ColorMatrix BWMatrix;
		private ImageAttributes attr = new ImageAttributes();

		public ColorCompressionDialog(string filename, string newFileName) : this(filename, newFileName, -1, -1)
		{
		}

		public ColorCompressionDialog(string filename, string newFileName, int startPage, int endPage)
		{
			InitializeComponent();

			fileName = filename;
			this.newFileName = newFileName;
			this.startPage = startPage;
			this.endPage = endPage;
			if(!string.IsNullOrEmpty(fileName))
				fi = new FileInfo(fileName);

			BWMatrix = new ColorMatrix(new[]
                                           {
                                               new[] {.3f, .3f, .3f, 0, 0},
                                               new[] {.59f, .59f, .59f, 0, 0},
                                               new[] {.11f, .11f, .11f, 0, 0},
                                               new float[] {0, 0, 0, 1, 0},
                                               new float[] {0, 0, 0, 0, 1}
                                           });

			attr.SetColorMatrix(BWMatrix);

			Width = (int)(Screen.PrimaryScreen.WorkingArea.Height / 1.6);
			Height = (int)(Width * 1.4);

			Load += ColorCompressionSelector_Load;
		}

		private void ColorCompressionSelector_Load(object sender, EventArgs e)
		{
			if(fi == null || File.GetLastWriteTimeUtc(fileName) != fi.LastWriteTimeUtc)
			{
				DialogResult = DialogResult.Cancel;
				Close();
				return;
			}

			int[] pages = Environment.LibTiff.GetColorPages(fileName, startPage, endPage);
			if(pages == null)
			{
				DialogResult = DialogResult.Cancel;
				Close();
				return;
			}
			for(int i = 0; i < pages.Length; i++)
			{
				ImagiesTabControl.TabPages.Add(Environment.StringResources.GetString("Page") + " " +
													(pages[i] + 1).ToString());
				ImagiesTabControl.TabPages[ImagiesTabControl.TabPages.Count - 1].Tag = pages[i];
				ht.Add(pages[i], false);
			}
			colorCompressionBlock1.ZoomInBut.Click += ZoomInBut_Click;
			colorCompressionBlock1.ZoomOutBut.Click += ZoomOutBut_Click;
			colorCompressionBlock1.ApplyToAllBut.Click += ApplyToAllBut_Click;
			colorCompressionBlock1.BlackWhiteRadio.CheckedChanged += CheckedChanged;
		}

		private void CheckedChanged(object sender, EventArgs e)
		{
			panel2.Invalidate();
		}

		private void ApplyToAllBut_Click(object sender, EventArgs e)
		{
			int[] keys = ht.Keys.Cast<int>().ToArray();
			for(int i = 0; i < ht.Count; i++)
				ht[keys[i]] = colorCompressionBlock1.BlackWhiteRadio.Checked;
		}

		private void ZoomOutBut_Click(object sender, EventArgs e)
		{
			zoom /= 2;
			if(zoom < 2)
				zoom = 2;
			if(zoom > ImageControl.ImageControl.MaxZoom)
				zoom = ImageControl.ImageControl.MaxZoom;
			if(currentImage == null)
				return;
			panelImage.Size = new System.Drawing.Size((int)Math.Round(currentImage.Width * zoom / 100f), (int)Math.Round(currentImage.Height * zoom / 100f));
			panelImage.Invalidate();
		}

		private void ZoomInBut_Click(object sender, EventArgs e)
		{
			zoom *= 2;
			if(zoom < 2)
				zoom = 2;
			if(zoom > ImageControl.ImageControl.MaxZoom)
				zoom = ImageControl.ImageControl.MaxZoom;
			if(currentImage == null)
				return;
			panelImage.Size = new System.Drawing.Size((int)Math.Round(currentImage.Width * zoom / 100f), (int)Math.Round(currentImage.Height * zoom / 100f));
			panelImage.Invalidate();
		}

		private void SaveBut_Click(object sender, EventArgs e)
		{
			var color = new List<int>();
			var bAW = new List<int>();
			var page = (int)ImagiesTabControl.SelectedTab.Tag;
			int[] keys = ht.Keys.Cast<int>().ToArray();
			for(int i = 0; i < ht.Count; i++)
			{
				if(keys[i] == page)
				{
					if(colorCompressionBlock1.BlackWhiteRadio.Checked)
						bAW.Add(keys[i]);
					else
						color.Add(keys[i]);
					continue;
				}
				if((bool)ht[keys[i]])
					bAW.Add(keys[i]);
				else
					color.Add(keys[i]);
			}
			if(color.Count < 1)
			{
				DialogResult = DialogResult.Retry;
				Close();
				return;
			}

			Environment.LibTiff.SavePart(fileName, startPage,
										 (startPage < 0 && endPage < 0) ? -1 : endPage - startPage + 1, newFileName,
										 color);
			DialogResult = DialogResult.OK;
			Close();
		}

		private void ImagiesTabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			zoom = 100;
			if(currentImage != null)
			{
				currentImage.Dispose();
				if(currentBWImage != null)
				{
					currentBWImage.Dispose();
					currentBWImage = null;
				}
			}
			var page = (int)ImagiesTabControl.SelectedTab.Tag;
			colorCompressionBlock1.BlackWhiteRadio.Checked = (bool)ht[page];
			colorCompressionBlock1.ColorRadio.Checked = !colorCompressionBlock1.BlackWhiteRadio.Checked;
			currentImage = Environment.LibTiff.GetImageFromTiff(fileName, page).Image;
			panelImage.Size = new System.Drawing.Size((int)Math.Round(currentImage.Width * zoom / 100f), (int)Math.Round(currentImage.Height * zoom / 100f));
			panelImage.Invalidate();
		}

		private void panelImage_Paint(object sender, PaintEventArgs e)
		{
			if(currentImage == null && ImagiesTabControl.SelectedTab != null)
			{
				var page = (int)ImagiesTabControl.SelectedTab.Tag;
				currentImage = Environment.LibTiff.GetImageFromTiff(fileName, page).Image;
				panelImage.Size = new System.Drawing.Size((int)Math.Round(currentImage.Width * this.zoom / 100f), (int)Math.Round(currentImage.Height * this.zoom / 100f));
			}
			if(currentImage == null)
				return;
			float zoom = 96 * this.zoom / (currentImage.VerticalResolution * 100f);
			if(colorCompressionBlock1.BlackWhiteRadio.Checked)
			{
				if(currentBWImage == null)
					currentBWImage = Environment.LibTiff.ConvertToBitonal(currentImage, false);
				e.Graphics.DrawImage(currentBWImage, 0f, 0f, currentBWImage.Width * zoom, currentBWImage.Height * zoom);
			}
			else
				e.Graphics.DrawImage(currentImage, 0f, 0f, currentImage.Width * zoom,  currentImage.Height * zoom);
		}

		private void ImagiesTabControl_Deselecting(object sender, TabControlCancelEventArgs e)
		{
			if(e.TabPage.Tag == null)
				return;
			var page = (int)e.TabPage.Tag;
			if(ht.ContainsKey(page))
				ht[page] = colorCompressionBlock1.BlackWhiteRadio.Checked;
			else
				ht.Add(page, colorCompressionBlock1.BlackWhiteRadio.Checked);
		}
	}

	public class ColorCompressionBlock : UserControl
	{
		public RadioButton BlackWhiteRadio;
		public RadioButton ColorRadio;
		public Button ApplyToAllBut;
		public Button ZoomInBut;
		public Button ZoomOutBut;

		public int PrefferedWidth { get; private set; }

		public ColorCompressionBlock()
		{
			PrefferedWidth = 700;

			BlackWhiteRadio = new RadioButton
									   {
										   Name = "BlackWhiteRadio",
										   Text = Environment.StringResources.GetString("Dialog_ColorCompressionDialog_BlackWhiteRadio"),
										   AutoSize = true,
										   TabIndex = 0,
										   Checked = true
									   };
			ColorRadio = new RadioButton
								  {
									  Name = "ColorRadio",
									  Text = Environment.StringResources.GetString("Dialog_ColorCompressionDialog_ColorRadio"),
									  AutoSize = true,
									  TabIndex = 1
								  };
			ApplyToAllBut = new Button
									 {
										 Name = "ApplyToAllBut",
										 Text = Environment.StringResources.GetString("Dialog_ColorCompressionDialog_ApplyToAllBut"),
										 AutoSize = true,
										 TabIndex = 2
									 };

			ZoomOutBut = new Button { Name = "ZoomOut", Text = "", TabIndex = 3 };
			ZoomInBut = new Button
							{
								Name = "ZoomInBut",
								Text = "",
								TabIndex = 4,
								Anchor = ZoomOutBut.Anchor = AnchorStyles.None,
								Size = ZoomOutBut.Size = new Size(22, 22),
								Image =
									Bitmap.FromHicon(new Icon(new MemoryStream(Convert.FromBase64String(zIn))).Handle)
							};
			ZoomOutBut.Image =
				Bitmap.FromHicon(new Icon(new MemoryStream(Convert.FromBase64String(zOut))).Handle);

			var ZoomToolTip = new ToolTip
									  {
										  // Set up the delays for the ToolTip.
										  AutoPopDelay = 5000,
										  InitialDelay = 500,
										  ReshowDelay = 200,
										  // Force the ToolTip text to be displayed whether or not the form is active.
										  ShowAlways = true
									  };
			ZoomToolTip.SetToolTip(ZoomInBut,
								   Environment.StringResources.GetString("Dialog_ColorCompressionDialog_ZoomIn_Text"));
			ZoomToolTip.SetToolTip(ZoomOutBut,
								   Environment.StringResources.GetString("Dialog_ColorCompressionDialog_ZoomOut_Text"));

			Controls.AddRange(new System.Windows.Forms.Control[]
                                       {
                                           BlackWhiteRadio, ColorRadio, ApplyToAllBut, ZoomOutBut,
                                           ZoomInBut
                                       });

			Dock = DockStyle.Top;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			BlackWhiteRadio.Size = TextRenderer.MeasureText(BlackWhiteRadio.Text, BlackWhiteRadio.Font);
			//(W = 92)
			BlackWhiteRadio.Location = new Point(3, 6);

			ColorRadio.Size = TextRenderer.MeasureText(ColorRadio.Text, ColorRadio.Font); //(W = 237)
			ColorRadio.Location =
				new Point(BlackWhiteRadio.Location.X + BlackWhiteRadio.ClientSize.Width + 3, 6); // X = 101
			ColorRadio.Checked = true;

			ApplyToAllBut.Size = TextRenderer.MeasureText(ApplyToAllBut.Text, ApplyToAllBut.Font);
			ApplyToAllBut.Location = new Point(BlackWhiteRadio.Location.X,
													BlackWhiteRadio.Location.Y + BlackWhiteRadio.Height + 3);

			ZoomOutBut.Location = new Point(Width - (22 * 2 + 5), 2);
			ZoomInBut.Location = new Point(Width - 22, 2);
			ZoomInBut.Anchor = ZoomOutBut.Anchor = AnchorStyles.Top | AnchorStyles.Right;

			if(PrefferedWidth < (ColorRadio.Location.X + (int)(ColorRadio.Width * 1.5) + 22 * 2 + 5))
				PrefferedWidth = ColorRadio.Location.X + (int)(ColorRadio.Width * 1.5) + 22 * 2 + 5;
			Height = ApplyToAllBut.Bottom + 5;
		}

		#region ZoomButtons

		private const string zOut =
			"AAABAAEAEBAAAAAAGABoAwAAFgAAACgAAAAQAAAAIAAAAAEAGAAAAAAAAAMAAAAAAAAAAAAAAAAAAAAA" +
			"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACkoKB3d3eysrIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			"AAAAAAAAAAAAAACkoKBmZpkzZsxmmcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACkoKBm" +
			"ZpkzZswzmf+ZzP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACkoKBmZpkzZswzmf9mzP8AAAAA" +
			"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAwMBmZpkzZswzmf9mzP8AAAAAAAAAAAAAAAAAAADMmZlN" +
			"TU1NTU1NTU1NTU3MmZmWlpYzZswzmf9mzP8AAAAAAAAAAAAAAAAAAADMmZlNTU3wyqb/7Mz/7MzwyqZN" +
			"TU2ZZmbAwMCZzP8AAAAAAAAAAAAAAAAAAADMmZlNTU3I0NTI0NTx8fH//8z/7Mz/7MxNTU3MmZkAAAAA" +
			"AAAAAAAAAAAAAAAAAABNTU3wyqbI0NTI0NTx8fH//8z/7Mz/zJnwyqZNTU0AAAAAAAAAAAAAAAAAAAAA" +
			"AABNTU3/7MxNTU1NTU1NTU1NTU1NTU1NTU3/7MxNTU0AAAAAAAAAAAAAAAAAAAAAAABNTU3/7MxNTU1N" +
			"TU1NTU1NTU1NTU1NTU3/7MxNTU0AAAAAAAAAAAAAAAAAAAAAAABNTU3wyqb/7Mz/7Mz//5n//5n/zJn/" +
			"7MzwyqZNTU0AAAAAAAAAAAAAAAAAAAAAAADMmZlNTU3/7Mz/zJn/zJn/7Mzx8fHI0NRNTU3AwMAAAAAA" +
			"AAAAAAAAAAAAAAAAAAAAAADMmWZNTU3wyqb/7Mz/7MzwyqZNTU3MmZkAAAAAAAAAAAAAAAAAAAAAAAAA" +
			"AAAAAAAAAADMmZlNTU1NTU1NTU1NTU3AwMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//wAA//EAAP/h" +
			"AAD/wQAA/4MAAP8HAADADwAAgB8AAAA/AAAAPwAAAD8AAAA/AAAAPwAAAD8AAIB/AADA/wAA";

		private const string zIn =
			"AAABAAEAEBAAAAAAGABoAwAAFgAAACgAAAAQAAAAIAAAAAEAGAAAAAAAAAMAAAAAAAAAAAAAAAAAAAAA" +
			"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACkpKRwcHC2trYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			"AAAAAAAAAAAAAACkpKRmZpkzZsxmmcwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACkpKRm" +
			"ZpkzZswzmf+ZzP8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACkpKRmZpkzZswzmf9mzP8AAAAA" +
			"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAwMBmZpkzZswzmf9mzP8AAAAAAAAAAAAAAAAAAADMmZlN" +
			"TU1NTU1NTU1NTU3U1NSUlJQzZswzmf9mzP8AAAAAAAAAAAAAAAAAAADMmZlNTU3/zJn//8z//8z/zJlN" +
			"TU3U1NQzmf+ZzP8AAAAAAAAAAAAAAAAAAADMmZlNTU3U1NTU1NRNTU1NTU3//8z//8xNTU3U1NQAAAAA" +
			"AAAAAAAAAAAAAAAAAABNTU3/zJnU1NTU1NRNTU1NTU3//8z/zJn/zJlNTU0AAAAAAAAAAAAAAAAAAAAA" +
			"AABNTU3//8xNTU1NTU1NTU1NTU1NTU1NTU3//8xNTU0AAAAAAAAAAAAAAAAAAAAAAABNTU3//8xNTU1N" +
			"TU1NTU1NTU1NTU1NTU3//8xNTU0AAAAAAAAAAAAAAAAAAAAAAABNTU3/zJn//8z//8xNTU1NTU3/zJn/" +
			"/8z/zJlNTU0AAAAAAAAAAAAAAAAAAAAAAADMmZlNTU3//8z/zJlNTU1NTU34+PjU1NRNTU3AwMAAAAAA" +
			"AAAAAAAAAAAAAAAAAAAAAADMmWZNTU3/zJn//8z//8z/zJlNTU3MmZkAAAAAAAAAAAAAAAAAAAAAAAAA" +
			"AAAAAAAAAADMmZlNTU1NTU1NTU1NTU3AwMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD//wAA//EAAP/h" +
			"AAD/wQAA/4MAAP8HAADADwAAgB8AAAA/AAAAPwAAAD8AAAA/AAAAPwAAAD8AAIB/AADA/wAA";

		#endregion
	}
}