using System;
using System.Collections;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.ListViews
{
    public class DelableListView : ListView
    {
        private ListViewColumnSorter sorter;

		public DelableListView() : this(false)
		{
		}

		public DelableListView(bool useCustomSorting) : base()
		{
			if(useCustomSorting)
			{
				sorter = new ListViewColumnSorter(true);

				ListViewItemSorter = sorter;
				ColumnClick += DelableListView_ColumnClick;
			}
		}

        public ListViewItem DeleteSelectedItem()
        {
            if (SelectedItems.Count > 0)
            {
                ListViewItem itemToDel = SelectedItems[0];
                int oldIndex = itemToDel.Index;
                itemToDel.Remove();

                if (Items.Count > 0)
                {
                    int newIndex = Math.Min(Items.Count - 1, oldIndex);
                    Items[newIndex].Selected = true;
                }

                return itemToDel;
            }

            return null;
        }

        public void SortByColumn(int idx, SortOrder order)
        {
            if (sorter == null || idx < 0 || idx > Columns.Count)
                return;

            sorter.SortColumn = idx;
            sorter.Order = order;

            Sort();
        }

        private void DelableListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == sorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                sorter.Order = sorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                sorter.SortColumn = e.Column;
                sorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            Sort();
        }
    }

    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        ///   Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;

        /// <summary>
        ///   Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;

        /// <summary>
        ///   Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        private bool SortSiblingColumn;

        /// <summary>
        ///   Class constructor. Initializes various elements
        /// </summary>
        public ListViewColumnSorter(bool sortSiblingColumn)
        {
            SortSiblingColumn = sortSiblingColumn;

            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        ///   This method is inherited from the IComparer interface. It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x"> First object to be compared </param>
        /// <param name="y"> Second object to be compared </param>
        /// <returns> The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y' </returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem) x;
            listviewY = (ListViewItem) y;

            // Compare the two items
            compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text,
                                                  listviewY.SubItems[ColumnToSort].Text);

            if (SortSiblingColumn && compareResult == 0)
            {
                int secondColumn = ColumnToSort ^ 1;
                compareResult = ObjectCompare.Compare(listviewX.SubItems[secondColumn].Text,
                                                      listviewY.SubItems[secondColumn].Text);
            }

            // Calculate correct return value based on object comparison
            switch (OrderOfSort)
            {
                case SortOrder.Ascending:
                    return compareResult;
                case SortOrder.Descending:
                    return (-compareResult);
                default:
                    return 0;
            }
        }

        /// <summary>
        ///   Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set { ColumnToSort = value; }
            get { return ColumnToSort; }
        }

        /// <summary>
        ///   Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set { OrderOfSort = value; }
            get { return OrderOfSort; }
        }
    }
}