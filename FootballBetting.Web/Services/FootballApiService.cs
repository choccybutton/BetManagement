using System.Diagnostics;
using FootballAPIWrapper;
using FootballAPIWrapper.Models;
using FootballAPIWrapper.Services;

namespace FootballBetting.Web.Services;

public class FootballApiService
{
    private readonly FixtureService _fixtureService;
    private readonly ILogger<FootballApiService> _logger;
    private readonly IApiUsageService _apiUsageService;

    public FootballApiService(FixtureService fixtureService, ILogger<FootballApiService> logger, IApiUsageService apiUsageService)
    {
        _fixtureService = fixtureService ?? throw new ArgumentNullException(nameof(fixtureService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiUsageService = apiUsageService ?? throw new ArgumentNullException(nameof(apiUsageService));
    }

    /// <summary>
    /// Helper method to execute API calls with tracking
    /// </summary>
    private async Task<T> ExecuteWithTrackingAsync<T>(string endpoint, Func<Task<T>> apiCall)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await apiCall();
            stopwatch.Stop();
            
            _apiUsageService.TrackRequest(endpoint, true, (int)stopwatch.ElapsedMilliseconds);
            
            // Trigger additional usage update to refresh rate limit data
            _apiUsageService.TriggerUsageUpdate();
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _apiUsageService.TrackRequest(endpoint, false, (int)stopwatch.ElapsedMilliseconds, ex.Message);
            
            // Trigger additional usage update to refresh rate limit data even on failure
            _apiUsageService.TriggerUsageUpdate();
            
            throw;
        }
    }

    /// <summary>
    /// Gets fixtures for today that haven't finished yet
    /// </summary>
    /// <param name="limit">Maximum number of fixtures to return</param>
    /// <returns>List of today's upcoming/live fixtures</returns>
    public async Task<List<FixtureDetails>> GetTodayUpcomingFixturesAsync(int limit = 10)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var endpoint = $"fixtures/date/{today}";
        
        return await ExecuteWithTrackingAsync(endpoint, async () =>
        {
            try
            {
                var response = await _fixtureService.GetFixturesByDateAsync(today);

                if (response?.Response == null)
                {
                    _logger.LogWarning("No fixtures data received for today: {Date}", today);
                    return new List<FixtureDetails>();
                }

                var upcomingFixtures = response.Response
                    .Where(f => f.Fixture.Status.Short != "FT" && // Not finished
                               f.Fixture.Status.Short != "AET" && // Not finished after extra time
                               f.Fixture.Status.Short != "PEN") // Not finished on penalties
                    .OrderBy(f => f.Fixture.Date)
                    .Take(limit)
                    .ToList();

                _logger.LogInformation("Retrieved {Count} upcoming fixtures for today", upcomingFixtures.Count);
                return upcomingFixtures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving today's upcoming fixtures");
                return new List<FixtureDetails>();
            }
        });
    }

    /// <summary>
    /// Gets fixtures for a specific date
    /// </summary>
    /// <param name="date">Date to get fixtures for</param>
    /// <returns>List of fixtures for the specified date</returns>
    public async Task<List<FixtureDetails>> GetFixturesForDateAsync(DateTime date)
    {
        var dateString = date.ToString("yyyy-MM-dd");
        var endpoint = $"fixtures/date/{dateString}";
        
        return await ExecuteWithTrackingAsync(endpoint, async () =>
        {
            try
            {
                var response = await _fixtureService.GetFixturesByDateAsync(dateString);

                if (response?.Response == null)
                {
                    _logger.LogWarning("No fixtures data received for date: {Date}", dateString);
                    return new List<FixtureDetails>();
                }

                var fixtures = response.Response
                    .OrderBy(f => f.Fixture.Date)
                    .ToList();

                _logger.LogInformation("Retrieved {Count} fixtures for date {Date}", fixtures.Count, dateString);
                return fixtures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fixtures for date {Date}", date);
                return new List<FixtureDetails>();
            }
        });
    }

    /// <summary>
    /// Gets fixtures for yesterday
    /// </summary>
    /// <returns>List of yesterday's fixtures</returns>
    public async Task<List<FixtureDetails>> GetYesterdayFixturesAsync()
    {
        var yesterday = DateTime.UtcNow.AddDays(-1);
        return await GetFixturesForDateAsync(yesterday);
    }

    /// <summary>
    /// Gets fixtures for today
    /// </summary>
    /// <returns>List of today's fixtures</returns>
    public async Task<List<FixtureDetails>> GetTodayFixturesAsync()
    {
        var today = DateTime.UtcNow;
        return await GetFixturesForDateAsync(today);
    }

    /// <summary>
    /// Gets fixtures for tomorrow
    /// </summary>
    /// <returns>List of tomorrow's fixtures</returns>
    public async Task<List<FixtureDetails>> GetTomorrowFixturesAsync()
    {
        var tomorrow = DateTime.UtcNow.AddDays(1);
        return await GetFixturesForDateAsync(tomorrow);
    }

    /// <summary>
    /// Gets live fixtures
    /// </summary>
    /// <returns>List of currently live fixtures</returns>
    public async Task<List<FixtureDetails>> GetLiveFixturesAsync()
    {
        var endpoint = "fixtures/live";
        
        return await ExecuteWithTrackingAsync(endpoint, async () =>
        {
            try
            {
                var response = await _fixtureService.GetLiveFixturesAsync();

                if (response?.Response == null)
                {
                    _logger.LogWarning("No live fixtures data received");
                    return new List<FixtureDetails>();
                }

                var liveFixtures = response.Response
                    .OrderBy(f => f.Fixture.Date)
                    .ToList();

                _logger.LogInformation("Retrieved {Count} live fixtures", liveFixtures.Count);
                return liveFixtures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving live fixtures");
                return new List<FixtureDetails>();
            }
        });
    }

    /// <summary>
    /// Gets a specific fixture by ID
    /// </summary>
    /// <param name="fixtureId">The ID of the fixture to retrieve</param>
    /// <returns>The fixture details if found, otherwise null</returns>
    public async Task<FixtureDetails?> GetFixtureByIdAsync(int fixtureId)
    {
        var endpoint = $"fixtures/id/{fixtureId}";
        
        return await ExecuteWithTrackingAsync(endpoint, async () =>
        {
            try
            {
                var response = await _fixtureService.GetFixtureByIdAsync(fixtureId);

                if (response?.Response == null || !response.Response.Any())
                {
                    _logger.LogWarning("No fixture data received for ID: {FixtureId}", fixtureId);
                    return null;
                }

                var fixture = response.Response.FirstOrDefault();
                _logger.LogInformation("Retrieved fixture {FixtureId}: {HomeTeam} vs {AwayTeam}", 
                    fixtureId, fixture?.Teams?.Home?.Name, fixture?.Teams?.Away?.Name);
                
                return fixture;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fixture {FixtureId}", fixtureId);
                return null;
            }
        });
    }

    /// <summary>
    /// Gets head-to-head fixtures between two teams with home/away configuration
    /// </summary>
    /// <param name="homeTeamId">Home team ID</param>
    /// <param name="awayTeamId">Away team ID</param>
    /// <param name="last">Number of last fixtures to retrieve (default: 5)</param>
    /// <returns>List of head-to-head fixtures with specified home/away configuration</returns>
    public async Task<List<FixtureDetails>> GetHeadToHeadFixturesAsync(int homeTeamId, int awayTeamId, int last = 5)
    {
        var h2hString = $"{homeTeamId}-{awayTeamId}";
        var endpoint = $"fixtures/headtohead/{h2hString}";
        
        return await ExecuteWithTrackingAsync(endpoint, async () =>
        {
            try
            {
                var response = await _fixtureService.GetHeadToHeadAsync(h2hString, last: last);

                if (response?.Response == null)
                {
                    _logger.LogWarning("No head-to-head data received for teams {HomeTeam} vs {AwayTeam}", homeTeamId, awayTeamId);
                    return new List<FixtureDetails>();
                }

                // Filter to only include matches with the specific home/away configuration
                var filteredFixtures = response.Response
                    .Where(f => f.Teams.Home.Id == homeTeamId && f.Teams.Away.Id == awayTeamId)
                    .Where(f => f.Fixture.Status.Short == "FT" || f.Fixture.Status.Short == "AET" || f.Fixture.Status.Short == "PEN") // Only finished matches
                    .OrderByDescending(f => f.Fixture.Date)
                    .Take(last)
                    .ToList();

                _logger.LogInformation("Retrieved {Count} head-to-head fixtures for {HomeTeam} vs {AwayTeam}", 
                    filteredFixtures.Count, homeTeamId, awayTeamId);
                
                return filteredFixtures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving head-to-head fixtures for teams {HomeTeam} vs {AwayTeam}", homeTeamId, awayTeamId);
                return new List<FixtureDetails>();
            }
        });
    }

    /// <summary>
    /// Gets head-to-head fixtures between two teams with opposite home/away configuration
    /// </summary>
    /// <param name="homeTeamId">Home team ID (will be away in the returned fixtures)</param>
    /// <param name="awayTeamId">Away team ID (will be home in the returned fixtures)</param>
    /// <param name="last">Number of last fixtures to retrieve (default: 5)</param>
    /// <returns>List of head-to-head fixtures with opposite home/away configuration</returns>
    public async Task<List<FixtureDetails>> GetHeadToHeadOppositeFixturesAsync(int homeTeamId, int awayTeamId, int last = 5)
    {
        var h2hString = $"{awayTeamId}-{homeTeamId}";
        var endpoint = $"fixtures/headtohead/{h2hString}";
        
        return await ExecuteWithTrackingAsync(endpoint, async () =>
        {
            try
            {
                var response = await _fixtureService.GetHeadToHeadAsync(h2hString, last: last);

                if (response?.Response == null)
                {
                    _logger.LogWarning("No head-to-head opposite data received for teams {HomeTeam} vs {AwayTeam}", homeTeamId, awayTeamId);
                    return new List<FixtureDetails>();
                }

                // Filter to only include matches with the opposite home/away configuration
                var filteredFixtures = response.Response
                    .Where(f => f.Teams.Home.Id == awayTeamId && f.Teams.Away.Id == homeTeamId)
                    .Where(f => f.Fixture.Status.Short == "FT" || f.Fixture.Status.Short == "AET" || f.Fixture.Status.Short == "PEN") // Only finished matches
                    .OrderByDescending(f => f.Fixture.Date)
                    .Take(last)
                    .ToList();

                _logger.LogInformation("Retrieved {Count} head-to-head opposite fixtures for {HomeTeam} vs {AwayTeam}", 
                    filteredFixtures.Count, homeTeamId, awayTeamId);
                
                return filteredFixtures;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving head-to-head opposite fixtures for teams {HomeTeam} vs {AwayTeam}", homeTeamId, awayTeamId);
                return new List<FixtureDetails>();
            }
        });
    }

    /// <summary>
    /// Gets comprehensive head-to-head data for two teams (both home/away configurations)
    /// </summary>
    /// <param name="homeTeamId">Home team ID</param>
    /// <param name="awayTeamId">Away team ID</param>
    /// <param name="last">Number of last fixtures to retrieve for each configuration (default: 5)</param>
    /// <returns>Tuple containing same H2H and opposite H2H fixtures</returns>
    public async Task<(List<FixtureDetails> SameConfiguration, List<FixtureDetails> OppositeConfiguration)> GetComprehensiveHeadToHeadAsync(int homeTeamId, int awayTeamId, int last = 5)
    {
        try
        {
            var sameConfigTask = GetHeadToHeadFixturesAsync(homeTeamId, awayTeamId, last);
            var oppositeConfigTask = GetHeadToHeadOppositeFixturesAsync(homeTeamId, awayTeamId, last);

            await Task.WhenAll(sameConfigTask, oppositeConfigTask);

            var sameConfig = await sameConfigTask;
            var oppositeConfig = await oppositeConfigTask;

            _logger.LogInformation("Retrieved comprehensive H2H data: {SameCount} same config, {OppositeCount} opposite config for teams {HomeTeam} vs {AwayTeam}",
                sameConfig.Count, oppositeConfig.Count, homeTeamId, awayTeamId);

            return (sameConfig, oppositeConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving comprehensive head-to-head data for teams {HomeTeam} vs {AwayTeam}", homeTeamId, awayTeamId);
            return (new List<FixtureDetails>(), new List<FixtureDetails>());
        }
    }
}