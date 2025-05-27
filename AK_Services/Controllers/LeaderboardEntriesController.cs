using Microsoft.AspNetCore.Mvc;

namespace AFKAT_Servies.Controllers
{
    [ApiController]
    [Route("afk_services")]
    public class LeaderboardEntriesController(ILogger<LeaderboardEntriesController> logger, Supabase.Client client) : ControllerBase
    {
        private readonly ILogger<LeaderboardEntriesController> _logger = logger;
        private readonly Supabase.Client _supabaseClient = client;

        [HttpGet]
        [Route("afk_leaderboard_entries/{leaderboardId}")]
        public IActionResult GetEntries(int leaderboardId, int page = 1, int pageSize = 20)
        {
            if (leaderboardId <= 0)
            {
                return BadRequest("Leaderboard ID is required.");
            }

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var response = _supabaseClient.From<LeaderboardEntries>()
                .Select("*")
                .Where(x => x.LeaderboardId == leaderboardId)
                .Get();

            if (response.Result.Models == null || !response.Result.Models.Any())
            {
                return NotFound($"Leaderboard entries with ID {leaderboardId} not found.");
            }

            var models = response.Result.Models
                .OrderByDescending(x => x.Score)
                .ToList();

            var pagedModels = models
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            List<LeaderboardEntryDTO> leaderboardEntries = pagedModels
            .Select((x, index) => new LeaderboardEntryDTO
            {
                UserId = x.PlayerId,
                Rank = ((page - 1) * pageSize) + index + 1,
                PlayerName = x.PlayerName,
                Score = x.Score
            })
            .ToList();

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = models.Count,
                Entries = leaderboardEntries
            });
        }
        [HttpGet]
        [Route("afk_leaderboard_entries/{leaderboardId}/user/{userId}")]
        public IActionResult GetUserEntry(int leaderboardId, int userId)
        {
            if (leaderboardId <= 0)
            {
                return BadRequest("Leaderboard ID is required.");
            }

            if (userId <= 0)
            {
                return BadRequest("User ID is required.");
            }

            var response = _supabaseClient.From<LeaderboardEntries>().
                Select("*").
                Where(x => x.LeaderboardId == leaderboardId && x.PlayerId == userId)
                .Get();

            if (response.Result.Model == null)
            {
                return NotFound($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
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

            if (allEntriesResponse.Result.Models != null)
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

            return Ok(
                leaderboardEntry
            );
        }
        [HttpPost]
        [Route("afk_leaderboard_entries/{leaderboardId}")]
        public IActionResult CreateEntry(int leaderboardId,[FromBody] LeaderboardEntryDTO entryDto)
        {
            if (entryDto == null)
            {
                return BadRequest("Entry data is required.");
            }

            if (entryDto.UserId <= 0 || string.IsNullOrWhiteSpace(entryDto.PlayerName) || entryDto.Score < 0)
            {
                return BadRequest("Invalid entry data.");
            }

            var newEntry = new LeaderboardEntries
            {
                PlayerId = entryDto.UserId,
                LeaderboardId = leaderboardId,
                PlayerName = entryDto.PlayerName,
                Score = entryDto.Score,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var response = _supabaseClient.From<LeaderboardEntries>()
                .Insert(newEntry);

            if (response.Result.Model == null)
            {
                return BadRequest("Failed to create leaderboard entry.");
            }

            newEntry = response.Result.Model;
            return CreatedAtAction(nameof(GetUserEntry), new { leaderboardId = leaderboardId, userId = newEntry.PlayerId }, newEntry);
        }
        [HttpPut]
        [Route("afk_leaderboard_entries/{leaderboardId}/{userId}")]
        public IActionResult UpdateEntry(int leaderboardId, int userId, [FromBody] LeaderboardEntryDTO entryDto)
        {
            if (leaderboardId <= 0 || userId <= 0 || entryDto == null)
            {
                return BadRequest("Invalid leaderboard or user ID.");
            }

            var existingEntry = _supabaseClient.From<LeaderboardEntries>()
                .Select("*")
                .Where(x => x.LeaderboardId == leaderboardId && x.PlayerId == userId)
                .Get();

            if (existingEntry.Result.Model == null)
            {
                return NotFound($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
            }

            existingEntry.Result.Model.PlayerName = entryDto.PlayerName;
            existingEntry.Result.Model.Score = entryDto.Score;
            existingEntry.Result.Model.UpdatedAt = DateTime.UtcNow;

            var updateResponse = _supabaseClient.From<LeaderboardEntries>()
                .Update(existingEntry.Result.Model);

            if (updateResponse.Result.Model == null)
            {
                return BadRequest("Failed to update leaderboard entry.");
            }

            return Ok(updateResponse.Result.Model);
        }
        [HttpDelete]
        [Route("afk_leaderboard_entries/{leaderboardId}/{userId}")]
        public IActionResult DeleteEntry(int leaderboardId, int userId)
        {
            if (leaderboardId <= 0 || userId <= 0)
            {
                return BadRequest("Invalid leaderboard or user ID.");
            }

            var existingEntry = _supabaseClient.From<LeaderboardEntries>()
                .Select("*")
                .Where(x => x.LeaderboardId == leaderboardId && x.PlayerId == userId)
                .Get();

            if (existingEntry.Result.Model == null)
            {
                return NotFound($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
            }

            return NoContent();
        }
        [HttpDelete]
        [Route("afk_leaderboard_entries/{leaderboardId}")]
        public IActionResult DeleteEntries(int leaderboardId)
        {
            if (leaderboardId <= 0)
            {
                return BadRequest("Leaderboard ID is required.");
            }

            var existingEntries = _supabaseClient.From<LeaderboardEntries>()
                .Select("*")
                .Where(x => x.LeaderboardId == leaderboardId)
                .Get();

            if (existingEntries.Result.Models == null || !existingEntries.Result.Models.Any())
            {
                return NotFound($"No entries found for leaderboard ID {leaderboardId}.");
            }

            var deleteResponse = _supabaseClient.From<LeaderboardEntries>()
                .Where(x => x.LeaderboardId == leaderboardId)
                .Delete();

            if (deleteResponse == null)
            {
                return StatusCode(500, "Failed to delete leaderboard entries.");
            }

            return NoContent();
        }
        [HttpPost]
        [Route("afk_leaderboard_entries/AddScore/{leaderboardId}/{userId}")]
        public IActionResult AddScore(int leaderboardId, int userId, [FromBody] int scoreToAdd)
        {
            if (leaderboardId <= 0)
            {
                return BadRequest("Invalid leaderboard");
            }
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID");
            }
            if (scoreToAdd <= 0)
            {
                return BadRequest("Score to add must be greater than zero.");
            }

            var existingEntry = _supabaseClient.From<LeaderboardEntries>()
                .Select("*")
                .Where(x => x.LeaderboardId == leaderboardId && x.PlayerId == userId)
                .Get();

            if (existingEntry.Result.Model == null)
            {
                return NotFound($"No entry found for user ID {userId} in leaderboard ID {leaderboardId}.");
            }

            existingEntry.Result.Model.Score += scoreToAdd;
            existingEntry.Result.Model.UpdatedAt = DateTime.UtcNow;

            var updateResponse = _supabaseClient.From<LeaderboardEntries>()
                .Update(existingEntry.Result.Model);

            if (updateResponse.Result.Model == null)
            {
                return BadRequest("Failed to update leaderboard entry.");
            }

            return Ok(updateResponse.Result.Model);
        }
    }
}
