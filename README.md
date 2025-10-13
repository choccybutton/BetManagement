# Football API Wrapper

A comprehensive C# wrapper for the API-Football.com v3 API, complete with a React testing interface for validating all functionality.

## Overview

This solution consists of two main components:

1. **FootballAPIWrapper** - A .NET 8 class library that wraps the API-Football.com v3 API
2. **football-api-wrapper-testui** - A React TypeScript application for testing the wrapper functionality

## Project Structure

```
Football Betting/
├── FootballAPIWrapper/              # C# API Wrapper Library
│   ├── Configuration/               # API configuration classes
│   ├── Models/                      # API response models
│   ├── Services/                    # Service classes for different API endpoints
│   ├── Usage/                       # Usage tracking functionality
│   └── Extensions/                  # Dependency injection extensions
├── football-api-wrapper-testui/    # React Testing Interface
│   ├── src/
│   │   ├── components/             # React components for testing
│   │   ├── services/               # API service classes
│   │   └── types/                  # TypeScript type definitions
└── Specs/                          # Project specifications
```

## Features

### FootballAPIWrapper (C# Library)

- ✅ **Complete API Coverage**: Supports all major API-Football.com v3 endpoints
- ✅ **Usage Tracking**: Monitors API usage, rate limits, and response times
- ✅ **Configurable**: Easy configuration of API keys and endpoints
- ✅ **Dependency Injection**: Full support for .NET dependency injection
- ✅ **Error Handling**: Comprehensive error handling and logging
- ✅ **Models**: Strongly typed models for all API responses

#### Supported Endpoints

- **Leagues**: Search, filter, and retrieve league information
- **Teams**: Get team details, statistics, and squad information  
- **Fixtures**: Access match data, live scores, and historical results
- **Players**: Player statistics, top scorers, assists, and cards
- **Statistics**: Team and player performance metrics
- **Head-to-Head**: Match history between teams

### React Testing Interface

- ✅ **Interactive Testing**: Web-based interface to test all API endpoints
- ✅ **Configuration Management**: Easy API key and endpoint configuration
- ✅ **Real-time Results**: Live display of API responses and data
- ✅ **Usage Monitoring**: Visual dashboard for API usage statistics
- ✅ **Responsive Design**: Mobile-friendly Bootstrap interface

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 16+ and npm
- API-Football.com API key from [RapidAPI](https://rapidapi.com/api-sports/api/api-football/)

### C# Wrapper Setup

1. **Build the wrapper**:
   ```bash
   cd FootballAPIWrapper
   dotnet build
   ```

2. **Add to your project**:
   ```bash
   dotnet add reference ../FootballAPIWrapper/FootballAPIWrapper.csproj
   ```

3. **Configure dependency injection**:
   ```csharp
   using FootballAPIWrapper.Extensions;
   using FootballAPIWrapper.Configuration;

   services.AddFootballApi(config => {
       config.ApiKey = "your-rapidapi-key";
       config.BaseUrl = "https://v3.football.api-sports.io";
       config.RapidApiHost = "v3.football.api-sports.io";
   });
   ```

4. **Use in your code**:
   ```csharp
   using FootballAPIWrapper;
   using FootballAPIWrapper.Services;

   public class MyService
   {
       private readonly LeagueService _leagueService;
       
       public MyService(IFootballApiClient apiClient)
       {
           _leagueService = new LeagueService(apiClient);
       }
       
       public async Task<ApiResponse<League>> GetPremierLeague()
       {
           return await _leagueService.GetLeagueByIdAsync(39);
       }
   }
   ```

### React Testing Interface Setup

1. **Install dependencies**:
   ```bash
   cd football-api-wrapper-testui
   npm install
   ```

2. **Start the development server**:
   ```bash
   npm start
   ```

3. **Configure API access**:
   - Open http://localhost:3000
   - Enter your RapidAPI key in the Configuration panel
   - Test the connection
   - Start testing API endpoints!

## Usage Examples

### Basic League Search

```csharp
// Get Premier League information
var premierLeague = await _leagueService.GetLeagueByIdAsync(39);

// Search leagues by country
var englishLeagues = await _leagueService.GetLeaguesByCountryAsync("England");

// Get current season leagues only
var currentLeagues = await _leagueService.GetLeaguesAsync(current: true);
```

### Team Operations

```csharp
// Get team by ID
var team = await _teamService.GetTeamByIdAsync(33); // Manchester United

// Get teams in Premier League 2023 season
var teams = await _teamService.GetTeamsByLeagueAsync(39, 2023);

// Search teams by name
var searchResults = await _teamService.SearchTeamsAsync("Manchester");
```

### Fixture Queries

```csharp
// Get fixtures for a specific date
var todayFixtures = await _fixtureService.GetFixturesByDateAsync("2023-12-25");

// Get live fixtures
var liveFixtures = await _fixtureService.GetLiveFixturesAsync();

// Head-to-head between two teams
var h2h = await _fixtureService.GetHeadToHeadAsync("33-34"); // Man United vs Man City
```

### Player Statistics

```csharp
// Get player by ID
var player = await _playerService.GetPlayerByIdAsync(276, 2023); // Harry Kane

// Get top scorers in Premier League
var topScorers = await _playerService.GetTopScorersAsync(39, 2023);

// Search players
var players = await _playerService.SearchPlayersAsync("Messi");
```

### Usage Tracking

```csharp
// Get current usage statistics
var usage = _apiClient.GetUsageStatistics();

Console.WriteLine($"API calls made: {usage.TotalApiCalls}");
Console.WriteLine($"Success rate: {usage.SuccessRate}%");
Console.WriteLine($"Requests remaining: {usage.RequestsRemaining}");
```

## API Rate Limits

The API-Football.com service has rate limits that vary by subscription plan:
- Free plan: 100 requests per day
- Basic plan: 1,000 requests per day
- Pro plan: 10,000 requests per day

The wrapper automatically tracks your usage and provides statistics to help you monitor your API consumption.

## Configuration Options

### FootballApiConfiguration

```csharp
public class FootballApiConfiguration
{
    public string ApiKey { get; set; }           // Your RapidAPI key
    public string BaseUrl { get; set; }          // API base URL
    public string RapidApiHost { get; set; }     // RapidAPI host header
    public int TimeoutSeconds { get; set; }      // Request timeout (default: 30)
    public bool EnableUsageTracking { get; set; } // Enable usage statistics (default: true)
}
```

## Error Handling

The wrapper includes comprehensive error handling:

```csharp
try
{
    var leagues = await _leagueService.GetLeaguesAsync();
}
catch (HttpRequestException ex)
{
    // Handle API errors (network issues, API down, etc.)
    Console.WriteLine($"API Error: {ex.Message}");
}
catch (ArgumentException ex)
{
    // Handle configuration errors
    Console.WriteLine($"Configuration Error: {ex.Message}");
}
```

## Testing

### C# Library Testing
```bash
cd FootballAPIWrapper
dotnet test
```

### React Interface Testing
The React interface serves as a comprehensive testing tool for all API endpoints. Simply:
1. Configure your API key
2. Navigate through the different tabs (Leagues, Teams, Fixtures, Players, Usage Stats)
3. Fill in search parameters and test various endpoints
4. View real-time results and API responses

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues, questions, or contributions:
- Create an issue in the repository
- Check the API-Football.com documentation: https://www.api-football.com/documentation-v3
- Review RapidAPI documentation: https://rapidapi.com/api-sports/api/api-football/

## Acknowledgments

- API-Football.com for providing the comprehensive football data API
- RapidAPI for hosting and managing API access
- The .NET and React communities for excellent documentation and tools