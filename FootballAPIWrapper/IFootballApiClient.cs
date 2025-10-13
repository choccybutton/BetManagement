using System.Threading.Tasks;
using FootballAPIWrapper.Usage;

namespace FootballAPIWrapper
{
    public interface IFootballApiClient
    {
        /// <summary>
        /// Gets the current API usage statistics
        /// </summary>
        ApiUsageStatistics GetUsageStatistics();

        /// <summary>
        /// Makes a GET request to the specified endpoint
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to</typeparam>
        /// <param name="endpoint">The API endpoint path</param>
        /// <param name="queryParameters">Optional query parameters</param>
        /// <returns>The deserialized response</returns>
        Task<T> GetAsync<T>(string endpoint, object queryParameters = null);

        /// <summary>
        /// Makes a raw GET request to the specified endpoint
        /// </summary>
        /// <param name="endpoint">The API endpoint path</param>
        /// <param name="queryParameters">Optional query parameters</param>
        /// <returns>The raw JSON response</returns>
        Task<string> GetRawAsync(string endpoint, object queryParameters = null);
    }
}