using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities.Cache;

public class CachedTeam
{
    [Key]
    public int Id { get; set; }
    
    public int ApiFootballId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int? Founded { get; set; }
    public bool National { get; set; }
    public string Logo { get; set; } = string.Empty;
    
    // Navigation properties
    public int? VenueId { get; set; }
    public CachedVenue? Venue { get; set; }
    
    // Cache metadata
    public DateTime CachedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
}