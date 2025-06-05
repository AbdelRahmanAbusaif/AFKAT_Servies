using AFKAT_Servies;
using AK_Services.DTOS;
using AK_Services.Interfaces;
namespace AK_Services.Services;

public class PlayerSavesService(Supabase.Client supabaceClient, IFileService fileService ) : IPlayerSaves
{
    private readonly Supabase.Client _supabaseClient = supabaceClient;
    private readonly IFileService _fileService = fileService;
    public Task<PlayerSavesDTO> GetPlayerSavesAsync(int playerId, int gameId, string? fileName)
    {
        if (playerId <= 0 || gameId <= 0)
        {
            throw new ArgumentException("Player ID and Game ID must be greater than zero.");
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.");
        }
        
        fileName = fileName.ToLower();

        Console.WriteLine($"Fetching saves for player ID: {playerId}, game ID: {gameId}, file name: {fileName}");
        var response = _supabaseClient.From<PlayerSaves>()
            .Select("*")
            .Where(x => x.PlayerId == playerId && x.GameId == gameId )
            .Filter("FileName", Supabase.Postgrest.Constants.Operator.Like, fileName) // Using Filter directly
            .Get();

        if (response.Result.Model == null)
        {
            throw new KeyNotFoundException($"No saves found for player ID {playerId} in game ID {gameId} with file name {fileName}.");
        }
        
        var playerSavesDTO = new PlayerSavesDTO
        {
            PlayerId = response.Result.Model.PlayerId,
            GameId = response.Result.Model.GameId,
            FileSaveURL = response.Result.Model.FileSaveURL
        };
        
        return Task.FromResult(playerSavesDTO);
    }

    public async Task<PlayerSaves> SavePlayerSavesAsync(PlayerSavesDTO playerSaves)
    {
        if (playerSaves == null)
        {
            throw new ArgumentNullException(nameof(playerSaves), "Player saves cannot be null.");
        }

        if (playerSaves.PlayerId <= 0 || playerSaves.GameId <= 0)
        {
            throw new ArgumentException("Player ID and Game ID must be greater than zero.");
        }
        
        if (playerSaves.FileSave == null || playerSaves.FileSave.Length == 0)
        {
            throw new ArgumentException("File save cannot be null or empty.");
        }
        
        var fileUrl = await _fileService.SaveFileAsync(playerSaves.FileSave,"saves/player_" + playerSaves.PlayerId + "/game_" + playerSaves.GameId);
        var playerSave = new PlayerSaves
        {
            PlayerId = playerSaves.PlayerId,
            GameId = playerSaves.GameId,
            FileSaveURL = fileUrl,
            FileName = playerSaves.FileSave?.FileName.ToLower() ?? string.Empty,
            UpdatedAt = DateTime.UtcNow
        };

        var response = _supabaseClient.From<PlayerSaves>()
            .Insert(playerSave);

        if (response.Result.Model == null)
        {
            throw new Exception($"Error saving player saves for player ID {playerSaves.PlayerId} in game ID {playerSaves.GameId}.");
        }

        return playerSave;
    }

    
    public Task DeletePlayerSavesAsync(int playerId, int gameId, string? fileName)
    {
        if (playerId <= 0 || gameId <= 0)
        {
            throw new ArgumentException("Player ID and Game ID must be greater than zero.");
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.");
        }

        var res = _supabaseClient.From<PlayerSaves>()
            .Select("*")
            .Where(x => x.PlayerId == playerId && x.GameId == gameId)
            .Filter("FileName", Supabase.Postgrest.Constants.Operator.Like, fileName.ToLower())
            .Get();

        if (res.Result.Model == null)
        {
            throw new KeyNotFoundException($"No saves found for player ID {playerId} in game ID {gameId} with file name {fileName}.");
        }

        fileName = fileName.ToLower();

        var response = _supabaseClient.From<PlayerSaves>()
            .Where(x => x.PlayerId == playerId && x.GameId == gameId)
            .Filter("FileName", Supabase.Postgrest.Constants.Operator.Like, fileName)
            .Delete();

        Console.WriteLine($"Delete response: {res}");
        fileService.DeleteFileAsync("saves/player_" + playerId + "/game_" + gameId + "/" + fileName);

        return Task.FromResult(true);
    }

    public Task<PlayerSaves> UpdatePlayerSavesAsync(PlayerSavesDTO playerSaves = null)
    {
        if (playerSaves == null)
        {
            throw new ArgumentNullException(nameof(playerSaves), "Player saves cannot be null.");
        }
        if (playerSaves.PlayerId <= 0 || playerSaves.GameId <= 0)
        {
            throw new ArgumentException("Player ID and Game ID must be greater than zero.");
        }
        
        if (playerSaves.FileSave == null || playerSaves.FileSave.Length == 0)
        {
            throw new ArgumentException("File save cannot be null or empty.");
        }
        var fileName = playerSaves.FileSave?.FileName.ToLower() ?? string.Empty;

        var existingSave = _supabaseClient.From<PlayerSaves>()
            .Select("*")
            .Where(x => x.PlayerId == playerSaves.PlayerId && x.GameId == playerSaves.GameId)
            .Filter("FileName", Supabase.Postgrest.Constants.Operator.Like, fileName)
            .Get();

        if (existingSave.Result.Model == null)
        {
            throw new KeyNotFoundException($"No saves found for player ID {playerSaves.PlayerId} in game ID {playerSaves.GameId} with file name {fileName}.");
        }

        _fileService.DeleteFileAsync("saves/player_" + playerSaves.PlayerId + "/game_" + playerSaves.GameId + "/" + fileName);
        var fileUrl = _fileService.SaveFileAsync(playerSaves.FileSave, "saves/player_" + playerSaves.PlayerId + "/game_" + playerSaves.GameId).Result;

        var updatedSave = new PlayerSaves
        {
            Id = existingSave.Result.Model.Id,
            PlayerId = playerSaves.PlayerId,
            GameId = playerSaves.GameId,
            FileName = playerSaves.FileSave?.FileName.ToLower() ?? string.Empty,
            FileSaveURL = fileUrl, // Assuming URL remains the same
            UpdatedAt = DateTime.UtcNow
        };

        var response = _supabaseClient.From<PlayerSaves>()
            .Where(x => x.Id == existingSave.Result.Model.Id)
            .Update(updatedSave);

        if (response.Result.Model == null)
        {
            throw new Exception($"Error updating player saves for player ID {playerSaves.PlayerId} in game ID {playerSaves.GameId}.");
        }
        return Task.FromResult(response.Result.Model);
    }
}