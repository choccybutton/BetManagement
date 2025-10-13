using System.ComponentModel.DataAnnotations;
using FootballBetting.Domain.Enums;

namespace FootballBetting.Domain.Entities;

public class Bet
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
    
    public int MatchId { get; set; }
    public virtual Match Match { get; set; } = null!;
    
    public int OddsId { get; set; }
    public virtual Odds Odds { get; set; } = null!;
    
    public BettingProvider Provider { get; set; }
    public BetType BetType { get; set; }
    public BetStatus Status { get; set; } = BetStatus.Pending;
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public decimal OddsAtTimeOfBet { get; set; }
    
    public decimal? PotentialReturn { get; set; }
    public decimal? ActualReturn { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? PlacedAt { get; set; }
    public DateTime? SettledAt { get; set; }
    
    // Provider specific data
    [MaxLength(100)]
    public string? ProviderBetId { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
}