using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Replication.UI.Model.Views;

namespace Replication.UI.Model.ViewModels
{
    [Export]
    public class MainViewModel: ViewModel<IMainView>, IDisposable
    {        
        #region Поля
        private object _currentView;
        private readonly DrawingViewModel _drawingViewModel; 
        #endregion Поля

        #region Свойства
        /// <summary>
        /// Вьюмодель текущего отображения
        /// </summary>
        public object CurrentView
        {
            get { return _currentView; }
            set { SetProperty(ref _currentView, value); }
        }

        /// <summary>
        /// Вьюмодель для рисования
        /// </summary>
        public DrawingViewModel DrawingViewModel
        {
            get { return _drawingViewModel; }
        }
        
        #endregion Свойства

        [ImportingConstructor]
        public MainViewModel(IMainView view, ExportFactory<DrawingViewModel> drawingViewModellExport)
            : base(view)
        {                                 
            _drawingViewModel = drawingViewModellExport.CreateExport().Value;

            CurrentView = DrawingViewModel.View;
        }

        #region Методы
        /// <summary>
        /// Инициализация
        /// </summary>
        public void Initialize()
        {
            _drawingViewModel.Initialize();
        }
        /// <summary>
        /// Отобразить
        /// </summary>
        public void Show()
        {
            ViewCore.Show();
        }
        /// <summary>
        /// Закрыть
        /// </summary>
        public void Close()
        {
            ViewCore.Close();
        }
        /// <summary>
        /// Деинициализация
        /// </summary>
        public void Dispose()
        {
            _drawingViewModel.Dispose();
        }
        #endregion Методы
    }
}