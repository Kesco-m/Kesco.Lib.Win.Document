using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Controls
{
    public class OptionsSettings : UserControl
    {
        private Panel panel1;
        private Container components;

        public delegate void Command_EventHandler(Label lLab);

        public event Command_EventHandler Command;

        public OptionsSettings()
        {
            InitializeComponent();
        }

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

        public void AddSettings(string sett)
        {
            panel1.Controls.Clear();
            if (sett == string.Empty)
                return;
            int x = 0;
            int y = 0;
            string[] split = sett.TrimEnd('\n').Split(new[] {"\n", "<br>"}, int.MaxValue,
                                                      StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < split.Length; i++)
            {
                if (i != 0)
                    split[i] = split[i].Trim();

                string newText = split[i].Replace("<br>", string.Empty);
                var l = new Label {AutoSize = true, Font = new Font("Verdana", 8), Text = newText};
                panel1.Controls.Add(l);
				int startIndex = newText.IndexOf("<A href=");
				if(startIndex > 0)
				{
					l.Text = newText.Substring(0, startIndex).TrimEnd('[').TrimEnd(' ') + " [";
					x += l.Width;
				}
				while(startIndex > 0)
				{
					string id = newText.Substring(startIndex + 8, newText.IndexOf('>', startIndex) - startIndex - 8);
					string tmpStr = newText.Substring(startIndex + 8 + id.Length + 1,
													  newText.IndexOf("</A>", startIndex) - startIndex - 8 - id.Length - 1);
					bool italic = tmpStr.IndexOf("<i>") != -1;
					if(italic)
					{
						int index = 0;
						do
						{
							index = tmpStr.IndexOf("<i>", index);
							CreateLinkLabel(
								tmpStr.Substring(tmpStr.IndexOf("<i>", index) + 3,
												 tmpStr.IndexOf("</i>", index) - index - 3), id, true, ref x, y);
							index = tmpStr.IndexOf("</i>", index) + 4;
							CreateLinkLabel(tmpStr.Substring(index, ((tmpStr.IndexOf("<i>", index) != -1)
																		 ? (tmpStr.IndexOf("<i>", index) - index)
																		 : tmpStr.Length - index)), id, false, ref x,
											y);
						} while(tmpStr.IndexOf("<i>", index) != -1);
					}
					else
						CreateLinkLabel(tmpStr, id, false, ref x, y);
					int tempIndex = startIndex;
					startIndex = newText.IndexOf("<A href=", startIndex + 8);
					var l2 = new Label
								 {
									 AutoSize = true,
									 Font = new Font("Verdana", 8),
									 Text =
										 newText.Substring(newText.IndexOf("</A>", tempIndex) + 4,
														   (startIndex > 0 ? startIndex : newText.Length) - newText.IndexOf("</A>", tempIndex) - 4).
										 Trim(),
									 Location = new Point(x, y)
								 };
					panel1.Controls.Add(l2);
					x += l2.Width;
				}

                l.Location = new Point(0, y);
                y += l.Height;
                x = 0;
            }
        }

        private void CreateLinkLabel(string text, object tag, bool italic, ref int x, int y)
        {
            var lLab = new Label
                           {
                               AutoSize = true,
                               Cursor = Cursors.Hand,
                               ForeColor = Color.Blue,
                               Font = !italic ? new Font("Verdana", 8) : new Font("Verdana", 8, FontStyle.Italic),
                               Text = text,
                               Tag = tag,
                               Location = new Point(x, y)
                           };
            lLab.Click += lLab_Click;
            panel1.Controls.Add(lLab);
            x += lLab.Width;
        }

        public void OnCommand(Label lLab)
        {
            if (Command != null)
                Command(lLab);
        }

        #region Component Designer generated code

        /// <summary>
        ///   Required method for Designer support - do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 328);
            this.panel1.TabIndex = 0;
            // 
            // OptionsSettings
            // 
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "OptionsSettings";
            this.Size = new System.Drawing.Size(400, 328);
            this.ResumeLayout(false);
        }

        #endregion

        private void lLab_Click(object sender, EventArgs e)
        {
            OnCommand(sender as Label);
        }
    }
}