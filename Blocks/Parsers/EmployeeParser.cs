using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using Kesco.Lib.Win.Data.DALC.Corporate;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Error;

namespace Kesco.Lib.Win.Document.Blocks.Parsers
{
    /// <summary>
    ///   Парсер сотрудников (для EmployeeBlock).
    /// </summary>
    public class EmployeeParser : Parser
    {
        private EmployeeDALC empData;
        private DataTable empTable;

        public EmployeeParser(EmployeeDALC empData, bool dismiss)
        {
            this.empData = empData;
            Load(dismiss);
        }

        public override void Load()
        {
            Load(false);
        }

        public void Load(bool dismiss)
        {
            if (empData != null)
                empTable = empData.GetCompEmployees(dismiss);
        }

        public override Employee[] Parse(ref string input)
        {
            string[] bits = SplitToBits(input);
            candidateCount = 0;

            var emps = new ArrayList();
            var candidateEmps = new ArrayList();
            for (int i = 0; i < bits.Length; i++)
            {
                string txt = bits[i].Trim();
                DataRow[] drs = ParseQuery(txt);

                if ((drs != null) && (drs.Length > 0))
                {
                    candidateCount = drs.Length;
                    for (int j = 0; j < candidateCount; j++)
                    {
                        DataRow dr = drs[j];
                        candidateEmps.Add(dr);
                    }
                    if (candidateCount == 1)
                    {
                        DataRow dr = drs[0];
                        var empID = (int) dr[empData.IDField];
                        var name = (string) dr[empData.NameField];
                        var fio = (string) dr[empData.FIOField];

                        var emp = new Employee(empID, fio, name, empData);
                        emps.Add(emp);
                    }
                }
            }

            candidateEmployee = new DataRow[candidateEmps.Count];
            for (int i = 0; i < candidateEmps.Count; i++)
                candidateEmployee[i] = candidateEmps[i] as DataRow;
            candidateEmps.Clear();
            candidateEmps = null;

            var empArray = new Employee[emps.Count];
            for (int i = 0; i < emps.Count; i++)
                empArray[i] = emps[i] as Employee;
            emps.Clear();
            emps = null;

            if (empArray.Length == 1)
                input = Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
                            ? empArray[0].LongName
                            : empArray[0].LongEngName;

            return empArray;
        }

        private DataRow[] ParseQuery(string txt)
        {
            if (txt == null)
                return null;

            txt = Regex.Replace(txt, "[" + Regex.Escape(@"]\*+?|{[()^$#%'") + "]+", "", RegexOptions.IgnoreCase);
            txt = Regex.Replace(txt, "[.]+", " ", RegexOptions.IgnoreCase).Replace("  ", " ").Trim();

            if (txt.Length == 0)
                return null;

            string[] w = Regex.Split(txt, @"\s");
            var rw = w.Clone() as string[] ?? new string[w.Length];

            for (int j = 0; j < rw.Length; j++)
                rw[j] = Replacer.ReplaceRusLat(w[j]);

            int len = w.Length;
            if (len > 0 && len < 4)
            {
                string query = "";
                var variants = new ArrayList();

                string f = empData.FField;
                string i = empData.IField;
                string o = empData.OField;
                string fe = empData.FEngField;
                string ie = empData.IEngField;
                string l = empData.LoginField;

                switch (len)
                {
                    case 1:
                        variants.Add(l + " LIKE '" + w[0] + "%'");
                        variants.Add(f + " LIKE '" + rw[0] + "%'");
                        variants.Add(i + " LIKE '" + rw[0] + "%'");
                        variants.Add(fe + " LIKE '" + rw[0] + "%'");
                        variants.Add(ie + " LIKE '" + rw[0] + "%'");
                        break;

                    case 2:
                        variants.Add(f + " LIKE '" + rw[0] + "%' AND " + i + " LIKE '" + rw[1] + "%'");
                        variants.Add(i + " LIKE '" + rw[0] + "%' AND " + o + " LIKE '" + rw[1] + "%'");
                        variants.Add(i + " LIKE '" + rw[0] + "%' AND " + f + " LIKE '" + rw[1] + "%'");
                        variants.Add(fe + " LIKE '" + rw[0] + "%' AND " + ie + " LIKE '" + rw[1] + "%'");
                        variants.Add(ie + " LIKE '" + rw[0] + "%' AND " + fe + " LIKE '" + rw[1] + "%'");
                        break;

                    case 3:
                        variants.Add(f + " LIKE '" + rw[0] + "%' AND " + i + " LIKE '" + rw[1] + "%' AND " + o +
                                     " LIKE '" + rw[2] + "%'");
                        variants.Add(i + " LIKE '" + rw[0] + "%' AND " + o + " LIKE '" + rw[1] + "%' AND " + f +
                                     " LIKE '" + rw[2] + "%'");
                        break;
                }

                for (int j = 0; j < variants.Count; j++)
                {
                    if (query.Length > 0)
                        query += " OR ";
                    query += "(" + (string) variants[j] + ")";
                }

                try
                {
                    return empTable.Select(query);
                }
                catch (Exception ex)
                {
                    Data.Env.WriteToLog(ex);
                    ErrorShower.OnShowError(null, ex.Message, "");
                }
            }

            return null;
        }
    }
}