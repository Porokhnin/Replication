using System;
using System.Waf.Applications;
using Replication.Abstraction;

namespace Replication.UI.Model.Views
{
    /// <summary>
    /// Вьюха для рисования
    /// </summary>
    public interface IDrawingView : IView
    {
        /// <summary>
        /// Обновить состояние прямоугольника
        /// </summary>
        Action<Rectangle, OperationType> UpdateRectangleState { get; set; }
        /// <summary>
        /// Перерисовать прямоугольник
        /// </summary>
        Action<Rectangle, OperationType> RedrawRectangle { get; set; }
    }
}
