using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities.Cache;

public class CachedSeason
{
    [Key]
    public int Id { get; set; }
    
    public int Year { get; set; }
    public string Start { get; set; } = string.Empty;
    public string End { get; set; } = string.Empty;
    public bool Current { get; set; }
    
    // Navigation properties
    public int LeagueId { get; set; }
    public CachedLeague League { get; set; } = null!;
    
    // Cache metadata
    public DateTime CachedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
}