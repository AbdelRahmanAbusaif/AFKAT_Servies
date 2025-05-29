using AK_Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace AFKAT_Servies.Controllers;

[ApiController]
[Route("afk_services/afk_achivements")]
public class AchivementsController(ILogger<LeaderboardController> logger, UnitOfWork unitOfWork) : ControllerBase
{
    private readonly ILogger<LeaderboardController> _logger = logger;
    private readonly UnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    [Route("{achivementId}")]
    public IActionResult GetAchievement(int achivementId)
    {
        var response = _unitOfWork.Achivements.GetAchivementAsync(achivementId);
        if (response.Result == null)
        {
            return NotFound($"Achievement with ID {achivementId} not found.");
        }
        
        return Ok(response.Result);
    }

    [HttpGet]
    [Route("game/{gameId}/")]
    public IActionResult GetAchievementById(int gameId,int page = 1, int pageSize = 20)
    {
        var response = _unitOfWork.Achivements.GetAchivementsAsync(gameId, page, pageSize);
        if (response.Result == null || !response.Result.Any())
        {
            return NotFound("No achievements found for the specified game ID.");
        }
        return Ok(response.Result);
    }
    
}