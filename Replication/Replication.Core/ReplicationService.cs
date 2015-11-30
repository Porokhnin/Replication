using System.Collections.Generic;
using System.ServiceModel;
using Replication.Abstraction;
using Replication.Core.Contract;

namespace Replication.Core
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class ReplicationService : IReplicationService
    {
        public SubscribersDistributer SubscribersDistributer { get; set; }
        public ReplicatedObjectsSubsystem ReplicatedObjectsSubsystem { get; set; }

        public ReplicationService(): this(ReplicationServiceEnvironment.Current, new List<ReplicationInfo>())
        {

        }
        public ReplicationService(ReplicationServiceEnvironment environment, List<ReplicationInfo> replicationObjects)
        {
            SubscribersDistributer = environment.SubscribersDistributer;
            ReplicatedObjectsSubsystem = environment.ReplicatedObjectsSubsystem;
        }

        public List<ReplicationInfo> Connect()
        {
            SubscribersDistributer.AddSubscriber(GetSubscriber());

            return ReplicatedObjectsSubsystem.GetReplicatedObjects();
        }
        
        public void Disconnect()
        {
            SubscribersDistributer.RemoveSubscriber(GetSubscriber());
        }

        public void SetObject(ReplicationInfo replicationInfo, OperationType operationType)
        {
            ReplicatedObjectsSubsystem.PrepareReplicationObject(replicationInfo, operationType);
            SubscribersDistributer.SendObjectChanged(replicationInfo, operationType, GetSubscriber());
        }        
        
        private IReplicationCallbackContract GetSubscriber()
        {
            return OperationContext.Current.GetCallbackChannel<IReplicationCallbackContract>();
        }
    }
}
