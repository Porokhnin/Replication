using System.Waf.Applications;

namespace Replication.UI.Model.Views
{
    /// <summary>
    /// Вьюха главного окна
    /// </summary>
    public interface IMainView : IView
    {
        void Show();

        void Close();
    }
}
