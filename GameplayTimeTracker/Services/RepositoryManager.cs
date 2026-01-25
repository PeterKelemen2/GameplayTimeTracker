using GameplayTimeTracker.Repositories;

namespace GameplayTimeTracker.Services;

public class RepositoryManager(
    GameRepository gameRepo,
    PlaytimeRepository playtimeRepo,
    SettingsRepository settingsRepo)
{
    public GameRepository GameRepository { get; } = gameRepo;
    public PlaytimeRepository PlaytimeRepository { get; } = playtimeRepo;
    public SettingsRepository SettingsRepository { get; } = settingsRepo;
}