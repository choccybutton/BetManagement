using System;

namespace FootballAPIWrapper.Configuration
{
    public class FootballApiConfiguration
    {
        /// <summary>
        /// The API key for authentication with the Football API
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// The base URL for the Football API
        /// </summary>
        public string BaseUrl { get; set; } = "https://v3.football.api-sports.io";

        /// <summary>
        /// The RapidAPI host header value
        /// </summary>
        public string RapidApiHost { get; set; } = "v3.football.api-sports.io";

        /// <summary>
        /// Timeout in seconds for HTTP requests (default: 30 seconds)
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Whether to enable detailed usage statistics tracking
        /// </summary>
        public bool EnableUsageTracking { get; set; } = true;

        /// <summary>
        /// Validates the configuration settings
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrEmpty(ApiKey))
                throw new ArgumentException("ApiKey is required", nameof(ApiKey));
            
            if (string.IsNullOrEmpty(BaseUrl))
                throw new ArgumentException("BaseUrl is required", nameof(BaseUrl));

            if (string.IsNullOrEmpty(RapidApiHost))
                throw new ArgumentException("RapidApiHost is required", nameof(RapidApiHost));

            if (TimeoutSeconds <= 0)
                throw new ArgumentException("TimeoutSeconds must be greater than 0", nameof(TimeoutSeconds));
        }
    }
}