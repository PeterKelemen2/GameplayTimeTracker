using GameplayTimeTracker.Repositories;

namespace GameplayTimeTracker.Services;

public class RepositoryManager(
    GameRepository gameRepo,
    PlaytimeRepository playtimeRepo,
    PlaytimeHistoryRepository historyRepo,
    SettingsRepository settingsRepo)
{
    public GameRepository GameRepository { get; } = gameRepo;
    public PlaytimeRepository PlaytimeRepository { get; } = playtimeRepo;
    public PlaytimeHistoryRepository PlaytimeHistoryRepository { get; } = historyRepo;
    public SettingsRepository SettingsRepository { get; } = settingsRepo;
}