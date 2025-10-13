using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities.Cache;

public class CachedPlayer
{
    [Key]
    public int Id { get; set; }
    
    public int ApiFootballId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime? BirthDate { get; set; }
    public string BirthPlace { get; set; } = string.Empty;
    public string BirthCountry { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string Height { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public bool Injured { get; set; }
    public string Photo { get; set; } = string.Empty;
    
    // Team relationship
    public int? CurrentTeamId { get; set; }
    public CachedTeam? CurrentTeam { get; set; }
    
    // Position and statistics
    public string Position { get; set; } = string.Empty;
    public int? Rating { get; set; }
    
    // Navigation properties for fixtures (many-to-many)
    public ICollection<CachedFixture> Fixtures { get; set; } = new List<CachedFixture>();
    public ICollection<CachedPlayerStatistic> Statistics { get; set; } = new List<CachedPlayerStatistic>();
    
    // Cache metadata
    public DateTime CachedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
}