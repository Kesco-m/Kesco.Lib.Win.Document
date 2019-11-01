namespace Kesco.Lib.Win.Document.Items
{
    public class DocListItem : ListItem
    {
        public DocListItem(int id, bool rights, string text)
            : base(id, text)
        {
            Rights = rights;
        }

        public DocListItem(int id, bool rights, string[] values)
            : base(id, values)
        {
            Rights = rights;
        }

        public DocListItem(int id, bool rights, bool haveData, string[] values)
            : this(id, rights, values)
        {
            HaveData = haveData;
        }

        public bool Rights { get; set; }
        public bool HaveData { get; set; }
    }
}