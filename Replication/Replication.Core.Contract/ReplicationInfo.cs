using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Replication.Core.Contract
{
    /// <summary>
    /// Информация о реплицируемом объекте
    /// </summary>
    [DataContract]
    public class ReplicationInfo
    {
        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        [DataMember]
        public Object Uid { get; private set; }

        /// <summary>
        /// Полное имя типа объекта
        /// </summary>
        [DataMember]
        public String FullName { get; private set; }
        
        /// <summary>
        /// Словарь свойств объекта и их значений
        /// </summary>
        [DataMember]
        public Dictionary<String, Object> Properties { get; private set; }

        /// <summary>
        /// Информация о реплицируемом объекте
        /// </summary>
        /// <param name="uid">Идентификатор объекта</param>
        /// <param name="fullName">Полное имя типа объекта</param>
        public ReplicationInfo(Object uid, String fullName): this(uid, fullName, new Dictionary<String, Object>())
        {

        }

        /// <summary>
        /// Информация о реплицируемом объекте
        /// </summary>
        /// <param name="uid">Идентификатор объекта</param>
        /// <param name="fullName">Полное имя типа объекта</param>
        /// <param name="properties">Словарь свойств объекта и их значений</param>
        public ReplicationInfo(Object uid, String fullName, Dictionary<String, Object> properties)
        {
            Uid = uid;
            FullName = fullName;
            Properties = properties;
        }

        /// <summary>
        /// Обновить значение свойств в словаре
        /// </summary>
        /// <param name="properties">Словарь свойств объекта и их значений</param>
        public void UpdateProperties(Dictionary<String, Object> properties)
        {
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    Properties[property.Key] = property.Value;
                }
            }
        }
    }
}
