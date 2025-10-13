using System.ComponentModel.DataAnnotations;
using FootballBetting.Domain.Enums;

namespace FootballBetting.Domain.Entities;

public class Odds
{
    public int Id { get; set; }
    
    public int MatchId { get; set; }
    public virtual Match Match { get; set; } = null!;
    
    public BettingProvider Provider { get; set; }
    public BetType BetType { get; set; }
    
    [Required]
    public decimal OddsValue { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ProviderOddsId { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public DateTime ScrapedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();
}
