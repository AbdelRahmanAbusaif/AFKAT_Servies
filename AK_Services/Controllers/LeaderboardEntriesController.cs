using AK_Services.Interfaces;
using AK_Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace AFKAT_Servies.Controllers
{
    [ApiController]
    [Route("afk_services")]
    public class LeaderboardEntriesController(ILogger<LeaderboardEntriesController> logger, IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly ILogger<LeaderboardEntriesController> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        [Route("afk_leaderboard_entries/{leaderboardId}")]
        public IActionResult GetEntries(int leaderboardId, int page = 1, int pageSize = 20)
        {
            try
            {
                var leaderboardEntriesService = _unitOfWork.LeaderboardEntries.GetLeaderboardEntriesAsync(leaderboardId, page, pageSize);
                return Ok(new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = leaderboardEntriesService.Result.Count,
                    Entries = leaderboardEntriesService.Result
                });
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid argument provided while retrieving leaderboard entries.");
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError(e, "Leaderboard entries not found.");
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving leaderboard entries.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        [Route("afk_leaderboard_entries/{leaderboardId}/user/{userId}")]
        public IActionResult GetUserEntry(int leaderboardId, int userId)
        {
            try
            {
                var entry = _unitOfWork.LeaderboardEntries.GetLeaderboardEntriesByUserAsync(leaderboardId, userId);
                if (entry == null)
                {
                    return NotFound($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
                }
                return Ok(entry.Result);
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e," Invalid argument provided while retrieving leaderboard entry.");
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError(e," Leaderboard entry not found.");
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e," An error occurred while retrieving leaderboard entry.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        [Route("afk_leaderboard_entries/{leaderboardId}")]
        public IActionResult CreateEntry(int leaderboardId,[FromBody] LeaderboardEntryDTO entryDto)
        {
            try
            {
                var leaderboard = _unitOfWork.LeaderboardEntries.CreateLeaderboardEntryAsync(leaderboardId, entryDto);
                if (leaderboard == null)
                {
                    return BadRequest("Failed to create leaderboard entry.");
                }
                return CreatedAtAction(nameof(GetUserEntry), new { leaderboardId = leaderboard.Result.LeaderboardId, userId = leaderboard.Result.PlayerId }, leaderboard.Result);
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid argument provided while creating leaderboard entry.");
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError(e, "Leaderboard not found.");
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating leaderboard entry.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        [Route("afk_leaderboard_entries/{leaderboardId}/{userId}")]
        public IActionResult AddScore(int leaderboardId, int userId, [FromBody] int scoreToAdd)
        {
            try
            {
                var leaderboardEntry = _unitOfWork.LeaderboardEntries.AddScoreToLeaderboardAsync(leaderboardId, userId, scoreToAdd);
                if (leaderboardEntry == null)
                {
                    return BadRequest("Failed to add score to leaderboard entry.");
                }
                return Ok(leaderboardEntry.Result);
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid argument provided while adding score to leaderboard entry.");
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError(e, "Leaderboard entry not found.");
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Failed to add score to leaderboard entry due to an invalid operation.");
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while adding score to leaderboard entry.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut]
        [Route("afk_leaderboard_entries/{leaderboardId}/{userId}")]
        public IActionResult UpdateEntry(int leaderboardId, int userId, [FromBody] LeaderboardEntryDTO entryDto)
        {
            try
            {

                var updatedEntry = _unitOfWork.LeaderboardEntries.UpdateLeaderboardEntryAsync(leaderboardId, entryDto);
                return Ok(updatedEntry.Result);
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid argument provided while adding score to leaderboard entry.");
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError(e, "Leaderboard entry not found.");
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Failed to update leaderboard entry due to an invalid operation.");
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while adding score to leaderboard entry.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete]
        [Route("afk_leaderboard_entries/{leaderboardId}/{userId}")]
        public IActionResult DeleteEntry(int leaderboardId, int userId)
        {
            try
            {
                var deleted = _unitOfWork.LeaderboardEntries.DeleteLeaderboardEntryAsync(leaderboardId, userId);
                return NoContent();
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid argument provided while deleting entry.");
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError(e, "Leaderboard entry not found.");
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while deleting leaderboard entry.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete]
        [Route("afk_leaderboard_entries/{leaderboardId}")]
        public IActionResult DeleteEntries(int leaderboardId)
        {
            try
            {
                var deleted = _unitOfWork.LeaderboardEntries.DeleteLeaderboardEntriesAsync(leaderboardId);
                return NoContent();
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e, "Invalid argument provided while deleting entry.");
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogError(e, "Leaderboard entry not found.");
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while deleting leaderboard entry.");
                return StatusCode(500, "Internal server error");
            }
        }
        
    }
}
