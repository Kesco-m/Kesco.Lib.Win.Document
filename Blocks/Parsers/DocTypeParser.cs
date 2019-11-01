using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Kesco.Lib.Win.Data.DALC.Documents;

namespace Kesco.Lib.Win.Document.Blocks.Parsers
{
	public class DocTypeParser
	{
		private DocTypeDALC data;
		private DataTable table;

		public DocTypeParser(DocTypeDALC data, DataTable table)
		{
			this.data = data;
			this.table = table;
		}

		public DataRow[] ParseQuery(string txt)
		{
			if(data == null)
				throw new Exception("DocTypeParser: " + Environment.StringResources.GetString("Select_SelectTypeDialog_SelectType_Load_Error1") +
										" data.");

			if(table == null)
				throw new Exception("DocTypeParser: " + Environment.StringResources.GetString("Select_SelectTypeDialog_SelectType_Load_Error1") +
					" table.");

			txt = Replacer.ReplaceSQLSymbols(Replacer.ReplaceRusLat(txt));

			txt = txt.Replace("-", " ");
			txt = Regex.Replace(txt, "[ ]+", " ");

			if(txt.Length == 0)
				return null;

			string[] w = Regex.Split(txt, @"\s");
			if(w.Length > 0)
			{
				string field = data.TypeDocField;
				if(Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru"))
					field = data.NameRLField;
				List<DataRow> rows =
					table.Rows.Cast<DataRow>().Where<DataRow>(x => x[field].ToString().StartsWith(w[0], true, CultureInfo.InvariantCulture)).ToList();
				for(int i = 1; i < w.Length; i++)
				{
					rows = rows.Where(x => x[field].ToString().ToLower().Contains(w[i].ToLower())).ToList();
				}
				try
				{
					return rows.ToArray();
				}
				catch(Exception ex)
				{ Data.Env.WriteToLog(ex); }
			}

			return null;
		}
	}
}