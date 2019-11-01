using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UDC;

namespace Kesco.Lib.Win.Document.Checkers
{
	public class TestPrinter
	{
		private const string PDFProfile = "Document to PDF, Color, Multipage";
		private const string TiffProfile = "Document to TIFF, Black&White, Multipage";

		private const string WripperGuid = "E81766E2-8035-4BC4-97DA-A4A4E584DBC9";
		private const string PrinterGuid = "D5970830-0EF6-43CC-886E-D3D249D6D519";

		private static IUDC objUDC = null;

		public static bool CheckPrinterExists()
		{
			if(objUDC != null)
			{
				return true;
			}
			object objPrn = null;
			try
			{
				bool v6 = true;
				Type objPrnType = Type.GetTypeFromCLSID(new Guid(WripperGuid));
				if(objPrnType == null)
				{
					objPrnType = Type.GetTypeFromCLSID(new Guid(PrinterGuid));
					v6 = false;
				}

				if(objPrnType == null)
					return false;
				else
				{
					objPrn = Activator.CreateInstance(objPrnType);
					objPrnType = null;
					if(objPrn == null)
						return false;
					else if(v6)
					{
						objUDC = (IUDC)objPrn;
						objPrn = null;
					}
					else
					{
						Marshal.ReleaseComObject(objPrn);
						objPrn = null;
					}
					return true;
				}
			}
			catch(Exception ex)
			{
				return false;
			}
			finally
			{
				if(objPrn != null)
				{
					Marshal.ReleaseComObject(objPrn);
					objPrn = null;
				}
			}
		}

		public static string GetPrinterPath()
		{
			Console.WriteLine("{0}: GetPrinterPath printer com exists : {1}", DateTime.Now.ToString("HH:mm:ss fff"), objUDC != null);
			object objPrn = null;
			object objPrf = null;

			if(objUDC == null || objUDC.Version < 6)
			{
				Type objPrnType = Type.GetTypeFromCLSID(new Guid(WripperGuid));
				if(objPrnType == null)
					objPrnType = Type.GetTypeFromCLSID(new Guid(PrinterGuid));

				if(objPrnType == null)
					return null;
				try
				{
					objPrn = Activator.CreateInstance(objPrnType);
					if(objPrn == null)
						return null;
					var ver = RunCom.GetProperty(objPrn, "Version");
					int v = 0;
					if(ver is float)
						v = (int)(float)ver;
					else
						v = (int)ver;
					if(v < 6)
					{
						var str = (string)RunCom.GetProperty(objPrn, "DefaultProfile");
						objPrf = RunCom.GetProperty(objPrn, "Profile", str);
						var path = (string)RunCom.GetProperty(objPrf, "PreDefinedImageFilePath");
						if(!string.IsNullOrEmpty(path))
							return path;
					}
					else
					{
						objUDC = (IUDC)objPrn;
						objPrf = RunCom.GetProperty(objPrn, "Printers", Environment.PrinterName);
						objPrn = null;
						if(objPrf == null)
							return null;
						object Profile = RunCom.GetProperty(objPrf, "Profile");
						if(Profile == null)
							return null;
						object OutputLocation = RunCom.GetProperty(Profile, "OutputLocation");
						Marshal.ReleaseComObject(Profile);
						Profile = null;
						if(OutputLocation == null)
							return null;
						var s = RunCom.GetProperty(OutputLocation, "FolderPath");

						string path = s as string;
						Marshal.ReleaseComObject(OutputLocation);
						OutputLocation = null;
						if(string.IsNullOrEmpty(path))
							return null;
						else
							path = Path.GetFullPath(path.Replace("&[TEMP]", Path.GetTempPath()).Replace("&[Documents]",
											System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)));
						return path;
					}
				}
				catch(Exception ex)
				{
                    Data.Env.WriteToLog(ex, "TestPrinter.GetPrinterPath");
				}
				finally
				{
					objPrnType = null;
					if(objPrn != null)
					{
						Marshal.ReleaseComObject(objPrn);
						objPrn = null;
					}
					if(objPrf != null)
					{
						Marshal.ReleaseComObject(objPrf);
						objPrf = null;
					}
				}
			}
			else
			{
				try
				{
					Printer pr = objUDC.Printers[Environment.PrinterName];
					string path = pr.Profile.OutputLocation.FolderPath;
					Marshal.ReleaseComObject(pr);
					pr = null;
					if(string.IsNullOrEmpty(path))
						return null;
					else
						path = Path.GetFullPath(path.Replace("&[TEMP]", Path.GetTempPath()).Replace("&[Documents]",
										System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)));
					return path;
				}
				catch(Exception ex)
				{
					Log.Logger.WriteEx(ex);
				}
			}
			return null;
		}

        /// <summary>
        /// Установить профиль принтера
        /// Этот метод может вызвать ошибку RPC_E_CANTCALLOUT_ININPUTSYNCCALL если вызвается WndProc
        /// </summary>
        /// <param name="profile"></param>
		public static void SetPrinterProfile(ProfileType profile)
		{
			Console.WriteLine("{0}: SetPrinterProfile printer com exists : {1}", DateTime.Now.ToString("HH:mm:ss fff"), objUDC != null);
			if(objUDC != null)
			{
				Printer pr = objUDC.Printers[Environment.PrinterName];
				pr.Profile.PageSetup.ResolutionX = 200;
				pr.Profile.PageSetup.ResolutionY = 200;
				pr.Profile.OutputLocation.FolderPath = "&[TEMP]\\Documents";
				pr.Profile.OutputLocation.FileName = "&[DocName(0)].&[ImageType]";
				pr.Profile.OutputLocation.Mode = LocationModeID.LM_PREDEFINED;
				pr.Profile.OutputLocation.OverwriteExistingFile = true;
				pr.Profile.PageSetup.Units = UnitID.UNIT_IN;
				pr.Profile.Advanced.ShowNotifications = false;
				pr.Profile.Advanced.ShowProgressWnd = false;
				if(pr.Profile.FileFormat.TIFF.ColorSpace == ColorSpaceID.CS_TRUECOLOR || pr.Profile.FileFormat.TIFF.ColorSpace == ColorSpaceID.CS_COLOR_256)
				{
					pr.Profile.FileFormat.TIFF.ColorSpace = ColorSpaceID.CS_TRUECOLOR;
					pr.Profile.FileFormat.TIFF.Compression = CompressionID.CMP_FLATE;
				}
				else
				{
					pr.Profile.FileFormat.TIFF.ColorSpace = ColorSpaceID.CS_BLACKWHITE;
					pr.Profile.FileFormat.TIFF.Compression = CompressionID.CMP_CCITTGR4;
				}
				if(pr.Profile.FileFormat.PDF.ColorSpace == ColorSpaceID.CS_TRUECOLOR || pr.Profile.FileFormat.PDF.ColorSpace == ColorSpaceID.CS_COLOR_256)
				{
					pr.Profile.FileFormat.PDF.ColorSpace = ColorSpaceID.CS_TRUECOLOR;
					pr.Profile.FileFormat.PDF.Compression = CompressionID.CMP_FLATE;
				}
				else
				{
					pr.Profile.FileFormat.PDF.ColorSpace = ColorSpaceID.CS_BLACKWHITE;
					pr.Profile.FileFormat.PDF.Compression = CompressionID.CMP_CCITTGR4;
				}
				pr.Profile.FileFormat.ActualFormat = ((profile == ProfileType.PDFProfile)
														  ? FormatID.FMT_PDF
														  : FormatID.FMT_TIFF);
				pr.Profile.PostProcessing.Mode = PostProcessingModeID.PP_OPEN_CUSTOM_APP;
				pr.Profile.PostProcessing.CustomAppPath = System.Windows.Forms.Application.ExecutablePath;
				pr.Profile.PostProcessing.CustomAppParameters = "\"&[OutFile(0)]\"";
				Marshal.ReleaseComObject(pr);
				pr = null;
				return;
			}
			Type objPrnType = Type.GetTypeFromCLSID(new Guid(PrinterGuid));

			if(objPrnType == null)
				return;
			if(!objPrnType.HasElementType)
				return;
			object objPrn = null;
			object objPrf = null;
			try
			{
				objPrn = Activator.CreateInstance(objPrnType);
				if(objPrn == null)
					return;
				if(string.IsNullOrEmpty(Environment.PrinterName))
					return;
				RunCom.SetProperty(objPrn, "PrinterName", Environment.PrinterName);
				var str = (string)RunCom.GetProperty(objPrn, "DefaultProfile");
				string profStr = profile == ProfileType.TiffProfile ? TiffProfile : PDFProfile;
				if(str != profStr)
				{
					objPrf = RunCom.GetProperty(objPrn, "Profile", profStr);
					if(objPrn != null)
					{
						Marshal.ReleaseComObject(objPrn);
						objPrn = null;
					}
					if(objPrf != null)
						RunCom.SetProperty(objPrf, "Default", 0);
				}
			}
			catch(Exception ex)
			{
			}
			finally
			{
				if(objPrnType != null)
					objPrnType = null;
				if(objPrn != null)
				{
					Marshal.ReleaseComObject(objPrn);
					objPrn = null;
				}
				if(objPrf != null)
				{
					Marshal.ReleaseComObject(objPrf);
					objPrf = null;
				}
			}
			Environment.PrinterPath = null;
		}

        /// <summary>
        /// Установить профиль принтера
        /// Этот метод безопасен для вызова из WndProc
        /// 
        /// </summary>
        /// <param name="profile"></param>
	    public static void SetPrinterProfileAsync(ProfileType profile)
	    {
            var thread = new Thread(() =>
            {
                SetPrinterProfile(profile);
            }) { IsBackground = true };
            thread.Start();
            thread.Join();
	    }

	    /// <summary>
        /// Проверка профиля принтера
        /// Этот метод может вызвать ошибку RPC_E_CANTCALLOUT_ININPUTSYNCCALL если вызвается WndProc
        /// </summary>
        /// <returns></returns>
		public static bool CheckPrinterProfile()
		{
			Console.WriteLine("{0}: CheckPrinterProfile printer com exists : {1}", DateTime.Now.ToString("HH:mm:ss fff"), objUDC != null);
			if(objUDC == null)
				return true;

			try
			{
				Printer pr = objUDC.Printers[Environment.PrinterName];

				bool ret = pr.Profile.PageSetup.ResolutionX <= 204 && pr.Profile.PageSetup.ResolutionY <= 200 &&
					pr.Profile.OutputLocation.Mode == LocationModeID.LM_PREDEFINED &&
						pr.Profile.OutputLocation.FolderPath == "&[TEMP]\\Documents" &&
						pr.Profile.OutputLocation.FileName == "&[DocName(0)].&[ImageType]" &&
						pr.Profile.PostProcessing.Mode == PostProcessingModeID.PP_OPEN_CUSTOM_APP &&
						pr.Profile.PostProcessing.CustomAppPath == System.Windows.Forms.Application.ExecutablePath &&
						pr.Profile.PostProcessing.CustomAppParameters == "\"&[OutFile(0)]\"" &&
						(((pr.Profile.FileFormat.TIFF.ColorSpace == ColorSpaceID.CS_BLACKWHITE && pr.Profile.FileFormat.TIFF.Compression == CompressionID.CMP_CCITTGR4) ||
						(pr.Profile.FileFormat.TIFF.ColorSpace == ColorSpaceID.CS_TRUECOLOR && pr.Profile.FileFormat.TIFF.Compression == CompressionID.CMP_FLATE)) ||
						((pr.Profile.FileFormat.PDF.ColorSpace == ColorSpaceID.CS_BLACKWHITE && pr.Profile.FileFormat.PDF.Compression == CompressionID.CMP_CCITTGR4) ||
						(pr.Profile.FileFormat.PDF.ColorSpace == ColorSpaceID.CS_TRUECOLOR && pr.Profile.FileFormat.PDF.Compression == CompressionID.CMP_FLATE) ||
						pr.Profile.FileFormat.PDF.VectorMode)) &&
						(pr.Profile.FileFormat.ActualFormat == FormatID.FMT_PDF ||
						pr.Profile.FileFormat.ActualFormat == FormatID.FMT_TIFF);
				Marshal.ReleaseComObject(pr);
				pr = null;
				return ret;
			}
			catch(Exception ex)
			{
				Log.Logger.WriteEx(ex);
			}
			return true;
		}

        /// <summary>
        ///  Проверка профиля принтера
        ///  Этот метод безопасен для вызова из WndProc
        /// </summary>
	    public static bool CheckPrinterProfileAsync()
	    {
	        bool res = false;

	        var thread = new Thread(() =>
	        {
	            res = CheckPrinterProfile();
	        }) {IsBackground = true};
	        thread.Start();
            thread.Join();

	        return res;
	    }

        /// <summary>
        /// Версия принтера.
        ///  Этот метод может вызвать ошибку RPC_E_CANTCALLOUT_ININPUTSYNCCALL если вызвается WndProc
        /// </summary>
        /// <returns></returns>
	    public static float GetPrinterVersion()
		{
			Console.WriteLine("{0}: GetPrinterVersion printer com exists : {1}", DateTime.Now.ToString("HH:mm:ss fff"), objUDC != null);
			return objUDC != null ? objUDC.Version : 0;
		}

	    /// <summary>
	    /// Версия принтера.
        /// Этот метод безопасен для вызова из WndProc
	    /// </summary>
	    /// <returns></returns>
	    public static float GetPrinterVersionAsync()
	    {
            float res = 0;

            var thread = new Thread(() =>
            {
                res = GetPrinterVersion();
            }) { IsBackground = true };
            thread.Start();
            thread.Join();

            return res;
	    }
	}

	public enum ProfileType
	{
		TiffProfile,
		PDFProfile
	}
}