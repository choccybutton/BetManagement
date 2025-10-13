using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities.Cache;

public class CachedPlayerStatistic
{
    [Key]
    public int Id { get; set; }
    
    public int PlayerId { get; set; }
    public CachedPlayer Player { get; set; } = null!;
    
    public int? TeamId { get; set; }
    public CachedTeam? Team { get; set; }
    
    public int? LeagueId { get; set; }
    public CachedLeague? League { get; set; }
    
    public int? SeasonId { get; set; }
    public CachedSeason? Season { get; set; }
    
    // Games statistics
    public int? GamesAppearances { get; set; }
    public int? GamesLineups { get; set; }
    public int? GamesMinutes { get; set; }
    public int? GamesNumber { get; set; }
    public string Position { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public bool? Captain { get; set; }
    
    // Substitutes statistics
    public int? SubstitutesIn { get; set; }
    public int? SubstitutesOut { get; set; }
    public int? SubstitutesBench { get; set; }
    
    // Shots statistics
    public int? ShotsTotal { get; set; }
    public int? ShotsOn { get; set; }
    
    // Goals statistics
    public int? GoalsTotal { get; set; }
    public int? GoalsConceded { get; set; }
    public int? GoalsAssists { get; set; }
    public int? GoalsSaves { get; set; }
    
    // Passes statistics
    public int? PassesTotal { get; set; }
    public int? PassesKey { get; set; }
    public int? PassesAccuracy { get; set; }
    
    // Tackles statistics
    public int? TacklesTotal { get; set; }
    public int? TacklesBlocks { get; set; }
    public int? TacklesInterceptions { get; set; }
    
    // Duels statistics
    public int? DuelsTotal { get; set; }
    public int? DuelsWon { get; set; }
    
    // Dribbles statistics
    public int? DribblesAttempts { get; set; }
    public int? DribblesSuccess { get; set; }
    public int? DribblesPast { get; set; }
    
    // Fouls statistics
    public int? FoulsDrawn { get; set; }
    public int? FoulsCommitted { get; set; }
    
    // Cards statistics
    public int? CardsYellow { get; set; }
    public int? CardsYellowRed { get; set; }
    public int? CardsRed { get; set; }
    
    // Penalty statistics
    public int? PenaltyWon { get; set; }
    public int? PenaltyCommitted { get; set; }
    public int? PenaltyScored { get; set; }
    public int? PenaltyMissed { get; set; }
    public int? PenaltySaved { get; set; }
    
    // Cache metadata
    public DateTime CachedAt { get; set; }
    public DateTime? LastUpdated { get; set; }
}