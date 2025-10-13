using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities.Cache;

public class CachedVenue
{
    [Key]
    public int Id { get; set; }
    
    public int ApiFootballId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int? Capacity { get; set; }
    public string Surface { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<CachedTeam> Teams { get; set; } = new List<CachedTeam>();
    public ICollection<CachedFixture> HomeFixtures { get; set; } = new List<CachedFixture>();
    
    // Cache metadata
    public DateTime CachedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
}