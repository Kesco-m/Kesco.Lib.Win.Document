using System.Drawing;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Dialogs
{
    public partial class CheckDateDialog : Form
    {
        public CheckDateDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Задать начальную позицию диалога относительно центра родительской формы
        /// </summary>
        /// <param name="parent">Экземпляр родительской формы</param>
        /// <param name="offsetX">Смещение окна по координате X</param>
        /// <param name="offsetY">Смещение окна по координате Y</param>
        public void SetStartPosition(Control parent, int offsetX, int offsetY)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(
                (parent.Location.X + parent.Width / 2) - (this.Width / 2) + offsetX,
                (parent.Location.Y + parent.Height / 2) - (this.Height / 2) + offsetY); 
        }
    }
}