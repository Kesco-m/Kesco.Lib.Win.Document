using System;
using System.Threading;

namespace Kesco.Lib.Win.Document.Extentions
{
    /// <summary>
    /// Расширение System.Windows.Forms.Control
    /// </summary>
    public static class ControlExtention
    {
        /// <summary>
        /// Маршлинг произвольного потока в основной UI поток. Синхронно.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="methodDelegate"></param>
        public static void UiThreadInvoke(this System.Windows.Forms.Control control, Action methodDelegate)
        {
            if (control == null || !control.IsHandleCreated)
                return;

            if (control.IsDisposed || control.Disposing)
                return;

            if (Environment.UIThreadSynchronizationContext != null)
            {
                Action task = () => Environment.UIThreadSynchronizationContext.Send(state => methodDelegate(), null);
                var result = task.BeginInvoke(null, null);
                result.AsyncWaitHandle.WaitOne();
            }
            else
                control.Invoke(methodDelegate);
        }

        /// <summary>
        /// Маршлинг произвольного потока в основной UI поток. Асинхронно.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="methodDelegate"></param>
        public static void UiThreadBeginInvoke(this System.Windows.Forms.Control control, Action methodDelegate)
        {
            if (control == null || !control.IsHandleCreated)
                return;

            if (control.IsDisposed || control.Disposing)
                return;

            if (Environment.UIThreadSynchronizationContext != null)
                ThreadPool.QueueUserWorkItem(state1 => Environment.UIThreadSynchronizationContext.Post(state2 => methodDelegate(), null));
            else
                control.BeginInvoke(methodDelegate);
        }
    }
}
