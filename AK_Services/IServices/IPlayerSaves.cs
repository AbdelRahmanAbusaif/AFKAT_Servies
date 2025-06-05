using AFKAT_Servies;
using AK_Services.DTOS;

namespace AK_Services.Interfaces;

public interface IPlayerSaves
{
    public Task<PlayerSavesDTO> GetPlayerSavesAsync(int playerId,int gameId, string? fileName = null);
    public Task<PlayerSaves> SavePlayerSavesAsync(PlayerSavesDTO playerSaves);
    public Task<PlayerSaves> UpdatePlayerSavesAsync(PlayerSavesDTO playerSavesDTO = null!);
    public Task DeletePlayerSavesAsync(int playerId, int gameId, string? fileName );
}