using System;
using System.Collections.Generic;
using System.Linq;
using Replication.Abstraction;
using Replication.Core.Contract;

namespace Replication.Core
{
    /// <summary>
    /// Подсистема объектов репликации
    /// </summary>
    public class ReplicatedObjectsSubsystem
    {
        /// <summary>
        /// Объекты репликации
        /// </summary>
        private readonly List<ReplicationInfo> _replicationObjects;

        /// <summary>
        /// Подсистема объектов репликации
        /// </summary>
        public ReplicatedObjectsSubsystem()
        {
            _replicationObjects = new List<ReplicationInfo>();
        }

        /// <summary>
        /// Получить объекты репликации
        /// </summary>
        public List<ReplicationInfo> GetReplicatedObjects()
        {
            lock (_replicationObjects)
            {
                return _replicationObjects;
            }
        }

        /// <summary>
        /// Подготовить объект для репликации
        /// </summary>
        /// <param name="replicationInfo">Информация о реплицируемом объекте</param>
        /// <param name="operationType">Тип операции</param>
        public void PrepareReplicationObject(ReplicationInfo replicationInfo, OperationType operationType)
        {
            switch (operationType)
            {
                case OperationType.Create:
                    CreateReplicationObject(replicationInfo);
                    break;
                case OperationType.Update:
                    UpdateReplicationObject(replicationInfo);
                    break;
                case OperationType.Delete:
                    DeleteReplicationObject(replicationInfo);
                    break;
            }
        }

        /// <summary>
        /// Создать объект репликации
        /// </summary>
        /// <param name="replicationInfo">Информация о реплицируемом объекте</param>
        public void CreateReplicationObject(ReplicationInfo replicationInfo)
        {
            lock (_replicationObjects)
            {
                _replicationObjects.Add(replicationInfo);
            }
        }

        /// <summary>
        /// Обновить объект репликации
        /// </summary>
        /// <param name="replicationInfo">Информация о реплицируемом объекте</param>
        public void UpdateReplicationObject(ReplicationInfo replicationInfo)
        {
            lock (_replicationObjects)
            {
                var existedReplicationClass = _replicationObjects.FirstOrDefault(r => String.Compare(r.Uid.ToString(), replicationInfo.Uid.ToString(), StringComparison.OrdinalIgnoreCase) == 0);
                if (existedReplicationClass != null)
                {
                    existedReplicationClass.UpdateProperties(replicationInfo.Properties);
                }
            }
        }

        /// <summary>
        /// Удалить объект репликации
        /// </summary>
        /// <param name="replicationInfo">Информация о реплицируемом объекте</param>
        public void DeleteReplicationObject(ReplicationInfo replicationInfo)
        {
            lock (_replicationObjects)
            {
                _replicationObjects.RemoveAll(r => String.Compare(r.Uid.ToString(), replicationInfo.Uid.ToString(), StringComparison.OrdinalIgnoreCase) == 0);
            }
        }
    }
}
