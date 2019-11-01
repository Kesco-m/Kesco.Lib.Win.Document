using System;
using System.Drawing;

namespace Kesco.Lib.Win.Document.Controls.PdfViewControl
{
	internal static class Common
	{
		internal const string MSG_FILE_NOT_FOUND = "Файл {0} не найден!";

		internal const double InchSm = 2.54;
		// iTextSharp uses a default of 72 pixels per inch.
		internal const double PdfDpi = 72;

		private static float dpiX = float.MinValue;
		private static float dpiY = float.MinValue;

		internal static float DpiX
		{
			get
			{
				if(dpiX == float.MinValue)
					Init();
				return dpiX;
			}
		}

		internal static float DpiY
		{
			get
			{
				if(dpiY == float.MinValue)
					Init();
				return dpiY;
			}
		}

		internal static double ScaleX(double santimeters)
		{
			return Math.Floor((DpiX * santimeters) / InchSm);
		}

		internal static double ScaleY(double santimeters)
		{
			return Math.Floor((DpiY * santimeters) / InchSm);
		}

		private static void Init()
		{
			using(Graphics g = Graphics.FromHwnd(IntPtr.Zero))
			{
				dpiX = g.DpiX;
				dpiY = g.DpiY;
			}
		}
	}
}