namespace AK_Services.Interfaces;

public interface IUnitOfWork
{
    public IAchivementService Achivements { get; }
    public ILeaderboardService Leaderboards { get; }
    public ILeaderboardEntriesService LeaderboardEntries { get; }
}