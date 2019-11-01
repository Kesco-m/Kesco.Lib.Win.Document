namespace Kesco.Lib.Win.Document
{
    public class SaveEventArgs : ImageControl.SaveEventArgs
    {
        private Environment.ActionBefore _action = Environment.ActionBefore.None;

        public Environment.ActionBefore Action
        {
            get { return _action; }
            set { _action = value; }
        }
    }
}
