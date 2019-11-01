using System;

namespace Kesco.Lib.Win.Document
{
    public class ObjectToString
    {
        public static string Convert(object obj)
        {
            string result = obj.ToString();
            if (obj is DateTime)
            {
                var date = (DateTime) obj;
                if (date - date.Date == TimeSpan.Zero) // time = 0
                    result = date.ToString("dd.MM.yyyy");
            }
            return result;
        }
    }
}