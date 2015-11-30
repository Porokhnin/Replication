using System.Collections.Generic;
using System.ServiceModel;
using Replication.Abstraction;

namespace Replication.Core.Contract
{
    /// <summary>
    /// Сервис репликации объектов
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IReplicationCallbackContract))]
    public interface IReplicationService
    {
        /// <summary>
        /// Соединиться с сервером
        /// </summary>
        /// <returns>Коллекция реплицируемых объектов</returns>
        [OperationContract]
        List<ReplicationInfo> Connect();

        /// <summary>
        /// Разорвать соединение с сервером
        /// </summary>
        [OperationContract]
        void Disconnect();

        /// <summary>
        /// Реплицировать объект
        /// </summary>
        /// <param name="replicationInfo"> Информация о реплицируемом объекте</param>
        /// <param name="operationType">Тип операции</param>
        [OperationContract]
        void SetObject(ReplicationInfo replicationInfo, OperationType operationType);
    }
}
