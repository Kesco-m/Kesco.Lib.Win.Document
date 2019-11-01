using System.ComponentModel;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
    /// <summary>
    /// Диалог настройки печати
    /// </summary>
    partial class DocPrintDialog : FreeDialog
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Environment.Printers.Updated -= prnList_Updated;
                Environment.Printers.Loaded -= prnList_Loaded;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocPrintDialog));
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.bRefresh = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBoxOrientation = new System.Windows.Forms.PictureBox();
            this.bookRButton = new System.Windows.Forms.RadioButton();
            this.albumRButton = new System.Windows.Forms.RadioButton();
            this.autoRButton = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.originalRButton = new System.Windows.Forms.RadioButton();
            this.fitRButton = new System.Windows.Forms.RadioButton();
            this.groupBoxMargins = new System.Windows.Forms.GroupBox();
            this.labelL = new System.Windows.Forms.Label();
            this.labelR = new System.Windows.Forms.Label();
            this.labelB = new System.Windows.Forms.Label();
            this.labelT = new System.Windows.Forms.Label();
            this.numMarginTop = new System.Windows.Forms.NumericUpDown();
            this.numMarginBottom = new System.Windows.Forms.NumericUpDown();
            this.numMarginLeft = new System.Windows.Forms.NumericUpDown();
            this.numMarginRight = new System.Windows.Forms.NumericUpDown();
            this.annotationCheck = new System.Windows.Forms.CheckBox();
            this.showCheck = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.doc = new System.Windows.Forms.Label();
            this.bProperties = new System.Windows.Forms.Button();
            this.duplexBox = new System.Windows.Forms.CheckBox();
            this.typeBox = new System.Windows.Forms.CheckedListBox();
            this.panelDoc = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonAll = new System.Windows.Forms.RadioButton();
            this.radioButtonOnlyMain = new System.Windows.Forms.RadioButton();
            this.checkPrintImage = new System.Windows.Forms.CheckBox();
            this.checkBoxEform = new System.Windows.Forms.CheckBox();
            this.panelImage1 = new System.Windows.Forms.Panel();
            this.groupBoxPrintPDF = new System.Windows.Forms.GroupBox();
            this.comboBoxPrintResolution = new System.Windows.Forms.ComboBox();
            this.panelImage2 = new System.Windows.Forms.Panel();
            this.checkBoxPrintBarCode = new System.Windows.Forms.CheckBox();
            this.labelEForm = new System.Windows.Forms.Label();
            this.imageListOrintation = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBoxCopy = new System.Windows.Forms.GroupBox();
            this.numericUpDownCopy = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.panelPrint = new System.Windows.Forms.Panel();
            this.groupBoxPrint = new System.Windows.Forms.GroupBox();
            this.textBoxStartPage = new System.Windows.Forms.NumericUpDown();
            this.textBoxEndPage = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.rbSelectedPages = new System.Windows.Forms.RadioButton();
            this.rbAllPages = new System.Windows.Forms.RadioButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.newWindowDocumentButton = new Kesco.Lib.Win.Document.Controls.NewWindowDocumentButton(this.components);
            this.printers = new Kesco.Lib.Win.Document.Dialogs.PrintDialogs.NoDoubleClickAutoCheckListView();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOrientation)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBoxMargins.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginRight)).BeginInit();
            this.panelDoc.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelImage1.SuspendLayout();
            this.groupBoxPrintPDF.SuspendLayout();
            this.panelImage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBoxCopy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopy)).BeginInit();
            this.panelPrint.SuspendLayout();
            this.groupBoxPrint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxStartPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxEndPage)).BeginInit();
            this.SuspendLayout();
            // 
            // bCancel
            // 
            resources.ApplyResources(this.bCancel, "bCancel");
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Name = "bCancel";
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOK
            // 
            resources.ApplyResources(this.bOK, "bOK");
            this.bOK.Name = "bOK";
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // bRefresh
            // 
            resources.ApplyResources(this.bRefresh, "bRefresh");
            this.bRefresh.Name = "bRefresh";
            this.bRefresh.Click += new System.EventHandler(this.bRefresh_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBoxOrientation);
            this.groupBox2.Controls.Add(this.bookRButton);
            this.groupBox2.Controls.Add(this.albumRButton);
            this.groupBox2.Controls.Add(this.autoRButton);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // pictureBoxOrientation
            // 
            resources.ApplyResources(this.pictureBoxOrientation, "pictureBoxOrientation");
            this.pictureBoxOrientation.Name = "pictureBoxOrientation";
            this.pictureBoxOrientation.TabStop = false;
            // 
            // bookRButton
            // 
            resources.ApplyResources(this.bookRButton, "bookRButton");
            this.bookRButton.Name = "bookRButton";
            this.bookRButton.CheckedChanged += new System.EventHandler(this.bookRButton_CheckedChanged);
            // 
            // albumRButton
            // 
            resources.ApplyResources(this.albumRButton, "albumRButton");
            this.albumRButton.Name = "albumRButton";
            this.albumRButton.CheckedChanged += new System.EventHandler(this.albumRButton_CheckedChanged);
            // 
            // autoRButton
            // 
            resources.ApplyResources(this.autoRButton, "autoRButton");
            this.autoRButton.Name = "autoRButton";
            this.autoRButton.CheckedChanged += new System.EventHandler(this.autoRButton_CheckedChanged);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.originalRButton);
            this.groupBox3.Controls.Add(this.fitRButton);
            this.groupBox3.Controls.Add(this.groupBoxMargins);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // originalRButton
            // 
            resources.ApplyResources(this.originalRButton, "originalRButton");
            this.originalRButton.Name = "originalRButton";
            this.originalRButton.CheckedChanged += new System.EventHandler(this.originalRButton_CheckedChanged);
            // 
            // fitRButton
            // 
            resources.ApplyResources(this.fitRButton, "fitRButton");
            this.fitRButton.Name = "fitRButton";
            this.fitRButton.CheckedChanged += new System.EventHandler(this.fitRButton_CheckedChanged);
            // 
            // groupBoxMargins
            // 
            this.groupBoxMargins.Controls.Add(this.labelL);
            this.groupBoxMargins.Controls.Add(this.labelR);
            this.groupBoxMargins.Controls.Add(this.labelB);
            this.groupBoxMargins.Controls.Add(this.labelT);
            this.groupBoxMargins.Controls.Add(this.numMarginTop);
            this.groupBoxMargins.Controls.Add(this.numMarginBottom);
            this.groupBoxMargins.Controls.Add(this.numMarginLeft);
            this.groupBoxMargins.Controls.Add(this.numMarginRight);
            resources.ApplyResources(this.groupBoxMargins, "groupBoxMargins");
            this.groupBoxMargins.Name = "groupBoxMargins";
            this.groupBoxMargins.TabStop = false;
            // 
            // labelL
            // 
            resources.ApplyResources(this.labelL, "labelL");
            this.labelL.Name = "labelL";
            // 
            // labelR
            // 
            resources.ApplyResources(this.labelR, "labelR");
            this.labelR.Name = "labelR";
            // 
            // labelB
            // 
            resources.ApplyResources(this.labelB, "labelB");
            this.labelB.Name = "labelB";
            // 
            // labelT
            // 
            resources.ApplyResources(this.labelT, "labelT");
            this.labelT.Name = "labelT";
            // 
            // numMarginTop
            // 
            this.numMarginTop.DecimalPlaces = 2;
            resources.ApplyResources(this.numMarginTop, "numMarginTop");
            this.numMarginTop.Name = "numMarginTop";
            // 
            // numMarginBottom
            // 
            this.numMarginBottom.DecimalPlaces = 2;
            resources.ApplyResources(this.numMarginBottom, "numMarginBottom");
            this.numMarginBottom.Name = "numMarginBottom";
            // 
            // numMarginLeft
            // 
            this.numMarginLeft.DecimalPlaces = 2;
            resources.ApplyResources(this.numMarginLeft, "numMarginLeft");
            this.numMarginLeft.Name = "numMarginLeft";
            // 
            // numMarginRight
            // 
            this.numMarginRight.DecimalPlaces = 2;
            resources.ApplyResources(this.numMarginRight, "numMarginRight");
            this.numMarginRight.Name = "numMarginRight";
            // 
            // annotationCheck
            // 
            resources.ApplyResources(this.annotationCheck, "annotationCheck");
            this.annotationCheck.Checked = true;
            this.annotationCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.annotationCheck.Name = "annotationCheck";
            this.annotationCheck.CheckedChanged += new System.EventHandler(this.annotationCheck_CheckedChanged);
            // 
            // showCheck
            // 
            resources.ApplyResources(this.showCheck, "showCheck");
            this.showCheck.Checked = true;
            this.showCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showCheck.Name = "showCheck";
            this.showCheck.CheckedChanged += new System.EventHandler(this.showCheck_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // doc
            // 
            this.doc.AutoEllipsis = true;
            resources.ApplyResources(this.doc, "doc");
            this.doc.Name = "doc";
            // 
            // bProperties
            // 
            resources.ApplyResources(this.bProperties, "bProperties");
            this.bProperties.Name = "bProperties";
            this.bProperties.Click += new System.EventHandler(this.bProperties_Click);
            // 
            // duplexBox
            // 
            resources.ApplyResources(this.duplexBox, "duplexBox");
            this.duplexBox.Name = "duplexBox";
            this.duplexBox.CheckedChanged += new System.EventHandler(this.duplexBox_CheckedChanged);
            // 
            // typeBox
            // 
            resources.ApplyResources(this.typeBox, "typeBox");
            this.typeBox.Name = "typeBox";
            this.toolTip.SetToolTip(this.typeBox, resources.GetString("typeBox.ToolTip"));
            this.typeBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.typeBox_MouseMove);
            // 
            // panelDoc
            // 
            resources.ApplyResources(this.panelDoc, "panelDoc");
            this.panelDoc.Controls.Add(this.groupBox1);
            this.panelDoc.Name = "panelDoc";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonAll);
            this.groupBox1.Controls.Add(this.radioButtonOnlyMain);
            this.groupBox1.Controls.Add(this.checkPrintImage);
            this.groupBox1.Controls.Add(this.checkBoxEform);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // radioButtonAll
            // 
            resources.ApplyResources(this.radioButtonAll, "radioButtonAll");
            this.radioButtonAll.Name = "radioButtonAll";
            // 
            // radioButtonOnlyMain
            // 
            this.radioButtonOnlyMain.Checked = true;
            resources.ApplyResources(this.radioButtonOnlyMain, "radioButtonOnlyMain");
            this.radioButtonOnlyMain.Name = "radioButtonOnlyMain";
            this.radioButtonOnlyMain.TabStop = true;
            this.radioButtonOnlyMain.CheckedChanged += new System.EventHandler(this.radioButtonOnlyMain_CheckedChanged);
            // 
            // checkPrintImage
            // 
            this.checkPrintImage.Checked = true;
            this.checkPrintImage.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.checkPrintImage, "checkPrintImage");
            this.checkPrintImage.Name = "checkPrintImage";
            this.checkPrintImage.CheckedChanged += new System.EventHandler(this.checkBoxPrintImage_CheckedChanged);
            // 
            // checkBoxEform
            // 
            this.checkBoxEform.Checked = true;
            this.checkBoxEform.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.checkBoxEform, "checkBoxEform");
            this.checkBoxEform.Name = "checkBoxEform";
            this.checkBoxEform.CheckedChanged += new System.EventHandler(this.checkBoxEform_CheckedChanged);
            // 
            // panelImage1
            // 
            this.panelImage1.Controls.Add(this.groupBox3);
            this.panelImage1.Controls.Add(this.groupBox2);
            this.panelImage1.Controls.Add(this.groupBoxPrintPDF);
            resources.ApplyResources(this.panelImage1, "panelImage1");
            this.panelImage1.Name = "panelImage1";
            // 
            // groupBoxPrintPDF
            // 
            this.groupBoxPrintPDF.Controls.Add(this.comboBoxPrintResolution);
            resources.ApplyResources(this.groupBoxPrintPDF, "groupBoxPrintPDF");
            this.groupBoxPrintPDF.Name = "groupBoxPrintPDF";
            this.groupBoxPrintPDF.TabStop = false;
            // 
            // comboBoxPrintResolution
            // 
            this.comboBoxPrintResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPrintResolution.FormattingEnabled = true;
            this.comboBoxPrintResolution.Items.AddRange(new object[] {
            resources.GetString("comboBoxPrintResolution.Items"),
            resources.GetString("comboBoxPrintResolution.Items1"),
            resources.GetString("comboBoxPrintResolution.Items2"),
            resources.GetString("comboBoxPrintResolution.Items3")});
            resources.ApplyResources(this.comboBoxPrintResolution, "comboBoxPrintResolution");
            this.comboBoxPrintResolution.Name = "comboBoxPrintResolution";
            // 
            // panelImage2
            // 
            resources.ApplyResources(this.panelImage2, "panelImage2");
            this.panelImage2.Controls.Add(this.checkBoxPrintBarCode);
            this.panelImage2.Controls.Add(this.annotationCheck);
            this.panelImage2.Controls.Add(this.labelEForm);
            this.panelImage2.Name = "panelImage2";
            // 
            // checkBoxPrintBarCode
            // 
            resources.ApplyResources(this.checkBoxPrintBarCode, "checkBoxPrintBarCode");
            this.checkBoxPrintBarCode.Name = "checkBoxPrintBarCode";
            this.checkBoxPrintBarCode.CheckedChanged += new System.EventHandler(this.checkBoxPrintBarCode_CheckedChanged);
            // 
            // labelEForm
            // 
            resources.ApplyResources(this.labelEForm, "labelEForm");
            this.labelEForm.Name = "labelEForm";
            // 
            // imageListOrintation
            // 
            this.imageListOrintation.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListOrintation.ImageStream")));
            this.imageListOrintation.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListOrintation.Images.SetKeyName(0, "");
            this.imageListOrintation.Images.SetKeyName(1, "");
            this.imageListOrintation.Images.SetKeyName(2, "");
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.groupBoxCopy);
            this.panel1.Name = "panel1";
            // 
            // groupBoxCopy
            // 
            this.groupBoxCopy.Controls.Add(this.numericUpDownCopy);
            this.groupBoxCopy.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBoxCopy, "groupBoxCopy");
            this.groupBoxCopy.Name = "groupBoxCopy";
            this.groupBoxCopy.TabStop = false;
            // 
            // numericUpDownCopy
            // 
            resources.ApplyResources(this.numericUpDownCopy, "numericUpDownCopy");
            this.numericUpDownCopy.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDownCopy.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCopy.Name = "numericUpDownCopy";
            this.numericUpDownCopy.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panelPrint
            // 
            this.panelPrint.Controls.Add(this.groupBoxPrint);
            resources.ApplyResources(this.panelPrint, "panelPrint");
            this.panelPrint.Name = "panelPrint";
            // 
            // groupBoxPrint
            // 
            this.groupBoxPrint.Controls.Add(this.textBoxStartPage);
            this.groupBoxPrint.Controls.Add(this.textBoxEndPage);
            this.groupBoxPrint.Controls.Add(this.label3);
            this.groupBoxPrint.Controls.Add(this.rbSelectedPages);
            this.groupBoxPrint.Controls.Add(this.rbAllPages);
            resources.ApplyResources(this.groupBoxPrint, "groupBoxPrint");
            this.groupBoxPrint.Name = "groupBoxPrint";
            this.groupBoxPrint.TabStop = false;
            // 
            // textBoxStartPage
            // 
            resources.ApplyResources(this.textBoxStartPage, "textBoxStartPage");
            this.textBoxStartPage.Name = "textBoxStartPage";
            // 
            // textBoxEndPage
            // 
            resources.ApplyResources(this.textBoxEndPage, "textBoxEndPage");
            this.textBoxEndPage.Name = "textBoxEndPage";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // rbSelectedPages
            // 
            resources.ApplyResources(this.rbSelectedPages, "rbSelectedPages");
            this.rbSelectedPages.Name = "rbSelectedPages";
            // 
            // rbAllPages
            // 
            this.rbAllPages.Checked = true;
            resources.ApplyResources(this.rbAllPages, "rbAllPages");
            this.rbAllPages.Name = "rbAllPages";
            this.rbAllPages.TabStop = true;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 50;
            this.toolTip.ReshowDelay = 100;
            // 
            // newWindowDocumentButton
            // 
            resources.ApplyResources(this.newWindowDocumentButton, "newWindowDocumentButton");
            this.newWindowDocumentButton.Name = "newWindowDocumentButton";
            this.newWindowDocumentButton.TabStop = false;
            this.newWindowDocumentButton.UseMnemonic = false;
            this.newWindowDocumentButton.UseVisualStyleBackColor = true;
            // 
            // printers
            // 
            resources.ApplyResources(this.printers, "printers");
            this.printers.FullRowSelect = true;
            this.printers.HideSelection = false;
            this.printers.MultiSelect = false;
            this.printers.Name = "printers";
            this.printers.UseCompatibleStateImageBehavior = false;
            this.printers.View = System.Windows.Forms.View.Details;
            this.printers.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.printers_ColumnClick);
            this.printers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.printers_ItemCheck);
            this.printers.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.printers_ItemChecked);
            this.printers.SelectedIndexChanged += new System.EventHandler(this.printers_SelectedIndexChanged);
            // 
            // DocPrintDialog
            // 
            this.AcceptButton = this.bOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.bCancel;
            this.Controls.Add(this.newWindowDocumentButton);
            this.Controls.Add(this.typeBox);
            this.Controls.Add(this.panelPrint);
            this.Controls.Add(this.bProperties);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelImage2);
            this.Controls.Add(this.panelImage1);
            this.Controls.Add(this.panelDoc);
            this.Controls.Add(this.duplexBox);
            this.Controls.Add(this.printers);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.doc);
            this.Controls.Add(this.showCheck);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bRefresh);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DocPrintDialog";
            this.Activated += new System.EventHandler(this.DocPrintDialog_Activated);
            this.Load += new System.EventHandler(this.DocPrintDialog_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DocPrintDialog_KeyUp);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOrientation)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBoxMargins.ResumeLayout(false);
            this.groupBoxMargins.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMarginRight)).EndInit();
            this.panelDoc.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panelImage1.ResumeLayout(false);
            this.groupBoxPrintPDF.ResumeLayout(false);
            this.panelImage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBoxCopy.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCopy)).EndInit();
            this.panelPrint.ResumeLayout(false);
            this.groupBoxPrint.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textBoxStartPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxEndPage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

		private Button bCancel;
        private GroupBox groupBox2;
        private RadioButton bookRButton;
        private RadioButton albumRButton;
        private RadioButton autoRButton;
        private GroupBox groupBox3;
        private CheckBox annotationCheck;
        private CheckBox showCheck;
        private RadioButton fitRButton;
        private RadioButton originalRButton;
        private Button bOK;
        private Button bRefresh;
        private Label label1;
        private Label doc;
        private Kesco.Lib.Win.Document.Dialogs.PrintDialogs.NoDoubleClickAutoCheckListView printers;
        private Button bProperties;
        private CheckBox duplexBox;
        private CheckedListBox typeBox;
        private Panel panelDoc;
        private CheckBox checkBoxEform;
        private GroupBox groupBox1;
        private CheckBox checkPrintImage;
        private RadioButton radioButtonOnlyMain;
        private RadioButton radioButtonAll;
        private Panel panelImage1;
        private Panel panelImage2;
        private Label labelEForm;
        private ImageList imageListOrintation;
        private PictureBox pictureBoxOrientation;
        private CheckBox checkBoxPrintBarCode;
        private Panel panel1;
        private GroupBox groupBoxCopy;
        private Label label2;
        private NumericUpDown numericUpDownCopy;
        private Panel panelPrint;
        private GroupBox groupBoxPrint;
        private Label label3;
        private RadioButton rbSelectedPages;
        private RadioButton rbAllPages;
        private NumericUpDown textBoxEndPage;
        private NumericUpDown textBoxStartPage;
        private ToolTip toolTip;
        private Controls.NewWindowDocumentButton newWindowDocumentButton;
		private NumericUpDown numMarginLeft;
		private NumericUpDown numMarginBottom;
		private NumericUpDown numMarginRight;
		private NumericUpDown numMarginTop;
		private ComboBox comboBoxPrintResolution;
        private IContainer components;
		private GroupBox groupBoxMargins;
		private Label labelL;
		private Label labelR;
		private Label labelB;
		private Label labelT;
		private GroupBox groupBoxPrintPDF;		
   }
}