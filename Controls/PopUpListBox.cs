using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Controls
{
    public class PopUpListBox : FreeDialog
    {
        private ListBox listBox;
        private ToolTip toolTip1;
        private Label labelCount;
        private IContainer components;

        public PopUpListBox(Rectangle rect, string actionText)
        {
            InitializeComponent();

            this.ActionText = actionText;

            Rectangle screenRect = Screen.FromControl(this).WorkingArea;
            int locationX = (screenRect.Width - rect.X - Size.Width > 0 || screenRect.Width/2 > rect.X)
                                ? rect.X
                                : (screenRect.Width - Size.Width - 1);
            int locationY = (screenRect.Height - rect.Y - Size.Height - rect.Height > 0 ||
                             screenRect.Height/2 > rect.Y + rect.Height)
                                ? (rect.Y + rect.Height + 3)
                                : (rect.Y - Size.Height);
            Location = new Point(locationX, locationY);
            toolTip1.ShowAlways = false;
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
            this.listBox = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.labelCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBox.Size = new System.Drawing.Size(128, 132);
            this.listBox.TabIndex = 0;
            this.listBox.TabStop = false;
            this.listBox.UseTabStops = false;
            this.listBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDown);
            this.listBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseMove);
            // 
            // labelCount
            // 
            this.labelCount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelCount.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelCount.Location = new System.Drawing.Point(0, 132);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(128, 24);
            this.labelCount.TabIndex = 1;
            this.labelCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PopUpListBox
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(128, 156);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.labelCount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopUpListBox";
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.PopUpListBox_Load);
            this.Deactivate += new System.EventHandler(this.PopUpListBox_Deactivate);
            this.ResumeLayout(false);
        }

        #endregion

        public ListBox.ObjectCollection Items
        {
            get { return listBox.Items; }
        }

        public int Count { get; private set; }

        public string ActionText { get; set; }

        private void listBox_MouseDown(object sender, MouseEventArgs e)
        {
            End(DialogResult.OK);
        }

        private void listBox_MouseMove(object sender, MouseEventArgs e)
        {
            SelectItems(e.X, e.Y);
        }

        private void SelectItems(int x, int y)
        {
            int hintindex = listBox.IndexFromPoint(x, y);
            if (hintindex > -1)
            {
                if (Count == hintindex)
                    return;

                Count = hintindex;
                labelCount.Text = calcActionStringRus(Count + 1);
                if (listBox.Items[Count] is ToolTopItem)
                    toolTip1.SetToolTip(listBox, ((ToolTopItem) listBox.Items[Count]).ToolTipText);
                else
                    toolTip1.SetToolTip(listBox, listBox.Items[Count].ToString());
            }
            else
                toolTip1.SetToolTip(listBox, "");
            for (int i = 0; i < listBox.Items.Count; ++i)
            {
                if (i <= Count)
                {
                    if (!listBox.GetSelected(i))
                        listBox.SetSelected(i, true);
                }
                else if (listBox.GetSelected(i))
                    listBox.SetSelected(i, false);
            }
        }

        private void PopUpListBox_Load(object sender, EventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                listBox.SetSelected(0, true);
                Count = 0;
                labelCount.Text = calcActionStringRus(Count + 1);
                if (listBox.Items[Count] is ToolTopItem)
                    toolTip1.SetToolTip(listBox, ((ToolTopItem) listBox.Items[Count]).ToolTipText);
                else
                    toolTip1.SetToolTip(listBox, listBox.Items[Count].ToString());
            }
        }

        private string calcActionStringRus(int count)
        {
            var sb = new StringBuilder();
            sb.Append(ActionText);
            sb.Append(" ");
            sb.Append(count.ToString());
            switch (count%10)
            {
                case 1:
                    sb.Append(count%100 != 11 ? " действия" : " действий");
                    break;
                case 2:
                case 3:
                case 4:
                    sb.Append(" действий");
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 0:
                    sb.Append(" действий");
                    break;
            }
            return sb.ToString();
        }

        private void PopUpListBox_Deactivate(object sender, EventArgs e)
        {
            End(DialogResult.Cancel);
        }
    }
}