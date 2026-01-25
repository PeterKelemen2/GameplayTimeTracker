using GameplayTimeTracker.Repositories;

namespace GameplayTimeTracker.Services;

public class RepositoryManager(
    GameRepository gameRepo,
    PlaytimeRepository playtimeRepo,
    SettingsRepository settingsRepo,
    ThemeRepository themeRepo,
    RemoteMachineRepository remoteMachineRepo)
{
    public GameRepository GameRepository { get; } = gameRepo;
    public PlaytimeRepository PlaytimeRepository { get; } = playtimeRepo;
    public SettingsRepository SettingsRepository { get; } = settingsRepo;
    public ThemeRepository ThemeRepository { get; } = themeRepo;
    public RemoteMachineRepository RemoteMachineRepository { get; } = remoteMachineRepo;
}