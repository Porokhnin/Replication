using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Replication.UI.Model.Views;
using Replication.Abstraction;
using Rectangle = System.Windows.Shapes.Rectangle;
using Point = System.Windows.Point;

namespace Replication.UI.Presentation
{
    /// <summary>
    /// Interaction logic for DrawingView.xaml
    /// </summary>
    [Export(typeof(IDrawingView))]
    public partial class DrawingView : IDrawingView
    {
        private Shape _currentShape;

        private Nullable<Point> _drawStart;
        private Nullable<Point> _dragStart;

        private static MouseButtonEventHandler _elementMouseDownHandler;
        private static MouseButtonEventHandler _elementMouseUpHandler;
        private static MouseEventHandler _elementMouseMoveHandler;

        private ICommand DeleteShapeCommand;

        public Action<Abstraction.Rectangle, OperationType> UpdateRectangleState { get; set; }
        public Action<Abstraction.Rectangle, OperationType> RedrawRectangle { get; set; }   

        public DrawingView()
        {
            InitializeComponent();

            DeleteShapeCommand = new DelegateCommand(DeleteShape);

            RedrawRectangle = (rectangle, operationType) =>
            {
                var uid = rectangle.Uid;
                if (operationType == OperationType.Create)
                {
                    var shapeRectangle = GetNewRectangle(uid);

                    SetShapeSize(shapeRectangle, rectangle);
                    SetDragEnableForUIElement(shapeRectangle);
                    ReplicationCanvas.Children.Add(shapeRectangle);
                }
                else if (operationType == OperationType.Update)
                {
                    foreach (Shape child in ReplicationCanvas.Children)
                    {
                        if (child.Uid == uid)
                        {
                            SetShapeSize(child, rectangle);
                            break;
                        }
                    }
                }
                else if (operationType == OperationType.Delete)
                {
                    Shape deletedObject = ReplicationCanvas.Children.Cast<Shape>().FirstOrDefault(child => child.Uid == uid);
                    if (deletedObject != null)
                    {
                        ReplicationCanvas.Children.Remove(deletedObject);
                    }
                }
            };

            _elementMouseDownHandler = (sender, args) =>
            {
                if (args.ChangedButton == MouseButton.Left)
                {
                    var element = (UIElement) sender;
                    _dragStart = args.GetPosition(element);
                    element.CaptureMouse();
                    args.Handled = true;
                }
            };

            _elementMouseMoveHandler = (sender, args) =>
            {
                if (_dragStart != null && args.LeftButton == MouseButtonState.Pressed)
                {
                    var element = (UIElement)sender;
                    var canvasPosition = args.GetPosition(ReplicationCanvas);
                    Canvas.SetLeft(element, canvasPosition.X - _dragStart.Value.X);
                    Canvas.SetTop(element, canvasPosition.Y - _dragStart.Value.Y);
                }
            };

            _elementMouseUpHandler = (sender, args) =>
            {
                if (args.ChangedButton == MouseButton.Left)
                {
                    var element = (UIElement) sender;
                    _dragStart = null;
                    element.ReleaseMouseCapture();

                    UpdateShapeState(element, OperationType.Update);
                }
            };
        }

        private void DeleteShape(object shapeObject)
        {
            Shape shape = shapeObject as Shape;
            if (shape != null)
            {
                ReplicationCanvas.Children.Remove(shape);
                UpdateRectangleState(new Abstraction.Rectangle(shape.Uid), OperationType.Delete);
            }
        }

        private Shape GetNewRectangle(String Uid)
        {            
            var rectangle = new Rectangle()
            {
                Uid = Uid,
                Fill = new SolidColorBrush(Colors.DeepSkyBlue)               
            };

            var menuItem = new MenuItem()
            {
                Header = "Удалить",
                Command = DeleteShapeCommand,
                CommandParameter = rectangle
            };

            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItem);
            rectangle.ContextMenu = contextMenu;

            return rectangle;
        }

        private void UpdateShapeState(UIElement element, OperationType operationType)
        {
            var rectangleElement = element as Rectangle;
            if (rectangleElement != null)
            {                
                var location = rectangleElement.TranslatePoint(new Point(0, 0), ReplicationCanvas);
                var rectangle = new Abstraction.Rectangle(rectangleElement.Uid, location.X, location.Y, rectangleElement.Width, rectangleElement.Height);
                if (operationType == OperationType.Create)
                {
                    SetDragEnableForUIElement(element);
                }
                UpdateRectangleState(rectangle, operationType);
            }
        }

        private void SetDragEnableForUIElement(UIElement element)
        {
            element.MouseDown += _elementMouseDownHandler;
            element.MouseMove += _elementMouseMoveHandler;
            element.MouseUp += _elementMouseUpHandler;
        }

        private void SetShapeSize(Shape shape, Point currentPosition, Point startPosition)
        {
            shape.SetValue(Canvas.LeftProperty, Math.Min(currentPosition.X, startPosition.X));
            shape.SetValue(Canvas.TopProperty, Math.Min(currentPosition.Y, startPosition.Y));
            shape.Width = Math.Abs(currentPosition.X - startPosition.X);
            shape.Height = Math.Abs(currentPosition.Y - startPosition.Y);
        }

        private void SetShapeSize(Shape shape, Abstraction.Rectangle rectangle)
        {
            shape.SetValue(Canvas.LeftProperty, rectangle.X);
            shape.SetValue(Canvas.TopProperty, rectangle.Y);
            shape.Width = rectangle.Width;
            shape.Height = rectangle.Height;
        }

        private void OnReplicationCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.ChangedButton == MouseButton.Left)
            {                
                _drawStart = e.MouseDevice.GetPosition(ReplicationCanvas);
                _currentShape = GetNewRectangle(Guid.NewGuid().ToString());

                ReplicationCanvas.Children.Add(_currentShape);
            }
        }
        
        private void OnReplicationCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_drawStart.HasValue)
            {                
                var shape = _currentShape;
                var currentPosition = e.MouseDevice.GetPosition(ReplicationCanvas);
                var startPosition = _drawStart.Value;

                if (shape != null)
                {
                    SetShapeSize(shape, currentPosition, startPosition);
                }
            }
        }
        
        private void OnReplicationCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_currentShape != null)
                {
                    UpdateShapeState(_currentShape, OperationType.Create);
                }
                _currentShape = null;
                _drawStart = null;
            }
        }
    }
}
