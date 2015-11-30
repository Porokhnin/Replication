using System;
using Replication.Abstraction;

namespace Replication.Library.EventArguments
{
    /// <summary>
    /// Аргументы для события репликации
    /// </summary>
    public class ReplicationEventArgs:EventArgs
    {
        /// <summary>
        /// Реплицируемый объект
        /// </summary>
        public IReplicationObject ReplicationObject { get; private set; }

        /// <summary>
        /// Тип операции репликации
        /// </summary>
        public OperationType OperationType { get; private set; }

        /// <summary>
        /// Аргументы для события репликации
        /// </summary>
        /// <param name="replicationObject">Реплицируемый объект</param>
        /// <param name="operationType">Тип операции репликации</param>
        public ReplicationEventArgs(IReplicationObject replicationObject, OperationType operationType)
        {
            ReplicationObject = replicationObject;
            OperationType = operationType;
        }
    }
}
