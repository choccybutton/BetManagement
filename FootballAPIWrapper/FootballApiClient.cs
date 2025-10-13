using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FootballAPIWrapper.Configuration;
using FootballAPIWrapper.Usage;
using Newtonsoft.Json;

namespace FootballAPIWrapper
{
    public class FootballApiClient : IFootballApiClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly FootballApiConfiguration _configuration;
        private readonly IUsageTracker _usageTracker;
        private bool _disposed = false;

        public FootballApiClient(HttpClient httpClient, FootballApiConfiguration configuration, IUsageTracker usageTracker)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _usageTracker = usageTracker ?? throw new ArgumentNullException(nameof(usageTracker));
            
            _configuration.Validate();
            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_configuration.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(_configuration.TimeoutSeconds);
            
            // Set required headers for RapidAPI
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", _configuration.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", _configuration.RapidApiHost);
        }

        public ApiUsageStatistics GetUsageStatistics()
        {
            return _usageTracker.GetUsageStatistics();
        }

        public async Task<T> GetAsync<T>(string endpoint, object queryParameters = null)
        {
            var json = await GetRawAsync(endpoint, queryParameters);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<string> GetRawAsync(string endpoint, object queryParameters = null)
        {
            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentException("Endpoint cannot be null or empty", nameof(endpoint));

            var url = BuildUrl(endpoint, queryParameters);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await _httpClient.GetAsync(url);
                stopwatch.Stop();

                if (_configuration.EnableUsageTracking)
                {
                    await _usageTracker.RecordApiCallAsync(response, stopwatch.Elapsed.TotalMilliseconds);
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                if (_configuration.EnableUsageTracking)
                {
                    await _usageTracker.RecordFailedCallAsync(endpoint, ex);
                }
                
                throw new HttpRequestException($"Failed to call API endpoint '{endpoint}': {ex.Message}", ex);
            }
        }

        private string BuildUrl(string endpoint, object queryParameters)
        {
            var url = new StringBuilder(endpoint.TrimStart('/'));

            if (queryParameters != null)
            {
                var queryString = BuildQueryString(queryParameters);
                if (!string.IsNullOrEmpty(queryString))
                {
                    url.Append(url.ToString().Contains("?") ? "&" : "?");
                    url.Append(queryString);
                }
            }

            return url.ToString();
        }

        private string BuildQueryString(object parameters)
        {
            if (parameters == null) return string.Empty;

            var queryParts = new StringBuilder();
            var properties = parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value = property.GetValue(parameters);
                if (value != null)
                {
                    if (queryParts.Length > 0)
                        queryParts.Append("&");
                    
                    var encodedName = HttpUtility.UrlEncode(property.Name.ToLowerInvariant());
                    var encodedValue = HttpUtility.UrlEncode(value.ToString());
                    queryParts.Append($"{encodedName}={encodedValue}");
                }
            }

            return queryParts.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}