using System.Data;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Document.Blocks.Parsers
{
    /// <summary>
    ///   Базовый парсер строк.
    /// </summary>
    public class Parser
    {
        protected int candidateCount;
        protected DataRow[] candidateEmployee;

        public virtual Data.Temp.Objects.Employee[] Parse(ref string txt)
        {
            return new Data.Temp.Objects.Employee[0];
        }

        #region Accessors

        public virtual int CandidateCount
        {
            get { return candidateCount; }
        }

        #endregion

        public string[] SplitToBits(string text)
        {
            return Regex.Split(text, "[,;]");
        }

        public virtual void Load()
        {
        }

        public virtual DataRow[] CandidateEmployees
        {
            get { return candidateEmployee; }
        }
    }
}