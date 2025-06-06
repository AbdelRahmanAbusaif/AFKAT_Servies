using AK_Services.DTOS;
using AK_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AFKAT_Servies.Controllers;

[ApiController]
[Route("afk_services/afk_player_saves")]
public class PlayerSavesController(ILogger<PlayerAchievementController> logger , IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly ILogger<PlayerAchievementController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    
    [HttpGet]
    [Route("player/{playerId}/game/{gameId}/filename/{fileName}")]
    public async Task<IActionResult> GetPlayerSaves(int playerId, int gameId , string fileName)
    {
        try
        {
            var response = await _unitOfWork.PlayerSaves.GetPlayerSavesAsync(playerId, gameId , fileName);
            return Ok(new
            {
                PlayerId = response.PlayerId,
                GameId = response.GameId,
                FileSaveURL = response.FileSaveURL
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error retrieving saves for player ID {PlayerId} in game ID {GameId}", playerId, gameId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving saves for player ID {PlayerId} in game ID {GameId} with file name {FileName}", playerId, gameId , fileName);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> SavePlayerSaves([FromForm] PlayerSavesDTO playerSaves)
    {
        try
        {
            var response = await _unitOfWork.PlayerSaves.SavePlayerSavesAsync(playerSaves);
            return CreatedAtAction(nameof(GetPlayerSaves), new { playerId = response.PlayerId, gameId = response.GameId , fileName = response.FileName }, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error saving player saves for player ID {PlayerId} in game ID {GameId}", playerSaves.PlayerId, playerSaves.GameId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving player saves for player ID {PlayerId} in game ID {GameId}", playerSaves.PlayerId, playerSaves.GameId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    [HttpPatch]
    [Route("")]
    public async Task<IActionResult> UpdatePlayerSaves([FromForm] PlayerSavesDTO playerSaves)
    {
        try
        {
            var response = await _unitOfWork.PlayerSaves.UpdatePlayerSavesAsync(playerSaves);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "No saves found for player ID {PlayerId} in game ID {GameId} with file name {FileName}", playerSaves.PlayerId, playerSaves.GameId, playerSaves.FileSave?.FileName);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating saves for player ID {PlayerId} in game ID {GameId}", playerSaves.PlayerId, playerSaves.GameId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    [HttpDelete]
    [Route("player/{playerId}/game/{gameId}/filename/{fileName?}")]
    public async Task<IActionResult> DeletePlayerSaves(int playerId, int gameId, string? fileName)
    {
        try
        {
            await _unitOfWork.PlayerSaves.DeletePlayerSavesAsync(playerId, gameId, fileName);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "No saves found for player ID {PlayerId} in game ID {GameId}", playerId, gameId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting saves for player ID {PlayerId} in game ID {GameId}", playerId, gameId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}