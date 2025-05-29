namespace AK_Services.Interfaces;

public interface IAchivementService
{
    public Task<List<AchivementsDTO>> GetAchivementsAsync(int gameId,int page = 1, int pageSize = 10);
    public Task<AchivementsDTO> GetAchivementAsync(int id);
}