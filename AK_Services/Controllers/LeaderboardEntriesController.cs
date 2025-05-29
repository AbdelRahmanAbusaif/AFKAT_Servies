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
            var leaderboardEntriesService = _unitOfWork.LeaderboardEntries.GetLeaderboardEntriesAsync(leaderboardId, page, pageSize);
            if (leaderboardEntriesService == null || !leaderboardEntriesService.Result.Any())
            {
                return NotFound($"No entries found for leaderboard ID {leaderboardId}.");
            }

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = leaderboardEntriesService.Result.Count,
                Entries = leaderboardEntriesService.Result
            });
        }
        [HttpGet]
        [Route("afk_leaderboard_entries/{leaderboardId}/user/{userId}")]
        public IActionResult GetUserEntry(int leaderboardId, int userId)
        {
            var entry = _unitOfWork.LeaderboardEntries.GetLeaderboardEntriesByUserAsync(leaderboardId, userId);
            if (entry == null)
            {
                return NotFound($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
            }
            return Ok(entry.Result);
        }
        [HttpPost]
        [Route("afk_leaderboard_entries/{leaderboardId}")]
        public IActionResult CreateEntry(int leaderboardId,[FromBody] LeaderboardEntryDTO entryDto)
        {
            var leaderboard = _unitOfWork.LeaderboardEntries.CreateLeaderboardEntryAsync(leaderboardId, entryDto);
            if (leaderboard == null)
            {
                return BadRequest("Failed to create leaderboard entry.");
            }
            return CreatedAtAction(nameof(GetUserEntry), new { leaderboardId = leaderboard.Result.LeaderboardId, userId = leaderboard.Result.PlayerId }, leaderboard.Result);
        }
        [HttpPost]
        [Route("afk_leaderboard_entries/AddScore/{leaderboardId}/{userId}")]
        public IActionResult AddScore(int leaderboardId, int userId, [FromBody] int scoreToAdd)
        {
            var leaderboardEntry = _unitOfWork.LeaderboardEntries.AddScoreToLeaderboardAsync(leaderboardId, userId, scoreToAdd);
            if (leaderboardEntry == null)
            {
                return BadRequest("Failed to add score to leaderboard entry.");
            }
            return Ok(leaderboardEntry.Result);
        }
        [HttpPut]
        [Route("afk_leaderboard_entries/{leaderboardId}/{userId}")]
        public IActionResult UpdateEntry(int leaderboardId, int userId, [FromBody] LeaderboardEntryDTO entryDto)
        {
            var existingEntry = _unitOfWork.LeaderboardEntries.GetLeaderboardEntriesByUserAsync(leaderboardId, userId);
            if (existingEntry == null)
            {
                return NotFound($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
            }
            var updatedEntry = _unitOfWork.LeaderboardEntries.UpdateLeaderboardEntryAsync(leaderboardId, entryDto);
            if (updatedEntry == null)
            {
                return BadRequest("Failed to update leaderboard entry.");
            }
            return Ok(updatedEntry.Result);
        }
        [HttpDelete]
        [Route("afk_leaderboard_entries/{leaderboardId}/{userId}")]
        public IActionResult DeleteEntry(int leaderboardId, int userId)
        {
            var deleted = _unitOfWork.LeaderboardEntries.DeleteLeaderboardEntryAsync(leaderboardId, userId);
            if (!deleted.Result)
            {
                return NotFound($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
            }
            return NoContent();
        }
        [HttpDelete]
        [Route("afk_leaderboard_entries/{leaderboardId}")]
        public IActionResult DeleteEntries(int leaderboardId)
        {
            var deleted = _unitOfWork.LeaderboardEntries.DeleteLeaderboardEntriesAsync(leaderboardId);
            if (!deleted.Result)
            {
                return NotFound($"No entries found for leaderboard ID {leaderboardId}.");
            }
            return NoContent();
        }
        
    }
}
