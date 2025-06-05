using AFKAT_Servies;
using AK_Services.Interfaces;
using Supabase;

namespace AK_Services.Services;

public class UnitOfWork : IUnitOfWork
{
    public IAchivementsService Achivementses { get; }
    public ILeaderboardService Leaderboards { get; }
    public ILeaderboardEntriesService LeaderboardEntries { get; }
    public IPlayerAchievementService PlayerAchievements { get; }
    
    private readonly Client _supabaseClient;
    private readonly IFileService _fileService;

    public UnitOfWork(Supabase.Client client, IFileService fileService)
    {
        _supabaseClient = client;
        _fileService = fileService;
        
        LeaderboardEntries = new LeaderboardEntriesService(_supabaseClient);
        Achivementses = new AchivementsService(_supabaseClient, _fileService);
        Leaderboards = new LeaderboardService(_supabaseClient);
        PlayerAchievements = new PlayerAchievementService(_supabaseClient, _fileService);
    }
    
}