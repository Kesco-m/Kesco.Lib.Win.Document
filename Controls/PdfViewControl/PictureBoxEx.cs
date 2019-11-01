using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Controls.PdfViewControl
{
    public class PictureBoxEx : Panel
    {
        public PictureBoxEx()
        {
            DoubleBuffered = true;

            _picture.Left = 0;
            _picture.SizeMode = PictureBoxSizeMode.Normal;
            _picture.Top = 0;

            Controls.Add(_picture);
        }

        #region Объявление структур, переменных

        private bool _auto_scroll = true;
        private PictureBox _picture = new PictureBox();

        #endregion

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoScroll
        {
            get { return _auto_scroll; }
            set { _auto_scroll = value; }
        }

        public Image Image
        {
            get { return _picture.Image; }
            set
            {
                _picture.Image = value;
                if (_picture.Image != null)
                {
                    _picture.Size = _picture.Image.Size;
                }
            }
        }
    }
}