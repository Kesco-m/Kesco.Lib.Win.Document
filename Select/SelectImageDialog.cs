using System;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using Kesco.Lib.Win.Document.Controls;
using Kesco.Lib.Win.Document.Items;
using Kesco.Lib.Win.Options;

namespace Kesco.Lib.Win.Document.Select
{
    public partial class SelectImageDialog : FreeDialog
    {
        private int docID;
        private bool isLoad;
        private int pagesCount;
        private Folder subLayout;

        public SelectImageDialog()
        {
            InitializeComponent();
        }

        public int DocumentID
        {
            get { return docID; }
            set
            {
                docID = value;
                LoadImages();
            }
        }

        public int ImageID { get; internal set; }
        public int Page { get; internal set; }

        public int PagesCount
        {
            get { return (pagesCount + docControl1.PageCount); }
            set { pagesCount = value; }
        }

        public string SrcFileName { get; set; }
        public int SrcStartPage { get; set; }
        public int SrcPagesCount { get; set; }

        public int OldImageID;

        private void SelectImageDialog_Load(object sender, EventArgs e)
        {
            docControl1.ZoomText = Environment.StringResources.GetString("ToWindow");
            isLoad = true;
            if (docID <= 0)
                return;
            subLayout = Environment.Layout.Folders.Add(Name);
            int size = subLayout.LoadIntOption("Width", Width);
            Width = (size > MinimumSize.Width) ? size : MinimumSize.Width;
            size = subLayout.LoadIntOption("Height", Height);
            Height = (size > MinimumSize.Height) ? size : MinimumSize.Height;
            LoadImages();
        }

        private void SelectImageDialog_Closed(object sender, EventArgs e)
        {
            if (subLayout == null || WindowState != FormWindowState.Normal)
                return;
            subLayout.Option("Width").Value = Width;
            subLayout.Option("Height").Value = Height;
            subLayout.Save();

            cdialog_DialogEvent();
        }

        private void cdialog_DialogEvent() //object source, Lib.Win.DialogEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return;

            string addFileName = Tag as string;
            if (string.IsNullOrEmpty(addFileName) || !System.IO.File.Exists(addFileName))
                return;
            try
            {
                int documentID = DocumentID;
                int imageID = ImageID;
                bool isPDF = Environment.IsPdf(addFileName);
                int pagesCount = PagesCount;
                if (imageID > 0)
                {
                    int page = Page - 1;
                    Kesco.Lib.Win.Document.Dialogs.AddDBDocDialog.AddAndSave(0, imageID, addFileName, page, pagesCount, documentID, 0, 0, Lib.Win.Document.Environment.GetRandomLocalServer(), Kesco.Lib.Win.Document.Dialogs.AddDBDocDialog.TypeSave.SaveAll);
                    Slave.DeleteFile(addFileName);
                }
                else
                {
                    DateTime creationTime = DateTime.Now;
                    ServerInfo server;
                    string fName;
                    if (Environment.MoveFile(addFileName, ref creationTime, out fName, out server))
                        Environment.DocImageData.DocImageInsert(server.ID, fName, ref imageID, ref documentID, 0, "", DateTime.MinValue, "", "", false, creationTime, 0, false, isPDF ? "PDF" : "TIF", pagesCount);
                }

                if (!string.IsNullOrEmpty(SrcFileName) && !SrcFileName.Equals(addFileName) && System.IO.File.Exists(SrcFileName))
                {
                    try
                    {
                        if (!isPDF)
                        {
                            string tempFileName = Lib.Win.Document.Environment.LibTiff.DeletePart(SrcFileName, SrcStartPage - 1, SrcPagesCount, SrcFileName);
                            if (!string.IsNullOrEmpty(tempFileName) && System.IO.File.Exists(tempFileName))
                            {
                                System.IO.File.Copy(tempFileName, SrcFileName, true);
                                Slave.DeleteFile(tempFileName);
                            }
                        }
                        else
                            Lib.Win.Document.Environment.PDFHelper.DelPart(SrcFileName, SrcStartPage, SrcStartPage + SrcPagesCount - 1);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);
            }
        }
        private void RefreshStatus()
        {
            docControl1.ShowAnnotationGroup(Missing.Value);
            radioButtonAfter.Enabled =
                radioButtonBefore.Enabled = docControl1.ImageDisplayed;
            radioButtonBetween.Enabled =
                numericUpDown1.Enabled =
                numericUpDown2.Enabled =
                label1.Enabled = radioButtonBefore.Enabled && docControl1.PageCount > 1;
            if (numericUpDown1.Enabled)
            {
                numericUpDown1.Maximum = docControl1.PageCount - 1;
                numericUpDown2.Maximum = docControl1.PageCount;
                numericUpDown1.Minimum = 1;
                numericUpDown2.Minimum = 2;
                radioButtonAfter.Checked = true;
            }
            else
            {
                if (radioButtonBetween.Checked)
                    radioButtonAfter.Checked = true;
                numericUpDown1.Minimum =
                    numericUpDown2.Minimum =
                    numericUpDown1.Value =
                    numericUpDown2.Value =
                    numericUpDown1.Maximum =
                    numericUpDown2.Maximum = 0;
            }
        }

        private void LoadImages()
        {
            RefreshStatus();
            if (listViewImages.Items.Count > 0)
                listViewImages.Clear();
            if (!isLoad)
                return;
            if (docID > 0)
            {
                ListItem selecteditem = null; // индекс текущего изображения
                int mainImageID = 0;
                bool hasImage = false;
                // получаем изображения
                using (DataTable dt = Environment.DocImageData.GetDocImages(docID, false))
                {
                    hasImage = (dt.Rows.Count > 0);
                    if (hasImage)
                    {
                        // получаем основное изображение
                        mainImageID = Environment.DocData.GetDocIntField(Environment.DocData.MainImageIDField, docID);
                    }
                    using (DataTableReader dr = dt.CreateDataReader())
                    {
                        while (dr.Read())
                        {
                            var imgId = (int) dr[Environment.DocImageData.IDField];
                            if (imgId == OldImageID)
                            {
                                hasImage = (dt.Rows.Count > 1);
                                continue;
                            }
                            var createDate = (DateTime) dr[Environment.DocImageData.CreateDateField];
                            var editedDate = (DateTime) dr[Environment.DocImageData.EditedField];
                            var archiveID = (int) dr[Environment.DocImageData.ArchiveIDField];
                            var item = new VariantListItem(
                                imgId,
                                VariantType.Image,
                                createDate.ToLocalTime().ToString("dd.MM.yyyy"))
                                           {CreateTime = createDate, EditedTime = editedDate};
                            int printed = (dr[Environment.DocImageData.PrintedField] is DBNull)
                                              ? 0
                                              : Convert.ToInt32(dr[Environment.DocImageData.PrintedField]);
                            item.Printed = printed > 0;

                            item.ImageType = dr[Environment.DocImageData.ImageTypeField].ToString();

                            if (item.Printed)
                            {
                                object a1 = Environment.PrintData.GetField(Environment.PrintData.NameField, printed);
                                if (a1 != null)
                                    item.Text = a1.ToString();
                            }
							if(imgId == mainImageID)
							{
								if(archiveID != 0)
								{
									if(printed > 0)
									{
										item.ImageIndex = (int)VariantType.MainImageOriginalPrinted;
										item.Type = VariantType.MainImageOriginalPrinted;
									}
									else
									{
										item.ImageIndex = item.IsPDF() ? 20 : (int)VariantType.MainImageOriginal;
										item.Type = VariantType.MainImageOriginal;
									}
								}
								else
								{
									if(printed > 0)
									{
										item.ImageIndex = (int)VariantType.MainImagePrinted;
										item.Type = VariantType.MainImagePrinted;
									}
									else
									{
										item.ImageIndex = item.IsPDF() ? 18 : (int)VariantType.MainImage;
										item.Type = VariantType.MainImage;
									}
								}
							}
							else
							{
								if(archiveID != 0)
								{
									if(printed > 0)
									{
										item.ImageIndex = (int)VariantType.ImageOriginalPrinted;
										item.Type = VariantType.ImageOriginalPrinted;
									}
									else
									{
										item.ImageIndex = item.IsPDF() ? 19 : (int)VariantType.ImageOriginal;
										item.Type = VariantType.ImageOriginal;
									}
								}
								else
								{
									if(printed > 0)
									{
										item.ImageIndex = (int)VariantType.ImagePrinted;
										item.Type = VariantType.ImagePrinted;
									}
									else
										item.ImageIndex = item.IsPDF() ? 17 : (int)VariantType.Image;
								}
							}
                            listViewImages.Items.Add(item);
                            if (mainImageID == imgId)
                                selecteditem = item;
                        }
                        dr.Close();
                        dr.Dispose();
                        dt.Dispose();
                    }
                }
                if (!hasImage)
                {
                    ImageID = 0;
                    if (
                        MessageBox.Show(Environment.StringResources.GetString("Select_SelectImageDialog_Message1"),
                                        Environment.StringResources.GetString("Conformation"), MessageBoxButtons.YesNoCancel) ==
                        DialogResult.Yes)
                        DialogResult = DialogResult.OK;
                    Close();
                }
            }
            else
                Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listViewImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            ImageID = 0;
            if (listViewImages.SelectedItems.Count > 0)
            {
                var item = listViewImages.SelectedItems[0] as VariantListItem;
                if (item != null)
                {
                    ImageID = item.ID;
                    docControl1.LoadComplete -= docControl1_LoadComplete;
                    docControl1.LoadComplete += docControl1_LoadComplete;
                    docControl1.SetImage(ImageID, item.IsPDF());
                }
            }
            if (!docControl1.CompliteLoading)
                RefreshStatus();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Maximum == 0)
                return;
            radioButtonBetween.Checked = true;
            docControl1.Page = (int) numericUpDown1.Value;
        }

        private void docControl1_PageChanged(object sender, EventArgs e)
        {
            Page = docControl1.Page;
            if (docControl1.Page == docControl1.PageCount)
                radioButtonAfter.Checked = true;
            else
            {
                if (numericUpDown1.Maximum > 0)
                {
                    numericUpDown1.Value = docControl1.Page;
                    numericUpDown2.Value = docControl1.Page + 1;
                    radioButtonBetween.Checked = true;
                }
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Maximum == 0) 
                return;
            radioButtonBetween.Checked = true;
            docControl1.Page = (int) numericUpDown2.Value - 1;
        }

        private void docControl1_LoadComplete(object sender, EventArgs e)
        {
            docControl1.LoadComplete -= docControl1_LoadComplete;
            docControl1.ShowAnnotationGroup(Missing.Value);
            RefreshStatus();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            buttonOk.Enabled = false;
            End(DialogResult.OK);
        }

        private void radioButtonBefore_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBefore.Checked)
                Page = 0;
        }

        private void radioButtonAfter_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAfter.Checked)
                Page = -1;
        }

        private void radioButtonBetween_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonBetween.Checked)
                Page = (int) numericUpDown1.Value;
        }
    }
}