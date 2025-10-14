using System;

namespace FootballAPIWrapper.Usage
{
    public class ApiUsageStatistics
    {
        // Daily Rate Limits (x-ratelimit-*)
        /// <summary>
        /// Maximum number of requests allowed per day according to subscription
        /// </summary>
        public int DailyRequestsLimit { get; set; }

        /// <summary>
        /// Number of requests remaining per day according to subscription
        /// </summary>
        public int DailyRequestsRemaining { get; set; }

        /// <summary>
        /// Number of requests used per day (calculated from limit - remaining)
        /// </summary>
        public int DailyRequestsUsed => Math.Max(0, DailyRequestsLimit - DailyRequestsRemaining);

        // Per-Minute Rate Limits (X-RateLimit-*)
        /// <summary>
        /// Maximum number of API calls per minute
        /// </summary>
        public int PerMinuteLimit { get; set; }

        /// <summary>
        /// Number of API calls remaining before reaching the limit per minute
        /// </summary>
        public int PerMinuteRemaining { get; set; }

        /// <summary>
        /// Number of API calls used in current minute (calculated from limit - remaining)
        /// </summary>
        public int PerMinuteUsed => Math.Max(0, PerMinuteLimit - PerMinuteRemaining);

        /// <summary>
        /// When the rate limit resets
        /// </summary>
        public DateTime? RateLimitReset { get; set; }

        // Legacy properties for backward compatibility
        /// <summary>
        /// Number of requests made in the current period (uses daily stats)
        /// </summary>
        public int RequestsUsed => DailyRequestsUsed;

        /// <summary>
        /// Maximum number of requests allowed in the current period (uses daily stats)
        /// </summary>
        public int RequestsLimit => DailyRequestsLimit;

        /// <summary>
        /// Number of requests remaining in the current period (uses daily stats)
        /// </summary>
        public int RequestsRemaining => DailyRequestsRemaining;

        /// <summary>
        /// Total number of API calls made since tracking started
        /// </summary>
        public long TotalApiCalls { get; set; }

        /// <summary>
        /// When tracking started
        /// </summary>
        public DateTime TrackingStarted { get; set; }

        /// <summary>
        /// Last time an API call was made
        /// </summary>
        public DateTime? LastApiCall { get; set; }

        /// <summary>
        /// Average response time in milliseconds
        /// </summary>
        public double AverageResponseTimeMs { get; set; }

        /// <summary>
        /// Number of failed requests
        /// </summary>
        public long FailedRequests { get; set; }

        /// <summary>
        /// Success rate as a percentage
        /// </summary>
        public double SuccessRate => TotalApiCalls > 0 ? ((double)(TotalApiCalls - FailedRequests) / TotalApiCalls) * 100 : 0;
    }
}