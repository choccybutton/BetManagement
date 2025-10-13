using System;

namespace FootballAPIWrapper.Usage
{
    public class ApiUsageStatistics
    {
        /// <summary>
        /// Number of requests made in the current period
        /// </summary>
        public int RequestsUsed { get; set; }

        /// <summary>
        /// Maximum number of requests allowed in the current period
        /// </summary>
        public int RequestsLimit { get; set; }

        /// <summary>
        /// Number of requests remaining in the current period
        /// </summary>
        public int RequestsRemaining => RequestsLimit - RequestsUsed;

        /// <summary>
        /// When the rate limit resets
        /// </summary>
        public DateTime? RateLimitReset { get; set; }

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