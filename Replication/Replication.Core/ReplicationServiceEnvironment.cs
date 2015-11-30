namespace Replication.Core
{
    /// <summary>
    /// Окружение сервиса репликации
    /// </summary>
    public class ReplicationServiceEnvironment
    {
        /// <summary>
        /// Экземпляр окружения
        /// </summary>
        public static ReplicationServiceEnvironment Current { get; set; }
        /// <summary>
        /// Класс рассылки уведомлений подписчикам
        /// </summary>
        public SubscribersDistributer SubscribersDistributer { get; set; }
        /// <summary>
        /// Подсистема объектов репликации
        /// </summary>
        public ReplicatedObjectsSubsystem ReplicatedObjectsSubsystem { get; set; }
    }
}
