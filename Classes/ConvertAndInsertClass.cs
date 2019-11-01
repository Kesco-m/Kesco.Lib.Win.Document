using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Kesco.Lib.Win.Tiff;
using iTextSharp.text.pdf;
using Kesco.Lib.Win.Document.Controls.PdfViewControl;
using Kesco.Lib.Win.MuPDFLib;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;

namespace Kesco.Lib.Win.Document.Classes
{
	public class ConvertAndInsertClass
	{
		public static bool AddPagesToPDF(string filename, List<Tiff.PageInfo> images, int page, int from, int to)
		{
			PdfReader pdfReader = null;

			try
			{
				string password = String.Empty;
				try
				{
					pdfReader = new PdfReader(filename);
					if(!pdfReader.IsOpenedWithFullPermissions)
						throw new BadPasswordException("");
				}
				catch(BadPasswordException)
				{
					while(!pdfReader.IsOpenedWithFullPermissions)
					{
						pdfReader.Close();

						if(
							InputBox.Show(Environment.StringResources.GetString("DocControl_PDF_Encrypted"),
										  Environment.StringResources.GetString("DocControl_PDF_EnterPass"),
										  ref password) == DialogResult.Cancel)
							return false;
						try
						{
							pdfReader = new PdfReader(filename, Encoding.ASCII.GetBytes(password));
						}
						catch(BadPasswordException)
						{
						}
						catch(Exception ex) { Data.Env.WriteToLog(ex); }
					}
				}
				catch(Exception ex) { Data.Env.WriteToLog(ex); }

				if(pdfReader.NumberOfPages == 0)
					throw new Exception("В документе отсутствуют страницы. Операция сохранения невозможна!");

				string tmpFileName = Path.GetTempFileName();
				int imagesCount = images.Count;
				using(var mem_stream = new MemoryStream())
				{
					using(var document = new iTextSharp.text.Document())
					using(PdfWriter pdfWriter = PdfWriter.GetInstance(document, mem_stream))
					{
						document.OpenDocument();
						for(int j = 0; j < imagesCount; j++)
						{
							Tiff.PageInfo info = images[j];
							bool needRotate = info.Image.Width > info.Image.Height;
							if(needRotate)
								info.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);

							Image gif = Image.GetInstance(info.Image, ImageFormat.Png);
							gif.SetAbsolutePosition(0, 0);
							gif.ScaleAbsolute((float)(info.Image.Width * (72.0 / info.Image.HorizontalResolution)),
											  (float)(info.Image.Height * (72.0 / info.Image.VerticalResolution)));
							gif.SetAbsolutePosition(1, 1);

							document.SetPageSize(new Rectangle(gif.ScaledWidth, gif.ScaledHeight));
							document.NewPage();

							pdfWriter.DirectContent.AddImage(gif);

							if(needRotate)
								pdfWriter.AddPageDictEntry(PdfName.ROTATE, new PdfNumber(270));
						}
						images.Clear();
						pdfWriter.CloseStream = false;
						document.CloseDocument();
					}
					mem_stream.Position = 0;
					var addPageReader = new PdfReader(mem_stream);

					using(var file_stream = new FileStream(tmpFileName, FileMode.Create, FileAccess.Write, FileShare.None))
					using(var document = new iTextSharp.text.Document(pdfReader.GetPageSize(1)))
					using(var pdfCopy = new PdfCopy(document, file_stream))
					{
						document.Open();
						if(-1 == page)
						{
							for(int j = 1; j <= imagesCount; j++)
							{
								PdfImportedPage pp = pdfCopy.GetImportedPage(addPageReader, j);
								pdfCopy.AddPage(pp);
							}
						}
						if(-2 == page)
							page = pdfReader.NumberOfPages - 1;
						for(int i = 1; i <= pdfReader.NumberOfPages; i++)
						{
							PdfImportedPage pp = pdfCopy.GetImportedPage(pdfReader, i);
							pdfCopy.AddPage(pp);
							if(i == page + 1)
							{
								for(int j = 1; j <= imagesCount; j++)
								{
									pp = pdfCopy.GetImportedPage(addPageReader, j);
									pdfCopy.AddPage(pp);
								}
							}
						}
						document.Close();
						if(pdfReader != null)
						{
							pdfReader.Close();
							pdfReader = null;
						}
						addPageReader.Close();
						File.Copy(tmpFileName, filename, true);
						File.Delete(tmpFileName);
						return true;
					}
				}
			}
			catch(Exception ex) { Data.Env.WriteToLog(ex); }
			finally
			{
				if(pdfReader != null)
					pdfReader.Close();
			}
			return false;
		}

		public static bool AddPagesToTiff(string filename, List<Tiff.PageInfo> images, int page, int from,
										  int to)
		{
			if(page == -2)
				Environment.LibTiff.AppendPages(filename, images);
			else
				Environment.LibTiff.SavePagesWithAdd(filename, images, page);
			return true;
		}

		public static void Convert(List<Tiff.PageInfo> il, string dstPath)
		{
			using(var file_stream = new FileStream(dstPath, FileMode.Create, FileAccess.Write, FileShare.None))
			using(var document = new iTextSharp.text.Document())
			using(PdfWriter pdfWriter = PdfWriter.GetInstance(document, file_stream))
			{
				document.SetMargins(0, 0, 0, 0);

				document.Open();

				foreach(Tiff.PageInfo info in il)
				{
					try
					{
						bool needRotate = info.Image.Width > info.Image.Height;
						if(needRotate)
							info.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);

						Image gif = Image.GetInstance(info.Image,
													  ImageFormat
														  .Png);
						gif.ScaleAbsolute((float)(info.Image.Width * (72.0 / info.Image.HorizontalResolution)),
										  (float)(info.Image.Height * (72.0 / info.Image.VerticalResolution)));
						gif.SetAbsolutePosition(1, 1);

						document.SetPageSize(new Rectangle(gif.ScaledWidth, gif.ScaledHeight));
						document.NewPage();

						pdfWriter.DirectContent.AddImage(gif);

						if(needRotate)
							pdfWriter.AddPageDictEntry(PdfName.ROTATE, new PdfNumber(270));
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
						return;
					}
				}
				document.Close();
			}
		}

		public static List<Tiff.PageInfo> GetPDFImages(string fileName, int from, int to)
		{
			var imagesToSave = new List<Tiff.PageInfo>();
			try
			{
				if(!File.Exists(fileName))
					return imagesToSave;

				using(var mp = new MuPDF(fileName, "", true))
				{
					int start = (@from > 0) ? @from : 1;
					int finish = (to > 0 && to <= mp.PageCount) ? to : mp.PageCount;
					if(start > finish)
					{
						finish = start - finish;
						start -= finish;
						finish += start;
					}
					if(finish > mp.PageCount)
						finish = mp.PageCount;
					int i = start;
					var pages = new int[finish - start + 1];
					pages = pages.Select(x => i++).ToArray();

					foreach(var p in pages)
					{
						mp.Page = p;
						Bitmap bm = mp.GetBitmap((int)Math.Round(mp.Width / 72.0 * 96.0), (int)Math.Round(mp.Height / 72.0 * 96.0),
												 96f, 96f, 0, RenderType.RGB, true, false, 0);
						imagesToSave.Add(new Tiff.PageInfo { Image = bm, Compression = new CompressionInfo() });
					}
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			return imagesToSave;
		}
	}
}