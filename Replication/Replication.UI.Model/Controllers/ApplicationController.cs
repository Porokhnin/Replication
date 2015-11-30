using System.ComponentModel.Composition;
using Replication.UI.Model.ViewModels;

namespace Replication.UI.Model.Controllers
{
    /// <summary>
    /// Контроллер приложения
    /// </summary>
    [Export(typeof(IApplicationController))]
    internal class ApplicationController : IApplicationController
    {
        private readonly ExportFactory<MainViewModel> _mainViewModelExportFactory;
        private ExportLifetimeContext<MainViewModel> _mainViewModelExport;
        /// <summary>
        /// Вью модель главного окна
        /// </summary>
        private MainViewModel _mainViewModel;

        [ImportingConstructor]
        public ApplicationController(ExportFactory<MainViewModel> mainViewModelExportFactory)
        {
            _mainViewModelExportFactory = mainViewModelExportFactory;
        }

        /// <summary>
        /// Инициализация
        /// </summary>
        public void Initialize()
        {
            InitializeMainViewModel();
        }

        /// <summary>
        /// Запуск
        /// </summary>
        public void Run()
        {
            RunMainViewModel();
        }

        /// <summary>
        /// Завершение
        /// </summary>
        public void Shutdown()
        {
            CloseMainViewModel();
        }
        /// <summary>
        /// Инициализация вью модели главного окна
        /// </summary>
        private void InitializeMainViewModel()
        {
            _mainViewModelExport = _mainViewModelExportFactory.CreateExport();
            _mainViewModel = _mainViewModelExport.Value;
            _mainViewModel.Initialize();
        }

        /// <summary>
        /// Запуск  вью модели главного окна
        /// </summary>
        private void RunMainViewModel()
        {
            _mainViewModel.Show();
        }

        /// <summary>
        /// Завершение вью модели главного окна
        /// </summary>
        private void CloseMainViewModel()
        {
            if (_mainViewModel != null)
            {
                _mainViewModel.Close();
                _mainViewModel.Dispose();
                
                _mainViewModelExport.Dispose();

                _mainViewModel = null;
                _mainViewModelExport = null;
            }
        }
    }
}
