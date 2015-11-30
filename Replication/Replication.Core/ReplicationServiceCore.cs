using System;
using System.Diagnostics;
using System.ServiceModel;

namespace Replication.Core
{
    /// <summary>
    /// Ядро сервиса репликации
    /// </summary>
    public class ReplicationServiceCore
    {
        private ServiceHost _replicationServiceHost;

        public void Start()
        {
            ReplicationServiceEnvironment environment = new ReplicationServiceEnvironment()
            {
                SubscribersDistributer = new SubscribersDistributer(),
                ReplicatedObjectsSubsystem = new ReplicatedObjectsSubsystem()
            };
            
            ReplicationServiceEnvironment.Current = environment;


            _replicationServiceHost = new ServiceHost(typeof(ReplicationService));
            _replicationServiceHost.Faulted += OnUpdateServiceHostFaulted;

            try
            {
                _replicationServiceHost.Open();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message, EventLogEntryType.Information.ToString());
            }
        }

        public void Stop()
        {
            CloseService();
        }

        private void CloseService()
        {
            if (_replicationServiceHost.State != CommunicationState.Faulted)
                _replicationServiceHost.Close(); 
        }

        private void OnUpdateServiceHostFaulted(object sender, EventArgs e)
        {
            CloseService();
        }
    }
}
