namespace AK_Services.Interfaces;

public interface ILeaderboardService
{
    public Task<List<LeaderboardDTO>> GetLeaderboardsAsync(int page = 1, int pageSize = 10);
    public Task<LeaderboardDTO> GetLeaderboardAsync(int id);
    public Task<List<LeaderboardDTO>> GetLeaderboardByGameIdAsync(int gameId, int page = 1, int pageSize = 10);
    public Task<LeaderboardDTO> CreateLeaderboardAsync(LeaderboardDTO leaderboard);
    public Task<LeaderboardDTO> UpdateLeaderboardAsync(LeaderboardDTO leaderboard);
    public Task DeleteLeaderboardAsync(int id);
}