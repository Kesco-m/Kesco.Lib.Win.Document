using System;
using System.Drawing;
using System.Windows.Forms;
using Kesco.Lib.Win.Web;

namespace Kesco.Lib.Win.Document.Controls
{
    public class HoverLinkLabel : LinkLabel
    {
        private Color _linkHoverColor;
        private Form _topLevelForm;

        public string Url { get; set; }
        public string Caption { get; set; }

        public HoverLinkLabel(Form topLevelForm)
        {
            DoubleBuffered = true;
            Caption = string.Empty;
            LinkBehavior = LinkBehavior.HoverUnderline;

            _topLevelForm = topLevelForm;

            _linkHoverColor = LinkColor;
            LinkColor = Color.Black;
        }

        protected override void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
        {
            base.OnLinkClicked(e);

            if (string.IsNullOrEmpty(Url))
                return;
            var udb = new UrlBrowseDialog(Url, Caption)
                          {
                              StartPosition = FormStartPosition.CenterScreen,
                              Width = 612,
                              Height = 430,
                              MaximizeBox = false,
                              MinimizeBox = false,
                              Owner = _topLevelForm
                          };
            udb.Show(_topLevelForm);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            LinkColor = _linkHoverColor;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            LinkColor = Color.Black;
            base.OnMouseLeave(e);
        }
    }
}