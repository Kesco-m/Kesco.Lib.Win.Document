namespace Kesco.Lib.Win.Document
{
    public class PrinterObjectClass
    {
        public PrinterObjectClass(int typeID, string typeName, string url, int printID, short paperSize)
        {
            TypeID = typeID;
            TypeName = typeName;
            URL = url;
            PrintID = printID;
            PrintType = printID;
            PaperSize = paperSize;
        }

        public int TypeID { get; private set; }
        public int PrintType { get; set; }
        public string TypeName { get; private set; }
        public string URL { get; private set; }
        public int PrintID { get; private set; }
        public short PaperSize { get; private set; }

        public override string ToString()
        {
            return TypeName;
        }
    }
}