using AFKAT_Servies;

namespace AK_Services.Interfaces;

public interface ILeaderboardEntriesService
{
    public Task<List<LeaderboardEntryDTO?>> GetLeaderboardEntriesAsync(int leaderboardId, int page = 1, int pageSize = 10);
    public Task<LeaderboardEntryDTO?> GetLeaderboardEntriesByUserAsync(int leaderboardId, int userId);
    public Task<LeaderboardEntries?> CreateLeaderboardEntryAsync(int leaderboardId, LeaderboardEntryDTO entry);
    public Task<LeaderboardEntries?> AddScoreToLeaderboardAsync(int leaderboardId, int userId, int score);
    public Task<LeaderboardEntries?> UpdateLeaderboardEntryAsync(int leaderboardId, LeaderboardEntryDTO entry);
    public Task<bool> DeleteLeaderboardEntryAsync(int leaderboardId, int userId);
    public Task<bool> DeleteLeaderboardEntriesAsync(int leaderboardId);
}