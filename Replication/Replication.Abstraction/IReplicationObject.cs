using System;
using System.ComponentModel;

namespace Replication.Abstraction
{
    /// <summary>
    /// Реплицируемый объект
    /// </summary>
    public interface IReplicationObject:INotifyPropertyChanged
    {
        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        String Uid { get; }
    }
}
