using AK_Services.DTOS;
using AK_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AFKAT_Servies.Controllers;

[ApiController]
[Route("afk_services/afk_player_achievements")]
public class PlayerAchievementController(ILogger<PlayerAchievementController> logger , IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly ILogger<PlayerAchievementController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    
    [HttpGet]
    [Route("player/{playerId}")]
    public IActionResult GetPlayerAchievements(int playerId, int page = 1, int pageSize = 20)
    {
        try
        {
            var response = _unitOfWork.PlayerAchievements.GetPlayerAchievementsByPlayerIdAsync(playerId, page, pageSize);
            if (response.Result.Count == 0)
            {
                return NotFound($"No achievements found for player ID {playerId}.");
            }

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = response.Result.Count,
                Entries = response.Result
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error retrieving achievements for player ID {PlayerId}", playerId);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "No achievements found for player ID {PlayerId}", playerId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving achievements for player ID {PlayerId}", playerId);
            return StatusCode(500, "An unexpected error occurred." + ex.Message );
        }
    }
    [HttpGet]
    [Route("game/{gameId}/player/{playerId}")]
    public IActionResult GetPlayerAchievementsByGameId(int playerId, int gameId, int page = 1, int pageSize = 20)
    {
        try
        {
            var response = _unitOfWork.PlayerAchievements.GetPlayerAchievementsByGameIdAsync(playerId, gameId, page, pageSize);
            if (response.Result.Count == 0)
            {
                return NotFound($"No achievements found for player ID {playerId} in game ID {gameId}.");
            }

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = response.Result.Count,
                Entries = response.Result
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error retrieving achievements for player ID {PlayerId} in game ID {GameId}", playerId, gameId);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "No achievements found for player ID {PlayerId} in game ID {GameId}", playerId, gameId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving achievements for player ID {PlayerId} in game ID {GameId}", playerId, gameId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet]
    [Route("achievement/{achievementId}")]
    public IActionResult GetPlayerAchievementByAchievementId(int achievementId , int page = 1, int pageSize = 20)
    {
        try
        {
            var response = _unitOfWork.PlayerAchievements.GetPlayerAchievementByAchievementIdAsync(achievementId, page, pageSize);
            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = response.Result.Count,
                Entries = response.Result
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error retrieving achievement with ID {AchievementId}", achievementId);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "No achievement found with ID {AchievementId}", achievementId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving achievement with ID {AchievementId}", achievementId);
            return StatusCode(500, "An unexpected error occurred.");
        }
        
    }
    [HttpPost]
    [Route("")]
    public IActionResult AddPlayerAchievement([FromBody] PlayerAchievementDTO playerAchievement)
    {
        try
        {
            var response = _unitOfWork.PlayerAchievements.AddPlayerAchievementAsync(playerAchievement);
            return CreatedAtAction(nameof(GetPlayerAchievementByAchievementId), new { achievementId = response.Result.Id }, response.Result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error adding player achievement");
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "No achievement found for player achievement");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding player achievement");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    [HttpPut]
    [Route("{playerAchievementId}")]
    public IActionResult UpdatePlayerAchievement(int playerAchievementId, [FromBody] PlayerAchievementDTO playerAchievement)
    {
        try
        {
            var response = _unitOfWork.PlayerAchievements.UpdatePlayerAchievementAsync(playerAchievementId, playerAchievement);
            return Ok(response.Result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error updating player achievement with ID {Id}", playerAchievementId);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "No achievement found with ID {Id}", playerAchievementId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating player achievement with ID {Id}", playerAchievementId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
    [HttpDelete]
    [Route("{playerAchievementId}")]
    public IActionResult DeletePlayerAchievement(int playerAchievementId)
    {
        try
        {
            _unitOfWork.PlayerAchievements.DeletePlayerAchievementAsync(playerAchievementId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error deleting player achievement with ID {Id}", playerAchievementId);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "No achievement found with ID {Id}", playerAchievementId);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting player achievement with ID {Id}", playerAchievementId);
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}