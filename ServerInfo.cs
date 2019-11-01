namespace Kesco.Lib.Win.Document
{
	/// <summary>
	///   Summary description for ServerInfo.
	/// </summary>
	public class ServerInfo
	{
		public int ID { get; private set; }
		public string Name { get; private set; }
		public string Path { get; private set; }
		public string ScanPath { get; private set; }
		public string FaxPath { get; private set; }

		public ServerInfo(int id, string name, string path, string scanPath, string faxPath)
		{
			ID = id;
			Name = name;
			Path = path;
			ScanPath = scanPath;
			FaxPath = faxPath;
		}
	}
}