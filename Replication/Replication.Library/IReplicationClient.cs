using System;
using System.Threading.Tasks;
using Replication.Abstraction;
using Replication.Library.EventArguments;

namespace Replication.Library
{
    /// <summary>
    /// Клиент для репликации объектов
    /// </summary>
    public interface IReplicationClient
    {
        /// <summary>
        /// Инициализация
        /// </summary>
        /// <returns></returns>
        Task<Boolean> Initialize();

        /// <summary>
        /// Деинициализация
        /// </summary>
        Task<Boolean> Deinitialize();

        /// <summary>
        /// Установить объект для репликации
        /// </summary>
        Boolean SetObject(IReplicationObject replicationObject);

        /// <summary>
        /// Снять объект с репликации
        /// </summary>
        Boolean FreeObject(IReplicationObject replicationObject);

        /// <summary>
        /// Объект реплицирован
        /// </summary>
        event EventHandler<ReplicationEventArgs> ObjectReplicated;
    }
}
