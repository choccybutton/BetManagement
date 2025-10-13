using FootballBetting.Domain.Enums;

namespace FootballBetting.Application.DTOs;

public class MatchDto
{
    public int Id { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public DateTime MatchDateTime { get; set; }
    public string? League { get; set; }
    public string? Competition { get; set; }
    public DateTime ScrapedAt { get; set; }
    public List<OddsDto> Odds { get; set; } = new();
}

public class OddsDto
{
    public int Id { get; set; }
    public BetType BetType { get; set; }
    public decimal OddsValue { get; set; }
    public string? Description { get; set; }
    public DateTime ScrapedAt { get; set; }
}

public class CreateBetDto
{
    public int MatchId { get; set; }
    public int OddsId { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}

public class BetDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public MatchDto Match { get; set; } = null!;
    public OddsDto Odds { get; set; } = null!;
    public BetType BetType { get; set; }
    public BetStatus Status { get; set; }
    public decimal Amount { get; set; }
    public decimal OddsAtTimeOfBet { get; set; }
    public decimal? PotentialReturn { get; set; }
    public decimal? ActualReturn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PlacedAt { get; set; }
    public DateTime? SettledAt { get; set; }
    public string? Notes { get; set; }
    public string? ErrorMessage { get; set; }
}

public class OddsComparisonDto
{
    public int MatchId { get; set; }
    public string MatchName { get; set; } = string.Empty;
    public BetType BetType { get; set; }
    public List<ProviderOddsDto> ProviderOdds { get; set; } = new();
    public ProviderOddsDto? BestOdds { get; set; }
}

public class ProviderOddsDto
{
    public BettingProvider Provider { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public decimal OddsValue { get; set; }
    public DateTime ScrapedAt { get; set; }
    public bool IsAvailable { get; set; }
}

public class ProviderAccountDto
{
    public BettingProvider Provider { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public decimal? Balance { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? Status { get; set; }
}
