
using System.ComponentModel.DataAnnotations;

public record AchivementsDTO
{
    public int Id { get; init; }
    public int GameId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string ImageUrl { get; init; }
    public IFormFile Image { get; init; }
}