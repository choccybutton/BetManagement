using System.ComponentModel.DataAnnotations;

namespace FootballBetting.Domain.Entities.Cache;

public class ApiUsageLog
{
    [Key]
    public int Id { get; set; }
    
    public string Endpoint { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string Parameters { get; set; } = string.Empty; // JSON serialized parameters
    public DateTime RequestTimestamp { get; set; }
    public int StatusCode { get; set; }
    public string ResponseSize { get; set; } = string.Empty;
    public TimeSpan ResponseTime { get; set; }
    public bool CacheHit { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Rate limiting info
    public int? RateLimitRemaining { get; set; }
    public DateTime? RateLimitReset { get; set; }
    
    // Cost tracking if API has usage costs
    public decimal? Cost { get; set; }
    public string CostCurrency { get; set; } = "USD";
}