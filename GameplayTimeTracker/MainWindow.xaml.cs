using System;
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

namespace GameplayTimeTracker;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        CustomButton AddButton = new CustomButton(width: 40, height: 40, hA: HorizontalAlignment.Left,
            buttonImagePath: Files.AddIcon);
        AddButton.Margin = new Thickness(15, 0, 0, 0);
        AddButton.Click += AddEntry_Click;
        Grid.SetRow(AddButton, 1);
        MainGrid.Children.Add(AddButton);
    }

    public void AddEntry_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Adding entry");
    }

    public void Settings_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Opening Settings menu");
    }
}