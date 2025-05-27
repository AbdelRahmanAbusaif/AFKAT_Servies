using Microsoft.AspNetCore.Mvc;

namespace AFKAT_Servies.Controllers
{
    [ApiController]
    [Route("afk_services")]
    public class LeaderboardController(ILogger<LeaderboardController> logger, Supabase.Client client) : ControllerBase
    {
        private readonly ILogger<LeaderboardController> _logger = logger;
        private readonly Supabase.Client _supabaseClient = client;

        [HttpGet]
        [Route("afk_leaderboard/{leaderboardId}")]
        public IActionResult GetLeaderboard(int leaderboardId)
        {
            if (leaderboardId <= 0)
            {
                return BadRequest("Game ID is required.");
            }

            var respose = _supabaseClient.From<Leaderboards>().
                Select("*").
                Where(x => x.Id == leaderboardId)
                .Get();

            if (respose.Result.Model == null)
            {
                return NotFound($"Leaderboard with ID {leaderboardId} not found.");
            }

            // Convert the response to a DTO
            LeaderboardDTO leaderboard = new LeaderboardDTO
            {
                Id = respose.Result.Model.Id,
                GameId = respose.Result.Model.GameId,
                LeaderboardName = respose.Result.Model.LeaderboardName
            };

            return Ok(
                leaderboard
            );
        }
        [HttpGet]
        [Route("afk_leaderboard")]
        public IActionResult GetLeaderboards(int page = 1, int pageSize = 20)
        {
            var respose = _supabaseClient.From<Leaderboards>().
                Select("*")
                .Get();

            if (respose.Result.Models == null || !respose.Result.Models.Any())
            {
                return NotFound("No leaderboards found.");
            }

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var pagedModels = respose.Result.Models
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            List<LeaderboardDTO> leaderboards = respose.Result.Models.Select(x => new LeaderboardDTO
            {
                Id = x.Id,
                GameId = x.GameId,
                LeaderboardName = x.LeaderboardName
            }).ToList();

            if (leaderboards == null || !leaderboards.Any())
            {
                return NotFound("No leaderboards found.");
            }

            return Ok(
                new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = respose.Result.Models.Count,
                    Leaderboards = leaderboards
                }
            );
        }
        [HttpPost]
        [Route("afk_leaderboard")]
        public IActionResult CreateLeaderboard([FromBody] LeaderboardDTO leaderboardDto)
        {
            if (leaderboardDto == null || string.IsNullOrEmpty(leaderboardDto.LeaderboardName) || leaderboardDto.GameId <= 0)
            {
                return BadRequest("Invalid leaderboard data.");
            }

            var newLeaderboard = new Leaderboards
            {
                GameId = leaderboardDto.GameId,
                LeaderboardName = leaderboardDto.LeaderboardName
            };

            var response = _supabaseClient.From<Leaderboards>()
                .Insert(newLeaderboard);

            if (response.Result.Model == null)
            {
                return StatusCode(500, "Failed to create leaderboard.");
            }

            return CreatedAtAction(nameof(GetLeaderboard), new { leaderboardId = response.Result.Model.Id }, response.Result.Model);
        }
        [HttpPut]
        [Route("afk_leaderboard/{leaderboardId}")]
        public IActionResult UpdateLeaderboard(int leaderboardId, [FromBody] LeaderboardDTO leaderboardDto)
        {
            if (leaderboardId <= 0 || leaderboardDto == null || string.IsNullOrEmpty(leaderboardDto.LeaderboardName) || leaderboardDto.GameId <= 0)
            {
                return BadRequest("Invalid leaderboard data.");
            }

            var existingLeaderboard = _supabaseClient.From<Leaderboards>()
                .Select("*")
                .Where(x => x.Id == leaderboardId)
                .Get();

            if (existingLeaderboard.Result.Model == null)
            {
                return NotFound($"Leaderboard with ID {leaderboardId} not found.");
            }

            existingLeaderboard.Result.Model.GameId = leaderboardDto.GameId;
            existingLeaderboard.Result.Model.LeaderboardName = leaderboardDto.LeaderboardName;

            var updateResponse = _supabaseClient.From<Leaderboards>()
                .Update(existingLeaderboard.Result.Model);

            if (updateResponse.Result.Model == null)
            {
                return StatusCode(500, "Failed to update leaderboard.");
            }

            return Ok(updateResponse.Result.Model);
        }
        [HttpDelete]
        [Route("afk_leaderboard/{leaderboardId}")]
        public IActionResult DeleteLeaderboard(int leaderboardId)
        {
            if (leaderboardId <= 0)
            {
                return BadRequest("Leaderboard ID is required.");
            }
            var existingLeaderboard = _supabaseClient.From<Leaderboards>()
                .Select("*")
                .Where(x => x.Id == leaderboardId)
                .Get();
            if (existingLeaderboard.Result.Model == null)
            {
                return NotFound($"Leaderboard with ID {leaderboardId} not found.");
            }
            var deleteResponse = _supabaseClient.From<Leaderboards>()
                .Delete(existingLeaderboard.Result.Model);

            if (deleteResponse.Result.Model == null)
            {
                return StatusCode(500, "Failed to delete leaderboard.");
            }
            return NoContent();
        }
	}
}
