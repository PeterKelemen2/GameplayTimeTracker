using System;
using System.Windows;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Repositories;
using GameplayTimeTracker.Services;
using Microsoft.Extensions.DependencyInjection;

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
            // DbContext
            services.AddSingleton<AppDbContext>();
            
            // Repos
            services.AddSingleton<GameRepository>();
            services.AddSingleton<PlaytimeRepository>();
            services.AddSingleton<PlaytimeHistoryRepository>();
            services.AddSingleton<SettingsRepository>();
            
            services.AddSingleton<RepositoryManager>();

            // Db service
            services.AddSingleton<DatabaseService>();

            services.AddSingleton<MainWindow>();
        }
    }
}