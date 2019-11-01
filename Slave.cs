using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Kesco.Lib.Log;

namespace Kesco.Lib.Win.Document
{
    /// <summary>
    /// Для запуска фоновых задач, чтобы избежать подвисаний Архива Документов и не блокировать диалоги или основное окно
    /// </summary>
    public class Slave
    {
        #region Вызовы_метода_DoWork

        /// <summary>
        ///   Запустить фоновую задачу удаления файла, чтобы избежать подвисания из-за задержек файловой системы.
        /// </summary>
        /// <param name="fileName"> Имя файла. Все необходимые проверки пройдут внутри метода. </param>
        public static void DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return;

            DoWork("DeleteFile", new object[] {fileName}, null);
        }

        /// <summary>
        ///   Запустить фоновую задачу
        /// </summary>
        /// <param name="kindOfWork"> Тип или имя задачи </param>
        /// <param name="onComplete"> Когда выполнение операции завершено, отменено или вызвало исключение </param>
        public static void DoWork(string kindOfWork, RunWorkerCompletedEventHandler onComplete)
        {
            DoWork(kindOfWork, null, onComplete);
        }

        #endregion

        /// <summary>
        ///   Запустить фоновую задачу
        /// </summary>
        /// <param name="kindOfWork"> Тип или имя задачи </param>
        /// <param name="args"> Параметр для использования фоновой задачей </param>
        /// <param name="onComplete"> Когда выполнение операции завершено, отменено или вызвало исключение </param>
        public static void DoWork(string kindOfWork, object[] args = null, RunWorkerCompletedEventHandler onComplete= null)
        {
            var slave = new BackgroundWorker();
            if (UnderstandWork(slave, kindOfWork))
            {
                if (onComplete != null)
                    slave.RunWorkerCompleted += onComplete;

                slave.RunWorkerCompleted += slave_Dispose;
                slave.RunWorkerAsync(args);
            }
            else
            {
                Logger.WriteEx(new Exception("Slave didn't understand the task '" + kindOfWork + "'"));
                slave.Dispose();
            }
        }

        /// <summary>
        ///   Запустить фоновую задачу
        /// </summary>
        /// <param name="work"> Задача </param>
        /// <param name="args"> Параметр для использования фоновой задачей </param>
        public static void DoWork(DoWorkEventHandler work, object[] args)
        {
            var slave = new BackgroundWorker();
            slave.DoWork += work;
            slave.RunWorkerCompleted += slave_Dispose;
            slave.RunWorkerAsync(args);
        }

        /// <summary>
        ///   Запустить фоновую задачу
        /// </summary>
        /// <param name="work"> Задача </param>
        /// <param name="args"> Параметр для использования фоновой задачей </param>
        /// <param name="onComplete"> Когда выполнение операции завершено, отменено или вызвало исключение </param>
        public static void DoWork(DoWorkEventHandler work, object[] args, RunWorkerCompletedEventHandler onComplete)
        {
            var slave = new BackgroundWorker();
            slave.DoWork += work;
            slave.RunWorkerCompleted += onComplete;
            slave.RunWorkerCompleted += slave_Dispose;
            slave.RunWorkerAsync(args);
        }

        protected static bool UnderstandWork(BackgroundWorker slave, string kindOfWork)
        {
            switch (kindOfWork)
            {
                case "DeleteFile":
                    slave.DoWork += DeleteFileWork;
                    break;
                case "BuildFriendlyDocList":
                    slave.DoWork += BuildFriendlyDocList;
                    break;

                default:
                    return false;
            }
            return true;
        }

        private static void slave_Dispose(object sender, RunWorkerCompletedEventArgs e)
        {
            if (sender is BackgroundWorker && !((BackgroundWorker) sender).IsBusy)
            {
                ((BackgroundWorker) sender).Dispose();
            }
            else
            {
                Logger.WriteEx(new Exception("Slave was busy or bad worker and wasn't disposed properly"));
            }
        }

        private static void DeleteFileWork(object sender, DoWorkEventArgs e)
        {
            var fileName = ((object[]) e.Argument)[0] as string;

            if (string.IsNullOrEmpty(fileName))
                return;

            while (File.Exists(fileName))
                try
                {
                    File.Delete(fileName);
                }
                catch (UnauthorizedAccessException)
                {
                    Application.DoEvents();
                }
                catch (IOException)
                {
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    Data.Env.WriteToLog(ex);
                    return;
                }
        }

        private static void BuildFriendlyDocList(object sender, DoWorkEventArgs e)
        {
            var docIDs = ((object[]) e.Argument)[0] as int[];

            var sb = new StringBuilder();
            foreach (int docID in docIDs)
                sb.Append(DBDocString.Format(docID) + "\n");

            e.Result = sb.ToString();
        }
    }
}