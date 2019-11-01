using System.Windows.Forms;

namespace Kesco.Lib.Win.Document.Controls.PdfViewControl
{
    public partial class InputBox : Form
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public static DialogResult Show(string promptText, string caption, ref string value)
        {
            DialogResult dialogResult;
            using (var inputBox = new InputBox {Text = caption, PromptText = {Text = promptText}})
            {
                dialogResult = inputBox.ShowDialog();
                if (dialogResult == DialogResult.OK)
                    value = inputBox.Input.Text;
                inputBox.Dispose();
            }
            return dialogResult;
        }
    }
}