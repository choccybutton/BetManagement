using System.Net.Http;
using System.Threading.Tasks;

namespace FootballAPIWrapper.Usage
{
    public interface IUsageTracker
    {
        /// <summary>
        /// Records an API call and updates usage statistics
        /// </summary>
        /// <param name="response">The HTTP response from the API</param>
        /// <param name="responseTimeMs">Response time in milliseconds</param>
        Task RecordApiCallAsync(HttpResponseMessage response, double responseTimeMs);

        /// <summary>
        /// Records a failed API call
        /// </summary>
        /// <param name="endpoint">The endpoint that failed</param>
        /// <param name="exception">The exception that occurred</param>
        Task RecordFailedCallAsync(string endpoint, System.Exception exception);

        /// <summary>
        /// Gets the current usage statistics
        /// </summary>
        /// <returns>Current API usage statistics</returns>
        ApiUsageStatistics GetUsageStatistics();

        /// <summary>
        /// Resets the usage tracking statistics
        /// </summary>
        void ResetStatistics();
    }
}