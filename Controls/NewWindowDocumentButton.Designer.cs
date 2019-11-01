namespace Kesco.Lib.Win.Document.Controls
{
    partial class NewWindowDocumentButton
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewWindowDocumentButton));
            this.toolbarImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // toolbarImageList
            // 
            this.toolbarImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolbarImageList.ImageStream")));
            this.toolbarImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.toolbarImageList.Images.SetKeyName(0, "");
            // 
            // NewWindowDocumentButton
            // 
            resources.ApplyResources(this, "$this");
            this.ImageList = this.toolbarImageList;
            this.TabStop = false;
            this.UseMnemonic = false;
            this.UseVisualStyleBackColor = true;
            this.Click += new System.EventHandler(this.NewWindowDocumentButton_Click);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList toolbarImageList;
    }
}
