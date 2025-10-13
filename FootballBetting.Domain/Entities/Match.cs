using System.ComponentModel.DataAnnotations;
using FootballBetting.Domain.Enums;

namespace FootballBetting.Domain.Entities;

public class Match
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string HomeTeam { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string AwayTeam { get; set; } = string.Empty;
    
    [Required]
    public DateTime MatchDateTime { get; set; }
    
    [MaxLength(100)]
    public string? League { get; set; }
    
    [MaxLength(100)]
    public string? Competition { get; set; }
    
    public DateTime ScrapedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Odds> Odds { get; set; } = new List<Odds>();
    public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();
    public virtual ICollection<ProviderMatchMapping> ProviderMappings { get; set; } = new List<ProviderMatchMapping>();
}
