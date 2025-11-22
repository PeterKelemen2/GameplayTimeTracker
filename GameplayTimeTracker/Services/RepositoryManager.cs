using GameplayTimeTracker.Repositories;

namespace GameplayTimeTracker.Services;

public class RepositoryManager
{
    public GameRepository GameRepository { get; }
    public PlaytimeRepository PlaytimeRepository { get; }
    public PlaytimeHistoryRepository PlaytimeHistoryRepository { get; }
    public SettingsRepository SettingsRepository { get; }
    public SettingsProfileRepository SettingsProfileRepository { get; }

    public RepositoryManager(
        GameRepository gameRepo,
        PlaytimeRepository playtimeRepo,
        PlaytimeHistoryRepository historyRepo,
        SettingsRepository settingsRepo,
        SettingsProfileRepository profileRepo)
    {
        GameRepository = gameRepo;
        PlaytimeRepository = playtimeRepo;
        PlaytimeHistoryRepository = historyRepo;
        SettingsRepository = settingsRepo;
        SettingsProfileRepository = profileRepo;
    }
}