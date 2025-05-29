using AFKAT_Servies;
using AK_Services.Interfaces;
using Supabase;

namespace AK_Services.Services;

public class UnitOfWork : IUnitOfWork
{
    public IAchivementService Achivements { get; }
    public ILeaderboardService Leaderboards { get; }
    public ILeaderboardEntriesService LeaderboardEntries { get; }
    
    private readonly Client _supabaseClient;

    public UnitOfWork(Supabase.Client client)
    {
        _supabaseClient = client;

        LeaderboardEntries = new LeaderboardEntriesService(_supabaseClient);
        Achivements = new AchivementsService(_supabaseClient);
        Leaderboards = new LeaderboardService(_supabaseClient);
    }
    
}