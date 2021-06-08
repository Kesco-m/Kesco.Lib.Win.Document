using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Kesco.Lib.Win.Document.Components
{
	public class ProcessingDocs
	{
		public string docString = string.Empty;
		private string fileName;
		public int docID = -1;
		public List<ImageToSend> imageToSends = null;
		private bool error = false;
		public bool Error { get { return error; } }

		public ProcessingDocs(int docID)
		{
			this.docID = docID;
			DataRow dr = Environment.DocData.GetDocProperties(docID, Environment.CurCultureInfo.TwoLetterISOLanguageName);
			docString = DBDocString.Format(dr, true);
			DataTable dt = Environment.DocImageData.GetDocImages(docID, true);
			if(imageToSends == null)
				imageToSends = new List<ImageToSend>();

			for(int i = 0; i < dt.Rows.Count; i++)
				if(!Environment.DocSignatureData.IsDocSignedAnnuled((int)dt.Rows[i]["КодИзображенияДокумента"]))
					imageToSends.Add(new ImageToSend(docID, dt.Rows[i]["КодИзображенияДокумента"]) { CanBeSended = true, Printed = !dt.Rows[i]["КодПечатнойФормы"].Equals(DBNull.Value) });
		}
		
		public ProcessingDocs(int docID, object imageID) : this(docID)
		{
			if(imageID is int && (int)imageID > 0)
				foreach(var its in imageToSends)
					its.CanBeSended = imageID.Equals(its.ImageID);
		}

		public ProcessingDocs(string docString, string fileName)
		{
			if(imageToSends == null)
				imageToSends = new List<ImageToSend>();
			imageToSends.Add(new ImageToSend(fileName) { CanBeSended = true });
			this.docString = docString;
		}

		public ProcessingDocs(string docString, string fileName, object faxID) : this(docString, fileName)
		{
			int fid = -1;
			if(int.TryParse(faxID.ToString(), out fid))
				imageToSends[0].FaxID = fid;
		}

		public ProcessingDocs(int docID, int imgID, int faxID): this(docID, imgID)
		{
			//this.faxID = faxID;
		}

		public long GetFullSize()
		{
			long size = 0;
			foreach(var its in imageToSends)
				if(!its.Error && its.CanBeSended)
					size += its.Size;
			return size;
		}
	}
}
