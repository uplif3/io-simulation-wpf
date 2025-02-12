using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using io_simulation_wpf.Models;
using io_simulation_wpf.Services;
using io_simulation_wpf.Views;
using io_simulation_wpf.ViewModels;

namespace io_simulation_wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        //this.DataContext = new MainViewModel();
    }
}