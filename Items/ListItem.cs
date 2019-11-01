using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Items
{
    public class ListItem : ListViewItem
    {
        public ListItem(int id, string text) : base(text)
        {
            ID = id;
        }

        public ListItem(int id, string[] values) : base(values)
        {
            ID = id;
        }

        public int ID { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}