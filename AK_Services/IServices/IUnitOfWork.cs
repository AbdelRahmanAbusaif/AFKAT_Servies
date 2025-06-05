namespace AK_Services.Interfaces;

public interface IUnitOfWork
{
    public IAchivementsService Achivementses { get; }
    public ILeaderboardService Leaderboards { get; }
    public ILeaderboardEntriesService LeaderboardEntries { get; }
    public IPlayerAchievementService PlayerAchievements { get; }
    public IPlayerSaves PlayerSaves { get; }
}