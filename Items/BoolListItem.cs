namespace Kesco.Lib.Win.Document.Items
{
    public class BoolListItem : ListItem
    {
        public BoolListItem(int id, string text, bool check) : base(id, text)
        {
            Check = check;
        }

        public BoolListItem(int id, string[] values, bool check) : base(id, values)
        {
            Check = check;
        }

        public bool Check { get; set; }
    }
}