namespace AK_Services.DTOS;

public class PlayerAchievementDTO
{
    public int PlayerId { get; init; }
    public int AchievementId { get; init; }
    public int GameId { get; init; }
    public bool IsCompleted { get; init; }
    public string AchievementIconURL { get; init; }
    public DateTime DateAchieved { get; init; }
}