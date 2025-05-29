using AK_Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace AFKAT_Servies.Controllers
{
    [ApiController]
    [Route("afk_services")]
    public class LeaderboardController(ILogger<LeaderboardController> logger, UnitOfWork unitOfWork) : ControllerBase
    {
        private readonly ILogger<LeaderboardController> _logger = logger;
        private readonly UnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        [Route("afk_leaderboard/{leaderboardId}")]
        public IActionResult GetLeaderboard(int leaderboardId)
        {
            var leaderboard = _unitOfWork.Leaderboards.GetLeaderboardAsync(leaderboardId);
            if (leaderboard == null)
            {
                return NotFound($"Leaderboard with ID {leaderboardId} not found.");
            }
            return Ok(leaderboard.Result);
        }
        [HttpGet]
        [Route("afk_leaderboard")]
        public IActionResult GetLeaderboards(int page = 1, int pageSize = 20)
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

        [HttpGet]
        [Route("afk_leaderboard/game/{gameId}")]
        public IActionResult GetLeaderboardBygGames(int page,int pageSize,int gameId)
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
        [HttpPost]
        [Route("afk_leaderboard")]
        public IActionResult CreateLeaderboard([FromBody] LeaderboardDTO leaderboardDto)
        {
            var response = _unitOfWork.Leaderboards.CreateLeaderboardAsync(leaderboardDto);
            if (response == null)
            {
                return BadRequest("Failed to create leaderboard.");
            }
            return CreatedAtAction(nameof(GetLeaderboard), new { leaderboardId = response.Result.Id }, response.Result);
        }
        [HttpPut]
        [Route("afk_leaderboard/{leaderboardId}")]
        public IActionResult UpdateLeaderboard([FromBody] LeaderboardDTO leaderboardDto)
        {
            var response = _unitOfWork.Leaderboards.UpdateLeaderboardAsync(leaderboardDto);
            
            if (response == null)
            {
                return NotFound("Leaderboard not found or update failed.");
            }
            
            return NoContent();
        }
        [HttpDelete]
        [Route("afk_leaderboard/{leaderboardId}")]
        public IActionResult DeleteLeaderboard(int leaderboardId)
        {
            var response = _unitOfWork.Leaderboards.DeleteLeaderboardAsync(leaderboardId);
            
            if (response == null)
            {
                return NotFound($"Leaderboard with ID {leaderboardId} not found.");
            }
            
            return NoContent();
        }
	}
}
