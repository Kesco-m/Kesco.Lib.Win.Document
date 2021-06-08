using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Kesco.Lib.Win.Document.Components
{
	public class ImageToSend
	{
		public string FileNameToSend;
		public int DocID;
		public int ImageID = -1;
		public string FileName = string.Empty;
		public int FaxID;
		public string SourceFileName { get; private set; }
		public string SendFileName;
		public bool CanBeSended;
		public bool Printed;
		public long Size;
		public int FormatType;
		public int PagesType;
		public string Pages;
		public bool? BlackWight;
		public bool SendNotes;
		public bool BurnNotes;
		public bool BurnStamps;
		private bool isFile;
		public bool HasStamp;
		private bool error = true;

		public bool Error { get { return error; } }

		public ImageToSend(string fileName)
		{
			FileName = fileName;
			SourceFileName = fileName;
			DocID = -1;
			isFile = true;
			error = string.IsNullOrEmpty(SourceFileName);
			if(!error)
			{
				FileInfo fi = new FileInfo(SourceFileName);
				error = !fi.Exists;
				if(!error)
					Size = fi.Length;
			}
		}

		public ImageToSend (string fileName, object faxID) : this(fileName)
		{
			int fid = -1;
			if(int.TryParse(faxID.ToString(), out fid))
				this.FaxID = fid;
		}

		public ImageToSend(int docID, object imageID)
		{
			DocID = docID;
			int iid = -1;
			if(int.TryParse(imageID.ToString(), out iid))
				ImageID = iid;
			isFile = false;
			bool _fileNotFound = true;
			DataRow dr = Environment.DocData.GetDocProperties(docID, Environment.CurCultureInfo.TwoLetterISOLanguageName);
			if(ImageID < 1)
				ImageID = dr[Environment.DocData.MainImageIDField] == DBNull.Value ? 0 : (int)dr[Environment.DocData.MainImageIDField];

			if(_fileNotFound && ImageID > 0)
			{
				if(Environment.GetServers().Count > 0)
				{
					List<int> serverIDs = Environment.DocImageData.GetLocalDocImageServers(ImageID, Environment.GetLocalServersString());
					HasStamp = Environment.DocSignatureData.IsImageSigned(ImageID);

					if(serverIDs != null && serverIDs.Count > 0)
					{
						FileInfo fi;
						for(int i = 0; _fileNotFound && i < serverIDs.Count; ++i)
						{
							SourceFileName = Environment.GetLocalServer(serverIDs[i]).Path + "\\" + Environment.GetFileNameFromID(ImageID) + ".tif";
							fi = new FileInfo(SourceFileName);
							if(fi.Exists)
							{
								_fileNotFound = false;
								Size = fi.Length;
							}
							else
							{
								SourceFileName = Environment.GetLocalServer(serverIDs[i]).Path + "\\" + Environment.GetFileNameFromID(ImageID) + ".pdf";
								fi = new FileInfo(SourceFileName);
								if(fi.Exists)
								{
									_fileNotFound = false;
									Size = fi.Length;
								}
							}
						}
					}
				}
			}
			error = _fileNotFound;

		}

		public ImageToSend(int docID, object imageID, int faxID) : this(docID, imageID)
		{
			FaxID = faxID;
		}
	}
}
