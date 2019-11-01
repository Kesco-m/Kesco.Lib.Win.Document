using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC.Documents;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public class PrintEFormSelectDialog : FreeDialog
    {
        private object lastSelectedItem;

        private Button buttonOK;
        private Label labelText;
        private CheckedListBox typeBox;

        private Container components;

        public PrintEFormSelectDialog(int typeID, int docID)
        {
            InitializeComponent();

            object obj = Environment.GetDocTypeName(typeID);
            if (obj != null)
                labelText.Text += obj.ToString();

            if (typeID > 0)
            {
                using (DataTable dt = Environment.PrintData.GetEFormPrintTypeData(typeID, docID))
                {
                    if (dt.Rows.Count > 0)
                    {
                        SettingsPrintForm[] settings =
                            Environment.SettingsPrintForm.GetSettings(typeID);
                        bool isOneItemSelect = false;
                        foreach (DataRow dr in dt.Rows)
                        {
                            var po = new PrinterObjectClass((int) dr[Environment.PrintData.IDField],
                                                                           dr[Environment.PrintData.NameField].ToString(),
                                                                           dr[Environment.PrintData.URL].ToString(),
                                                                           Convert.ToInt32(
                                                                               dr[Environment.PrintData.PrintIDField]),
                                                                           (short)
                                                                           dr[Environment.PrintData.PaperSizeField]);

                            if (settings.Any(t => t.PrintID == po.TypeID && t.TypeID == typeID))
                            {
                                typeBox.Items.Add(po, CheckState.Checked);
                                isOneItemSelect = true;
                                typeBox.Items.Add(po, CheckState.Unchecked);
                            }
                        }
                        if (!isOneItemSelect)
                            typeBox.SetItemChecked(0, true);
                    }
                    else
                    {
                        Close();
                        throw new Exception(Environment.StringResources.GetString("Dialog_DocPrintDialog_Error1"));
                    }
                    dt.Dispose();
                }
            }
            typeBox.ItemCheck += typeBox_ItemCheck;
            typeBox.SelectedIndexChanged += typeBox_SelectedIndexChanged;
        }

        #region Accessors

        public List<PrinterObjectClass> PrinterObjectList { get; private set; }

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

        #region Windows Form Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (PrintEFormSelectDialog));
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelText = new System.Windows.Forms.Label();
            this.typeBox = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelText
            // 
            resources.ApplyResources(this.labelText, "labelText");
            this.labelText.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelText.Name = "labelText";
            // 
            // typeBox
            // 
            resources.ApplyResources(this.typeBox, "typeBox");
            this.typeBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.typeBox.Name = "typeBox";
            // 
            // PrintEFormSelectDialog
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.typeBox);
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.buttonOK);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PrintEFormSelectDialog";

            this.Load += new System.EventHandler(this.PrintEFormSelectDialog_Load);
            this.ResumeLayout(false);
        }

        #endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (PrinterObjectList == null)
                PrinterObjectList = new List<PrinterObjectClass>();
            foreach (var po in from object item in typeBox.CheckedItems select item as PrinterObjectClass)
            {
                po.PrintType = po.PrintID;
                PrinterObjectList.Add(po);
            }
            End(DialogResult.OK);
        }

        private void PrintEFormSelectDialog_Load(object sender, EventArgs e)
        {
            if (typeBox.Items.Count == 1)
            {
                PrinterObjectList = new List<PrinterObjectClass> {typeBox.Items[0] as PrinterObjectClass};
                End(DialogResult.OK);
            }
        }

        private void typeBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (typeBox.CheckedItems.Count == 0)
            {
                e.NewValue = CheckState.Checked;
            }
            else if (typeBox.CheckedItems.Count == 1 && e.NewValue != CheckState.Checked)
            {
                e.NewValue = CheckState.Checked;
            }
        }

        private void typeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeBox.CheckedItems.Count == 0)
            {
                typeBox.SetItemChecked(typeBox.SelectedIndex, true);
            }
            else if (lastSelectedItem != typeBox.SelectedItem)
            {
                bool isChecked = typeBox.CheckedItems.Cast<object>().Any(selItem => selItem == typeBox.SelectedItem);
                typeBox.SetItemChecked(typeBox.SelectedIndex, !isChecked);
            }
            lastSelectedItem = typeBox.SelectedItem;
        }
    }
}