using System;
using System.ComponentModel;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.Temp;

namespace Kesco.Lib.Win.Document.Blocks
{
    public class DateBlock : UserControl
    {
        public event EventHandler<DateBlockEventArgs> RaiseDateBlockEvent;

        private string error;
        private TextBox date;
        private DateTimePicker datePicker;

        private Container components;

        public DateBlock()
        {
            InitializeComponent();

            datePicker.Value = DateTime.Now;
            date.Text = "";

            date.TextChanged += dateTextChanged;
            datePicker.ValueChanged += datePickerValueChanged;
        }

        #region Accessors

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime Value
        {
            get
            {
                if (DesignerDetector.IsComponentInDesignMode(this))
                    return datePicker.MinDate;
                if (Check())
                    return datePicker.Value;
                throw new Exception(error);
            }
            set { datePicker.Value = value; }
        }

        public string Error
        {
            get { return error; }
        }

        public override string Text
        {
            get { return date.Text; }
            set { date.Text = value; }
        }

        #endregion

        /// <summary>
        ///   Clean up any resources being used.
        /// </summary>
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
            this.date = new System.Windows.Forms.TextBox();
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // date
            // 
            this.date.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                   | System.Windows.Forms.AnchorStyles.Right)));
            this.date.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.date.Location = new System.Drawing.Point(0, 0);
            this.date.Name = "date";
            this.date.Size = new System.Drawing.Size(174, 20);
            this.date.TabIndex = 7;
            // 
            // datePicker
            // 
            this.datePicker.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                 ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.datePicker.CustomFormat = "";
            this.datePicker.Location = new System.Drawing.Point(168, 0);
            this.datePicker.Name = "datePicker";
            this.datePicker.Size = new System.Drawing.Size(24, 20);
            this.datePicker.TabIndex = 8;
            this.datePicker.TabStop = false;
            // 
            // DateBlock
            // 
            this.Controls.Add(this.date);
            this.Controls.Add(this.datePicker);
            this.DoubleBuffered = true;
            this.Name = "DateBlock";
            this.Size = new System.Drawing.Size(192, 20);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        protected virtual void dateTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsEmpty())
                {
                    datePicker.Checked = false;
                    OnRaiseDateBlockEvent(new DateBlockEventArgs(DateTime.Now, false));
                }
                else
                {
                    if (date.Text != datePicker.Value.Date.ToString("dd.MM.yyyy") &&
                        date.Text.Length == datePicker.Value.Date.ToString("dd.MM.yyyy").Length)
                        Check();
                }
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        protected virtual void datePickerValueChanged(object sender, EventArgs e)
        {
            try
            {
                date.Text = datePicker.Value.Date.ToString("dd.MM.yyyy");
                OnRaiseDateBlockEvent(new DateBlockEventArgs(datePicker.Value, true));
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        protected virtual void OnRaiseDateBlockEvent(DateBlockEventArgs d)
        {
            EventHandler<DateBlockEventArgs> handler = RaiseDateBlockEvent;

            if (handler != null)
                handler(this, d);
        }

        public bool IsEmpty()
        {
            return (date.Text.Length == 0);
        }

        public bool Check()
        {
            if (!IsEmpty())
            {
                DateTime d = DateTime.Now;

                try
                {
                    d = NewDateParser.Parse(date.Text);
                }
                catch (Exception ex)
                {
                    error = ex.Message; //"Неправильный формат даты";
                    return false;
                }

                datePicker.Value = d;
            }
            return true;
        }
    }

    public class DateBlockEventArgs : EventArgs
    {
        public DateBlockEventArgs(DateTime value, bool filterUsedPersons)
        {
            Value = value;
            FilterUsedPersons = filterUsedPersons;
        }

        public DateTime Value { get; private set; }
        public bool FilterUsedPersons { get; private set; }
    }
}