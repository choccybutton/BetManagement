using System.Threading.Tasks;
using FootballAPIWrapper.Models;

namespace FootballAPIWrapper.Services
{
    public class LeagueService
    {
        private readonly IFootballApiClient _apiClient;

        public LeagueService(IFootballApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new System.ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Gets all available leagues
        /// </summary>
        /// <param name="name">Filter by league name</param>
        /// <param name="country">Filter by country name</param>
        /// <param name="code">Filter by country code</param>
        /// <param name="season">Filter by season year</param>
        /// <param name="id">Filter by league ID</param>
        /// <param name="search">Search term for leagues</param>
        /// <param name="type">Filter by league type (league or cup)</param>
        /// <param name="current">Filter by current season only</param>
        /// <returns>API response containing leagues</returns>
        public async Task<ApiResponse<League>> GetLeaguesAsync(
            string name = null,
            string country = null,
            string code = null,
            int? season = null,
            int? id = null,
            string search = null,
            string type = null,
            bool? current = null)
        {
            var parameters = new
            {
                name,
                country,
                code,
                season,
                id,
                search,
                type,
                current
            };

            return await _apiClient.GetAsync<ApiResponse<League>>("leagues", parameters);
        }

        /// <summary>
        /// Gets leagues by ID
        /// </summary>
        /// <param name="id">League ID</param>
        /// <returns>API response containing the specific league</returns>
        public async Task<ApiResponse<League>> GetLeagueByIdAsync(int id)
        {
            return await GetLeaguesAsync(id: id);
        }

        /// <summary>
        /// Gets leagues by country
        /// </summary>
        /// <param name="country">Country name</param>
        /// <param name="current">Filter by current season only</param>
        /// <returns>API response containing leagues from the specified country</returns>
        public async Task<ApiResponse<League>> GetLeaguesByCountryAsync(string country, bool? current = null)
        {
            return await GetLeaguesAsync(country: country, current: current);
        }

        /// <summary>
        /// Searches for leagues by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>API response containing matching leagues</returns>
        public async Task<ApiResponse<League>> SearchLeaguesAsync(string searchTerm)
        {
            return await GetLeaguesAsync(search: searchTerm);
        }

        /// <summary>
        /// Gets leagues for a specific season
        /// </summary>
        /// <param name="season">Season year</param>
        /// <returns>API response containing leagues for the specified season</returns>
        public async Task<ApiResponse<League>> GetLeaguesBySeasonAsync(int season)
        {
            return await GetLeaguesAsync(season: season);
        }
    }
}