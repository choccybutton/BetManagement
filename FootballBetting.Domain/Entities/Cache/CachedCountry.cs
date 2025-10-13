using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities.Cache;

public class CachedCountry
{
    [Key]
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Flag { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<CachedLeague> Leagues { get; set; } = new List<CachedLeague>();
    
    // Cache metadata
    public DateTime CachedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
}