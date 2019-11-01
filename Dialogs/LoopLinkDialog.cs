using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public partial class LoopLinkDialog : FreeDialog
    {
        private DataTable table;

        private int mainDocID;
        private int secDocID;
		
		private ContextMenu contextMenu;

        private MenuItem openItem;
        private MenuItem rightsItem;

        public LoopLinkDialog(int mainDocID, int secDocID, DataTable table)
        {
            InitializeComponent();

            this.table = table;
            this.mainDocID = mainDocID;
            this.secDocID = secDocID;

            // контекстное меню
            contextMenu = new ContextMenu();

            // пункты меню
            openItem = new MenuItem();
            rightsItem = new MenuItem();

            // открыть
            openItem.Text = Environment.StringResources.GetString("Show");
            openItem.Click += openItem_Click;

            // права
            rightsItem.Text = Environment.StringResources.GetString("Rights");
            rightsItem.Click += rightsItem_Click;
        }

        private void ChooseDocDialog_Load(object sender, EventArgs e)
        {
            if (table == null || table.Rows.Count == 0)
                return;

            labelMsg.Text =
                string.Format(Environment.StringResources.GetString("LoopLinkDialog.ChooseDocDialog_Load.Message1"),
                              new object[]
                                  {
                                      DBDocString.Format(mainDocID),
                                      DBDocString.Format(secDocID)
                                  });

            list.Columns.Add(Environment.StringResources.GetString("ID"), 60, HorizontalAlignment.Left);
            list.Columns.Add(Environment.StringResources.GetString("DocumentType"), 120, HorizontalAlignment.Left);
            list.Columns.Add(Environment.StringResources.GetString("DocumentNumber"), 80, HorizontalAlignment.Left);
            list.Columns.Add(Environment.StringResources.GetString("Description"), 180, HorizontalAlignment.Left);
            list.Columns.Add(Environment.StringResources.GetString("LoopLinkDialog.ChooseDocDialog_Load.Message2"), 130,
                             HorizontalAlignment.Center);

            foreach (DataRow dr in table.Rows)
            {
                var docID = (int) dr[Environment.DocData.IDField];

                var values = new string[5];

                // код документа
                values[0] = docID.ToString();

                // название документа
                values[1] = dr[Environment.DocData.NameField].ToString();

                // номер
                object obj = dr[Environment.DocData.NumberField];
                if (!obj.Equals(DBNull.Value))
                    values[2] = obj.ToString();

                // описание
                obj = dr[Environment.DocData.DescriptionField];
                if (!obj.Equals(DBNull.Value))
                    values[3] = obj.ToString();

                // доступен?
                var avail = (int) dr[Environment.DocData.RightsField];
                bool rights = (avail == 1);
                if (rights)
                    values[4] = Environment.StringResources.GetString("Available");
                else
                    values[4] = Environment.StringResources.GetString("NotAvailable");

                var item = new Items.DocListItem(docID, rights, values);
                item.ToolTipText = item.Text;
                if (!item.Rights)
                    item.ForeColor = Color.Gray;

                list.Items.Add(item);
            }
        }

        private void list_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || contextMenu == null)
                return;

            contextMenu.MenuItems.Clear();

            var item = (Items.DocListItem) list.GetItemAt(e.X, e.Y);
            item.Selected = true;

            contextMenu.MenuItems.AddRange(item.Rights
                                               ? new[] {openItem, rightsItem}
                                               : new[] {rightsItem});

            if (contextMenu.MenuItems.Count > 0)
                contextMenu.Show(this, new Point(e.X + list.Left, e.Y + list.Top));
        }

        private void openItem_Click(object sender, EventArgs e)
        {
            if (list.SelectedItems.Count != 1)
                return;

            var item = list.SelectedItems[0] as Items.DocListItem;
            if (item != null && item.Rights)
                Environment.OnNewWindow(sender, new Components.DocumentSavedEventArgs(item.ID, -1));
        }

        private void rightsItem_Click(object sender, EventArgs e)
        {
            if (list.SelectedItems.Count != 1)
                return;

            var item = list.SelectedItems[0] as Items.DocListItem;
            if (item == null)
                return;
            using (var dialog = new DocUserListDialog(item.ID))
                dialog.Show();
        }

        private void list_DoubleClick(object sender, EventArgs e)
        {
            openItem_Click(sender, e);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }
    }
}