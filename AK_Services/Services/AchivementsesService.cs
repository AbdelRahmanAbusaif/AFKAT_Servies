using AFKAT_Servies;
using AK_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Supabase;

namespace AK_Services.Services;

public class AchivementsService(Client client,IFileService fileService) : IAchivementsService
{
    private readonly Client _supabaseClient = client;
    private readonly IFileService _fileService = fileService;
    public Task<List<AchivementsDTO>> GetAchivementsAsync(int gameId, int page = 1, int pageSize = 10)
    {
        if(gameId <= 0)
        {
            throw new ArgumentException("Invalid game ID", nameof(gameId));
        }
        
        if(page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        
        var response = _supabaseClient.From<Achievements>()
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

        var response = _supabaseClient.From<Achievements>()
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

    public Task<AchivementsDTO> CreateAchivementAsync(AchivementsDTO achivement, IFormFile file)
    {
        if (achivement == null)
        {
            throw new ArgumentNullException(nameof(achivement), "Achivement cannot be null.");
        }

        if (file == null || (file.ContentType != "image/png" && file.ContentType != "image/jpeg"))
        {
            throw new ArgumentException("Invalid file type. Only PNG and JPEG are allowed.", nameof(file));
        }
        
        var achivementModel = new Achievements
        {
            GameId = achivement.GameId,
            AchivementName = achivement.Name,
            AchivementDescription = achivement.Description,
            AchivementIconURL = _fileService.SaveFileAsync(file).Result
        };
        
        var response = _supabaseClient.From<Achievements>()
            .Insert(achivementModel);

        if (response.Result.Model == null)
        {
            throw new InvalidOperationException("Failed to create achivement.");
        }

        AchivementsDTO createdAchivement = new()
        {
            Id = response.Result.Model.Id,
            GameId = response.Result.Model.GameId,
            Name = response.Result.Model.AchivementName,
            Description = response.Result.Model.AchivementDescription,
            ImageUrl = response.Result.Model.AchivementIconURL,
            Image = file
        };

        return Task.FromResult(createdAchivement);
    }
    public Task<AchivementsDTO> UpdateAchivementAsync(AchivementsDTO achivement, IFormFile? file = null)
    {
        if (achivement == null)
        {
            throw new ArgumentNullException(nameof(achivement), "Achivement cannot be null.");
        }

        var existingAchivement = _supabaseClient.From<Achievements>()
            .Select("*")
            .Where(x => x.Id == achivement.Id)
            .Get();

        if (existingAchivement.Result.Model == null)
        {
            throw new ArgumentException($"No achivement found with ID {achivement.Id}.");
        }
        
        if (file != null && (file.ContentType != "image/png" && file.ContentType != "image/jpeg"))
        {
            throw new ArgumentException("Invalid file type. Only PNG and JPEG are allowed.", nameof(file));
        }

        existingAchivement.Result.Model.AchivementName = achivement.Name;
        existingAchivement.Result.Model.AchivementDescription = achivement.Description;

        if (file != null)
        {
            fileService.DeleteFileAsync(existingAchivement.Result.Model.AchivementIconURL);
            existingAchivement.Result.Model.AchivementIconURL = _fileService.SaveFileAsync(file).Result;
        }

        var updateResponse = _supabaseClient.From<Achievements>()
            .Update(existingAchivement.Result.Model);

        if (updateResponse.Result.Model == null)
        {
            throw new InvalidOperationException("Failed to update achivement.");
        }

        AchivementsDTO updatedAchivement = new()
        {
            Id = updateResponse.Result.Model.Id,
            GameId = updateResponse.Result.Model.GameId,
            Name = updateResponse.Result.Model.AchivementName,
            Description = updateResponse.Result.Model.AchivementDescription,
            ImageUrl = updateResponse.Result.Model.AchivementIconURL
        };

        return Task.FromResult(updatedAchivement);
    }
    public Task DeleteAchivementAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid achivement ID", nameof(id));
        }
        
        var existingAchivement = _supabaseClient.From<Achievements>()
            .Select("*")
            .Where(x => x.Id == id)
            .Get();
        
        if (existingAchivement.Result.Model == null)
        {
            throw new KeyNotFoundException($"No achivement found with ID {id}.");
        }
        
        var response = _supabaseClient.From<Achievements>()
            .Where(x => x.Id == id)
            .Delete();

        fileService.DeleteFileAsync(existingAchivement.Result.Model.AchivementIconURL).Wait();
        
        return Task.FromResult(true);
    }
    
}