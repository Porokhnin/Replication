using System;
using System.ServiceProcess;

namespace Replication.Core.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (Environment.UserInteractive)
                {
                    RunAsConsoleHost();
                }
                else
                {
                    RunAsService();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка запуска сервера обновлений {0}", ex.Message);
            }
        }

        #region Режим сервиса Windows

        static void RunAsService()
        {
            ServiceBase[] servicesToRun = new ServiceBase[] { new ReplicationService()};

            Console.WriteLine("Запуск сервиса");

            ServiceBase.Run(servicesToRun);
        }

        #endregion

        #region Консольный режим

        static void RunAsConsoleHost()
        {
            ReplicationServiceCore serviceCore = new ReplicationServiceCore();
            Console.WriteLine("Запуск сервера репликации");
            bool isStarted = false;

            try
            {
                serviceCore.Start();
                isStarted = true;
            }
            catch (Exception ex)
            {

            }

            if (isStarted)
            {
                Console.WriteLine("Нажмите Enter для остановки сервера репликации");
                Console.ReadLine();
            }

            try
            {
                serviceCore.Stop();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}
