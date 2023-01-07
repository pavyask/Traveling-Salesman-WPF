using System.Windows;
using TSP_WPF.ViewModels;

namespace TSP_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            layoutGrid.DataContext = new MainViewModel(this);
        }
    }
}
