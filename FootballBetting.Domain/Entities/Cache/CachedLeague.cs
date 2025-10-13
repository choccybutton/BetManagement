using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities.Cache;

public class CachedLeague
{
    [Key]
    public int Id { get; set; }
    
    public int ApiFootballId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "league" or "cup"
    public string Logo { get; set; } = string.Empty;
    
    // Navigation properties
    public int? CountryId { get; set; }
    public CachedCountry? Country { get; set; }
    public ICollection<CachedSeason> Seasons { get; set; } = new List<CachedSeason>();
    
    // Cache metadata
    public DateTime CachedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
}