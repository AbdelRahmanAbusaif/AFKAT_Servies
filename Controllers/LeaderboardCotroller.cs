using Microsoft.AspNetCore.Mvc;

namespace AFKAT_Servies.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderboardController(ILogger<LeaderboardController> logger) : ControllerBase
    {
        private readonly ILogger<LeaderboardController> _logger = logger;

		[HttpGet]
		public IActionResult GetLeaderboard([FromHeader] string gameId)
		{
            return Ok(new { gameId });
		}
	}
}
