using System.Threading.Tasks;
using FootballAPIWrapper.Models;

namespace FootballAPIWrapper.Services
{
    public class PlayerService
    {
        private readonly IFootballApiClient _apiClient;

        public PlayerService(IFootballApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new System.ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Gets players by various filters
        /// </summary>
        /// <param name="id">Player ID</param>
        /// <param name="team">Team ID</param>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="search">Search term</param>
        /// <returns>API response containing players</returns>
        public async Task<ApiResponse<PlayerStatistics>> GetPlayersAsync(
            int? id = null,
            int? team = null,
            int? league = null,
            int? season = null,
            int? page = null,
            string search = null)
        {
            var parameters = new
            {
                id,
                team,
                league,
                season,
                page,
                search
            };

            return await _apiClient.GetAsync<ApiResponse<PlayerStatistics>>("players", parameters);
        }

        /// <summary>
        /// Gets a player by ID
        /// </summary>
        /// <param name="id">Player ID</param>
        /// <param name="season">Season year</param>
        /// <returns>API response containing the specific player</returns>
        public async Task<ApiResponse<PlayerStatistics>> GetPlayerByIdAsync(int id, int season)
        {
            return await GetPlayersAsync(id: id, season: season);
        }

        /// <summary>
        /// Gets players by team
        /// </summary>
        /// <param name="team">Team ID</param>
        /// <param name="season">Season year</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>API response containing players from the specified team</returns>
        public async Task<ApiResponse<PlayerStatistics>> GetPlayersByTeamAsync(int team, int season, int? page = null)
        {
            return await GetPlayersAsync(team: team, season: season, page: page);
        }

        /// <summary>
        /// Gets players by league
        /// </summary>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <param name="team">Team ID (optional)</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>API response containing players from the specified league</returns>
        public async Task<ApiResponse<PlayerStatistics>> GetPlayersByLeagueAsync(int league, int season, int? team = null, int? page = null)
        {
            return await GetPlayersAsync(league: league, season: season, team: team, page: page);
        }

        /// <summary>
        /// Searches for players by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="league">League ID (optional)</param>
        /// <param name="team">Team ID (optional)</param>
        /// <param name="season">Season year (optional)</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>API response containing matching players</returns>
        public async Task<ApiResponse<PlayerStatistics>> SearchPlayersAsync(string searchTerm, int? league = null, int? team = null, int? season = null, int? page = null)
        {
            return await GetPlayersAsync(search: searchTerm, league: league, team: team, season: season, page: page);
        }

        /// <summary>
        /// Gets top scorers for a league and season
        /// </summary>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <returns>API response containing top scorers</returns>
        public async Task<ApiResponse<PlayerStatistics>> GetTopScorersAsync(int league, int season)
        {
            return await _apiClient.GetAsync<ApiResponse<PlayerStatistics>>("players/topscorers", new { league, season });
        }

        /// <summary>
        /// Gets top assists for a league and season
        /// </summary>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <returns>API response containing top assists</returns>
        public async Task<ApiResponse<PlayerStatistics>> GetTopAssistsAsync(int league, int season)
        {
            return await _apiClient.GetAsync<ApiResponse<PlayerStatistics>>("players/topassists", new { league, season });
        }

        /// <summary>
        /// Gets top yellow cards for a league and season
        /// </summary>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <returns>API response containing top yellow cards</returns>
        public async Task<ApiResponse<PlayerStatistics>> GetTopYellowCardsAsync(int league, int season)
        {
            return await _apiClient.GetAsync<ApiResponse<PlayerStatistics>>("players/topyellowcards", new { league, season });
        }

        /// <summary>
        /// Gets top red cards for a league and season
        /// </summary>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <returns>API response containing top red cards</returns>
        public async Task<ApiResponse<PlayerStatistics>> GetTopRedCardsAsync(int league, int season)
        {
            return await _apiClient.GetAsync<ApiResponse<PlayerStatistics>>("players/topredcards", new { league, season });
        }

        /// <summary>
        /// Gets players squads for a team
        /// </summary>
        /// <param name="team">Team ID</param>
        /// <returns>Raw JSON response containing team squad</returns>
        public async Task<string> GetTeamSquadAsync(int team)
        {
            return await _apiClient.GetRawAsync("players/squads", new { team });
        }
    }
}