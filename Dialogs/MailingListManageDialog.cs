using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document.ListViews;
using Kesco.Lib.Win.Options;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public class MailingListManageDialog : FreeDialog
    {
        private List<MailingListItem> mls;
        private Button buttonCreate;
        private Panel panelBottom;
        private Button buttonEdit;
        private Button buttonDelete;
        private Button buttonClose;
        private Button buttonShare;
        private ToolTip toolTip;
        private ImageList imageList;
        private Timer timerTooltip;
        private DelableListView list;
        private CheckBox ckbSortByAuthor;
        private IContainer components;

        private bool lockList = true;
        private int minColumnNameWidth;
        private int minColumnAuthorWidth;

        private Folder subLayout;

        private ListViewItem modifiedItem;

        public MailingListManageDialog()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (MailingListManageDialog));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.buttonCreate = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonShare = new System.Windows.Forms.Button();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ckbSortByAuthor = new System.Windows.Forms.CheckBox();
            this.timerTooltip = new System.Windows.Forms.Timer(this.components);
            this.list = new DelableListView(true);
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream =
                ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.SystemColors.Control;
            this.imageList.Images.SetKeyName(0, "Gnome-Emblem-Shared.ico");
            // 
            // buttonCreate
            // 
            resources.ApplyResources(this.buttonCreate, "buttonCreate");
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonShare);
            this.panelBottom.Controls.Add(this.buttonCreate);
            this.panelBottom.Controls.Add(this.buttonEdit);
            this.panelBottom.Controls.Add(this.buttonDelete);
            this.panelBottom.Controls.Add(this.buttonClose);
            resources.ApplyResources(this.panelBottom, "panelBottom");
            this.panelBottom.Name = "panelBottom";
            // 
            // buttonShare
            // 
            resources.ApplyResources(this.buttonShare, "buttonShare");
            this.buttonShare.Name = "buttonShare";
            this.buttonShare.Click += new System.EventHandler(this.buttonShare_Click);
            // 
            // buttonEdit
            // 
            resources.ApplyResources(this.buttonEdit, "buttonEdit");
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonDelete
            // 
            resources.ApplyResources(this.buttonDelete, "buttonDelete");
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonClose
            // 
            resources.ApplyResources(this.buttonClose, "buttonClose");
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // ckbSortByAuthor
            // 
            resources.ApplyResources(this.ckbSortByAuthor, "ckbSortByAuthor");
            this.ckbSortByAuthor.Name = "ckbSortByAuthor";
            this.ckbSortByAuthor.UseVisualStyleBackColor = true;
            this.ckbSortByAuthor.CheckedChanged += new System.EventHandler(this.ckbSortByAuthor_CheckedChanged);
            // 
            // timerTooltip
            // 
            this.timerTooltip.Interval = 500;
            this.timerTooltip.Tick += new System.EventHandler(this.timerTooltip_Tick);
            // 
            // list
            // 
            resources.ApplyResources(this.list, "list");
            this.list.FullRowSelect = true;
            this.list.HideSelection = false;
            this.list.MultiSelect = false;
            this.list.Name = "list";
            this.list.SmallImageList = this.imageList;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.Details;
            this.list.ColumnWidthChanged +=
                new System.Windows.Forms.ColumnWidthChangedEventHandler(this.list_ColumnWidthChanged);
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            this.list.DoubleClick += new System.EventHandler(this.list_DoubleClick);
            this.list.MouseEnter += new System.EventHandler(this.list_MouseEnter);
            this.list.MouseMove += new System.Windows.Forms.MouseEventHandler(this.list_MouseMove);
            this.list.ColumnWidthChanging +=
                new System.Windows.Forms.ColumnWidthChangingEventHandler(this.list_ColumnWidthChanging);
            this.list.MouseLeave += new System.EventHandler(this.list_MouseLeave);
            // 
            // MailingListManageDialog
            // 
            this.AcceptButton = this.buttonEdit;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonClose;
            this.Controls.Add(this.ckbSortByAuthor);
            this.Controls.Add(this.list);
            this.Controls.Add(this.panelBottom);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MailingListManageDialog";
            this.Load += new System.EventHandler(this.MailingListManageDialog_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MailingListManageDialog_FormClosed);
            this.Resize += new System.EventHandler(this.MailingListManageDialog_Resize);
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private void MailingListManageDialog_Load(object sender, EventArgs e)
        {
            try
            {
                list.Columns.Add(Environment.StringResources.GetString("MailingListName"),
                                 (int) (list.ClientSize.Width*0.7),
                                 HorizontalAlignment.Left);
                list.Columns.Add(Environment.StringResources.GetString("Author"), (int) (list.ClientSize.Width*0.3) + 1,
                                 HorizontalAlignment.Center);

                //Обязательно делать до заполнения листа
                list.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                minColumnNameWidth = list.Columns[0].Width;

                LoadMailingLists();

                LoadParameters();
                UpdateControls();

                ckbSortByAuthor.Checked = Environment.UserSettings.SortMailingListByAuthor;
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        private void LoadMailingLists()
        {
            list.Items.Clear();

            try
            {
                mls = Environment.MailingListData.GetMailingListsEx(Environment.CurEmp.ID);
                if (mls != null && mls.Count > 0)
                    foreach (MailingListItem t in mls)
                    {
                        list.Items.Add(new ListViewItem(new[] {t.Name, t.Author})
                                           {
                                               Tag = t,
                                               ImageIndex = t.Editable && t.SharedEmploees.Count != 0 ? 0 : -1
                                           });
                    }

                if (list.Items.Count > 0)
                    list.Items[0].Selected = true;
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
                MessageForm.Show(ex.Message);
            }
        }

        private void LoadParameters()
        {
            try
            {
                list.Columns[1].AutoResize(list.Items.Count > 0
                                               ? ColumnHeaderAutoResizeStyle.ColumnContent
                                               : ColumnHeaderAutoResizeStyle.HeaderSize);
                minColumnAuthorWidth = list.Columns[1].Width;

                lockList = false;
                SetColumnSizeSafe(-1);

                subLayout = Environment.Layout.Folders.Add(Name);

                Width = subLayout.LoadIntOption("Width", Width);
                Height = subLayout.LoadIntOption("Height", Height);

                int locationX = subLayout.LoadIntOption("LocationX", Location.X);
                int locationY = subLayout.LoadIntOption("LocationY", Location.Y);

                if (locationX != Location.X || locationY != Location.Y)
                    Location = new Point(locationX, locationY);

                lockList = true;
                list.Columns[0].Width = subLayout.LoadIntOption("ColumnNameWidth", list.Columns[0].Width);
                list.Columns[1].Width = subLayout.LoadIntOption("ColumnAuthorWidth", list.Columns[1].Width);
                lockList = false;
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            bool allowModify = false;
            if (list.SelectedItems.Count > 0)
            {
                var mailing = (MailingListItem) list.SelectedItems[0].Tag;
                allowModify = mailing.Editable;
            }

            if (buttonEdit.Enabled != allowModify)
                buttonEdit.Enabled = allowModify;

            if (buttonDelete.Enabled != allowModify)
                buttonDelete.Enabled = allowModify;

            if (buttonShare.Enabled != allowModify)
                buttonShare.Enabled = allowModify;
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            ProceedDialog(new MailingListEditDialog());
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (list.SelectedItems.Count <= 0)
                return;
            modifiedItem = list.SelectedItems[0];
            if (list.SelectedItems[0].Tag is MailingListItem)
                ProceedDialog(new MailingListEditDialog(list.SelectedItems[0].Tag as MailingListItem));
        }

        private void ProceedDialog(FreeDialog dialog)
        {
            dialog.DialogEvent += MLEdit_DialogEvent;
            ShowSubForm(dialog);
            Enabled = false;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            End(DialogResult.OK);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Environment.StringResources.GetString("DeleteConfirmationMailingList"),
                                Environment.StringResources.GetString("DeleteConfirmation"),
                                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) !=
                DialogResult.Yes)
                return;

            if (list.SelectedItems.Count <= 0)
                return;

            ListViewItem item = list.DeleteSelectedItem();
            if (item != null)
            {
                var ml = item.Tag as MailingListItem;
                if (ml != null) 
                    ml.Delete();
            }

            UpdateControls();
        }

        private void MLEdit_DialogEvent(object source, DialogEventArgs e)
        {
            Enabled = true;
            Focus();

            if (e.Dialog.DialogResult == DialogResult.OK)
            {
                if (modifiedItem != null)
                {
                    var ml = modifiedItem.Tag as MailingListItem;
                    if (ml != null)
                    {
                        modifiedItem.Text = ml.Name;
                        modifiedItem.SubItems[1].Text = ml.Author;
                        modifiedItem.ImageIndex = ml.Editable && ml.SharedEmploees.Count != 0 ? 0 : -1;
                    }

                    list.Focus();
                }
                else
                {
                    LoadMailingLists();
                }
            }
        }

        private void buttonShare_Click(object sender, EventArgs e)
        {
            if (list.SelectedItems.Count > 0)
            {
                modifiedItem = list.SelectedItems[0];
                if (list.SelectedItems[0].Tag is MailingListItem)
                {
                    var dialog =
                        new MailingListShareDialog(list.SelectedItems[0].Tag as MailingListItem);
                    ProceedDialog(dialog);
                }
            }
        }

        #region Tooltip

        private int prevX = -1;
        private int prevY = -1;

        private void list_MouseEnter(object sender, EventArgs e)
        {
            timerTooltip.Start();
        }

        private void list_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X != prevX || e.Y != prevY)
            {
                HideTooltip();
                timerTooltip.Stop();
                timerTooltip.Start();

                prevX = e.X;
                prevY = e.Y;
            }
        }

        private void list_MouseLeave(object sender, EventArgs e)
        {
            timerTooltip.Stop();
            HideTooltip();
        }

        private void timerTooltip_Tick(object sender, EventArgs e)
        {
            ShowTooltip();
            timerTooltip.Stop();
        }

        private void ShowTooltip()
        {
            Point listPoint = list.PointToClient(MousePosition);
            ListViewItem item = list.GetItemAt(listPoint.X, listPoint.Y);

            if (item == null)
                return;

            var mailing = (MailingListItem) item.Tag;
            string text = string.Empty;

            if (item.GetBounds(ItemBoundsPortion.Icon).Contains(listPoint))
            {
                text = GetTextFromEmployees(mailing.SharedEmploees);

                if (text != string.Empty)
                    text = text.Insert(0, "Доступна для:\n");
            }
            else if (item.GetBounds(ItemBoundsPortion.Label).Contains(listPoint))
            {
                text = GetTextFromEmployees(mailing.Employees);

                if (text != string.Empty)
                    text = text.Insert(0, "Содержит:\n");
            }
            else
            {
                text = mailing.Author == string.Empty ? "Личная" : mailing.Author;
            }

            text = text.TrimEnd('\n');

            if (!string.IsNullOrEmpty(text))
            {
                toolTip.Tag = item;
                toolTip.Show(text, this, listPoint.X + 20, listPoint.Y + 30);
            }
        }

        private void HideTooltip()
        {
            if (toolTip.Tag != null)
            {
                toolTip.Tag = null;
                toolTip.Hide(this);
            }
        }

        private static string GetTextFromEmployees(List<Employee> employees)
        {
            string text = string.Empty;

            employees = Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
                            ? employees.OrderBy(x => x.LongName).ToList()
                            : employees.OrderBy(x => x.LongEngName).ToList();

            return employees.Aggregate(text,
                                       (current, emp) =>
                                       current +
                                       (Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru")
                                            ? emp.LongName + "\n"
                                            : emp.LongEngName + "\n"));
        }

        #endregion

        #region Doubli click

        private void list_DoubleClick(object sender, EventArgs e)
        {
            Point pt = list.PointToClient(MousePosition);
            ListViewItem item = list.GetItemAt(pt.X, pt.Y);
            if (item == null)
                return;

            if (item.GetBounds(ItemBoundsPortion.Icon).Contains(pt) && buttonShare.Enabled)
            {
                buttonShare_Click(sender, e);
                return;
            }

            if (buttonEdit.Enabled)
                buttonEdit_Click(sender, e);
        }

        #endregion

        #region Column size

        private void list_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (lockList)
                return;

            switch (e.ColumnIndex)
            {
                case 0:
                    if (e.NewWidth < minColumnNameWidth)
                    {
                        e.Cancel = true;
                    }
                    else if (e.NewWidth + list.Columns[1].Width < list.ClientSize.Width) //Появилась третья колонка
                    {
                        lockList = true;
                        list.Columns[1].Width = list.ClientSize.Width - e.NewWidth - 1;
                        lockList = false;
                    }
                    break;
                case 1:
                    if (e.NewWidth < minColumnAuthorWidth)
                    {
                        e.Cancel = true;
                    }
                    else if (list.Columns[0].Width + e.NewWidth < list.ClientSize.Width) //Появилась третья колонка
                    {
                        lockList = true;
                        list.Columns[0].Width = list.ClientSize.Width - e.NewWidth - 1;
                        lockList = false;
                    }
                    break;
            }
        }

        private void list_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            SetColumnSizeSafe(e.ColumnIndex);
        }

        private void MailingListManageDialog_Resize(object sender, EventArgs e)
        {
            SetColumnSizeSafe(-1);
        }

        private void SetColumnSizeSafe(int modifiedColumnIdx)
        {
            if (lockList)
                return;

            switch (modifiedColumnIdx)
            {
                case 0: //Меняется размер первой колонки
                    if (list.Columns[0].Width < minColumnNameWidth) //Задали меньше минимального размера
                        list.Columns[0].Width = minColumnNameWidth;
                    if (list.Columns[0].Width + list.Columns[1].Width < list.ClientSize.Width)
                        //Появилась третья колонка
                        list.Columns[1].Width = list.ClientSize.Width - list.Columns[0].Width;
                    break;
                case 1: //Меняется размер второй колонки
                    if (list.Columns[1].Width < minColumnAuthorWidth) //Задали меньше минимального размера
                        list.Columns[1].Width = minColumnAuthorWidth;
                    if (list.Columns[0].Width + list.Columns[1].Width < list.ClientSize.Width)
                        //Появилась третья колонка
                        list.Columns[0].Width = list.ClientSize.Width - list.Columns[1].Width;
                    break;
                case -1: //Меняется размер окна
                    if (list.Columns[0].Width + list.Columns[1].Width < list.ClientSize.Width)
                        //Появилась третья колонка
                        list.Columns[0].Width = list.ClientSize.Width - list.Columns[1].Width;
                    break;
            }
        }

        #endregion

        private void ckbSortByAuthor_CheckedChanged(object sender, EventArgs e)
        {
            list.SortByColumn(ckbSortByAuthor.Checked ? 1 : 0, SortOrder.Ascending);
        }

        private void MailingListManageDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            subLayout.Option("Width").Value = Width;
            subLayout.Option("Height").Value = Height;

            subLayout.Option("LocationX").Value = Location.X;
            subLayout.Option("LocationY").Value = Location.Y;

            subLayout.Option("ColumnNameWidth").Value = list.Columns[0].Width;
            subLayout.Option("ColumnAuthorWidth").Value = list.Columns[1].Width;

            subLayout.Save();

            if (Environment.UserSettings.SortMailingListByAuthor != ckbSortByAuthor.Checked)
            {
                Environment.UserSettings.SortMailingListByAuthor = ckbSortByAuthor.Checked;
                Environment.UserSettings.Save();
            }
        }
    }
}