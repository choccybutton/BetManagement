using System.Threading.Tasks;
using FootballAPIWrapper.Models;

namespace FootballAPIWrapper.Services
{
    public class TeamService
    {
        private readonly IFootballApiClient _apiClient;

        public TeamService(IFootballApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new System.ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Gets teams by various filters
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <param name="name">Team name</param>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <param name="country">Country name</param>
        /// <param name="code">Country code</param>
        /// <param name="venue">Venue ID</param>
        /// <param name="search">Search term</param>
        /// <returns>API response containing teams</returns>
        public async Task<ApiResponse<Team>> GetTeamsAsync(
            int? id = null,
            string name = null,
            int? league = null,
            int? season = null,
            string country = null,
            string code = null,
            int? venue = null,
            string search = null)
        {
            var parameters = new
            {
                id,
                name,
                league,
                season,
                country,
                code,
                venue,
                search
            };

            return await _apiClient.GetAsync<ApiResponse<Team>>("teams", parameters);
        }

        /// <summary>
        /// Gets a team by ID
        /// </summary>
        /// <param name="id">Team ID</param>
        /// <returns>API response containing the specific team</returns>
        public async Task<ApiResponse<Team>> GetTeamByIdAsync(int id)
        {
            return await GetTeamsAsync(id: id);
        }

        /// <summary>
        /// Gets teams by league and season
        /// </summary>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <returns>API response containing teams from the specified league and season</returns>
        public async Task<ApiResponse<Team>> GetTeamsByLeagueAsync(int league, int season)
        {
            return await GetTeamsAsync(league: league, season: season);
        }

        /// <summary>
        /// Gets teams by country
        /// </summary>
        /// <param name="country">Country name</param>
        /// <returns>API response containing teams from the specified country</returns>
        public async Task<ApiResponse<Team>> GetTeamsByCountryAsync(string country)
        {
            return await GetTeamsAsync(country: country);
        }

        /// <summary>
        /// Searches for teams by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>API response containing matching teams</returns>
        public async Task<ApiResponse<Team>> SearchTeamsAsync(string searchTerm)
        {
            return await GetTeamsAsync(search: searchTerm);
        }

        /// <summary>
        /// Gets teams statistics for a specific league and season
        /// </summary>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <param name="team">Team ID (optional)</param>
        /// <param name="date">Date (optional)</param>
        /// <returns>API response containing team statistics</returns>
        public async Task<string> GetTeamStatisticsAsync(int league, int season, int? team = null, string date = null)
        {
            var parameters = new
            {
                league,
                season,
                team,
                date
            };

            return await _apiClient.GetRawAsync("teams/statistics", parameters);
        }
    }
}