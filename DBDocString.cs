using System;
using System.Data;

namespace Kesco.Lib.Win.Document
{
    /// <summary>
    ///   ������������� �������� ���������
    /// </summary>
    public class DBDocString
    {
        /// <summary>
        ///   ���������� ��������� ������������ �������� ��������� � ��������� ������� ���������
        /// </summary>
        /// <param name="docID"> ��� ��������� </param>
        public static string Format(int docID)
        {
            return Format(Environment.DocData.GetDocProperties(docID, Environment.CurCultureInfo.TwoLetterISOLanguageName));
        }

        /// <summary>
        ///   ���������� ��������� ������������ �������� ��������� � ��������� ������� ���������
        /// </summary>
        /// <param name="row"> ������ ������, ���������� �� ������� ��������� </param>
       /// <param name="noID">�� ��������� ��� � ������ ���������</param>
		public static string Format(DataRow row, bool noID = false)
		{
			string s = "";

			if(row != null && Environment.DocData != null)
			{
				// id
				object obj = row[Environment.DocData.IDField];
				if(!noID)
					s = "[" + (int)obj + "]";

				// doc type
				{
					var name = row[Environment.DocData.NameField] as string;
					var type = row[Environment.DocData.DocTypeField] as string;

					if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type) &&
						name.ToLower().Equals(type.ToLower()))
						s = TextProcessor.StuffSpace(s) + name;
					else if(!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type) &&
							 !name.ToLower().Equals(type.ToLower()))
						s = TextProcessor.StuffSpace(s) + name + " / " + type;
					else if(string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
						s = TextProcessor.StuffSpace(s) + type;
				}
				// number
				string someValue = row[Environment.DocData.NumberField] as string;
				if(!string.IsNullOrEmpty(someValue))
					s = TextProcessor.StuffSpace(s) + Environment.StringResources.GetString("Num") + someValue;

				// date
				obj = row[Environment.DocData.DateField];
				if(obj != null && !obj.Equals(DBNull.Value) && obj is DateTime)
					s = TextProcessor.StuffSpace(s) + Environment.StringResources.GetString("Of") + " " +
						((DateTime)obj).ToString("dd.MM.yyyy");

				// descr
				someValue = row[Environment.DocData.DescriptionField] as string;
				if(!string.IsNullOrEmpty(someValue))
					s = TextProcessor.StuffSpace(s) + "(" + someValue + ")";
			}
			return s;
		}
    }
}