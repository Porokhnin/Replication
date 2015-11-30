using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Waf.Applications;
using System.Windows;
using Replication.UI.Model.Controllers;

namespace Replication.UI.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AggregateCatalog _catalog;
        private CompositionContainer _container;
        private IApplicationController _controller;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _catalog = new AggregateCatalog();
            // Add the WpfApplicationFramework assembly to the catalog
            _catalog.Catalogs.Add(new AssemblyCatalog(typeof(ViewModel).Assembly));
            // Add the Writer.Presentation assembly to the catalog
            _catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            // Add the Writer.Applications assembly to the catalog
            _catalog.Catalogs.Add(new AssemblyCatalog(typeof(IApplicationController).Assembly));

            _container = new CompositionContainer(_catalog, CompositionOptions.DisableSilentRejection);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(_container);
            _container.Compose(batch);

            _controller = _container.GetExportedValue<IApplicationController>();
            _controller.Initialize();
            _controller.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _controller.Shutdown();
            _container.Dispose();
            _catalog.Dispose();

            base.OnExit(e);
        }
    }
}
