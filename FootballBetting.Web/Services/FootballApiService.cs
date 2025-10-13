using FootballAPIWrapper;
using FootballAPIWrapper.Models;
using FootballAPIWrapper.Services;

namespace FootballBetting.Web.Services;

public class FootballApiService
{
    private readonly FixtureService _fixtureService;
    private readonly ILogger<FootballApiService> _logger;

    public FootballApiService(FixtureService fixtureService, ILogger<FootballApiService> logger)
    {
        _fixtureService = fixtureService ?? throw new ArgumentNullException(nameof(fixtureService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets fixtures for today that haven't finished yet
    /// </summary>
    /// <param name="limit">Maximum number of fixtures to return</param>
    /// <returns>List of today's upcoming/live fixtures</returns>
    public async Task<List<FixtureDetails>> GetTodayUpcomingFixturesAsync(int limit = 10)
    {
        try
        {
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
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
    }

    /// <summary>
    /// Gets fixtures for a specific date
    /// </summary>
    /// <param name="date">Date to get fixtures for</param>
    /// <returns>List of fixtures for the specified date</returns>
    public async Task<List<FixtureDetails>> GetFixturesForDateAsync(DateTime date)
    {
        try
        {
            var dateString = date.ToString("yyyy-MM-dd");
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
    }
}