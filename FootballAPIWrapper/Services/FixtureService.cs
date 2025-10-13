using System;
using System.Threading.Tasks;
using FootballAPIWrapper.Models;

namespace FootballAPIWrapper.Services
{
    public class FixtureService
    {
        private readonly IFootballApiClient _apiClient;

        public FixtureService(IFootballApiClient apiClient)
        {
            _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        /// <summary>
        /// Gets fixtures by various filters
        /// </summary>
        /// <param name="id">Fixture ID</param>
        /// <param name="ids">Comma-separated fixture IDs</param>
        /// <param name="live">Live fixtures only</param>
        /// <param name="date">Date in YYYY-MM-DD format</param>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <param name="team">Team ID</param>
        /// <param name="last">Last N fixtures</param>
        /// <param name="next">Next N fixtures</param>
        /// <param name="from">From date in YYYY-MM-DD format</param>
        /// <param name="to">To date in YYYY-MM-DD format</param>
        /// <param name="round">Round number</param>
        /// <param name="status">Fixture status</param>
        /// <param name="venue">Venue ID</param>
        /// <param name="timezone">Timezone</param>
        /// <returns>API response containing fixtures</returns>
        public async Task<ApiResponse<FixtureDetails>> GetFixturesAsync(
            int? id = null,
            string ids = null,
            string live = null,
            string date = null,
            int? league = null,
            int? season = null,
            int? team = null,
            int? last = null,
            int? next = null,
            string from = null,
            string to = null,
            string round = null,
            string status = null,
            int? venue = null,
            string timezone = null)
        {
            var parameters = new
            {
                id,
                ids,
                live,
                date,
                league,
                season,
                team,
                last,
                next,
                from,
                to,
                round,
                status,
                venue,
                timezone
            };

            return await _apiClient.GetAsync<ApiResponse<FixtureDetails>>("fixtures", parameters);
        }

        /// <summary>
        /// Gets a fixture by ID
        /// </summary>
        /// <param name="id">Fixture ID</param>
        /// <param name="timezone">Timezone</param>
        /// <returns>API response containing the specific fixture</returns>
        public async Task<ApiResponse<FixtureDetails>> GetFixtureByIdAsync(int id, string timezone = null)
        {
            return await GetFixturesAsync(id: id, timezone: timezone);
        }

        /// <summary>
        /// Gets fixtures by date
        /// </summary>
        /// <param name="date">Date in YYYY-MM-DD format</param>
        /// <param name="league">League ID (optional)</param>
        /// <param name="timezone">Timezone</param>
        /// <returns>API response containing fixtures for the specified date</returns>
        public async Task<ApiResponse<FixtureDetails>> GetFixturesByDateAsync(string date, int? league = null, string timezone = null)
        {
            return await GetFixturesAsync(date: date, league: league, timezone: timezone);
        }

        /// <summary>
        /// Gets fixtures by league and season
        /// </summary>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <param name="team">Team ID (optional)</param>
        /// <param name="round">Round (optional)</param>
        /// <param name="timezone">Timezone</param>
        /// <returns>API response containing fixtures for the specified league and season</returns>
        public async Task<ApiResponse<FixtureDetails>> GetFixturesByLeagueAsync(int league, int season, int? team = null, string round = null, string timezone = null)
        {
            return await GetFixturesAsync(league: league, season: season, team: team, round: round, timezone: timezone);
        }

        /// <summary>
        /// Gets fixtures by team
        /// </summary>
        /// <param name="team">Team ID</param>
        /// <param name="season">Season year</param>
        /// <param name="league">League ID (optional)</param>
        /// <param name="last">Last N fixtures</param>
        /// <param name="next">Next N fixtures</param>
        /// <param name="timezone">Timezone</param>
        /// <returns>API response containing fixtures for the specified team</returns>
        public async Task<ApiResponse<FixtureDetails>> GetFixturesByTeamAsync(int team, int season, int? league = null, int? last = null, int? next = null, string timezone = null)
        {
            return await GetFixturesAsync(team: team, season: season, league: league, last: last, next: next, timezone: timezone);
        }

        /// <summary>
        /// Gets live fixtures
        /// </summary>
        /// <param name="league">League ID (optional)</param>
        /// <param name="timezone">Timezone</param>
        /// <returns>API response containing live fixtures</returns>
        public async Task<ApiResponse<FixtureDetails>> GetLiveFixturesAsync(int? league = null, string timezone = null)
        {
            return await GetFixturesAsync(live: "all", league: league, timezone: timezone);
        }

        /// <summary>
        /// Gets fixtures in a date range
        /// </summary>
        /// <param name="from">From date in YYYY-MM-DD format</param>
        /// <param name="to">To date in YYYY-MM-DD format</param>
        /// <param name="league">League ID (optional)</param>
        /// <param name="team">Team ID (optional)</param>
        /// <param name="timezone">Timezone</param>
        /// <returns>API response containing fixtures in the specified date range</returns>
        public async Task<ApiResponse<FixtureDetails>> GetFixturesInRangeAsync(string from, string to, int? league = null, int? team = null, string timezone = null)
        {
            return await GetFixturesAsync(from: from, to: to, league: league, team: team, timezone: timezone);
        }

        /// <summary>
        /// Gets head to head fixtures between two teams
        /// </summary>
        /// <param name="h2h">Team IDs in format "team1-team2"</param>
        /// <param name="date">Date in YYYY-MM-DD format</param>
        /// <param name="league">League ID</param>
        /// <param name="season">Season year</param>
        /// <param name="last">Last N fixtures</param>
        /// <param name="next">Next N fixtures</param>
        /// <param name="from">From date in YYYY-MM-DD format</param>
        /// <param name="to">To date in YYYY-MM-DD format</param>
        /// <param name="status">Fixture status</param>
        /// <param name="venue">Venue ID</param>
        /// <param name="timezone">Timezone</param>
        /// <returns>API response containing head to head fixtures</returns>
        public async Task<ApiResponse<FixtureDetails>> GetHeadToHeadAsync(
            string h2h,
            string date = null,
            int? league = null,
            int? season = null,
            int? last = null,
            int? next = null,
            string from = null,
            string to = null,
            string status = null,
            int? venue = null,
            string timezone = null)
        {
            var parameters = new
            {
                h2h,
                date,
                league,
                season,
                last,
                next,
                from,
                to,
                status,
                venue,
                timezone
            };

            return await _apiClient.GetAsync<ApiResponse<FixtureDetails>>("fixtures/headtohead", parameters);
        }

        /// <summary>
        /// Gets fixture statistics
        /// </summary>
        /// <param name="fixture">Fixture ID</param>
        /// <param name="team">Team ID (optional)</param>
        /// <param name="type">Statistics type (optional)</param>
        /// <returns>Raw JSON response containing fixture statistics</returns>
        public async Task<string> GetFixtureStatisticsAsync(int fixture, int? team = null, string type = null)
        {
            var parameters = new
            {
                fixture,
                team,
                type
            };

            return await _apiClient.GetRawAsync("fixtures/statistics", parameters);
        }

        /// <summary>
        /// Gets fixture events
        /// </summary>
        /// <param name="fixture">Fixture ID</param>
        /// <param name="team">Team ID (optional)</param>
        /// <param name="player">Player ID (optional)</param>
        /// <param name="type">Event type (optional)</param>
        /// <returns>Raw JSON response containing fixture events</returns>
        public async Task<string> GetFixtureEventsAsync(int fixture, int? team = null, int? player = null, string type = null)
        {
            var parameters = new
            {
                fixture,
                team,
                player,
                type
            };

            return await _apiClient.GetRawAsync("fixtures/events", parameters);
        }

        /// <summary>
        /// Gets fixture lineups
        /// </summary>
        /// <param name="fixture">Fixture ID</param>
        /// <param name="team">Team ID (optional)</param>
        /// <param name="player">Player ID (optional)</param>
        /// <param name="type">Lineup type (optional)</param>
        /// <returns>Raw JSON response containing fixture lineups</returns>
        public async Task<string> GetFixtureLineupsAsync(int fixture, int? team = null, int? player = null, string type = null)
        {
            var parameters = new
            {
                fixture,
                team,
                player,
                type
            };

            return await _apiClient.GetRawAsync("fixtures/lineups", parameters);
        }

        /// <summary>
        /// Gets fixture player statistics
        /// </summary>
        /// <param name="fixture">Fixture ID</param>
        /// <param name="team">Team ID (optional)</param>
        /// <returns>Raw JSON response containing fixture player statistics</returns>
        public async Task<string> GetFixturePlayerStatisticsAsync(int fixture, int? team = null)
        {
            var parameters = new
            {
                fixture,
                team
            };

            return await _apiClient.GetRawAsync("fixtures/players", parameters);
        }
    }
}