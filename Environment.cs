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
	///   статичиский класс содержаший общие для библиотеки переменные. требует обязательной инициализации методом Init(string).
	/// </summary>
	public class Environment
	{
		private const string subscribeTable = "Документы.dbo.РегистрацияРассылки";

		/// <summary>
		/// Словарь co штампами.
		/// </summary>
		private static OrderedDictionary stampDictionary;

		/// <summary>
		/// штамп "ДСП"
		/// </summary>
		private static Bitmap imgDSP;

		/// <summary>
		/// переменные для класса
		/// </summary>
		private static object[] archivPrinterParams; // параметры для повторного вызова печати документа
		private static string archivPrinterPath;

		private static string connectionStringDocument; // строка подключения к БД
		private static string connectionStringAccounting; // строка подключения к Бухгалтерии
		private static string connectionStringUser; // персональная строка для изменения языка
		private static PersonWord personWord; // объект с падежами "лица"
		private static Data.Temp.Objects.Employee curEmp; // текущий сотрудник
		private static bool isConnectedDocs = false; // есть ли связь с БД документов
		private static bool isConnectedBuh = false; // есть ли связь с БД бугалтерии
		private static string showHelpString; // Адрес строки помощи.
		private static string eFormString; // Адрес строки электронной формы
		private static string icExportString; // Адрес строки 1c экспорта
		private static string scalaExportString; // Адрес строки Scala экспорта
		private static string icMRExportString; // Адрес строки экспорта ресторана
		private static string settingsURLString; // Адрес строки htlm-ых свойств
		private static string reportingServiceURLString; // Адрес Reporting Servic-а
		private static string printTemplateString; // Адрес строки шаблона печати
		private static string personSearchString; // Адрес строки поиска лиц
		private static string employeeSearchString; // Адрес строки поиска сотрудников
		private static string createContactString; // Адрес строки создания контакта
		private static string createDocsLinkString; // Адрес строки создания связи между документами
		private static string personURL; // Адрес строки показа инф-ии по лицу
		private static string createClientString; // Адрес строки создания юридического лица
		private static string createClientPersonString; // Адрес строки создания физического лица
		private static string createTransactionString; // Адрес строки создания транзакции
		private static string showTransactionString; // Адрес строки показа транзакции
		private static string usersURL; // Адрес строки показа инф-ии по пользователю
		private static string dialURL; // Адрес строки показа контактов по пользователю
		private static string personParamStr; // параметры URL'а выбора лиц
		private static string empName; // ФИО сотрудника

		private static string mailNickname; // алиас сотрудника на маилсервере
		private static string mailBoxName; // email сотрудника на маилсервере

		internal static string userMultipleParamStr = "clid=3&return=2&UserOur=true"; // параметры URL'а выбора сотрудников (множественный)

		private static Form activeForm; // текущяя активная форма, для активации в случае перехвата фокуса 

		private static Options.Folder settings; // весь набор опций
		private static Options.Folder layout; // опции внешнего вида

		private static SynchronizedCollection<int> _1CDocTypeIDs; // документы проводимые в 1С
		private static SynchronizedCollection<int> _1CFoodDocTypeIDs; // документы проводимые в 1С Ресторан

		private static SynchronizedCollection<int> _1CDictionaryDocTypeIDs;// документы проводимые в 1С как справочники.

		private static PrinterList printers; // инфа по принтерам

		private static SynchronizedCollection<ServerInfo> servers; // инфа по архивам
		private static BackgroundWorker serversLoader;

		private static PrintReport.IReport report; // подключение к репортиг сервису


		private static ResourceManager stringResources; // данные для локализации

		private static CultureInfo curCultureInfo; // текущая локализация

		private static bool personMessage = true;

		private static ConcurrentDictionary<string, Form> docToSave;
		private static ConcurrentDictionary<string, Form> docToSend;
		private static Hashtable docToPrint;

		private static DataTable iC;

		private static UndoRedoStaсk undoredoStack; // стек отмены

		private static string server = string.Empty;		// URL EWS почтового сервера

		private static string mailServer = string.Empty;	// URL OWA почтового сервера

		private static string mailDomain = string.Empty; // строка домена из почты

		private static UserSettings userSettings; // настройки пользователя
		public static float Dpi;

		private static SynchronizedCollection<DataRow> usedPersonsNames; // список лиц-контрагентов с именами
		private static SynchronizedCollection<DataRow> usedPersonsDates; // список лиц-контрагентов с датами действия
		public static BackgroundWorker UsedPersonsLoader;

		public static SynchronizedCollection<KeyValuePair<int, Form>> OpenDocs =
			new SynchronizedCollection<KeyValuePair<int, Form>>(); // Открытые дополнительные окна

		private static DataTable docTypes;

		#region DALCs & Readers

		private static DocumentDALC docData; // документы
		private static WorkDocDALC workDocData; // рабочие документы
		private static DocDataDALC docDataData; // документыДанные
		private static QueryDALC queryData; // запросы

		private static FaxDALC faxData; // факсы
		private static FaxInDALC faxInData; // вх. факсы
		private static FaxOutDALC faxOutData; // исх. факсы
		private static FaxFolderDALC faxFolderData; // папки факсов

		private static SettingsDALC settingsData; // настройки
		private static DocLinksDALC docLinksData; // связи документов
		private static DocImageDALC docImageData; // изображ.
		private static DocTypeDALC docTypeData; // типы док.

		private static SignMessageTextDALC signTextData;// тексты для отпрвки при подписи документов

		private static DocTreeSPDALC docTreeSPData; // дерево док.

		private static FolderDALC folderData; // папки документов
		private static SharedFolderDALC sharedFolderData; // общие папки документов
		private static MessageDALC messageData; // сообщения
		private static FieldDALC fieldData; // поля
		private static ArchiveDALC archiveData; // хранилища
		private static FolderRuleDALC folderRuleData; // правила папок
		private static LogEmailDALC logEmailData; // лог почты
		private static PrintDataDALC printData; // дерево док.
		private static MailingListDALC mailingListData; // списки рассылки
		private static TransactionDALC transactionData; // транзакции по документу
		private static TransactionTypeDALC transactionTypeData; // типы транзакции 
		private static DocSignatureDALC docSignatureData; // подписи документов
		private static StampDALC stampData; // штампы документов

		private static URLsDALC urlsData; // адреса вызываемых веб диалогов
		private static PersonsUsedDALC personsUsedData; // последние использованные лица

		private static BuhParamDocDALC buhParamDocData; // бухгалтерские параметры документа

		private static SettingsPrintFormDALC settingsPrintForm; //настройки печатных форм

		private static PersonDALC personData; // лица
		private static PersonLinkDALC personLinkData; // связи лиц
		private static StoryDALC storyData; // склады
		private static FaxRecipientDALC faxRecipientData; // получатели факсов
		private static ResourceDALC resourceData; // ресурсы
		private static UnitDimensionDALC unitDimensionData; // единицы измерения

		private static BuhDALC buhData; // синхранизация с бухгалтерией
		private static EmployeeDALC empData; // сотрудники
		private static PhoneDALC phoneData; // обращения к телефонам.
		private static AreaDALC areaData; // страны
		private static OPFormsDALC oPFormsData; // организационно-правовые формы
		private static TypesPersonDALC typesPersonData; // типы лиц
		private static BusinessProjectDALC businessProjectData; // бизнес проекты

		private static ReplacementEmployeeDALС replacementEmployeeData;//список замещаемых сотрудников

		#endregion

		#region Accessors

        /// <summary>
        /// Контекст синхронизации. UI Thread
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static SynchronizationContext UIThreadSynchronizationContext { get; set; }

	    /// <summary>
		///   строка подключения к базе данных Документы
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
		///   строка подключения к базе данных для Бухгалтерии
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
		///   строка подключения к базе данных Инвентаризация
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
		///   строка вызова для создания электронной формы
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
		///   строка вызова html-ых свойств
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
		///   строка вызова Reporting Servica
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
		///   строка вызова помощи
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
		///   строка шаблона печати
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
		///   строка вызова экспорта 1c
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
		///   строка вызова экспорта Scala
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
		///   строка вызова экспорта ресторана
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
		///   строка вызова поиска лиц
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
		///   строка вызова поиска сотрудника
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
		///   строка вызова создания контакта
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
		///   строка вызова создания юридического лица
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
		///   строка вызова создания физического лица
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
		///   строка вызова создания связи между документами
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
		///   строка вызова создания транзакции
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
		///   строка вызова создания транзакции
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
		///   Строка вызова инф-ии о пользователе
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
		///   Строка вызова инф-ии о лице-контрагенте
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
		///   Строка вызова окна связи
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
		///   фиг знает что
		/// </summary>
		public static PersonWord PersonWord
		{
			get { return personWord; }
		}

		/// <summary>
		///   данные по подписке
		/// </summary>
		public static string SubscribeTable => subscribeTable;
		/// <summary>
		///   код текущего сотрудника из базы инвентаризация
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
		///   ФИО сотрудника
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
		///   проверка наличия подключения к базе документов
		/// </summary>
		public static bool IsConnectedDocs => isConnectedDocs;
		/// <summary>
		///   проверка наличия подключения к базе бухгалтерии
		/// </summary>
		public static bool IsConnectedBuh
		{
			get
			{
				return ((isConnectedBuh) ? isConnectedBuh : isConnectedBuh = Data.DALC.DALC.TestConnection(connectionStringAccounting));
			}
		}

		/// <summary>
		///   католог в реестре для сохранения установок
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
		///   папка в реестре, в которой хранятся данные о внешнем виде
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
		///   Настройки пользователя из базы
		/// </summary>
		public static UserSettings UserSettings
		{
			get { return isConnectedDocs ? userSettings ?? (userSettings = new UserSettings(SettingsData)) : null; }
		}

		/// <summary>
		///   информация о принтерах
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
		///   информация об 1С
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
		///   информация о архивах (серверах)
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
		///   Код организации архива сотрудника
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
		/// Url Ews почтового сервера
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
		/// Url Owa почтового сервера
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
		///   почтовое имя пользователя
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
		///   почтовое имя пользователя
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
		///   Основной далк для работы с документами
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
		/// доступ к папкам факсов
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
		///   доступ к типам документов
		/// </summary>
		public static DocTypeDALC DocTypeData
		{
			get { return docTypeData ?? (docTypeData = new DocTypeDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   доступ к тексту для отправки, при подписи документов
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
		///   Обрашение к подписям документа.
		/// </summary>
		public static DocSignatureDALC DocSignatureData
		{
			get { return docSignatureData ?? (docSignatureData = new DocSignatureDALC(connectionStringDocument)); }
		}

		/// <summary>
		///   Доступ к правам и изображением штампов
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
		///   DALC для работы с запросами
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
		///   DALC таблицы связи лиц
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
		///   DALC обращения к телефонам
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
		public static ReplacementEmployeeDALС ReplacementEmployeeData
		{
			get
			{
				return replacementEmployeeData ??
					   (replacementEmployeeData =
						new ReplacementEmployeeDALС(connectionStringDocument));
			}
		}

		#endregion

		#endregion

		/// <summary>
		///   статическая инициализация класса
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
		/// генерация временного имени
		/// </summary>
		/// <param name="extension"> расширение файла </param>
		/// <returns> строка с именем файла </returns>
		public static string GenerateFileName(string extension)
		{
			return "~" + Guid.NewGuid().ToString() + "." + extension;
		}

		/// <summary>
		/// генерация полного пути временного имени
		/// </summary>
		/// <param name="extension">расширение файла</param>
		/// <returns> строка с полным путём </returns>
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
		///   проверка на возможность получения факсов
		/// </summary>
		/// <returns> </returns>
		public static bool IsFaxReceiver()
		{
			return isConnectedDocs && EmpData.IsFaxReceiver();
		}

		/// <summary>
		///   Проверка, входит ли залогиненый пользователь windows в группу доменных админов
		/// </summary>
		/// <returns> true - входит, иначе false </returns>
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
		///   Путь к папке драйвера печати.
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
		///   Имя принтера UDC
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
		///   Версия принтера UDC
		/// </summary>
		public static float PrinterVersion => Checkers.TestPrinter.GetPrinterVersion();
		/// <summary>
		/// Версия принтера.
		/// Этот метод безопасен для вызова из WndProc
		/// </summary>
		/// <returns></returns>
		public static float PrinterVersionAsync => Checkers.TestPrinter.GetPrinterVersionAsync();
		/// <summary>
		///   Документ печатается с принтера UDC
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
		///   Получить параметры для повторной отправки документа на принтер
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
		///   Документ соответствует указанным параметрам
		/// </summary>
		public static object[] GetPrinterDocParams(string fileName)
		{
			if(archivPrinterParams == null || archivPrinterParams.Length != 7)
				return null;

			return archivPrinterParams;
		}

		/// <summary>
		///   проверка на возможность отправки из офиса
		/// </summary>
		public static bool IsFaxSender()
		{
			return isConnectedDocs && EmpData.IsFaxSender();
		}

		/// <summary>
		///   проверка на возможность отправки из офиса по коду документа
		/// </summary>
		public static bool IsFaxSender(int docID)
		{
			if(docID <= 0)
				return IsFaxSender();
			return isConnectedDocs && EmpData.IsFaxSender(docData.GetDocPersonsIDs(docID));
		}

		/// <summary>
		///   строка для задачи параметра Для Person
		/// </summary>
		public static string PersonParamStr
		{
			get { return personParamStr; }
			set { personParamStr = value; }
		}

		#region SendMessage

		/// <summary>
		///   определение функции посылки текста в окно
		/// </summary>
		/// <param name="wndHandle"> указатель на окно </param>
		/// <param name="sendText"> посылаемый текст </param>
		/// <returns> 0 - оправка не удалась 1 - удалась </returns>
		public static int SendMessage(IntPtr wndHandle, string sendText)
		{
			return SendMessage(wndHandle, sendText, true);
		}

		/// <summary>
		///   определение функции посылки отказа в окно
		/// </summary>
		/// <param name="wndHandle"> указатель на окно </param>
		/// <returns> 0 - оправка не удалась 1 - удалась </returns>
		public static int SendMessage(IntPtr wndHandle)
		{
			return SendMessage(wndHandle, "", false);
		}

		/// <summary>
		///   определение функции посылки текста в окно
		/// </summary>
		/// <param name="wndHandle"> указатель на окно </param>
		/// <param name="sendText"> текст посылки </param>
		/// <param name="send"> true текст отправляется, false нет </param>
		/// <returns> 0 - оправка не удалась 1 - удалась </returns>
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
		///   сохранение активной формы
		/// </summary>
		public static void SaveActiveForm()
		{
			activeForm = Form.ActiveForm;
		}

		/// <summary>
		///   востановление активной формы
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
		///   Проверка на возможность проводки типа документа в 1С
		/// </summary>
		/// <param name="docTypeID"> код проверяемого типа документов </param>
		/// <returns> есть ли данный тип документов в проводках </returns>
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
		///   Проверка на возможность проводки типа документа в 1С Ресторан
		/// </summary>
		/// <param name="docTypeID"> код проверяемого типа документов </param>
		/// <returns> есть ли данный тип документов в проводках </returns>
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
		///   Проверка на возможность проводки типа документа в 1С как справочник
		/// </summary>
		/// <param name="docTypeID"> код проверяемого типа документов </param>
		/// <returns> есть ли данный тип документов в проводках </returns>
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
		///   Сброс типов 1С
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
				Bitmap bmp = (Bitmap)global::Kesco.Lib.Win.Document.Properties.Resources.ResourceManager.GetObject("ДСП");
				bmp.MakeTransparent(Color.White);
				imgDSP = new Bitmap(bmp);
				imgDSP.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
			}
			return (Image)imgDSP;
		}

		#endregion

		/// <summary>
		///   Стек отмены.
		/// </summary>
		public static UndoRedoStaсk UndoredoStack
		{
			get { return undoredoStack ?? (undoredoStack = new UndoRedoStaсk()); }
		}

		public static Rectangle FormRectangle;

		public static bool IEOpenOnURL(string sURL)
		{
			Type objExpType = Type.GetTypeFromProgID("InternetExplorer.Application");

			if(objExpType == null)
			{
				Data.Env.WriteToLog("Не найден тип интернет эксплорера. Сообщите разработчику.");
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
		///   получение имени и пути файла по коду
		/// </summary>
		/// <param name="id"> код документа </param>
		/// <returns> строка пути и имени файла без расширения </returns>
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
							//errorStr = "Связь уже есть.";
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
		///   Указывает возможно ли указание названия документа отличного от типа
		/// </summary>
		/// <param name="docTypeID"> Код типа документа </param>
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
		///   Получает строку данных со свойствами типа документов.
		/// </summary>
		/// <param name="docTypeID"> Код типа документов </param>
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
		///   Получает отображаемое пользователю название типа документов.
		/// </summary>
		/// <param name="docTypeID"> Код типа документов </param>
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
		///   Получить список использованных контр-агентов, действительных на указанную дату документа
		/// </summary>
		/// <param name="docDate"> Дата документа </param>
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
		///   Проверяет, действителен ли указанный контр-агент на указанную дату документа
		/// </summary>
		/// <param name="personID"> Код контр-агента </param>
		/// <param name="docDate"> Дата документа </param>
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
		///   проверка является файл временным файлом Outlook
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
		///   проверка на доступ к файлу
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
		///   проверка является файл временной копией какого-либо readonly-файла
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
		///   Возвращает отображемое имя контр-агента
		/// </summary>
		/// <param name="personID"> Код контр-агента </param>
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
		/// функция вызова события открытия нового дополнительного окна
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
		/// функция вызова события открытия нового дополнительного окна
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


		#region Read-only файлы

		//Может есть смысл перенести этот region из Environment в класс TmpFile?
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