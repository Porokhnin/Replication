using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Replication.Library.Settings
{
    /// <summary>
    /// Класс чтения параметров 
    /// </summary>
    internal class ReplicationSettingsReader
    {
        /// <summary>
        /// Прочитать параметры
        /// </summary>
        /// <returns>Параметры библиотеки Replication</returns>
        public static ReplicationSettings ReadSettings()
        {
            ReplicationSettings result = new ReplicationSettings();

            try
            {
                var updaterSettings = ConfigurationManager.GetSection("ReplicationService.Setting") as NameValueCollection;
                if (updaterSettings != null)
                {
                    result.ServiceAddress = updaterSettings["ServiceAddress"];
                }
            }
            catch (Exception) { }

            return result;
        }
    }
}
