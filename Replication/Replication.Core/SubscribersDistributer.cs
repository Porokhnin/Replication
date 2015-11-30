using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Replication.Abstraction;
using Replication.Core.Contract;

namespace Replication.Core
{
    /// <summary>
    /// Класс рассылки уведомлений подписчикам
    /// </summary>
    public class SubscribersDistributer
    {
        /// <summary>
        /// Подписчики
        /// </summary>
        private readonly List<IReplicationCallbackContract> _subscribers;

        /// <summary>
        /// Класс рассылки уведомлений подписчикам
        /// </summary>
        public SubscribersDistributer()
        {
            _subscribers = new List<IReplicationCallbackContract>();
        }

        /// <summary>
        /// Добавить подписчика
        /// </summary>
        /// <param name="subscriber">Подписчик</param>
        public void AddSubscriber(IReplicationCallbackContract subscriber)
        {
            lock (_subscribers)
            {
                _subscribers.Add(subscriber);
            }
        }

        /// <summary>
        /// Удалить подписчика
        /// </summary>
        /// <param name="subscriber">Подписчик</param>
        public void RemoveSubscriber(IReplicationCallbackContract subscriber)
        {
            lock (_subscribers)
            {
                _subscribers.RemoveAll(s => s == subscriber);
            }
        }

        /// <summary>
        /// Отправить уведомление об изменении состоянии объекта
        /// </summary>
        /// <param name="replicationInfo">Информация о реплицируемом объекте</param>
        /// <param name="operationType">Тип операции репликации</param>
        /// <param name="sender">Отправитель</param>
        public void SendObjectChanged(ReplicationInfo replicationInfo, OperationType operationType, IReplicationCallbackContract sender)
        {
            IReplicationCallbackContract[] subscribersCopy;

            lock (_subscribers)
            {
                subscribersCopy = _subscribers.Where(s=> s!=sender).ToArray();
            }

            foreach (var subscriber in subscribersCopy)
            {
                if (((ICommunicationObject)subscriber).State == CommunicationState.Opened)
                {
                    try
                    {
                        subscriber.SimulateObjectChanged(replicationInfo, operationType);
                    }
                    catch (Exception ex)
                    {
                        //TODO сделать лог
                    }
                }
                else
                {
                    RemoveSubscriber(subscriber);
                }
            }
        }
    }
}
