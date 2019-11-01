using System;
using System.IO;

namespace Kesco.Lib.Win.Document
{
	public class GetFileType
	{
		public enum SFIndex // индексы "особенных" папок
		{
			Faxes = -2,
			None = -1,
			Catalog,
			CatalogScan,
			FaxIn,
			FaxOut,
			Outlook,
			DiscImage
		}

		public static string OutlookSecureTempFolder = "";

		public static int GetType(string fileName)
		{
			if(!string.IsNullOrEmpty(fileName))
				if(File.Exists(fileName))
				{
					var fi = new FileInfo(fileName);
					string dirName = fi.DirectoryName;
					dirName = TextProcessor.ReplaceKesco(dirName);

					for(int i = 0; i < Environment.GetServers().Count; i++)
					{
						string scan = Environment.GetServers()[i].ScanPath;
						string fax = Environment.GetServers()[i].FaxPath;
						if(dirName.ToLower().StartsWith(Environment.GetServers()[i].Path.ToLower()))
							return 0;

						if(!string.IsNullOrEmpty(scan) && dirName.ToLower().StartsWith(scan.ToLower()))
							return 1;

						if(!string.IsNullOrEmpty(fax) && dirName.ToLower().StartsWith(fax.ToLower()))
							return 2;
					}
					return dirName.ToLower().StartsWith(OutlookSecureTempFolder) ? 4 : 5;
				}
			return -1;
		}
	}
}