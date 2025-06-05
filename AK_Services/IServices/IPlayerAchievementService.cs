using AFKAT_Servies;
using AK_Services.DTOS;

public interface IPlayerAchievementService
{
    public Task<List<PlayerAchievementDTO>> GetPlayerAchievementsByPlayerIdAsync(int playerId, int page = 1, int pageSize = 10);
    public Task<List<PlayerAchievementDTO>> GetPlayerAchievementsByGameIdAsync(int playerId,int gameId, int page = 1, int pageSize = 10);
    public Task<List<PlayerAchievementDTO>> GetPlayerAchievementByAchievementIdAsync(int id, int playerId, int gameId);
    public Task<PlayerAchievement> AddPlayerAchievementAsync(PlayerAchievementDTO playerAchievement);
    public Task<PlayerAchievement> UpdatePlayerAchievementAsync(int id, PlayerAchievementDTO playerAchievement);
    public Task DeletePlayerAchievementAsync(int id);
}