using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Controls;

namespace Kesco.Lib.Win.Document.Dialogs
{
	public class CheckedControlCollection : ICollection<CheckControl>
    {
        private int xOrigin;
        private int yOrigin;
        private int yOffset;

        private int panelWidth;


        public event EventHandler Checked;

        private void OnChecked()
        {
            if (Checked != null)
                Checked(this, EventArgs.Empty);
        }

        private Panel panel;
        private int number;

		private List<CheckControl> list;

        public CheckedControlCollection(TableLayoutPanel panel)
        {
            this.panel = panel;

			list = new List<CheckControl>();

            xOrigin = 8;
            yOrigin = 0;
            yOffset = 20;

            number = 0;

            panelWidth = panel.Width;
        }

        public void Add(CheckControl control)
        {
            if (!ExistAndEdit(control))
            {
                panelWidth = panel.Width;
                list.Add(control);
                control.TabIndex = number + 2;
                control.Width = panelWidth - 4 - xOrigin;
                control.Anchor = (((AnchorStyles.Left | AnchorStyles.Top |
                                    AnchorStyles.Right)));
                control.CheckedChanged += control_CheckedChanged;
                panel.Controls.Add(control);
                number ++;
            }
        }

		public bool Remove(CheckControl control)
		{
			if(!list.Contains(control))
				return false;

			list.Remove(control);
			int place = panel.Controls.IndexOf(control);
			panel.Controls.RemoveAt(place);
			for(int i = place; i < panel.Controls.Count; i++)
			{
				panel.Controls[i].Top -= yOffset;
				panel.Controls[i].TabIndex -= 1;
			}
			OnChecked();
			number--;
			return true;
		}

        public int Count
        {
            get { return list.Count; }
        }

        public CheckControl this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                    return list[index] as CheckControl;
                
                throw new Exception("RecipientList: " + Environment.StringResources.GetString("Index") + " " + index +
                                    " " +
                                    Environment.StringResources.GetString("Dialog_CheckedControlCollection_Error1") +
                                    ": " + Count + ")");
            }
        }

        public int CheckedCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Count; i++)
                    if (this[i].Checked)
                        count++;

                return count;
            }
        }

        public int YOrigin
        {
            get { return yOrigin; }
            set
            {
                if (yOrigin != value && value > 0)
                {
                    int delta = yOrigin - value;
                    yOrigin = value;
                    for (int i = 0; i < list.Count; i ++)
                        this[i].Top -= delta;
                }
            }
        }

        public int XOrigin
        {
            get { return xOrigin; }
            set
            {
                if (xOrigin != value)
                {
                    xOrigin = value;
                    for (int i = 0; i < list.Count; i ++)
                        this[i].Left = xOrigin;
                }
            }
        }

        public bool ExistAndEdit(CheckControl control)
        {
            if (!list.Contains(control))
            {
                for (int i = 0; i < list.Count; i ++)
                {
                    var con = list[i] as CheckControl;
                    if (con != null && ((control.ID > 0 && con.ID == control.ID) || (control.ID == 0 && control.Tag == con.Tag)))
                    {
                        int posY = list.IndexOf(con);
                        list.Remove(con);
                        int place = panel.Controls.IndexOf(con);
                        panel.Controls.RemoveAt(place);
                        list.Insert(posY, control);
                        control.TabIndex = con.TabIndex;
                        control.Width = con.Width;
                        control.Anchor = (((AnchorStyles.Left |
                                            AnchorStyles.Top |
                                            AnchorStyles.Right)));
                        control.CheckedChanged += control_CheckedChanged;
                        panel.Controls.Add(control);
                        return true;
                    }
                }
            }
            else
                return true;
            return false;
        }

        public bool Exist(CheckControl control)
        {
            return list.Contains(control) || (from object t in list select t as CheckControl).Any(con => con != null && ((control.ID > 0 && con.ID == control.ID) || (control.ID == 0 && control.Tag == con.Tag)));
        }

        public void Clear()
        {
            foreach (object t in list)
                panel.Controls.Remove((CheckControl) t);
            list.Clear();
            number = 0;
            panelWidth = panel.Width;
        }

        private void control_CheckedChanged(object sender, EventArgs e)
        {
            OnChecked();
        }

		public void CopyTo(CheckControl[] array, int index)
		{
			list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}


		public bool Contains(CheckControl item)
		{
			return list.Contains(item);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		IEnumerator<CheckControl> IEnumerable<CheckControl>.GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}