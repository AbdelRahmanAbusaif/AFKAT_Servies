using AFKAT_Servies;
using AK_Services.Interfaces;

namespace AK_Services.Services;

public class LeaderboardEntriesService(Supabase.Client client) : ILeaderboardEntriesService
{
    private readonly Supabase.Client _supabaseClient = client;
    public Task<List<LeaderboardEntryDTO?>> GetLeaderboardEntriesAsync(int leaderboardId, int page = 1, int pageSize = 10)
    {
        if (leaderboardId <= 0)
        {
            throw new ArgumentException("Leaderboard ID must be greater than zero.", nameof(leaderboardId));
        }
        
        if(page <= 0) page = 1;
        if(pageSize <= 0) pageSize = 10;
        
        var response = _supabaseClient.From<LeaderboardEntries>()
            .Select("*")
            .Where(x => x.LeaderboardId == leaderboardId)
            .Get();

        if (response.Result.Models == null || !response.Result.Models.Any())
        {
            throw new KeyNotFoundException($"No leaderboard entries found for leaderboard ID {leaderboardId}.");
        }
        var pagedEntries = response.Result.Models
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        List<LeaderboardEntryDTO> leaderboardEntries = pagedEntries
            .Select((x, index) => new LeaderboardEntryDTO
            {
                UserId = x.PlayerId,
                Rank = ((page - 1) * pageSize) + index + 1,
                PlayerName = x.PlayerName,
                Score = x.Score
            })
            .ToList();
        
        if (leaderboardEntries == null || leaderboardEntries.Count == 0)
        {
            throw new KeyNotFoundException($"No leaderboard entries found for leaderboard ID {leaderboardId}.");
        }

        return Task.FromResult(leaderboardEntries);
    }

    public Task<LeaderboardEntryDTO?> GetLeaderboardEntriesByUserAsync(int leaderboardId, int userId)
    {
        if (leaderboardId <= 0)
        {
            throw new ArgumentException("Leaderboard ID must be greater than zero.", nameof(leaderboardId));
        }

        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
        }

        var response = _supabaseClient.From<LeaderboardEntries>()
            .Select("*")
            .Where(x => x.LeaderboardId == leaderboardId && x.PlayerId == userId)
            .Get();

        if (response.Result.Model == null)
        {
            throw new KeyNotFoundException($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
        }
        
        LeaderboardEntryDTO leaderboardEntry = new LeaderboardEntryDTO
        {
            UserId = response.Result.Model.PlayerId,
            PlayerName = response.Result.Model.PlayerName,
            Score = response.Result.Model.Score
        };
        
        // Calculate the rank
        var allEntriesResponse = _supabaseClient.From<LeaderboardEntries>().
            Select("*").
            Where(x => x.LeaderboardId == leaderboardId)
            .Get();
        
        if (allEntriesResponse.Result.Models.Any())
        {
            var allEntries = allEntriesResponse.Result.Models
                .OrderByDescending(x => x.Score)
                .ToList();
        
            // Convert the response to a DTO
            leaderboardEntry = new LeaderboardEntryDTO
            {
                UserId = response.Result.Model.PlayerId,
                Rank = allEntries.FindIndex(x => x.PlayerId == userId) + 1,
                PlayerName = response.Result.Model.PlayerName,
                Score = response.Result.Model.Score
            };
        }
        
        if (leaderboardEntry == null)
        {
            throw new KeyNotFoundException($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
        }
        
        return Task.FromResult(leaderboardEntry);
    }

    public Task<LeaderboardEntries?> CreateLeaderboardEntryAsync(int leaderboardId, LeaderboardEntryDTO entry)
    {
        if (leaderboardId <= 0)
        {
            throw new ArgumentException("Leaderboard ID must be greater than zero.", nameof(leaderboardId));
        }

        if (entry == null || entry.UserId <= 0 || string.IsNullOrEmpty(entry.PlayerName) || entry.Score < 0)
        {
            throw new ArgumentException("Invalid entry data.", nameof(entry));
        }

        var newEntry = new LeaderboardEntries
        {
            PlayerId = entry.UserId,
            LeaderboardId = leaderboardId,
            PlayerName = entry.PlayerName,
            Score = entry.Score,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var response = _supabaseClient.From<LeaderboardEntries>()
            .Insert(newEntry);

        if (response.Result.Model == null)
        {
            throw new InvalidOperationException("Failed to create leaderboard entry.");
        }

        return Task.FromResult(response.Result.Model);
    }

    public Task<LeaderboardEntries?> AddScoreToLeaderboardAsync(int leaderboardId, int userId, int score)
    {
        if (leaderboardId <= 0)
        {
            throw new ArgumentException("Leaderboard ID must be greater than zero.", nameof(leaderboardId));
        }

        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
        }

        if (score < 0)
        {
            throw new ArgumentException("Score must be non-negative.", nameof(score));
        }

        var existingEntryResponse = _supabaseClient.From<LeaderboardEntries>()
            .Select("*")
            .Where(x => x.LeaderboardId == leaderboardId && x.PlayerId == userId)
            .Get();

        if (existingEntryResponse.Result.Model == null)
        {
            throw new KeyNotFoundException($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
        }

        var existingEntry = existingEntryResponse.Result.Model;
        existingEntry.Score += score;
        existingEntry.UpdatedAt = DateTime.UtcNow;

        var updateResponse = _supabaseClient.From<LeaderboardEntries>()
            .Update(existingEntry);

        if (updateResponse.Result.Model == null)
        {
            throw new InvalidOperationException("Failed to update leaderboard entry.");
        }

        return Task.FromResult(updateResponse.Result.Model);
        
    }

    public Task<LeaderboardEntries?> UpdateLeaderboardEntryAsync(int leaderboardId, LeaderboardEntryDTO entry)
    {
        if(leaderboardId <= 0)
        {
            throw new ArgumentException("Leaderboard ID must be greater than zero.", nameof(leaderboardId));
        }
        if (entry == null || entry.UserId <= 0 || string.IsNullOrEmpty(entry.PlayerName) || entry.Score < 0)
        {
            throw new ArgumentException("Invalid entry data.", nameof(entry));
        }
        
        var existingEntryResponse = _supabaseClient.From<LeaderboardEntries>()
            .Select("*")
            .Where(x => x.LeaderboardId == leaderboardId && x.PlayerId == entry.UserId)
            .Get();
        if (existingEntryResponse.Result.Model == null)
        {
            throw new KeyNotFoundException($"No entry found for user ID {entry.UserId} in leaderboard ID {leaderboardId}.");
        }
        
        var existingEntry = existingEntryResponse.Result.Model;
        existingEntry.PlayerName = entry.PlayerName;
        existingEntry.Score = entry.Score;
        existingEntry.UpdatedAt = DateTime.UtcNow;
        
        var updateResponse = _supabaseClient.From<LeaderboardEntries>()
            .Update(existingEntry);
        
        if (updateResponse.Result.Model == null)
        {
            throw new InvalidOperationException("Failed to update leaderboard entry.");
        }
        return Task.FromResult(updateResponse.Result.Model);
    }

    public Task DeleteLeaderboardEntryAsync(int leaderboardId, int userId)
    {
        if (leaderboardId <= 0)
        {
            throw new ArgumentException("Leaderboard ID must be greater than zero.", nameof(leaderboardId));
        }

        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
        }

        var existingEntryResponse = _supabaseClient.From<LeaderboardEntries>()
            .Select("*")
            .Where(x => x.LeaderboardId == leaderboardId && x.PlayerId == userId)
            .Get();

        if (existingEntryResponse.Result.Model == null)
        {
            throw new KeyNotFoundException($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
        }

        var deleteResponse = _supabaseClient.From<LeaderboardEntries>()
            .Where(x=> x.LeaderboardId == leaderboardId && x.PlayerId == userId)
            .Delete();
        
        return Task.FromResult(true);
    }

    public Task DeleteLeaderboardEntriesAsync(int leaderboardId)
    {
        if(leaderboardId <= 0)
        {
            throw new ArgumentException("Leaderboard ID must be greater than zero.", nameof(leaderboardId));
        }
        
        var existingEntriesResponse = _supabaseClient.From<LeaderboardEntries>()
            .Select("*")
            .Where(x => x.LeaderboardId == leaderboardId)
            .Get();
        
        if (existingEntriesResponse.Result.Models == null || !existingEntriesResponse.Result.Models.Any())
        {
            throw new KeyNotFoundException($"No entries found for leaderboard ID {leaderboardId}.");
        }
        
        var deleteResponse = _supabaseClient.From<LeaderboardEntries>()
            .Where(x => x.LeaderboardId == leaderboardId)
            .Delete();
        
        return Task.FromResult(true);
    }
}