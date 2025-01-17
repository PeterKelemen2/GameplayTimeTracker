using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GameplayTimeTracker.Menu;

namespace GameplayTimeTracker;

public partial class MainWindow : Window
{
    private EntryRepository entryRepository;

    public MainWindow()
    {
        InitializeComponent();
        MainGrid.SizeChanged += MainGrid_SizeChanged;
        Loaded += OnLoaded;
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        CreateFooterButtons();
        SetBaseColors();
        entryRepository = new EntryRepository();

        GameCardRepository gameCardRepository = new GameCardRepository();
        foreach (var entry in entryRepository.EntriesList)
        {
            // GameCard gc = new GameCardHorizontal(entry, gameCardRepository, MainPanel);
            GameCard gc = new GameCardVertical(entry, gameCardRepository, MainPanel);
            gameCardRepository.GameCards.Add(gc);
            MainPanel.Children.Add(gc);
        }

        CustomMenu testCustomMenu = new CustomMenu();
    }

    private void SetBaseColors()
    {
        Footer.Background = new SolidColorBrush(AppColors.Footer);
        MainScrollViewer.Background = new SolidColorBrush(AppColors.Background);
    }

    private void CreateFooterButtons()
    {
        CustomButton AddButton = new CustomButton(width: 40, height: 40, hA: HorizontalAlignment.Left,
            buttonImagePath: AppFiles.AddIcon);
        AddButton.Margin = new Thickness(15, 0, 0, 0);
        AddButton.Effect = AppEffects.dropShadowIcon;
        AddButton.Click += AddEntry_Click;
        Grid.SetRow(AddButton, 1);
        MainGrid.Children.Add(AddButton);

        CustomButton SettingsButton = new CustomButton(width: 40, height: 40, hA: HorizontalAlignment.Left,
            buttonImagePath: AppFiles.CogIcon);
        SettingsButton.Margin = new Thickness(70, 0, 0, 0);
        SettingsButton.Effect = AppEffects.dropShadowIcon;
        SettingsButton.Click += Settings_Click;
        Grid.SetRow(SettingsButton, 1);
        MainGrid.Children.Add(SettingsButton);
    }

    public void AddEntry_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Adding entry");
    }

    public void Settings_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Opening Settings menu");
    }
    
    private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var grid = sender as Grid;
        var scaleTransform = grid.RenderTransform as ScaleTransform;

        if (scaleTransform != null)
        {
            scaleTransform.CenterX = grid.ActualWidth / 2;
            scaleTransform.CenterY = grid.ActualHeight / 2;
        }
    }
}