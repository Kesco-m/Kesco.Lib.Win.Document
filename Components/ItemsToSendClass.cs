using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Kesco.Lib.Win.Document.Components
{
	public class ProcessingDocs
	{
		public string sourceFileName = string.Empty;
		public string sendFileName = string.Empty;
		public string docString = string.Empty;
		public int docID = -1;
		public int docImageID = -1;
		public int faxID = -1;
		public long Size = -1;
		public int formatType = 0;
		public int pagesType;
		public string pages;
		public bool? bw;
		private bool error = true;
		public bool Error { get { return error; } }

		public ProcessingDocs(int docID, object imageID)
		{
			this.docID = docID;
			int iid = -1;
			if(int.TryParse(imageID.ToString(), out iid))
				docImageID = iid;
			DataRow dr = Environment.DocData.GetDocProperties(docID, Environment.CurCultureInfo.TwoLetterISOLanguageName);
			docString = DBDocString.Format(dr, true);
			bool _fileNotFound = true;
			if(docImageID < 1)
				docImageID = dr[Environment.DocData.MainImageIDField] == DBNull.Value ? 0 : (int)dr[Environment.DocData.MainImageIDField];

			if(_fileNotFound && docImageID > 0)
			{
				if(Environment.GetServers().Count > 0)
				{
					List<int> serverIDs = Environment.DocImageData.GetLocalDocImageServers(docImageID, Environment.GetLocalServersString());

					if(serverIDs != null && serverIDs.Count > 0)
					{
						FileInfo fi;
						for(int i = 0; _fileNotFound && i < serverIDs.Count; ++i)
						{
							sourceFileName = Environment.GetLocalServer(serverIDs[i]).Path + "\\" + Environment.GetFileNameFromID(docImageID) + ".tif";
							fi = new FileInfo(sourceFileName);
							if(fi.Exists)
							{
								_fileNotFound = false;
								Size = fi.Length;
							}
							else
							{
								sourceFileName = Environment.GetLocalServer(serverIDs[i]).Path + "\\" + Environment.GetFileNameFromID(docImageID) + ".pdf";
								fi = new FileInfo(sourceFileName);
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

		public ProcessingDocs(string docString, string fileName)
		{
			sourceFileName = fileName;
			this.docString = docString;
			error = string.IsNullOrEmpty(sourceFileName);
			if(!error)
			{
				FileInfo fi = new FileInfo(sourceFileName);
				error = !fi.Exists;
				if(!error)
					Size = fi.Length;
			}
		}

		public ProcessingDocs(string docString, string fileName, object faxID)
		{
			int fid = -1;
			if(int.TryParse(faxID.ToString(), out fid))
				this.faxID = fid;
			sourceFileName = fileName;
			this.docString = docString;
			error = string.IsNullOrEmpty(sourceFileName);
			if(!error)
			{
				FileInfo fi = new FileInfo(sourceFileName);
				error = !fi.Exists;
				if(!error)
					Size = fi.Length;
			}
		}

		public ProcessingDocs(int docID, int imgID, int faxID): this(docID, imgID)
		{
			this.faxID = faxID;
		}
	}
}
