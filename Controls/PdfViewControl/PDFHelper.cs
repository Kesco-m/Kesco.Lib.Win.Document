using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using Kesco.Lib.Win.Error;
using Kesco.Lib.Win.MuPDFLib;

namespace Kesco.Lib.Win.Document.Classes
{
    public class PDFHelper
    {
        private bool useLock;
		private MuPDF mp;
        private string _fileName;
        private string _pass;
        private int page;

        public bool Open(string filename, string password)
        {

            if (!string.IsNullOrEmpty(_fileName) && useLock && _fileName.Equals(filename))
            {
                if (mp == null)
                {
                    _pass = password;
					mp = new MuPDF(_fileName, _pass, !useLock);
                }
                return mp != null;
            }
            _fileName = filename;
            _pass = password;
            if (!string.IsNullOrEmpty(_fileName))
                mp = new MuPDF(_fileName, _pass, !useLock);
            bool ret = mp != null;
            if (!useLock && mp != null)
            {
                mp.Dispose();
                mp = null;
            }
            return ret;
        }

        public bool UseLock
        {
            get { return useLock; }
            set { useLock = value; }
        }

        public void Close()
        {
            Clean();
        }

        public void SavePart(string srcFile, string dstFile, int startPage, int endPage)
        {
            PdfReader pdfReader = null;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                pdfReader = new PdfReader(srcFile);

                pdfReader.RemoveUnusedObjects();
                if (pdfReader.NumberOfPages == 0)
                    throw new Exception("В документе отсутствуют страницы. Операция сохранения невозможна!");

                startPage = (startPage <= 0 || startPage > pdfReader.NumberOfPages) ? 1 : startPage;
                endPage = (endPage <= 0 || endPage < startPage || endPage > pdfReader.NumberOfPages) ? pdfReader.NumberOfPages : endPage;

                pdfReader.SelectPages(startPage.ToString() + "-" + endPage.ToString());

                using (var file_stream = new FileStream(dstFile, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var stamper = new PdfStamper(pdfReader, file_stream))
                {
                    stamper.SetFullCompression();

                    for (int numberPage = startPage; numberPage <= endPage; numberPage++)
                    {
                        if (numberPage <= 0 || numberPage > pdfReader.NumberOfPages)
                            continue;
                        PdfDictionary page = pdfReader.GetPageN(numberPage);
                        var resources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
                        var xobject = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));
                        if (xobject != null)
                        {
                            foreach (PdfName pdname in xobject.Keys)
                            {
                                PdfObject obj = xobject.Get(pdname);
                                if (obj.IsIndirect())
                                {
                                    var tg = (PdfDictionary)PdfReader.GetPdfObject(obj); //resolve indirect reference
                                    var subType = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                                    if (PdfName.IMAGE.Equals(subType))
                                    {
                                        int xrefIndex = ((PRIndirectReference)obj).Number;
                                        PdfObject imgPdfObj = pdfReader.GetPdfObject(xrefIndex);
                                        var imgPdfStream = (PdfStream)imgPdfObj;
                                        var imgPRStream = (PRStream)imgPdfStream;
                                        byte[] bytes = PdfReader.GetStreamBytesRaw(imgPRStream);

                                        if (bytes != null && bytes.Length > 0)
                                        {
                                            try
                                            {
                                                var pdfImage = new iTextSharp.text.pdf.parser.PdfImageObject(imgPRStream);

                                                Image img = pdfImage.GetDrawingImage();
                                                if (img != null)
                                                {
                                                    var filter = (PdfName)pdfImage.Get(PdfName.FILTER);
                                                    if (filter != null)
                                                        continue;
                                                    System.Drawing.Imaging.ImageFormat format;
                                                    byte[] updatedImgBytes = Controls.PdfViewControl.PDFView.ConvertImageToBytes(img, 75, out format);

                                                    iTextSharp.text.Image compressedImage = iTextSharp.text.Image.GetInstance(updatedImgBytes);
                                                    if (format == System.Drawing.Imaging.ImageFormat.Png)
                                                        compressedImage.Deflated = true;

                                                    PdfReader.KillIndirect(obj);
                                                    stamper.Writer.AddDirectImageSimple(compressedImage, (PRIndirectReference)obj);
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.ToString());
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    stamper.Close();
                }
            }
            finally
            {
                if (pdfReader != null)
                    pdfReader.Close();

                Cursor.Current = Cursors.Default;
            }
        }

		public void SavePart(string srcFile, string dstFile, List<int> pages)
		{
			PdfReader pdfReader = null;

			try
			{
				Cursor.Current = Cursors.WaitCursor;

				pdfReader = new PdfReader(srcFile);

				pdfReader.RemoveUnusedObjects();
				if(pdfReader.NumberOfPages == 0)
					throw new Exception("В документе отсутствуют страницы. Операция сохранения невозможна!");



				pdfReader.SelectPages(string.Join(",", pages.Select(id => id.ToString()).ToArray()));

				using(var file_stream = new FileStream(dstFile, FileMode.Create, FileAccess.Write, FileShare.None))
				using(var stamper = new PdfStamper(pdfReader, file_stream))
				{
					stamper.SetFullCompression();

					int numberPage = 0;
					for(int numb = 0; numb <= pages.Count; numb++)
					{
						numb = pages[numb];
						if(numberPage <= 0 || numberPage > pdfReader.NumberOfPages)
							continue;
						PdfDictionary page = pdfReader.GetPageN(numberPage);
						var resources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
						var xobject = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));
						if(xobject != null)
						{
							foreach(PdfName pdname in xobject.Keys)
							{
								PdfObject obj = xobject.Get(pdname);
								if(obj.IsIndirect())
								{
									var tg = (PdfDictionary)PdfReader.GetPdfObject(obj); //resolve indirect reference
									var subType = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
									if(PdfName.IMAGE.Equals(subType))
									{
										int xrefIndex = ((PRIndirectReference)obj).Number;
										PdfObject imgPdfObj = pdfReader.GetPdfObject(xrefIndex);
										var imgPdfStream = (PdfStream)imgPdfObj;
										var imgPRStream = (PRStream)imgPdfStream;
										byte[] bytes = PdfReader.GetStreamBytesRaw(imgPRStream);

										if(bytes != null && bytes.Length > 0)
										{
											try
											{
												var pdfImage = new iTextSharp.text.pdf.parser.PdfImageObject(imgPRStream);

												Image img = pdfImage.GetDrawingImage();
												if(img != null)
												{
													var filter = (PdfName)pdfImage.Get(PdfName.FILTER);
													if(filter != null)
														continue;
													System.Drawing.Imaging.ImageFormat format;
													byte[] updatedImgBytes = Controls.PdfViewControl.PDFView.ConvertImageToBytes(img, 75, out format);

													iTextSharp.text.Image compressedImage = iTextSharp.text.Image.GetInstance(updatedImgBytes);
													if(format == System.Drawing.Imaging.ImageFormat.Png)
														compressedImage.Deflated = true;

													PdfReader.KillIndirect(obj);
													stamper.Writer.AddDirectImageSimple(compressedImage, (PRIndirectReference)obj);
												}
											}
											catch(Exception e)
											{
												Console.WriteLine(e.ToString());
												continue;
											}
										}
									}
								}
							}
						}
					}
					stamper.Close();
				}
			}
			finally
			{
				if(pdfReader != null)
					pdfReader.Close();

				Cursor.Current = Cursors.Default;
			}
		}


        public void DelPart(string srcFile, int page_from, int page_to)
        {
            DelPart(srcFile, srcFile, page_from, page_to, true);
        }

        public void DelPart(string srcFile, string dstFile, int page_from, int page_to)
        {
            DelPart(srcFile, dstFile, page_from, page_to, false);
        }

        private void DelPart(string srcFile, string dstFile, int page_from, int page_to, bool savePermission)
        {
            PdfReader basePDFReader = null;

            string tmpF = (srcFile == dstFile) ? Path.GetTempFileName() : dstFile;

            FileAttributes fa = FileAttributes.Normal;
            try
            {
                if (srcFile == dstFile)
                {
                    if (((int) File.GetAttributes(srcFile) & (int) FileAttributes.ReadOnly) ==
                        (int) FileAttributes.ReadOnly)
                    {
                        try
                        {
                            fa = File.GetAttributes(srcFile);
                            File.SetAttributes(srcFile, FileAttributes.Normal);
                        }
                        catch (Exception ex)
                        {
                            ErrorShower.OnShowError(null, ex.Message,
                                                    Environment.StringResources.GetString("DocControl_Page_Error1"));
                            return;
                        }

                        FileStream fs = null;
                        try
                        {
                            fs = File.Open(dstFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        }
                        catch (IOException)
                        {
                            ErrorShower.OnShowError(null,
                                                    string.Format(
                                                        Environment.StringResources.GetString("DocControl_PDF_FileBusy"),
                                                        dstFile),
                                                    Environment.StringResources.GetString("DocControl_Page_Error1"));
                            return;
                        }
                        catch (UnauthorizedAccessException ue)
                        {
                            ErrorShower.OnShowError(null, ue.Message,
                                                    Environment.StringResources.GetString("DocControl_Page_Error1"));
                            return;
                        }
                        finally
                        {
                            if (fs != null) fs.Close();
                        }
                    }
                }

                using (
                    var fio = new FileStream(srcFile, FileMode.Open, FileAccess.Read,
                                             FileShare.ReadWrite | FileShare.Delete))
                {
                    string password = String.Empty;
                    try
                    {
                        fio.Position = 0;
                        basePDFReader = new PdfReader(fio, Encoding.UTF8.GetBytes(password));

                        if (!basePDFReader.IsOpenedWithFullPermissions)
                            throw new BadPasswordException("");
                    }
                    catch (BadPasswordException)
                    {
                        while (basePDFReader == null || !basePDFReader.IsOpenedWithFullPermissions)
                        {
                            if (basePDFReader != null)
                                basePDFReader.Close();

                            password = String.Empty;
                            if (
								Controls.PdfViewControl.InputBox.Show(Environment.StringResources.GetString("DocControl_PDF_Encrypted"),
                                              Environment.StringResources.GetString("DocControl_PDF_EnterPass"),
                                              ref password) == DialogResult.Cancel)
                                return;

                            try
                            {
                                fio.Position = 0;
                                basePDFReader = new PdfReader(fio, Encoding.UTF8.GetBytes(password));
                            }
                            catch (BadPasswordException)
                            {
                            }
                        }
                    }

                    if (basePDFReader.NumberOfPages == 0)
                        throw new Exception("В документе отсутствуют страницы. Операция сохранения невозможна!");

                    page_from = (page_from <= 0 || page_from > basePDFReader.NumberOfPages) ? 1 : page_from;
                    page_to = (page_to <= 0 || page_to > basePDFReader.NumberOfPages)
                                  ? basePDFReader.NumberOfPages
                                  : page_to;

                    if ((page_to - page_from + 1) == basePDFReader.NumberOfPages)
                        return;

                    int i = page_from - 1;
                    int j = 1;

                    var pages = new int[basePDFReader.NumberOfPages];

                    pages = pages.Select(x => x = j++).ToArray();
                    pages = pages.Where(x => x < page_from || x > page_to).ToArray();

                    using (var file_stream = new FileStream(tmpF, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var document = new iTextSharp.text.Document())
                    using (var pdfWriter = new PdfCopy(document, file_stream))
                    {
                        pdfWriter.CompressionLevel = PdfStream.BEST_COMPRESSION;
                        pdfWriter.SetFullCompression();
                        pdfWriter.RotateContents = true;

                        if (savePermission && password != String.Empty)
                        {
                            pdfWriter.SetEncryption(null, Encoding.UTF8.GetBytes(password), basePDFReader.Permissions,
                                                    PdfWriter.STANDARD_ENCRYPTION_128);
                        }

                        document.Open();

                        foreach (int numberPage in pages)
                        {
                            PdfImportedPage pp = pdfWriter.GetImportedPage(basePDFReader, numberPage);
                            pdfWriter.AddPage(pp);
                        }
                        document.Close();
                    }
                }
                if (srcFile == dstFile)
                    File.Copy(tmpF, dstFile, true);
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
            finally
            {
                if (basePDFReader != null)
                    basePDFReader.Close();

                if (srcFile == dstFile)
                {
                    Slave.DeleteFile(tmpF);

                    if (fa != File.GetAttributes(dstFile))
                        try
                        {
                            File.SetAttributes(dstFile, fa);
                        }
                        catch (Exception ex)
                        {
                            Data.Env.WriteToLog(ex);
                        }
                }
            }
        }

        public string Version
        {
            get
            {
                if (checkMP())
                    return "1.4";
                return null;
            }
        }

        internal Dictionary<int, Controls.PdfViewControl.PageInfo> Refresh(string path, ref string pass, ref string uPass)
        {
            if (!File.Exists(path))
				throw new Exception(String.Format(Controls.PdfViewControl.Common.MSG_FILE_NOT_FOUND, path));

			Dictionary<int, Controls.PdfViewControl.PageInfo> ret = new Dictionary<int, Controls.PdfViewControl.PageInfo>();
            PdfReader pdfReader = null;

            try
            {
                try
                {
                    pdfReader = new PdfReader(path, Encoding.UTF8.GetBytes(pass));
                }
                catch (BadPasswordException)
                {
                    string fp = pass;
                    while (true)
                    {
                        if (pdfReader != null)
                            pdfReader.Close();

						if(Controls.PdfViewControl.InputBox.Show(Environment.StringResources.GetString("DocControl_PDF_Encrypted"),
                                Environment.StringResources.GetString("DocControl_PDF_EnterPass"), ref pass) == DialogResult.Cancel)
                        {
                            pass = fp;
                            uPass = "";
                            return ret;
                        }
                        try
                        {
                            pdfReader = new PdfReader(path, Encoding.UTF8.GetBytes(pass));
                            break;
                        }
                        catch (BadPasswordException) { }
                        catch (Exception e) { pass = fp; throw e; }
                    }
                }

                byte[] ub = pdfReader.ComputeUserPassword();
                uPass = ub != null ? Encoding.UTF8.GetString(ub) : pass;

				Controls.PdfViewControl.PageInfo[] piL = new Controls.PdfViewControl.PageInfo[pdfReader.NumberOfPages];
                int i = 1;
				ret = piL.Select(x => x = new Controls.PdfViewControl.PageInfo(pdfReader.GetPageSize(i), pdfReader.GetPageSizeWithRotation(i), pdfReader.GetPageRotation(i) / 90)).ToDictionary(x => i++);
            }
            catch (iTextSharp.text.exceptions.InvalidPdfException)
            {
				Error.ErrorShower.OnShowError(null, string.Format(Environment.StringResources.GetString("DocControl_BadPDF"), Path.GetFileName(path)/*.Substring(0, Path.GetFileName(path).Length - (Path.GetExtension(path).Length))*/), Environment.StringResources.GetString("Error"));
            }
            finally
            {
                try
                {
                    if (pdfReader != null)
                        pdfReader.Close();
                }
                catch
                {
                }

            }

            return ret;
        }


        internal List<Tiff.PageInfo> GetBitmapsCollectionFromFile(string fileName, bool color)
        {
            return GetBitmapsCollectionFromFile(fileName, 1, 0, color);
        }

        internal List<Tiff.PageInfo> GetBitmapsCollectionFromFile(string fileName, int startPage, int endPage,
                                                                  bool color)
        {
            return GetBitmapsCollectionFromFile(fileName, startPage, endPage, 96, 96, color);
        }

        internal List<Tiff.PageInfo> GetBitmapsCollectionFromFile(string fileName, int startPage, int endPage, int dpiX,
                                                                  int dpiY, bool color)
        {
            var imagesToSave = new List<Tiff.PageInfo>();

            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return imagesToSave;

            string uP = "", oP = "";

			Dictionary<int, Controls.PdfViewControl.PageInfo> pInfo = Refresh(fileName, ref oP, ref uP);
            if (pInfo == null || pInfo.Count == 0)
                return imagesToSave;

            try
            {
                using (MuPDF mp = new MuPDF(fileName, oP, true))
                {
                    if (mp == null)
                        return imagesToSave;

                    int stP = (startPage <= 0 ? 1 : startPage),
                        endP = (endPage <= 0 || endPage > mp.PageCount ? mp.PageCount : endPage);
                    pInfo = pInfo.Where(x => x.Key >= stP && x.Key <= endP).ToDictionary(x => x.Key, x => x.Value);

                    foreach (var p in pInfo)
                    {
                        mp.Page = p.Key;
                        Bitmap bm = mp.GetBitmap((int) Math.Round(mp.Width/72.0*dpiX),
                                                 (int) Math.Round(mp.Height/72.0*dpiY), dpiX, dpiY, 0,
                                                 color ? RenderType.RGB : RenderType.Monochrome, true, false, 0);

                        imagesToSave.Add(new Tiff.PageInfo {Image = bm});
                    }
                }
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }

            return imagesToSave;
        }

		internal List<Tiff.PageInfo> GetBitmapsCollectionFromFile(string fileName, List<int> pages, int dpiX, int dpiY, bool color)
		{
			var imagesToSave = new List<Tiff.PageInfo>();

			if(string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
				return imagesToSave;

			string uP = "", oP = "";

			Dictionary<int, Controls.PdfViewControl.PageInfo> pInfo = Refresh(fileName, ref oP, ref uP);
			if(pInfo == null || pInfo.Count == 0)
				return imagesToSave;

			try
			{
				using(MuPDF mp = new MuPDF(fileName, oP, true))
				{
					if(mp == null)
						return imagesToSave;
					pInfo = pInfo.Where(x => pages.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

					foreach(var p in pInfo)
					{
						mp.Page = p.Key;
						Bitmap bm = mp.GetBitmap((int)Math.Round(mp.Width / 72.0 * dpiX), (int)Math.Round(mp.Height / 72.0 * dpiY), dpiX, dpiY, 0,
												 color ? RenderType.RGB : RenderType.Monochrome, true, false, 0);

						imagesToSave.Add(new Tiff.PageInfo { Image = bm });
					}
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			return imagesToSave;
		}


        private void Clean()
        {
            if (mp != null)
            {
                mp.Dispose();
                mp = null;
            }
            page = 0;
            _fileName = null;
            _pass = null;
        }

        internal Bitmap[] GetBitmapMatrix(int width, int height, float dpix, float dpiy, int rotation,
                                          RenderType renderType, bool rotateLandscapePages, bool convertToLetter,
                                          int maxSize)
        {
            if (checkMP())
            {
                Bitmap[] bmp = mp.GetBitmapMatrix(width, height, 0, 100, dpix, dpiy, rotation, renderType,
                                                  rotateLandscapePages, convertToLetter, maxSize);
                if (!useLock)
                {
                    mp.Dispose();
                    mp = null;
                }
                return bmp;
            }

            return null;
        }

        internal Bitmap GetBitmap(int width, int height, float dpix, float dpiy, int rotation, RenderType renderType, bool rotateLandscapePages, bool convertToLetter, int maxSize)
        {
            if (checkMP())
            {
                Bitmap bmp = mp.GetBitmap(width, height, dpix, dpiy, rotation, renderType, rotateLandscapePages, convertToLetter, maxSize);
                if (!useLock)
                {
                    mp.Dispose();
                    mp = null;
                }
                return bmp;
            }
            return null;
        }

        public int Page
        {
            get { return page; }
            set
            {
                if (checkMP())
                {
                    if (value > 0 && value <= mp.PageCount)
                    {
                        mp.Page = value;
                        page = mp.Page;
                    }
                }
                else
                    page = 0;
            }
        }

        public double Width
        {
            get { return checkMP() ? mp.Width : 0; }
        }

        public double Height
        {
            get { return checkMP() ? mp.Height : 0; }
        }

        public int PageCount
        {
            get { return checkMP() ? mp.PageCount : 0; }
        }

        private bool checkMP()
        {
            if (useLock)
                return mp != null;

            if (mp == null)
            {
                mp = new MuPDF(_fileName, _pass, !useLock);
                if (page > 0 && page <= mp.PageCount)
                    mp.Page = page;
            }
            return mp != null;
        }

        internal void Flush()
        {
            if (!useLock && mp != null)
            {
                mp.Dispose();
                mp = null;
            }
        }
    }
}
