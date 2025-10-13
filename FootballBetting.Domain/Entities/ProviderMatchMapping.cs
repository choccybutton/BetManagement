using System.ComponentModel.DataAnnotations;
using FootballBetting.Domain.Enums;

namespace FootballBetting.Domain.Entities;

/// <summary>
/// Maps a match to provider-specific identifiers
/// </summary>
public class ProviderMatchMapping
{
    public int Id { get; set; }
    
    public int MatchId { get; set; }
    public virtual Match Match { get; set; } = null!;
    
    public BettingProvider Provider { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ProviderMatchId { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? ProviderUrl { get; set; }
    
    [MaxLength(200)]
    public string? ProviderEventName { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    
    public bool IsActive { get; set; } = true;
}