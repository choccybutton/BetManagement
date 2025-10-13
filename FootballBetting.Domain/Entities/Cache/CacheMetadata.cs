using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities.Cache;

public class CacheMetadata
{
    [Key]
    public int Id { get; set; }
    
    public string EntityType { get; set; } = string.Empty;
    public string EntityKey { get; set; } = string.Empty;
    public DateTime CachedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastApiCall { get; set; }
    public int ApiCallCount { get; set; } = 0;
    
    // Additional metadata
    public int? CacheHitCount { get; set; } = 0;
    public DateTime? LastAccessed { get; set; }
    public string? Source { get; set; }
    public bool IsStale { get; set; } = false;
}