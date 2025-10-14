using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FootballAPIWrapper.Usage
{
    public class UsageTracker : IUsageTracker
    {
        private readonly ApiUsageStatistics _statistics;
        private readonly object _lock = new object();
        private readonly ILogger<UsageTracker>? _logger;

        public UsageTracker(ILogger<UsageTracker>? logger = null)
        {
            _logger = logger;
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

                // Log all response headers for debugging
                _logger?.LogInformation("API Response Headers:");
                foreach (var header in response.Headers)
                {
                    _logger?.LogInformation("{HeaderName}: {HeaderValue}", header.Key, string.Join(", ", header.Value));
                }
                
                // Extract rate limit information from headers
                // Check multiple possible header names for different APIs
                
                _logger?.LogInformation("=== Processing Rate Limit Headers ===");
                
                // Process Daily Rate Limits (x-ratelimit-*)
                if (response.Headers.Contains("x-ratelimit-requests-limit"))
                {
                    var limitValue = response.Headers.GetValues("x-ratelimit-requests-limit").FirstOrDefault();
                    _logger?.LogInformation("Found x-ratelimit-requests-limit header (Daily): {Value}", limitValue);
                    if (int.TryParse(limitValue, out int dailyLimit))
                    {
                        _statistics.DailyRequestsLimit = dailyLimit;
                        _logger?.LogInformation("Set DailyRequestsLimit to: {Limit}", dailyLimit);
                    }
                    else
                    {
                        _logger?.LogWarning("Failed to parse x-ratelimit-requests-limit value: {Value}", limitValue);
                    }
                }
                else
                {
                    _logger?.LogInformation("No x-ratelimit-requests-limit header found");
                }
                
                if (response.Headers.Contains("x-ratelimit-requests-remaining"))
                {
                    var remainingValue = response.Headers.GetValues("x-ratelimit-requests-remaining").FirstOrDefault();
                    _logger?.LogInformation("Found x-ratelimit-requests-remaining header (Daily): {Value}", remainingValue);
                    if (int.TryParse(remainingValue, out int dailyRemaining))
                    {
                        var previousRemaining = _statistics.DailyRequestsRemaining;
                        _statistics.DailyRequestsRemaining = dailyRemaining;
                        _logger?.LogInformation("Set DailyRequestsRemaining to: {Remaining} (was {Previous})", dailyRemaining, previousRemaining);
                        _logger?.LogInformation("Calculated DailyRequestsUsed: {Used} (Limit: {Limit} - Remaining: {Remaining})", 
                            _statistics.DailyRequestsUsed, _statistics.DailyRequestsLimit, dailyRemaining);
                    }
                    else
                    {
                        _logger?.LogWarning("Failed to parse x-ratelimit-requests-remaining value: {Value}", remainingValue);
                    }
                }
                else
                {
                    _logger?.LogInformation("No x-ratelimit-requests-remaining header found");
                }
                
                // Process Per-Minute Rate Limits (X-RateLimit-*)
                if (response.Headers.Contains("X-RateLimit-Limit"))
                {
                    var limitValue = response.Headers.GetValues("X-RateLimit-Limit").FirstOrDefault();
                    _logger?.LogInformation("Found X-RateLimit-Limit header (Per-Minute): {Value}", limitValue);
                    if (int.TryParse(limitValue, out int perMinuteLimit))
                    {
                        _statistics.PerMinuteLimit = perMinuteLimit;
                        _logger?.LogInformation("Set PerMinuteLimit to: {Limit}", perMinuteLimit);
                    }
                    else
                    {
                        _logger?.LogWarning("Failed to parse X-RateLimit-Limit value: {Value}", limitValue);
                    }
                }
                else
                {
                    _logger?.LogInformation("No X-RateLimit-Limit header found");
                }
                
                if (response.Headers.Contains("X-RateLimit-Remaining"))
                {
                    var remainingValue = response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault();
                    _logger?.LogInformation("Found X-RateLimit-Remaining header (Per-Minute): {Value}", remainingValue);
                    if (int.TryParse(remainingValue, out int perMinuteRemaining))
                    {
                        var previousRemaining = _statistics.PerMinuteRemaining;
                        _statistics.PerMinuteRemaining = perMinuteRemaining;
                        _logger?.LogInformation("Set PerMinuteRemaining to: {Remaining} (was {Previous})", perMinuteRemaining, previousRemaining);
                        _logger?.LogInformation("Calculated PerMinuteUsed: {Used} (Limit: {Limit} - Remaining: {Remaining})", 
                            _statistics.PerMinuteUsed, _statistics.PerMinuteLimit, perMinuteRemaining);
                    }
                    else
                    {
                        _logger?.LogWarning("Failed to parse X-RateLimit-Remaining value: {Value}", remainingValue);
                    }
                }
                else
                {
                    _logger?.LogInformation("No X-RateLimit-Remaining header found");
                }

                // Check for reset time in various formats
                if (response.Headers.Contains("x-ratelimit-reset"))
                {
                    var resetValue = response.Headers.GetValues("x-ratelimit-reset").FirstOrDefault();
                    _logger?.LogInformation("Found x-ratelimit-reset header: {Value}", resetValue);
                    if (long.TryParse(resetValue, out long resetTimestamp))
                    {
                        var resetDateTime = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp).DateTime;
                        _statistics.RateLimitReset = resetDateTime;
                        _logger?.LogInformation("Set RateLimitReset to: {ResetTime}", resetDateTime);
                    }
                    else
                    {
                        _logger?.LogWarning("Failed to parse x-ratelimit-reset value: {Value}", resetValue);
                    }
                }
                else if (response.Headers.Contains("X-RateLimit-Reset"))
                {
                    var resetValue = response.Headers.GetValues("X-RateLimit-Reset").FirstOrDefault();
                    _logger?.LogInformation("Found X-RateLimit-Reset header: {Value}", resetValue);
                    if (long.TryParse(resetValue, out long resetTimestamp))
                    {
                        var resetDateTime = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp).DateTime;
                        _statistics.RateLimitReset = resetDateTime;
                        _logger?.LogInformation("Set RateLimitReset to: {ResetTime}", resetDateTime);
                    }
                    else
                    {
                        _logger?.LogWarning("Failed to parse X-RateLimit-Reset value: {Value}", resetValue);
                    }
                }
                else
                {
                    _logger?.LogInformation("No reset time header found");
                }

                if (!response.IsSuccessStatusCode)
                {
                    _statistics.FailedRequests++;
                    _logger?.LogWarning("API call failed with status: {StatusCode}", response.StatusCode);
                }
                
                // Log final statistics summary
                _logger?.LogInformation("=== Final Usage Statistics ===");
                _logger?.LogInformation("Daily Limits - Used: {DailyUsed}, Limit: {DailyLimit}, Remaining: {DailyRemaining}", 
                    _statistics.DailyRequestsUsed, _statistics.DailyRequestsLimit, _statistics.DailyRequestsRemaining);
                _logger?.LogInformation("Per-Minute Limits - Used: {MinuteUsed}, Limit: {MinuteLimit}, Remaining: {MinuteRemaining}", 
                    _statistics.PerMinuteUsed, _statistics.PerMinuteLimit, _statistics.PerMinuteRemaining);
                _logger?.LogInformation("TotalApiCalls: {Total}, FailedRequests: {Failed}, SuccessRate: {SuccessRate:F1}%", 
                    _statistics.TotalApiCalls, _statistics.FailedRequests, _statistics.SuccessRate);
                _logger?.LogInformation("AverageResponseTime: {AvgTime:F1}ms, RateLimitReset: {ResetTime}", 
                    _statistics.AverageResponseTimeMs, _statistics.RateLimitReset?.ToString("yyyy-MM-dd HH:mm:ss") ?? "None");
                _logger?.LogInformation("=== End Statistics ===");
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
                    DailyRequestsLimit = _statistics.DailyRequestsLimit,
                    DailyRequestsRemaining = _statistics.DailyRequestsRemaining,
                    PerMinuteLimit = _statistics.PerMinuteLimit,
                    PerMinuteRemaining = _statistics.PerMinuteRemaining,
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
                _statistics.DailyRequestsLimit = 0;
                _statistics.DailyRequestsRemaining = 0;
                _statistics.PerMinuteLimit = 0;
                _statistics.PerMinuteRemaining = 0;
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