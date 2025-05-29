using AFKAT_Servies;
using AK_Services.Interfaces;
using Supabase;

namespace AK_Services.Services;

public class AchivementsService(Client client) : IAchivementService
{
    private readonly Client _supabaseClient = client;
    public Task<List<AchivementsDTO>> GetAchivementsAsync(int gameId, int page = 1, int pageSize = 10)
    {
        if(gameId <= 0)
        {
            throw new ArgumentException("Invalid game ID", nameof(gameId));
        }
        
        if(page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        
        var response = _supabaseClient.From<Achivements>()
            .Select("*")
            .Where(x => x.GameId == gameId)
            .Get();

        if (response.Result.Model == null)
        {
            throw new ArgumentException($"No achivements found for game ID {gameId}.");
        }
        
        var pagedResults = response.Result.Models
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        List<AchivementsDTO> achivements = pagedResults.Select(x => new AchivementsDTO
        {
            Id = x.Id,
            GameId = x.GameId,
            Name = x.AchivementName,
            Description = x.AchivementDescription,
            ImageUrl = x.AchivementIconURL
        }).ToList();
        
        return Task.FromResult(achivements);
    }

    public Task<AchivementsDTO> GetAchivementAsync(int gameId)
    {
        if (gameId <= 0)
        {
            throw new ArgumentException("Invalid game ID", nameof(gameId));
        }

        var response = _supabaseClient.From<Achivements>()
            .Select("*")
            .Where(x => x.GameId == gameId)
            .Get();

        if (response.Result.Model == null)
        {
            throw new ArgumentException($"No achivement found for game ID {gameId}.");
        }

        AchivementsDTO achivement = new()
        {
            Id = response.Result.Model.Id,
            GameId = response.Result.Model.GameId,
            Name = response.Result.Model.AchivementName,
            Description = response.Result.Model.AchivementDescription,
            ImageUrl = response.Result.Model.AchivementIconURL
        };

        return Task.FromResult(achivement);
    }
}