using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Kesco.Lib.Log;
using Kesco.Lib.Win.Data.DALC.Buhgalteriya;
using Kesco.Lib.Win.Data.DALC.Corporate;
using Kesco.Lib.Win.Data.DALC.Directory;
using Kesco.Lib.Win.Data.DALC.Documents;
using Kesco.Lib.Win.Data.Documents;

namespace Kesco.Lib.Win.Document
{
	/// <summary>
	///   ����������� ����� ���������� ����� ��� ���������� ����������. ������� ������������ ������������� ������� Init(string).
	/// </summary>
	public class Environment
	{
		private const string subscribeTable = "���������.dbo.�������������������";

		/// <summary>
		/// ������� co ��������.
		/// </summary>
		private static OrderedDictionary stampDictionary;

		/// <summary>
		/// ����� "���"
		/// </summary>
		private static Bitmap imgDSP;

		/// <summary>
		/// ���������� ��� ������
		/// </summary>
		private static object[] archivPrinterParams; // ��������� ��� ���������� ������ ������ ���������
		private static string archivPrinterPath;

		private static string connectionStringDocument; // ������ ����������� � ��
		private static string connectionStringAccounting; // ������ ����������� � �����������
		private static string connectionStringUser; // ������������ ������ ��� ��������� �����
		private static PersonWord personWord; // ������ � �������� "����"
		private static Data.Temp.Objects.Employee curEmp; // ������� ���������
		private static bool isConnectedDocs = false; // ���� �� ����� � �� ����������
		private static bool isConnectedBuh = false; // ���� �� ����� � �� ����������
		private static string showHelpString; // ����� ������ ������.
		private static string eFormString; // ����� ������ ����������� �����
		private static string icExportString; // ����� ������ 1c ��������
		private static string scalaExportString; // ����� ������ Scala ��������
		private static string icMRExportString; // ����� ������ �������� ���������
		private static string settingsURLString; // ����� ������ htlm-�� �������
		private static string reportingServiceURLString; // ����� Reporting Servic-�
		private static string printTemplateString; // ����� ������ ������� ������
		private static string personSearchString; // ����� ������ ������ ���
		private static string employeeSearchString; // ����� ������ ������ �����������
		private static string createContactString; // ����� ������ �������� ��������
		private static string createDocsLinkString; // ����� ������ �������� ����� ����� �����������
		private static string personURL; // ����� ������ ������ ���-�� �� ����
		private static string createClientString; // ����� ������ �������� ������������ ����
		private static string createClientPersonString; // ����� ������ �������� ����������� ����
		private static string createTransactionString; // ����� ������ �������� ����������
		private static string showTransactionString; // ����� ������ ������ ����������
		private static string usersURL; // ����� ������ ������ ���-�� �� ������������
		private static string dialURL; // ����� ������ ������ ��������� �� ������������
		private static string personParamStr; // ��������� URL'� ������ ���
		private static string empName; // ��� ����������

		private static string mailNickname; // ����� ���������� �� �����������
		private static string mailBoxName; // email ���������� �� �����������

		internal static string userMultipleParamStr = "clid=3&return=2&UserOur=true"; // ��������� URL'� ������ ����������� (�������������)

		private static Form activeForm; // ������� �������� �����, ��� ��������� � ������ ��������� ������ 

		private static Options.Folder settings; // ���� ����� �����
		private static Options.Folder layout; // ����� �������� ����

		private static SynchronizedCollection<int> _1CDocTypeIDs; // ��������� ���������� � 1�
		private static SynchronizedCollection<int> _1CFoodDocTypeIDs; // ��������� ���������� � 1� ��������

		private static SynchronizedCollection<int> _1CDictionaryDocTypeIDs;// ��������� ���������� � 1� ��� �����������.

		private static PrinterList printers; // ���� �� ���������

		private static SynchronizedCollection<ServerInfo> servers; // ���� �� �������
		private static BackgroundWorker serversLoader;

		private static PrintReport.IReport report; // ����������� � �������� �������


		private static ResourceManager stringResources; // ������ ��� �����������

		private static CultureInfo curCultureInfo; // ������� �����������

		private static bool personMessage = true;

		private static ConcurrentDictionary<string, Form> docToSave;
		private static ConcurrentDictionary<string, Form> docToSend;
		private static Hashtable docToPrint;

		private static DataTable iC;

		private static UndoRedoSta�k undoredoStack; // ���� ������

		private static string server = string.Empty;		// URL EWS ��������� �������

		private static string mailServer = string.Empty;	// URL OWA ��������� �������

		private static string mailDomain = string.Empty; // ������ ������ �� �����

		private static UserSettings userSettings; // ��������� ������������
		public static float Dpi;

		private static SynchronizedCollection<DataRow> usedPersonsNames; // ������ ���-������������ � �������
		private static SynchronizedCollection<DataRow> usedPersonsDates; // ������ ���-������������ � ������ ��������
		public static BackgroundWorker UsedPersonsLoader;

		public static SynchronizedCollection<KeyValuePair<int, Form>> OpenDocs =
			new SynchronizedCollection<KeyValuePair<int, Form>>(); // �������� �������������� ����

		private static DataTable docTypes;

		#region DALCs & Readers

		private static DocumentDALC docData; // ���������
		private static WorkDocDALC workDocData; // ������� ���������
		private static DocDataDALC docDataData; // ���������������
		private static QueryDALC queryData; // �������

		private static FaxDALC faxData; // �����
		private static FaxInDALC faxInData; // ��. �����
		private static FaxOutDALC faxOutData; // ���. �����
		private static FaxFolderDALC faxFolderData; // ����� ������

		private static SettingsDALC settingsData; // ���������
		private static DocLinksDALC docLinksData; // ����� ����������
		private static DocImageDALC docImageData; // �������.
		private static DocTypeDALC docTypeData; // ���� ���.

		private static SignMessageTextDALC signTextData;// ������ ��� ������� ��� ������� ����������

		private static DocTreeSPDALC docTreeSPData; // ������ ���.

		private static FolderDALC folderData; // ����� ����������
		private static SharedFolderDALC sharedFolderData; // ����� ����� ����������
		private static MessageDALC messageData; // ���������
		private static FieldDALC fieldData; // ����
		private static ArchiveDALC archiveData; // ���������
		private static FolderRuleDALC folderRuleData; // ������� �����
		private static LogEmailDALC logEmailData; // ��� �����
		private static PrintDataDALC printData; // ������ ���.
		private static MailingListDALC mailingListData; // ������ ��������
		private static TransactionDALC transactionData; // ���������� �� ���������
		private static TransactionTypeDALC transactionTypeData; // ���� ���������� 
		private static DocSignatureDALC docSignatureData; // ������� ����������
		private static StampDALC stampData; // ������ ����������

		private static URLsDALC urlsData; // ������ ���������� ��� ��������
		private static PersonsUsedDALC personsUsedData; // ��������� �������������� ����

		private static BuhParamDocDALC buhParamDocData; // ������������� ��������� ���������

		private static SettingsPrintFormDALC settingsPrintForm; //��������� �������� ����

		private static PersonDALC personData; // ����
		private static PersonLinkDALC personLinkData; // ����� ���
		private static StoryDALC storyData; // ������
		private static FaxRecipientDALC faxRecipientData; // ���������� ������
		private static ResourceDALC resourceData; // �������
		private static UnitDimensionDALC unitDimensionData; // ������� ���������

		private static BuhDALC buhData; // ������������� � ������������
		private static EmployeeDALC empData; // ����������
		private static PhoneDALC phoneData; // ��������� � ���������.
		private static AreaDALC areaData; // ������
		private static OPFormsDALC oPFormsData; // ��������������-�������� �����
		private static TypesPersonDALC typesPersonData; // ���� ���
		private static BusinessProjectDALC businessProjectData; // ������ �������

		private static ReplacementEmployeeDAL� replacementEmployeeData;//������ ���������� �����������

		#endregion

		#region Accessors

        /// <summary>
        /// �������� �������������. UI Thread
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static SynchronizationContext UIThreadSynchronizationContext { get; set; }

	    /// <summary>
		///   ������ ����������� � ���� ������ ���������
		/// </summary>
		public static string ConnectionStringDocument
		{
			get { return connectionStringDocument; }
			set
			{
				if(!string.IsNullOrEmpty(value))
				{
					connectionStringDocument = value;
					Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), connectionStringDocument);
					isConnectedDocs = Data.DALC.DALC.TestConnection(connectionStringDocument);
					if(isConnectedDocs)
						Data.Env.Docs = new Data.Business.V2.Docs.DocsModule(connectionStringDocument);
				}
				else
				{
					isConnectedDocs = false;
					connectionStringDocument = "";
				}
			}
		}

		/// <summary>
		///   ������ ����������� � ���� ������ ��� �����������
		/// </summary>
		public static string ConnectionStringAccounting
		{
			get { return connectionStringAccounting; }
			set
			{
				if(!string.IsNullOrEmpty(value))
				{
					connectionStringAccounting = value;
					Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), connectionStringAccounting);
					isConnectedBuh = Data.DALC.DALC.TestConnection(connectionStringAccounting);
					if(isConnectedBuh)
						Data.Env.Buh = new Data.Business.V2.Buh.BuhModule(connectionStringAccounting);
				}
				else
					isConnectedBuh = false;
			}
		}

		/// <summary>
		///   ������ ����������� � ���� ������ ��������������
		/// </summary>
		public static string ConnectionStringUser
		{
			get { return connectionStringUser; }
			set { connectionStringUser = value + ";Application Name=DocView"; }
		}

		public static ResourceManager StringResources
		{
			get
			{
				return stringResources ?? (stringResources = new ResourceManager("Kesco.Lib.Win.Document.StringResources", Assembly.GetExecutingAssembly()));
			}
		}

		public static CultureInfo CurCultureInfo
		{
			get { return curCultureInfo ?? (curCultureInfo = Thread.CurrentThread.CurrentUICulture); }
			set { curCultureInfo = value; }
		}

		/// <summary>
		///   ������ ������ ��� �������� ����������� �����
		/// </summary>
		public static string EFormString
		{
			get
			{
				if(string.IsNullOrEmpty(eFormString))
				{
					if(isConnectedDocs)
					{
						try
						{
							eFormString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.EFormCreateURL).ToString().Trim();
						}
						catch(Exception ex)
						{
							Data.Env.WriteToLog(ex);
						}
					}
				}
				return eFormString;
			}
			set { eFormString = value; }
		}

		/// <summary>
		///   ������ ������ html-�� �������
		/// </summary>
		public static string SettingsURLString
		{
			get
			{
				if(string.IsNullOrEmpty(settingsURLString))
				{
					settingsURLString = "about:blank";
				}
				return settingsURLString;
			}
			set { settingsURLString = value; }
		}

		/// <summary>
		///   ������ ������ Reporting Servica
		/// </summary>
		public static string ReportingServiceURLString
		{
			get
			{
				if(string.IsNullOrEmpty(reportingServiceURLString))
				{
					if(isConnectedDocs)
					{
						try
						{
							reportingServiceURLString =
								URLsData.GetField(URLsData.NameField,
												  (int)URLsDALC.URLsCode.ReportingServiceURL).ToString().Trim();
						}
						catch(Exception ex)
						{
							Data.Env.WriteToLog(ex);
						}
					}
				}
				return reportingServiceURLString;
			}
			set { reportingServiceURLString = value; }
		}

		/// <summary>
		///   ������ ������ ������
		/// </summary>
		public static string ShowHelpString
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(showHelpString))
				{
					try
					{
						showHelpString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.HelpURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return showHelpString;
			}
			set { showHelpString = value; }
		}

		/// <summary>
		///   ������ ������� ������
		/// </summary>
		public static string PrintTemplateString
		{
			get
			{
				if(string.IsNullOrEmpty(printTemplateString))
				{
					if(isConnectedDocs)
					{
						try
						{
							printTemplateString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.PrintTemplate).ToString().Trim();
						}
						catch(Exception ex)
						{
							Data.Env.WriteToLog(ex);
						}
					}
				}
				return printTemplateString;
			}
			set { printTemplateString = value; }
		}

		/// <summary>
		///   ������ ������ �������� 1c
		/// </summary>
		public static string IcExportString
		{
			get
			{
				if(string.IsNullOrEmpty(icExportString))
				{
					if(isConnectedDocs)
					{
						try
						{
							icExportString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.ICExportURL).ToString().Trim();
						}
						catch(Exception ex)
						{
							Data.Env.WriteToLog(ex);
						}
					}
				}
				return icExportString;
			}
			set { icExportString = value; }
		}

		/// <summary>
		///   ������ ������ �������� Scala
		/// </summary>
		public static string ScalaExportString
		{
			get
			{
				if(string.IsNullOrEmpty(scalaExportString))
				{
					if(isConnectedDocs)
					{
						try
						{
							scalaExportString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.ScalaExportURL).ToString().Trim();
						}
						catch(Exception ex)
						{
							Data.Env.WriteToLog(ex);
						}
					}
				}
				return scalaExportString;
			}
			set { scalaExportString = value; }
		}

		/// <summary>
		///   ������ ������ �������� ���������
		/// </summary>
		public static string ICMRExportString
		{
			get
			{
				if(string.IsNullOrEmpty(icMRExportString))
				{
					if(isConnectedDocs)
					{
						try
						{
							icMRExportString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.ICMFExportURL).ToString().Trim();
						}
						catch(Exception ex)
						{
							Data.Env.WriteToLog(ex);
						}
					}
				}
				return icMRExportString;
			}
			set { icMRExportString = value; }
		}


		/// <summary>
		///   ������ ������ ������ ���
		/// </summary>
		public static string PersonSearchString
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(personSearchString))
				{
					try
					{
						personSearchString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.SearchPersonURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return personSearchString;
			}
			set { personSearchString = value; }
		}

		/// <summary>
		///   ������ ������ ������ ����������
		/// </summary>
		public static string EmployeeSearchString
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(employeeSearchString))
				{
					try
					{
						employeeSearchString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.SearchEmployeeURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return employeeSearchString;
			}
			set { employeeSearchString = value; }
		}

		/// <summary>
		///   ������ ������ �������� ��������
		/// </summary>
		public static string CreateContactString
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(createContactString))
				{
					try
					{
						createContactString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.CreateContractURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return createContactString;
			}
			set { createContactString = value; }
		}

		/// <summary>
		///   ������ ������ �������� ������������ ����
		/// </summary>
		public static string CreateClientString
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(createClientString))
				{
					try
					{
						createClientString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.CreateClientURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return createClientString;
			}
			set { createClientString = value; }
		}

		/// <summary>
		///   ������ ������ �������� ����������� ����
		/// </summary>
		public static string CreateClientPersonString
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(createClientPersonString))
				{
					try
					{
						createClientPersonString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.CreateClientPersonURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return createClientPersonString;
			}
			set { createClientPersonString = value; }
		}

		/// <summary>
		///   ������ ������ �������� ����� ����� �����������
		/// </summary>
		public static string CreateDocsLinkString
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(createDocsLinkString))
				{
					try
					{
						createDocsLinkString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.CreateDocsLinkURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return createDocsLinkString;
			}
			set { createDocsLinkString = value; }
		}

		/// <summary>
		///   ������ ������ �������� ����������
		/// </summary>
		public static string CreateTransactionString
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(createTransactionString))
				{
					try
					{
						createTransactionString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.CreateTransactionURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return createTransactionString;
			}
			set { createTransactionString = value; }
		}

		/// <summary>
		///   ������ ������ �������� ����������
		/// </summary>
		public static string ShowTransactionString
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(showTransactionString))
				{
					try
					{
						showTransactionString = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.ShowTransactionURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return showTransactionString;
			}
			set { showTransactionString = value; }
		}


		/// <summary>
		///   ������ ������ ���-�� � ������������
		/// </summary>
		public static string UsersURL
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(usersURL))
				{
					try
					{
						usersURL = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.UsersURL).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return usersURL;
			}
			set { usersURL = value; }
		}


		/// <summary>
		///   ������ ������ ���-�� � ����-�����������
		/// </summary>
		public static string PersonURL
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(personURL))
				{
					try
					{
						personURL = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.ShowPersonUrl).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return personURL;
			}
			set { personURL = value; }
		}

		/// <summary>
		///   ������ ������ ���� �����
		/// </summary>
		public static string DialURL
		{
			get
			{
				if(isConnectedDocs && string.IsNullOrEmpty(dialURL))
				{
					try
					{
						dialURL = URLsData.GetField(URLsData.NameField, (int)URLsDALC.URLsCode.DialUrl).ToString().Trim();
					}
					catch(Exception ex)
					{
						Data.Env.WriteToLog(ex);
					}
				}
				return dialURL;
			}
			set { dialURL = value; }
		}

		/// <summary>
		///   ��� ����� ���
		/// </summary>
		public static PersonWord PersonWord
		{
			get { return personWord; }
		}

		/// <summary>
		///   ������ �� ��������
		/// </summary>
		public static string SubscribeTable => subscribeTable;
		/// <summary>
		///   ��� �������� ���������� �� ���� ��������������
		/// </summary>
		public static Data.Temp.Objects.Employee CurEmp
		{
			get
			{
				if(isConnectedDocs && curEmp == null)
					curEmp = Data.Temp.Objects.Employee.GetSystemEmployee(EmpData);

				return curEmp;
			}
		}

		/// <summary>
		///   ��� ����������
		/// </summary>
		public static string EmpName
		{
			get { return empName; }
			set
			{
				if(value != null)
					empName = value;
			}
		}

		/// <summary>
		///   �������� ������� ����������� � ���� ����������
		/// </summary>
		public static bool IsConnectedDocs => isConnectedDocs;
		/// <summary>
		///   �������� ������� ����������� � ���� �����������
		/// </summary>
		public static bool IsConnectedBuh
		{
			get
			{
				return ((isConnectedBuh) ? isConnectedBuh : isConnectedBuh = Data.DALC.DALC.TestConnection(connectionStringAccounting));
			}
		}

		/// <summary>
		///   ������� � ������� ��� ���������� ���������
		/// </summary>
		public static Options.Folder Settings
		{
			get
			{
				if(settings == null)
					throw new Exception("Environment: settings not initialized");
				return settings;
			}
		}

		/// <summary>
		///   ����� � �������, � ������� �������� ������ � ������� ����
		/// </summary>
		public static Options.Folder Layout
		{
			get
			{
				if(layout != null)
					return layout;
				throw new Exception("Environment: layout not initialized");
			}
		}

		/// <summary>
		///   ��������� ������������ �� ����
		/// </summary>
		public static UserSettings UserSettings
		{
			get { return isConnectedDocs ? userSettings ?? (userSettings = new UserSettings(SettingsData)) : null; }
		}

		/// <summary>
		///   ���������� � ���������
		/// </summary>
		public static PrinterList Printers
		{
			get { return printers; }
		}

		public static PrintReport.IReport Report
		{
			get { return report ?? (report = new PrintReport.Report2005(ReportingServiceURLString)); }
		}

		/// <summary>
		///   ���������� �� 1�
		/// </summary>
		public static DataTable IC
		{
			get
			{
				if(iC == null && isConnectedBuh)
				{
					DataTable dt = BuhData.Get1CName();
					if(dt != null && dt.Rows.Count > 0)
						iC = dt;
				}
				return iC;
			}
		}

		/// <summary>
		///   ���������� � ������� (��������)
		/// </summary>
		public static SynchronizedCollection<ServerInfo> GetServers()
		{
			try
			{
				if(!serversLoader.IsBusy && (servers == null || servers.Count == 0))
					serversLoader.RunWorkerAsync();

				while(serversLoader.IsBusy)
				{
					Application.DoEvents();
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			return servers ?? new SynchronizedCollection<ServerInfo>();
		}

		/// <summary>
		///   ��� ����������� ������ ����������
		/// </summary>
		public static int PersonID 
		{
			get;
			set;
		}

		public static bool PersonMessage
		{
			get { return personMessage; }
			set { personMessage = value; }
		}

		public static ConcurrentDictionary<string, Form> DocToSave
		{
			get { return docToSave ?? (docToSave = new ConcurrentDictionary<string, Form>()); }
		}

		public static ConcurrentDictionary<string, Form> DocToSend
		{
			get { return docToSend ?? (docToSend = new ConcurrentDictionary<string, Form>()); }
		}

		public static Hashtable DocToPrint
		{
			get { return docToPrint ?? (docToPrint = new Hashtable()); }
		}


		/// <summary>
		/// GUID for SCP URL keyword.
		/// </summary>
		private const string ScpUrlGuidString = @"77378F46-2C66-4aa9-A6A6-3E7A48B19596";

		/// <summary>
		/// GUID for SCP pointer keyword.
		/// </summary>
		private const string ScpPtrGuidString = @"67661d7F-8FC4-4fa7-BFAC-E1D7794C1F68";

		/// <summary>
		/// Url Ews ��������� �������
		/// </summary>
		public static string FullExchangeServerEwsUrl
		{
			get
			{
				if(string.IsNullOrEmpty(server))
				{
					string domain = System.Environment.UserDomainName;

					DirectoryEntry dir = new DirectoryEntry { Path = @"LDAP://" + domain };
					DirectorySearcher sea = new DirectorySearcher(dir)
												{
													Filter =
														"(&(objectCategory=person)(objectClass=user)(samaccountname=" +
														System.Environment.UserName + "))"
												};
					SearchResult resEnt = sea.FindOne();
					if(resEnt.Properties.Contains("msexchhomeservername"))
					{
						string filter = resEnt.Properties["msexchhomeservername"][0].ToString();

						filter = filter.Substring(filter.LastIndexOf("/cn=") + 4, filter.Length - filter.LastIndexOf("/cn=") - 4);
						mailDomain = "";
						string adspath = resEnt.Path.ToLower();
						while(adspath.IndexOf("dc=") > 0)
						{
							int last = adspath.LastIndexOf("dc=");
							mailDomain = "." + adspath.Substring(last + 3, adspath.Length - last - 3) + mailDomain;
							if(last > 0)
								adspath = adspath.Substring(0, last - 1);
						}
						string rootDSEPath = "LDAP://RootDSE";
						// Get the root directory entry.
						DirectoryEntry rootDSE = new DirectoryEntry(rootDSEPath);

						// Get the configuration path.
						string configPath = rootDSE.Properties["configurationNamingContext"].Value as string;

						rootDSE.Dispose();
						// Get the configuration entry.
						dir.Path = "LDAP://" + configPath;
						sea.Filter = "(&((objectClass=serviceConnectionPoint)(cn=" + filter + "))" + "(|(keywords=" + ScpPtrGuidString + ")(keywords=" + ScpUrlGuidString + ")))";
						sea.PropertiesToLoad.Add("keywords");
						sea.PropertiesToLoad.Add("serviceBindingInformation");
						resEnt = sea.FindOne();
						ResultPropertyValueCollection entryKeywords = resEnt.Properties["keywords"];
						string ptrLdapPath = resEnt.Properties["serviceBindingInformation"][0] as string;
						sea.Dispose();
						dir.Dispose();
						HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(new Uri(ptrLdapPath));
						client.Credentials = CredentialCache.DefaultNetworkCredentials;
						client.PreAuthenticate = true;
						client.Method = "POST";
						client.ContentType = "text/xml";
						client.Timeout = 5000;

						string QueryString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
"<Autodiscover xmlns=\"http://schemas.microsoft.com/exchange/autodiscover/outlook/requestschema/2006\">" +
"  <Request>" +
"    <EMailAddress>" + MailBoxName + "</EMailAddress>" +
"    <AcceptableResponseSchema>http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a</AcceptableResponseSchema>" +
"  </Request>" +
"</Autodiscover>";
						byte[] buf = System.Text.Encoding.UTF8.GetBytes(QueryString);
						client.ContentLength = buf.Length;
						try
						{
							Stream sout = client.GetRequestStream();
							sout.Write(buf, 0, buf.Length);
							sout.Close();
							HttpWebResponse wr = (HttpWebResponse)client.GetResponse();
							StreamReader sr = new StreamReader(wr.GetResponseStream());
							string xmlRes = sr.ReadToEnd();
							XmlDocument doc = new XmlDocument();
							doc.LoadXml(xmlRes);
							XmlNodeList list = doc.GetElementsByTagName("EwsUrl");
							if(list.Count > 0)
								server = list[0].InnerXml;
							list = doc.GetElementsByTagName("Internal");
							if(list.Count > 0)
								mailServer = list[0].FirstChild.InnerXml;
						}
						catch(Exception ex)
						{
							Logger.WriteEx(ex);
							ptrLdapPath = ptrLdapPath.TrimStart("https://".ToCharArray()).TrimStart("http://".ToCharArray());
							ptrLdapPath = ptrLdapPath.Substring(0, ptrLdapPath.IndexOf('/'));
							server = "https://" + ptrLdapPath + "/EWS/Exchange.asmx";
							mailServer = "https://" + ptrLdapPath + "/owa/";
						}
					}
				}
				return server;
			}
		}

		/// <summary>
		/// Url Owa ��������� �������
		/// </summary>
		public static string FullExchangeServerOwaUrl
		{
			get
			{
				if(string.IsNullOrEmpty(mailServer))
				{
					server = FullExchangeServerEwsUrl;
				}
				return mailServer;
			}
		}

		static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		static string GetString(byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			Buffer.BlockCopy(bytes, 0, chars, 0, sizeof(char) * chars.Length);
			return new string(chars);
		}

		/// <summary>
		///   �������� ��� ������������
		/// </summary>
		public static string MailAlias
		{
			get
			{
				if(string.IsNullOrEmpty(mailNickname))
				{
					string domain = System.Environment.UserDomainName;
					var dir = new DirectoryEntry { Path = @"LDAP://" + domain };
					var sea = new DirectorySearcher(dir)
								  {
									  Filter = "(&(objectCategory=person)(objectClass=user)(samaccountname=" +
											   System.Environment.UserName + "))"
								  };
					SearchResult resEnt = sea.FindOne();
					if(resEnt.Properties.Contains("msexchhomeservername"))
					{
						if(resEnt.Properties.Contains("mailNickname"))
						{
							if(string.IsNullOrEmpty(mailDomain))
							{
								mailNickname = FullExchangeServerEwsUrl;
								mailNickname = null;
							}
							ResultPropertyValueCollection res =
								resEnt.Properties["mailNickname"];
							mailNickname = res[0].ToString();
							if(string.IsNullOrEmpty(mailNickname))
								return System.Environment.UserName;
						}
					}
					else
						return System.Environment.UserName;
				}
				return mailNickname;
			}
			set { mailNickname = value; }
		}

		public static string MailDomain => mailDomain;
		/// <summary>
		///   �������� ��� ������������
		/// </summary>
		public static string MailBoxName
		{
			get
			{
				if(string.IsNullOrEmpty(mailBoxName))
				{
					string domain = System.Environment.UserDomainName;
					var dir = new DirectoryEntry { Path = @"LDAP://" + domain };
					var sea = new DirectorySearcher(dir)
								  {
									  Filter = "(&(objectCategory=person)(objectClass=user)(samaccountname=" +
											   System.Environment.UserName + "))"
								  };
					SearchResult resEnt = sea.FindOne();
					if(resEnt.Properties.Contains("msexchhomeservername"))
					{
						if(resEnt.Properties.Contains("mail"))
						{
							if(string.IsNullOrEmpty(mailDomain))
							{
								mailBoxName = FullExchangeServerEwsUrl;
								mailBoxName = null;
							}
							ResultPropertyValueCollection res = resEnt.Properties["mail"];
							mailBoxName = res[0].ToString();
							if(string.IsNullOrEmpty(mailBoxName))
								return System.Environment.UserName;
						}
					}
					else
						return System.Environment.UserName;
				}
				return mailBoxName;
			}
			set { mailBoxName = value; }
		}

		public static DataTable DocTypes
		{
			get
			{
				if(docTypes == null && DocTypeData != null)
					docTypes = docTypeData.GetDocTypes();
				return docTypes;
			}
		}

		#region DALCs & Readers Accessors

		/// <summary>
		///   �������� ���� ��� ������ � �����������
		/// </summary>
		public static DocumentDALC DocData
		{
			get { return docData ?? (docData = new DocumentDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static WorkDocDALC WorkDocData
		{
			get { return workDocData ?? (workDocData = new WorkDocDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static DocDataDALC DocDataData
		{
			get { return docDataData ?? (docDataData = new DocDataDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static FaxDALC FaxData
		{
			get { return faxData ?? (faxData = new FaxDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static FaxInDALC FaxInData
		{
			get { return faxInData ?? (faxInData = new FaxInDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static FaxOutDALC FaxOutData
		{
			get { return faxOutData ?? (faxOutData = new FaxOutDALC(connectionStringDocument)); }
		}

		///<summary>
		/// ������ � ������ ������
		///</summary>
		public static FaxFolderDALC FaxFolderData 
		{
		    get{ return faxFolderData ?? (faxFolderData = new FaxFolderDALC(connectionStringDocument));}
		}


		/// <summary>
		///   Dalc
		/// </summary>
		public static SettingsDALC SettingsData
		{
			get { return isConnectedDocs ? settingsData ?? (settingsData = new SettingsDALC(connectionStringDocument)) : null; ; }
		}

		public static SettingsPrintFormDALC SettingsPrintForm
		{
			get { return settingsPrintForm ?? (settingsPrintForm = new SettingsPrintFormDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static DocLinksDALC DocLinksData
		{
			get { return docLinksData ?? (docLinksData = new DocLinksDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static DocImageDALC DocImageData
		{
			get { return docImageData ?? (docImageData = new DocImageDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   ������ � ����� ����������
		/// </summary>
		public static DocTypeDALC DocTypeData
		{
			get { return docTypeData ?? (docTypeData = new DocTypeDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   ������ � ������ ��� ��������, ��� ������� ����������
		/// </summary>
		public static SignMessageTextDALC SignTextData
		{
			get { return signTextData ?? (signTextData = new SignMessageTextDALC(connectionStringDocument)); }
		}


		/// <summary>
		///   Dalc
		/// </summary>
		public static DocTreeSPDALC DocTreeSPData
		{
			get { return docTreeSPData ?? (docTreeSPData = new DocTreeSPDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static FolderDALC FolderData
		{
			get { return folderData ?? (folderData = new FolderDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static SharedFolderDALC SharedFolderData
		{
			get { return sharedFolderData ?? (sharedFolderData = new SharedFolderDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static MessageDALC MessageData
		{
			get { return messageData ?? (messageData = new MessageDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static FieldDALC FieldData
		{
			get { return fieldData ?? (fieldData = new FieldDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static ArchiveDALC ArchiveData
		{
			get { return archiveData ?? (archiveData = new ArchiveDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static FolderRuleDALC FolderRuleData
		{
			get { return folderRuleData ?? (folderRuleData = new FolderRuleDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static LogEmailDALC LogEmailData
		{
			get { return logEmailData ?? (logEmailData = new LogEmailDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static PrintDataDALC PrintData
		{
			get { return printData ?? (printData = new PrintDataDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static MailingListDALC MailingListData
		{
			get { return mailingListData ?? (mailingListData = new MailingListDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static TransactionDALC TransactionData
		{
			get { return transactionData ?? (transactionData = new TransactionDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static TransactionTypeDALC TransactionTypeData
		{
			get { return transactionTypeData ?? (transactionTypeData = new TransactionTypeDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   ��������� � �������� ���������.
		/// </summary>
		public static DocSignatureDALC DocSignatureData
		{
			get { return docSignatureData ?? (docSignatureData = new DocSignatureDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   ������ � ������ � ������������ �������
		/// </summary>
		public static StampDALC StampData
		{
			get { return stampData ?? (stampData = new StampDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static PersonsUsedDALC PersonsUsedData
		{
			get { return personsUsedData ?? (personsUsedData = new PersonsUsedDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   DALC ��� ������ � ���������
		/// </summary>
		public static QueryDALC QueryData
		{
			get { return queryData ?? (queryData = new QueryDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static URLsDALC URLsData
		{
			get { return urlsData ?? (urlsData = new URLsDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static PersonDALC PersonData
		{
			get { return personData ?? (personData = new PersonDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   DALC ������� ����� ���
		/// </summary>
		public static PersonLinkDALC PersonLinkData
		{
			get { return personLinkData ?? (personLinkData = new PersonLinkDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static StoryDALC StoryData
		{
			get { return storyData ?? (storyData = new StoryDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static ResourceDALC ResourceData
		{
			get { return resourceData ?? (resourceData = new ResourceDALC(connectionStringDocument)); }
		}


		/// <summary>
		///   Dalc
		/// </summary>
		public static UnitDimensionDALC UnitDimensionData
		{
			get { return unitDimensionData ?? (unitDimensionData = new UnitDimensionDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static FaxRecipientDALC FaxRecipientData
		{
			get { return faxRecipientData ?? (faxRecipientData = new FaxRecipientDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static BuhDALC BuhData
		{
			get { return buhData ?? (buhData = new BuhDALC(connectionStringAccounting)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static BuhParamDocDALC BuhParamDocData
		{
			get { return buhParamDocData ?? (buhParamDocData = new BuhParamDocDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static EmployeeDALC EmpData
		{
			get { return isConnectedDocs ? empData ?? (empData = new EmployeeDALC(connectionStringDocument)) : null; ; }
		}

		/// <summary>
		///   DALC ��������� � ���������
		/// </summary>
		public static PhoneDALC PhoneData
		{
			get
			{
				if(!string.IsNullOrEmpty(connectionStringUser) && phoneData == null)
					phoneData = new PhoneDALC(connectionStringUser);

				return phoneData;
			}
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static AreaDALC AreaData
		{
			get { return areaData ?? (areaData = new AreaDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static TypesPersonDALC TypesPersonData
		{
			get { return typesPersonData ?? (typesPersonData = new TypesPersonDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Dalc
		/// </summary>
		public static OPFormsDALC OPFormsData
		{
			get { return oPFormsData ?? (oPFormsData = new OPFormsDALC(connectionStringDocument)); }
		}

		/// <summary>
		/// Dalc
		/// </summary>
		public static BusinessProjectDALC BusinessProjectData
		{
			get
			{
				return businessProjectData ??
					   (businessProjectData = new BusinessProjectDALC(connectionStringDocument));
			}
		}

		/// <summary>
		/// Dalc
		/// </summary>
		public static ReplacementEmployeeDAL� ReplacementEmployeeData
		{
			get
			{
				return replacementEmployeeData ??
					   (replacementEmployeeData =
						new ReplacementEmployeeDAL�(connectionStringDocument));
			}
		}

		#endregion

		#endregion

		/// <summary>
		///   ����������� ������������� ������
		/// </summary>
		public static void Init()
		{
			if(serversLoader == null)
			{
				serversLoader = new BackgroundWorker();
				serversLoader.DoWork += serversLoader_DoWork;
			}

			if(personWord == null)
				personWord = new PersonWord(PersonModes.PersonContragent);

			if(settings == null)
			{
				settings = new Options.Root("DocView\\Document");
				personParamStr = settings.LoadStringOption("PersonParamString", "clid=4&return=1");

				printers = new PrinterList();

				layout = settings.Folders.Add("Layout");
			}
		}

		private static void serversLoader_DoWork(object sender, DoWorkEventArgs e)
		{
			if(!IsConnectedDocs || DocImageData == null)
				return;

			if(servers == null)
				servers = new SynchronizedCollection<ServerInfo>();
			else
				servers.Clear();

			List<ServerInfo> sis = DocImageData.GetLocalServers<ServerInfo>(delegate(IDataRecord dr)
			{
				ServerInfo si = new ServerInfo(dr.GetInt32(0), dr.GetString(1), dr.GetString(2), dr.GetString(3), dr.GetString(4));
				servers.Add(si);
				return si;
			});

			sis.Clear();
			if(servers.Count == 0)
				throw new Exception(StringResources.GetString("Enviroment.Servers.Error1"));
		}

		private static Tiff.LibTiffHelper libTiff;

		private static Classes.PDFHelper pdfHelper;

		public static Tiff.LibTiffHelper LibTiff
		{
			get { return libTiff ?? (libTiff = new Tiff.LibTiffHelper()); }
		}

		public static Classes.PDFHelper PDFHelper
		{
			get { return pdfHelper ?? (pdfHelper = new Classes.PDFHelper()); }
		}

		/// <summary>
		/// ��������� ���������� �����
		/// </summary>
		/// <param name="extension"> ���������� ����� </param>
		/// <returns> ������ � ������ ����� </returns>
		public static string GenerateFileName(string extension)
		{
			return "~" + Guid.NewGuid().ToString() + "." + extension;
		}

		/// <summary>
		/// ��������� ������� ���� ���������� �����
		/// </summary>
		/// <param name="extension">���������� �����</param>
		/// <returns> ������ � ������ ���� </returns>
		public static string GenerateFullFileName(string extension)
		{
			string newFileName = Path.Combine(Path.GetTempPath(), GenerateFileName(extension));
			while(File.Exists(newFileName))
			{
				newFileName = Path.Combine(Path.GetTempPath(), GenerateFileName(extension));
			}

			return newFileName;
		}


		/// <summary>
		///   �������� �� ����������� ��������� ������
		/// </summary>
		/// <returns> </returns>
		public static bool IsFaxReceiver()
		{
			return isConnectedDocs && EmpData.IsFaxReceiver();
		}

		/// <summary>
		///   ��������, ������ �� ����������� ������������ windows � ������ �������� �������
		/// </summary>
		/// <returns> true - ������, ����� false </returns>
		public static bool IsDomainAdmin()
		{
			using(var sea = new DirectorySearcher { Filter = "(&(objectCategory=person)(objectClass=user)(samaccountname=" + System.Environment.UserName + "))" })
			{
				try
				{
					SearchResult resEnt = sea.FindOne();
					if(resEnt != null)
						if(resEnt.Properties["memberof"].Cast<string>().Any(resu => resu.ToLower().Contains("cn=domain admins")))
						{
							return true;
						}
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
				}
			}

			return false;
		}

		/// <summary>
		///   ���� � ����� �������� ������.
		/// </summary>
		public static string PrinterPath
		{
			get
			{
				if(string.IsNullOrEmpty(archivPrinterPath))
				{
					archivPrinterPath = Checkers.TestPrinter.GetPrinterPath();
					if(string.IsNullOrEmpty(archivPrinterPath))
					{
						Data.Env.WriteExtExToLog("UDC printer error", "Error while trying to get printer information. Support service need to manually setup UDC printer on this computer.",
										   Assembly.GetExecutingAssembly().GetName(), MethodBase.GetCurrentMethod());
						return string.Empty;
					}
				}

				return Path.GetFullPath(archivPrinterPath);
			}
			set
			{
				if(string.IsNullOrEmpty(value))
					archivPrinterPath = null;
			}
		}

		/// <summary>
		///   ��� �������� UDC
		/// </summary>
		public static string PrinterName
		{
			get
			{
				string archivPrinterName = Dialogs.PrinterOp.GetPrinterName("Universal Document Converter");
				if(string.IsNullOrEmpty(archivPrinterName))
				{
					Data.Env.WriteExtExToLog("UDC printer error", "Printer name is wrong or printer is not installed!",
									   Assembly.GetExecutingAssembly().GetName(), MethodBase.GetCurrentMethod());
					return string.Empty;
				}
				return archivPrinterName;
			}
		}

		/// <summary>
		///   ������ �������� UDC
		/// </summary>
		public static float PrinterVersion => Checkers.TestPrinter.GetPrinterVersion();
		/// <summary>
		/// ������ ��������.
		/// ���� ����� ��������� ��� ������ �� WndProc
		/// </summary>
		/// <returns></returns>
		public static float PrinterVersionAsync => Checkers.TestPrinter.GetPrinterVersionAsync();
		/// <summary>
		///   �������� ���������� � �������� UDC
		/// </summary>
		public static bool DocIsPrintedFromUDC(string fileName)
		{
			Console.WriteLine("{0}: DocIsPrintedFromUDC : {1}", DateTime.Now.ToString("HH:mm:ss fff"), fileName);
			if(string.IsNullOrEmpty(fileName))
				return false;
			try
			{
				string fileDirectory = Path.GetDirectoryName(fileName) ?? string.Empty;
				string printerDirectory = PrinterPath.TrimEnd(Path.DirectorySeparatorChar);
				Console.WriteLine("{0}: Printer path : {1}, file path : {2}", DateTime.Now.ToString("HH:mm:ss fff"), printerDirectory, fileDirectory);
				return !string.IsNullOrEmpty(printerDirectory) && !string.IsNullOrEmpty(fileDirectory) &&
					  string.Compare(printerDirectory, fileDirectory.TrimEnd(Path.DirectorySeparatorChar), true) == 0;
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
			return false;
		}

		/// <summary>
		///   �������� ��������� ��� ��������� �������� ��������� �� �������
		/// </summary>
		public static void SetPrinterDocParams(int imageID, string fileName, bool isPDFMode, int startPage, int endPage,
											   int countPage, string docName)
		{
			archivPrinterParams = new object[7];
			archivPrinterParams[0] = imageID;
			archivPrinterParams[1] = fileName;
			archivPrinterParams[2] = isPDFMode;
			archivPrinterParams[3] = startPage;
			archivPrinterParams[4] = endPage;
			archivPrinterParams[5] = countPage;
			archivPrinterParams[6] = docName;
		}

		/// <summary>
		///   �������� ������������� ��������� ����������
		/// </summary>
		public static object[] GetPrinterDocParams(string fileName)
		{
			if(archivPrinterParams == null || archivPrinterParams.Length != 7)
				return null;

			return archivPrinterParams;
		}

		/// <summary>
		///   �������� �� ����������� �������� �� �����
		/// </summary>
		public static bool IsFaxSender()
		{
			return isConnectedDocs && EmpData.IsFaxSender();
		}

		/// <summary>
		///   �������� �� ����������� �������� �� ����� �� ���� ���������
		/// </summary>
		public static bool IsFaxSender(int docID)
		{
			if(docID <= 0)
				return IsFaxSender();
			return isConnectedDocs && EmpData.IsFaxSender(docData.GetDocPersonsIDs(docID));
		}

		/// <summary>
		///   ������ ��� ������ ��������� ��� Person
		/// </summary>
		public static string PersonParamStr
		{
			get { return personParamStr; }
			set { personParamStr = value; }
		}

		#region SendMessage

		/// <summary>
		///   ����������� ������� ������� ������ � ����
		/// </summary>
		/// <param name="wndHandle"> ��������� �� ���� </param>
		/// <param name="sendText"> ���������� ����� </param>
		/// <returns> 0 - ������� �� ������� 1 - ������� </returns>
		public static int SendMessage(IntPtr wndHandle, string sendText)
		{
			return SendMessage(wndHandle, sendText, true);
		}

		/// <summary>
		///   ����������� ������� ������� ������ � ����
		/// </summary>
		/// <param name="wndHandle"> ��������� �� ���� </param>
		/// <returns> 0 - ������� �� ������� 1 - ������� </returns>
		public static int SendMessage(IntPtr wndHandle)
		{
			return SendMessage(wndHandle, "", false);
		}

		/// <summary>
		///   ����������� ������� ������� ������ � ����
		/// </summary>
		/// <param name="wndHandle"> ��������� �� ���� </param>
		/// <param name="sendText"> ����� ������� </param>
		/// <param name="send"> true ����� ������������, false ��� </param>
		/// <returns> 0 - ������� �� ������� 1 - ������� </returns>
		public static int SendMessage(IntPtr wndHandle, string sendText, bool send)
		{
			int result = 0;
			IntPtr lpB = IntPtr.Zero;
			IntPtr textPointer = IntPtr.Zero;
			try
			{
				Win32.User32.ShowWindow(wndHandle, 8);
				if(send)
				{
					byte[] B = Encoding.UTF8.GetBytes(sendText);
					lpB = Marshal.AllocHGlobal(B.Length);
					Marshal.Copy(B, 0, lpB, B.Length);
					var cdt = new Win32.User32.COPYDATASTRUCT { dwData = 1024, lpData = lpB, cbData = B.Length };
					textPointer = Marshal.AllocHGlobal(Marshal.SizeOf(cdt));
					Marshal.StructureToPtr(cdt, textPointer, false);
				}
				if(wndHandle != IntPtr.Zero)
				{
					result = Win32.User32.SendMessage(wndHandle, (int)Win32.Msgs.WM_COPYDATA, IntPtr.Zero, textPointer).ToInt32();
				}
				if(send)
				{
					Marshal.FreeHGlobal(lpB);
					Marshal.FreeHGlobal(textPointer);
				}
			}
			catch(Exception ex)
			{
				result = 0;
				MessageBox.Show(ex.Message);
			}

			return result;
		}

		#endregion

		#region Form

		/// <summary>
		///   ���������� �������� �����
		/// </summary>
		public static void SaveActiveForm()
		{
			activeForm = Form.ActiveForm;
		}

		/// <summary>
		///   ������������� �������� �����
		/// </summary>
		public static void RestoreActiveForm()
		{
			if((activeForm != null) && (activeForm != Form.ActiveForm))
				activeForm.BringToFront();
		}

		#endregion

		#region Server

		public static void RefreshServers()
		{
			if(serversLoader.IsBusy)
				return;

			serversLoader.RunWorkerAsync();
		}

		public static ServerInfo GetRandomLocalServer()
		{
			try
			{
				if(GetServers().Count > 0)
				{
					var rand = new Random();
					return servers[rand.Next(servers.Count)];
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			return null;
		}

		public static ServerInfo GetLocalServer(int serverID)
		{
			try
			{
				return GetServers().FirstOrDefault(s => s.ID == serverID);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
				MessageBox.Show(ex.Message, StringResources.GetString("Error"));
			}

			return null;
		}

		public static string GetLocalServersString()
		{
			try
			{
				return string.Join(",", GetServers().Select(s => s.ID.ToString()).ToArray());
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			return "0";
		}

		#endregion

		#region Check 1C

		/// <summary>
		///   �������� �� ����������� �������� ���� ��������� � 1�
		/// </summary>
		/// <param name="docTypeID"> ��� ������������ ���� ���������� </param>
		/// <returns> ���� �� ������ ��� ���������� � ��������� </returns>
		public static bool Is1CType(int docTypeID)
		{
			try
			{
				if(IsConnectedBuh)
				{
					if(_1CDocTypeIDs == null)
					{
						_1CDocTypeIDs = new SynchronizedCollection<int>();
						using(DataTable dt = BuhData.Get1CTypes())
						using(DataTableReader dr = dt.CreateDataReader())
						{
							while(dr.Read())
							{
								var item = (int)dr[BuhData.DocTypeIDField];
								_1CDocTypeIDs.Add(item);
							}
							dr.Close();
							dr.Dispose();
							dt.Dispose();
						}
					}
					if(_1CDocTypeIDs != null)
						return _1CDocTypeIDs.Contains(docTypeID);
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
			return false;
		}

		/// <summary>
		///   �������� �� ����������� �������� ���� ��������� � 1� ��������
		/// </summary>
		/// <param name="docTypeID"> ��� ������������ ���� ���������� </param>
		/// <returns> ���� �� ������ ��� ���������� � ��������� </returns>
		public static bool Is1CFoodType(int docTypeID)
		{
			try
			{
				if(IsConnectedBuh)
				{
					if(_1CFoodDocTypeIDs == null)
					{
						_1CFoodDocTypeIDs = new SynchronizedCollection<int>();
						using(DataTable dt = BuhData.Get1CFoodTypes())
						using(DataTableReader dr = dt.CreateDataReader())
						{
							while(dr.Read())
							{
								var item = (int)dr[BuhData.DocTypeIDField];
								_1CFoodDocTypeIDs.Add(item);
							}
							dr.Close();
							dr.Dispose();
							dt.Dispose();
						}
					}
					if(_1CFoodDocTypeIDs != null)
						return _1CFoodDocTypeIDs.Contains(docTypeID);
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
			return false;
		}

		/// <summary>
		///   �������� �� ����������� �������� ���� ��������� � 1� ��� ����������
		/// </summary>
		/// <param name="docTypeID"> ��� ������������ ���� ���������� </param>
		/// <returns> ���� �� ������ ��� ���������� � ��������� </returns>
		public static bool Is1CDictionaryType(int docTypeID)
		{
			try
			{
				if(IsConnectedDocs)
				{
					if(_1CDictionaryDocTypeIDs == null)
					{
						_1CDictionaryDocTypeIDs = new SynchronizedCollection<int>();
						using(DataTable dt = DocTypeData.GetBuhDirectoryDocTypes())
						using(DataTableReader dr = dt.CreateDataReader())
						{
							while(dr.Read())
							{
								var item = (int)dr[DocTypeData.IDField];
								_1CDictionaryDocTypeIDs.Add(item);
							}
							dr.Close();
							dr.Dispose();
							dt.Dispose();
						}
					}
					if(_1CDictionaryDocTypeIDs != null)
						return _1CDictionaryDocTypeIDs.Contains(docTypeID);
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
			return false;
		}

		/// <summary>
		///   ����� ����� 1�
		/// </summary>
		public static void ResetTypes()
		{
			_1CDictionaryDocTypeIDs = null;
			_1CDocTypeIDs = null;
			_1CFoodDocTypeIDs = null;
		}

		#endregion

		#region Stamps

		public static void ClearStamp()
		{
			if(stampDictionary != null)
				stampDictionary.Clear();
		}

		public static Image GetStamp(int stampID, int imageID)
		{
			if(stampDictionary == null)
				stampDictionary = new OrderedDictionary(10);
			if(stampDictionary.Contains(stampID))
				return stampDictionary[stampID as object] as Image;

			Image img = null;
			byte[] imageByte = StampData.GetStampImage(stampID, imageID);
			if(imageByte != null && imageByte.Length > 20)
			{
				MemoryStream ms = new MemoryStream(imageByte);
				Bitmap bmp = new Bitmap(ms);
				float resX = bmp.VerticalResolution;
				float resY = bmp.HorizontalResolution;
				bmp.MakeTransparent(Color.White);
				img = (Image)bmp.Clone();
				((Bitmap)img).SetResolution(resX, resY);
				bmp.Dispose();
				ms.Close();
				if(stampDictionary.Count == 10)
					stampDictionary.RemoveAt(9);
				stampDictionary.Insert(0, stampID, img);
			}
			return img;
		}

		public static Image GetDSP()
		{
			if(imgDSP == null)
			{
				Bitmap bmp = (Bitmap)global::Kesco.Lib.Win.Document.Properties.Resources.ResourceManager.GetObject("���");
				bmp.MakeTransparent(Color.White);
				imgDSP = new Bitmap(bmp);
				imgDSP.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
			}
			return (Image)imgDSP;
		}

		#endregion

		/// <summary>
		///   ���� ������.
		/// </summary>
		public static UndoRedoSta�k UndoredoStack
		{
			get { return undoredoStack ?? (undoredoStack = new UndoRedoSta�k()); }
		}

		public static Rectangle FormRectangle;

		public static bool IEOpenOnURL(string sURL)
		{
			Type objExpType = Type.GetTypeFromProgID("InternetExplorer.Application");

			if(objExpType == null)
			{
				Data.Env.WriteToLog("�� ������ ��� �������� ����������. �������� ������������.");
				return false;
			}
			object objExp = null;
			try
			{
				object oEmpty = String.Empty;
				object oURL = sURL;
				objExp = Activator.CreateInstance(objExpType);
				Screen sr = Screen.AllScreens.FirstOrDefault(x => x.Bounds.IntersectsWith(FormRectangle));
				if(sr != null)
				{
					object t = objExpType.InvokeMember("Top", BindingFlags.GetProperty, null, objExp, null);
					object l = objExpType.InvokeMember("Left", BindingFlags.GetProperty, null, objExp, null);
					object h = objExpType.InvokeMember("Width", BindingFlags.GetProperty, null, objExp, null);
					object w = objExpType.InvokeMember("Height", BindingFlags.GetProperty, null, objExp, null);
					int to = 0, le = 0, he = 0, wi = 0;
					if(t is int)
						to = (int)t;
					if(l is int)
						le = (int)l;
					if(w is int)
						he = (int)w;
					if(h is int)
						wi = (int)h;
					if(sr.Bounds.Width < wi || wi < 100)
					{ wi = sr.Bounds.Width; le = sr.Bounds.Left; }
					if(sr.Bounds.Height < he || he < 100)
					{ he = sr.Bounds.Height; to = sr.Bounds.Top; }
					if(sr.Bounds.Left > le || le > sr.Bounds.Right - wi)
						le = sr.Bounds.Left;
					if(sr.Bounds.Top > to || to > sr.Bounds.Bottom - he)
						to = sr.Bounds.Top;
					objExpType.InvokeMember("Top", BindingFlags.SetProperty, null, objExp, new object[] { to });
					objExpType.InvokeMember("Left", BindingFlags.SetProperty, null, objExp, new object[] { le });
					objExpType.InvokeMember("Width", BindingFlags.SetProperty, null, objExp, new object[] { wi });
					objExpType.InvokeMember("Height", BindingFlags.SetProperty, null, objExp, new object[] { he });
				}
				objExpType.InvokeMember("MenuBar", BindingFlags.SetProperty, null, objExp, new object[] { false });
				objExpType.InvokeMember("AddressBar", BindingFlags.SetProperty, null, objExp, new object[] { false });
				objExpType.InvokeMember("StatusBar", BindingFlags.SetProperty, null, objExp, new object[] { false });
				objExpType.InvokeMember("ToolBar", BindingFlags.SetProperty, null, objExp, new object[] { 0 });

				object obj = (uint)0x258;
				objExpType.InvokeMember("Navigate2", BindingFlags.InvokeMethod, null, objExp,
											  new[] { oURL, obj, "_top", oEmpty, oEmpty });
				objExpType.InvokeMember("Visible", BindingFlags.SetProperty, null, objExp, new object[] { true });
				object hw = objExpType.InvokeMember("HWND", BindingFlags.GetProperty, null, objExp, null);
				Win32.User32.SetForegroundWindow(new IntPtr((int)hw));
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
			finally
			{
				if(objExp != null)
					Marshal.ReleaseComObject(objExp);
				objExp = null;
			}
			return true;
		}

		/// <summary>
		///   ��������� ����� � ���� ����� �� ����
		/// </summary>
		/// <param name="id"> ��� ��������� </param>
		/// <returns> ������ ���� � ����� ����� ��� ���������� </returns>
		public static string GetFileNameFromID(int id)
		{
			return id < 10000000
					   ? (id / 10000).ToString("D4") + "\\" + id.ToString("D8")
					   : (id / 10000).ToString() + "\\" + id.ToString();
		}

		#region Links

		public static bool MakeDocsLink(int firstID, int secondID, int docString, int docType, DateTime? docTime)
		{
			if(secondID > 0)
			{
				string errorStr = null;

				object obj = DocLinksData.CheckDocLink(firstID, secondID);
				if(obj is int)
				{
					var result = (int)obj;
					switch(result)
					{
						case 1:
							//errorStr = "����� ��� ����.";
							return false;

						case 2:
							errorStr = StringResources.GetString("Environment.CheckLinkDoc.Error");
							break;
					}
				}
				else if(obj is DataTable)
				{
					Dialogs.LoopLinkDialog llDialog = new Dialogs.LoopLinkDialog(firstID, secondID, (DataTable)obj);
					llDialog.Show();

					return false;
				}

				if(errorStr != null)
				{
					MessageForm.Show(
						StringResources.GetString("Environment.CheckLinkDoc.Error2") + ":\n\n" + errorStr,
						StringResources.GetString("Error"));
					return false;
				}

				return true;
			}
			else
			{
			}
			return false;
		}

		#endregion

		#region DocTypesTable

		/// <summary>
		///   ��������� �������� �� �������� �������� ��������� ��������� �� ����
		/// </summary>
		/// <param name="docTypeID"> ��� ���� ��������� </param>
		/// <returns> </returns>
		public static bool DoesDocTypeNameExist(int docTypeID)
		{
			if(DocTypeData != null && DocTypes != null)
			{
				using(DataTableReader dr = docTypes.CreateDataReader())
				{
					while(dr.Read())
					{
						if((int)dr[docTypeData.IDField] != docTypeID)
							continue;

						object obj = dr[docTypeData.NameExistField];
						if(obj is bool)
							return (bool)obj;
						if(obj is byte)
							return (byte)obj > 0;
						if(obj is int)
							return (int)obj > 0;
					}
				}
			}

			return false;
		}

		/// <summary>
		///   �������� ������ ������ �� ���������� ���� ����������.
		/// </summary>
		/// <param name="docTypeID"> ��� ���� ���������� </param>
		public static DataRow GetDocTypeProperties(int docTypeID)
		{
			if(DocTypeData != null && DocTypes != null)
			{
				List<DataRow> dr =
					docTypes.Rows.Cast<DataRow>().Where<DataRow>(x => x[docTypeData.IDField].Equals(docTypeID)).ToList();
				if(dr != null && dr.Count > 0)
					return dr[0];

				docTypes.Dispose();
				docTypes = null;

				try
				{
					return DocTypes.Rows.Cast<DataRow>().Where<DataRow>(x => x[docTypeData.IDField].Equals(docTypeID)).ToList()[0];
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
				}
			}

			return null;
		}

		/// <summary>
		///   �������� ������������ ������������ �������� ���� ����������.
		/// </summary>
		/// <param name="docTypeID"> ��� ���� ���������� </param>
		public static string GetDocTypeName(int docTypeID)
		{
			if(DocTypeData != null && DocTypes != null)
			{
				List<DataRow> dr =
					docTypes.Rows.Cast<DataRow>().Where<DataRow>(x => x[docTypeData.IDField].Equals(docTypeID)).ToList();
				if(dr != null && dr.Count > 0)
					return dr[0][docTypeData.NameField].ToString();

				docTypes.Dispose();
				docTypes = null;

				try
				{
					return DocTypes.Rows.Cast<DataRow>().Where<DataRow>(
						x => x[docTypeData.IDField].Equals(docTypeID)).ToList()[0][docTypeData.NameField].ToString();
				}
				catch(Exception ex)
				{
					Data.Env.WriteToLog(ex);
				}
			}

			return string.Empty;
		}

		#endregion

		#region UsedPersons

		public static SynchronizedCollection<DataRow> UsedPersonsNames
		{
			get
			{
				if(UsedPersonsLoader == null)
				{
					UsedPersonsLoader = new BackgroundWorker();
					UsedPersonsLoader.DoWork += usedPersonsLoader_DoWork;
				}
				checkIfUsedPersonsAreLoaded();

				return usedPersonsNames;
			}
		}

		public static SynchronizedCollection<DataRow> UsedPersonsDates
		{
			get
			{
				if(UsedPersonsLoader == null)
				{
					UsedPersonsLoader = new BackgroundWorker();
					UsedPersonsLoader.DoWork += usedPersonsLoader_DoWork;
				}
				checkIfUsedPersonsAreLoaded();

				return usedPersonsDates;
			}
		}

		private static void usedPersonsLoader_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
                Console.WriteLine("{0}: UsedPersons are trying to be loaded", DateTime.Now.ToString("HH:mm:ss fff"));
				if(!IsConnectedDocs || PersonsUsedData == null)
					return;

				if(usedPersonsNames == null)
					usedPersonsNames = new SynchronizedCollection<DataRow>();
				if(usedPersonsDates == null)
					usedPersonsDates = new SynchronizedCollection<DataRow>();

				using(DataTable dt = PersonsUsedData.GetPersons())
				{
					foreach(DataRow dr in dt.Rows.Cast<DataRow>().Where(dr =>
						usedPersonsNames.Count(x => x[PersonsUsedData.IDField].Equals(dr[PersonsUsedData.IDField])) == 0))
						usedPersonsNames.Add(dr);
					dt.Dispose();
				}

				using(DataTable dtExt = PersonsUsedData.GetPersonsExtended())
				{
					foreach(DataRow dr in dtExt.Rows.Cast<DataRow>().Where(dr => usedPersonsDates.Count(
						x => x[PersonsUsedData.IDField].Equals(dr[PersonsUsedData.IDField]) &&
							 x[PersonData.FromFieldName].Equals(dr[PersonData.FromFieldName]) &&
							 x[PersonData.ToFieldName].Equals(dr[PersonData.ToFieldName])) == 0))
						usedPersonsDates.Add(dr);
					dtExt.Dispose();
				}
                Console.WriteLine("{0}: UsedPersons have been loaded", DateTime.Now.ToString("HH:mm:ss fff"));
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		internal static void checkIfUsedPersonsAreLoaded()
		{
			if(!Environment.IsConnectedDocs)
				return;
			if(usedPersonsDates == null || usedPersonsNames == null || usedPersonsDates.Count == 0 || usedPersonsNames.Count == 0)
			{
				if(!UsedPersonsLoader.IsBusy)
					UsedPersonsLoader.RunWorkerAsync();

				while(UsedPersonsLoader.IsBusy)
				{
					Application.DoEvents();
				}
			}
			while(usedPersonsDates == null || usedPersonsNames == null)
			{
				Application.DoEvents();
			}
		}

		/// <summary>
		///   �������� ������ �������������� �����-�������, �������������� �� ��������� ���� ���������
		/// </summary>
		/// <param name="docDate"> ���� ��������� </param>
		/// <returns> </returns>
		public static List<DataRow> GetUsedPersons(DateTime docDate)
		{
			checkIfUsedPersonsAreLoaded();

			if(docDate != null && usedPersonsDates.Count > 0 && usedPersonsNames.Count > 0 && PersonsUsedData != null && PersonData != null)
			{
				var personsList = new List<DataRow>();
				personsList.Clear();

				for(int i = usedPersonsDates.Count - 1; i > -1; i--)
				{
					DateTime fromDate = DateTime.MinValue;
					DateTime toDate = DateTime.MaxValue;
					try
					{
						fromDate = (DateTime)usedPersonsDates[i][personData.FromFieldName];
					}
					catch
					{
						fromDate = DateTime.MinValue;
					}
					try
					{
						toDate = (DateTime)usedPersonsDates[i][personData.ToFieldName];
					}
					catch
					{
						toDate = DateTime.MaxValue;
					}
					if(docDate >= fromDate && docDate < toDate)
					{
						while(UsedPersonsLoader.IsBusy)
						{
							Application.DoEvents();
						}
						personsList.Add(usedPersonsNames.Where(x => x[personsUsedData.IDField].Equals(usedPersonsDates[i][personsUsedData.IDField])).ToList()[0]);
					}
				}

				return personsList;
			}

			while(UsedPersonsLoader.IsBusy)
			{
				Application.DoEvents();
			}
			return usedPersonsNames.ToList();
		}

		/// <summary>
		///   ���������, ������������ �� ��������� �����-����� �� ��������� ���� ���������
		/// </summary>
		/// <param name="personID"> ��� �����-������ </param>
		/// <param name="docDate"> ���� ��������� </param>
		public static bool IsPersonValid(int personID, DateTime docDate)
		{
			if(docDate == null)
				return true;

			checkIfUsedPersonsAreLoaded();

			if(personID > 0 && usedPersonsDates.Count > 0 && PersonsUsedData != null && PersonData != null)
			{
				while(UsedPersonsLoader.IsBusy)
				{
					Application.DoEvents();
				}
				List<DataRow> validDates = usedPersonsDates.Where(x => x[personsUsedData.IDField].Equals(personID)).ToList();
				if(validDates != null && validDates.Count > 0)
					for(int i = validDates.Count - 1; i > -1; i--)
					{
						DateTime fromDate = DateTime.MinValue;
						DateTime toDate = DateTime.MaxValue;
						try
						{
							fromDate = (DateTime)validDates[i][personData.FromFieldName];
						}
						catch
						{
							fromDate = DateTime.MinValue;
						}
						try
						{
							toDate = (DateTime)validDates[i][personData.ToFieldName];
						}
						catch
						{
							toDate = DateTime.MaxValue;
						}
						if(docDate >= fromDate && docDate < toDate)
						{
							return true;
						}
					}
				else
					return DocData.CheckIsPersonValid(personID, docDate);
			}

			return false;
		}


		/// <summary>
		///   �������� �������� ���� ��������� ������ Outlook
		/// </summary>
		public static bool IsOutlookFile(string fileName)
		{
			if(!File.Exists(fileName))
				return false;

			var fi = new FileInfo(fileName);
			if(fi.Directory == null)
				return false;

			string dirName = fi.Directory.FullName.TrimEnd(Path.DirectorySeparatorChar);
			try
			{
				using(var regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office"))
				{
					var info = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
					info.NumberDecimalSeparator = ".";

					if(regKey == null)
						return false;

					double ver;
					string[] versions = regKey.GetSubKeyNames();
					foreach(string t in versions.Where(t => double.TryParse(t, NumberStyles.Float, info, out ver) && ver > 0))
					{
						using(Microsoft.Win32.RegistryKey versionKey = regKey.OpenSubKey(t + "\\Outlook\\Security"))
						{
							if(versionKey == null)
								continue;
							string opath = versionKey.GetValue("OutlookSecureTempFolder").ToString();
							if(!string.IsNullOrEmpty(opath) && opath.TrimEnd(Path.DirectorySeparatorChar).Equals(dirName, StringComparison.CurrentCultureIgnoreCase))
								return true;
						}
					}
				}
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}

			return false;
		}

		/// <summary>
		///   �������� �� ������ � �����
		/// </summary>
		public static bool IsAccessible(string fileName)
		{
			if(!File.Exists(fileName))
				return false;

			if((File.GetAttributes(fileName) & FileAttributes.ReadOnly) ==
				FileAttributes.ReadOnly)
				return false;

			if(IsOutlookFile(fileName))
				return false;

			try
			{
				using(FileStream fs = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
				{
				} //File.OpenWrite(fileName)) { }

				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		///   �������� �������� ���� ��������� ������ ������-���� readonly-�����
		/// </summary>
		public static bool IsTempFile(string fileName)
		{
			if(!File.Exists(fileName))
				return false;

			var fi = new FileInfo(fileName);
			string dirName = fi.Directory.FullName.TrimEnd(Path.DirectorySeparatorChar);

			return Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar).Equals(dirName, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		///   ���������� ����������� ��� �����-������
		/// </summary>
		/// <param name="personID"> ��� �����-������ </param>
		public static string GetPersonName(int personID)
		{
			checkIfUsedPersonsAreLoaded();
			DataRow dr = usedPersonsNames.FirstOrDefault(x => x[PersonsUsedData.IDField].Equals(personID));
			return dr == null ? PersonData.GetPerson(personID) : dr[PersonData.NameField].ToString();
		}

		#endregion

		#region Events

		public static event Components.DocumentSavedEventHandle NewWindow;

		/// <summary>
		/// ������� ������ ������� �������� ������ ��������������� ����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected internal static void OnNewWindow(object sender, Components.DocumentSavedEventArgs e)
		{
			if(NewWindow != null)
				NewWindow(sender, e);
		}

		public static event EventHandler NeedRefresh;

		/// <summary>
		/// ������� ������ ������� �������� ������ ��������������� ����
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected internal static void OnNeedRefresh(object sender, EventArgs e)
		{
			if(NeedRefresh != null)
				NeedRefresh(sender, e);
		}

		#endregion

		public static bool MoveFile(string file, ref DateTime creationTime, out string fName, out ServerInfo server)
        {
            try
            {
                var fileInfo = new System.IO.FileInfo(file);
                Console.WriteLine("{0}: file = {1}", DateTime.Now.ToString("HH:mm:ss fff"), file);
                server = Lib.Win.Document.Environment.GetRandomLocalServer();
                string ext = (Environment.IsPdf(fileInfo.FullName) ? "pdf" : "tif");
                fName = Environment.GenerateFileName(ext);
                string path = server.Path + "\\TEMP\\" + fName;
                Console.WriteLine("{0}: path = {1}", DateTime.Now.ToString("HH:mm:ss fff"), path);
                if (File.Exists(path))
                    File.Delete(path);
                creationTime = fileInfo.CreationTimeUtc;
                File.Move(fileInfo.FullName, path);
                return File.Exists(path);
            }
            catch (Exception ex)
            {
                Lib.Win.Data.Env.WriteToLog(ex);

                fName = string.Empty;
                server = null;
                return false;
            }
        }

		public static bool IsPdf(string fileName)
		{
			using( var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
			{
				int len = 1024;
				if(fs.Length < 1024)
					len = (int)fs.Length;
				if(len < 8)
					return false;
				var buf = new byte[len];
				fs.Position = 0;
				int res = fs.Read(buf, 0, len);
				if(res > 0)
					for(int i = 0; i < len - 3; i++)
					{
						if(buf[i] == 0x25 && buf[i + 1] == 0x50 && buf[i + 2] == 0x44 && buf[i + 3] == 0x46)
							return true;
					}
			}
			return false;
		}


		#region Read-only �����

		//����� ���� ����� ��������� ���� region �� Environment � ����� TmpFile?
		public static SynchronizedCollection<KeyValuePair<string, Objects.TmpFile>> TmpFiles = new SynchronizedCollection<KeyValuePair<string, Objects.TmpFile>>();

		public static bool TmpFilesContains(string fileName)
		{
			return TmpFiles.Any(t => fileName == t.Key || fileName == t.Value.TmpFullName);
		}

		public static bool AddTmpFile(string fileName, string tmpFileName, bool ispdf)
		{
			for(int i = 0; i < TmpFiles.Count; i++)
				if(fileName == TmpFiles[i].Key)
				{
					TmpFiles[i].Value.TmpFullName = tmpFileName;
					return false;
				}

			var tf = new KeyValuePair<string, Objects.TmpFile>(fileName, new Objects.TmpFile(fileName) { TmpFullName = tmpFileName });
			//if (ispdf)
			//{
			//    tf.Value.Phlp = new PDFHelper() {UseLock = true};
			//    string fP = "", uP = "";
			//    if (tf.Value.Phlp.Refresh(fileName, ref fP, ref uP).Count > 0)
			//    {
			//        tf.Value.Phlp.Open(fileName, fP);
			//        tf.Value.FilePass = fP;
			//    }
			//}
			//else
			//{
			//    tf.Value.Thlp = new Lib.Win.Tiff.LibTiffHelper() { IsUseLock = true };
			//    Bitmap bmp = null;
			//    tf.Value.TiffPtr = tf.Value.Thlp.TiffOpenRead(ref fileName, out bmp, false);
			//    if (bmp != null)
			//    {
			//        bmp.Dispose();
			//        bmp = null;
			//    }
			//}

			TmpFiles.Add(tf);
			return true;
		}

		public static void RemoveTmpFile(string fileName)
		{
			for(int i = 0; i < TmpFiles.Count; i++)
				if(fileName == TmpFiles[i].Key || fileName == TmpFiles[i].Value.TmpFullName)
				{
					try
					{
						if(TmpFiles[i].Value.Phlp != null)
							TmpFiles[i].Value.Phlp.Close();
						else if(TmpFiles[i].Value.Thlp != null && TmpFiles[i].Value.TiffPtr != IntPtr.Zero)
							TmpFiles[i].Value.Thlp.TiffCloseRead(ref TmpFiles[i].Value.TiffPtr);

						//Document.Slave.DeleteFile(TmpFiles[i].Value.TmpFullName);
						File.Delete(TmpFiles[i].Value.TmpFullName);
					}
					catch { }
					finally
					{
						TmpFiles.RemoveAt(i);
					}

					return;
				}
		}

		public static Objects.TmpFile GetTmpFileByKey(string fileName)
		{
			for(int i = 0; i < TmpFiles.Count; i++)
				if(fileName == TmpFiles[i].Key)
					return TmpFiles[i].Value;

			return null;
		}

		public static Objects.TmpFile GetTmpFileByValue(string fileName)
		{
			for(int i = 0; i < TmpFiles.Count; i++)
				if(fileName == TmpFiles[i].Value.TmpFullName)
					return TmpFiles[i].Value;

			return null;
		}

		public static Objects.TmpFile GetTmpFile(string fileName)
		{
			return (from t in TmpFiles where fileName == t.Key || fileName == t.Value.TmpFullName select t.Value).FirstOrDefault();
		}

		public static string GetTmpFileKey(string fileName)
		{
			return (from t in TmpFiles where fileName == t.Key || fileName == t.Value.TmpFullName select t.Key).FirstOrDefault();
		}

		public static KeyValuePair<string, Objects.TmpFile> GetTmpFilePair(string fileName)
		{
			foreach(var t in TmpFiles.Where(t => fileName == t.Key || fileName == t.Value.TmpFullName))
				return t;

			return new KeyValuePair<string, Objects.TmpFile>();
		}

		public static bool AnalyzeTmpFile(Objects.TmpFile tFile)
		{
			return AnalyzeTmpFile(true, tFile);
		}

		public static bool AnalyzeTmpFile(bool isMain, Objects.TmpFile tFile)
		{
			if(tFile.Modified)
			{
				string file = (tFile.DocId > 0 ? Environment.StringResources.GetString("SaveChangesDialog_Doc2") : Environment.StringResources.GetString("SaveChangesDialog_File2")).ToLower();
				if(new Lib.Win.MessageForm(string.Format(Environment.StringResources.GetString("DocControl_TmpFileWarning") + " " + Environment.StringResources.GetString("DocControl_TmpFileWarning0") + "{1}" +
									  Environment.StringResources.GetString(isMain ? "DocControl_TmpFileWarning1" : "DocControl_TmpFileWarning2"),
									  file/*Path.GetFileName(tFile.originalName)*/, System.Environment.NewLine), "", MessageBoxButtons.YesNo).ShowDialog() == DialogResult.Yes)
					return true;
			}
			return false;
		}
		#endregion

		public enum ActionBefore
		{
			None,
			LeaveFile,
			Save,
			SavePart,
			Print,
			SendFaxAndMail,
			DelPart,
			DelPartAfterSave
		}
	}
}