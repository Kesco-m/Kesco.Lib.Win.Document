using System;
using System.Threading;

namespace Kesco.Lib.Win.Document.Extentions
{
    /// <summary>
    /// Расширение объекта. Маршалинг в UI поток
    /// </summary>
    public static class ObjectExtention
    {
        /// <summary>
        /// Маршлинг произвольного потока в основной UI поток. Синхронно.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodDelegate"></param>
        public static void UiThreadInvoke(this object obj, Action methodDelegate)
        {
            if (obj == null)
                return;

            if (Environment.UIThreadSynchronizationContext != null)
            {
                Action task = () => Environment.UIThreadSynchronizationContext.Send(state => methodDelegate(), null);
                var result = task.BeginInvoke(null, null);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        /// <summary>
        /// Маршлинг произвольного потока в основной UI поток. Асинхронно.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodDelegate"></param>
        public static void UiThreadBeginInvoke(this object obj, Action methodDelegate)
        {
            if (obj == null)
                return;

            if (Environment.UIThreadSynchronizationContext != null)
                ThreadPool.QueueUserWorkItem(state1 => Environment.UIThreadSynchronizationContext.Post(state2 => methodDelegate(), null));
        }
    }
}
