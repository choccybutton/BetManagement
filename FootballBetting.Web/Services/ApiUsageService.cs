using System.Collections.Concurrent;
using FootballAPIWrapper.Usage;

namespace FootballBetting.Web.Services;

public class ApiRequestLog
{
    public DateTime Timestamp { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public int ResponseTime { get; set; } // in milliseconds
    public string? ErrorMessage { get; set; }
}

public class ExtendedApiUsageStatistics
{
    // Daily rate limits (primary display as per spec)
    public int TotalRequests { get; set; }
    public int DailyLimit { get; set; }
    public int RemainingRequests => Math.Max(0, DailyLimit - TotalRequests);
    public double DailyUsagePercentage => DailyLimit > 0 ? (TotalRequests * 100.0) / DailyLimit : 0;
    
    // Per-minute rate limits (for additional monitoring)
    public int PerMinuteUsed { get; set; }
    public int PerMinuteLimit { get; set; }
    public int PerMinuteRemaining { get; set; }
    public double PerMinuteUsagePercentage => PerMinuteLimit > 0 ? (PerMinuteUsed * 100.0) / PerMinuteLimit : 0;
    
    public DateTime LastReset { get; set; }
    public List<ApiRequestLog> RecentRequests { get; set; } = new();
    public double AverageResponseTime { get; set; }
    public long FailedRequests { get; set; }
    public double SuccessRate { get; set; }
    public DateTime? RateLimitReset { get; set; }
}

public interface IApiUsageService
{
    ExtendedApiUsageStatistics GetUsageStatistics();
    void TrackRequest(string endpoint, bool isSuccess, int responseTime, string? errorMessage = null);
    void ResetDailyUsage();
    void TriggerUsageUpdate();
    event EventHandler<ExtendedApiUsageStatistics>? UsageUpdated;
}

public class ApiUsageService : IApiUsageService
{
    private readonly ConcurrentQueue<ApiRequestLog> _requestLogs = new();
    private readonly object _lockObject = new();
    private readonly IUsageTracker _usageTracker;
    private const int MaxLogEntries = 1000;
    
    public event EventHandler<ExtendedApiUsageStatistics>? UsageUpdated;

    public ApiUsageService(IUsageTracker usageTracker)
    {
        _usageTracker = usageTracker ?? throw new ArgumentNullException(nameof(usageTracker));
    }

    public ExtendedApiUsageStatistics GetUsageStatistics()
    {
        lock (_lockObject)
        {
            // Get real usage data from the FootballAPIWrapper's usage tracker
            var apiStats = _usageTracker.GetUsageStatistics();
            
            // Update the recent requests list from the concurrent queue
            var recentRequests = _requestLogs.ToList()
                .OrderByDescending(x => x.Timestamp)
                .Take(50) // Keep last 50 requests for display
                .ToList();
                
            return new ExtendedApiUsageStatistics
            {
                // Use daily rate limit data as primary display (as per spec requirement)
                TotalRequests = apiStats.DailyRequestsUsed,
                DailyLimit = apiStats.DailyRequestsLimit > 0 ? apiStats.DailyRequestsLimit : 100, // Default fallback
                
                // Per-minute rate limit data for additional monitoring
                PerMinuteUsed = apiStats.PerMinuteUsed,
                PerMinuteLimit = apiStats.PerMinuteLimit,
                PerMinuteRemaining = apiStats.PerMinuteRemaining,
                
                LastReset = apiStats.RateLimitReset ?? DateTime.Today,
                RecentRequests = recentRequests,
                AverageResponseTime = apiStats.AverageResponseTimeMs,
                FailedRequests = apiStats.FailedRequests,
                SuccessRate = apiStats.SuccessRate,
                RateLimitReset = apiStats.RateLimitReset
            };
        }
    }

    public void TrackRequest(string endpoint, bool isSuccess, int responseTime, string? errorMessage = null)
    {
        var logEntry = new ApiRequestLog
        {
            Timestamp = DateTime.Now,
            Endpoint = endpoint,
            IsSuccess = isSuccess,
            ResponseTime = responseTime,
            ErrorMessage = errorMessage
        };

        _requestLogs.Enqueue(logEntry);

        lock (_lockObject)
        {
            // Keep the queue size manageable
            while (_requestLogs.Count > MaxLogEntries)
            {
                _requestLogs.TryDequeue(out _);
            }
        }
        
        // Notify subscribers of the update
        UsageUpdated?.Invoke(this, GetUsageStatistics());
    }
    
    /// <summary>
    /// Triggers a usage update event - useful when the underlying tracker data changes
    /// </summary>
    public void TriggerUsageUpdate()
    {
        UsageUpdated?.Invoke(this, GetUsageStatistics());
    }

    public void ResetDailyUsage()
    {
        lock (_lockObject)
        {
            // Reset the FootballAPIWrapper's usage tracker
            _usageTracker.ResetStatistics();
            
            // Clear request logs
            while (_requestLogs.TryDequeue(out _)) { }
        }
        
        UsageUpdated?.Invoke(this, GetUsageStatistics());
    }
}