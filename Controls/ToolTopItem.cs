namespace Kesco.Lib.Win.Document.Controls
{
    public class ToolTopItem
    {
        public ToolTopItem(string text, string toolTipText)
        {
            Text = text;
            ToolTipText = toolTipText;
        }

        public string Text { get; set; }

        public string ToolTipText { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}