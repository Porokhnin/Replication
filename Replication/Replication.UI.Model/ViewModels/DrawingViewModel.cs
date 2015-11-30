using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using Replication.Abstraction;
using Replication.Library;
using Replication.Library.EventArguments;
using Replication.UI.Model.Views;

namespace Replication.UI.Model.ViewModels
{
    /// <summary>
    /// Вью модель для рисования
    /// </summary>
    [Export]
    public class DrawingViewModel : ViewModel<IDrawingView>, IDisposable
    {        
        #region Поля
        /// <summary>
        /// Клиент для репликации объектов
        /// </summary>
        private readonly IReplicationClient _replicationClient;
        /// <summary>
        /// Прямоуголники
        /// </summary>
        private readonly List<Rectangle> _rectangles;

        #endregion Поля

        [ImportingConstructor]
        public DrawingViewModel(IDrawingView view): base(view)
        {
            view.UpdateRectangleState = UpdateRectangleState;

            _rectangles = new List<Rectangle>();

            _replicationClient = new ReplicationClient<Rectangle>();
            _replicationClient.ObjectReplicated += OnReplicationClientObjectReplicated;
        }

        #region Методы
        /// <summary>
        /// Инициализация
        /// </summary>
        public void Initialize()
        {
            _replicationClient.Initialize();
        }

        /// <summary>
        /// Деинициализация
        /// </summary>
        public void Dispose()
        {
            _replicationClient.Deinitialize();
        }

        /// <summary>
        /// Обновить состояние прямоугольника
        /// </summary>
        /// <param name="rectangle">Прямоугольник</param>
        /// <param name="operationType">Тип операции</param>
        private void UpdateRectangleState(Rectangle rectangle, OperationType operationType)
        {
            UpdateRectangleState(rectangle, operationType, true);
        }

        /// <summary>
        /// Обновить состояние прямоугольника
        /// </summary>
        /// <param name="rectangle">Прямоугольник</param>
        /// <param name="operationType">Тип операции</param>
        /// <param name="isNotify">Уведомлят клиента</param>
        private void UpdateRectangleState(Rectangle rectangle, OperationType operationType, Boolean isNotify)
        {
            if (operationType == OperationType.Create)
            {
                if (isNotify)
                {
                    if (_replicationClient.SetObject(rectangle))
                    {
                        _rectangles.Add(rectangle);    
                    }
                }
                else
                {
                    _rectangles.Add(rectangle);                    
                }
            }
            else
            {
                var existedRectangle = _rectangles.FirstOrDefault(r => String.Compare(r.Uid, rectangle.Uid, StringComparison.OrdinalIgnoreCase) == 0);
                if (operationType == OperationType.Update)
                {
                    if (existedRectangle != null)
                    {
                        existedRectangle.Update(rectangle);
                    }
                }
                else
                {
                    if (existedRectangle != null)
                    {
                        if (isNotify)
                        {
                            if (_replicationClient.FreeObject(existedRectangle))
                            {
                                _rectangles.Remove(existedRectangle);
                            }
                        }
                        else
                        {
                            _rectangles.Remove(existedRectangle);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Обработчик события репликации объектов (нужен для перерисовки)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplicationClientObjectReplicated(object sender, ReplicationEventArgs e)
        {
            var rectangle = (Rectangle)e.ReplicationObject;
            UpdateRectangleState(rectangle, e.OperationType, false);
            this.ViewCore.RedrawRectangle(rectangle, e.OperationType);
        }
        #endregion Методы
    }
}