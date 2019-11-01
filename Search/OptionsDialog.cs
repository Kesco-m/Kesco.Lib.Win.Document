using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search;
using Kesco.Lib.Win.Document.Controls;
using Kesco.Lib.Win.Document.Search.Dialogs;

namespace Kesco.Lib.Win.Document.Search
{
	public class OptionsDialog : FreeDialog
	{
		[Flags]
		public enum EnabledFeatures
		{
			None = 0,
			Save = 1,
			SaveAs = 2,
			Search = 4,
			Clear = 8,
			SearchInFound = 16,
			AddToFound = 32,
			ShowCheckBoxes = SearchInFound | AddToFound,
			All = Save | SaveAs | Search | Clear | ShowCheckBoxes
		}

		private ItemCheckEventHandler lv_ItemCheckEventHandler;
		private Options.Root r;
		protected XmlDocument xml;
		protected int id;
		private int counterRefreshHtml;
		private string _text;
		private EnabledFeatures features;

		private Button bShowOptionsList;
		private ColumnHeader columnHeader1;
		private ListView lv;
		private TabControl tab;
		private Button bSave;
		private Panel panel1;
		private Button bCancel;
		private Panel panel2;
		private CheckBox cbInFound;
		private CheckBox cbToFound;
		private Splitter splitter1;
		private Button bClear;
		private ContextMenu contextMenu1;
		private MenuItem mnuXML;
		private MenuItem mnuSQL;
		private MenuItem mnuRefreshCount;
		private Panel panel3;
		private MenuItem mnuDsSchema;
		private Button bSaveAs;
		private Button bSearch;
		private OptionsSettings optionsSettings;

		/// <summary>
		///   Required designer variable.
		/// </summary>
		private Container components;

		private OptionsDialog(string xml, int id, EnabledFeatures features)
		{
			this.xml = new XmlDocument();
			this.id = id;

			if(id > 0)
				LoadXML(id);
			else if(xml != null)
				LoadXML(xml);
			else
				InitXML();

			this.features = features;

			InitializeComponent();
		}

		#region PUBLIC CONSTRUCTORS

		public OptionsDialog(EnabledFeatures features)
			: this(null, 0, features)
		{
		}

		public OptionsDialog(int id, EnabledFeatures features)
			: this(null, id, features)
		{
		}

		public OptionsDialog(int id)
			: this(null, id, EnabledFeatures.Clear |
							 EnabledFeatures.Save | EnabledFeatures.SaveAs)
		{
		}

		public OptionsDialog(string xml, EnabledFeatures features)
			: this(xml, 0, features)
		{
		}

		public OptionsDialog(string xml)
			: this(xml, 0, EnabledFeatures.Clear |
						   EnabledFeatures.SaveAs | EnabledFeatures.Search)
		{
		}

		public OptionsDialog()
			: this(null, 0, EnabledFeatures.Clear |
							EnabledFeatures.SaveAs | EnabledFeatures.Search |
							EnabledFeatures.ShowCheckBoxes)
		{
		}

		public OptionsDialog(bool showCheckBoxes)
			: this(null, 0,
				   (showCheckBoxes ? EnabledFeatures.ShowCheckBoxes : EnabledFeatures.None) |
				   EnabledFeatures.Clear |
				   EnabledFeatures.SaveAs |
				   EnabledFeatures.Search
				)
		{
		}

		#endregion

		/// <summary>
		///   Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
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
				new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
			this.bShowOptionsList = new System.Windows.Forms.Button();
			this.lv = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.tab = new System.Windows.Forms.TabControl();
			this.bSearch = new System.Windows.Forms.Button();
			this.bSave = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.bSaveAs = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.bClear = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.cbToFound = new System.Windows.Forms.CheckBox();
			this.cbInFound = new System.Windows.Forms.CheckBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.mnuXML = new System.Windows.Forms.MenuItem();
			this.mnuSQL = new System.Windows.Forms.MenuItem();
			this.mnuRefreshCount = new System.Windows.Forms.MenuItem();
			this.mnuDsSchema = new System.Windows.Forms.MenuItem();
			this.optionsSettings = new OptionsSettings();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// bShowOptionsList
			// 
			resources.ApplyResources(this.bShowOptionsList, "bShowOptionsList");
			this.bShowOptionsList.Name = "bShowOptionsList";
			// 
			// lv
			// 
			this.lv.CheckBoxes = true;
			this.lv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[]
                                         {
                                             this.columnHeader1
                                         });
			resources.ApplyResources(this.lv, "lv");
			this.lv.FullRowSelect = true;
			this.lv.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lv.Name = "lv";
			this.lv.UseCompatibleStateImageBehavior = false;
			this.lv.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			resources.ApplyResources(this.columnHeader1, "columnHeader1");
			// 
			// tab
			// 
			resources.ApplyResources(this.tab, "tab");
			this.tab.Multiline = true;
			this.tab.Name = "tab";
			this.tab.SelectedIndex = 0;
			// 
			// bSearch
			// 
			this.bSearch.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.bSearch, "bSearch");
			this.bSearch.Name = "bSearch";
			this.bSearch.Click += new System.EventHandler(this.bSearch_Click);
			// 
			// bSave
			// 
			resources.ApplyResources(this.bSave, "bSave");
			this.bSave.Name = "bSave";
			this.bSave.Click += new System.EventHandler(this.bSave_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel3);
			this.panel1.Controls.Add(this.bClear);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.bSaveAs);
			this.panel3.Controls.Add(this.bSave);
			this.panel3.Controls.Add(this.bSearch);
			this.panel3.Controls.Add(this.bCancel);
			resources.ApplyResources(this.panel3, "panel3");
			this.panel3.Name = "panel3";
			// 
			// bSaveAs
			// 
			resources.ApplyResources(this.bSaveAs, "bSaveAs");
			this.bSaveAs.Name = "bSaveAs";
			this.bSaveAs.Click += new System.EventHandler(this.bSaveAs_Click);
			// 
			// bCancel
			// 
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.bCancel, "bCancel");
			this.bCancel.Name = "bCancel";
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// bClear
			// 
			resources.ApplyResources(this.bClear, "bClear");
			this.bClear.Name = "bClear";
			this.bClear.Click += new System.EventHandler(this.bClear_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.cbToFound);
			this.panel2.Controls.Add(this.cbInFound);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			// 
			// cbToFound
			// 
			resources.ApplyResources(this.cbToFound, "cbToFound");
			this.cbToFound.Name = "cbToFound";
			this.cbToFound.CheckedChanged += new System.EventHandler(this.cbToFound_CheckedChanged);
			// 
			// cbInFound
			// 
			resources.ApplyResources(this.cbInFound, "cbInFound");
			this.cbInFound.Name = "cbInFound";
			this.cbInFound.CheckedChanged += new System.EventHandler(this.cbInFound_CheckedChanged);
			// 
			// splitter1
			// 
			resources.ApplyResources(this.splitter1, "splitter1");
			this.splitter1.Name = "splitter1";
			this.splitter1.TabStop = false;
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
                                                     {
                                                         this.mnuXML,
                                                         this.mnuSQL,
                                                         this.mnuRefreshCount,
                                                         this.mnuDsSchema
                                                     });
			// 
			// mnuXML
			// 
			this.mnuXML.Index = 0;
			resources.ApplyResources(this.mnuXML, "mnuXML");
			this.mnuXML.Click += new System.EventHandler(this.mnuXML_Click);
			// 
			// mnuSQL
			// 
			this.mnuSQL.Index = 1;
			resources.ApplyResources(this.mnuSQL, "mnuSQL");
			this.mnuSQL.Click += new System.EventHandler(this.mnuSQL_Click);
			// 
			// mnuRefreshCount
			// 
			this.mnuRefreshCount.Index = 2;
			resources.ApplyResources(this.mnuRefreshCount, "mnuRefreshCount");
			this.mnuRefreshCount.Click += new System.EventHandler(this.mnuRefreshCount_Click);
			// 
			// mnuDsSchema
			// 
			this.mnuDsSchema.Index = 3;
			resources.ApplyResources(this.mnuDsSchema, "mnuDsSchema");
			this.mnuDsSchema.Click += new System.EventHandler(this.mnuDsSchema_Click);
			// 
			// optionsSettings
			// 
			resources.ApplyResources(this.optionsSettings, "optionsSettings");
			this.optionsSettings.Name = "optionsSettings";
			// 
			// OptionsDialog
			// 
			this.AcceptButton = this.bSearch;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.bCancel;
			this.Controls.Add(this.optionsSettings);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.lv);
			this.Controls.Add(this.tab);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsDialog";
			this.Load += new System.EventHandler(this.OptionsDialog_Load);
			this.Closed += new System.EventHandler(this.OptionsDialog_Closed);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OptionsDialog_KeyDown);
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		#endregion

		private void MyInitializeComponent(EnabledFeatures features)
		{
			Text = id > 0 ? _text : Environment.StringResources.GetString("DocumentSearch");

			cbInFound.Visible = ((features & EnabledFeatures.SearchInFound) > 0);
			cbToFound.Visible = ((features & EnabledFeatures.AddToFound) > 0);
			cbInFound.Left = 8;
			cbToFound.Left = 8 +
							 (cbToFound.Visible ? cbToFound.Width + 8 : 0);
			panel2.Size = new Size(520, cbInFound.Visible || cbToFound.Visible ? 24 : 0);

			bClear.Visible = ((features & EnabledFeatures.Clear) > 0);
			bSaveAs.Visible = ((features & EnabledFeatures.SaveAs) > 0);

			bSave.Visible = ((features & EnabledFeatures.Save) > 0);
			bSave.Visible = (bSave.Visible && id > 0 && !bSaveAs.Visible);
			bSaveAs.Text = (id == 0)
							   ? Environment.StringResources.GetString("SaveInquiry")
							   : Environment.StringResources.GetString("SaveAs") + " ...";

			bSearch.Visible = ((features & EnabledFeatures.Search) > 0);

			bSearch.Left = 8;
			bCancel.Left = 8 +
						   (bSearch.Visible ? bSearch.Width + 8 : 0);

			bSave.Left = 8 +
						 (bSearch.Visible ? bSearch.Width + 8 : 0) +
						 (bCancel.Visible ? bCancel.Width + 8 : 0);
			AcceptButton = (bSearch.Visible) ? bSearch : bSave;
			bSaveAs.Left = 8 +
						   (bSearch.Visible ? bSearch.Width + 8 : 0) +
						   (bCancel.Visible ? bCancel.Width + 8 : 0) +
						   (bSave.Visible ? bSave.Width + 8 : 0);
			panel3.Width = 8 +
						   (bSearch.Visible ? bSearch.Width + 8 : 0) +
						   (bCancel.Visible ? bCancel.Width + 8 : 0) +
						   (bSave.Visible ? bSave.Width + 8 : 0) +
						   (bSaveAs.Visible ? bSaveAs.Width + 8 : 0);
			lv_ItemCheckEventHandler = lv_ItemCheck;
			lv.ItemCheck += lv_ItemCheckEventHandler;
		}

		private void OptionsDialog_Load(object sender, EventArgs e)
		{
			MyInitializeComponent(features);
			UserSettingsLoad();
			tab_init();

			optionsSettings.Command += optionsSettings_Command;

			string title = xml.DocumentElement.GetAttribute("title");
			if(title.Length > 0)
				Text = title;

			tab.Height = tab.ClientSize.Height - tab.DisplayRectangle.Height;
			columnHeader1.Width = lv.ClientSize.Width;

			try
			{
				WindowsIdentity wi = WindowsIdentity.GetCurrent();
				var p = new WindowsPrincipal(wi);
				if(p.IsInRole("EURO\\Programists") || p.IsInRole("TEST\\Programists") ||
					wi.Name.EndsWith("\\semen", true, null) || wi.Name.EndsWith("\\koval", true, null) ||
					wi.Name.EndsWith("\\zhdanov", true, null) || wi.Name.EndsWith("\\kubay", true, null))
					panel1.ContextMenu = contextMenu1;
			}
			catch
			{
			}

			panel1.Height = 40;
			all_refresh();
		}

		#region TAB

		private void tab_init()
		{
			var IDs = new int[Data.DALC.Documents.Search.Options.maxGroupCount];
			var names = new string[Data.DALC.Documents.Search.Options.maxGroupCount];
			var visible = new bool[Data.DALC.Documents.Search.Options.maxGroupCount];
			int n = Data.DALC.Documents.Search.Options.GetGroups(ref IDs, ref names);
			for(int i = 0; i < n; i++)
			{
				tab.TabPages.Add(new TabPage { Text = names[i], Tag = IDs[i] });
			}
			tab.SelectedIndexChanged += tab_SelectedIndexChanged;

			lv_refresh();
		}

		private void tab_Refresh()
		{
			if(tab == null)
				return;
			string s = xml.DocumentElement.GetAttribute("groupIndex");
			int groupIndex = (Regex.IsMatch(s, "^\\d+$") ? int.Parse(s) : 0);
			for(int i = 0; i < tab.TabPages.Count; i++)
				if((int)tab.TabPages[i].Tag == groupIndex)
					tab.SelectedIndex = i;
		}

		private void tab_SelectedIndexChanged(object sender, EventArgs e)
		{
			xml.DocumentElement.SetAttribute("groupIndex", tab.TabPages[tab.SelectedIndex].Tag.ToString());
			lv_refresh();
		}

		#endregion

		#region BROWSER

		private void optionsSettings_Command(Label lLab)
		{
			string url = lLab.Tag.ToString();
			Match m = Regex.Match(url, "#([^#;]+)(;|$)(([^#]+)$)?");
			if(!m.Success)
				return;

			var elOption =
				(XmlElement)xml.SelectSingleNode("Options/Option[@name=\"" + m.Groups[1].Value + "\"]");
			if(Regex.IsMatch(elOption.GetAttribute("fixed"), "^true$", RegexOptions.IgnoreCase))
				return;
			if(!string.IsNullOrEmpty(m.Groups[3].Value))
				EditNode(elOption);
			else
				EditNode(elOption);
		}

		private void html_refresh()
		{
			counterRefreshHtml++;

			optionsSettings.AddSettings(Data.DALC.Documents.Search.Options.GetHTML(xml));

			bClear.Enabled =
				cbToFound.Enabled =
				cbInFound.Enabled =
				bSearch.Enabled =
				bSaveAs.Enabled =
				bSave.Enabled =
				(xml.SelectNodes("Options/Option").Count > 0);

			bSave.Enabled = bSave.Enabled && (id > 0);

			bSearch.Focus();
		}

		#endregion

		#region LV

		private void lv_refresh()
		{
			if(lv == null)
				return;

			OptionAttribute[] metas = new OptionAttribute[Data.DALC.Documents.Search.Options.maxOptionsInGroupCount];
			int n = Data.DALC.Documents.Search.Options.GetOptionsInGroup((int)tab.TabPages[tab.SelectedIndex].Tag,
																			ref metas);
			lv.ItemCheck -= lv_ItemCheckEventHandler;
			lv.Items.Clear();
			for(int i = 0; i < n; i++)
			{
				XmlElement elOption = (XmlElement)xml.SelectSingleNode("Options/Option[@name=\"" + metas[i].Name + "\"]");
				ListViewItem li = new ListViewItem { Text = metas[i].Description, Tag = metas[i].Name, Checked = (elOption != null) };

				if(elOption != null && elOption.GetAttribute("open") == "true")
					EditNode(elOption);
				lv.Items.Add(li);
			}
			lv.ItemCheck += lv_ItemCheckEventHandler;
		}

		private void lv_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			ListViewItem li = lv.Items[e.Index];

			var elOptions = xml.SelectSingleNode("Options") as XmlElement;
			if(elOptions == null)
				return;

			var elOption = xml.SelectSingleNode("Options/Option[@name=\"" + li.Tag + "\"]") as XmlElement;

			if(elOption != null && elOption.GetAttribute("fixed") == "true")
			{
				e.NewValue = CheckState.Checked;
				return;
			}
			if(elOption != null && e.NewValue == CheckState.Unchecked)
			{
				elOptions.RemoveChild(elOption);
				Option option = Option.CreateOption(elOption);
				var suboptions = option.GetSubOptions();
				if(suboptions != null && suboptions.Count > 0)
				{
					foreach(string str in suboptions)
					{
						elOption = (XmlElement)xml.SelectSingleNode("Options/Option[@name='" + str + "']");
						if(elOption != null)
							elOptions.RemoveChild(elOption);
					}
				}
				html_refresh();
				return;
			}

			if(elOption == null && e.NewValue == CheckState.Checked)
			{
				elOption = xml.CreateElement("Option");
				elOption.SetAttribute("name", (string)li.Tag);
				Option option = Option.CreateOption(elOption);
				for(int i = 0; option.NegativeOption != null && i < option.NegativeOption.Length; i++)
				{
					XmlElement testOption = (XmlElement)xml.SelectSingleNode("Options/Option[@name=\"" + option.NegativeOption[i] + "\"]");
					if(testOption != null)
					{
						Option confOption = Option.CreateOption(testOption);
						MessageBox.Show(Environment.StringResources.GetString("ConflictItem") + Option.GetMeta(confOption.GetType()).Description);
						e.NewValue = CheckState.Unchecked;
						return;
					}
				}

				elOptions.AppendChild(elOption);
				var suboptions = option.GetSubOptions();
				if(suboptions != null && suboptions.Count > 0)
				{
					foreach(string str in suboptions)
					{
						XmlElement subxml = (XmlElement)xml.SelectSingleNode("Options/Option[@name='" + str + "']");
						if(subxml == null)
						{
							subxml = xml.CreateElement("Option");
							subxml.SetAttribute("name", str);
							elOptions.AppendChild(subxml);
						}
					}
				}
				html_refresh();
				EditNode(elOption, true);
			}
		}

		#endregion

		private void all_refresh()
		{
			tab_Refresh();
			lv_refresh();
			html_refresh();
		}

		#region XML

		private void InitXML()
		{
			xml.AppendChild(xml.CreateElement("Options"));
		}

		private void LoadXML(int id)
		{
			LoadXML((string)Environment.QueryData.GetRow(id)[Environment.QueryData.XMLField]);

			_text = ((string)Environment.QueryData.GetRow(id)[Environment.QueryData.NameField]);
		}

		private void LoadXML(string str)
		{
			xml.LoadXml(str);
			if(xml.DocumentElement == null ||
				xml.DocumentElement.NodeType != XmlNodeType.Element ||
				!xml.DocumentElement.Name.Equals("Options"))
				throw new Exception(Environment.StringResources.GetString("Search_OptionsDialog_LoadXML_Error1"));

			var el = xml.SelectSingleNode("Options/Option[@name=\"находится в Работе\"]") as XmlElement;
			if(el != null)
			{
				var elOptions = xml.SelectSingleNode("Options") as XmlElement;
				if(elOptions != null)
				{
					XmlElement elOption = xml.CreateElement("Option");
					elOption.SetAttribute("name", "ВРаботе");
					if(el.HasAttribute("open"))
						elOption.SetAttribute("open", el.GetAttribute("open"));
					elOption.SetAttribute("value", Environment.CurEmp.ID.ToString());
					elOptions.AppendChild(elOption);
					elOptions.RemoveChild(el);
				}
			}
			XmlNodeList list = xml.SelectNodes("Options/Option[@name='EForm.NoSing']");
			if(list != null && xml.DocumentElement != null)
				foreach(XmlElement elOption in list)
					xml.DocumentElement.RemoveChild(elOption);
		}

		private bool SaveXML(string name)
		{
			int p = Environment.QueryData.GetIDByName(name, Environment.CurEmp.ID);
			if(p != id)
			{
				if(
					MessageBox.Show(Environment.StringResources.GetString("Search_OptionsDialog_SaveXML_Message1") +
						System.Environment.NewLine + Environment.StringResources.GetString("Overwrite"),
						Environment.StringResources.GetString("SavingInquiry"), MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					Environment.QueryData.Delete(p);
				}
				else
				{
					SaveDialogOld(name);
					return false;
				}
			}
			return Environment.QueryData.Save(name, xml.OuterXml, Environment.CurEmp.ID, ref id);
		}

		private bool ValidateXML()
		{
			XmlNodeList list = xml.SelectNodes("Options/Option");

			if(list != null && xml.DocumentElement != null)
				foreach(XmlElement elOption in list)
				{
					Option option = Option.CreateOption(elOption);
					try
					{
						option.GetSQL(true);
					}
					catch(Exception ex)
					{
						if(
							MessageBox.Show(
								ex.Message + System.Environment.NewLine +
								Environment.StringResources.GetString("Search_OptionsDialog_ValidateXML_Error1"),
								Environment.StringResources.GetString("Warning"), MessageBoxButtons.YesNoCancel,
								MessageBoxIcon.Warning) == DialogResult.Yes)
							xml.DocumentElement.RemoveChild(elOption);
						else
							return false;
					}
				}
			all_refresh();
			var xmlNodeList = xml.SelectNodes("Options/Option");
			if(xmlNodeList != null && xmlNodeList.Count > 0)
				return true;

			if(
				MessageBox.Show(
					Environment.StringResources.GetString("Search_OptionsDialog_ValidateXML_Error2") +
					System.Environment.NewLine +
					Environment.StringResources.GetString("Search_OptionsDialog_ValidateXML_Error3"),
					Environment.StringResources.GetString("Warning"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) !=
				DialogResult.Yes)
				End(DialogResult.Cancel);

			return false;
		}

		#endregion

		private void OptionsDialog_Closed(object sender, EventArgs e)
		{
			UserSettingsSave();
		}

		public string GetXML()
		{
			return xml.OuterXml;
		}

		public bool GetInFound()
		{
			return cbInFound.Checked;
		}

		public bool GetToFound()
		{
			return cbToFound.Checked;
		}

		public int GetID()
		{
			return id;
		}

		#region UserInterfaceSettings

		private void UserSettingsLoad()
		{
			try
			{
				r = new Options.Root("DocView\\Layout\\Search");
				Width = r.LoadIntOption("Width", Width);
				Height = r.LoadIntOption("Height", Height);
				Left = r.LoadIntOption("Left", Left);
				Top = r.LoadIntOption("Top", Top);
				lv.Height = r.LoadIntOption("SplitterY", lv.Height);
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		private void UserSettingsSave()
		{
			if(r == null)
				return;
			try
			{
				r.Option("Width").Value = Width;
				r.Option("Height").Value = Height;
				r.Option("Left").Value = Left;
				r.Option("Top").Value = Top;
				r.Option("SplitterY").Value = lv.Height;
				r.Save();
			}
			catch(Exception ex)
			{
				Data.Env.WriteToLog(ex);
			}
		}

		#endregion

		private void mnuXML_Click(object sender, EventArgs e)
		{
			MessageBox.Show(xml.OuterXml);
		}

		private void mnuSQL_Click(object sender, EventArgs e)
		{
			try
			{
				MessageBox.Show(Data.DALC.Documents.Search.Options.GetSQL(xml.OuterXml));
			}
			catch(Exception ex)
			{
				Error.ErrorShower.OnShowError(this, ex.Message, Environment.StringResources.GetString("Error"));
			}
		}

		private void mnuRefreshCount_Click(object sender, EventArgs e)
		{
			MessageBox.Show(Data.DALC.Documents.Search.Options.GetHTML(xml.OuterXml));
		}

		private void mnuDsSchema_Click(object sender, EventArgs e)
		{
			MessageBox.Show(Data.DALC.Documents.Search.Options.GetText(xml.OuterXml));
		}

		#region EDIT NODE

		public void EditNode(XmlElement elOption)
		{
			EditNode(elOption, false);
		}

		/// <summary>
		///   редактирования элемента списка
		/// </summary>
		/// <param name="elOption"> опция для редактировния </param>
		/// <param name="check"> </param>
		public void EditNode(XmlElement elOption, bool check)
		{
			EditNode(elOption, null, check);
		}

		public void EditNode(XmlElement elOption, string value, bool check)
		{
			elOption.SetAttribute("open", "false");
			switch(elOption.GetAttribute("name"))
			{
				case "Message.Incoming.Date":
				case "Message.Outgoing.Date":
				case "Дата":
				case "Image.ДатаАрхивирования":
				case "Image.ДатаИзменения":
				case "Image.ДатаОтправки":
				case "Image.ИзмененоХранилище":
				//case "Image.ДатаОтправки":
				case "Image.ДатаНеОтправлялось":
				case "Image.Sing.ДатаАннулирования":
				case "Image.Sing.ДатаПодписания":
				case "EForm.Sing.ДатаВыполнения":
				case "EForm.Sing.ДатаПодписания":
				case "EForm.ДатаОплаты":
				case "Transaction.ДатаФиноперации":
					EditNodeWithForm(elOption, typeof(DateInterval), check);
					break;

				case "Message.Incoming.By":
				case "Message.Outgoing.By":
				case "Document.ChangedBy":
				case "Image.ChangedBy":
				case "Image.Сохранил":
				case "Image.ИзменилХранилище":
				case "Image.Отправитель":
				case "Image.НеОтправлялось":
				case "Image.NoSing.НеПодписан":
				case "Image.Sing.Аннулировано":
				case "Image.Sing.УстановленДСП":
				case "Image.Sing.Подписан":
				case "EForm.ChangedBy":
				case "EForm.NoSing.НеПодписан":
				case "EForm.LinkedToByCurator":
				case "EForm.Sing.Выполнен":
				case "EForm.Sing.Подписан":
					EditNodeWithForm(elOption, typeof(EmployeeList), check);
					break;
				case "Message.FromTo":
					EditNodeWithForm(elOption, typeof(TwoEmployeesLists), check);
					break;
				case "СуммаДокумента":
				case "Image.РазмерИзображения":
				case "Image.КоличествоСтраниц":
					EditNodeWithForm(elOption, typeof(NumericInterval), check);
					break;
				case "Описание":
					EditNodeWithForm(elOption, typeof(TextList), check);
					break;
				case "СвязанСДокументом":
					EditNodeWithForm(elOption, typeof(SvyazannyiDokument), check);
					break;
				case "ПечатнаяФорма":
				case "ОтсутствуетПечатнаяФорма":
					EditNodeWithForm(elOption, typeof(ТипПечатнойФормы), check);
					break;

				case "Название":
				case "Message.Incoming.Text":
				case "Message.Outgoing.Text":
					EditNodeWithForm(elOption, typeof(Text), check);
					break;

				case "СтрокаПоиска":
					EditNodeWithForm(elOption, typeof(Text0), check);
					break;
				case "КодДокумента":
					EditNodeWithForm(elOption, typeof(КодДокумента), check);
					break;
				case "ЛицаКонтрагенты":
					EditNodeWithForm(elOption, typeof(PersonListNew), check);
					break;
				case "Номер":
					EditNodeWithForm(elOption, typeof(Номер), check);
					break;
				case "ТипДокумента":
				case "ТипОснования":
				case "НетТипаОснования":
				case "ТипВытекающего":
				case "НетТипаВытекающего":
					EditNodeWithForm(elOption, typeof(ТипДокумента), check);
					break;
				case "Image.Хранилище":
					EditNodeWithForm(elOption, typeof(Хранилище), check);
					break;
				case "Transaction.ТипФиноперации":
					EditNodeWithForm(elOption, typeof(ТипыФинопераций), check);
					break;

				case "ВРаботе":
				case "Message.Incoming.Chief":
				case "Message.Incoming.Employee":
				case "EForm.NoSing.НеПодписанМной":
				case "EForm.Sing.ПодписанМной":
				case "Image.NoSing.НеПодписанМной":
				case "Image.Sing.ПодписанМной":
				case "EForm.ChangedByMe":
				case "EForm.LinkedToByCuratorMe":
				case "Message.UnReadByMe":
					elOption.SetAttribute("value", Environment.CurEmp.ID.ToString());
					break;

				case "1C.НеПроведен":
				case "1C.Проведен":
					EditNodeWithForm(elOption, typeof(Бухгалтерии), check);
					break;
				case "Image.ТипОтправки":
					EditNodeWithForm(elOption, typeof(ТипОтправки), check);
					break;
				case "Image.ТипФайлаИзображения":
					EditNodeWithForm(elOption, typeof(ТипФайлаИзображения), check);
					break;
				case "Image.Изображение":
				case "Image.ИзображениеОтсутствует":
				case "Image.Получено":
				case "EForm.NoSing.НеВыполнен":
				case "ЭлФорма":
				case "ЭлФормаОтсутствует":
					break;
			}
		}

		private void EditNodeWithForm(XmlElement elOption, Type type, bool check)
		{
			Option option = Option.CreateOption(elOption);
			if(!check || option.OpenWindow())
			{
				var frm = (Base)Activator.CreateInstance(type);
				frm.elOption = elOption;
				frm.option = option;
				frm.Text = frm.option.Meta.Description;
				frm.DialogEvent += EditNodeWithFormComplete;
				Enabled = false;
				ShowSubForm(frm);
			}
		}

		private void EditNodeWithFormComplete(object source, DialogEventArgs e)
		{
			Enabled = true;
			Focus();
			var frm = (Base)e.Dialog;
			if(frm.DialogResult == DialogResult.OK)
				all_refresh();
		}

		#endregion

		#region BUTTONS

		// CLEAR	
		private void bClear_Click(object sender, EventArgs e)
		{
			XmlNodeList list = xml.SelectNodes("Options/Option");
			if(list != null && xml.DocumentElement != null)
				foreach(XmlElement elOption in list.Cast<XmlElement>().Where(elOption => !Regex.IsMatch(elOption.GetAttribute("fixed"), "^true$", RegexOptions.IgnoreCase)))
				{
					xml.DocumentElement.RemoveChild(elOption);
				}
			all_refresh();
		}

		// SAVE AS	
		private void bSaveAs_Click(object sender, EventArgs e)
		{
			if(!ValidateXML())
				return;

			SaveDialog();
		}

		private void bSave_Click(object sender, EventArgs e)
		{
			if(id == 0)
				return;
			if(!ValidateXML())
				return;

			SaveDialog();
		}

		private void SaveDialog()
		{
			if(id == 0)
				SaveDialogOld();
			else
			{
				string newName = Data.DALC.Documents.Search.Options.GetShortText(xml);
				if(newName.Equals(_text))
					SaveDialogOld();
				else
					SaveDialogNew(newName);
			}
		}

		private void SaveDialogOld(string queryName)
		{
			var dlg =
				new EnterStringDialog(Environment.StringResources.GetString("Search_FrmSave_FrmSave_Load_Title1"),
									  Environment.StringResources.GetString("Search_OptionsDialog_SaveDialogOld_Message1") + ":",
									  queryName, false);
			dlg.DialogEvent += SaveDialogOldResult;
			ShowSubForm(dlg);
			Enabled = false;
		}

		private void SaveDialogOld()
		{
			SaveDialogOld(Data.DALC.Documents.Search.Options.GetShortText(xml));
		}

		private void SaveDialogNew(string newName)
		{
			var dlg = new Document.Dialogs.EnterQueryNameDialog(_text, newName);
			dlg.DialogEvent += SaveDialogNewResult;
			ShowSubForm(dlg);
			Enabled = false;
		}

		private void SaveDialogOldResult(object source, DialogEventArgs e)
		{
			Enabled = true;
			Focus();
			var dlg = (EnterStringDialog)e.Dialog;
			if(dlg.DialogResult != DialogResult.OK)
				return;

			SaveInsert(dlg.Input);
		}

		private void SaveDialogNewResult(object source, DialogEventArgs e)
		{
			Enabled = true;
			Focus();
			var dlg = (Document.Dialogs.EnterQueryNameDialog)e.Dialog;
			if(dlg.DialogResult != DialogResult.OK)
				return;

			if(dlg.QueryName.Equals(_text))
				SaveUpdate(dlg.QueryName);
			else
				SaveInsert(dlg.QueryName);
		}

		private void SaveInsert(string queryName)
		{
			int oldID = id;
			id = 0;
			if(SaveXML(queryName))
				End(DialogResult.Yes);
			else
				id = oldID; //if failed
		}

		private void SaveUpdate(string queryName)
		{
			if(SaveXML(queryName))
				End(DialogResult.Yes);
		}

		// SEARCH
		private void bSearch_Click(object sender, EventArgs e)
		{
			if(!ValidateXML())
				return;
			End(DialogResult.OK);
		}

		// CANCEL		
		private void bCancel_Click(object sender, EventArgs e)
		{
			End(DialogResult.Cancel);
		}

		#endregion

		private void OptionsDialog_KeyDown(object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
				case Keys.F3:
				case Keys.OemPeriod:
					var elOptions = (XmlElement)xml.SelectSingleNode("Options");
					var elOption = (XmlElement)xml.SelectSingleNode("Options/Option[@name=\"КодДокумента\"]");
					if(elOption == null && elOptions != null)
					{
						elOption = xml.CreateElement("Option");
						elOption.SetAttribute("name", "КодДокумента");
						elOptions.AppendChild(elOption);
						html_refresh();
					}
					EditNode(elOption);

					break;
			}
		}

		private void cbToFound_CheckedChanged(object sender, EventArgs e)
		{
			if(cbToFound.Checked)
				cbInFound.Checked = false;
		}

		private void cbInFound_CheckedChanged(object sender, EventArgs e)
		{
			if(cbInFound.Checked)
				cbToFound.Checked = false;
		}
	}
}