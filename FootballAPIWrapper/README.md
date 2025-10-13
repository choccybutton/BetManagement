# FootballAPIWrapper

A comprehensive .NET 8 C# wrapper library for the API-Football.com v3 API.

## Installation

Add the project reference to your solution:

```bash
dotnet add reference path/to/FootballAPIWrapper.csproj
```

Or include the project in your solution and reference it.

## Quick Start

### 1. Configure Dependency Injection

```csharp
using FootballAPIWrapper.Extensions;
using FootballAPIWrapper.Configuration;

// In your Program.cs or Startup.cs
services.AddFootballApi(config => {
    config.ApiKey = "your-rapidapi-key";
    config.BaseUrl = "https://v3.football.api-sports.io";
    config.RapidApiHost = "v3.football.api-sports.io";
});
```

### 2. Inject and Use Services

```csharp
using FootballAPIWrapper.Services;
using FootballAPIWrapper.Models;

public class FootballService
{
    private readonly LeagueService _leagueService;
    private readonly TeamService _teamService;
    private readonly FixtureService _fixtureService;
    private readonly PlayerService _playerService;

    public FootballService(IFootballApiClient apiClient)
    {
        _leagueService = new LeagueService(apiClient);
        _teamService = new TeamService(apiClient);
        _fixtureService = new FixtureService(apiClient);
        _playerService = new PlayerService(apiClient);
    }

    public async Task<ApiResponse<League>> GetPremierLeague()
    {
        return await _leagueService.GetLeagueByIdAsync(39);
    }
}
```

## Configuration

### FootballApiConfiguration Options

```csharp
public class FootballApiConfiguration
{
    public string ApiKey { get; set; }           // Required: Your RapidAPI key
    public string BaseUrl { get; set; }          // Default: "https://v3.football.api-sports.io"
    public string RapidApiHost { get; set; }     // Default: "v3.football.api-sports.io"
    public int TimeoutSeconds { get; set; }      // Default: 30 seconds
    public bool EnableUsageTracking { get; set; } // Default: true
}
```

### Manual Configuration

```csharp
var config = new FootballApiConfiguration
{
    ApiKey = "your-rapidapi-key",
    BaseUrl = "https://v3.football.api-sports.io",
    RapidApiHost = "v3.football.api-sports.io",
    TimeoutSeconds = 60,
    EnableUsageTracking = true
};

services.AddFootballApi(config);
```

## Available Services

### LeagueService

```csharp
// Get league by ID
var league = await _leagueService.GetLeagueByIdAsync(39); // Premier League

// Search leagues by country
var leagues = await _leagueService.GetLeaguesByCountryAsync("England");

// Get current season leagues
var currentLeagues = await _leagueService.GetLeaguesAsync(current: true);

// Search leagues by name
var searchResults = await _leagueService.SearchLeaguesAsync("Premier");
```

### TeamService

```csharp
// Get team by ID
var team = await _teamService.GetTeamByIdAsync(33); // Manchester United

// Get teams by league and season
var teams = await _teamService.GetTeamsByLeagueAsync(39, 2023);

// Search teams
var searchResults = await _teamService.SearchTeamsAsync("Manchester");

// Get team statistics
var stats = await _teamService.GetTeamStatisticsAsync(39, 2023, 33);
```

### FixtureService

```csharp
// Get fixture by ID
var fixture = await _fixtureService.GetFixtureByIdAsync(215662);

// Get fixtures by date
var todayFixtures = await _fixtureService.GetFixturesByDateAsync("2023-12-25");

// Get live fixtures
var liveFixtures = await _fixtureService.GetLiveFixturesAsync();

// Head-to-head fixtures
var h2h = await _fixtureService.GetHeadToHeadAsync("33-34");

// Get fixture statistics
var fixtureStats = await _fixtureService.GetFixtureStatisticsAsync(215662);

// Get fixture events (goals, cards, substitutions)
var events = await _fixtureService.GetFixtureEventsAsync(215662);

// Get fixture lineups
var lineups = await _fixtureService.GetFixtureLineupsAsync(215662);
```

### PlayerService

```csharp
// Get player by ID
var player = await _playerService.GetPlayerByIdAsync(276, 2023); // Harry Kane

// Get players by team
var teamPlayers = await _playerService.GetPlayersByTeamAsync(33, 2023);

// Search players
var players = await _playerService.SearchPlayersAsync("Messi");

// Get top scorers
var topScorers = await _playerService.GetTopScorersAsync(39, 2023);

// Get top assists
var topAssists = await _playerService.GetTopAssistsAsync(39, 2023);

// Get team squad
var squad = await _playerService.GetTeamSquadAsync(33);
```

## Usage Tracking

The wrapper automatically tracks API usage and provides detailed statistics:

```csharp
// Get usage statistics
var usage = _apiClient.GetUsageStatistics();

Console.WriteLine($"Total API calls: {usage.TotalApiCalls}");
Console.WriteLine($"Requests used: {usage.RequestsUsed}/{usage.RequestsLimit}");
Console.WriteLine($"Requests remaining: {usage.RequestsRemaining}");
Console.WriteLine($"Success rate: {usage.SuccessRate}%");
Console.WriteLine($"Average response time: {usage.AverageResponseTimeMs}ms");
```

## Models

The library includes strongly typed models for all API responses:

### Common Models

- `ApiResponse<T>` - Generic wrapper for all API responses
- `League` - League information with seasons and coverage
- `Team` - Team details including venue information  
- `FixtureDetails` - Complete fixture information with scores and status
- `Player` - Player information and birth details
- `PlayerStatistics` - Detailed player performance statistics

### Response Structure

All API calls return an `ApiResponse<T>` with this structure:

```csharp
public class ApiResponse<T>
{
    public string Get { get; set; }                    // Endpoint called
    public Dictionary<string, object> Parameters { get; set; } // Parameters used
    public List<string> Errors { get; set; }          // Any errors
    public int Results { get; set; }                   // Number of results
    public Paging Paging { get; set; }               // Pagination info
    public List<T> Response { get; set; }            // Actual data
}
```

## Error Handling

The wrapper provides comprehensive error handling:

```csharp
try
{
    var leagues = await _leagueService.GetLeaguesAsync();
    
    if (leagues.Errors?.Any() == true)
    {
        // Handle API errors
        foreach (var error in leagues.Errors)
        {
            Console.WriteLine($"API Error: {error}");
        }
    }
}
catch (HttpRequestException ex)
{
    // Handle network/HTTP errors
    Console.WriteLine($"HTTP Error: {ex.Message}");
}
catch (ArgumentException ex)
{
    // Handle configuration errors
    Console.WriteLine($"Configuration Error: {ex.Message}");
}
```

## Advanced Usage

### Custom HTTP Client Configuration

```csharp
services.AddHttpClient<IFootballApiClient, FootballApiClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
    // Add custom headers, handlers, etc.
});
```

### Direct API Client Usage

```csharp
public class CustomService
{
    private readonly IFootballApiClient _apiClient;

    public CustomService(IFootballApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<string> GetCustomData()
    {
        // Use raw API client for custom endpoints
        return await _apiClient.GetRawAsync("custom-endpoint", new { param = "value" });
    }
}
```

## API Endpoints Coverage

### Leagues
- ✅ Get leagues with filters (country, season, type, etc.)
- ✅ Search leagues by name
- ✅ Get league by ID

### Teams  
- ✅ Get teams with filters (league, season, country, etc.)
- ✅ Search teams by name
- ✅ Get team by ID
- ✅ Get team statistics

### Fixtures
- ✅ Get fixtures by date, league, team
- ✅ Get live fixtures
- ✅ Get fixture by ID
- ✅ Head-to-head fixtures
- ✅ Fixture statistics
- ✅ Fixture events
- ✅ Fixture lineups
- ✅ Fixture player statistics

### Players
- ✅ Get players with filters (team, league, season)
- ✅ Search players by name
- ✅ Get player by ID
- ✅ Top scorers, assists, cards
- ✅ Team squads

## Rate Limits

Be aware of API rate limits based on your subscription:
- **Free**: 100 requests/day
- **Basic**: 1,000 requests/day  
- **Pro**: 10,000+ requests/day

The wrapper tracks usage automatically to help you stay within limits.

## Dependencies

- .NET 8.0
- Microsoft.Extensions.Http
- Microsoft.Extensions.Configuration.Abstractions
- Microsoft.Extensions.DependencyInjection.Abstractions
- Newtonsoft.Json

## License

This project is licensed under the MIT License.