using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities.Cache;

public class CachedFixture
{
    [Key]
    public int Id { get; set; }
    
    public int ApiFootballId { get; set; }
    public string Referee { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public long Timestamp { get; set; }
    
    // Periods
    public DateTime? FirstPeriod { get; set; }
    public DateTime? SecondPeriod { get; set; }
    
    // Venue
    public int? VenueId { get; set; }
    public CachedVenue? Venue { get; set; }
    
    // Status
    public string StatusLong { get; set; } = string.Empty;
    public string StatusShort { get; set; } = string.Empty;
    public int? StatusElapsed { get; set; }
    
    // League
    public int LeagueId { get; set; }
    public CachedLeague League { get; set; } = null!;
    
    // Teams
    public int HomeTeamId { get; set; }
    public CachedTeam HomeTeam { get; set; } = null!;
    
    public int AwayTeamId { get; set; }
    public CachedTeam AwayTeam { get; set; } = null!;
    
    // Goals
    public int? HomeGoals { get; set; }
    public int? AwayGoals { get; set; }
    
    // Score details (halftime, fulltime, extratime, penalty)
    public int? HalfTimeHomeGoals { get; set; }
    public int? HalfTimeAwayGoals { get; set; }
    public int? FullTimeHomeGoals { get; set; }
    public int? FullTimeAwayGoals { get; set; }
    public int? ExtraTimeHomeGoals { get; set; }
    public int? ExtraTimeAwayGoals { get; set; }
    public int? PenaltyHomeGoals { get; set; }
    public int? PenaltyAwayGoals { get; set; }
    
    // Navigation properties
    public ICollection<CachedPlayer> Players { get; set; } = new List<CachedPlayer>();
    
    // Cache metadata
    public DateTime CachedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
}