using System;
using System.IO;
using System.Text.RegularExpressions;
using Kesco.Lib.Log;
using Kesco.Lib.Win.Data.DALC;

namespace Kesco.Lib.Win.Document
{
    #region ScanInfo

    public class ScanInfo
    {
        public ScanInfo()
        {
            Date = DateTime.MinValue;
            Descr = "";
        }

        public ScanInfo(DateTime date, string descr)
        {
            Date = date;
            Descr = descr;
        }

        #region Accessors

        public DateTime Date { get; set; }

        public string Descr { get; set; }

        #endregion
    }

	#endregion

	public class TextProcessor : LocalObject
	{
		#region Like

		public static bool MultiLike(string first, string second)
		{
			string[] secondArray = second.Split(new[] { ' ' });

			bool found = true;

			for(int i = 0; i < secondArray.Length; i++)
			{
				found = (first.ToLower().IndexOf(secondArray[i].ToLower()) != -1);
				if(!found)
					break;
			}

			return found;
		}

		#endregion

		#region Replace

		public static string ReplaceKesco(string s)
		{
			const string bad = @"^\\\\Kescom\.com\\common";
			const string good = "U:";

			var r = new Regex(bad, RegexOptions.IgnoreCase);

			Match m = r.Match(s);

			if(m.Success)
				s = r.Replace(s, good);

			return s;
		}

		#endregion

		#region Parse Scan Info

		public static ScanInfo ParseScanInfo(FileInfo f)
		{
			return ParseScanInfo(f.Name, f.CreationTimeUtc.Year);
		}

		public static ScanInfo ParseScanInfo(string name, int year)
		{
			if(Regex.IsMatch(name, @"^_\d+"))
			{
				var r = new Regex(@"^_(?<month>\d\d)(?<day>\d\d)(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)");
				Match m = r.Match(name);

				if(m.Success)
				{
					try
					{
						var fromFile = new DateTime(
							year,
							Int32.Parse(m.Groups["month"].Value),
							Int32.Parse(m.Groups["day"].Value),
							Int32.Parse(m.Groups["hour"].Value),
							Int32.Parse(m.Groups["minute"].Value),
							Int32.Parse(m.Groups["second"].Value));

						return new ScanInfo(fromFile, "");
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(new DetailedException("Wrong info in scan " + name, ex));
					}
				}
			}
			else if(name.Length == 21 && Regex.IsMatch(name, @"^\d{17}.(tif|pdf)$", RegexOptions.IgnoreCase))
			{
				var r = new Regex(@"^(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)");
				Match m = r.Match(name);

				if(m.Success)
				{
					try
					{
						var fromFile = new DateTime(
							Int32.Parse(m.Groups["year"].Value),
							Int32.Parse(m.Groups["month"].Value),
							Int32.Parse(m.Groups["day"].Value),
							Int32.Parse(m.Groups["hour"].Value),
							Int32.Parse(m.Groups["minute"].Value),
							Int32.Parse(m.Groups["second"].Value));

						return new ScanInfo(fromFile, "");
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(new DetailedException("Wrong info in scan " + name, ex));
					}
				}
			}
			else if(Regex.IsMatch(name, @"^.+_\d+\.(tif|pdf)$"))
			{
				var r = new Regex(@"^(?<descr>.+)_(?<year>\d\d\d\d)(?<month>\d\d)(?<day>\d\d)(?<hour>\d\d)(?<minute>\d\d)(?<second>\d\d)\.(tif|pdf)$");
				Match m = r.Match(name);

				if(m.Success)
				{
					try
					{
						var fromFile = new DateTime(
							Int32.Parse(m.Groups["year"].Value),
							Int32.Parse(m.Groups["month"].Value),
							Int32.Parse(m.Groups["day"].Value),
							Int32.Parse(m.Groups["hour"].Value),
							Int32.Parse(m.Groups["minute"].Value),
							Int32.Parse(m.Groups["second"].Value));

						return new ScanInfo(fromFile, m.Groups["descr"].Value);
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(new DetailedException("Wrong info in scan " + name, ex));
					}
				}
			}

			return null;
		}

		#endregion

		#region Form Scan File Name

		public static string FormScanFileName(FileInfo f, ScanInfo info)
		{
			string name = string.IsNullOrEmpty(info.Descr) ? "_" + info.Date.ToString("MMddHHmmss") : info.Descr + "_" + info.Date.ToString("yyyyMMddHHmmss");

			name += f.Extension.ToLower();

			return name;
		}

		#endregion

		#region Stuff

		public static string Stuff(string s, string stuff)
		{
			return s.Length > 0 ? s + stuff : s;
		}

		public static string StuffSpace(string s)
		{
			return Stuff(s, " ");
		}

		public static string StuffNewLine(string s)
		{
			return Stuff(s, System.Environment.NewLine);
		}

		public static string StuffComma(string s)
		{
			return Stuff(s, ", ");
		}

		#endregion
	}
}