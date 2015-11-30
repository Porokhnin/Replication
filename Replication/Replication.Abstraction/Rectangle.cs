using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Replication.Abstraction.Annotations;

namespace Replication.Abstraction
{
    /// <summary>
    /// Прямоугольник
    /// </summary>
    public class Rectangle:IReplicationObject
    {
        private String _uid;
        private Double _x;
        private Double _y;
        private Double _width;
        private Double _height;

        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        public String Uid
        {
            get { return _uid; }
            private set
            {
                if (value == _uid) return;
                _uid = value;
                InvokePropertyChanged();
            }
        }

        /// <summary>
        /// X точки отсчёта (верхнего левого угла)
        /// </summary>
        public Double X
        {
            get { return _x; }
            set
            {
                if (value.Equals(_x)) return;
                _x = value;
                InvokePropertyChanged();
            }
        }
        /// <summary>
        /// Y точки отсчёта (верхнего левого угла)
        /// </summary>
        public Double Y
        {
            get { return _y; }
            set
            {
                if (value.Equals(_y)) return;
                _y = value;
                InvokePropertyChanged();
            }
        }
        /// <summary>
        /// Ширина
        /// </summary>
        public Double Width
        {
            get { return _width; }
            set
            {
                if (value.Equals(_width)) return;
                _width = value;
                InvokePropertyChanged();
            }
        }

        /// <summary>
        /// Высота
        /// </summary>
        public Double Height
        {
            get { return _height; }
            set
            {
                if (value.Equals(_height)) return;
                _height = value;
                InvokePropertyChanged();
            }
        }
        /// <summary>
        /// Прямоугольник
        /// </summary>
        public Rectangle()
        {

        }
        /// <summary>
        /// Прямоугольник
        /// </summary>
        /// <param name="uid">Идентификатор объекта</param>
        public Rectangle(String uid)
        {
            Uid = uid;
        }
        /// <summary>
        /// Прямоугольник
        /// </summary>
        /// <param name="uid">Идентификатор объекта</param>
        /// <param name="location">Точка отсчета</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        public Rectangle(String uid, Point location, Int32 width, Int32 height): this(uid, location.X, location.Y, width, height)
        {

        }
        /// <summary>
        /// Прямоугольник
        /// </summary>
        /// <param name="uid">Идентификатор объекта</param>
        /// <param name="x">X точки отсчёта</param>
        /// <param name="y">Y точки отсчёта</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        public Rectangle(String uid, Double x, Double y, Double width, Double height)
        {
            Uid = uid;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void InvokePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update(Rectangle rectangle)
        {
            if (rectangle != null)
            {
                this.X = rectangle.X;
                this.Y = rectangle.Y;
                this.Width = rectangle.Width;
                this.Height = rectangle.Height;
            }
        }
    }
}
