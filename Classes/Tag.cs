namespace Kesco.Lib.Win.Document.Classes
{
    public struct Tag
    {
        public int TypeID;
        public string Path;
        public short PaperSize;
        public short CopiesCount;

        public override string ToString()
        {
            return Path;
        }
    }
}
