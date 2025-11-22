using System;
using System.Windows;
using Window = System.Windows.Window;

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
        
    }
    

    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            Console.WriteLine("Exiting...");
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
            e.Cancel = true;
        }
    }
}