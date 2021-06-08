using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using iTextSharp.text.pdf;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document.EWSMail;

namespace Kesco.Lib.Win.Document.Dialogs
{
	public partial class SendOutDialog : Win.FreeDialog
	{
		bool send = false;

		public SendOutDialog()
		{
			InitializeComponent();
			this.docControl.ExternalSave = true;
			//this.toolStripButton2.Visible = false;
			contacts = new List<KeyValuePair<int, DataRow>>();
			splitContainer1.Panel2Collapsed = true;
		}

		#region Veribles

		private SynchronizedCollection<Components.ProcessingDocs> infos;

		public SynchronizedCollection<Components.ProcessingDocs> ListAdded
		{
			get { return infos; }
		}

		public List<KeyValuePair<int, DataRow>> contacts;
		private List<Tuple<int, string, string>> sendmail;
		Options.Folder settings;

		Dictionary<string, string> mails;

		Components.ImageToSend currentImageToSend = null;

		public event EventHandler EditPush;

		private void OnEditPush()
		{
			try
			{
				if(EditPush != null)
					EditPush.DynamicInvoke(new object[] { this, EventArgs.Empty });
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		private void SendOutDialog_Load(object sender, EventArgs e)
		{
			settings = Environment.Layout.Folders.Add(this.Name);
			
			// координата X окна
			int l = settings.LoadIntOption("WindowX", Left);

			// координата Y окна
			int t = settings.LoadIntOption("WindowY", Top);

			// высота окна
			int h = settings.LoadIntOption("WindowHeight", Height);

			// ширина окна
			int w = settings.LoadIntOption("WindowWidth", Width);

			Screen sr = Screen.AllScreens.FirstOrDefault(x => x.Bounds.IntersectsWith(new Rectangle(l, t, w, h)));
			if(sr != null)
			{
				Width = w;
				Height = h;
				Top = t;
				Left = l;
			}

			splitContainer1.SplitterDistance = settings.LoadIntOption("SplitterDistance", splitContainer1.SplitterDistance);

			if(DesignerDetector.IsComponentInDesignMode(this))
				return;
			string domain = System.Environment.UserDomainName;
			var dir = new DirectoryEntry { Path = @"LDAP://" + domain };
			var sea = new DirectorySearcher(dir)
			{
				Filter = "(&(objectCategory=person)(objectClass=user)(samaccountname=" +
						 System.Environment.UserName + "))"
			};
			SearchResultCollection resEnts = sea.FindAll();
			for(int i = 0; i < resEnts.Count; i++)
			{
				SearchResult resEnt = resEnts[i];
				if(resEnt.Properties.Contains("msexchhomeservername"))
				{
					if(resEnt.Properties.Contains("proxyaddresses"))
					{
						if(string.IsNullOrEmpty(Environment.MailDomain))
						{
							string uri = Environment.FullExchangeServerEwsUrl;
						}
						ResultPropertyValueCollection res = resEnt.Properties["proxyaddresses"];
						for(int j = 0; j < res.Count; j++)
						{
							if(mails == null)
								mails = new Dictionary<string, string>();
							string mail = res[j].ToString();
							if(mail.StartsWith("smtp:", StringComparison.CurrentCultureIgnoreCase))
							{
								mails.Add(mail.Substring(5), "");
							}
						}
					}
				}
			}
			comboBox2.DataSource = mails.Keys.ToList();
			richTextBoxContact.EditPush += new EventHandler(richTextBoxContact_EditPush);
		}

		void SendOutDialog_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
		{
			settings.Option("WindowHeight").Value = Height;
			settings.Option("WindowWidth").Value = Width;
			settings.Option("WindowX").Value = Left;
			settings.Option("WindowY").Value = Top;
			settings.Option("SplitterDistance").Value = splitContainer1.SplitterDistance;
			settings.Save();
		}

		void richTextBoxContact_EditPush(object sender, EventArgs e)
		{
			OnEditPush();
		}

		#region Interface Work

		private void toolStripButtonImage_Click(object sender, EventArgs e)
		{
			this.SuspendLayout();
			splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
			if(!splitContainer1.Panel2Collapsed)
			{
				if(string.IsNullOrEmpty(docControl.FileName))
				{
					if(infos != null && infos.Count > 0)
					foreach( var con in flowLayoutPanel.Controls)
					{
						Controls.CheckebleImageLinkLabel li = con as Controls.CheckebleImageLinkLabel;
						if(li != null && li.Tag.Equals(infos[0]))
						{
							linkLabel_LinkClicked(li, new LinkLabelLinkClickedEventArgs(null));
							break;
						}
					}
				}
			}
			this.ResumeLayout();
		}

		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if(splitContainer1.Panel2Collapsed)
				splitContainer1.Panel2Collapsed = false;
			//((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			//this.splitContainer1.Panel1.SuspendLayout();
			//this.splitContainer1.Panel2.SuspendLayout();
			//this.splitContainer1.SuspendLayout();
			//tableLayoutPanelImage.SuspendLayout();
			this.tableLayoutPanelImages.SuspendLayout();
			for(int i = tableLayoutPanelImages.Controls.Count -1; i > -1; i-- )
			{
				var con = tableLayoutPanelImages.Controls[i];
				tableLayoutPanelImages.Controls.RemoveAt(i);
				con.Dispose();
			}
			Controls.CheckebleImageLinkLabel ll = (Controls.CheckebleImageLinkLabel )sender;
			//linkLabel1.Text = ll.Text;
			Components.ProcessingDocs a = ll.Tag as Components.ProcessingDocs;
			Components.ImageToSend its = null;
			int row = 0;
			foreach(var cits in a.imageToSends)
			{
				if(!cits.Error)
				{
					var li = AddLabel(cits, a.docString, row++);
					if(its == null && cits.CanBeSended)
					{
						its = cits;
						li.LinkVisited = true;
						li.CheckBoxVisible = a.imageToSends.Count > 1;
					}
				}
			}
			if(its != null)
				LoadImage(its);
			this.tableLayoutPanelImages.ResumeLayout(true);
			//this.splitContainer1.Panel1.ResumeLayout(false);
			//this.splitContainer1.Panel2.ResumeLayout(false);
			//((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			//this.splitContainer1.ResumeLayout(false);
			//this.tableLayoutPanelImage.ResumeLayout(false);
			this.PerformLayout();
		}

		private Controls.CheckebleImageLinkLabel AddLabel(Components.ImageToSend its, string text, int row)
		{
			Controls.CheckebleImageLinkLabel li = new Controls.CheckebleImageLinkLabel();
			li.Tag = its;
			li.Checked = its.CanBeSended;
			li.CheckBoxVisible = true;
			li.ImageHead = null;
			li.AutoSize = true;
			li.Dock = DockStyle.Fill;
			li.LinkClicked += linkLabel1_LinkClicked;
			li.ImageClose = (System.Drawing.Image)(global::Kesco.Lib.Win.Document.Properties.Resources.ResourceManager.GetObject("ImageRemove"));
			if(li.ImageClose != null)
				li.Click += new EventHandler(LinkImage_Click);
			li.CheckedChanged += LinkImage_CheckedChanged;
			tableLayoutPanelImages.Controls.Add(li,0,row);
			li.Text = text;
			return li;
		}

		private void LinkImage_CheckedChanged(object sender, EventArgs e)
		{
			Controls.CheckebleImageLinkLabel li = sender as Controls.CheckebleImageLinkLabel;
			if(li == null)
				return;
			Components.ImageToSend its = li.Tag as Components.ImageToSend;
			Components.ImageToSend cits = tabControlSettings.Tag as Components.ImageToSend;
			Components.ProcessingDocs cpd = null;
			int id = -1;
			foreach(var pd in infos)
			{
				if(pd.imageToSends!= null && pd.imageToSends.Contains(its))
				{
					cpd = pd;
					id = cpd.imageToSends.IndexOf(its);
					break;
				}
			}
			its.CanBeSended = li.Checked;
			foreach(var con in flowLayoutPanel.Controls)
			{
				Controls.CheckebleImageLinkLabel ll = con as Controls.CheckebleImageLinkLabel;
				if(ll != null && ll.Tag.Equals(cpd))
				{
					ll.Text = cpd.docString + ' ' + "(" + (cpd.GetFullSize() > 1024 ? cpd.GetFullSize() > 820000 ? (cpd.GetFullSize() / 1048576f).ToString("N2") + "MB" : (cpd.GetFullSize() / 1024f).ToString("N2") + "KB" : cpd.GetFullSize().ToString() + "B") + ")";
					break;
				}
			}
			if((its.DocID == cits.DocID && its.ImageID == cits.ImageID) || its.FileName == cits.FileName)
				tabControlSettings.Tag = its;
			if(cpd != null)
				cpd.imageToSends[id] = its;
		}

		private void LoadImage(Components.ImageToSend a)
		{
			radioButtonMainFormat.Visible = false;
			if(a != null)
			{
				tabControlSettings.Tag = a;
				bool isPDF = Environment.IsPdf(a.SourceFileName);
				int pagecount = 0;
				if(isPDF)
				{
					pdfRadio.Enabled = true;
					pdfRadio.Checked = true;
					tifRadio.Enabled = true;
					Environment.PDFHelper.Open(a.SourceFileName, null);
					pagecount = Environment.PDFHelper.PageCount;
					Environment.PDFHelper.Close();
				}
				else
				{
					tifRadio.Enabled = true;
					pdfRadio.Enabled = true;
					tifRadio.Checked = true;
					pagecount = Environment.LibTiff.GetCountPages(a.SourceFileName);
				}

				textBoxEndPage.Maximum = pagecount;
				textBoxStartPage.Maximum = pagecount;
				textBoxEndPage.Value = pagecount;
				switch(a.PagesType)
				{
					case 1:
						rbSelectedPages.Checked = true;
						string[] ar = a.Pages.Split('-');
						textBoxStartPage.Value = decimal.Parse(ar[0]);
						textBoxEndPage.Value = decimal.Parse(ar[1]);
						break;
					case 2:
						rbSelectNum.Checked = true;
						textBoxSelectNum.Text = a.Pages;
						break;
					default:
						rbAllPages.Checked = true;
						break;
				}
				switch(a.FormatType)
				{
					case 1:
						tifRadio.Checked = true;
						break;
					case 2:
						pdfRadio.Checked = true;
						break;
						//default:
						//	radioButtonMainFormat.Checked = true;
						//	break;
				}
				if(a.BlackWight.HasValue)
				{
					radioButtonBlackWhite.Checked = a.BlackWight.Value;
					radioButtonColor.Checked = !a.BlackWight.Value;
				}
				else
					radioButtonBlackWhite.Checked = radioButtonColor.Checked = false;
				checkBoxSendNotes.Checked = a.SendNotes;
				checkBoxBurnNotes.Checked = a.SendNotes && a.BurnNotes;
				radioButtonBurnStamps.Checked = a.BurnStamps;
				radioButtonSendStamps.Checked = !a.BurnStamps;
				docControl.LoadImage(a);
				docControl.SelectTool(0);
			}
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var con = sender as Controls.CheckebleImageLinkLabel;
			if(con == null)
				return;
			Components.ImageToSend its = con.Tag as Components.ImageToSend;
			if(its != null)
			{
				ClearSelectedImage();
				LoadImage(its);
				con.LinkVisited = true;
			}
		}

		private void ClearSelectedImage()
		{
			foreach(var con in tableLayoutPanelImages.Controls)
			{
				var cll = con as Controls.CheckebleImageLinkLabel;
				if(cll != null)
					cll.LinkVisited = false;
			}
		}

		void richTextBoxContact_SearchStart(object sender, System.EventArgs e)
		{
			FindPersonWeb();
		}

		void richTextBoxContact_FindEMail(object sender, System.EventArgs e)
		{
			StartContactWork(4, false);
		}

		private void radioButtonColor_CheckedChanged(object sender, EventArgs e)
		{
			docControl.SetImagePalette(1);
			if(!buttonApply.Visible && radioButtonColor.Checked)
				buttonApply.Visible = true;
		}

		private void radioButtonBlackWhite_CheckedChanged(object sender, EventArgs e)
		{
			docControl.SetImagePalette(3);
			if(!buttonApply.Visible && radioButtonBlackWhite.Checked)
				buttonApply.Visible = true;
		}

		private void toolStripButtonSend_Click(object sender, EventArgs e)
		{
			send = true;
			Send();
		}

		private bool AddLabel(Components.ProcessingDocs pd)
		{
			if(Environment.GetServers().Count <= 0)
				throw new Exception(Environment.StringResources.GetString("Enviroment_Servers_Error1"));

			if(pd.Error)
				return false;

			Controls.CheckebleImageLinkLabel li = new Controls.CheckebleImageLinkLabel();
			li.AutoSize = true;
			li.CheckBoxVisible = false;
			li.Text = pd.docString + "(" + (pd.GetFullSize() > 1024 ? pd.GetFullSize() > 820000 ? (pd.GetFullSize() / 1048576f).ToString("N2") + "MB" : (pd.GetFullSize() / 1024f).ToString("N2") + "KB" : pd.GetFullSize().ToString() + "B") + ")";
			li.Tag = pd;
			if(pd.imageToSends.Count > 1)
				li.ImageHead = foldersList.Images[0];
			else
				li.ImageHead = null;
			li.ImageClose = (System.Drawing.Image)(global::Kesco.Lib.Win.Document.Properties.Resources.ResourceManager.GetObject("ImageRemove"));
			if(li.ImageClose != null)
				li.Click += new EventHandler(li_Click);
			flowLayoutPanel.Controls.Add(li);
			li.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel_LinkClicked);
			return true;
		}

		private void LinkImage_Click(object sender, EventArgs e)
		{
			Controls.CheckebleImageLinkLabel li = sender as Controls.CheckebleImageLinkLabel;
			if(li != null)
			{
				Components.ImageToSend its = li.Tag as Components.ImageToSend;
				tableLayoutPanelImages.Controls.Remove(li);
				its.CanBeSended = false;
			}
		}

		void li_Click(object sender, EventArgs e)
		{
			Controls.CheckebleImageLinkLabel li = sender as Controls.CheckebleImageLinkLabel;
			if(li != null && infos != null && infos.Count > 0)
			{
				Components.ProcessingDocs pd = li.Tag as Components.ProcessingDocs;
				flowLayoutPanel.Controls.Remove(li);
				li.Dispose();
				if(pd == null)
					return;
				foreach(var its in pd.imageToSends)
					if(!string.IsNullOrEmpty(its.SendFileName) && File.Exists(its.SendFileName))
						File.Delete(its.SendFileName);
				int num = infos.IndexOf(pd);
				if(num > -1)
					infos.RemoveAt(num);
				if(!splitContainer1.Panel2Collapsed)
					splitContainer1.Panel2Collapsed = true;
			}
		}

		#endregion

		private void Send()
		{
			if(infos == null || infos.Count < 1)
			{
				var mf = new Win.MessageForm("Отправить письмо без вложений?", Environment.StringResources.GetString("Warning"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
				if(mf.ShowDialog(this) == DialogResult.No)
					return;
			}
			if((contacts == null || contacts.Count < 1) && (sendmail == null || sendmail.Count < 1))
			{
				if(richTextBoxContact.IsEmail)
					StartContactWork(4, false);
				return;
			}
			bool usePdf = pdfRadio.Checked;
			bool useTif = tifRadio.Checked;

			string exte = "tif";//Path.GetExtension(infos[0].imageToSends[0].SourceFileName).TrimStart('.');
			if(usePdf)
				exte = "pdf";
			//if(useTif)
			//	exte = "tif";

			string newFileName = Path.Combine(Path.GetTempPath(), Environment.GenerateFileName(exte));
			Console.WriteLine("new filename: " + newFileName);

			Cursor = Cursors.WaitCursor;
			// проверка на наличие файлов
			//File.Copy(infos[0].sourceFileName, newFileName);

			//if(!File.Exists(newFileName))
			//	return;
			//else
			//	File.Delete(newFileName);
			if(!string.IsNullOrEmpty(Environment.FullExchangeServerEwsUrl))
			{

				//Lib.Win.Options.Folder root = new Lib.Win.Options.Root();
				//System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(root.OptionForced<string>("SmtpServer").GetValue<string>());
				//client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
				//System.Net.Mail.MailMessage mess = new System.Net.Mail.MailMessage("nikiforov@testcom.com", "nikiforov@kescom.com");
				//mess.AlternateViews.Add( System.Net.Mail.AlternateView.CreateAlternateViewFromString(richTextBox1.Rtf, System.Text.Encoding.Unicode, System.Net.Mime.MediaTypeNames.Text.RichText));
				//mess.Body = richTextBox1.Text;
				//client.Send(mess);
				//Close();
				//return;

				ExchangeServiceBinding esb = null;
				CreateItemType cit = null;
				esb = new ExchangeServiceBinding();
				esb.Credentials = CredentialCache.DefaultNetworkCredentials;
				esb.Url = Environment.FullExchangeServerEwsUrl;
				esb.RequestServerVersionValue = new RequestServerVersion();
				esb.RequestServerVersionValue.Version = ExchangeVersionType.Exchange2010_SP2;

				cit = new CreateItemType
				{
					Items = new NonEmptyArrayOfAllItemsType(),
					MessageDispositionSpecified = true,
					SavedItemFolderId = new TargetFolderIdType() { Item = new DistinguishedFolderIdType() { Id = DistinguishedFolderIdNameType.sentitems } },
					MessageDisposition = MessageDispositionType.SaveOnly
				};

				MessageType mt = new MessageType();

				mt.From = new SingleRecipientType { Item = new EmailAddressType { EmailAddress = Environment.MailBoxName, Name = Environment.MailAlias } };
				mt.ToRecipients = new EmailAddressType[contacts.Count + (sendmail== null?0:sendmail.Count)];
				for(int j = 0; j < contacts.Count; j++)
				{
					EmailAddressType mat = new EmailAddressType();
					//mat.Name = contacts[j].Value[Environment.PersonData.NameField].ToString();
					mat.EmailAddress = contacts[j].Value[Environment.FaxRecipientData.ContactField].ToString();
					mt.ToRecipients[j] = mat;
				}
				if(sendmail != null)
					for(int j = 0; j < sendmail.Count; j++)
					{

						EmailAddressType mat = new EmailAddressType();
						mat.Name = sendmail[j].Item2;
						mat.EmailAddress = sendmail[j].Item3;
						mt.ToRecipients[contacts.Count + j] = mat;

					}
				mt.Subject = textBoxTitle.Text;
				mt.Body = new BodyType();
				mt.Body.BodyType1 = BodyTypeType.HTML;
				mt.Body.Value = editor.InnerHtml;
				cit.Items.Items = new ItemType[1];
				cit.Items.Items[0] = mt;

				EWSMail.AttachmentType[ ] attachments = new EWSMail.AttachmentType[infos.Count];
				if(infos != null)
				for(int i = 0; i < infos.Count; i++)
				{
					foreach(var ino in infos[i].imageToSends)
					{
						if(!ino.CanBeSended)
							continue;
						usePdf = ino.FormatType == 2;
						useTif = ino.FormatType == 1;
						if(!usePdf && !useTif)
							exte = Path.GetExtension(ino.SourceFileName).TrimStart('.');
						else
							if(useTif)
							 exte = "tif";
						if(usePdf)
							exte = "pdf";
						if(!string.IsNullOrEmpty(ino.SendFileName) && File.Exists(ino.SendFileName))
						{
							File.Copy(ino.SendFileName, newFileName);
						}
						else
							File.Copy(ino.SourceFileName, newFileName);

						string sdfn = ino.FileNameToSend;
						if(string.IsNullOrWhiteSpace(sdfn))
							sdfn = "Document" + i.ToString() + "." + exte;
						string sendDocFileName = Path.Combine(Path.GetTempPath(), sdfn);
						try
						{
							if(File.Exists(sendDocFileName))
								File.Delete(sendDocFileName);

							File.Move(newFileName, sendDocFileName);
						}
						catch(System.Exception ex)
						{
							Data.Env.WriteToLog(ex);
							Error.ErrorShower.OnShowError(this, ex.Message, "");
						}
						FileAttachmentType fat = new FileAttachmentType();
						byte[] binaryData;
						using(FileStream inFile = new FileStream(sendDocFileName, FileMode.Open, FileAccess.Read))
						{
							binaryData = new Byte[inFile.Length];
							inFile.Read(binaryData, 0, (int)inFile.Length);
							inFile.Close();
						}
						fat.Content = binaryData;
						if(exte == "pdf")
							fat.ContentType = "application/pdf";
						else
							fat.ContentType = "image/tiff";
						fat.Name = Name = infos[i].docString + "." + exte;
						attachments[i] = fat;
					}
				}
				try
				{
					CreateItemResponseType crit = esb.CreateItem(cit);
					if(crit == null || crit.ResponseMessages.Items.Length == 0 || crit.ResponseMessages.Items[0].ResponseClass != ResponseClassType.Success)
						throw new System.Exception("Can't save message/n" + ((crit == null) ? "no response" : "responce length " + crit.ResponseMessages.Items.Length.ToString()) + "\n" + Environment.FullExchangeServerEwsUrl);
					ItemIdType iid = ((ItemInfoResponseMessageType)(((crit)).ResponseMessages.Items[0])).Items.Items[0].ItemId;
					CreateAttachmentResponseType cart = esb.CreateAttachment
					(
						new CreateAttachmentType
						{
							Attachments = attachments,
							ParentItemId = iid
						}
						);

					if(cart == null || cart.ResponseMessages.Items.Length == 0 || cart.ResponseMessages.Items[0].ResponseClass != ResponseClassType.Success)
					{
						esb.DeleteItem(new DeleteItemType { ItemIds = new BaseItemIdType[1] { iid } });
						throw new System.Exception("Can't create attachment");
					}
					AttachmentInfoResponseMessageType attachmentResponseMessage = (AttachmentInfoResponseMessageType)cart.ResponseMessages.Items[0];

					ItemIdType attachmentItemId = new ItemIdType();
					attachmentItemId.ChangeKey = attachmentResponseMessage.Attachments[0].AttachmentId.RootItemChangeKey;
					attachmentItemId.Id = attachmentResponseMessage.Attachments[0].AttachmentId.RootItemId;

					SendItemType si = new SendItemType();
					si.ItemIds = new BaseItemIdType[1];
					si.ItemIds[0] = attachmentItemId;
					si.SaveItemToFolder = false;
					SendItemResponseType siSendItemResponse = esb.SendItem(si);

					this.Close();
				}
				catch(System.Exception ex)
				{

					Slave.DeleteFile(newFileName);
					Cursor = Cursors.Default;
					if(!ex.Message.Contains(" Insufficient Storage"))
					{
						Data.Env.WriteToLog(ex, Environment.FullExchangeServerEwsUrl);

						Error.ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
					}
					else
						Error.ErrorShower.OnShowError(this, "Не удалось присоеденить документ. Превышен объём сообщения или не доступен почтовый ящик.", Environment.StringResources.GetString("Error"));
					return;
				}
				finally
				{

				}
			}
			foreach(Components.ProcessingDocs pd in infos.Where(x => x.imageToSends!=null))
			{
				foreach(var ino in pd.imageToSends.Where(x => x.ImageID > 0))
				{
					if(contacts != null && contacts.Count > 0)
						Environment.LogEmailData.LogEmail(ino.ImageID, Environment.PersonData.GetPerson(contacts[0].Key), contacts[0].Value[Environment.FaxRecipientData.ContactField].ToString());
					else if(sendmail.Count > 0)
						Environment.LogEmailData.LogEmail(ino.ImageID, sendmail[0].Item2, sendmail[0].Item3);
				}
				foreach(var ino in pd.imageToSends.Where(x => x.FaxID > 0))
					Task.Factory.StartNew(() => { Environment.FaxOutData.MarkResend(ino.FaxID); });

				foreach(var ino in pd.imageToSends.Where(x => x.SendFileName != null))
					Slave.DeleteFile(ino.SendFileName);
			}
			Slave.DeleteFile(newFileName);
		}

		#region Add
		public bool FileAdd(string filename, string curDocString)
		{
			bool add = false;
			if(infos == null)
			{
				infos = new SynchronizedCollection<Components.ProcessingDocs>();
				add = true;
			}
			if(infos.FirstOrDefault(x => x.imageToSends != null && x.imageToSends.FirstOrDefault(y => y.SourceFileName == filename) ==null) == null)
			{
				Components.ProcessingDocs docc = new Components.ProcessingDocs(curDocString, filename);
				infos.Add(docc);
				if(add)
					textBoxTitle.Text = curDocString;
				else
					textBoxTitle.Text = "";
				return AddLabel(docc);
			}
			return false;
		}

		public bool ImageAdd(int docID,object imageID)
		{
			bool add = false;
			if(infos == null)
			{
				infos = new SynchronizedCollection<Components.ProcessingDocs>();
				add = true;
			}
			if(!(imageID is int)  || infos.FirstOrDefault(x=> x.docID == docID) == null)
			{
				Components.ProcessingDocs docc = new Components.ProcessingDocs(docID, imageID);
				if(infos.FirstOrDefault(x => x.docID == docc.docID) == null && !docc.Error)
				{
					infos.Add(docc);
					if(add)
						textBoxTitle.Text = docc.docString;
					else
						textBoxTitle.Text = "";
					return AddLabel(docc);
				}
			}
			return false;
		}

		public bool FaxAdd(int docID,int imgID,int faxID)
		{
			if(infos == null)
				infos = new SynchronizedCollection<Components.ProcessingDocs>();
			if(infos.FirstOrDefault(x => x.docID == docID) == null)
			{
				Components.ProcessingDocs docc = new Components.ProcessingDocs(docID, imgID, faxID);
				if(!docc.Error)
				{
					infos.Add(docc);
					return AddLabel(docc);
				}
			}
			return false;
		}

		# endregion

		private void docControl_LoadComplete(object sender, EventArgs e)
		{
			docControl.ShowAnnotationGroup(System.Reflection.Missing.Value);
			buttonApply.Visible = false;
			//CleanTabSettings();
		}

		void CleanTabSettings()
		{
			if(!radioButtonMainFormat.Checked)
				radioButtonMainFormat.Checked = true;
			if(!rbAllPages.Checked)
				rbAllPages.Checked = true;
			radioButtonColor.Checked = radioButtonBlackWhite.Checked = false;
			textBoxSelectNum.Clear();
			textBoxStartPage.ResetText();
			textBoxEndPage.ResetText();
		}

		private void label1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			//if((contacts == null || contacts.Count< 1) && (sendmail == null || sendmail.Count < 1))
				FindPersonWeb();
		}

		private void FindPersonWeb()
		{
			string perText = string.IsNullOrEmpty(richTextBoxContact.Text) ? string.Empty : richTextBoxContact.SearchText;
			
			string paramStr = "return=1&clid=27&_personwheresearch=5&_personforsend=1&_personvalidat=" + DateTime.Today.ToString("dd.MM.yyyy");
			if(!string.IsNullOrEmpty(perText))
				paramStr += "&search=" + HttpUtility.UrlEncode(perText);

			paramStr = paramStr.Replace("+", "%20");
			Web.PersonDialog dialog = new Web.PersonDialog(Environment.PersonSearchString, paramStr);
			dialog.DialogEvent += personDialog_DialogEvent;
			ShowSubForm(dialog);
			Enabled = false;
		}

		public void StartContactWork(int typeID, bool create)
		{
			string cont = richTextBoxContact.SearchText;
			if(typeID == 3 && !cont.StartsWith("+"))
			{
				String name = null;
				if(SystemInformation.TerminalServerSession)
				{
					uint bytesReturned;

					try
					{
						bool sessionInfo = Win32.wtsapi32.GetWTSQuerySessionInformation(IntPtr.Zero, Win32.wtsapi32.WTS_CURRENT_SESSION, Win32.wtsapi32.WTSInfoClass.WTSClientName, out name, out bytesReturned);
					}
					catch
					{

					}
				}
				else
					name = SystemInformation.ComputerName;
				cont = "+" + Environment.PhoneData.GetInternationalNumber(cont, name);
				if(string.IsNullOrEmpty(cont) || cont == "+")
				{
					FindPersonWeb();
					return;
				}
			}
			if(!create)
			{
				Select.SelectSenderDialog dialog = new Select.SelectSenderDialog(cont);
				if(typeID == 4)
				{
					dialog.DialogEvent += SelectSenderDialogEmail_DialogEvent;
				}
				else
				{
					dialog.DialogEvent += SelectSenderDialogFax_DialogEvent;
				}
				dialog.Owner = this;
				dialog.Show();
				Enabled = false;
			}
			else
			{
				Win.Web.ContactDialog dialog = new Win.Web.ContactDialog(Environment.CreateContactString, "personContactCategor=" + typeID.ToString() + "&personContactText=" + cont + "&docview=yes");
				dialog.DialogEvent += CreateContactDialog_DialogEvent;
				dialog.Show();
				Enabled = false;
			}
		}

		private void personDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Enabled = true;
			if(e.Dialog.DialogResult == DialogResult.OK)
			{
				Web.PersonDialog dialog = e.Dialog as Web.PersonDialog;
				dialog.DialogEvent -= personDialog_DialogEvent;
				Web.PersonInfo person = (Web.PersonInfo)dialog.Persons[0];
				AddPersonContact(person.ID);
			}
		}

		private void AddPersonContact(int id, string phoneNumber)
		{
			if(id > 0)
			{
				string name = Environment.PersonData.GetFullPerson(id);
				PersonContactDialog dialog;
				if(phoneNumber != null && phoneNumber.Length > 0)
					dialog = new PersonContactDialog(id, name, phoneNumber);
				else
					dialog = new PersonContactDialog(id, name);
				if((richTextBoxContact.Text.IndexOf('@') >= 0 && Environment.PersonData.PersonType == 2) ||
					dialog.Collection.Count == 1)
				{
					if(dialog.Collection.Count == 1)
						dialog.Collection[0].Checked = true;
					else
					{
						for(int i = 0; i < dialog.Collection.Count; i++)
							if(dialog.Collection[i].Text.IndexOf(richTextBoxContact.Text) != -1)
							{
								dialog.Collection[i].Checked = true;
								break;
							}
					}
					dialog.DialogResult = DialogResult.OK;
					addPersonContactDialog_DialogEvent(null, new DialogEventArgs(dialog));
				}
				else
				{
					dialog.DialogEvent += addPersonContactDialog_DialogEvent;
					Enabled = false;
					ShowSubForm(dialog);
				}
			}
		}

		private void AddPersonContact(int id)
		{
			AddPersonContact(id, null);
		}

		private void addPersonContactDialog_DialogEvent(object source, DialogEventArgs e)
		{
			if(!Enabled)
				Enabled = true;
			Focus();
			if(e.Dialog.DialogResult != DialogResult.OK)
				return;
			PersonContactDialog dialog = e.Dialog as PersonContactDialog;
			if(dialog == null)
				return;

			string personName = dialog.PersonName;
			int personID = dialog.PersonID;
			for(int i = 0; i < dialog.Collection.Count; i++)
			{
				if(!dialog.Collection[i].Checked)
					continue;
				DataRow dr = dialog.Collection[i].Tag as DataRow;
				if(dr == null || dr.IsNull(Environment.FaxRecipientData.PersonLinkIDField))
					AddContact(dr, new KeyValuePair<int, string>(personID, personName));
				else
				{
					KeyValuePair<KeyValuePair<int, string>, KeyValuePair<int, string>> res = Environment.PersonLinkData.GetFormatedLink((int)dr[Environment.FaxRecipientData.PersonLinkIDField]);
					AddContact(dr, res.Key, res.Value);
				}
			}
		}

		private void SelectSenderDialogEmail_DialogEvent(object source, DialogEventArgs e)
		{
			try
			{
				Enabled = true;
				Select.SelectSenderDialog dialog = e.Dialog as Select.SelectSenderDialog;
				if(dialog != null)
				{
					switch(dialog.DialogResult)
					{
						case DialogResult.OK:
							{
								Web.ContactDialog ccDialog = new Web.ContactDialog(Environment.CreateContactString, "personContactCategor=4&personContactText=" + "" + "&docview=yes");
								ccDialog.Owner = this;
								ccDialog.DialogEvent += CreateContactDialog_DialogEvent;
								ccDialog.Show();
								Enabled = false;
								send = false;
								return;
							}
						default:
							if(dialog.DialogResult != DialogResult.Yes)
							{
								send = false;
								return;
							}
							break;
					}
					AddContact(2, dialog.SenderText, dialog.ContactString);
					if(send)
					{
						send = false;
						Send();
					}
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private void SelectSenderDialogFax_DialogEvent(object source, DialogEventArgs e)
		{
			Enabled = true;
			Select.SelectSenderDialog dialog = e.Dialog as Select.SelectSenderDialog;
			if(dialog.DialogResult == DialogResult.OK)
			{
				Web.ContactDialog ccDialog = new Web.ContactDialog(Environment.CreateContactString, "personContactCategor=3&personContactText=" + "" + "&docview=yes");
				ccDialog.Owner = this;
				ccDialog.DialogEvent += CreateContactDialog_DialogEvent;
				ccDialog.Show();
				Enabled = false;
				return;
			}
			else
				if(dialog.DialogResult != DialogResult.Yes)
					return;
			AddContact(1, dialog.SenderText, dialog.ContactString);
		}


		private void CreateContactDialog_DialogEvent(object source, DialogEventArgs e)
		{
			Enabled = true;
			Web.ContactDialog dialog = e.Dialog as Web.ContactDialog;
			dialog.DialogEvent -= CreateContactDialog_DialogEvent;
			if(dialog.DialogResult == DialogResult.OK)
			{
				if(dialog.ContactID > 0)
				{
					DataRow dr = Environment.FaxRecipientData.GetPersonContact(dialog.ContactID);
					if(dr != null)
					{
						if(!dr.IsNull(Environment.PersonData.IDField))
							AddContact(dr, new KeyValuePair<int, string>((int)dr[Environment.PersonData.IDField], Environment.PersonData.GetPerson((int)dr[Environment.PersonData.IDField])));
						else
							AddContact(dr, new KeyValuePair<int, string>());
					}
					else
						MessageForm.Show(Environment.StringResources.GetString("Dialog_SendFaxDialog_SendFax_Error2"), Environment.StringResources.GetString("Error"));
				}
			}
		}

		private void AddContact(int type, string text, string contact)
		{
			if(sendmail == null)
				sendmail = new List<Tuple<int, string, string>>();
			sendmail.Add(new Tuple<int, string, string>(type, text, contact));
			richTextBoxContact.AddContact(new Tuple<int, string, string>(type, text, contact));
		}

		private void AddContact(DataRow dr, KeyValuePair<int, string> keyValuePair)
		{
			contacts.Add(new KeyValuePair<int,DataRow>(keyValuePair.Key, dr));
			richTextBoxContact.AddContact(dr, keyValuePair);
		}

		private void AddContact(DataRow dr, KeyValuePair<int, string> main, KeyValuePair<int, string> second)
		{
			contacts.Add(new KeyValuePair<int, DataRow>(main.Key, dr));
			richTextBoxContact.AddContact(dr, main, second);
		}

		private void richTextBoxContact_TextChanged(object sender, EventArgs e)
		{
			if(richTextBoxContact.Text.Trim().Length > 0)
				return;
		}

		private void richTextBoxContact_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			if(e.LinkText.EndsWith("#Edit"))
			{
				OnEditPush();
				cont_EditPush(e.LinkText);
			}
			else
				Environment.IEOpenOnURL(e.LinkText.Substring(e.LinkText.IndexOf('#') + 1));
		}

		private void cont_EditPush(string url)
		{
			Match m = Regex.Match(url, "(\\d+)#Edit");
			if(!m.Success)
				return;
			int personID = int.Parse(m.Groups[1].ToString());
			DataRow dr = contacts.FirstOrDefault(x => x.Key == personID).Value;
			if(dr == null)
				return;
			Web.ContactDialog ccDialog = new Web.ContactDialog(Environment.CreateContactString,
											 "docview=yes&id=" + dr["КодКонтакта"].ToString() + "&idclient=" +  personID.ToString(),
											 Environment.StringResources.GetString("ContactEdit"));
			ccDialog.DialogEvent += ccDialog_DialogEvent;
			ccDialog.Show();
		}

		private void ccDialog_DialogEvent(object source, DialogEventArgs e)
		{
			var ccDialog = e.Dialog as Web.ContactDialog;
			ccDialog.DialogEvent -= ccDialog_DialogEvent;
			//ReloadContact();
		}

		public static void Send(int[ ] docIDs, object[ ] mainImageIDs)
		{
			if(docIDs.Length < 1 || mainImageIDs == null || mainImageIDs.Length == 0)
				return;
			if(docIDs.Length != mainImageIDs.Length)
			{
				MessageForm.Show(Environment.StringResources.GetString(""));
			}

			SendOutDialog dialog = new SendOutDialog();
			for(int i = 0; i < docIDs.Length; i++)
			{
				dialog.ImageAdd(docIDs[i], mainImageIDs[i]); 
			}
			dialog.Show();
		}

		public static void Send(object[ ] curFileNames, string[ ] curDocStrings)
		{
			if(curFileNames.Length < 1)
				return;
			
			SendOutDialog dialog = new SendOutDialog();
			for(int i = 0; i < curFileNames.Length; i++)
			{
				dialog.FileAdd(curFileNames[i].ToString(), curDocStrings[i]);
			}
			dialog.Show();
		}

		private void formatRadio_CheckedChanged(object sender, EventArgs e)
		{
			Components.ImageToSend its = tabControlSettings.Tag as Components.ImageToSend;
			if(its != null && !buttonApply.Visible)
				buttonApply.Visible = its.FormatType != (pdfRadio.Checked?2:tifRadio.Checked?1:0);
		}

		private void rbSelectedPages_CheckedChanged(object sender, EventArgs e)
		{
			Components.ImageToSend its = tabControlSettings.Tag as Components.ImageToSend;
			if(!buttonApply.Visible)
				buttonApply.Visible = its.PagesType != 1;
			textBoxStartPage.Enabled = textBoxEndPage.Enabled = rbSelectedPages.Checked;
		}

		private void rbAllPages_CheckedChanged(object sender, EventArgs e)
		{
			Components.ImageToSend its = tabControlSettings.Tag as Components.ImageToSend;
			if(!buttonApply.Visible)
				buttonApply.Visible = its.PagesType != 0;
		}

		private void rbSelectNum_CheckedChanged(object sender, EventArgs e)
		{
			Components.ImageToSend its = tabControlSettings.Tag as Components.ImageToSend;
			if(!buttonApply.Visible)
				buttonApply.Visible = its.PagesType != 2;
			textBoxSelectNum.Enabled = rbSelectNum.Checked;
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			Components.ImageToSend its = tabControlSettings.Tag as Components.ImageToSend;
			if(its == null)
				return;
			send = true;
			int num = -1;
			var doc = infos.FirstOrDefault(x => x.imageToSends != null && x.imageToSends.Contains(its));
			if(doc != null)
				 num = doc.imageToSends.IndexOf(its);
			if(radioButtonMainFormat.Checked && rbAllPages.Checked && radioButtonColor.Checked && doc != null && !doc.imageToSends[num].HasStamp)
			{
				FileInfo fi = new FileInfo(its.SourceFileName);
				doc.imageToSends[num].Size = fi.Length;
				doc.imageToSends[num].Pages = "";
				doc.imageToSends[num].PagesType = 0;
				doc.imageToSends[num].FormatType = 0;
				doc.imageToSends[num].BlackWight = null;
				if(!string.IsNullOrEmpty(doc.imageToSends[num].SendFileName) && File.Exists(doc.imageToSends[num].SendFileName))
				{
					File.Delete(doc.imageToSends[num].SendFileName);
					doc.imageToSends[num].SendFileName = null;
				}
				foreach(var cont in flowLayoutPanel.Controls)
				{
					///?
					Controls.CheckebleImageLinkLabel li = cont as Controls.CheckebleImageLinkLabel;
					if(li != null && li.Tag.Equals(its))
					{
						li.Text = doc.docString + "(" + (fi.Length > 1024 ? fi.Length > 820000 ? (fi.Length / 1048576f).ToString("N2") + "MB" : (fi.Length / 1024f).ToString("N2") + "KB" : fi.Length.ToString() + "B") + ")";
						li.Tag = doc.imageToSends[num];
						linkLabel_LinkClicked(li, new LinkLabelLinkClickedEventArgs(null));
						break;
					}
				}

				return;
			}		
			if(doc == null)
			{
				CleanTabSettings();
				splitContainer1.Panel2Collapsed = true;
				return;
			}
			doc.imageToSends[num].PagesType = rbSelectedPages.Checked ? 1 : rbSelectNum.Checked ? 2 : 0;
			doc.imageToSends[num].Pages ="";
			if(rbSelectedPages.Checked)
				doc.imageToSends[num].Pages = string.Format("{0}-{1}", (int)textBoxStartPage.Value, (int)textBoxEndPage.Value);
			if( rbSelectNum.Checked)
				doc.imageToSends[num].Pages = textBoxSelectNum.Text;
			doc.imageToSends[num].FormatType = pdfRadio.Checked?2: tifRadio.Checked?1:0;
			if(radioButtonBlackWhite.Checked || radioButtonColor.Checked)
				doc.imageToSends[num].BlackWight = radioButtonBlackWhite.Checked;
			doc.imageToSends[num].SendNotes = checkBoxSendNotes.Checked;
			doc.imageToSends[num].BurnNotes = checkBoxSendNotes.Checked && checkBoxBurnNotes.Checked;
			doc.imageToSends[num].BurnStamps = radioButtonBurnStamps.Checked;
			if(!string.IsNullOrEmpty(its.SendFileName))
			{
				File.Delete(its.SendFileName);
				its.SendFileName = null;
			}

			Task task = Task<bool>.Factory.StartNew(new Func<object, bool>(MakeImage), (object)(new object[ ] { its, (int)textBoxEndPage.Maximum}));
			Task.Factory.ContinueWhenAny(new Task[] {task}, new Action<Task>(EndConvert));
		}

		private void buttonApplyAll_Click(object sender, EventArgs e)
		{

		}


		public bool MakeImage(object obj)
		{
			List<int> pages = null;
			object[ ] objs = obj as object[ ];
			Components.ImageToSend pd = objs[0] as Components.ImageToSend;
			int count = (int)objs[1];
			
			int startPage = 0;
			int endPage = 0;
			bool isPDF = Environment.IsPdf(pd.SourceFileName);
			if(pd.PagesType < 2)
			{
				if(pd.PagesType == 1)
				{
					string[] ar = pd.Pages.Split('-');
					startPage = int.Parse(ar[0]);
					endPage = int.Parse(ar[1]);
				}
				if(startPage < 1)
					startPage = 1;
				if(endPage < startPage)
					endPage = startPage;
				if(endPage > count || pd.PagesType == 0)
					endPage = count;
				if(endPage < startPage)
					endPage = startPage;
				pages = new List<int>();
				for(int i = startPage; i <= endPage; i++)
					pages.Add( i);
			}
			else
			{
				string[ ] nums = pd.Pages.Split(" ,;.".ToCharArray());
				List<int> ints = new List<int>();
				int lastGood = -1;
				int val = -1;
				for(int j = 0; j < nums.Length; j++)
				{
					if(nums[j].Length < 1)
						continue;
					if(nums[j].Contains('-'))
					{
						//if(nums.Length < 2)
						//    continue;
						if(nums[j].StartsWith("-"))
						{
							if(nums[j].Replace("-", "").Length + 1 < nums[j].Length)
								break;
							int start = lastGood;
							if(start < 1)
							{
								nums[j] = nums[j].Replace("-", "");
								lastGood = j;
							}
						}
						else
						{
							if(lastGood > -1)
								if(int.TryParse(nums[lastGood], out val) && val > 0 && val <= docControl.PageCount)
								{
									if(!ints.Contains(val))
										ints.Add(val);
								}
							if(nums[j].EndsWith("-"))
							{
								if(int.TryParse(nums[j].Replace("-", ""), out val) && val > 0 && val <= docControl.PageCount)
								{
									lastGood = -1;
								}
							}
							else
							{
								string[ ] vals = nums[j].Split('-');
								int start = -1;
								if(int.TryParse(vals[0], out start) && int.TryParse(vals[1], out val))
								{
									ints.Add(start);
									while(start <= val)
										ints.Add(start++);
									lastGood = -1;
									val = -1;
								}
							}
						}
					}
					else
					{
						if(val > 0)
						{
							int start = val;
							if(int.TryParse(nums[j], out val) && val > 0 && val <= count)
							{
								ints.Add(start);
								while(start <= val)
									ints.Add(start++);
								lastGood = -1;
								val = -1;
							}
						}
						else if(lastGood > -1)
						{
							if(int.TryParse(nums[lastGood], out val) && val > 0 && val <= count)
							{
								if(!ints.Contains(val))
									ints.Add(val);
							}
						}
						lastGood = j;
						val = -1;
					}
					if(j == nums.Length - 1)
					{
						if(int.TryParse(nums[j], out val) && val > 0 && val <= count)
						if(lastGood == j - 1)
						{
							int start = 0 ;
							if(int.TryParse(nums[j - 1], out start) && start > 0 && start <= count)
							{
								ints.Add(start);
								while(start <= val)
									ints.Add(start++);
								lastGood = -1;
								val = -1;
							}
						}
						else
							ints.Add(val);
							
					}
				}
				pages = ints.Distinct().OrderBy(x => x).Where(x=> x<= count).ToList();
			}

			if(pages.Count == 0)
				return false;

			if(pd.FormatType == 2 || (pd.FormatType == 0 && isPDF))
			{
				if(!string.IsNullOrEmpty(pd.SendFileName))
					File.Delete(pd.SendFileName);
				else
					pd.SendFileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
				if(isPDF)
				{
					if(pd.HasStamp)
					{ }
					else
					Environment.PDFHelper.SavePart(pd.SourceFileName, pd.SendFileName, pages);
				}
				else
				{
					TiffToPDF(pd.SourceFileName, pd.SendFileName, pages, (pd.BlackWight.HasValue && pd.BlackWight.Value ? null : pages), pd.ImageID);
				}
			}
			else if(pd.FormatType == 1 || !isPDF)
			{
				if(!string.IsNullOrEmpty(pd.SendFileName))
					File.Delete(pd.SendFileName);
				else
					pd.SendFileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
				ushort[] ar = pages.Select(x => (ushort)(x - 1)).ToArray();
				if(isPDF)
				{
					PDFToTiff(pd.SourceFileName, pd.SendFileName, pages, (pd.BlackWight.HasValue && pd.BlackWight.Value ? null : pages), pd.ImageID, pd.BurnStamps);
				}
				else
				{
					if(!pd.HasStamp && (!pd.SendNotes || !pd.BurnNotes))
					{
						Environment.LibTiff.ExtLT.TIFCopyFile(pd.SourceFileName, pd.SendFileName, ar, pd.BlackWight.HasValue && pd.BlackWight.Value ? null : ar, 0, pd.SendNotes);
						return false;
					}
					return MakeTiffImage(pd, ar);
				}
			}

			return false;
		}

		private bool MakeTiffImage(Components.ImageToSend its, ushort[] ar)
		{
			if(its.BurnNotes || its.BurnStamps)
			{
				IntPtr tifw = IntPtr.Zero;
				ImageControl.TiffAnnotation tiffAnnotation = null;
				List<StampItem> stampItems = (its.ImageID > 0) ? Environment.DocSignatureData.GetStamps(its.ImageID) : null;
				try
				{
					if(string.IsNullOrEmpty(its.SendFileName))
						its.SendFileName = Path.GetTempFileName();
					File.Delete(its.SendFileName);
					tifw = Environment.LibTiff.ExtLT.TIFOpenW(its.SendFileName, "w");
					for(int n = 0; n < ar.Length; n++)
					{
						Tiff.PageInfo info = Environment.LibTiff.GetImageFromTiff(its.SourceFileName, ar[n]);
						if(info != null)
						{
							System.Drawing.Imaging.PixelFormat pf;
							Bitmap bmp = info.Image;
							pf = bmp.PixelFormat;
							if(bmp.HorizontalResolution > MaxSendResolution )
								if(bmp.VerticalResolution > MaxSendResolution)
									bmp.SetResolution(MaxSendResolution, MaxSendResolution);
								else
									bmp.SetResolution(MaxSendResolution, bmp.VerticalResolution);

							if(bmp.VerticalResolution > MaxSendResolution)
								bmp.SetResolution(bmp.HorizontalResolution, MaxSendResolution);
							if(its.BurnNotes || stampItems != null && stampItems.Count > 0)
							{

								IEnumerable<Data.Temp.Objects.StampItem> pageStamps = null;
								if(stampItems != null)
									pageStamps = stampItems.Where(x => x.Page == ar[n]);

								if(pageStamps != null && pageStamps.Any() || info.Annotation != null)
								{
									if(bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb || bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
									{
										Bitmap tm = new Bitmap(bmp);
										tm.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
										bmp = tm;
									}
									float drez = bmp.VerticalResolution / bmp.HorizontalResolution;
									System.Drawing.Drawing2D.Matrix mx = null;
									using(Graphics g = Graphics.FromImage(bmp))
									{
										if(its.BurnNotes && info.Annotation != null)
										{
											tiffAnnotation = new ImageControl.TiffAnnotation(this);
											tiffAnnotation.Parse(info.Annotation);
											ArrayList figuresList = tiffAnnotation.GetFigures(false);
											foreach(object figure in figuresList)
											{
												ImageControl.TiffAnnotation.IBufferBitmap bb = figure as ImageControl.TiffAnnotation.IBufferBitmap;
												if(bb != null)
												{
													switch(figure.GetType().Name)
													{
														case "ImageEmbedded":
															ImageControl.TiffAnnotation.ImageEmbedded img = (ImageControl.TiffAnnotation.ImageEmbedded)figure;
															g.DrawImage(img.Img, img.LrBounds.Location.X, img.LrBounds.Location.Y, img.LrBounds.Size.Width, img.LrBounds.Size.Height);
															break;
														case "StraightLine":
															ImageControl.TiffAnnotation.StraightLine line = (ImageControl.TiffAnnotation.StraightLine)figure;
															if(line.LinePoints == null)
																continue;
															g.DrawLine(new Pen(new SolidBrush(line.RgbColor1), Convert.ToSingle(line.ULineSize)), line.LinePoints[0], line.LinePoints[1]);
															break;
														case "FreehandLine":
															ImageControl.TiffAnnotation.FreehandLine fline = (ImageControl.TiffAnnotation.FreehandLine)figure;
															if(fline.LinePoints == null)
																continue;
															for(int i = 0; i < fline.LinePoints.Length; i += 2)
															{
																if(i != 0)
																	g.DrawLine(new Pen(new SolidBrush(fline.RgbColor1), Convert.ToSingle(fline.ULineSize)), fline.LinePoints[i - 1], fline.LinePoints[i]);
																g.DrawLine(new Pen(new SolidBrush(fline.RgbColor1), Convert.ToSingle(fline.ULineSize)), fline.LinePoints[i], fline.LinePoints[i + 1]);
															}
															break;
														case "HollowRectangle":
															{
																ImageControl.TiffAnnotation.HollowRectangle rect = (ImageControl.TiffAnnotation.HollowRectangle)figure;
																Bitmap bitmapUp = bb.GetBitmap(bmp, InterpolationMode.Bilinear);
																g.DrawImage(bitmapUp, rect.LrBounds.X, rect.LrBounds.Y);
															}
															break;
														case "FilledRectangle":
															{
																ImageControl.TiffAnnotation.FilledRectangle frect = (ImageControl.TiffAnnotation.FilledRectangle)figure;
																Bitmap bitmapUp = bb.GetBitmap(bmp, InterpolationMode.Bilinear);
																g.DrawImage(bitmapUp, frect.LrBounds.X, frect.LrBounds.Y);
															}
															break;
														case "TypedText":
															ImageControl.TiffAnnotation.TypedText tt = (ImageControl.TiffAnnotation.TypedText)figure;
															StringFormat sf = new StringFormat();
															mx = null;
															Rectangle newRect = tt.LrBounds;
															switch(tt.TextPrivateData.NCurrentOrientation)
															{
																case 900:
																	mx = new System.Drawing.Drawing2D.Matrix();
																	newRect = new Rectangle(tt.LrBounds.X, tt.LrBounds.Y + tt.LrBounds.Height, tt.LrBounds.Height, tt.LrBounds.Width);
																	mx.RotateAt(270, new PointF(newRect.X, newRect.Y));
																	g.Transform = mx;
																	break;
																case 1800:
																	mx = new System.Drawing.Drawing2D.Matrix();
																	newRect = tt.LrBounds;
																	mx.RotateAt(180, new PointF(tt.LrBounds.Location.X + tt.LrBounds.Width / 2, tt.LrBounds.Location.Y + tt.LrBounds.Height / 2));
																	g.Transform = mx;
																	break;
																case 2700:
																	mx = new System.Drawing.Drawing2D.Matrix();
																	newRect = new Rectangle(tt.LrBounds.X + tt.LrBounds.Width, tt.LrBounds.Y, tt.LrBounds.Height, tt.LrBounds.Width);
																	mx.RotateAt(90, new PointF(newRect.X, newRect.Y));
																	g.Transform = mx;

																	break;

															}

															g.TextRenderingHint = tt.FontRenderingHint;
															sf.Trimming = StringTrimming.Word;
															using(Font f = new Font(tt.LfFont.FontFamily, tt.LfFont.SizeInPoints, tt.LfFont.Style))
																g.DrawString(tt.TextPrivateData.SzAnoText, f, new SolidBrush(tt.RgbColor1), newRect, sf);

															g.ResetTransform();
															g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
															break;
														case "TextStump":
															ImageControl.TiffAnnotation.TextStump ts = (ImageControl.TiffAnnotation.TextStump)figure;
															StringFormat sf3 = new StringFormat();
															g.TextRenderingHint = ts.FontRenderingHint;

															g.DrawString(ts.TextPrivateData.SzAnoText, ts.LfFont, new SolidBrush(ts.RgbColor1), ts.LrBounds, sf3);
															g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
															break;
														case "TextFromFile":
															ImageControl.TiffAnnotation.TextFromFile tf = (ImageControl.TiffAnnotation.TextFromFile)figure;
															StringFormat sf2 = new StringFormat();
															g.TextRenderingHint = tf.FontRenderingHint;

															g.DrawString(tf.TextPrivateData.SzAnoText, tf.LfFont, new SolidBrush(tf.RgbColor1), tf.LrBounds, sf2);
															g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
															break;
														case "AttachANote":
															ImageControl.TiffAnnotation.AttachANote an = (ImageControl.TiffAnnotation.AttachANote)figure;
															StringFormat sf1 = new StringFormat();

															g.TextRenderingHint = an.FontRenderingHint;

															g.FillRectangle(Brushes.Black, an.LrBounds.X + 2, an.LrBounds.Y + 2, an.LrBounds.Width, an.LrBounds.Height);
															g.FillRectangle(new SolidBrush(an.RgbColor1), an.LrBounds);
															g.DrawRectangle(Pens.Black, an.LrBounds.X, an.LrBounds.Y, an.LrBounds.Width, an.LrBounds.Height);

															System.Drawing.Drawing2D.Matrix mx1 = null;
															Rectangle newRect1 = an.LrBounds;
															switch(an.TextPrivateData.NCurrentOrientation)
															{
																case 900:
																	mx1 = new System.Drawing.Drawing2D.Matrix();
																	newRect1 = new Rectangle(an.LrBounds.X, an.LrBounds.Y + an.LrBounds.Height, an.LrBounds.Height, an.LrBounds.Width);
																	mx1.RotateAt(270, new PointF(newRect1.X, newRect1.Y));
																	g.Transform = mx1;
																	break;
																case 1800:
																	mx1 = new System.Drawing.Drawing2D.Matrix();
																	newRect1 = an.LrBounds;
																	mx1.RotateAt(180, new PointF(an.LrBounds.Location.X + an.LrBounds.Width / 2, an.LrBounds.Location.Y + an.LrBounds.Height / 2));
																	g.Transform = mx1;
																	break;
																case 2700:
																	mx1 = new System.Drawing.Drawing2D.Matrix();
																	newRect1 = new Rectangle(an.LrBounds.X + an.LrBounds.Width, an.LrBounds.Y, an.LrBounds.Height, an.LrBounds.Width);
																	mx1.RotateAt(90, new PointF(newRect1.X, newRect1.Y));
																	g.Transform = mx1;
																	break;
															}
															g.DrawString(an.TextPrivateData.SzAnoText, new Font(an.LfFont.FontFamily, an.LfFont.SizeInPoints, an.LfFont.Style), new SolidBrush(an.RgbColor2), newRect1, sf1);
															g.ResetTransform();
															g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
															break;
													}
												}
											}
											tiffAnnotation.Dispose();
										}
										Image stamp;
										if(pageStamps != null)
											if(its.BurnStamps)
											foreach(Data.Temp.Objects.StampItem si in pageStamps)
											{
												if(si.TypeID == 101)
													stamp = Environment.GetDSP();
												else
													stamp = Environment.GetStamp(si.StampID, its.ImageID);
												if(!si.Rotate.Equals(0))
												{
													mx = new System.Drawing.Drawing2D.Matrix();
													mx.RotateAt(si.Rotate, new PointF(si.X + (stamp.Width * si.Zoom) / 200, si.Y + (stamp.Height * si.Zoom * drez) / 200));
													g.Transform = mx;
												}
												g.DrawImage(stamp, si.X, si.Y, (stamp.Width * si.Zoom) / 100, (stamp.Height * si.Zoom * drez) / 100);
												if(!si.Rotate.Equals(0))
													g.ResetTransform();
											}
										else
											{
												tiffAnnotation = new ImageControl.TiffAnnotation(this);
												if(!its.BurnNotes && info.Annotation != null)
													tiffAnnotation.Parse(info.Annotation);
												foreach(StampItem si in pageStamps)
												{
													if(si.TypeID == 101)
														continue;
													Image img = Environment.GetStamp(si.StampID, its.ImageID);
													tiffAnnotation.CreateImage(new Rectangle(si.X, si.Y, (img.Width * si.Zoom) / 32, (img.Height * si.Zoom) / 32), img, "Stamp");

												}
												info.Annotation = tiffAnnotation.GetAnnotationBytes(true);
												tiffAnnotation.Dispose();
											}
									}
								}
							}
							if(its.BlackWight.HasValue && its.BlackWight.Value && bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
								Environment.LibTiff.SetImageToTiff(tifw, Environment.LibTiff.ConvertToBitonal(bmp), its.SendNotes && (!its.BurnNotes || !its.BurnStamps) && info.Annotation != null && info.Annotation.Length > 8 ? info.Annotation : null);
							else
								if(bmp.PixelFormat != pf)
								Environment.LibTiff.SetImageToTiff(tifw, Environment.LibTiff.ConvertTo(pf, bmp), its.SendNotes &&(!its.BurnNotes || !its.BurnStamps) && info.Annotation != null && info.Annotation.Length > 8 ? info.Annotation : null);
							else
								Environment.LibTiff.SetImageToTiff(tifw, bmp, its.SendNotes && (!its.BurnNotes || !its.BurnStamps) && info.Annotation != null && info.Annotation.Length > 8 ? info.Annotation : null);
							Environment.LibTiff.SavePage(tifw);
						}
					}
					Environment.LibTiff.TiffCloseWrite(ref tifw);

				}
				catch(Exception ex)
				{
					Error.ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
					if(!IntPtr.Zero.Equals(tifw))
						Environment.LibTiff.TiffCloseWrite(ref tifw);
					Slave.DeleteFile(its.SendFileName);
					return true;

				}
			}
			return false;
		}

		private bool PDFToTiff(string input, string output, List< int> pages, List<int> colorPages, int imageID, bool burnStamp)
		{
			List<Tiff.PageInfo> il = new List<Tiff.PageInfo>();

			List<StampItem> stL = (imageID > 0) ? Environment.DocSignatureData.GetStamps(imageID) : null;

			bool saveColor = pages != null && pages.Count > 0;
			il = Environment.PDFHelper.GetBitmapsCollectionFromFile(input, pages, MaxSendResolution, MaxSendResolution, colorPages.Count > 0);

			for(int i = 0; i < il.Count; i++)
			{
				Tiff.PageInfo info = il[i];
				if(info != null)
				{
					System.Drawing.Imaging.PixelFormat pf;
					Bitmap bmp = info.Image;
					pf = bmp.PixelFormat;
					if(bmp.HorizontalResolution > MaxSendResolution)
						if(bmp.VerticalResolution > MaxSendResolution)
							bmp.SetResolution(MaxSendResolution, MaxSendResolution);
						else
							bmp.SetResolution(MaxSendResolution, bmp.VerticalResolution);

					if(bmp.VerticalResolution > MaxSendResolution)
						bmp.SetResolution(bmp.HorizontalResolution, MaxSendResolution);
					if(stL != null && stL.Count > 0)
					{
						var pageStamps = stL.Where(x => x.Page == pages[i]);
						if(pageStamps != null && pageStamps.Count() > 0)
						{
							if(burnStamp)
							{
								if(bmp.PixelFormat != PixelFormat.Format24bppRgb ||
									bmp.PixelFormat != PixelFormat.Format32bppArgb)
								{
									Bitmap tm = new Bitmap(bmp);
									tm.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
									bmp = tm;
									tm = null;
								}
								float drez = bmp.VerticalResolution / bmp.HorizontalResolution;
								Matrix mx = null;
								using(Graphics g = Graphics.FromImage(bmp))
								{
									foreach(StampItem si in pageStamps)
									{
										if(si.TypeID == 101)
											continue;
										Image img = Environment.GetStamp(si.StampID, imageID);
										if(!si.Rotate.Equals(0))
										{
											mx = new Matrix();
											mx.RotateAt(si.Rotate,
														new PointF(si.X + (img.Width * si.Zoom) / 64, si.Y + (img.Height * si.Zoom * drez) / 64));
											g.Transform = mx;
										}
										g.DrawImage(img, si.X* 300/96, si.Y* 300/96, (img.Width * si.Zoom)/32, (img.Height * si.Zoom * drez) /32);
										if(!si.Rotate.Equals(0))
											g.ResetTransform();
									}
								}
							}
							else
							{
								ImageControl.TiffAnnotation tiffAnnotation = new ImageControl.TiffAnnotation(this.docControl);
								foreach(StampItem si in pageStamps)
								{
									if(si.TypeID == 101)
										continue;
									Image img = Environment.GetStamp(si.StampID, imageID);
									tiffAnnotation.CreateImage(new Rectangle(si.X, si.Y, (img.Width * si.Zoom) / 32, (img.Height * si.Zoom) / 32), img, "Stamp");
									
								}
								info.Annotation = tiffAnnotation.GetAnnotationBytes(true);
								tiffAnnotation.Dispose();
							}	
						}
					}

					if(!saveColor && bmp.PixelFormat != PixelFormat.Format1bppIndexed)
						bmp = Environment.LibTiff.ConvertToBitonal(bmp);

					info.Image = bmp;
				}
			}
			if(il.Count > 0)
				Environment.LibTiff.SaveBitmapsCollectionToFile(output, il, saveColor);
			return true;
		}

		private void TiffToPDF(string input, string output, List<int> pages, List<int> colorPages, int imageID)
		{
			IntPtr tif = IntPtr.Zero;
			Bitmap bmp = null;
			tif = Environment.LibTiff.TiffOpenRead(ref input, out bmp, false);
			if(IntPtr.Zero.Equals(tif) && bmp == null)
				return;
			using(var file_stream = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
			using(var document = new iTextSharp.text.Document())
			using(PdfWriter pdfWriter = PdfWriter.GetInstance(document, file_stream))
			{
				document.SetMargins(0, 0, 0, 0);

				document.Open();

				int pagecount = 0;
				if(!IntPtr.Zero.Equals(tif))
				{
					try
					{
						pagecount = Environment.LibTiff.ExtLT.TIFNumberOfDirectories(tif);
						if(pagecount == 0)
							pagecount = 1;
						Environment.LibTiff.ExtLT.TIFSetDirectory(tif, (ushort)(pagecount - 1));
						int i = Environment.LibTiff.ExtLT.TIFLastDirectory(tif);
						if(i == 0)
							++pagecount;

					}
					catch
					{
					}
				}
				else 
					pagecount = 1;
				System.Drawing.Imaging.PixelFormat pf;
				ImageControl.TiffAnnotation tiffAnnotation = null;
				List<Data.Temp.Objects.StampItem> stampItems = (imageID > 0) ? Environment.DocSignatureData.GetStamps(imageID) : null;
				for(int i = 0; i < pages.Count; i++)
				{
				
					if(pages[i] < 1 || pages[i] > pagecount)
						continue;
					if(!IntPtr.Zero.Equals(tif))
					{
						var pi = Environment.LibTiff.GetImageFromTiff(tif, pages[i] - 1);
						if(pi == null)
							continue;
						bmp = pi.Image;
						pf = bmp.PixelFormat;
						if(bmp.HorizontalResolution > 300)
							if(bmp.VerticalResolution > 300)
								bmp.SetResolution(300, 300);
							else
								bmp.SetResolution(300, bmp.VerticalResolution);

						if(bmp.VerticalResolution > 300)
							bmp.SetResolution(bmp.HorizontalResolution, 300);


						IEnumerable<Data.Temp.Objects.StampItem> pageStamps = null;
						if(stampItems != null)
							pageStamps = stampItems.Where(x => x.Page == pages[i]);
						if((pi.Annotation!= null&& pi.Annotation.Length > 0) || (pageStamps != null && pageStamps.Any()))
						{
							if(bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb || bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
							{
								Bitmap tm = new Bitmap(bmp);
								tm.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
								bmp = tm;
							}
							float drez = bmp.VerticalResolution / bmp.HorizontalResolution;
							System.Drawing.Drawing2D.Matrix mx = null;
							using(Graphics g = Graphics.FromImage(bmp))
							{
								if(pi.Annotation != null)
								{
									tiffAnnotation = new ImageControl.TiffAnnotation(this);
									tiffAnnotation.Parse(pi.Annotation);
									ArrayList figuresList = tiffAnnotation.GetFigures(false);
									foreach(object figure in figuresList)
									{
										ImageControl.TiffAnnotation.IBufferBitmap bb = figure as ImageControl.TiffAnnotation.IBufferBitmap;
										if(bb != null)
										{
											switch(figure.GetType().Name)
											{
												case "ImageEmbedded":
													ImageControl.TiffAnnotation.ImageEmbedded img = (ImageControl.TiffAnnotation.ImageEmbedded)figure;
													g.DrawImage(img.Img, img.LrBounds.Location.X, img.LrBounds.Location.Y, img.LrBounds.Size.Width, img.LrBounds.Size.Height);
													break;
												case "StraightLine":
													ImageControl.TiffAnnotation.StraightLine line = (ImageControl.TiffAnnotation.StraightLine)figure;
													if(line.LinePoints == null)
														continue;
													g.DrawLine(new Pen(new SolidBrush(line.RgbColor1), Convert.ToSingle(line.ULineSize)), line.LinePoints[0], line.LinePoints[1]);
													break;
												case "FreehandLine":
													ImageControl.TiffAnnotation.FreehandLine fline = (ImageControl.TiffAnnotation.FreehandLine)figure;
													if(fline.LinePoints == null)
														continue;
													for(int k = 0; k < fline.LinePoints.Length; k += 2)
													{
														if(k != 0)
															g.DrawLine(new Pen(new SolidBrush(fline.RgbColor1), Convert.ToSingle(fline.ULineSize)), fline.LinePoints[k - 1], fline.LinePoints[k]);
														g.DrawLine(new Pen(new SolidBrush(fline.RgbColor1), Convert.ToSingle(fline.ULineSize)), fline.LinePoints[k], fline.LinePoints[k + 1]);
													}
													break;
												case "HollowRectangle":
													{
														ImageControl.TiffAnnotation.HollowRectangle rect = (ImageControl.TiffAnnotation.HollowRectangle)figure;
														Bitmap bitmapUp = bb.GetBitmap(bmp, InterpolationMode.High);
														g.DrawImage(bitmapUp, rect.LrBounds.X, rect.LrBounds.Y);
													}
													break;
												case "FilledRectangle":
													{
														ImageControl.TiffAnnotation.FilledRectangle frect = (ImageControl.TiffAnnotation.FilledRectangle)figure;
														Bitmap bitmapUp = bb.GetBitmap(bmp, InterpolationMode.High);
														g.DrawImage(bitmapUp, frect.LrBounds.X, frect.LrBounds.Y);
													}
													break;
												case "TypedText":
													ImageControl.TiffAnnotation.TypedText tt = (ImageControl.TiffAnnotation.TypedText)figure;
													StringFormat sf = new StringFormat();
													mx = null;
													Rectangle newRect = tt.LrBounds;
													switch(tt.TextPrivateData.NCurrentOrientation)
													{
														case 900:
															mx = new System.Drawing.Drawing2D.Matrix();
															newRect = new Rectangle(tt.LrBounds.X, tt.LrBounds.Y + tt.LrBounds.Height, tt.LrBounds.Height, tt.LrBounds.Width);
															mx.RotateAt(270, new PointF(newRect.X, newRect.Y));
															g.Transform = mx;
															break;
														case 1800:
															mx = new System.Drawing.Drawing2D.Matrix();
															newRect = tt.LrBounds;
															mx.RotateAt(180, new PointF(tt.LrBounds.Location.X + tt.LrBounds.Width / 2, tt.LrBounds.Location.Y + tt.LrBounds.Height / 2));
															g.Transform = mx;
															break;
														case 2700:
															mx = new System.Drawing.Drawing2D.Matrix();
															newRect = new Rectangle(tt.LrBounds.X + tt.LrBounds.Width, tt.LrBounds.Y, tt.LrBounds.Height, tt.LrBounds.Width);
															mx.RotateAt(90, new PointF(newRect.X, newRect.Y));
															g.Transform = mx;

															break;

													}

													g.TextRenderingHint = tt.FontRenderingHint;
													sf.Trimming = StringTrimming.Word;
													using(Font f = new Font(tt.LfFont.FontFamily, tt.LfFont.SizeInPoints, tt.LfFont.Style))
														g.DrawString(tt.TextPrivateData.SzAnoText, f, new SolidBrush(tt.RgbColor1), newRect, sf);

													g.ResetTransform();
													g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
													break;
												case "TextStump":
													ImageControl.TiffAnnotation.TextStump ts = (ImageControl.TiffAnnotation.TextStump)figure;
													StringFormat sf3 = new StringFormat();
													g.TextRenderingHint = ts.FontRenderingHint;

													g.DrawString(ts.TextPrivateData.SzAnoText, ts.LfFont, new SolidBrush(ts.RgbColor1), ts.LrBounds, sf3);
													g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
													break;
												case "TextFromFile":
													ImageControl.TiffAnnotation.TextFromFile tf = (ImageControl.TiffAnnotation.TextFromFile)figure;
													StringFormat sf2 = new StringFormat();
													g.TextRenderingHint = tf.FontRenderingHint;

													g.DrawString(tf.TextPrivateData.SzAnoText, tf.LfFont, new SolidBrush(tf.RgbColor1), tf.LrBounds, sf2);
													g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
													break;
												case "AttachANote":
													ImageControl.TiffAnnotation.AttachANote an = (ImageControl.TiffAnnotation.AttachANote)figure;
													StringFormat sf1 = new StringFormat();

													g.TextRenderingHint = an.FontRenderingHint;

													g.FillRectangle(Brushes.Black, an.LrBounds.X + 2, an.LrBounds.Y + 2, an.LrBounds.Width, an.LrBounds.Height);
													g.FillRectangle(new SolidBrush(an.RgbColor1), an.LrBounds);
													g.DrawRectangle(Pens.Black, an.LrBounds.X, an.LrBounds.Y, an.LrBounds.Width, an.LrBounds.Height);

													System.Drawing.Drawing2D.Matrix mx1 = null;
													Rectangle newRect1 = an.LrBounds;
													switch(an.TextPrivateData.NCurrentOrientation)
													{
														case 900:
															mx1 = new System.Drawing.Drawing2D.Matrix();
															newRect1 = new Rectangle(an.LrBounds.X, an.LrBounds.Y + an.LrBounds.Height, an.LrBounds.Height, an.LrBounds.Width);
															mx1.RotateAt(270, new PointF(newRect1.X, newRect1.Y));
															g.Transform = mx1;
															break;
														case 1800:
															mx1 = new System.Drawing.Drawing2D.Matrix();
															newRect1 = an.LrBounds;
															mx1.RotateAt(180, new PointF(an.LrBounds.Location.X + an.LrBounds.Width / 2, an.LrBounds.Location.Y + an.LrBounds.Height / 2));
															g.Transform = mx1;
															break;
														case 2700:
															mx1 = new System.Drawing.Drawing2D.Matrix();
															newRect1 = new Rectangle(an.LrBounds.X + an.LrBounds.Width, an.LrBounds.Y, an.LrBounds.Height, an.LrBounds.Width);
															mx1.RotateAt(90, new PointF(newRect1.X, newRect1.Y));
															g.Transform = mx1;
															break;
													}
													g.DrawString(an.TextPrivateData.SzAnoText, new Font(an.LfFont.FontFamily, an.LfFont.SizeInPoints, an.LfFont.Style), new SolidBrush(an.RgbColor2), newRect1, sf1);
													g.ResetTransform();
													g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
													break;
											}
										}

										tiffAnnotation.Dispose();
									}
								}
								Image stamp;
								if(pageStamps != null)
									foreach(Data.Temp.Objects.StampItem si in pageStamps)
									{
										if(si.TypeID == 101)
											stamp = Environment.GetDSP();
										else
											stamp = Environment.GetStamp(si.StampID, imageID);
										if(!si.Rotate.Equals(0))
										{
											mx = new System.Drawing.Drawing2D.Matrix();
											mx.RotateAt(si.Rotate, new PointF(si.X + (stamp.Width * si.Zoom) / 200, si.Y + (stamp.Height * si.Zoom * drez) / 200));
											g.Transform = mx;
										}
										g.DrawImage(stamp, si.X, si.Y, (stamp.Width * si.Zoom) / 100, (stamp.Height * si.Zoom * drez) / 100);
										if(!si.Rotate.Equals(0))
											g.ResetTransform();
									}
							}
						}

						if(!(colorPages !=null && colorPages.Count > 0) && bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
							bmp = Environment.LibTiff.ConvertToBitonal(bmp);
					}
					try
					{
						bool needRotate = bmp.Width > bmp.Height;
						if(needRotate)
							bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);

						iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(bmp, ImageFormat.Png);
						gif.ScaleAbsolute((float)(bmp.Width * (72.0 / bmp.HorizontalResolution)),
										  (float)(bmp.Height * (72.0 / bmp.VerticalResolution)));
						gif.SetAbsolutePosition(1, 1);

						document.SetPageSize(new iTextSharp.text.Rectangle(gif.ScaledWidth, gif.ScaledHeight));
						document.NewPage();

						pdfWriter.DirectContent.AddImage(gif);

						if(needRotate)
							pdfWriter.AddPageDictEntry(PdfName.ROTATE, new PdfNumber(270));
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
						return;
					}
				}
				document.Close();
			}
		}

		public void EndConvert(Task t)
		{
			object[] objs = t.AsyncState as object[];
			if(objs == null)
				return;
			Components.ImageToSend pd = objs[0] as Components.ImageToSend;
			if(pd == null)
				return;
			int num = -1;
			FileInfo fi = null;
			Components.ProcessingDocs doc = null;
			if(File.Exists(pd.SendFileName))
			{
				fi = new FileInfo(pd.SendFileName);
				doc = infos.FirstOrDefault(x => x.imageToSends != null && x.imageToSends.Contains(pd));
				num = doc.imageToSends.IndexOf(pd);
				if(num > -1)
					doc.imageToSends[num].Size = fi.Length;
				doc.imageToSends[num].Pages = pd.Pages;
				doc.imageToSends[num].PagesType = pd.PagesType;
				doc.imageToSends[num].FormatType = pd.FormatType;
				doc.imageToSends[num].BlackWight = pd.BlackWight;
			}
			if(num > -1)
				InvokeIfRequired(new MethodInvoker(() =>
					{
						foreach(var con in tableLayoutPanelImages.Controls)
						{
							Controls.CheckebleImageLinkLabel li = con as Controls.CheckebleImageLinkLabel;
							if(li != null && li.Tag.Equals(pd))
							{
								li.Text = doc.docString + "(" + (fi.Length > 1024 ? fi.Length > 820000 ? (fi.Length / 1048576f).ToString("N2") + "MB" : (fi.Length / 1024f).ToString("N2") + "KB" : fi.Length.ToString() + "B") + ")";
								linkLabel1_LinkClicked(li, new LinkLabelLinkClickedEventArgs(null));
								break;
							}
						}
					}));
		}

		private void tabControlSettings_Selected(object sender, TabControlEventArgs e)
		{
			if(e.TabPage == tabPage2)
			{
				textBoxStartPage.Hexadecimal = true;
				textBoxEndPage.Hexadecimal = true;
				textBoxStartPage.Hexadecimal = false;
				textBoxEndPage.Hexadecimal = false;
			}
		}

		private void textBoxPage_ValueChanged(object sender, EventArgs e)
		{
			if(!buttonApply.Visible)
				buttonApply.Visible=true;
			List<int> list = new List<int>();
			for(int i = (int)textBoxStartPage.Value; i <= textBoxEndPage.Value; i++)
				list.Add(i);
			docControl.SetVisiblePages(list);
		}

		private void textBoxSelectNum_TextChanged(object sender, EventArgs e)
		{
			if(textBoxSelectNum.Text.Trim().Length > 0 && !buttonApply.Visible)
				buttonApply.Visible = true;
		}

		private void checkBoxSendNotes_CheckedChanged(object sender, EventArgs e)
		{
			checkBoxBurnNotes.Enabled = checkBoxSendNotes.Checked;
			buttonApply.Visible = true;
		}

		private void checkBoxBurnNotes_CheckedChanged(object sender, EventArgs e)
		{
			if(!buttonApply.Visible)
				buttonApply.Visible = true;
		}

		private void radioButtonBurnStamps_CheckedChanged(object sender, EventArgs e)
		{
			if(!buttonApply.Visible)
				buttonApply.Visible = true;
		}

		private void radioButtonSendStamps_CheckedChanged(object sender, EventArgs e)
		{
			if(!buttonApply.Visible)
				buttonApply.Visible = true;
		}

		private void docControl_NeedSave(object sender, EventArgs e)
		{

			if(MessageBox.Show("Сохранить изменения", Environment.StringResources.GetString("Confirmation"), MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				var t = tabControlSettings.Tag as Components.ImageToSend;
				if(t!= null)
					docControl.SaveAs(t.SendFileName);
			}
		}

		#region Search Image

		private void toolStripButtonFind_Click(object sender, EventArgs e)
		{
			var doc = new XmlDocument();
			XmlElement elOptions = doc.CreateElement("Options");
			XmlElement elOption = doc.CreateElement("Option");
			elOption.SetAttribute("name", "Image.Изображение");
			elOption.SetAttribute("fixed", "true");
			elOptions.AppendChild(elOption);
			Search.OptionsDialog optionsDialog = new Search.OptionsDialog(elOptions.OuterXml.Clone() as string, Search.OptionsDialog.EnabledFeatures.Clear | Search.OptionsDialog.EnabledFeatures.Search);
			optionsDialog.DialogEvent += OptionsDialog_DialogEvent;
			optionsDialog.Show();
		}

		private void OptionsDialog_DialogEvent(object source, DialogEventArgs e)
		{
			if(e.Dialog.DialogResult == DialogResult.Cancel)
				return;
			var dialog = source as Search.OptionsDialog;
			if(dialog == null)
				return;
			if(Environment.DocData.FoundDocsCount() > 0)                    // есть найденные документы
			{
				// select doc
				var uniDialog = new Select.SelectDocUniversalDialog(Environment.DocData.GetFoundDocsIDQuery(Environment.CurEmp.ID), 0, dialog.GetXML(), true);
				uniDialog.ResultEvent += UniDialog_ResultEvent;
				uniDialog.Show();
			}
			//if(dialog.)
		}

		private void UniDialog_ResultEvent(object source, DialogResultEvent e)
		{
			switch(e.Result)
			{
				case DialogResult.OK:
					if(e.Params.Length == 3)
					{
						if(e.Params[1] is int)
						{
							var ID = (int)e.Params[1];
							if(ID > 0)
								ImageAdd(ID, null);
						}
						else if(e.Params[1] is int[])
						{
							var IDs = (int[])e.Params[1];
							foreach(int ID in IDs)
								ImageAdd(ID, null);
						}
					}
					break;
				case DialogResult.Retry:
					if(e.Params.Length == 3 && e.Params[0] is int && e.Params[2] is string)
					{
						var searchDialog = new Search.OptionsDialog(e.Params[2].ToString(), Search.OptionsDialog.EnabledFeatures.Clear |  Search.OptionsDialog.EnabledFeatures.Search);
						searchDialog.DialogEvent += OptionsDialog_DialogEvent;
						searchDialog.Show();
					}
					break;
			}
		}

		#endregion

		public const int MaxSendResolution = 300;
		public const int MaxFaxHorizontalResolution = 204;
		public const int MaxFaxVerticalResolution = 196;

	}
}