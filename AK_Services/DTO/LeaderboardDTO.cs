public record LeaderboardDTO
{
    public int Id { get; init; }
    public int GameId { get; init; }
    public required string LeaderboardName { get; init; }
}