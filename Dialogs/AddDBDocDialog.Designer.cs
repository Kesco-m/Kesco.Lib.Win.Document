using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Blocks;
using Kesco.Lib.Win.Document.Controls;

namespace Kesco.Lib.Win.Document.Dialogs
{
	partial class AddDBDocDialog : Lib.Win.FreeDialog
	{

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddDBDocDialog));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.number = new System.Windows.Forms.TextBox();
			this.groupDocument = new System.Windows.Forms.GroupBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.checkBoxName = new System.Windows.Forms.CheckBox();
			this.checkBoxNoNumber = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.buttonChange = new System.Windows.Forms.Button();
			this.description = new System.Windows.Forms.TextBox();
			this.groupPersons = new System.Windows.Forms.GroupBox();
			this.checkMain = new System.Windows.Forms.CheckBox();
			this.archiveLabel = new System.Windows.Forms.Label();
			this.buttonSelectArchive = new System.Windows.Forms.Button();
			this.labelScanDate = new System.Windows.Forms.Label();
			this.groupImage = new System.Windows.Forms.GroupBox();
			this.archive = new System.Windows.Forms.TextBox();
			this.original = new System.Windows.Forms.CheckBox();
			this.checkAddToWork = new System.Windows.Forms.CheckBox();
			this.checkSendMessage = new System.Windows.Forms.CheckBox();
			this.groupDescription = new System.Windows.Forms.GroupBox();
			this.checkTemplate = new System.Windows.Forms.CheckBox();
			this.checkGotoDoc = new System.Windows.Forms.CheckBox();
			this.groupLinks = new System.Windows.Forms.GroupBox();
			this.checkBoxProtected = new System.Windows.Forms.CheckBox();
			this.contextMenuOpenDocs = new System.Windows.Forms.ContextMenu();
			this.checkBoxOpenDoc = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.newWindowDocumentButton = new Kesco.Lib.Win.Document.Controls.NewWindowDocumentButton(this.components);
			this.docLinksBlock = new Kesco.Lib.Win.Document.Blocks.DocLinksBlock();
			this.personBlock = new Kesco.Lib.Win.Document.Blocks.PersonBlock();
			this.dateBlock = new Kesco.Lib.Win.Document.Blocks.DateBlock();
			this.docTypeBlock = new Kesco.Lib.Win.Document.Blocks.DocTypeBlock();
			this.groupDocument.SuspendLayout();
			this.groupPersons.SuspendLayout();
			this.groupImage.SuspendLayout();
			this.groupDescription.SuspendLayout();
			this.groupLinks.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// number
			// 
			resources.ApplyResources(this.number, "number");
			this.number.Name = "number";
			// 
			// groupDocument
			// 
			resources.ApplyResources(this.groupDocument, "groupDocument");
			this.groupDocument.Controls.Add(this.textBox1);
			this.groupDocument.Controls.Add(this.checkBoxName);
			this.groupDocument.Controls.Add(this.checkBoxNoNumber);
			this.groupDocument.Controls.Add(this.dateBlock);
			this.groupDocument.Controls.Add(this.number);
			this.groupDocument.Controls.Add(this.label1);
			this.groupDocument.Controls.Add(this.label2);
			this.groupDocument.Controls.Add(this.label4);
			this.groupDocument.Controls.Add(this.buttonChange);
			this.groupDocument.Controls.Add(this.docTypeBlock);
			this.groupDocument.Name = "groupDocument";
			this.groupDocument.TabStop = false;
			// 
			// textBox1
			// 
			resources.ApplyResources(this.textBox1, "textBox1");
			this.textBox1.AutoCompleteCustomSource.AddRange(new string[] {
            resources.GetString("textBox1.AutoCompleteCustomSource")});
			this.textBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.textBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.textBox1.Name = "textBox1";
			this.toolTip1.SetToolTip(this.textBox1, resources.GetString("textBox1.ToolTip"));
			// 
			// checkBoxName
			// 
			resources.ApplyResources(this.checkBoxName, "checkBoxName");
			this.checkBoxName.Name = "checkBoxName";
			this.toolTip1.SetToolTip(this.checkBoxName, resources.GetString("checkBoxName.ToolTip"));
			this.checkBoxName.UseVisualStyleBackColor = true;
			this.checkBoxName.CheckedChanged += new System.EventHandler(this.checkBoxName_CheckedChanged);
			// 
			// checkBoxNoNumber
			// 
			resources.ApplyResources(this.checkBoxNoNumber, "checkBoxNoNumber");
			this.checkBoxNoNumber.Name = "checkBoxNoNumber";
			this.checkBoxNoNumber.UseVisualStyleBackColor = true;
			this.checkBoxNoNumber.CheckedChanged += new System.EventHandler(this.checkBoxNoNumber_CheckedChanged);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// buttonChange
			// 
			resources.ApplyResources(this.buttonChange, "buttonChange");
			this.buttonChange.Name = "buttonChange";
			this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
			// 
			// description
			// 
			resources.ApplyResources(this.description, "description");
			this.description.Name = "description";
			// 
			// groupPersons
			// 
			resources.ApplyResources(this.groupPersons, "groupPersons");
			this.groupPersons.Controls.Add(this.personBlock);
			this.groupPersons.Name = "groupPersons";
			this.groupPersons.TabStop = false;
			// 
			// checkMain
			// 
			resources.ApplyResources(this.checkMain, "checkMain");
			this.checkMain.Checked = true;
			this.checkMain.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkMain.Name = "checkMain";
			// 
			// archiveLabel
			// 
			resources.ApplyResources(this.archiveLabel, "archiveLabel");
			this.archiveLabel.Name = "archiveLabel";
			// 
			// buttonSelectArchive
			// 
			resources.ApplyResources(this.buttonSelectArchive, "buttonSelectArchive");
			this.buttonSelectArchive.Name = "buttonSelectArchive";
			this.buttonSelectArchive.Click += new System.EventHandler(this.buttonSelectArchive_Click);
			// 
			// labelScanDate
			// 
			resources.ApplyResources(this.labelScanDate, "labelScanDate");
			this.labelScanDate.Name = "labelScanDate";
			// 
			// groupImage
			// 
			resources.ApplyResources(this.groupImage, "groupImage");
			this.groupImage.Controls.Add(this.labelScanDate);
			this.groupImage.Controls.Add(this.archiveLabel);
			this.groupImage.Controls.Add(this.buttonSelectArchive);
			this.groupImage.Controls.Add(this.archive);
			this.groupImage.Controls.Add(this.original);
			this.groupImage.Name = "groupImage";
			this.groupImage.TabStop = false;
			// 
			// archive
			// 
			resources.ApplyResources(this.archive, "archive");
			this.archive.Name = "archive";
			this.archive.ReadOnly = true;
			this.archive.TabStop = false;
			// 
			// original
			// 
			resources.ApplyResources(this.original, "original");
			this.original.Name = "original";
			this.original.CheckedChanged += new System.EventHandler(this.original_CheckedChanged);
			// 
			// checkAddToWork
			// 
			resources.ApplyResources(this.checkAddToWork, "checkAddToWork");
			this.checkAddToWork.Name = "checkAddToWork";
			this.checkAddToWork.CheckedChanged += new System.EventHandler(this.checkAddToWork_CheckedChanged);
			// 
			// checkSendMessage
			// 
			resources.ApplyResources(this.checkSendMessage, "checkSendMessage");
			this.checkSendMessage.Checked = true;
			this.checkSendMessage.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkSendMessage.Name = "checkSendMessage";
			// 
			// groupDescription
			// 
			resources.ApplyResources(this.groupDescription, "groupDescription");
			this.groupDescription.Controls.Add(this.description);
			this.groupDescription.Name = "groupDescription";
			this.groupDescription.TabStop = false;
			// 
			// checkTemplate
			// 
			resources.ApplyResources(this.checkTemplate, "checkTemplate");
			this.checkTemplate.Name = "checkTemplate";
			// 
			// checkGotoDoc
			// 
			resources.ApplyResources(this.checkGotoDoc, "checkGotoDoc");
			this.checkGotoDoc.Name = "checkGotoDoc";
			// 
			// groupLinks
			// 
			resources.ApplyResources(this.groupLinks, "groupLinks");
			this.groupLinks.Controls.Add(this.docLinksBlock);
			this.groupLinks.Name = "groupLinks";
			this.groupLinks.TabStop = false;
			// 
			// checkBoxProtected
			// 
			resources.ApplyResources(this.checkBoxProtected, "checkBoxProtected");
			this.checkBoxProtected.Name = "checkBoxProtected";
			this.checkBoxProtected.CheckedChanged += new System.EventHandler(this.checkBoxProtected_CheckedChanged);
			// 
			// contextMenuOpenDocs
			// 
			resources.ApplyResources(this.contextMenuOpenDocs, "contextMenuOpenDocs");
			// 
			// checkBoxOpenDoc
			// 
			resources.ApplyResources(this.checkBoxOpenDoc, "checkBoxOpenDoc");
			this.checkBoxOpenDoc.Checked = true;
			this.checkBoxOpenDoc.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxOpenDoc.Name = "checkBoxOpenDoc";
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
			this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
			// 
			// newWindowDocumentButton
			// 
			resources.ApplyResources(this.newWindowDocumentButton, "newWindowDocumentButton");
			this.newWindowDocumentButton.Name = "newWindowDocumentButton";
			this.newWindowDocumentButton.TabStop = false;
			this.newWindowDocumentButton.UseMnemonic = false;
			this.newWindowDocumentButton.UseVisualStyleBackColor = true;
			// 
			// docLinksBlock
			// 
			resources.ApplyResources(this.docLinksBlock, "docLinksBlock");
			this.docLinksBlock.DocDate = new System.DateTime(((long)(0)));
			this.docLinksBlock.Name = "docLinksBlock";
			this.docLinksBlock.PersonIDs = null;
			// 
			// personBlock
			// 
			this.personBlock.Able = true;
			resources.ApplyResources(this.personBlock, "personBlock");
			this.personBlock.Name = "personBlock";
			this.personBlock.CountChanged += new System.EventHandler(this.personBlock_CountChanged);
			// 
			// dateBlock
			// 
			resources.ApplyResources(this.dateBlock, "dateBlock");
			this.dateBlock.Name = "dateBlock";
			this.dateBlock.Load += new System.EventHandler(this.buttonChange_Click);
			this.dateBlock.Leave += new System.EventHandler(this.dateBlock_Leave);
			// 
			// docTypeBlock
			// 
			resources.ApplyResources(this.docTypeBlock, "docTypeBlock");
			this.docTypeBlock.ID = 0;
			this.docTypeBlock.Name = "docTypeBlock";
			this.docTypeBlock.Selected += new Kesco.Lib.Win.Document.Blocks.BlockEventHandler(this.docTypeBlock_Selected);
			this.docTypeBlock.DocTypeTextChanged += new System.EventHandler(this.docTypeBlock_DocTypeTextChanged);
			// 
			// AddDBDocDialog
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.newWindowDocumentButton);
			this.Controls.Add(this.checkBoxProtected);
			this.Controls.Add(this.checkMain);
			this.Controls.Add(this.checkSendMessage);
			this.Controls.Add(this.checkBoxOpenDoc);
			this.Controls.Add(this.groupLinks);
			this.Controls.Add(this.groupDescription);
			this.Controls.Add(this.checkAddToWork);
			this.Controls.Add(this.groupImage);
			this.Controls.Add(this.groupPersons);
			this.Controls.Add(this.groupDocument);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.checkTemplate);
			this.Controls.Add(this.checkGotoDoc);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddDBDocDialog";
			this.Load += new System.EventHandler(this.DocProperties_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AddDBDocDialog_KeyDown);
			this.groupDocument.ResumeLayout(false);
			this.groupDocument.PerformLayout();
			this.groupPersons.ResumeLayout(false);
			this.groupImage.ResumeLayout(false);
			this.groupImage.PerformLayout();
			this.groupDescription.ResumeLayout(false);
			this.groupDescription.PerformLayout();
			this.groupLinks.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Clean up any resources being used.
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

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox number;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox description;
		private System.Windows.Forms.CheckBox checkMain;
		private System.Windows.Forms.Label archiveLabel;
		private System.Windows.Forms.Button buttonSelectArchive;
		private System.Windows.Forms.GroupBox groupImage;
		private System.Windows.Forms.Label labelScanDate;
		private System.Windows.Forms.CheckBox checkAddToWork;
		private System.Windows.Forms.CheckBox checkSendMessage;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox original;
		private System.Windows.Forms.TextBox archive;
		private System.Windows.Forms.GroupBox groupDocument;
		private System.Windows.Forms.GroupBox groupPersons;
		private System.Windows.Forms.GroupBox groupDescription;
		private System.Windows.Forms.CheckBox checkTemplate;
		private PersonBlock personBlock;
		private System.Windows.Forms.CheckBox checkGotoDoc;
		private DocTypeBlock docTypeBlock;
		private System.Windows.Forms.Button buttonChange;
		private System.Windows.Forms.GroupBox groupLinks;
		private DocLinksBlock docLinksBlock;
		private System.Windows.Forms.CheckBox checkBoxProtected;
		private System.Windows.Forms.ContextMenu contextMenuOpenDocs;
		private System.Windows.Forms.CheckBox checkBoxOpenDoc;
		private DateBlock dateBlock;
        private System.Windows.Forms.CheckBox checkBoxNoNumber;
        private System.Windows.Forms.CheckBox checkBoxName;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolTip toolTip1;
		private System.ComponentModel.IContainer components;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private NewWindowDocumentButton newWindowDocumentButton;
	}
}