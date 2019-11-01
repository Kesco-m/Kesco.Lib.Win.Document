using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Web;
using Timer = System.Threading.Timer;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public class EmpContacts : Form
    {
        private IContainer components;

        private Panel MainPanel;
        private PictureBox pbMessenger;
        private PictureBox pbEmail;
        private PictureBox pbPhone;
        private ToolTip toolTip1;
        private Employee emp;
        private Timer timer;
        private const int timeout = 1000;
        private static ArrayList dialPhoneList = new ArrayList();
        private UrlBrowseDialog udb;

        /// <summary>
        ///   Конструктор класса
        /// </summary>
        /// <param name="e"> Выбранный сотрудник </param>
        public EmpContacts(Employee e)
        {
            InitializeComponent();
            emp = e;
            StartPosition = FormStartPosition.Manual;
            CreateForm();
            SetFormLocation();
            RunTimer();
        }

        /// <summary>
        ///   Инициализация формы, вывод ин-ии по сотруднику
        /// </summary>
        private void CreateForm()
        {
            const int x = 5;
            int y = 5;
            var phoneList = new ArrayList();

            using (DataTable dt = Environment.EmpData.GetContacts(emp.ID))
            using (DataTableReader dr = dt.CreateDataReader())
            {
                while (dr.Read())
                {
                    if ((dr["dial"].ToString().Equals(string.Empty)) || (phoneList.Contains(dr["view"].ToString())) ||
                        (!dr["type"].Equals(20) &&
                         !dr["type"].Equals(21) &&
                         !dr["type"].Equals(22) &&
                         !dr["type"].Equals(0)))
                        continue;
                    try
                    {
                        phoneList.Add(dr["view"].ToString());
                        PictureBox pb = CreateContactIcon(x, y, Convert.ToInt32(dr["type"]));
                        pb.Tag = dr["dial"];

                        Label lab = CreateContactLabel(pb.Location.X + pb.Width + 10, y, dr["view"].ToString(),
                                                       Convert.ToInt32(dr["type"]));
                        lab.Tag = dr["dial"];

                        MainPanel.Controls.Add(pb);
                        MainPanel.Controls.Add(lab);
                        y += pb.Height + 5;
                    }
                    catch (Exception ex)
                    {
                        Data.Env.WriteToLog(ex);
                    }
                }
                dr.Close();
                dr.Dispose();
                dt.Dispose();
            }
            PictureBox pBox = CreateContactIcon(x, y, 1);
            toolTip1.SetToolTip(pBox, "Email");

            Label l = CreateContactLabel(pBox.Location.X + pBox.Width + 10, y, emp.EmpEMail, 1);
            toolTip1.SetToolTip(l, "Email");

            MainPanel.Controls.Add(pBox);
            MainPanel.Controls.Add(l);
            y += pBox.Height + 5;

            pBox = CreateContactIcon(x, y, 2);
            toolTip1.SetToolTip(pBox, "Messanger");
            l = CreateContactLabel(pBox.Location.X + pBox.Width + 10, y, emp.EmpEMail, 2);
            toolTip1.SetToolTip(l, "Messanger");
            MainPanel.Controls.Add(pBox);
            MainPanel.Controls.Add(l);

            Height = l.Location.Y + l.Height + 10;
            if (MainPanel.Height > 500)
            {
                MainPanel.AutoScroll = true;
                Height = 500;
            }
        }

        /// <summary>
        ///   Устанавливаем позицию формы
        /// </summary>
        private void SetFormLocation()
        {
            int screenWidth = SystemInformation.WorkingArea.Width;
            int screenHeight = SystemInformation.WorkingArea.Height;

            var areaI = new Rectangle(0, 0, Width, Height);
            var areaII = new Rectangle(screenWidth - Width, 0, Width, Height);
            var areaIII = new Rectangle(0, screenHeight - Height, Width, Height);
            var areaIV = new Rectangle(screenWidth - Width, screenHeight - Height, Width,
                                             Height);
            var areaV = new Rectangle(Width, screenHeight - Height, screenWidth - Width,
                                            Height);
            var areaVI = new Rectangle(screenWidth - Width, Height, Width,
                                             screenHeight - Height);

            Point cursorPosition = Cursor.Position;
            Point formLocation = cursorPosition;

            if (areaI.Contains(cursorPosition))
                formLocation = new Point(cursorPosition.X + 5, cursorPosition.Y + 5);
            else if (areaII.Contains(cursorPosition))
                formLocation = new Point(screenWidth - Width - 5, cursorPosition.Y + 5);
            else if (areaIII.Contains(cursorPosition))
                formLocation = new Point(cursorPosition.X + 5, screenHeight - Height - 5);
            else if (areaIV.Contains(cursorPosition))
                formLocation = new Point(screenWidth - Width - 5, screenHeight - Height - 5);
            else if (areaV.Contains(cursorPosition))
                formLocation = new Point(cursorPosition.X + 5, screenHeight - Height - 5);
            else if (areaVI.Contains(cursorPosition))
                formLocation = new Point(screenWidth - Width - 5, cursorPosition.Y + 5);

            Location = formLocation;
        }

        /// <summary>
        ///   Создание иконки
        /// </summary>
        /// <param name="x"> Расположение иконки по оси Х </param>
        /// <param name="y"> Расположение иконки по оси Y </param>
        /// <param name="type"> Тип контакта </param>
        private PictureBox CreateContactIcon(int x, int y, int type)
        {
            var pb = new PictureBox {Size = new Size(16, 16), Cursor = Cursors.Hand, Location = new Point(x, y)};
            pb.MouseLeave += control_MouseLeave;
            pb.MouseEnter += control_MouseEnter;
            switch (type)
            {
                case 0:
                case 20:
                case 21:
                case 22:
                    pb.Image = pbPhone.Image;
                    pb.Click += Calling;
                    break;
                case 1:
                    pb.Image = pbEmail.Image;
                    pb.Click += SendEmail;
                    break;
                case 2:
                    pb.Image = pbMessenger.Image;
                    pb.Click += SendMessage;
                    break;
            }
            return pb;
        }

        /// <summary>
        ///   Создание ссылки на контакт
        /// </summary>
        /// <param name="x"> Расположение иконки по оси Х </param>
        /// <param name="y"> Расположение иконки по оси Y </param>
        /// <param name="text"> Текст </param>
        /// <param name="type"> Тип контакта </param>
        private Label CreateContactLabel(int x, int y, string text, int type)
        {
            var lab = new Label {AutoSize = true, Cursor = Cursors.Hand, Location = new Point(x, y), Text = text};
            lab.MouseEnter += lab_MouseEnter;
            lab.MouseLeave += lab_MouseLeave;
            switch (type)
            {
                case 0:
                case 40:
                case 21:
                case 22:
                    lab.Click += Calling;
                    break;
                case 1:
                    lab.Click += SendEmail;
                    break;
                case 2:
                    lab.Click += SendMessage;
                    break;
            }
            return lab;
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

        #region Component Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (EmpContacts));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.pbMessenger = new System.Windows.Forms.PictureBox();
            this.pbEmail = new System.Windows.Forms.PictureBox();
            this.pbPhone = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.MainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.pbMessenger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.pbEmail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.pbPhone)).BeginInit();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.SystemColors.Info;
            this.MainPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MainPanel.Controls.Add(this.pbMessenger);
            this.MainPanel.Controls.Add(this.pbEmail);
            this.MainPanel.Controls.Add(this.pbPhone);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(250, 88);
            this.MainPanel.TabIndex = 1;
            this.MainPanel.MouseEnter += new System.EventHandler(this.control_MouseEnter);
            this.MainPanel.MouseLeave += new System.EventHandler(this.control_MouseLeave);
            // 
            // pbMessenger
            // 
            this.pbMessenger.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbMessenger.Image = ((System.Drawing.Image) (resources.GetObject("pbMessenger.Image")));
            this.pbMessenger.Location = new System.Drawing.Point(8, 40);
            this.pbMessenger.Name = "pbMessenger";
            this.pbMessenger.Size = new System.Drawing.Size(16, 15);
            this.pbMessenger.TabIndex = 4;
            this.pbMessenger.TabStop = false;
            this.pbMessenger.Visible = false;
            // 
            // pbEmail
            // 
            this.pbEmail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbEmail.Image = ((System.Drawing.Image) (resources.GetObject("pbEmail.Image")));
            this.pbEmail.Location = new System.Drawing.Point(5, 24);
            this.pbEmail.Name = "pbEmail";
            this.pbEmail.Size = new System.Drawing.Size(16, 15);
            this.pbEmail.TabIndex = 2;
            this.pbEmail.TabStop = false;
            this.pbEmail.Visible = false;
            // 
            // pbPhone
            // 
            this.pbPhone.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbPhone.Image = ((System.Drawing.Image) (resources.GetObject("pbPhone.Image")));
            this.pbPhone.Location = new System.Drawing.Point(5, 5);
            this.pbPhone.Name = "pbPhone";
            this.pbPhone.Size = new System.Drawing.Size(16, 15);
            this.pbPhone.TabIndex = 0;
            this.pbPhone.TabStop = false;
            this.pbPhone.Visible = false;
            // 
            // EmpContacts
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(250, 88);
            this.Controls.Add(this.MainPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "EmpContacts";
            this.ShowInTaskbar = false;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EmpContacts_KeyDown);
            this.MouseEnter += new System.EventHandler(this.control_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.control_MouseLeave);
            this.MainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.pbMessenger)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.pbEmail)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.pbPhone)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>
        ///   Звонок сотруднику
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void Calling(object sender, EventArgs e)
        {
            if (!dialPhoneList.Contains(((System.Windows.Forms.Control) sender).Tag))
                dialPhoneList.Add(((System.Windows.Forms.Control) sender).Tag);
            else
            {
                Close();
                return;
            }
            string url = Environment.DialURL;
            //url = "http://beta.testcom.com/webdialer/dialer.aspx?dialnumber=";
            udb = new UrlBrowseDialog(url + ((System.Windows.Forms.Control) sender).Tag, "Связь")
                      {
                          Width = 500,
                          Height = 150,
                          MaximizeBox = false,
                          MinimizeBox = false,
                          Tag = ((System.Windows.Forms.Control) sender).Tag
                      };
            udb.Closed += udb_Closed;
            udb.Show();
            Close();
        }


        /// <summary>
        ///   Отправка электронного сообщения
        /// </summary>
        private void SendEmail(object sender, EventArgs e)
        {
            Process.Start("iexplore", Environment.FullExchangeServerOwaUrl + "/?Cmd=new&mailtoaddr=" + emp.EmpEMail);
            Close();
        }


        /// <summary>
        ///   Отправка сообщения по MSN
        /// </summary>
        private void SendMessage(object sender, EventArgs e)
        {
            Type objMsgType = Type.GetTypeFromProgID("Messenger.MessengerApp");
            if (objMsgType == null)
                return;
            object objMsg = null;

            try
            {
                objMsg = Activator.CreateInstance(objMsgType);

                if (objMsg == null) return;
                objMsg.GetType().InvokeMember("LaunchIMUI", BindingFlags.InvokeMethod, null, objMsg,
                                              new object[] {emp.EmpEMail});
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
            finally
            {
                if (objMsg != null)
                    Marshal.ReleaseComObject(objMsg);
                objMsgType = null;
                objMsg = null;
            }
            Close();
        }

        #region Мышинные ивенты

        private void lab_MouseEnter(object sender, EventArgs e)
        {
            ((Label) sender).ForeColor = Color.Blue;
            ((Label) sender).Font = new Font(((Label) sender).Font, FontStyle.Underline);
            control_MouseEnter(sender, e);
        }

        private void lab_MouseLeave(object sender, EventArgs e)
        {
            ((Label) sender).ForeColor = SystemColors.ControlText;
            ((Label) sender).Font = new Font(((Label) sender).Font, FontStyle.Regular);
            control_MouseLeave(sender, e);
        }

        private void control_MouseLeave(object sender, EventArgs e)
        {
            RunTimer();
        }

        private void control_MouseEnter(object sender, EventArgs e)
        {
            StopTimer();
        }

        #endregion

        #region Закрытие формы

        private void CloseForm(object obj)
        {
            if (InvokeRequired)
                Invoke(new Action<object>(CloseForm), obj);
            else
            {
                Point cursorPosition = Cursor.Position;
                if (Bounds.Contains(cursorPosition))
                {
                    RunTimer();
                    return;
                }
                Close();
            }
        }

        private void EmpContacts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData.Equals(Keys.Escape))
                Close();
        }

        #endregion

        /// <summary>
        ///   Запуск таймера на закрытие формы
        /// </summary>
        private void RunTimer()
        {
            if (timer != null)
                timer.Dispose();
            timer = new Timer(CloseForm, null, timeout, Timeout.Infinite);
        }

        /// <summary>
        ///   Остановка таймера на закрытие формы
        /// </summary>
        private void StopTimer()
        {
            if (timer != null)
                timer.Dispose();
        }

        private void udb_Closed(object sender, EventArgs e)
        {
            dialPhoneList.Remove(((UrlBrowseDialog) sender).Tag);
        }
    }
}