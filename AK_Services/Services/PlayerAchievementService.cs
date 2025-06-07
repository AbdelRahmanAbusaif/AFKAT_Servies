
using AFKAT_Servies;
using AK_Services.DTOS;
using AK_Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Supabase;

public class PlayerAchievementService(Client supabaseClient, IFileService fileService) : IPlayerAchievementService
{
    private readonly Client _supabaseClient = supabaseClient;
    private readonly IFileService _fileService = fileService;


    public Task<List<PlayerAchievementDTO>> GetPlayerAchievementsByPlayerIdAsync(int playerId, int page = 1, int pageSize = 10)
    {
        if (playerId <= 0)
        {
            throw new ArgumentException("Player ID must be greater than zero.", nameof(playerId));
        }

        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var response = _supabaseClient.From<PlayerAchievement>()
            .Select("*")
            .Where(x => x.PlayerId == playerId)
            .Get();

        if (response.Result.Models == null || !response.Result.Models.Any())
        {
            throw new KeyNotFoundException($"No achievements found for player ID {playerId}.");
        }

        var pagedAchievements = response.Result.Models
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        List<Achievements> achievements = new();
        
        foreach (var achievement in pagedAchievements)
        {
            var achievementResponse = _supabaseClient.From<Achievements>()
                .Select("*")
                .Where(x => x.Id == achievement.AchievementId)
                .Get();

            if (achievementResponse.Result.Model != null)
            {
                achievements.Add(achievementResponse.Result.Model);
            }
            else
            {
                throw new KeyNotFoundException($"No achievement found with ID {achievement.AchievementId}.");
            }
        }
        
        List<PlayerAchievementDTO> playerAchievements = pagedAchievements
            .Select(x => new PlayerAchievementDTO
            {
                Id = x.Id,
                PlayerId = x.PlayerId,
                GameId = x.GameId,
                AchievementId = x.AchievementId,
                AchievementName = achievements.FirstOrDefault(a => a.Id == x.AchievementId)?.AchivementName ?? "Unknown Achievement",
                AchievementDescription = achievements.FirstOrDefault(a => a.Id == x.AchievementId)?.AchivementDescription ?? "No description available",
                DateAchieved = x.UnlockedAt,
                AchievementIconURL = x.AchievementIconURL,
                IsCompleted = x.UnlockedAt.Date != DateTime.MinValue
            })
            .ToList();
        
        playerAchievements = playerAchievements
            .OrderByDescending(x => x.IsCompleted)
            .ToList();
        
        return Task.FromResult(playerAchievements);
    }

    public Task<List<PlayerAchievementDTO>> GetPlayerAchievementsByGameIdAsync(int playerId, int gameId, int page = 1, int pageSize = 10)
    {
        if (playerId <= 0)
        {
            throw new ArgumentException("Player ID must be greater than zero.", nameof(playerId));
        }

        if (gameId <= 0)
        {
            throw new ArgumentException("Game ID must be greater than zero.", nameof(gameId));
        }

        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var response = _supabaseClient.From<PlayerAchievement>()
            .Select("*")
            .Where(x => x.PlayerId == playerId && x.GameId == gameId)
            .Get();

        if (response.Result.Models == null || !response.Result.Models.Any())
        {
            throw new KeyNotFoundException($"No achievements found for player ID {playerId} in game ID {gameId}.");
        }

        var pagedAchievements = response.Result.Models
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        List<Achievements> achievements = new();
        
        foreach (var achievement in pagedAchievements)
        {
            var achievementResponse = _supabaseClient.From<Achievements>()
                .Select("*")
                .Where(x => x.Id == achievement.AchievementId)
                .Get();

            if (achievementResponse.Result.Model != null)
            {
                achievements.Add(achievementResponse.Result.Model);
            }
            else
            {
                throw new KeyNotFoundException($"No achievement found with ID {achievement.AchievementId}.");
            }
        }
        
        List<PlayerAchievementDTO> playerAchievements = pagedAchievements
            .Select(x => new PlayerAchievementDTO
            {
                Id = x.Id,
                PlayerId = x.PlayerId,
                GameId = x.GameId,
                AchievementId = x.AchievementId,
                AchievementName = achievements.FirstOrDefault(a => a.Id == x.AchievementId)?.AchivementName ?? "Unknown Achievement",
                AchievementDescription = achievements.FirstOrDefault(a => a.Id == x.AchievementId)?.AchivementDescription ?? "No description available",
                DateAchieved = x.UnlockedAt ,
                AchievementIconURL = x.AchievementIconURL,
                IsCompleted = x.UnlockedAt.Date != DateTime.MinValue
            })
            .ToList();
        
        playerAchievements = playerAchievements
            .OrderByDescending(x => x.IsCompleted)
            .ToList();

        return Task.FromResult(playerAchievements);
    }

    public Task<List<PlayerAchievementDTO>> GetPlayerAchievementByAchievementIdAsync(int id, int page = 1, int pageSize = 20)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Achievement ID must be greater than zero.", nameof(id));
        }

        var response = _supabaseClient.From<PlayerAchievement>()
            .Select("*")
            .Where(x => x.AchievementId == id)
            .Get();
        
        if (response.Result.Models == null || !response.Result.Models.Any())
        {
            throw new KeyNotFoundException($"No achievements found {id}.");
        }
        
        var pgaeModel = response.Result.Models
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        List<Achievements> achievements = new();
        
        foreach (var achievement in pgaeModel)
        {
            var achievementResponse = _supabaseClient.From<Achievements>()
                .Select("*")
                .Where(x => x.Id == achievement.AchievementId)
                .Get();

            if (achievementResponse.Result.Model != null)
            {
                achievements.Add(achievementResponse.Result.Model);
            }
            else
            {
                throw new KeyNotFoundException($"No achievement found with ID {achievement.AchievementId}.");
            }
        }
        
        List<PlayerAchievementDTO> playerAchievements = pgaeModel
            .Select(x => new PlayerAchievementDTO
            {
                Id = id,
                PlayerId = x.PlayerId,
                GameId = x.GameId,
                AchievementId = x.AchievementId,
                AchievementName = achievements.FirstOrDefault(a => a.Id == x.AchievementId)?.AchivementName ?? "Unknown Achievement",
                AchievementDescription = achievements.FirstOrDefault(a => a.Id == x.AchievementId)?.AchivementDescription ?? "No description available",
                DateAchieved = x.UnlockedAt,
                AchievementIconURL = x.AchievementIconURL,
                IsCompleted = x.UnlockedAt.Date != DateTime.MinValue
            })
            .ToList();
        
        playerAchievements = playerAchievements
            .OrderByDescending(x => x.IsCompleted)
            .ToList();
        
        return Task.FromResult(playerAchievements);
    }

    public Task<PlayerAchievement> AddPlayerAchievementAsync(PlayerAchievementDTO playerAchievement)
    {
        if (playerAchievement == null)
        {
            throw new ArgumentNullException(nameof(playerAchievement), "Player achievement cannot be null.");
        }
        
        var achievementResponse = _supabaseClient.From<Achievements>()
            .Select("*")
            .Where(x => x.Id == playerAchievement.AchievementId)
            .Get();

        if (achievementResponse.Result.Model == null || achievementResponse.Result.Model.Id <= 0)
        {
            throw new KeyNotFoundException($"No achievement found with ID {playerAchievement.AchievementId}.");
        }
        
        var newAchievement = new PlayerAchievement
        {
            Id = 0, // Assuming Id is auto-generated by the database
            PlayerId = playerAchievement.PlayerId,
            AchievementId = playerAchievement.AchievementId,
            GameId = playerAchievement.GameId,
            UnlockedAt = playerAchievement.DateAchieved,
            AchievementIconURL = playerAchievement.AchievementIconURL
        };

        var response = _supabaseClient.From<PlayerAchievement>()
            .Insert(newAchievement);

        if (response.Result.Model == null)
        {
            throw new Exception("Failed to add player achievement.");
        }

        return Task.FromResult(response.Result.Model);
    }

    public Task<PlayerAchievement> UpdatePlayerAchievementAsync(PlayerAchievementDTO playerAchievement)
    {
        if (playerAchievement == null)
        {
            throw new ArgumentNullException(nameof(playerAchievement), "Player achievement cannot be null.");
        }

        var existingAchievementResponse = _supabaseClient.From<PlayerAchievement>()
            .Select("*")
            .Where(x => x.AchievementId == playerAchievement.AchievementId)
            .Get();

        if (existingAchievementResponse.Result.Model == null)
        {
            throw new KeyNotFoundException($"No achievement found.");
        }

        var existingAchievement = existingAchievementResponse.Result.Model;
        existingAchievement.PlayerId = playerAchievement.PlayerId;
        existingAchievement.AchievementId = playerAchievement.AchievementId;
        existingAchievement.GameId = playerAchievement.GameId;
        existingAchievement.UnlockedAt = playerAchievement.DateAchieved;
        existingAchievement.AchievementIconURL = playerAchievement.AchievementIconURL;

        var updateResponse = _supabaseClient.From<PlayerAchievement>()
            .Update(existingAchievement);

        if (updateResponse.Result.Model == null)
        {
            throw new ArgumentException("Failed to update player achievement.");
        }

        return Task.FromResult(updateResponse.Result.Model);
    }

    public Task DeletePlayerAchievementAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Achievement ID must be greater than zero.", nameof(id));
        }
        
        var existingAchievementResponse = _supabaseClient.From<PlayerAchievement>()
            .Select("*")
            .Where(x => x.Id == id)
            .Get();

        if (existingAchievementResponse.Result.Model == null)
        {
            throw new KeyNotFoundException($"No achievement found with ID {id}.");
        }
        
        var response = _supabaseClient.From<PlayerAchievement>()
            .Where(x => x.Id == id)
            .Delete();

        return Task.CompletedTask;
    }
}