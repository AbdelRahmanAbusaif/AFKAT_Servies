using AK_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AFKAT_Servies.Controllers
{
    [ApiController]
    [Route("afk_services")]
    public class LeaderboardController(ILogger<LeaderboardController> logger, IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly ILogger<LeaderboardController> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        [Route("afk_leaderboard/{leaderboardId}")]
        public IActionResult GetLeaderboard(int leaderboardId)
        {
            try
            {
                var leaderboard = _unitOfWork.Leaderboards.GetLeaderboardAsync(leaderboardId);
                if (leaderboard == null)
                {
                    return NotFound($"Leaderboard with ID {leaderboardId} not found.");
                }
                return Ok(leaderboard.Result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error retrieving leaderboard with ID {LeaderboardId}", leaderboardId);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Leaderboard with ID {LeaderboardId} not found", leaderboardId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving leaderboard with ID {LeaderboardId}", leaderboardId);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet]
        [Route("afk_leaderboard")]
        public IActionResult GetLeaderboards(int page = 1, int pageSize = 20)
        {
            try
            {
                var leaderboard = _unitOfWork.Leaderboards.GetLeaderboardsAsync(page, pageSize);
                if (leaderboard == null || !leaderboard.Result.Any())
                {
                    return NotFound("No leaderboards found.");
                }

                return Ok(new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = leaderboard.Result.Count,
                    Entries = leaderboard.Result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error retrieving leaderboards");
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "No leaderboards found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving leaderboards");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("afk_leaderboard/game/{gameId}")]
        public IActionResult GetLeaderboardBygGames(int page,int pageSize,int gameId)
        {
            try
            {
                var leaderboard = _unitOfWork.Leaderboards.GetLeaderboardByGameIdAsync(gameId, page, pageSize);
                if (leaderboard == null)
                {
                    return NotFound($"No leaderboards found for game ID {gameId}.");
                }

                return Ok(new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = leaderboard.Result.Count,
                    Entries = leaderboard.Result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error retrieving leaderboards for game ID {GameId}", gameId);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "No leaderboards found for game ID {GameId}", gameId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving leaderboards for game ID {GameId}", gameId);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpPost]
        [Route("afk_leaderboard")]
        public IActionResult CreateLeaderboard([FromBody] LeaderboardDTO leaderboardDto)
        {
            try
            {
                var response = _unitOfWork.Leaderboards.CreateLeaderboardAsync(leaderboardDto);
                if (response == null)
                {
                    return BadRequest("Failed to create leaderboard.");
                }
                return CreatedAtAction(nameof(GetLeaderboard), new { leaderboardId = response.Result.Id }, response.Result);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Invalid operation while creating leaderboard");
                return BadRequest("Invalid operation: " + e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating leaderboard");
                return StatusCode(500, "Internal server error: " + e.Message);
            }
        }
        [HttpPut]
        [Route("afk_leaderboard/{leaderboardId}")]
        public IActionResult UpdateLeaderboard([FromBody] LeaderboardDTO leaderboardDto)
        {
            try
            {
                var response = _unitOfWork.Leaderboards.UpdateLeaderboardAsync(leaderboardDto);
                if (response == null)
                {
                    return NotFound("Leaderboard not found or update failed.");
                }
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid argument while updating leaderboard");
                return BadRequest("Invalid argument: " + e.Message);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Invalid operation while updating leaderboard");
                return BadRequest("Invalid operation: " + e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while updating leaderboard");
                return StatusCode(500, "Internal server error: " + e.Message);
            }
            
            return NoContent();
        }
        [HttpDelete]
        [Route("afk_leaderboard/{leaderboardId}")]
        public IActionResult DeleteLeaderboard(int leaderboardId)
        {
            try
            {
                var response = _unitOfWork.Leaderboards.DeleteLeaderboardAsync(leaderboardId);
                if (response == null)
                {
                    return NotFound($"Leaderboard with ID {leaderboardId} not found.");
                }
                
                return NoContent();
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid argument while deleting leaderboard");
                return BadRequest("Invalid argument: " + e.Message);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError(e, "Invalid leaderboard id");
                return NotFound("Leaderboard not found: " + e.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting leaderboard with ID {LeaderboardId}", leaderboardId);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
	}
}
