public record LeaderboardEntryDTO
{
    public int UserId { get; init; }
    public int Rank { get; set; } 
    public required string PlayerName { get; init; }
    public int Score { get; init; }
}