using System.ServiceProcess;

namespace Replication.Core.Host
{
    partial class ReplicationService : ServiceBase
    {
        private ReplicationServiceCore _serviceCore;
        
        public ReplicationService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _serviceCore = new ReplicationServiceCore();
            _serviceCore.Start();
        }

        protected override void OnStop()
        {
            _serviceCore.Stop();
        }
    }

}
