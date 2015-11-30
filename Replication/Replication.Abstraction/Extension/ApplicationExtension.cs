using System;
using System.Windows;

namespace Replication.Abstraction.Extension
{
    /// <summary>
    /// Расширения класса приложения
    /// </summary>
    public static class ApplicationExtension
    {
        /// <summary>
        /// Безопасный синхронный вызов
        /// </summary>
        /// <param name="current">Экземпляр приложения</param>
        /// <param name="callback">Колбек</param>
        public static void InvokeSafe(this Application current, Action callback)
        {
            if (current != null && current.Dispatcher != null)
            {
                if (!current.Dispatcher.HasShutdownStarted)
                {
                    current.Dispatcher.Invoke(callback);
                }
            }
        }
        /// <summary>
        /// Безопасный асинхронный вызов
        /// </summary>
        /// <param name="current">Экземпляр приложения</param>
        /// <param name="callback">Колбек</param>
        public static void InvokeAsyncSafe(this Application current, Action callback)
        {
            if (current != null && current.Dispatcher != null)
            {
                if (!current.Dispatcher.HasShutdownStarted)
                {
                    current.Dispatcher.InvokeAsync(callback);
                }
            }
        }
    }
}
