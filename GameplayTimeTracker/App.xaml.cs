using System;
using System.Windows;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Repositories;
using GameplayTimeTracker.Services;
using GameplayTimeTracker.Services.Tracker;
using GameplayTimeTracker.ViewModels;
using GameplayTimeTracker.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GameplayTimeTracker
{
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();

            var databaseService = Services.GetService<DatabaseService>();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // DbContext
            services.AddSingleton<AppDbContext>();

            // Repos
            services.AddSingleton<GameRepository>();
            services.AddSingleton<PlaytimeRepository>();
            services.AddSingleton<SettingsRepository>();
            services.AddSingleton<ThemeRepository>();
            services.AddSingleton<RemoteMachineRepository>();

            services.AddSingleton<RepositoryManager>();

            // Db service
            services.AddSingleton<DatabaseService>();

            services.AddSingleton<GameTrackingManager>();

            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<MainWindow>();
        }
    }
}