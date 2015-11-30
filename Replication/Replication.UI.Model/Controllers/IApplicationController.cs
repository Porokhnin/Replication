namespace Replication.UI.Model.Controllers
{
    /// <summary>
    /// Контроллер приложения
    /// </summary>
    public interface IApplicationController
    {
        /// <summary>
        /// Инициализация
        /// </summary>
        void Initialize();
        /// <summary>
        /// Запуск
        /// </summary>
        void Run();
        /// <summary>
        /// Завершение
        /// </summary>
        void Shutdown();
    }
}
