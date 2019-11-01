using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.Document.EWSMail;
using iTextSharp.text.pdf;
using System.Collections;

namespace Kesco.Lib.Win.Document.Dialogs
{
	public partial class SendOutDialog : Win.FreeDialog
	{
		bool send = false;

		public SendOutDialog()
		{
			InitializeComponent();
			this.docControl.ExternalSave = true;
			this.toolStripButton2.Visible = false;
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

		private SynchronizedCollection<object[]> InternalObjects;

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
						LinkLabel li = con as LinkLabel;
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
            if (splitContainer1.Panel2Collapsed)
                splitContainer1.Panel2Collapsed = false;
			LinkLabel ll = (LinkLabel)sender;
            linkLabel1.Text = ll.Text;
			Components.ProcessingDocs a = ll.Tag as Components.ProcessingDocs ;

			LoadImage(a);
        }

		private void LoadImage(Components.ProcessingDocs a)
		{
			if(a != null)
			{
				bool isPDF = Environment.IsPdf(a.sourceFileName);
				int pagecount = 0;
				if(isPDF)
				{
					pdfRadio.Enabled = false;
					tifRadio.Enabled = true;
					Environment.PDFHelper.Open(a.sourceFileName, null);
					pagecount = Environment.PDFHelper.PageCount;
					Environment.PDFHelper.Close();
				}
				else
				{
					tifRadio.Enabled = false;
					pdfRadio.Enabled = true;
					pagecount = Environment.LibTiff.GetCountPages(a.sourceFileName);
				}

				textBoxEndPage.Maximum = pagecount;
				textBoxStartPage.Maximum = pagecount;
				textBoxEndPage.Value = pagecount;
				switch(a.pagesType)
				{
					case 1:
						rbSelectedPages.Checked = true;
						string[] ar = a.pages.Split('-');
						textBoxStartPage.Value = decimal.Parse(ar[0]);
						textBoxEndPage.Value = decimal.Parse(ar[1]);
						break;
					case 2:
						rbSelectNum.Checked = true;
						textBoxSelectNum.Text = a.pages;
						break;
					default:
						rbAllPages.Checked = true;
						break;
				}
				switch(a.formatType)
				{
					case 1:
						tifRadio.Checked = true;
						break;
					case 2:
						pdfRadio.Checked = true;
						break;
					default:
						radioButtonMainFormat.Checked = true;
						break;
				}
				if(a.bw.HasValue)
				{
					radioButtonBlackWhite.Checked = a.bw.Value;
					radioButtonColor.Checked = !a.bw.Value;
				}
				linkLabel1.Tag = a;
				docControl.LoadImage(a);
			}
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if(linkLabel1.Tag != null)
				Environment.OnNewWindow(this, new Components.DocumentSavedEventArgs(linkLabel1.Tag as string, linkLabel1.Text));
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

			LinkLabel li = new LinkLabel();
			li.AutoSize = true;

			li.Text = pd.docString + "(" + (pd.Size > 1024 ? pd.Size > 820000 ? (pd.Size / 1048576f).ToString("N2") + "MB" : (pd.Size / 1024f).ToString("N2") + "KB" : pd.Size.ToString() + "B") + ")";
			li.Tag = pd;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SendOutDialog));
			li.Image = (System.Drawing.Image)(resources.GetObject("ImageRemove"));
			if(li.Image != null)
			{
				li.ImageAlign = ContentAlignment.TopRight;
				li.Click += new EventHandler(li_Click);
				int wid = li.PreferredWidth;
				li.AutoSize = false;
				li.Width = wid + li.Image.Width;
			}
			flowLayoutPanel.Controls.Add(li);
			li.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel_LinkClicked);
			return true;
		}

		void li_Click(object sender, EventArgs e)
		{
			LinkLabel li = sender as LinkLabel;
			MouseEventArgs ar = e as MouseEventArgs;
			if(ar != null &&li != null && infos != null && infos.Count > 1)
			{
				if(ar.Location.X > li.Width - li.Image.Width - 8)
				{
					Components.ProcessingDocs pd = li.Tag as Components.ProcessingDocs;
					flowLayoutPanel.Controls.Remove(li);
					if(pd == null)
						return;
					if(!string.IsNullOrEmpty(pd.sendFileName) && File.Exists(pd.sendFileName))
						File.Delete(pd.sendFileName);
					int num = infos.IndexOf(pd);
					if(num > -1)
						infos.RemoveAt(num);
					if(pd.Equals(linkLabel1.Tag))
					{
						linkLabel1.Tag = null;
						splitContainer1.Panel2Collapsed = true;
					}
				}
			}
			
		}



		#endregion

		private void Send()
		{
			if(infos == null || infos.Count < 1)
				return;
			if((contacts == null || contacts.Count < 1) && (sendmail == null || sendmail.Count < 1))
			{
				if(richTextBoxContact.IsEmail)
					StartContactWork(4, false);
				return;
			}
			bool usePdf = pdfRadio.Checked;
			bool useTif = tifRadio.Checked;

			string exte = Path.GetExtension(infos[0].sourceFileName).TrimStart('.');
			if(usePdf)
				exte = "pdf";
			if(useTif)
				exte = "tif";

			string newFileName = Path.Combine(Path.GetTempPath(), Environment.GenerateFileName(exte));
			Console.WriteLine("new filename: " + newFileName);

			Cursor = Cursors.WaitCursor;
			// проверка на наличие файлов
			File.Copy(infos[0].sourceFileName, newFileName);

			if(!File.Exists(newFileName))
				return;
			else
				File.Delete(newFileName);
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

				for(int i = 0; i < infos.Count; i++)
				{
					if(!usePdf && !useTif)
						exte = Path.GetExtension(infos[i].sourceFileName).TrimStart('.');
					if(!string.IsNullOrEmpty(infos[i].sendFileName) && File.Exists(infos[i].sendFileName))
					{	
						File.Copy(infos[i].sendFileName, newFileName);
					}
					else
						File.Copy(infos[i].sourceFileName, newFileName);
									
					string sendDocFileName = Path.Combine(Path.GetTempPath(), "Document" + i.ToString() + "." + exte);
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
					byte[ ] binaryData;
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
				try
				{
					CreateItemResponseType crit = esb.CreateItem(cit);
					if(crit == null || crit.ResponseMessages.Items.Length == 0 || crit.ResponseMessages.Items[0].ResponseClass != ResponseClassType.Success)
						throw new System.Exception("Can't save message");
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
			foreach(Components.ProcessingDocs pd in infos.Where(x => x.docImageID > 0))
			{
				if(contacts != null && contacts.Count > 0)
					Environment.LogEmailData.LogEmail(pd.docImageID, Environment.PersonData.GetPerson(contacts[0].Key), contacts[0].Value[Environment.FaxRecipientData.ContactField].ToString());
				else if(richTextBoxContact.SearchText.Length > 0)
					Environment.LogEmailData.LogEmail(pd.docImageID, "", richTextBoxContact.SearchText);
			}
			foreach(Components.ProcessingDocs pd in infos.Where(x => x.faxID > 0))
				Task.Factory.StartNew(() => { Environment.FaxOutData.MarkResend(pd.faxID); });
			foreach(Components.ProcessingDocs pd in infos.Where(x => !string.IsNullOrEmpty(x.sendFileName)))
				Slave.DeleteFile(pd.sendFileName);
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
			if(infos.FirstOrDefault(x => x.sourceFileName == filename) == null)
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
			if(!(imageID is int)  || infos.FirstOrDefault(x=> x.docImageID == (int)imageID) == null)
			{
				Components.ProcessingDocs docc = new Components.ProcessingDocs(docID, imageID);
				if(infos.FirstOrDefault(x => x.docImageID == docc.docImageID) == null && !docc.Error)
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
			if(infos.FirstOrDefault(x => x.faxID == faxID) == null)
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
			if((contacts == null || contacts.Count< 1) && (sendmail == null || sendmail.Count < 1))
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
			Components.ProcessingDocs pd = linkLabel1.Tag as Components.ProcessingDocs;
			if(!buttonApply.Visible)
				buttonApply.Visible = pd.formatType != (pdfRadio.Checked?2:tifRadio.Checked?1:0);
		}

		private void rbSelectedPages_CheckedChanged(object sender, EventArgs e)
		{
			Components.ProcessingDocs pd = linkLabel1.Tag as Components.ProcessingDocs;
			if(!buttonApply.Visible)
				buttonApply.Visible = pd.pagesType != 1;
			textBoxStartPage.Enabled = textBoxEndPage.Enabled = rbSelectedPages.Checked;
		}

		private void rbAllPages_CheckedChanged(object sender, EventArgs e)
		{
			Components.ProcessingDocs pd = linkLabel1.Tag as Components.ProcessingDocs;
			if(!buttonApply.Visible)
				buttonApply.Visible = pd.pagesType != 0;
		}

		private void rbSelectNum_CheckedChanged(object sender, EventArgs e)
		{
			Components.ProcessingDocs pd = linkLabel1.Tag as Components.ProcessingDocs;
			if(!buttonApply.Visible)
				buttonApply.Visible = pd.pagesType != 2;
			textBoxSelectNum.Enabled = rbSelectNum.Checked;
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			Components.ProcessingDocs pd = linkLabel1.Tag as Components.ProcessingDocs;
			if(pd == null)
				return;
			send = true;
			if(radioButtonMainFormat.Checked && rbAllPages.Checked)
			{
				int num = infos.IndexOf(pd);
				if(num > -1)
				{
					FileInfo fi = new FileInfo(pd.sourceFileName);
					infos[num].Size = fi.Length;
					infos[num].pages = "";
					infos[num].pagesType = 0;
					infos[num].formatType = 0;
					infos[num].bw = null;
					if(!string.IsNullOrEmpty(infos[num].sendFileName) && File.Exists(infos[num].sendFileName))
					{
						File.Delete(infos[num].sendFileName);
						infos[num].sendFileName = null;
					}
					foreach(var con in flowLayoutPanel.Controls)
					{
						LinkLabel li = con as LinkLabel;
						if(li != null && li.Tag.Equals(pd))
						{
							li.Text = pd.docString + "(" + (fi.Length > 1024 ? fi.Length > 820000 ? (fi.Length / 1048576f).ToString("N2") + "MB" : (fi.Length / 1024f).ToString("N2") + "KB" : fi.Length.ToString() + "B") + ")";
							li.Tag = infos[num];
							linkLabel_LinkClicked(li, new LinkLabelLinkClickedEventArgs(null));
							break;
						}
					}

					return;
				}
			}
			
			Components.ProcessingDocs info = infos.FirstOrDefault(x => x.sourceFileName == pd.sourceFileName || (pd.docImageID > 0 && x.docImageID == pd.docImageID));
			if(info == null)
			{
				CleanTabSettings();
				splitContainer1.Panel2Collapsed = true;
				return;
			}
			info.pagesType = rbSelectedPages.Checked ? 1 : rbSelectNum.Checked ? 2 : 0;
			info.pages ="";
			if(rbSelectedPages.Checked)
			info.pages = string.Format("{0}-{1}", (int)textBoxStartPage.Value, (int)textBoxEndPage.Value);
			if( rbSelectNum.Checked)
				info.pages = textBoxSelectNum.Text;
			info.formatType = pdfRadio.Checked?2: tifRadio.Checked?1:0;
			if(radioButtonBlackWhite.Checked || radioButtonColor.Checked)
			info.bw = radioButtonBlackWhite.Checked;
			if(!string.IsNullOrEmpty(pd.sendFileName))
			{
				File.Delete(pd.sendFileName);
				pd.sendFileName = null;
			}

			Task task = Task<bool>.Factory.StartNew(new Func<object, bool>(MakeImage), (object)(new object[ ] { pd, (int)textBoxEndPage.Maximum}));
			Task.Factory.ContinueWhenAny(new Task[] {task}, new Action<Task>(EndConvert));
		}

		public bool MakeImage(object obj)
		{
			List<int> pages = null;
			object[ ] objs = obj as object[ ];
			Components.ProcessingDocs pd = objs[0] as Components.ProcessingDocs;
			int count = (int)objs[1];
			
			int startPage = 0;
			int endPage = 0;
			bool isPDF = Environment.IsPdf(pd.sourceFileName);
			if(pd.pagesType < 2)
			{
				if(pd.pagesType == 1)
				{
					string[] ar = pd.pages.Split('-');
					startPage = int.Parse(ar[0]);
					endPage = int.Parse(ar[1]);
				}
				if(startPage < 1)
					startPage = 1;
				if(endPage < startPage)
					endPage = startPage;
				if(endPage > count || pd.pagesType == 0)
					endPage = count;
				if(endPage < startPage)
					endPage = startPage;
				pages = new List<int>();
				for(int i = startPage; i <= endPage; i++)
					pages.Add( i);
			}
			else
			{
				string[ ] nums = pd.pages.Split(" ,;.".ToCharArray());
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

			if(pd.formatType == 2 || (pd.formatType == 0 && isPDF))
			{
				if(!string.IsNullOrEmpty(pd.sendFileName))
					File.Delete(pd.sendFileName);
				else
					pd.sendFileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
				if(isPDF)
				{
					Environment.PDFHelper.SavePart(pd.sourceFileName, pd.sendFileName, pages);
				}
				else
				{
					TiffToPDF(pd.sourceFileName, pd.sendFileName, pages, (pd.bw.HasValue && pd.bw.Value ? null : pages), pd.docImageID);
				}
			}
			else if(pd.formatType == 1 || !isPDF)
			{
				if(!string.IsNullOrEmpty(pd.sendFileName))
					File.Delete(pd.sendFileName);
				else
					pd.sendFileName = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
				ushort[] ar = pages.Select(x => (ushort)(x - 1)).ToArray();
				if(isPDF)
				{
					PDFToTiff(pd.sourceFileName, pd.sendFileName, pages, (pd.bw.HasValue && pd.bw.Value?null:pages), pd.docImageID);
				}
				else
					Environment.LibTiff.ExtLT.TIFCopyFile(pd.sourceFileName, pd.sendFileName, ar, pd.bw.HasValue && pd.bw.Value? null:ar, 0);
			}

			return false;
		}

		private bool PDFToTiff(string input, string output, List< int> pages, List<int> colorPages, int imageID)
		{
			List<Tiff.PageInfo> il = new List<Tiff.PageInfo>();

			List<StampItem> stL = (imageID > 0) ? Environment.DocSignatureData.GetStamps(imageID) : null;

			bool saveColor = pages != null && pages.Count > 0;
			il = Environment.PDFHelper.GetBitmapsCollectionFromFile(input, pages, 300, 300, colorPages.Count > 0);

			for(int i = 0; i < il.Count; i++)
			{
				Tiff.PageInfo info = il[i];
				if(info != null)
				{
					System.Drawing.Imaging.PixelFormat pf;
					Bitmap bmp = info.Image;
					pf = bmp.PixelFormat;
					if(bmp.HorizontalResolution > 300)
						if(bmp.VerticalResolution > 300)
							bmp.SetResolution(300, 300);
						else
							bmp.SetResolution(300, bmp.VerticalResolution);

					if(bmp.VerticalResolution > 300)
						bmp.SetResolution(bmp.HorizontalResolution, 300);
					if(stL != null && stL.Count > 0)
					{
						var pageStamps = stL.Where(x => x.Page == pages[i]);
						if(pageStamps != null && pageStamps.Count() > 0)
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
													new PointF(si.X + (img.Width * si.Zoom) / 200,
															   si.Y + (img.Height * si.Zoom * drez) / 200));
										g.Transform = mx;
									}
									g.DrawImage(img, si.X, si.Y, (img.Width * si.Zoom) / 100,
												(img.Height * si.Zoom * drez) / 100);
									if(!si.Rotate.Equals(0))
										g.ResetTransform();
								}
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
													using(Font f = new Font(tt.LfFont.FontFamily, tt.LfFont.SizeInPoints * g.DpiY / (float)ImageControl.TiffAnnotation.GetDevicePixel(), tt.LfFont.Style))
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
													g.DrawString(an.TextPrivateData.SzAnoText, new Font(an.LfFont.FontFamily, an.LfFont.SizeInPoints * g.DpiY / (float)ImageControl.TiffAnnotation.GetDevicePixel(), an.LfFont.Style), new SolidBrush(an.RgbColor2), newRect1, sf1);
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
			Components.ProcessingDocs pd = objs[0] as Components.ProcessingDocs;
			if(pd == null) return;
			int num = -1;
			FileInfo fi = null;
			if(File.Exists(pd.sendFileName))
			{
				fi = new FileInfo(pd.sendFileName);
				num = infos.IndexOf(pd);
				if(num > -1)
				infos[num].Size = fi.Length;
				infos[num].pages = pd.pages;
				infos[num].pagesType = pd.pagesType;
				infos[num].formatType = pd.formatType;
				infos[num].bw = pd.bw;
			}
			if(num > -1)
			InvokeIfRequired(new MethodInvoker(() => 
				{
					foreach( var con in flowLayoutPanel.Controls)
					{
						LinkLabel li = con as LinkLabel;
						if(li != null && li.Tag.Equals(pd))
						{
							li.Text = pd.docString + "(" + (fi.Length > 1024 ? fi.Length > 820000 ? (fi.Length / 1048576f).ToString("N2") + "MB" : (fi.Length / 1024f).ToString("N2") + "KB" : fi.Length.ToString() + "B") + ")";
							li.Tag = infos[num];
							linkLabel_LinkClicked(li, new LinkLabelLinkClickedEventArgs(null));
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
	}
}
