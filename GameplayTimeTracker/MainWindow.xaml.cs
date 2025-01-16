using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameplayTimeTracker;

public partial class MainWindow : Window
{
    private EntryRepository entryRepository;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        CreateFooterButtons();
        SetBaseColors();
        entryRepository = new EntryRepository();
        // Entry entry = new Entry();
        // entry.Name = "TestName";
        // entry.Arguments = "TestArguments";
        // entry.TotalPlay = new[] { 12, 2, 45 };
        // entry.LastPlay = new[] { 8, 6, 33 };
        // entry.IconPath = "C:\\Users\\Peti\\Documents\\Gameplay Time Tracker\\SteamGridDB Images\\20530_icon.png";
        // entry.HeroPath = "C:\\Users\\Peti\\Documents\\Gameplay Time Tracker\\SteamGridDB Images\\20530_hero.png";
        // entry.ExePath = "C:\\Program Files\\VSCodium\\VSCodium.exe";
        // entryRepository.AddEntry(entry);
        
        // entryRepository.PrintEntryList();
        // DataHandler.WriteEntriesToFile(entryRepository.EntriesList, AppFiles.DataFilePath);

        GameCardRepository gameCardRepository = new GameCardRepository();
        foreach (var entry in entryRepository.EntriesList)
        {
            // GameCard gc = new GameCardHorizontal(entry, gameCardRepository, MainPanel);
            GameCard gc = new GameCardVertical(entry, gameCardRepository, MainPanel);
            gameCardRepository.GameCards.Add(gc);
            MainPanel.Children.Add(gc);
        }
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
}