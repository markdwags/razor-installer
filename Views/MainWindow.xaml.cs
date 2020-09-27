using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using RazorInstaller.ViewModel;

namespace RazorInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(DialogCoordinator.Instance);
        }
    }
}
