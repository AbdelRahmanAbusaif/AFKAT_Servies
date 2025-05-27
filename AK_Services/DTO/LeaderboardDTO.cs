public record LeaderboardDTO
{
    public int Id { get; init; }
    public int GameId { get; init; }
    public string LeaderboardName { get; init; }
}