using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;
using Kesco.Lib.Win.Document.Blocks;
using Kesco.Lib.Win.Web;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
    public class PersonListNew : Base
    {
        private GroupBox gbOrganizations;
        private GroupBox gbOrganizationForm;
        private GroupBox gbEmploees;
        private RadioButton rbOR;
        private RadioButton rbAND;
        private PersonBlock pb;
        private CheckBox chBoxOrganizationsEnable;
        private CheckBox chBoxOrganization;
        private CheckBox chBoxBank;
        private CheckBox chBoxPhysicalPersons;
        private GroupBox gbPersons;
        private CheckBox chBoxPersonsEnable;
        private CheckBox chBoxPersonsCheck;
        private CheckBox chBoxPersonsUncheck;
        private CheckBox chBoxOrganizationalFormEnable;
        private ComboBox cbOrganizationalForm;
        private CheckBox chBoxEmployees;
        private GroupBox gbTypes;
        private CheckBox chBoxTypesEnable;
        private CheckBox chBoxSubtypes;
        private Button bnSelectEmployee;
        private ListBox lbSelectedEmploees;
        private Button bnSelectTypes;
        private ListBox lbTypes;
        private GroupBox gBCountryRegistration;
        private CheckBox chBoxCountryRegistrationEnable;
        private TextBox tbCountry;
        private Button bnSelectCountry;
        private GroupBox gbProject;
        private CheckBox chBoxBusinessProjectEnable;
        private TextBox tbBusinessProject;
        private Button bnSelectBusinessProject;
        private CheckBox chBoxAnyBusinessProject;
        private CheckBox chBoxIncludSubProject;

        private IContainer components;

        public PersonListNew()
        {
            InitializeComponent();
            DoubleBuffered = true;
            DopInitComponent();
            Init();
        }

        private void Init()
        {
            using (DataTable dt = Environment.OPFormsData.OPForms())
            using (DataTableReader dr = dt.CreateDataReader())
            {
                int maxWidth = cbOrganizationalForm.DropDownWidth;
                using (Graphics g = Graphics.FromHwnd(Handle))
                {
                    while (dr.Read())
                    {
                        var id = (int) dr[Environment.OPFormsData.IDField];
                        string name = dr[Environment.OPFormsData.NameField].ToString();
                        var item = new ComboBoxItem(id, name);
                        SizeF size = g.MeasureString(item.ToString(), Font);
                        int width = (int) size.Width + 5;
                        if (width <= 500)
                            maxWidth = Math.Max(maxWidth, width);
                        cbOrganizationalForm.Items.Add(item);
                    }
                }
                cbOrganizationalForm.DropDownWidth = maxWidth;
                dr.Close();
                dr.Dispose();
                dt.Dispose();
            }
            gbProject.Enabled = false;
        }


        private class ComboBoxItem
        {
            public int ID { get; private set; }

            public string Name { get; private set; }

            public ComboBoxItem(int id, string name)
            {
                ID = id;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }

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

        #region Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources =
                new System.ComponentModel.ComponentResourceManager(typeof (PersonListNew));
            this.pb = new PersonBlock();
            this.rbOR = new System.Windows.Forms.RadioButton();
            this.rbAND = new System.Windows.Forms.RadioButton();
            this.gbOrganizations = new System.Windows.Forms.GroupBox();
            this.chBoxPhysicalPersons = new System.Windows.Forms.CheckBox();
            this.chBoxOrganization = new System.Windows.Forms.CheckBox();
            this.chBoxOrganizationsEnable = new System.Windows.Forms.CheckBox();
            this.chBoxBank = new System.Windows.Forms.CheckBox();
            this.gbPersons = new System.Windows.Forms.GroupBox();
            this.chBoxPersonsCheck = new System.Windows.Forms.CheckBox();
            this.chBoxPersonsUncheck = new System.Windows.Forms.CheckBox();
            this.chBoxPersonsEnable = new System.Windows.Forms.CheckBox();
            this.gbOrganizationForm = new System.Windows.Forms.GroupBox();
            this.cbOrganizationalForm = new System.Windows.Forms.ComboBox();
            this.chBoxOrganizationalFormEnable = new System.Windows.Forms.CheckBox();
            this.gbEmploees = new System.Windows.Forms.GroupBox();
            this.lbSelectedEmploees = new System.Windows.Forms.ListBox();
            this.bnSelectEmployee = new System.Windows.Forms.Button();
            this.chBoxEmployees = new System.Windows.Forms.CheckBox();
            this.gbTypes = new System.Windows.Forms.GroupBox();
            this.lbTypes = new System.Windows.Forms.ListBox();
            this.bnSelectTypes = new System.Windows.Forms.Button();
            this.chBoxSubtypes = new System.Windows.Forms.CheckBox();
            this.chBoxTypesEnable = new System.Windows.Forms.CheckBox();
            this.gBCountryRegistration = new System.Windows.Forms.GroupBox();
            this.chBoxCountryRegistrationEnable = new System.Windows.Forms.CheckBox();
            this.tbCountry = new System.Windows.Forms.TextBox();
            this.bnSelectCountry = new System.Windows.Forms.Button();
            this.gbProject = new System.Windows.Forms.GroupBox();
            this.chBoxIncludSubProject = new System.Windows.Forms.CheckBox();
            this.chBoxAnyBusinessProject = new System.Windows.Forms.CheckBox();
            this.bnSelectBusinessProject = new System.Windows.Forms.Button();
            this.tbBusinessProject = new System.Windows.Forms.TextBox();
            this.chBoxBusinessProjectEnable = new System.Windows.Forms.CheckBox();
            this.gbOrganizations.SuspendLayout();
            this.gbPersons.SuspendLayout();
            this.gbOrganizationForm.SuspendLayout();
            this.gbEmploees.SuspendLayout();
            this.gbTypes.SuspendLayout();
            this.gBCountryRegistration.SuspendLayout();
            this.gbProject.SuspendLayout();
            this.SuspendLayout();
            // 
            // pb
            // 
            this.pb.Able = true;
            resources.ApplyResources(this.pb, "pb");
            this.pb.Name = "pb";
            // 
            // rbOR
            // 
            this.rbOR.Checked = true;
            resources.ApplyResources(this.rbOR, "rbOR");
            this.rbOR.Name = "rbOR";
            this.rbOR.TabStop = true;
            // 
            // rbAND
            // 
            resources.ApplyResources(this.rbAND, "rbAND");
            this.rbAND.Name = "rbAND";
            // 
            // gbOrganizations
            // 
            this.gbOrganizations.Controls.Add(this.chBoxPhysicalPersons);
            this.gbOrganizations.Controls.Add(this.chBoxOrganization);
            this.gbOrganizations.Controls.Add(this.chBoxOrganizationsEnable);
            this.gbOrganizations.Controls.Add(this.chBoxBank);
            resources.ApplyResources(this.gbOrganizations, "gbOrganizations");
            this.gbOrganizations.Name = "gbOrganizations";
            this.gbOrganizations.TabStop = false;
            // 
            // chBoxPhysicalPersons
            // 
            resources.ApplyResources(this.chBoxPhysicalPersons, "chBoxPhysicalPersons");
            this.chBoxPhysicalPersons.Name = "chBoxPhysicalPersons";
            // 
            // chBoxOrganization
            // 
            resources.ApplyResources(this.chBoxOrganization, "chBoxOrganization");
            this.chBoxOrganization.Name = "chBoxOrganization";
            // 
            // chBoxOrganizationsEnable
            // 
            resources.ApplyResources(this.chBoxOrganizationsEnable, "chBoxOrganizationsEnable");
            this.chBoxOrganizationsEnable.Name = "chBoxOrganizationsEnable";
            this.chBoxOrganizationsEnable.CheckedChanged += new System.EventHandler(this.chBoxEnable_CheckedChanged);
            // 
            // chBoxBank
            // 
            resources.ApplyResources(this.chBoxBank, "chBoxBank");
            this.chBoxBank.Name = "chBoxBank";
            // 
            // gbPersons
            // 
            this.gbPersons.Controls.Add(this.chBoxPersonsCheck);
            this.gbPersons.Controls.Add(this.chBoxPersonsUncheck);
            this.gbPersons.Controls.Add(this.chBoxPersonsEnable);
            resources.ApplyResources(this.gbPersons, "gbPersons");
            this.gbPersons.Name = "gbPersons";
            this.gbPersons.TabStop = false;
            // 
            // chBoxPersonsCheck
            // 
            resources.ApplyResources(this.chBoxPersonsCheck, "chBoxPersonsCheck");
            this.chBoxPersonsCheck.Name = "chBoxPersonsCheck";
            // 
            // chBoxPersonsUncheck
            // 
            resources.ApplyResources(this.chBoxPersonsUncheck, "chBoxPersonsUncheck");
            this.chBoxPersonsUncheck.Name = "chBoxPersonsUncheck";
            // 
            // chBoxPersonsEnable
            // 
            resources.ApplyResources(this.chBoxPersonsEnable, "chBoxPersonsEnable");
            this.chBoxPersonsEnable.Name = "chBoxPersonsEnable";
            this.chBoxPersonsEnable.CheckedChanged += new System.EventHandler(this.chBoxEnable_CheckedChanged);
            // 
            // gbOrganizationForm
            // 
            this.gbOrganizationForm.Controls.Add(this.cbOrganizationalForm);
            this.gbOrganizationForm.Controls.Add(this.chBoxOrganizationalFormEnable);
            resources.ApplyResources(this.gbOrganizationForm, "gbOrganizationForm");
            this.gbOrganizationForm.Name = "gbOrganizationForm";
            this.gbOrganizationForm.TabStop = false;
            // 
            // cbOrganizationalForm
            // 
            resources.ApplyResources(this.cbOrganizationalForm, "cbOrganizationalForm");
            this.cbOrganizationalForm.Name = "cbOrganizationalForm";
            // 
            // chBoxOrganizationalFormEnable
            // 
            resources.ApplyResources(this.chBoxOrganizationalFormEnable, "chBoxOrganizationalFormEnable");
            this.chBoxOrganizationalFormEnable.Name = "chBoxOrganizationalFormEnable";
            this.chBoxOrganizationalFormEnable.CheckedChanged += new System.EventHandler(this.chBoxEnable_CheckedChanged);
            // 
            // gbEmploees
            // 
            this.gbEmploees.Controls.Add(this.lbSelectedEmploees);
            this.gbEmploees.Controls.Add(this.bnSelectEmployee);
            this.gbEmploees.Controls.Add(this.chBoxEmployees);
            resources.ApplyResources(this.gbEmploees, "gbEmploees");
            this.gbEmploees.Name = "gbEmploees";
            this.gbEmploees.TabStop = false;
            // 
            // lbSelectedEmploees
            // 
            resources.ApplyResources(this.lbSelectedEmploees, "lbSelectedEmploees");
            this.lbSelectedEmploees.Name = "lbSelectedEmploees";
            // 
            // bnSelectEmployee
            // 
            resources.ApplyResources(this.bnSelectEmployee, "bnSelectEmployee");
            this.bnSelectEmployee.Name = "bnSelectEmployee";
            this.bnSelectEmployee.Click += new System.EventHandler(this.bnSelectEmployee_Click);
            // 
            // chBoxEmployees
            // 
            resources.ApplyResources(this.chBoxEmployees, "chBoxEmployees");
            this.chBoxEmployees.Name = "chBoxEmployees";
            this.chBoxEmployees.CheckedChanged += new System.EventHandler(this.chBoxEnable_CheckedChanged);
            // 
            // gbTypes
            // 
            this.gbTypes.Controls.Add(this.lbTypes);
            this.gbTypes.Controls.Add(this.bnSelectTypes);
            this.gbTypes.Controls.Add(this.chBoxSubtypes);
            this.gbTypes.Controls.Add(this.chBoxTypesEnable);
            resources.ApplyResources(this.gbTypes, "gbTypes");
            this.gbTypes.Name = "gbTypes";
            this.gbTypes.TabStop = false;
            // 
            // lbTypes
            // 
            resources.ApplyResources(this.lbTypes, "lbTypes");
            this.lbTypes.Name = "lbTypes";
            // 
            // bnSelectTypes
            // 
            resources.ApplyResources(this.bnSelectTypes, "bnSelectTypes");
            this.bnSelectTypes.Name = "bnSelectTypes";
            this.bnSelectTypes.Click += new System.EventHandler(this.bnSelectTypes_Click);
            // 
            // chBoxSubtypes
            // 
            resources.ApplyResources(this.chBoxSubtypes, "chBoxSubtypes");
            this.chBoxSubtypes.Name = "chBoxSubtypes";
            // 
            // chBoxTypesEnable
            // 
            resources.ApplyResources(this.chBoxTypesEnable, "chBoxTypesEnable");
            this.chBoxTypesEnable.Name = "chBoxTypesEnable";
            this.chBoxTypesEnable.CheckedChanged += new System.EventHandler(this.chBoxEnable_CheckedChanged);
            // 
            // gBCountryRegistration
            // 
            this.gBCountryRegistration.Controls.Add(this.chBoxCountryRegistrationEnable);
            this.gBCountryRegistration.Controls.Add(this.tbCountry);
            this.gBCountryRegistration.Controls.Add(this.bnSelectCountry);
            resources.ApplyResources(this.gBCountryRegistration, "gBCountryRegistration");
            this.gBCountryRegistration.Name = "gBCountryRegistration";
            this.gBCountryRegistration.TabStop = false;
            // 
            // chBoxCountryRegistrationEnable
            // 
            resources.ApplyResources(this.chBoxCountryRegistrationEnable, "chBoxCountryRegistrationEnable");
            this.chBoxCountryRegistrationEnable.Name = "chBoxCountryRegistrationEnable";
            this.chBoxCountryRegistrationEnable.CheckedChanged +=
                new System.EventHandler(this.chBoxEnable_CheckedChanged);
            // 
            // tbCountry
            // 
            resources.ApplyResources(this.tbCountry, "tbCountry");
            this.tbCountry.Name = "tbCountry";
            // 
            // bnSelectCountry
            // 
            resources.ApplyResources(this.bnSelectCountry, "bnSelectCountry");
            this.bnSelectCountry.Name = "bnSelectCountry";
            this.bnSelectCountry.Click += new System.EventHandler(this.bnSelectCountry_Click);
            // 
            // gbProject
            // 
            this.gbProject.Controls.Add(this.chBoxIncludSubProject);
            this.gbProject.Controls.Add(this.chBoxAnyBusinessProject);
            this.gbProject.Controls.Add(this.bnSelectBusinessProject);
            this.gbProject.Controls.Add(this.tbBusinessProject);
            this.gbProject.Controls.Add(this.chBoxBusinessProjectEnable);
            resources.ApplyResources(this.gbProject, "gbProject");
            this.gbProject.Name = "gbProject";
            this.gbProject.TabStop = false;
            // 
            // chBoxIncludSubProject
            // 
            resources.ApplyResources(this.chBoxIncludSubProject, "chBoxIncludSubProject");
            this.chBoxIncludSubProject.Name = "chBoxIncludSubProject";
            // 
            // chBoxAnyBusinessProject
            // 
            resources.ApplyResources(this.chBoxAnyBusinessProject, "chBoxAnyBusinessProject");
            this.chBoxAnyBusinessProject.Name = "chBoxAnyBusinessProject";
            this.chBoxAnyBusinessProject.CheckedChanged +=
                new System.EventHandler(this.chBoxAnyBusinessProject_CheckedChanged);
            // 
            // bnSelectBusinessProject
            // 
            resources.ApplyResources(this.bnSelectBusinessProject, "bnSelectBusinessProject");
            this.bnSelectBusinessProject.Name = "bnSelectBusinessProject";
            this.bnSelectBusinessProject.Click += new System.EventHandler(this.bnSelectBusinessProject_Click);
            // 
            // tbBusinessProject
            // 
            resources.ApplyResources(this.tbBusinessProject, "tbBusinessProject");
            this.tbBusinessProject.Name = "tbBusinessProject";
            this.tbBusinessProject.TextChanged += new System.EventHandler(this.tbBusinessProject_TextChanged);
            // 
            // chBoxBusinessProjectEnable
            // 
            resources.ApplyResources(this.chBoxBusinessProjectEnable, "chBoxBusinessProjectEnable");
            this.chBoxBusinessProjectEnable.Name = "chBoxBusinessProjectEnable";
            this.chBoxBusinessProjectEnable.CheckedChanged += new System.EventHandler(this.chBoxEnable_CheckedChanged);
            // 
            // PersonListNew
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.gbProject);
            this.Controls.Add(this.gBCountryRegistration);
            this.Controls.Add(this.gbTypes);
            this.Controls.Add(this.gbEmploees);
            this.Controls.Add(this.gbOrganizationForm);
            this.Controls.Add(this.gbPersons);
            this.Controls.Add(this.gbOrganizations);
            this.Controls.Add(this.rbAND);
            this.Controls.Add(this.rbOR);
            this.Controls.Add(this.pb);
            this.Name = "PersonListNew";
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PersonListNew_MouseUp);
            this.Controls.SetChildIndex(this.pb, 0);
            this.Controls.SetChildIndex(this.rbOR, 0);
            this.Controls.SetChildIndex(this.rbAND, 0);
            this.Controls.SetChildIndex(this.gbOrganizations, 0);
            this.Controls.SetChildIndex(this.gbPersons, 0);
            this.Controls.SetChildIndex(this.gbOrganizationForm, 0);
            this.Controls.SetChildIndex(this.gbEmploees, 0);
            this.Controls.SetChildIndex(this.gbTypes, 0);
            this.Controls.SetChildIndex(this.gBCountryRegistration, 0);
            this.Controls.SetChildIndex(this.gbProject, 0);
            this.gbOrganizations.ResumeLayout(false);
            this.gbPersons.ResumeLayout(false);
            this.gbOrganizationForm.ResumeLayout(false);
            this.gbEmploees.ResumeLayout(false);
            this.gbTypes.ResumeLayout(false);
            this.gBCountryRegistration.ResumeLayout(false);
            this.gBCountryRegistration.PerformLayout();
            this.gbProject.ResumeLayout(false);
            this.gbProject.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private void DopInitComponent()
        {
            CloseOnEnter = false;
        }

        protected override void FillForm()
        {
            base.FillForm();

            if (option == null)
                return;

            var o =(PersonListOption) option;

            switch (o.Mode)
            {
                case PersonListOption.Modes.AND:
                    rbAND.Checked = true;
                    break;
                case PersonListOption.Modes.OR:
                    rbOR.Checked = true;
                    break;
                default:
                    rbAND.Checked = true;
                    break;
            }

            if (PersonListOption.Modes.PersonOrganization ==
                (o.Mode & PersonListOption.Modes.PersonOrganization))
                chBoxOrganization.Checked = true;
            if (PersonListOption.Modes.PersonBank ==
                (o.Mode & PersonListOption.Modes.PersonBank))
                chBoxBank.Checked = true;
            if (PersonListOption.Modes.PersonPhysical ==
                (o.Mode & PersonListOption.Modes.PersonPhysical))
                chBoxPhysicalPersons.Checked = true;
            if (chBoxPhysicalPersons.Checked || chBoxBank.Checked || chBoxOrganization.Checked)
                chBoxOrganizationsEnable.Checked = true;
            else
                chBoxOrganizationsEnable.Checked = false;
            ChangeStateControl(chBoxOrganizationsEnable);

            if (PersonListOption.Modes.PersonUncheck ==
                (o.Mode & PersonListOption.Modes.PersonUncheck))
                chBoxPersonsUncheck.Checked = true;
            if (PersonListOption.Modes.PersonCheck ==
                (o.Mode & PersonListOption.Modes.PersonCheck))
                chBoxPersonsCheck.Checked = true;
            if (chBoxPersonsCheck.Checked || chBoxPersonsUncheck.Checked)
                chBoxPersonsEnable.Checked = true;
            else
                chBoxPersonsEnable.Checked = false;
            ChangeStateControl(chBoxPersonsEnable);

            //организационно правовая форма
            if (PersonListOption.Modes.PersonOPForma ==
                (o.Mode & PersonListOption.Modes.PersonOPForma))
            {
                if (o.IDOPForm >= 0)
                {
                    foreach (ComboBoxItem item in cbOrganizationalForm.Items)
                    {
                        if (item.ID == o.IDOPForm)
                        {
                            cbOrganizationalForm.SelectedItem = item;
                            break;
                        }
                    }
                }
                chBoxOrganizationalFormEnable.Checked = true;
            }
            ChangeStateControl(chBoxOrganizationalFormEnable);

            //страна регистрации
            if (PersonListOption.Modes.PersonArea ==
                (o.Mode & PersonListOption.Modes.PersonArea))
            {
                if (o.IDArea >= 0)
                {
                    tbCountry.Tag = o.IDArea;
                    DataRow row = Environment.AreaData.GetRow(o.IDArea);
                    if (row != null)
                    {
                        tbCountry.Text = (string) row["Страна"];
                    }
                }
                chBoxCountryRegistrationEnable.Checked = true;
            }
            ChangeStateControl(chBoxCountryRegistrationEnable);

            //ответственные сотрудники
            if (PersonListOption.Modes.PersonUsers ==
                (o.Mode & PersonListOption.Modes.PersonUsers))
            {
                if (o.ListUsers.Count > 0)
                {
                    lbSelectedEmploees.Items.Clear();
                    foreach (string k in o.ListUsers.Keys)
                    {
                        var item = new ComboBoxItem(Convert.ToInt32(k), o.ListUsers[k].ToString());

                        lbSelectedEmploees.Items.Add(item);
                    }
                }
                chBoxEmployees.Checked = true;
            }
            ChangeStateControl(chBoxEmployees);

            //оиграничение по типам
            if (PersonListOption.Modes.PersonThemes ==
                (o.Mode & PersonListOption.Modes.PersonThemes))
            {
                if (o.ListTypes.Count > 0)
                {
                    lbTypes.Items.Clear();
                    foreach (string k in o.ListTypes.Keys)
                    {
                        var item = new ComboBoxItem(Convert.ToInt32(k), o.ListTypes[k].ToString());

                        lbTypes.Items.Add(item);
                    }
                }
                chBoxTypesEnable.Checked = true;
                if (PersonListOption.Modes.PersonSubThemes ==
                    (o.Mode & PersonListOption.Modes.PersonSubThemes))
                    chBoxSubtypes.Checked = true;
            }
            ChangeStateControl(chBoxTypesEnable);

            //Бизнес проекты
            if (PersonListOption.Modes.PersonAnyBusinessProject ==
                (o.Mode & PersonListOption.Modes.PersonAnyBusinessProject))
            {
                chBoxBusinessProjectEnable.Checked = true;
                chBoxAnyBusinessProject.Checked = true;
            }
            else if (PersonListOption.Modes.PersonBusinessProject ==
                     (o.Mode & PersonListOption.Modes.PersonBusinessProject))
            {
                chBoxBusinessProjectEnable.Checked = true;
                tbBusinessProject.Text = o.NameBusinessProject;
                tbBusinessProject.Tag = o.IDBusinessProject;

                if (PersonListOption.Modes.PersonSubBusinessProject ==
                    (o.Mode & PersonListOption.Modes.PersonSubBusinessProject))
                    chBoxIncludSubProject.Checked = true;
            }
            else if (PersonListOption.Modes.PersonNullBusinessProject ==
                     (o.Mode &
                      PersonListOption.Modes.PersonNullBusinessProject))
            {
                chBoxBusinessProjectEnable.Checked = true;
            }
            ChangeStateControl(chBoxBusinessProjectEnable);

            foreach (string key in o.GetValues(false))
            {
                pb.AddPerson(int.Parse(key), o.GetItemText(key));
            }
        }

        protected override void FillElement()
        {
            base.FillElement();

            if (option == null) return;
            var o =
                (PersonListOption) option;

            o.Mode = rbAND.Checked
                         ? PersonListOption.Modes.AND
                         : PersonListOption.Modes.OR;

            #region типы

            if (chBoxOrganizationsEnable.Checked)
            {
                if (chBoxOrganization.Checked)
                {
                    o.AddAttribute("PersonOrganization", "true");
                }
                else
                {
                    o.RemoveAttribute("PersonOrganization");
                }
                if (chBoxPhysicalPersons.Checked)
                {
                    o.AddAttribute("PersonPhysical", "true");
                }
                else
                {
                    o.RemoveAttribute("PersonPhysical");
                }
                if (chBoxBank.Checked)
                {
                    o.AddAttribute("PersonBank", "true");
                }
                else
                {
                    o.RemoveAttribute("PersonBank");
                }
            }
            else
            {
                o.RemoveAttribute("PersonOrganization");
                o.RemoveAttribute("PersonPhysical");
                o.RemoveAttribute("PersonBank");
            }

            #endregion

            if (chBoxPersonsEnable.Checked)
            {
                if (chBoxPersonsCheck.Checked)
                    o.AddAttribute("PersonCheck", "true");
                else
                    o.RemoveAttribute("PersonCheck");
                if (chBoxPersonsUncheck.Checked)
                    o.AddAttribute("PersonUncheck", "true");
                else
                    o.RemoveAttribute("PersonUncheck");
            }
            else
            {
                o.RemoveAttribute("PersonCheck");
                o.RemoveAttribute("PersonUncheck");
            }

            #region страны

            if (chBoxCountryRegistrationEnable.Checked && !string.IsNullOrEmpty(tbCountry.Text) &&
                tbCountry.Tag != null)
            {
                o.AddAttribute("PersonArea", tbCountry.Tag.ToString());
                o.AddAttribute("PersonAreaName", tbCountry.Text);
            }
            else
            {
                o.RemoveAttribute("PersonArea");
                o.RemoveAttribute("PersonAreaName");
            }

            #endregion

            #region орг-прав формы

            if (chBoxOrganizationalFormEnable.Checked && cbOrganizationalForm.SelectedItem != null)
            {
                o.AddAttribute("PersonOPForma", ((ComboBoxItem) cbOrganizationalForm.SelectedItem).ID.ToString());
                o.AddAttribute("PersonOPFormaName", ((ComboBoxItem) cbOrganizationalForm.SelectedItem).Name);
            }
            else
            {
                o.RemoveAttribute("PersonOPForma");
                o.RemoveAttribute("PersonOPFormaName");
            }

            #endregion

            #region ответственные сотрудники

            if (chBoxEmployees.Checked)
            {
                string users = "";
                bool isFirst = true;
                foreach (ComboBoxItem item in lbSelectedEmploees.Items)
                {
                    if (!isFirst)
                        users += ";";
                    users += item.ID + ";" + item.Name;
                    isFirst = false;
                }
                if (users != "")
                    o.AddAttribute("PersonUsers", users);
            }
            else
            {
                o.RemoveAttribute("PersonUsers");
            }

            #endregion

            #region Типы лиц

            if (chBoxTypesEnable.Checked)
            {
                string types = "";
                bool isFirsttypes = true;
                foreach (ComboBoxItem item in lbTypes.Items)
                {
                    if (!isFirsttypes)
                        types += ";";
                    types += item.ID + ";" + item.Name;
                    isFirsttypes = false;
                }
                if (types != "")
                {
                    o.AddAttribute("PersonThemes", types);
                    if (chBoxSubtypes.Checked)
                        o.AddAttribute("PersonSubThemes", "1");
                    else
                        o.RemoveAttribute("PersonSubThemes");
                }
                else
                {
                    o.AddAttribute("PersonThemes", "-1");
                    o.RemoveAttribute("PersonSubThemes");
                }
            }
            else
            {
                o.RemoveAttribute("PersonThemes");
                o.RemoveAttribute("PersonSubThemes");
            }

            #endregion

            #region Бизнес проекты

            if (chBoxBusinessProjectEnable.Checked)
            {
                if (chBoxAnyBusinessProject.Checked)
                {
                    o.AddAttribute("PersonAnyBusinessProject", "1");
                    o.RemoveAttribute("PersonBusinessProject");
                    o.RemoveAttribute("PersonBusinessProjectName");
                    o.RemoveAttribute("PersonSubBusinessProject");
                    o.RemoveAttribute("PersonNullBusinessProject");
                }
                else if (tbBusinessProject.Text != "" && tbBusinessProject.Tag != null)
                {
                    o.AddAttribute("PersonBusinessProject", Convert.ToInt32(tbBusinessProject.Tag).ToString());
                    o.AddAttribute("PersonBusinessProjectName", tbBusinessProject.Text);
                    if (chBoxIncludSubProject.Checked)
                        o.AddAttribute("PersonSubBusinessProject", "1");
                    else
                        o.RemoveAttribute("PersonSubBusinessProject");
                    o.RemoveAttribute("PersonNullBusinessProject");
                    o.RemoveAttribute("PersonAnyBusinessProject");
                }
                else if (tbBusinessProject.Text == "" && tbBusinessProject.Tag == null)
                {
                    o.AddAttribute("PersonNullBusinessProject", "1");
                    o.RemoveAttribute("PersonBusinessProject");
                    o.RemoveAttribute("PersonBusinessProjectName");
                    o.RemoveAttribute("PersonAnyBusinessProject");
                    o.RemoveAttribute("PersonSubBusinessProject");
                }
                else
                {
                    o.RemoveAttribute("PersonBusinessProject");
                    o.RemoveAttribute("PersonBusinessProjectName");
                    o.RemoveAttribute("PersonAnyBusinessProject");
                    o.RemoveAttribute("PersonSubBusinessProject");
                    o.RemoveAttribute("PersonNullBusinessProject");
                }
            }
            else
            {
                o.RemoveAttribute("PersonBusinessProject");
                o.RemoveAttribute("PersonBusinessProjectName");
                o.RemoveAttribute("PersonAnyBusinessProject");
                o.RemoveAttribute("PersonSubBusinessProject");
                o.RemoveAttribute("PersonNullBusinessProject");
            }

            #endregion

            o.SetValue(pb.PersonIDs.Aggregate("", (current, key) => current + ((current.Length == 0 ? "" : ",") + key.ToString())));
        }


        private void ChangeStateControl(CheckBox chBox)
        {
            bool result = chBox.Checked;
            switch (chBox.Name)
            {
                case "chBoxOrganizationsEnable":
                    chBoxBank.Enabled = chBoxPhysicalPersons.Enabled = chBoxOrganization.Enabled = result;
                    break;
                case "chBoxPersonsEnable":
                    chBoxPersonsCheck.Enabled = chBoxPersonsUncheck.Enabled = result;
                    break;
                case "chBoxCountryRegistrationEnable":
                    tbCountry.Enabled = bnSelectCountry.Enabled = result;
                    break;
                case "chBoxOrganizationalFormEnable":
                    cbOrganizationalForm.Enabled = result;
                    break;
                case "chBoxEmployees":
                    bnSelectEmployee.Enabled = result;
                    lbSelectedEmploees.Enabled = result;
                    lbSelectedEmploees.BackColor = lbSelectedEmploees.Enabled
                                                       ? SystemColors.Window
                                                       : SystemColors.Control;
                    break;
                case "chBoxTypesEnable":
                    chBoxSubtypes.Enabled = result;
                    lbTypes.Enabled = result;
                    bnSelectTypes.Enabled = result;
                    lbTypes.BackColor = lbTypes.Enabled ? SystemColors.Window : SystemColors.Control;
                    break;
                case "chBoxBusinessProjectEnable":
                    chBoxIncludSubProject.Enabled = result;
                    chBoxAnyBusinessProject.Enabled = result;
                    tbBusinessProject.Enabled = result;
                    bnSelectBusinessProject.Enabled = result;
                    if (chBoxAnyBusinessProject.Enabled && chBoxAnyBusinessProject.Checked)
                    {
                        chBoxIncludSubProject.Enabled = false;
                        tbBusinessProject.Enabled = false;
                        bnSelectBusinessProject.Enabled = false;
                    }
                    break;
            }
            if (chBoxBusinessProjectEnable.Checked || chBoxOrganizationsEnable.Checked || chBoxPersonsEnable.Checked ||
                chBoxCountryRegistrationEnable.Checked ||
                chBoxOrganizationalFormEnable.Checked || chBoxEmployees.Checked || chBoxTypesEnable.Checked)
            {
                rbAND.Enabled = false;
                rbOR.Enabled = false;
                pb.Enabled = false;
            }
            else
            {
                rbAND.Enabled = true;
                rbOR.Enabled = true;
                pb.Enabled = true;
            }
        }

        private void chBoxEnable_CheckedChanged(object sender, EventArgs e)
        {
            var chBox = (CheckBox) sender;
            ChangeStateControl(chBox);
        }

        private void bnSelectCountry_Click(object sender, EventArgs e)
        {
            AreasList dialog = !string.IsNullOrEmpty(tbCountry.Text) ? new AreasList(tbCountry.Text) : new AreasList();
            dialog.DialogEvent += areasList_DialogEvent;
            ShowSubForm(dialog);
        }

        private void areasList_DialogEvent(object source, DialogEventArgs e)
        {
            if (!Enabled)
                Enabled = true;
            Focus();
            if (e.Dialog.DialogResult == DialogResult.OK)
            {
                var dialog = e.Dialog as AreasList;
                if (dialog != null && dialog.ID >= 0)
                {
                    tbCountry.Text = dialog.Area;
                    tbCountry.Tag = dialog.ID;
                }
            }
        }

        private bool isShowUserDialog;

        private void bnSelectEmployee_Click(object sender, EventArgs e)
        {
            string selectedid = "";
            int count = 0;
            foreach (ComboBoxItem item in lbSelectedEmploees.Items)
            {
                count++;
                selectedid += item.ID.ToString() + ((count != lbSelectedEmploees.Items.Count) ? "," : "");
            }
            var dialog =
                new UserDialog(Environment.EmployeeSearchString,
                                           "search=" +
                                           HttpUtility.UrlEncode("").Replace("+", "%20") + "&" +
                                           "clid=3&UserAccountDisabled=0&return=2" +
                                           ((selectedid == "") ? "" : ("&selectedid=" + selectedid)));

            dialog.DialogEvent += UserDialog_DialogEvent;
            dialog.Show();

            isShowUserDialog = true;
            FindForm().Enabled = false;
        }

        private void UserDialog_DialogEvent(object source, DialogEventArgs e)
        {
            if (isShowUserDialog)
            {
                Form form = FindForm();
                if (form != null)
                {
                    form.Enabled = true;
                    form.Focus();

                    var dialog = e.Dialog as UserDialog;
                    if (dialog != null && dialog.DialogResult == DialogResult.OK && dialog.Users != null)
                    {
                        lbSelectedEmploees.Items.Clear();
                        foreach (
                            var item in
                                from UserInfo newUser in dialog.Users select new ComboBoxItem(newUser.ID, newUser.Name))
                            lbSelectedEmploees.Items.Add(item);
                    }
                }
            }
            isShowUserDialog = false;
        }

        private void bnSelectTypes_Click(object sender, EventArgs e)
        {
            TypesList dialog = null;
            if (lbTypes.Items.Count > 0)
            {
                var list = new ArrayList();
                foreach (ComboBoxItem item in lbTypes.Items)
                    list.Add(item.ID);

                dialog = new TypesList(list);
            }
            else
                dialog = new TypesList();
            dialog.DialogEvent += SelectTypes_DialogEvent;
            ShowSubForm(dialog);
        }

        private void SelectTypes_DialogEvent(object source, DialogEventArgs e)
        {
            if (!Enabled)
                Enabled = true;
            Focus();

            if (e.Dialog.DialogResult != DialogResult.OK) 
                return;

            lbTypes.Items.Clear();
            var dialog = e.Dialog as TypesList;
            if (dialog != null && dialog.IdList.Count > 0)
            {
                foreach (
                    var item in
                        from TypesList.TypesInfo type in dialog.IdList select new ComboBoxItem(type.ID, type.Name))
                    lbTypes.Items.Add(item);
            }
        }

        private void EnablePersonalBlock()
        {
            pb.Enabled = true;
            chBoxOrganizationsEnable.Checked = false;
            ChangeStateControl(chBoxOrganizationsEnable);
            chBoxPersonsEnable.Checked = false;
            ChangeStateControl(chBoxPersonsEnable);
            chBoxCountryRegistrationEnable.Checked = false;
            ChangeStateControl(chBoxCountryRegistrationEnable);
            chBoxOrganizationalFormEnable.Checked = false;
            ChangeStateControl(chBoxOrganizationalFormEnable);
            chBoxEmployees.Checked = false;
            ChangeStateControl(chBoxEmployees);
            chBoxTypesEnable.Checked = false;
            ChangeStateControl(chBoxTypesEnable);
            chBoxBusinessProjectEnable.Checked = false;
            ChangeStateControl(chBoxBusinessProjectEnable);
        }

        private void PersonListNew_MouseUp(object sender, MouseEventArgs e)
        {
            if (pb.Enabled)
                return;
            try
            {
                Rectangle rect = Rectangle.Empty;
                foreach (TextBox control in pb.Controls.OfType<TextBox>())
                {
                    rect = control.Bounds;
                    break;
                }
                if (rect == Rectangle.Empty)
                    return;
                if (e.X >= pb.Bounds.X + rect.X && e.X <= pb.Bounds.X + rect.Right
                    && e.Y >= pb.Bounds.Y + rect.Y && e.Y <= pb.Bounds.Y + rect.Bottom)
                    EnablePersonalBlock();
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        /// <summary>
        ///   Выбор бизнес проекта
        /// </summary>
        private void bnSelectBusinessProject_Click(object sender, EventArgs e)
        {
            BusinessProjectList dialog = null;

            dialog = new BusinessProjectList();
            dialog.DialogEvent += BusinessProject_DialogEvent;
            ShowSubForm(dialog);
        }

        private void BusinessProject_DialogEvent(object source, DialogEventArgs e)
        {
            if (!Enabled)
                Enabled = true;
            Focus();

            if (e.Dialog.DialogResult == DialogResult.OK)
            {
                var dialog = e.Dialog as BusinessProjectList;
                if (dialog != null && dialog.BusinessProjectRow != null)
                {
                    tbBusinessProject.Text = dialog.BusinessProjectRow["БизнесПроект"].ToString();
                    tbBusinessProject.Tag = dialog.BusinessProjectRow["КодБизнесПроекта"];
                }
            }
        }

        private void chBoxAnyBusinessProject_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxAnyBusinessProject.Checked)
            {
                chBoxIncludSubProject.Enabled = false;
                tbBusinessProject.Enabled = false;
                bnSelectBusinessProject.Enabled = false;
            }
            else
            {
                chBoxIncludSubProject.Enabled = true;
                bnSelectBusinessProject.Enabled = true;
                tbBusinessProject.Enabled = true;
            }
        }

        private void tbBusinessProject_TextChanged(object sender, EventArgs e)
        {
            if (tbBusinessProject.Text == "")
                tbBusinessProject.Tag = null;
        }
    }
}