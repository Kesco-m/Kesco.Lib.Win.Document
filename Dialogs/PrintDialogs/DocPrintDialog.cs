using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Checkers;

namespace Kesco.Lib.Win.Document.Dialogs
{
	/// <summary>
	/// Диалог настройки печати
	/// </summary>
	public partial class DocPrintDialog : FreeDialog
	{
		private int countPages;

		private Options.Folder options; // все настройки

		private string printer;
		private string defPrn;

		private bool duplex;
		public bool PdfMode;
		private bool printImage;
		private int _ttIndex = -1; // индекс в списке типов эл. форм, на котором мышь

        private int _checkedCnt = 0;
        private bool _dialogActivated = false;

		#region Sorters

		public class SorterClass : IComparer
		{
			private static int col = -1;
			private static int direct;

			public SorterClass(ListView listView, int col)
			{
				direct = col == SorterClass.col ? (direct == 1 ? -1 : 1) : 1;
				SorterClass.col = col;
			}

			public int Compare(object x, object y)
			{
				var item1 = (ListViewItem)x;
				var item2 = (ListViewItem)y;

				int result = string.Compare(item1.SubItems[col].Text, item2.SubItems[col].Text);
				if(result == 0)
					result = string.Compare(item1.SubItems[0].Text, item2.SubItems[0].Text);
				return result < 0 ? -1 * direct : (result > 0 ? direct : 0);
			}
		}

		public class DoubleSorterClass : IComparer
		{
			private int col1;
			private int col2 = 1;

			public DoubleSorterClass(int col1, int col2)
			{
				this.col1 = col1;
				this.col2 = col2;
			}

			public int Compare(object x, object y)
			{
				var item1 = (ListViewItem)x;
				var item2 = (ListViewItem)y;

				int result = string.Compare(item1.SubItems[col1].Text, item2.SubItems[col1].Text);

				if(result < 0)
					return -1;
				if(result > 0)
					return 1;
				result = string.Compare(item1.SubItems[col2].Text, item2.SubItems[col2].Text);

				return result < 0 ? -1 : (result > 0 ? 1 : 0);
			}
		}

		#endregion

		public DocPrintDialog(int typeID, Options.Folder opts, int docID, string docString, bool isPdfMode)
			: this(typeID, opts, docID, docString)
		{
			PdfMode = isPdfMode;
			annotationCheck.Enabled = !PdfMode;
			annotationCheck.Checked &= (!PdfMode);
		}

		public DocPrintDialog(int typeID, Options.Folder opts, int docID, string docString)
			: this(docID, 0, "", 1, 1, 1, opts, docString)
		{
			DocTypeID = typeID;
			panelDoc.Visible = true;
			Height += panelDoc.Height;

			if(typeID > 0)
			{
				WebMode = true;
				using(DataTable dt = Environment.PrintData.GetEFormPrintTypeData(typeID, docID))
				{
					Data.DALC.Documents.SettingsPrintForm[] settings = Environment.SettingsPrintForm.GetSettings(typeID);
					bool isOneItemSelect = false;

					if(dt.Rows.Count > 0)
					{
						foreach(DataRow dr in dt.Rows)
						{
							var po = new PrinterObjectClass((int)dr[Environment.PrintData.IDField],
															dr[Environment.PrintData.NameField].ToString(),
															dr[Environment.PrintData.URL].ToString(),
															Convert.ToInt32(dr[Environment.PrintData.PrintIDField]),
															(short)dr[Environment.PrintData.PaperSizeField]);
							bool isAdd = false;
							if(settings.Any(t => t.PrintID == po.TypeID && t.TypeID == DocTypeID))
							{
								typeBox.Items.Add(po, CheckState.Checked);
								isOneItemSelect = true;
								isAdd = true;
							}
							if(!isAdd)
								typeBox.Items.Add(po, CheckState.Unchecked);
						}
						ChangeStateControl(settings.Length < 2);
						if(!isOneItemSelect)
							typeBox.SetItemChecked(0, true);
					}
					else
					{
						Close();
						throw new Exception(Environment.StringResources.GetString("Dialog_DocPrintDialog_Error1"));
					}
					dt.Dispose();
				}
				typeBox.SelectedIndexChanged += typeBox_SelectedIndexChanged;
				typeBox.ItemCheck += typeBox_ItemCheck;
			}

			if(typeBox.Items.Count > 0)
			{
				typeBox.Visible = PrintEForm;
			}

			LoadPrinters();
		}

		public DocPrintDialog(int typeID, Options.Folder opts, int[ ] docIDs, string docString)
			: this(0, 0, "", 1, 1, 1, opts, docString)
		{
			newWindowDocumentButton.Set(docIDs);

			DocTypeID = typeID;
			panelDoc.Visible = true;
			Height += panelDoc.Height;

			if(typeID > 0)
			{
				WebMode = true;
				using(DataTable dt = Environment.PrintData.GetEFormPrintTypeData(typeID, DocID))
				{
					if(dt.Rows.Count > 0)
					{
						Data.DALC.Documents.SettingsPrintForm[ ] settings =
							Environment.SettingsPrintForm.GetSettings(typeID);
						bool isOneItemSelect = false;
						foreach(DataRow dr in dt.Rows)
						{
							var po = new PrinterObjectClass((int)dr[Environment.PrintData.IDField],
															dr[Environment.PrintData.NameField].ToString(),
															dr[Environment.PrintData.URL].ToString(),
															Convert.ToInt32(dr[Environment.PrintData.PrintIDField]),
															(short)dr[Environment.PrintData.PaperSizeField]);
							bool isAdd = false;
							if(settings.Any(t => t.PrintID == po.TypeID && t.TypeID == DocTypeID))
							{
								typeBox.Items.Add(po, CheckState.Checked);
								isOneItemSelect = true;
								isAdd = true;
							}
							if(!isAdd)
								typeBox.Items.Add(po, CheckState.Unchecked);
						}
						ChangeStateControl(settings.Length < 2);
						if(!isOneItemSelect)
							typeBox.SetItemChecked(0, true);
					}
					else
					{
						Close();
						throw new Exception(Environment.StringResources.GetString("Dialog_DocPrintDialog_Error1"));
					}
					dt.Dispose();
				}
				typeBox.SelectedIndexChanged += typeBox_SelectedIndexChanged;
				typeBox.ItemCheck += typeBox_ItemCheck;
			}

			if(typeBox.Items.Count > 0)
			{
				typeBox.Visible = PrintEForm;
			}

			LoadPrinters();
		}

		public DocPrintDialog(int docID, int imageID, string fileName, int startPage, int endPage, int countPage, Options.Folder opts, string docString, short counCopies, bool isPdfMode)
			: this(docID, imageID, fileName, startPage, endPage, countPage, opts, docString, counCopies)
		{
			PdfMode = isPdfMode;
			groupBoxPrintPDF.Visible = isPdfMode;
			annotationCheck.Enabled = !PdfMode;
			annotationCheck.Checked &= (!PdfMode);
		}

		public DocPrintDialog(int docID, int imageID, string fileName, int startPage, int endPage, int countPage,  Options.Folder opts, string docString, short counCopies)
			: this(docID, imageID, fileName, startPage, endPage, countPage, opts, docString)
		{
			CopiesCount = counCopies;
		}

		public DocPrintDialog(int docID, int imageID, string fileName, int startPage, int endPage, int countPage, Options.Folder opts, short countCopies, bool isPdfMode) //ref PDFView _pdf)
			: this(docID, imageID, fileName, startPage, endPage, countPage, opts, countCopies)
		{
			PdfMode = isPdfMode;
			groupBoxPrintPDF.Visible = isPdfMode;
			annotationCheck.Enabled = !PdfMode;
			annotationCheck.Checked &= (!PdfMode);
		}

		public DocPrintDialog(int docID, int imageID, string fileName, int startPage, int endPage, int countPage, Options.Folder opts, short countCopies)
			: this(docID, imageID, fileName, startPage, endPage, countPage, opts, null, countCopies)
		{
		}

		public DocPrintDialog(Options.Folder opts, int docID, string docString)
			: this(docID, 0, "", 0, 0, 0, opts, docString)
		{
			WebMode = true;

			panelImage1.Visible = true;
			panelImage1.Enabled = true;
			groupBox2.Visible = true;
			groupBox3.Visible = false;
			autoRButton.Visible = false;
			annotationCheck.Visible = false;
			panelPrint.Visible = false;
			checkBoxPrintBarCode.Visible = false;
			labelEForm.Visible = true;

			object obj = Environment.DocData.GetField(Environment.DocData.DocTypeIDField, docID);
			if(obj != null)
				DocTypeID = (int)obj;
			else
				Close();

			if(DocTypeID != 0)
			{
				using(DataTable dt = Environment.PrintData.GetEFormPrintTypeData(DocTypeID, docID))
				{
					if(dt.Rows.Count > 0)
					{
						Data.DALC.Documents.SettingsPrintForm[ ] settings = Environment.SettingsPrintForm.GetSettings(DocTypeID);
						bool isOneItemSelect = false;
						foreach(DataRow dr in dt.Rows)
						{
							var po = new PrinterObjectClass((int)dr[Environment.PrintData.IDField],
															dr[Environment.PrintData.NameField].ToString(),
															dr[Environment.PrintData.URL].ToString(),
															Convert.ToInt32(dr[Environment.PrintData.PrintIDField]),
															(short)dr[Environment.PrintData.PaperSizeField]);

							bool isAdd = false;
							if(settings.Any(t => t.PrintID == po.TypeID && t.TypeID == DocTypeID))
							{
								typeBox.Items.Add(po, CheckState.Checked);
								isOneItemSelect = true;
								isAdd = true;
							}
							if(!isAdd)
								typeBox.Items.Add(po, CheckState.Unchecked);
						}
						ChangeStateControl(settings.Length < 2);
						if(!isOneItemSelect)
							typeBox.SetItemChecked(0, true);
					}
					else
					{
						Close();
						throw new Exception(Environment.StringResources.GetString("Dialog_DocPrintDialog_Error1"));
					}
					dt.Dispose();
				}
				typeBox.ItemCheck += typeBox_ItemCheck;
				typeBox.SelectedIndexChanged += typeBox_SelectedIndexChanged;
			}
			else
			{
				Close();
				throw new Exception(Environment.StringResources.GetString("Dialog_DocPrintDialog_Error1"));
			}

			if(typeBox.Items.Count > 0)
			{
				typeBox.Visible = true;
				typeBox.Top = panelImage2.Top + 8;
			}

			LoadPrinters();
		}

		public DocPrintDialog(int docID, int imageID, string fileName, bool isPdfMode, int startPage, int endPage, int countPage, Options.Folder opts, string docString)
			: this(docID, imageID, fileName, startPage, endPage, countPage, opts, docString)
		{
			PdfMode = isPdfMode;
			groupBoxPrintPDF.Visible = isPdfMode;
			annotationCheck.Enabled = !PdfMode;
			annotationCheck.Checked &= (!PdfMode);
		}

		public DocPrintDialog(int docID, int imageID, string fileName, int startPage, int endPage, int countPage, Options.Folder opts, string docString)
		{
			PrinterObjectsList = new List<PrinterObjectClass>();
			CopiesCount = 1;
			DocID = docID;
			ImageID = imageID;
			WebMode = false;
			StartPage = startPage;
			EndPage = endPage;
			countPages = countPage;

			options = opts;
			printer = null;

			InitializeComponent();

			rbAllPages.Checked = true;
			if(startPage >= 1 && startPage == endPage)
			{
				rbAllPages.Checked = false;
				rbSelectedPages.Checked = true;
			}
			if(fileName != "" && startPage > 0 && endPage > 0 && countPage > 0)
			{
				textBoxStartPage.Minimum = 1;
				textBoxEndPage.Minimum = 1;
				textBoxStartPage.Maximum = countPage;
				textBoxEndPage.Maximum = countPage;
				textBoxStartPage.Value = startPage;
				textBoxEndPage.Value = endPage;
				textBoxStartPage.ValueChanged += textBoxPage_ValueChanged;
				textBoxEndPage.ValueChanged += textBoxPage_ValueChanged;
				textBoxEndPage.Validating += textBoxEndPage_Validating;
			}
			else
			{
				textBoxStartPage.Minimum = 1;
				textBoxEndPage.Minimum = 1;
				panelPrint.Visible = false;
				Height -= panelPrint.Height;
			}

			if(docString != null)
				doc.Text = docString;

			FileName = fileName;

			printers.CheckBoxes = true; // используем чекбоксы
			printers.Columns.Add(Environment.StringResources.GetString("DefaultPrinter"), 130, HorizontalAlignment.Left);
			printers.Columns.Add(Environment.StringResources.GetString("Name"), 120, HorizontalAlignment.Left);
			printers.Columns.Add(Environment.StringResources.GetString("Place"), 200, HorizontalAlignment.Left);
			printers.Columns.Add(Environment.StringResources.GetString("Comment"), 100, HorizontalAlignment.Left);

			printers.Columns[0].DisplayIndex = printers.Columns.Count - 1;  // откидываем чекбоксы последним столбцом
			printers.Invalidate();

			if(!Environment.Printers.LoadComplete && !Environment.Printers.LoadStarted)
				Environment.Printers.Load();

			Environment.Printers.Updated += prnList_Updated;
			Environment.Printers.Loaded += prnList_Loaded;

			LoadPrinters();

			panelDoc.Visible = false;
			Height -= panelDoc.Height;

			// ориентация
			string opt = options.LoadStringOption("Orientation", PrintOrientation.Auto.ToString());
			switch(opt)
			{
				case "Book":
					bookRButton.Checked = true;
					break;

				case "Album":
					albumRButton.Checked = true;
					break;

				case "Auto":
					autoRButton.Checked = true;
					break;
			}

			// масштаб
			opt = options.LoadStringOption("ScaleMode", PrintScale.InchToInch.ToString());
			switch(opt)
			{
				case "InchToInch":
					originalRButton.Checked = true;
					break;

				case "FitToPage":
					fitRButton.Checked = true;
					break;
			}

			//if(PdfMode)
			comboBoxPrintResolution.SelectedIndex = options.LoadIntOption("PrintResolution", 0);

			////поля
			//object opt = options.LoadStringOption("Margin", PrintOrientation.Auto.ToString());
			//switch(opt as string)
			//{
			//    case "Book":
			//        bookRButton.Checked = true;
			//        break;

			//    case "Album":
			//        albumRButton.Checked = true;
			//        break;

			//    case "Auto":
			//        autoRButton.Checked = true;
			//        break;
			//}

			// печатать ли пометки
			opt = options.LoadStringOption("Annotations", annotationCheck.Checked.ToString());
			annotationCheck.Checked = Convert.ToBoolean(opt);
			Annotations = annotationCheck.Checked;

			// печатать ли штирхкод
			opt = options.LoadStringOption("PrintBarcode", checkBoxPrintBarCode.Checked.ToString());
			checkBoxPrintBarCode.Checked = Convert.ToBoolean(opt);
			PrintBarCode = checkBoxPrintBarCode.Checked;

			// всегда ли отображать
			opt = options.LoadStringOption("AlwaysShow", showCheck.Checked.ToString());
			showCheck.Checked = Convert.ToBoolean(opt);
			AlwaysShow = showCheck.Checked;

			// двухсторонняя печать
			opt = options.LoadStringOption("Duplex", duplexBox.Checked.ToString());
			duplexBox.Checked = Convert.ToBoolean(opt);
			duplex = duplexBox.Checked;

			// печатать изображения
			opt = options.LoadStringOption("PrintImage", checkPrintImage.Checked.ToString());
			checkPrintImage.Checked = Convert.ToBoolean(opt);
			printImage = checkPrintImage.Checked;
			radioButtonOnlyMain.Enabled = checkPrintImage.Checked;
			radioButtonAll.Enabled = checkPrintImage.Checked;

			// печатать изображения (все / только основное)
			opt = options.LoadStringOption("PrintOnlyMain", radioButtonOnlyMain.Checked.ToString());
			radioButtonOnlyMain.Checked = Convert.ToBoolean(opt);
			radioButtonAll.Checked = !radioButtonOnlyMain.Checked;
			PrintOnlyMain = radioButtonOnlyMain.Checked;

			// печать изображения (все страницы / диапазон)
			opt = options.LoadStringOption("PrintSelectedPages", rbSelectedPages.Checked.ToString());
			rbSelectedPages.Checked = Convert.ToBoolean(opt);
			rbAllPages.Checked = !rbSelectedPages.Checked;
			PrintSelectedPages = rbSelectedPages.Checked;

			PrintEForm = checkBoxEform.Checked;
		}

		#region Accessors

		public static string Printer { get; private set; }

		public PrintOrientation Orientation { get; private set; } // ориентация страницы

		public PrintScale ScaleMode { get; private set; } // масштаб печати (два режима)

		public int StartPage { get; private set; }

		public int EndPage { get; private set; }

		public bool Annotations { get; private set; } // печатать ли аннотации

		public bool PrintBarCode { get; private set; } // Печать штрихкода

		public bool AlwaysShow { get; private set; } // показывать ли всегда это окно

		public bool WebMode { get; private set; }

		public int DocID { get; private set; } //Код документа

		public int ImageID { get; private set; } // Код изображения документа

		public int DocTypeID { get; private set; } //Код типа документа

		public bool PrintEForm { get; private set; }

		public bool PrintOnlyMain { get; private set; } // Печать изображения (основное / все)

		public bool PrintImage// Печатать изображения (чекбокс)
		{
			get { return printImage; }
			set
			{
				printImage = value;
				checkPrintImage.Checked = value;
			}
		}

        public bool PrintSelectedPages { get; private set; }

		public string FileName { get; private set; }

		public short CopiesCount { get; private set; } // Количество копий

		public List<PrinterObjectClass> PrinterObjectsList { get; private set; }

		#endregion

		private void LoadPrinters()
		{
			bRefresh.Enabled = false;

            if (printers.Items.Count > 0)
            {
                printers.Items.Clear();
                _lastItemChecked = null;
                _lastItemCheckedIndex = -1;
            }

            // принтер по умолчанию
            var prnSet = new PrinterSettings();
            if (prnSet.IsDefaultPrinter)
                defPrn = prnSet.PrinterName;

			var param = new string[4];

			// инициализация списка принтеров
			int selectedIndex = -1;
            int defaultIndex = -1;
			try
			{
				int n = 0;
				foreach(string prnName in PrinterSettings.InstalledPrinters)
				{
					int pos = prnName.LastIndexOf(@"\");
					param[1] = prnName.Substring(pos + 1);

					if(Environment.Printers.List.ContainsKey(prnName))
					{
						var prnInfo = Environment.Printers.List[prnName] as PrinterInfo;
						if(prnInfo != null)
						{
							param[2] = prnInfo.Location;
							param[3] = prnInfo.Comment;
						}
					}
					else
					{
						param[2] = "";
						param[3] = "";
					}

                    if (!WebMode)
                    {
                        if (param[1] != Environment.PrinterName && param[1] != "Печать в PDF")
                        {
                            printers.Items.Add(new ListViewItem(param) {Tag = prnName});
                            n++;
                        }
                    }
                    else
                    {
                        printers.Items.Add(new ListViewItem(param) {Tag = prnName});
                        n++;
                    }
                }

                var personPrinter = Environment.UserSettings.PersonPrinter;
                foreach (ListViewItem item in printers.Items)
                {
                    if (!string.IsNullOrEmpty(personPrinter))
                    {
                        if ((string)item.Tag == personPrinter)
                            selectedIndex = item.Index;
                        if ((string)item.Tag == defPrn)
                            defaultIndex = item.Index;
                    }
                    else
                    {
                        if ((string)item.Tag == defPrn)
                        {
                            defaultIndex = item.Index;
                            selectedIndex = item.Index;
                        }
                    }
                }

				if(printers.Items.Count > 0 && printers.SelectedItems.Count == 0)
				{
					bool isSelect = printers.Items.Cast<ListViewItem>().Any(item => item.Selected);
					foreach(ListViewItem item in printers.Items.Cast<ListViewItem>().Where(item => item.Selected))
					{
						printer = (string)item.Tag;
						break;
					}
					bOK.Enabled = isSelect;
					bProperties.Enabled = isSelect;
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			if(printers.Items.Count > 0)
			{
				if(printers.SelectedItems.Count == 0)
				{
					printers.HideSelection = false;
					printers.Focus();

                    if (selectedIndex >= 0)
                        printers.Items[selectedIndex].Selected = true;
                    if (defaultIndex >= 0)
                        printers.Items[defaultIndex].Checked = true;
                    if (selectedIndex < 0 && defaultIndex >= 0)
                        printers.Items[defaultIndex].Selected = true;
                    if (defaultIndex < 0 && selectedIndex < 0)
                        printers.Items[0].Selected = true;
                }
            }
            else
                MessageBox.Show(Environment.StringResources.GetString("Dialog_DocPrintDialog_LoadPrinters_Error1"));

			if(printers.ListViewItemSorter == null)
				printers.ListViewItemSorter = new SorterClass(printers, 0);
			printers.Sort();

			bRefresh.Enabled = Environment.Printers.LoadComplete;
		}

		public void SaveOptions()
		{
			if(!WebMode)
				options.Option("Orientation").Value = Orientation.ToString();
			if(PdfMode)
				options.Option("PrintResolution").Value = comboBoxPrintResolution.SelectedIndex;
			options.Option("ScaleMode").Value = ScaleMode.ToString();
			options.Option("Annotations").Value = annotationCheck.Checked.ToString();
			options.Option("PrintBarcode").Value = checkBoxPrintBarCode.Checked.ToString();
			options.Option("AlwaysShow").Value = AlwaysShow.ToString();
			options.Option("Duplex").Value = duplex.ToString();
            options.Option("PrintOnlyMain").Value = radioButtonOnlyMain.Checked.ToString();
            options.Option("PrintImage").Value = checkPrintImage.Checked.ToString();
            options.Option("PrintSelectedPages").Value = rbSelectedPages.Checked.ToString();

			options.Save();
		}

		#region Orientation

		private void bookRButton_CheckedChanged(object sender, EventArgs e)
		{
			if(bookRButton.Checked)
			{
				Orientation = PrintOrientation.Book;
				pictureBoxOrientation.Image = imageListOrintation.Images[0];
			}
		}

		private void albumRButton_CheckedChanged(object sender, EventArgs e)
		{
			if(albumRButton.Checked)
			{
				Orientation = PrintOrientation.Album;
				pictureBoxOrientation.Image = imageListOrintation.Images[1];
			}
		}

		private void autoRButton_CheckedChanged(object sender, EventArgs e)
		{
			if(autoRButton.Checked)
			{
				Orientation = PrintOrientation.Auto;
				pictureBoxOrientation.Image = imageListOrintation.Images[2];
			}
		}

		#endregion

		private void textBoxPage_ValueChanged(object sender, EventArgs e)
		{
			if(!rbSelectedPages.Checked)
				rbSelectedPages.Checked = true;

			var tempEndPage = (int)textBoxEndPage.Value;
			if(tempEndPage < textBoxStartPage.Value)
				textBoxEndPage.Value = textBoxStartPage.Value;
			StartPage = (int)textBoxStartPage.Value;
			EndPage = (int)textBoxEndPage.Value;
		}

		private void textBoxEndPage_Validating(object sender, CancelEventArgs e)
		{
			var tempEndPage = (int)textBoxEndPage.Value;
			if(tempEndPage < textBoxStartPage.Value)
				e.Cancel = true;
		}

		private void originalRButton_CheckedChanged(object sender, EventArgs e)
		{
			if(originalRButton.Checked)
				ScaleMode = PrintScale.InchToInch;
		}

		private void fitRButton_CheckedChanged(object sender, EventArgs e)
		{
			if(fitRButton.Checked)
				ScaleMode = PrintScale.FitToPage;
			//groupBoxMargins.Visible = fitRButton.Checked;
		}

		private void annotationCheck_CheckedChanged(object sender, EventArgs e)
		{
			Annotations = annotationCheck.Checked;
		}

		private void showCheck_CheckedChanged(object sender, EventArgs e)
		{
			AlwaysShow = showCheck.Checked;
		}

		private void DocPrintDialog_Activated(object sender, EventArgs e)
		{
			if(!WebMode)
			{
				for(int i = 0; i < printers.Items.Count; i++)
				{
					if(printers.Items[i].SubItems[1].Text == Environment.PrinterName || printers.Items[i].SubItems[1].Text == "Печать в PDF")
					{
						TestPrinter.SetPrinterProfile(ProfileType.TiffProfile);
						printers.Items.RemoveAt(i);
						break;
					}
				}
			}
			else if(!TestPrinter.CheckPrinterProfile())
				TestPrinter.SetPrinterProfile(ProfileType.TiffProfile);
			printers.Focus();
			if(printers.SelectedItems.Count > 0)
				printers.SelectedItems[0].EnsureVisible();

            _dialogActivated = true;
		}

		private void bProperties_Click(object sender, EventArgs e)
		{
			var prn = new PrinterOp(printer, this);
			prn.PrinterPropertiesPrintUI(printer);
		}

		private void duplexBox_CheckedChanged(object sender, EventArgs e)
		{
			duplex = duplexBox.Checked;
		}

		private void SaveSettingsPrintForms()
		{
			var settingsForRemove = new Data.DALC.Documents.SettingsPrintForm[typeBox.Items.Count - typeBox.CheckedItems.Count];
			var settingsForInsert = new Data.DALC.Documents.SettingsPrintForm[typeBox.CheckedItems.Count];
			int nr = 0;
			int ni = 0;
			for(int n = 0; n < typeBox.Items.Count; n++)
			{
				var po = typeBox.Items[n] as PrinterObjectClass;
				bool isChecked = typeBox.GetItemChecked(n);
				if(isChecked)
				{
					settingsForInsert[ni] = new Data.DALC.Documents.SettingsPrintForm(DocTypeID, po.TypeID);
					ni++;
				}
				else
				{
					settingsForRemove[nr] = new Data.DALC.Documents.SettingsPrintForm(DocTypeID, po.TypeID);
					nr++;
				}
			}
			if(settingsForRemove.Length > 0)
				Environment.SettingsPrintForm.DeleteSettings(settingsForRemove);
			if(settingsForInsert.Length > 0)
				Environment.SettingsPrintForm.SetSettings(settingsForInsert);
		}

		private void bOK_Click(object sender, EventArgs e)
		{
			End();
		}

		private void End()
		{
			SaveOptions();
			Printer = printer;
			CopiesCount = (short)numericUpDownCopy.Value;
			if(WebMode)
			{
				foreach(var po in from object item in typeBox.CheckedItems select item as PrinterObjectClass)
				{
					if(po.PrintID == 0)
					{
						if(albumRButton.Checked)
							po.PrintType = 3;
						else if(bookRButton.Checked)
							po.PrintType = 2;
					}
					else
						po.PrintType = po.PrintID;
					PrinterObjectsList.Add(po);
				}
			}

            // сохраняем принтер по умолчанию в базу
            Environment.UserSettings.PersonPrinter = printer;
            Environment.UserSettings.Save();

			if(Printer == Environment.PrinterName)
				Environment.SetPrinterDocParams(ImageID, FileName, PdfMode, StartPage, EndPage, countPages, doc.Text);

			SaveSettingsPrintForms();
			End(DialogResult.OK);
		}

		private void bCancel_Click(object sender, EventArgs e)
		{
			End(DialogResult.Cancel);
		}

		#region Print

		public void Print()
		{
			Print(false);
		}

		public void Print(bool force, int attempt = 0)
		{
			int resX = 0, resY = 0;
			string driverName = null;
			try
			{
				Visible = false;
				if(printer == null)
				{
					if(string.IsNullOrEmpty(Environment.UserSettings.PersonPrinter))
					{
						MessageBox.Show(Environment.StringResources.GetString("Dialog_DocPrintDialog_Print_Error1"),
										Environment.StringResources.GetString("Error"));
						return;
					}
					else
					{
						printer = Environment.UserSettings.PersonPrinter;
					}
				}

				// настроить печать...
				var printerInfo = Environment.Printers.List[printer] as PrinterInfo;
				if(printerInfo == null)
				{
					var prn = new PrinterOp(printer, this);
					printerInfo = prn.GetInfo();
				}

				if(!WebMode)
				{
					List<Tiff.PageInfo> images = null;
					resX = printerInfo.HorizontalResolution; resY = printerInfo.VerticalResolution;
					driverName = printerInfo.DriverName;
					if(PdfMode)
					{
						if(printerInfo.DriverName.Equals("Samsung") || attempt == 1 || printer.EndsWith("SlNPS") || printer.EndsWith("SlIC"))
						{
							if(resX > 200)
								resX = 200;
							if(resY > 200)
								resY = 200;
						}
						else
							switch(comboBoxPrintResolution.SelectedIndex)
							{
								case 1:
									if(resX > 600)
										resX = 600;
									if(resY > 600)
										resY = 600;
									break;
								case 2:
									if(resX > 300)
										resX = 300;
									if(resY > 300)
										resY = 300;
									break;
								case 3:
									if(resX > 200)
										resX = 200;
									if(resY > 200)
										resY = 200;
									break;
							}
					}
					if(!force && rbAllPages.Checked)
						force = true;
					if(ImageID > 0)
						images = LoadData(PdfMode, force ? 1 : StartPage, force ? countPages : EndPage, resX, resY);
					else
					{
						if(PdfMode)
						{
							images = new List<Tiff.PageInfo>();
							{
								var ph = new Classes.PDFHelper { UseLock = true };
								ph.Open(FileName, null);
								if(countPages < 1)
									countPages = ph.PageCount;
								for(int i = (force || StartPage < 1) ? 1 : StartPage;
									 i <= ((force || EndPage > countPages) ? countPages : EndPage);
									 i++)
									images.Add(new Classes.PrintImageInfo(i - 1, FileName, PdfMode, ph) { ResX = resX, ResY = resY });
							}
						}
						else
						{
							images = new List<Tiff.PageInfo>();
							{
								if(countPages < 1)
									countPages = Environment.LibTiff.GetCountPages(FileName);
								for(int i = (force || StartPage < 1) ? 1 : StartPage;
									 i <= ((force || EndPage > countPages) ? countPages : EndPage);
									 i++)
									images.Add(new Classes.PrintImageInfo(i - 1, FileName, PdfMode));
							}
						}
					}
					if(images != null && images.Count > 0)
					{
						var printImageInst = new Classes.PrintImage();
						printImageInst.EndPrint += printImageInst_EndPrint;
						printImageInst.PrintPage(images.ToArray(), null, force ? 1 : StartPage,
												 force ? images.Count : EndPage, (int)ScaleMode,
												 (ImageControl.PrintOrientation)(int)Orientation,
												 Annotations, CopiesCount, printerInfo.Name, printerInfo.DriverName,
												 printerInfo.PortName);
					}
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex, "Printer: " + printer + "\n resoluton: " + resX.ToString() + "x" + resY.ToString() + "/n driverName: " + driverName??"null");
				string mess = ex.Message;
				if(string.Compare(mess, "overflow error", true) == 0)
					mess = Environment.StringResources.GetString("Error") + " печати";

				if(attempt == 0)
					Print(force, 1);
				else
					MessageForm.Show(mess, Environment.StringResources.GetString("Error"));
			}
		}

		public List<Tiff.PageInfo> LoadData(bool pdfMode, int startPage, int endPage, int horizontalResolution, int verticalResolution)
		{
			var imageList = new List<Tiff.PageInfo>();
			if(Environment.GetServers().Count > 0)
			{
				List<int> serverIDs = Environment.DocImageData.GetLocalDocImageServers(ImageID, Environment.GetLocalServersString());
				if(serverIDs != null && serverIDs.Count > 0)
				{
					var rand = new Random();
					ServerInfo curServer = Environment.GetLocalServer(serverIDs[rand.Next(serverIDs.Count)]);
					string fileName = curServer.Path + "\\" + Environment.GetFileNameFromID(ImageID) + (pdfMode ? ".pdf" : ".tif");
					if(File.Exists(fileName))
					{
						if(endPage < 1)
						{
							if(pdfMode)
							{
								Environment.PDFHelper.Open(fileName, null);
								endPage = Environment.PDFHelper.PageCount;
								Environment.PDFHelper.Close();
							}
							else
								endPage = Environment.LibTiff.GetCountPages(fileName);
						}
					}
					else
					{
						bool err = true;
						if(serverIDs.Count > 0)
						{
							foreach(int id in serverIDs)
							{
								curServer = Environment.GetLocalServer(id);
								fileName = curServer.Path + "\\" + Environment.GetFileNameFromID(ImageID) + (pdfMode ? ".pdf" : ".tif");
								if(!File.Exists(fileName))
									continue;
								err = false;
								if(endPage < 1)
								{
									if(pdfMode)
									{
										Environment.PDFHelper.Open(fileName, null);
										endPage = Environment.PDFHelper.PageCount;
										Environment.PDFHelper.Close();
									}
									else
										endPage = Environment.LibTiff.GetCountPages(fileName);
								}
								break;
							}
						}
						if(err)
							return null;
					}

					List<Data.Temp.Objects.StampItem> stampItems = Environment.DocSignatureData.GetStamps(ImageID);
					bool isAnnuled = Environment.DocSignatureData.IsDocSignedAnnuled(ImageID);
					for(int i = startPage; i <= endPage; i++)
					{
						if(stampItems != null)
						{
							var pageStampItems = stampItems.FindAll(x => x.Page == i);
							if(pageStampItems.Count > 0)
							{
								if(pdfMode)
								{
									double diff = horizontalResolution / 96.0;
									foreach(Data.Temp.Objects.StampItem it in pageStampItems)
									{
										it.Zoom = (int)Math.Round(it.Zoom * diff);
										it.X = (int)Math.Round(it.X * diff);
										it.Y = (int)Math.Round(it.Y * diff);
									}
								}
								imageList.Add(new Classes.PrintImageInfo(i - 1, fileName, pdfMode, pageStampItems.ToArray()) { ResX = verticalResolution, ResY = horizontalResolution, IsAnnuled = isAnnuled });
								continue;
							}
							imageList.Add(new Classes.PrintImageInfo(i - 1, fileName, pdfMode) { ResX = verticalResolution, ResY = horizontalResolution, IsAnnuled = isAnnuled });
						}
						else
							imageList.Add(new Classes.PrintImageInfo(i - 1, fileName, pdfMode) { ResX = verticalResolution, ResY = horizontalResolution, IsAnnuled = isAnnuled });
					}
				}
			}
			return imageList;
		}

		#endregion

		#region Printer List

		private void prnList_Updated(object source, InfoArgs e)
		{
			if(InvokeRequired)
			{
				BeginInvoke(new PrinterList.InfoEventHandler(prnList_Updated), new[] {source, e});
			}
			else
				for(int i = 0; i < printers.Items.Count; i++)
				{
					var prnName = printers.Items[i].Tag as string;
					if(prnName == e.Info.Name)
					{
						printers.Items[i].SubItems[2].Text = e.Info.Location;
						printers.Items[i].SubItems[3].Text = e.Info.Comment;
						return;
					}
				}
		}

		private void prnList_Loaded(object source, EventArgs e)
		{
			Environment.Printers.Updated -= prnList_Updated;
			Environment.Printers.Loaded -= prnList_Loaded;
			if(InvokeRequired)
				BeginInvoke(new PrinterList.SimpleEventHandler(prnList_Loaded), new[] {source, e});
			else
				bRefresh.Enabled = true;
		}

		private void printers_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool selected = (printers.SelectedItems.Count > 0);
			printer = selected ? printers.SelectedItems[0].Tag as string : null;

			if(selected)
			{
				if(printer == Environment.PrinterName)
				{
				}
				using(PrintDocument pd = new PrintDocument())
				{
					pd.PrinterSettings.PrinterName = printer;
					var m = PrinterUnitConvert.Convert(pd.PrinterSettings.DefaultPageSettings.Margins,PrinterUnit.ThousandthsOfAnInch,PrinterUnit.TenthsOfAMillimeter);
					var x = PrinterUnitConvert.Convert(pd.PrinterSettings.DefaultPageSettings.HardMarginX, PrinterUnit.ThousandthsOfAnInch, PrinterUnit.TenthsOfAMillimeter);
					var y = PrinterUnitConvert.Convert(pd.PrinterSettings.DefaultPageSettings.HardMarginY, PrinterUnit.ThousandthsOfAnInch, PrinterUnit.TenthsOfAMillimeter);
					if(pd.PrinterSettings.IsValid)
					{
						numMarginTop.Text = (m.Top / 100.0).ToString();
						numMarginBottom.Text = (m.Bottom / 100.0).ToString();
						numMarginLeft.Text = (m.Left / 100.0).ToString();
						numMarginRight.Text = (m.Right / 100.0).ToString();
					}
				}
			}
			bOK.Enabled = selected;
			bProperties.Enabled = selected;
		}

		private void printers_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			printers.ListViewItemSorter = new SorterClass(printers, e.Column);
			printers.Sort();
		}

		private void bRefresh_Click(object sender, EventArgs e)
		{
			bRefresh.Enabled = false;

            var prnSet = new PrinterSettings();
            if (printer != null && prnSet.IsDefaultPrinter && prnSet.PrinterName == printer)
                defPrn = printer;

			if(!Environment.Printers.LoadStarted)
			{
				Environment.Printers.Updated -= prnList_Updated;
				Environment.Printers.Loaded -= prnList_Loaded;
				Environment.Printers.Updated += prnList_Updated;
				Environment.Printers.Loaded += prnList_Loaded;
				Environment.Printers.Load();
			}

			LoadPrinters();
		}

		#endregion

		private void checkBoxPrintImage_CheckedChanged(object sender, EventArgs e)
		{
			printImage = checkPrintImage.Checked;
			radioButtonOnlyMain.Enabled = printImage;
			radioButtonAll.Enabled = printImage;
			annotationCheck.Enabled = printImage;
			checkBoxPrintBarCode.Enabled = printImage;
		}

		private void checkBoxEform_CheckedChanged(object sender, EventArgs e)
		{
			PrintEForm = checkBoxEform.Checked;
			typeBox.Visible = WebMode && PrintEForm;
		}

		private void radioButtonOnlyMain_CheckedChanged(object sender, EventArgs e)
		{
			PrintOnlyMain = radioButtonOnlyMain.Checked;
		}

		private void typeBox_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if(typeBox.CheckedItems.Count == 0)
				e.NewValue = CheckState.Checked;
			else if(typeBox.CheckedItems.Count == 1 && e.NewValue != CheckState.Checked)
				e.NewValue = CheckState.Checked;

			ChangeStateControl(typeBox.CheckedItems.Count < 3 && (typeBox.CheckedItems.Count < 1 || e.CurrentValue == CheckState.Checked || e.NewValue != CheckState.Checked));
		}

		private object lastSelectedItem;

		private void typeBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(typeBox.CheckedItems.Count == 0)
			{
				typeBox.SetItemChecked(typeBox.SelectedIndex, true);
				ChangeStateControl(typeBox.CheckedItems.Count == 1);
			}
			else if(lastSelectedItem != typeBox.SelectedItem)
			{
				bool isChecked = false;
				if(typeBox.CheckedItems.Count > 0)
					foreach(object selItem in typeBox.CheckedItems)
					{
						if(selItem == typeBox.SelectedItem)
							isChecked = true;
					}

				typeBox.SetItemChecked(typeBox.SelectedIndex, !isChecked);
			}
			lastSelectedItem = typeBox.SelectedItem;
		}

		private void ChangeStateControl(bool isSelectedOne)
		{
			autoRButton.Visible = true;
			if(isSelectedOne && typeBox.Items.Count > 0)
			{
				object selItem = typeBox.CheckedItems.Count == 0 ? typeBox.Items[0] : typeBox.CheckedItems[0];
				if(selItem is PrinterObjectClass)
				{
					var po = selItem as PrinterObjectClass;
					switch(po.PrintID)
					{
						case 0:
							groupBox2.Enabled = true;
							autoRButton.Checked = true;
							numericUpDownCopy.Value = 1;
							break;
						case 1:
							groupBox2.Enabled = false;
							autoRButton.Checked = true;
							numericUpDownCopy.Value = 1;
							break;
						case 2:
							groupBox2.Enabled = false;
							bookRButton.Checked = true;
							numericUpDownCopy.Value = 1;
							break;
						case 3:
							groupBox2.Enabled = false;
							albumRButton.Checked = true;
							numericUpDownCopy.Value = 1;
							break;
					}
				}
				else
				{
					autoRButton.Visible = false;
					groupBox2.Enabled = false;
				}
			}
			else
			{
				groupBox2.Enabled = false;
				autoRButton.Checked = true;
			}
		}

		private void checkBoxPrintBarCode_CheckedChanged(object sender, EventArgs e)
		{
			PrintBarCode = checkBoxPrintBarCode.Checked;
		}

		private void DocPrintDialog_Load(object sender, EventArgs e)
		{
			if(!WebMode)
			{
				for(int i = 0; i < printers.Items.Count; i++)
				{
					if(printers.Items[i].SubItems[1].Text == Environment.PrinterName)
					{
						printers.Items.RemoveAt(i);
						if(TestPrinter.CheckPrinterExists())
							TestPrinter.SetPrinterProfile(ProfileType.TiffProfile);
						break;
					}
				}
			}
			else if(TestPrinter.CheckPrinterExists())
				TestPrinter.SetPrinterProfile(ProfileType.TiffProfile);
			if(!typeBox.Visible && groupBox1.Visible)
			{
				Height -= 36;
				groupBox1.Size = new Size(groupBox1.Width, groupBox1.Height - 36);
				groupBox1.Location = new Point(groupBox1.Location.X, groupBox1.Location.Y + 36);
			}
			else if(typeBox.Visible && !groupBox1.Visible && labelEForm.Visible)
			{
                Height -= 36;
                panel1.Location = new Point(panel1.Location.X, panel1.Location.Y - 59);
                panelImage2.Location = new Point(panelImage2.Location.X, panelImage2.Location.Y - 59);
                typeBox.Location = new Point(typeBox.Location.X, typeBox.Location.Y - 59);
                groupBox2.Visible = false;
			}
			if(DocID > 0)
				newWindowDocumentButton.Set(DocID);
			else
				newWindowDocumentButton.Set(FileName, doc.Text);
            bProperties.Location = new Point(363, 262);
		}

		private void printImageInst_EndPrint(object sender, EventArgs e)
		{
			var printImageInst = sender as ImageControl.PrintImage;
			printImageInst.EndPrint -= printImageInst_EndPrint;
		}

		/// <summary>
		/// Отслеживаем, на какой элемент списка типов пользователь навёл мышь.
		/// Если текст элемента не умещается в окне списка, показываем всплывающую подсказку.
		/// </summary>
		private void typeBox_MouseMove(object sender, MouseEventArgs e)
		{
			var listControl = (CheckedListBox)sender;
			int index = listControl.IndexFromPoint(e.Location);
			if(index != _ttIndex)
				if((_ttIndex = index) > -1)
				{
					Rectangle controlRect = listControl.GetItemRectangle(_ttIndex);
					string itemText = listControl.Items[_ttIndex].ToString();
					using(var g = listControl.CreateGraphics())
						if(g.MeasureString(itemText, listControl.Font).ToSize().Width >= controlRect.Width)
							toolTip.SetToolTip(listControl, itemText);
						else
							toolTip.RemoveAll();
				}
				else
					toolTip.RemoveAll();
		}

		private void DocPrintDialog_KeyUp(object sender, KeyEventArgs e)
		{
			newWindowDocumentButton.ProcessKey(e.KeyData);
		}

        private ListViewItem _lastItemChecked; // последний выбранный item
        private int _lastItemCheckedIndex; // его же индекс
        private bool _reallyChecked; // флаг - мы сменили текущий item
        private bool _uncheckBox; // сняли галку (используем, чтобы избежать StackOverflow)
        private bool _isChecking;

        /// <summary>
        /// Установка принтера по умолчанию чекбоксом
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if ((_lastItemChecked != null && _lastItemChecked.Checked && _lastItemChecked != printers.Items[e.Index]) || (_lastItemChecked != null && _uncheckBox))
            {
                _lastItemCheckedIndex = _lastItemChecked.Index;
                _lastItemChecked.Checked = false;
                _reallyChecked = true;
                _uncheckBox = false;
            }
            _lastItemChecked = printers.Items[e.Index];

            if ((e.NewValue == CheckState.Checked && _reallyChecked && _lastItemChecked != printers.Items[_lastItemCheckedIndex]) || (_checkedCnt == 0 && _dialogActivated))
            {
                switch (MessageBox.Show(
                    Environment.StringResources.GetString("SetPrinterDefault_Question"),
                    Environment.StringResources.GetString("Confirmation"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    case true:
                        {
                            printers.ItemCheck -= printers_ItemCheck;
                            if (!_isChecking)
                            {
                                _isChecking = true;
                                foreach (ListViewItem item in printers.Items)
                                {
                                    item.Checked = false;
                                }
                                printers.Items[e.Index].Checked = true;
                                e.NewValue = CheckState.Checked;
                                Win32.SystemPrinters.SetDefaultPrinter((string)printers.Items[e.Index].Tag);
                                _isChecking = false;
                            }
                            printers.ItemCheck += printers_ItemCheck;
                            break;
                        }
                    case false:
                        {
                            e.NewValue = CheckState.Unchecked;
                            if (_checkedCnt > 0)
                            {
                                if (_lastItemChecked != null)
                                {
                                    _lastItemChecked.Checked = false;
                                }
                                if (_lastItemCheckedIndex >= 0)
                                    _lastItemChecked = printers.Items[_lastItemCheckedIndex];
                                _lastItemChecked.Checked = true;
                            }
                            break;
                        }
                }
                _uncheckBox = false;
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                if (e.Index == _lastItemChecked.Index)
                {
                    e.NewValue = CheckState.Checked;
                }
                else
                {
                    _uncheckBox = true;
                }
            }
            _reallyChecked = false;
        }

        private void printers_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (printers.CheckedItems.Count > 0 && _checkedCnt > 0)
                return;
            if (e.Item.Checked)
                _checkedCnt++;
        }
	}
}