using System;

namespace Kesco.Lib.Win.Document
{
	/// <summary>
	/// Команды отмены и возврата действия, произведённого пользователем
	/// </summary>
	public class UndoRedoCommands
	{
		public static bool UndoAddToWork(object[] args)
		{
			return Environment.WorkDocData.RemoveDocFromWorkFolder(args, false);
		}

		public static bool RedoAddToWork(object[] args)
		{
			try
			{
				if(args.Length == 3 && args[0] is int && args[1] is int && args[2] is int)
					return Environment.WorkDocData.AddDocToWorkFolder((int)args[0], (int)args[1], (int)args[2]);

				if(args.Length == 3 && args[0] is int[] && args[1] is int && args[2] is int)
				{
					Environment.WorkDocData.AddDocToWorkFolder((int[])args[0], (int)args[1], (int)args[2]);
					return true;
				}
				if(args.Length == 3 && args[0] is int && args[1] is int[] && args[2] is int)
				{
					Environment.WorkDocData.AddDocToWorkFolder((int)args[0], (int[])args[1], (int)args[2]);
					return true;
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			return false;
		}

		public static bool RedoEditDocProp(object[] args)
		{
			if(args.Length == 13 && args[0] is int && args[1] is int && args[2] is string && args[3] is string &&
				args[4] is DateTime && args[5] is bool && args[6] is string
				&& args[7] is int && args[8] is string && args[9] is string && args[10] is DateTime && args[11] is bool &&
				args[12] is string)
			{
				var docID = (int)args[0];
				var typeID = (int)args[7];
				var docType = (string)args[8];
				var number = (string)args[9];
				var date = (DateTime)args[10];
				var protectedDoc = (bool)args[11];
				var descr = (string)args[12];
				Environment.DocData.SetDocProperties(docID, typeID, docType, number, date, protectedDoc, descr);
				return true;
			}
			return false;
		}

		public static bool EditDocProp(object[] args)
		{
			if(args.Length == 13 && args[0] is int && args[1] is int && args[2] is string && args[3] is string &&
				args[4] is DateTime && args[5] is bool && args[6] is string && args[7] is int && args[8] is string &&
				args[9] is string && args[10] is DateTime && args[11] is bool && args[12] is string)
			{
				var docID = (int)args[0];
				var typeID = (int)args[1];
				var docType = (string)args[2];
				var number = (string)args[3];
				var date = (DateTime)args[4];
				var protectedDoc = (bool)args[5];
				var descr = (string)args[6];
				Environment.DocData.SetDocProperties(docID, typeID, docType, number, date, protectedDoc, descr);
				return true;
			}
			return false;
		}

		public static bool AddPerson(object[] args)
		{
			if(args.Length == 3 && args[0] is int && args[1] is int[] && args[2] is int[])
			{
				return Environment.DocData.SetDocPersons((int)args[0], (int[])args[1]);
			}
			return false;
		}

		public static bool RemovePerson(object[] args)
		{
			if(args.Length == 3 && args[0] is int && args[1] is int[] && args[2] is int[])
			{
				return Environment.DocData.SetDocPersons((int)args[0], (int[])args[2]);
			}
			return false;
		}

		public static bool AddLink(object[] args)
		{
			if(args[0] is int && args[1] is int)
			{
				Environment.DocLinksData.DelLink((int)args[0], (int)args[1]);
				return true;
			}
			return false;
		}

		public static bool RemoveLink(object[] args)
		{
			if(args[0] is int && args[1] is int)
			{
				Environment.DocLinksData.AddDocLink((int)args[0], (int)args[1]);
				return true;
			}
			return false;
		}

        /// <summary>
        /// Undo SetDocImageArchive
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool UndoSetDocImageArchive(object[] o)
        {
            int imgId = (int)o[0];
            int docId = (int)o[3];
            int originalArchiveId = (int)o[4];

            return Environment.DocImageData.SetDocImageProperties(imgId, docId, originalArchiveId);
        }

        /// <summary>
        /// Redo SetDocImageArchive
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool RedoSetDocImageArchive(object[] o)
        {
            int imgId = (int)o[0];
            int docId = (int)o[3];
            int archiveId = (int)o[5];

            return Environment.DocImageData.SetDocImageProperties(imgId, docId, archiveId);
        }

        /// <summary>
        /// Undo Sign
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool UndoSign(object[] o)
        {
            int signId = (int)o[0];

            return Environment.DocSignatureData.Delete(signId);
        }

        /// <summary>
        /// Redo Sign
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool RedoSign(object[] o)
        {
            int? кодПодписиДокумента;
            int docId = (int)o[3];
            bool isImage = (bool)o[4];
            int? imgId = (int?)o[5];
            int curEmpId = (int)o[6];
            int employeeId = (int)o[7];
            byte singT = (byte)o[8];

            bool result;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (isImage)
                result = Environment.DocSignatureData.AddSign(docId, imgId.Value, curEmpId, employeeId, singT, out кодПодписиДокумента);
            else
                result = Environment.DocSignatureData.AddSign(docId, curEmpId, employeeId, singT, out кодПодписиДокумента);

            if (result)
                o[0] = кодПодписиДокумента;

            return result;
        }

        /// <summary>
        /// Undo SetMainImage
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool UndoSetMainImage(object[] o)
        {
            int docId = (int)o[0];
            int originalImgId = (int)o[3];

            return Environment.DocData.SetMainImage(docId, originalImgId);
        }

        /// <summary>
        /// Redo SetMainImage
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool RedoSetMainImage(object[] o)
        {
            int docId = (int)o[0];
            int imgId = (int)o[4];

            return Environment.DocData.SetMainImage(docId, imgId);
        }

		public static bool MarkDocMessagesAsNotRead(object[] args)
		{
			try
			{
				switch(args.Length)
				{
					case 3:
						return Environment.WorkDocData.MarkAsRead((int)args[0], (int)args[1], (int)args[2], false);
					case 2:
						return Environment.WorkDocData.MarkAsRead((int)args[0], (int)args[1]);
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
			return false;
		}

		public static bool MarkDocMessagesAsRead(object[] args)
		{
			try
			{
				switch(args.Length)
				{
					case 3:
						return Environment.WorkDocData.MarkAsRead((int)args[0], (int)args[1], (int)args[2], true);
					case 2:
						return Environment.WorkDocData.MarkAsRead((int)args[0], (int)args[1]);
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
			return false;
		}
	}
}