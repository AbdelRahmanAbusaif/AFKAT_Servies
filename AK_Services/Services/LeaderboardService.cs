using AFKAT_Servies;
using AK_Services.Interfaces;
using Supabase;

namespace AK_Services.Services;

public class LeaderboardService(Client client) : ILeaderboardService
{
    private readonly Client _supabaseClient = client;
    
    public Task<List<LeaderboardDTO>> GetLeaderboardsAsync(int page = 1, int pageSize = 10)
    {
        var response = _supabaseClient.From<Leaderboards>()
            .Select("*")
            .Get();

        if (response.Result.Models == null || !response.Result.Models.Any())
        {
            throw new ArgumentException("No leaderboards found.");
        }
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        
        var pagedModels = response.Result.Models
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        List<LeaderboardDTO> leaderboards = pagedModels.Select(x => new LeaderboardDTO
        {
            Id = x.Id,
            GameId = x.GameId,
            LeaderboardName = x.LeaderboardName
        }).ToList();
        
        return Task.FromResult(leaderboards);
    }

    public Task<LeaderboardDTO> GetLeaderboardAsync(int leadbaordId)
    {
        if(leadbaordId <= 0)
        {
            throw new ArgumentException("Leaderboard ID must be greater than zero.");
        }

        var response = _supabaseClient.From<Leaderboards>()
            .Select("*")
            .Where(x => x.Id == leadbaordId)
            .Get();

        if (response.Result.Models == null || !response.Result.Models.Any())
        {
            throw new ArgumentException($"Leaderboard with ID {leadbaordId} not found.");
        }

        var leaderboard = response.Result.Model;
        LeaderboardDTO leaderboardDTO = new LeaderboardDTO
        {
            Id = leaderboard.Id,
            GameId = leaderboard.GameId,
            LeaderboardName = leaderboard.LeaderboardName
        };
        return Task.FromResult(leaderboardDTO);
    }

    public Task<List<LeaderboardDTO>> GetLeaderboardByGameIdAsync(int gameId, int page = 1, int pageSize = 10)
    {
        if (gameId <= 0)
        {
            throw new ArgumentException("Game ID must be greater than zero.");
        }

        var response = _supabaseClient.From<Leaderboards>()
            .Select("*")
            .Where(x => x.GameId == gameId)
            .Get();

        if (response.Result.Models == null || !response.Result.Models.Any())
        {
            throw new ArgumentException($"No leaderboards found for game ID {gameId}.");
        }

        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var pagedModels = response.Result.Models
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        List<LeaderboardDTO> leaderboards = pagedModels.Select(x => new LeaderboardDTO
        {
            Id = x.Id,
            GameId = x.GameId,
            LeaderboardName = x.LeaderboardName
        }).ToList();

        return Task.FromResult(leaderboards);
    }

    public Task<LeaderboardDTO> CreateLeaderboardAsync(LeaderboardDTO leaderboard)
    {
        if(leaderboard.Id <= 0)
        {
            throw new ArgumentException("Invalid leaderboard data.");
        }
        
        var response = _supabaseClient.From<Leaderboards>()
            .Insert(new Leaderboards
            {
                GameId = leaderboard.GameId,
                LeaderboardName = leaderboard.LeaderboardName
            });
        
        if (response.Result.Model == null)
        {
            throw new InvalidOperationException("Failed to create leaderboard.");
        }
        
        LeaderboardDTO createdLeaderboard = new LeaderboardDTO
        {
            Id = response.Result.Model.Id,
            GameId = response.Result.Model.GameId,
            LeaderboardName = response.Result.Model.LeaderboardName
        };
        return Task.FromResult(createdLeaderboard);
    }

    public Task<LeaderboardDTO> UpdateLeaderboardAsync(LeaderboardDTO leaderboard)
    {
        if (leaderboard.Id <= 0)
        {
            throw new ArgumentException("Invalid leaderboard ID.");
        }

        var extistingLeaderboard = _supabaseClient.From<Leaderboards>()
            .Select("*")
            .Where(x => x.Id == leaderboard.Id)
            .Get();
        
        if (extistingLeaderboard.Result.Model == null)
        {
            throw new ArgumentException($"Leaderboard with ID {leaderboard.Id} not found.");
        }

        var updatedLeaderboard = _supabaseClient.From<Leaderboards>()
            .Update(new Leaderboards
            {
                Id = leaderboard.Id,
                GameId = leaderboard.GameId,
                LeaderboardName = leaderboard.LeaderboardName
            });

        if (updatedLeaderboard.Result.Model == null)
        {
            throw new InvalidOperationException("Failed to update leaderboard.");
        }
        LeaderboardDTO updatedLeaderboardDTO = new LeaderboardDTO
        {
            Id = updatedLeaderboard.Result.Model.Id,
            GameId = updatedLeaderboard.Result.Model.GameId,
            LeaderboardName = updatedLeaderboard.Result.Model.LeaderboardName
        };
        return Task.FromResult(updatedLeaderboardDTO);
    }

    public Task DeleteLeaderboardAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Leaderboard ID must be greater than zero.");
        }

        var response = _supabaseClient.From<Leaderboards>()
            .Delete();

        if (response == null)
        {
            return Task.FromResult(false);
        }
        
        return Task.FromResult(true);
    }
}