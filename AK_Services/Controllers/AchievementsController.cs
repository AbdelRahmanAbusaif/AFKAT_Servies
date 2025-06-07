using AK_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AFKAT_Servies.Controllers
{

    [ApiController]
    [Route("afk_services/afk_achievements")]
    public class AchievementsController(ILogger<LeaderboardController> logger, IUnitOfWork unitOfWork) : ControllerBase
    {
        private readonly ILogger<LeaderboardController> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet]
        [Route("{achivementId}")]
        public IActionResult GetAchievement(int achivementId)
        {
            try
            {
                var response = _unitOfWork.Achivementses.GetAchivementAsync(achivementId);
                if (response.Result.GameId == 0)
                {
                    return NotFound($"Achievement with ID {achivementId} not found.");
                }

                return Ok(new
                {
                    Id = response.Result.Id,
                    GameId = response.Result.GameId,
                    Name = response.Result.Name,
                    Description = response.Result.Description,
                    ImageUrl = response.Result.ImageUrl
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error retrieving achievement with ID {AchivementId}", achivementId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving achievement with ID {AchivementId}", achivementId);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        [Route("game/{gameId}/")]
        public IActionResult GetAchievementById(int gameId, int page = 1, int pageSize = 20)
        {
            try
            {
                var response = _unitOfWork.Achivementses.GetAchivementsAsync(gameId, page, pageSize);
                if (response.Result.Count == 0)
                {
                    return NotFound("No achievements found for the specified game ID.");
                }

                return Ok(new 
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = response.Result.Count,
                    Achievements = response.Result.Select(a => new 
                    {
                        Id = a.Id,
                        GameId = a.GameId,
                        Name = a.Name,
                        Description = a.Description,
                        ImageUrl = a.ImageUrl
                    }).ToList()
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error retrieving achievements for game ID {GameId}", gameId);
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving achievements for game ID {GameId}", gameId);
                return StatusCode(500, "An unexpected error occurred.");
            }
            
        }

        [HttpPost]
        [Route("")]
        public IActionResult CreateAchievement([FromForm] AchivementsDTO achivementDto)
        {
            try
            {
                var response = _unitOfWork.Achivementses.CreateAchivementAsync(achivementDto, achivementDto.Image);
                if (response.Result.GameId == 0)
                {
                    return BadRequest("Failed to create achievement.");
                }
                return CreatedAtAction(nameof(GetAchievement), new { achivementId = response.Result.Id }, response.Result);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Error creating achievement");
                return BadRequest("Achievement cannot be null.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error creating achievement");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error creating achievement");
                return BadRequest("Invalid operation: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating achievement");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpPut]
        [Route("")]
        public IActionResult UpdateAchievement([FromForm] AchivementsDTO achivementDto)
        {
            try
            {
                var response = _unitOfWork.Achivementses.UpdateAchivementAsync(achivementDto, achivementDto.Image);
                if (response.Result.GameId == 0)
                {
                    return NotFound($"Achievement with ID not found.");
                }

                return Ok(response.Result);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Error updating achievement with ID {AchivementId}", achivementDto.Id);
                return BadRequest("Achievement cannot be null.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error updating achievement with ID {AchivementId} ", achivementDto.Id);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error updating achievement with ID {AchivementId}", achivementDto.Id);
                return BadRequest("Invalid operation: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating achievement with ID {AchivementId}", achivementDto.Id);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
        [HttpDelete]
        [Route("{achivementId}")]
        public IActionResult DeleteAchievement(int achivementId)
        {
            try
            {
                _unitOfWork.Achivementses.DeleteAchivementAsync(achivementId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error deleting achievement with ID {AchivementId}", achivementId);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Achievement with ID {AchivementId} not found.", achivementId);
                return NotFound($"Achievement with ID {achivementId} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting achievement with ID {AchivementId}", achivementId);
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}

