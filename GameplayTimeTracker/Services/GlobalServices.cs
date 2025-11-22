using Microsoft.Extensions.DependencyInjection;

namespace GameplayTimeTracker.Services;

public static class GlobalServices
{
    public static RepositoryManager Repositories => App.Current.Services.GetRequiredService<RepositoryManager>();
}