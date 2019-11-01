using System;
using System.Drawing;

namespace Kesco.Lib.Win.Document.Items
{
    public class PersonListItem : ListItem
    {
        private int position;
        private bool is_valid;
		private string url;

        public PersonListItem(int id, string text, string url, int position, bool is_valid)
            : base(id, text)
        {
            this.position = position;
            this.is_valid = is_valid;
			this.url = url;
            Name = text;
			this.SubItems.Add(new ListViewSubItem() { Text = url });
            UpdateStyle();
        }

		//public PersonListItem(int id, string[] values, int position, bool is_valid)
		//    : base(id, values)
		//{
		//    this.position = position;
		//    this.is_valid = is_valid;
		//    UpdateStyle();
		//}


        public PersonListItem(int id, string text, string url, int position) : base(id, text)
        {
            this.position = position;
            this.is_valid = true;
			this.url = url;
            Name = text;
			this.SubItems.Add(new ListViewSubItem() { Text = url });
            UpdateStyle();
        }

		//public PersonListItem(int id, string[] values, int position) : base(id, values)
		//{
		//    this.position = position;
		//    is_valid = true;
		//    UpdateStyle();
		//}

        public void UpdateStyle()
        {
            ForeColor = Removable ? Color.Black : Color.Silver;

            if (is_valid)
                ToolTipText = String.Empty;
            else
            {
                ToolTipText = Environment.StringResources.GetString("PersonIsNotValid");
                ForeColor = Color.Red;
            }
        }

        public new int Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateStyle();
            }
        }

        public bool IsValid
        {
            get { return is_valid; }
            set
            {
                is_valid = value;
                UpdateStyle();
            }
        }

        public bool Removable
        {
            get { return (position == 0); }
        }
    }
}