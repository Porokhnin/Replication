using System.ServiceModel;
using Replication.Abstraction;

namespace Replication.Core.Contract
{
    public interface IReplicationCallbackContract
    {
        /// <summary>
        /// Уведомить об изменении объекта
        /// </summary>
        /// <param name="replicationInfo">Информация о реплицируемом объекте</param>
        /// <param name="operationType">Тип операции репликации</param>
        [OperationContract(IsOneWay = true)]
        void SimulateObjectChanged(ReplicationInfo replicationInfo, OperationType operationType);
    }
}
