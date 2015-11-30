using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using Replication.Abstraction;
using Replication.Abstraction.Extension;
using Replication.Core.Contract;
using Replication.Library.EventArguments;
using Replication.Library.Settings;

namespace Replication.Library
{
    /// <summary>
    /// Клиент для репликации объектов
    /// </summary>
    public sealed class ReplicationClient<T> : IReplicationClient, IReplicationCallbackContract where T: IReplicationObject, new()
    {
        private const string UidKey = "Uid";
        private const string DefaultserviceAddress = "net.pipe://localhost/ReplicationService";
        
        /// <summary>
        /// Канал для связи с сервисом
        /// </summary>
        private readonly IReplicationService _channel;
        /// <summary>
        /// Объекты репликации
        /// </summary>
        private readonly List<IReplicationObject> _replicationObjects;
        
        /// <summary>
        /// Обработчик изменения свойств
        /// </summary>
        public PropertyChangedEventHandler PropertyChangedHandler { get; set; }

        /// <summary>
        /// Клиент для репликации объектов
        /// </summary>
        public ReplicationClient()
        {
            var settings = ReplicationSettingsReader.ReadSettings();

            String serviceAddress = String.IsNullOrEmpty(settings.ServiceAddress)? DefaultserviceAddress: settings.ServiceAddress;

            var pipeChannelFactory = new DuplexChannelFactory<IReplicationService>(new InstanceContext(this),
                                                                                   new NetNamedPipeBinding(), new EndpointAddress(serviceAddress));
            _channel = pipeChannelFactory.CreateChannel();
            _replicationObjects = new List<IReplicationObject>();

            PropertyChangedHandler += async (sender, args) =>
            {
                var replicationInfo = GetReplicationInfo(sender, UidKey, args.PropertyName);
                try
                {
                    await Task.Factory.StartNew(() => _channel.SetObject(replicationInfo, OperationType.Update));
                }
                catch (Exception)
                {
                }
            };
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <returns></returns>
        public async Task<Boolean> Initialize()
        {
            var result = true;
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    var collection = _channel.Connect();

                    foreach (var replicationInfo in collection)
                    {
                        var uid = replicationInfo.Uid.ToString();

                        if (typeof (T).FullName == replicationInfo.FullName)
                        {
                            var item = _replicationObjects.FirstOrDefault(r => r.Uid == uid);
                            if (item != null)
                            {
                                SimulateObjectChanged(replicationInfo, OperationType.Update);
                            }
                            else
                            {
                                SimulateObjectChanged(replicationInfo, OperationType.Create);
                            }
                        }
                    }
                });
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Деинициализация
        /// </summary>
        public async Task<Boolean> Deinitialize()
        {
            var result = true;
            try
            {
                await Task.Factory.StartNew(() => _channel.Disconnect());
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Установить объект для репликации
        /// </summary>
        public Boolean SetObject(IReplicationObject replicationObject)
        {
            var result = true;
            try
            {
                replicationObject.PropertyChanged += PropertyChangedHandler;
                _replicationObjects.Add(replicationObject);

                _channel.SetObject(GetReplicationInfo(replicationObject), OperationType.Create);

            }
            catch (Exception)
            {
                replicationObject.PropertyChanged -= PropertyChangedHandler;
                _replicationObjects.Remove(replicationObject);

                result = false;
            }
            return result;
        }

        /// <summary>
        /// Снять объект с репликации
        /// </summary>
        public Boolean FreeObject(IReplicationObject replicationObject)
        {
            var result = true;
            var existReplicationObject = _replicationObjects.FirstOrDefault(r => r.Uid == replicationObject.Uid);
            try
            {
                replicationObject.PropertyChanged -= PropertyChangedHandler;

                if (existReplicationObject != null)
                {
                    existReplicationObject.PropertyChanged -= PropertyChangedHandler;

                    _replicationObjects.RemoveAll(r => r.Uid == replicationObject.Uid);
                    _channel.SetObject(GetReplicationInfo(replicationObject), OperationType.Delete);
                }
            }
            catch (Exception)
            {
                if (existReplicationObject != null)
                {
                    existReplicationObject.PropertyChanged += PropertyChangedHandler;
                }
                replicationObject.PropertyChanged += PropertyChangedHandler;
                _replicationObjects.Add(replicationObject);

                result = false;
            }
            return result;
        }

        /// <summary>
        /// Уведомить об изменении объекта
        /// </summary>
        /// <param name="replicationInfo">Информация о реплицируемом объекте</param>
        /// <param name="operationType">Тип операции репликации</param>
        public void SimulateObjectChanged(ReplicationInfo replicationInfo, OperationType operationType)
        {
            var newProperties = replicationInfo.Properties;
            var uid = replicationInfo.Uid.ToString();

            if (typeof(T).FullName == replicationInfo.FullName)
            {
                IReplicationObject replicationObject;
                if (operationType == OperationType.Create)
                {
                    replicationObject = new T();
                    SetReplicationObjectProperties(replicationObject, newProperties);
                    replicationObject.PropertyChanged += PropertyChangedHandler;
                    _replicationObjects.Add(replicationObject);
                }
                else
                {
                    replicationObject =_replicationObjects.FirstOrDefault(ro => String.Compare(ro.Uid, uid, StringComparison.OrdinalIgnoreCase) == 0) ?? new T();
                    SetReplicationObjectProperties(replicationObject, newProperties);
                    if (operationType == OperationType.Delete)
                    {
                        _replicationObjects.RemoveAll(r => r.Uid == uid);
                    }
                }
                RaiseObjectReplicated(replicationObject, operationType);
            }
        }
        
        /// <summary>
        /// Получить информацию о реплицируемом объекте
        /// </summary>
        /// <param name="item">Реплицируемый объект</param>
        /// <returns>Информация о реплицируемом объекте</returns>
        private ReplicationInfo GetReplicationInfo(Object item)
        {
            var type = item.GetType();
            var propertiesInfo = type.GetProperties();

            var properties = propertiesInfo.ToDictionary(propertiyInfo => propertiyInfo.Name, propertiyInfo => propertiyInfo.GetValue(item, null));

            var uid = properties[UidKey].ToString();
            //properties.Remove(UidKey);

            return new ReplicationInfo(uid, type.FullName, properties);
        }

        /// <summary>
        /// Получить информацию о реплицируемом объекте
        /// </summary>
        /// <param name="item">Реплицируемый объект</param>
        /// <param name="propertyNames">Имена свойств</param>
        /// <returns>Информация о реплицируемом объекте</returns>
        private ReplicationInfo GetReplicationInfo(Object item, params String[] propertyNames)
        {
            var properties = new Dictionary<String, Object>();

            var type = item.GetType();
            foreach (var propertyName in propertyNames)
            {
                var propertyValue = type.GetProperty(propertyName).GetValue(item, null);
                properties.Add(propertyName, propertyValue);
            }

            var uid = properties[UidKey];
            //properties.Remove(UidKey);

            return new ReplicationInfo(uid, type.FullName, properties);
        }

        /// <summary>
        /// Установка значений свойств реплицируемого объекта
        /// </summary>
        /// <param name="replicationObject">Объект репликации</param>
        /// <param name="properties">Имя и значения свойств</param>
        private void SetReplicationObjectProperties(object replicationObject, Dictionary<String, Object> properties)
        {
            var replicationProperties = replicationObject.GetType().GetProperties();
            foreach (var newProperty in properties)
            {
                var property = replicationProperties.FirstOrDefault(p => p.Name == newProperty.Key);
                if (property != null)
                {
                    property.SetValue(replicationObject, Convert.ChangeType(newProperty.Value, property.PropertyType),null);
                }
            }
        }

        /// <summary>
        /// Вызвать событие репликации объекта
        /// </summary>
        /// <param name="replicationObject">Объект репликации</param>
        /// <param name="operationType">Тип операции</param>
        private void RaiseObjectReplicated(IReplicationObject replicationObject, OperationType operationType)
        {
            Application.Current.InvokeSafe(()=>InvokeObjectReplicated(new ReplicationEventArgs(replicationObject, operationType)));
        }
        /// <summary>
        /// Событие репликации объекта
        /// </summary>
        public event EventHandler<ReplicationEventArgs> ObjectReplicated;
        /// <summary>
        /// Вызвать событие репликации объекта
        /// </summary>
        /// <param name="e">Аргументы для события репликации</param>
        private void InvokeObjectReplicated(ReplicationEventArgs e)
        {
            EventHandler<ReplicationEventArgs> handler = ObjectReplicated;
            if (handler != null) handler(this, e);
        }
    }
}
