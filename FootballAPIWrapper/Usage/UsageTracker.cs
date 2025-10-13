using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FootballAPIWrapper.Usage
{
    public class UsageTracker : IUsageTracker
    {
        private readonly ApiUsageStatistics _statistics;
        private readonly object _lock = new object();

        public UsageTracker()
        {
            _statistics = new ApiUsageStatistics
            {
                TrackingStarted = DateTime.UtcNow
            };
        }

        public Task RecordApiCallAsync(HttpResponseMessage response, double responseTimeMs)
        {
            lock (_lock)
            {
                _statistics.TotalApiCalls++;
                _statistics.LastApiCall = DateTime.UtcNow;

                // Update average response time
                var totalCalls = _statistics.TotalApiCalls;
                _statistics.AverageResponseTimeMs = (_statistics.AverageResponseTimeMs * (totalCalls - 1) + responseTimeMs) / totalCalls;

                // Extract rate limit information from headers
                if (response.Headers.Contains("X-RateLimit-Requests"))
                {
                    if (int.TryParse(response.Headers.GetValues("X-RateLimit-Requests").FirstOrDefault(), out int used))
                    {
                        _statistics.RequestsUsed = used;
                    }
                }

                if (response.Headers.Contains("X-RateLimit-Limit"))
                {
                    if (int.TryParse(response.Headers.GetValues("X-RateLimit-Limit").FirstOrDefault(), out int limit))
                    {
                        _statistics.RequestsLimit = limit;
                    }
                }

                if (response.Headers.Contains("X-RateLimit-Reset"))
                {
                    if (long.TryParse(response.Headers.GetValues("X-RateLimit-Reset").FirstOrDefault(), out long resetTimestamp))
                    {
                        _statistics.RateLimitReset = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp).DateTime;
                    }
                }

                if (!response.IsSuccessStatusCode)
                {
                    _statistics.FailedRequests++;
                }
            }

            return Task.CompletedTask;
        }

        public Task RecordFailedCallAsync(string endpoint, Exception exception)
        {
            lock (_lock)
            {
                _statistics.TotalApiCalls++;
                _statistics.FailedRequests++;
                _statistics.LastApiCall = DateTime.UtcNow;
            }

            return Task.CompletedTask;
        }

        public ApiUsageStatistics GetUsageStatistics()
        {
            lock (_lock)
            {
                // Return a copy to prevent external modification
                return new ApiUsageStatistics
                {
                    RequestsUsed = _statistics.RequestsUsed,
                    RequestsLimit = _statistics.RequestsLimit,
                    RateLimitReset = _statistics.RateLimitReset,
                    TotalApiCalls = _statistics.TotalApiCalls,
                    TrackingStarted = _statistics.TrackingStarted,
                    LastApiCall = _statistics.LastApiCall,
                    AverageResponseTimeMs = _statistics.AverageResponseTimeMs,
                    FailedRequests = _statistics.FailedRequests
                };
            }
        }

        public void ResetStatistics()
        {
            lock (_lock)
            {
                _statistics.RequestsUsed = 0;
                _statistics.RequestsLimit = 0;
                _statistics.RateLimitReset = null;
                _statistics.TotalApiCalls = 0;
                _statistics.TrackingStarted = DateTime.UtcNow;
                _statistics.LastApiCall = null;
                _statistics.AverageResponseTimeMs = 0;
                _statistics.FailedRequests = 0;
            }
        }
    }
}