using System.ComponentModel.Composition;
using Replication.UI.Model.Views;

namespace Replication.UI.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export(typeof(IMainView))]
    public partial class MainWindow : IMainView
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
