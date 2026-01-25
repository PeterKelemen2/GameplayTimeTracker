using System.Windows;
using GameplayTimeTracker.ViewModels;
using Window = System.Windows.Window;

namespace GameplayTimeTracker.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _vm;

    public MainWindow(MainWindowViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;
        Loaded += OnLoaded;
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        _vm.OnLoaded();
    }
}