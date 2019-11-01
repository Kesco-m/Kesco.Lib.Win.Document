using System;
using System.Collections;
using System.Windows.Forms;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Document.Search.Dialogs
{
	public class TemplateList : Base
	{
		public event EventHandler SelectedIndexChanged;

		private void OnSelectedIndexChanged()
		{
			if(SelectedIndexChanged != null)
				SelectedIndexChanged(this, EventArgs.Empty);
		}

		protected internal Panel panelList;
		private System.Windows.Forms.Panel panelListButtons;
		private System.Windows.Forms.Button bRemove;
		private System.Windows.Forms.RadioButton rbAND;
		private System.Windows.Forms.RadioButton rbOR;
		protected internal ListBox lb;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TemplateList()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateList));
			this.panelList = new System.Windows.Forms.Panel();
			this.lb = new System.Windows.Forms.ListBox();
			this.panelListButtons = new System.Windows.Forms.Panel();
			this.rbOR = new System.Windows.Forms.RadioButton();
			this.rbAND = new System.Windows.Forms.RadioButton();
			this.bRemove = new System.Windows.Forms.Button();
			this.panelList.SuspendLayout();
			this.panelListButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelList
			// 
			this.panelList.Controls.Add(this.lb);
			this.panelList.Controls.Add(this.panelListButtons);
			resources.ApplyResources(this.panelList, "panelList");
			this.panelList.Name = "panelList";
			// 
			// lb
			// 
			resources.ApplyResources(this.lb, "lb");
			this.lb.Name = "lb";
			this.lb.SelectedIndexChanged += new System.EventHandler(this.lb_SelectedIndexChanged);
			// 
			// panelListButtons
			// 
			this.panelListButtons.Controls.Add(this.rbOR);
			this.panelListButtons.Controls.Add(this.rbAND);
			this.panelListButtons.Controls.Add(this.bRemove);
			resources.ApplyResources(this.panelListButtons, "panelListButtons");
			this.panelListButtons.Name = "panelListButtons";
			// 
			// rbOR
			// 
			resources.ApplyResources(this.rbOR, "rbOR");
			this.rbOR.Name = "rbOR";
			this.rbOR.CheckedChanged += new System.EventHandler(this.rbOR_CheckedChanged);
			// 
			// rbAND
			// 
			resources.ApplyResources(this.rbAND, "rbAND");
			this.rbAND.Name = "rbAND";
			this.rbAND.CheckedChanged += new System.EventHandler(this.rbOR_CheckedChanged);
			// 
			// bRemove
			// 
			resources.ApplyResources(this.bRemove, "bRemove");
			this.bRemove.Name = "bRemove";
			this.bRemove.Click += new System.EventHandler(this.bRemove_Click);
			// 
			// TemplateList
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panelList);
			this.Name = "TemplateList";
			this.Controls.SetChildIndex(this.panelList, 0);
			this.panelList.ResumeLayout(false);
			this.panelListButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		void lb_SelectedIndexChanged(object sender, EventArgs e)
		{

		}
		#endregion

		protected override void FillElement()
		{
			base.FillElement();
			if(option == null)
				return;

			ListOption o = (ListOption)option;

			o.Mode = (rbAND.Checked == true) ? ListOption.Modes.AND : ListOption.Modes.OR;

			string s = "";
			foreach(string key in keys)
			{
				s += (s.Length == 0 ? "" : ",") + key;
			}
			o.SetValue(s);
		}

		protected override void FillForm()
		{
			base.FillForm();
			if(option == null)
				return;

			ListOption o = (ListOption)option;
			switch(o.Mode)
			{
				case ListOption.Modes.AND:
					rbAND.Checked = true;
					break;
				case ListOption.Modes.OR:
				default:
					rbOR.Checked = true;
					break;
			}
			foreach(string key in o.GetValues(false))
				keys.Add(key);
			lbRefresh();
		}

		protected ArrayList keys = new ArrayList();

		public void AddKey(string key)
		{
			if(keys.Contains(key))
				return;
			keys.Add(key);
			lbRefresh();
		}

		public virtual void RemoveKey(string key)
		{
			if(!keys.Contains(key))
				return;
			keys.Remove(key);
			lbRefresh();
		}

		public virtual void RemoveAllKeys()
		{
			keys.Clear();
			lbRefresh();
		}

		protected void lbRefresh()
		{
			ListOption o = (ListOption)option;
			lb.Items.Clear();
			string s;
			foreach(string key in keys)
			{
				s = (lb.Items.Count > 0 ? (rbAND.Checked ? Environment.StringResources.GetString("And") + " " : Environment.StringResources.GetString("Or") + " ") : "");
				lb.Items.Add(s + o.GetItemText(key));
			}
		}

		private void bRemove_Click(object sender, System.EventArgs e)
		{
			if(lb.SelectedIndex < 0 || lb.SelectedIndex >= lb.Items.Count)
				return;
			RemoveKey((string)keys[lb.SelectedIndex]);
		}

		private void rbOR_CheckedChanged(object sender, System.EventArgs e)
		{
			lbRefresh();
		}

		public string[] GetValues()
		{
			string[] S = new string[keys.Count];
			for(int i = 0; i < S.Length; i++)
				S[i] = (string)keys[i];
			return S;
		}
	}
}