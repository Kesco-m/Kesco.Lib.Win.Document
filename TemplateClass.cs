using System;

namespace Kesco.Lib.Win.Document
{
    /// <summary>
    /// Шаблон сохранения документа в Архив
    /// </summary>
    public class TemplateClass
    {
        public TemplateClass(int typeID, DateTime dateTime, int[] personIDs)
        {
            DocTypeID = typeID;
            DocDateTime = dateTime;
            DocPersonsIDs = personIDs;
        }

        static TemplateClass()
        {
            DocDateTime = DateTime.MinValue;
            DocTypeID = -1;
        }

        public static int DocTypeID { get; set; }

        public static DateTime DocDateTime { get; set; }

        public static int[] DocPersonsIDs { get; set; }

        public static void ClearAll()
        {
            DocTypeID = -1;
            DocDateTime = DateTime.MinValue;
            DocPersonsIDs = null;
        }

        public static bool Template()
        {
            return (DocTypeID > -1);
        }
    }
}