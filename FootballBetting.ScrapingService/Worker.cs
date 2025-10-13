using FootballBetting.Domain.Interfaces;
using FootballBetting.Domain.Enums;
using FootballBetting.ScrapingService.Services;

namespace FootballBetting.ScrapingService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBettingProviderFactory _providerFactory;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IBettingProviderFactory providerFactory, IConfiguration configuration)
    {
        _logger = logger;
        _providerFactory = providerFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Football Betting Scraping Service starting...");
        
        // Get enabled providers
        var enabledProviders = _providerFactory.GetEnabledProviders().ToList();
        
        if (!enabledProviders.Any())
        {
            _logger.LogWarning("No betting providers configured. Scraping service will not function.");
            return;
        }
        
        _logger.LogInformation("Found {ProviderCount} enabled providers: {Providers}", 
            enabledProviders.Count, 
            string.Join(", ", enabledProviders.Select(p => p.ProviderName)));
        
        // Login to all providers
        var loggedInProviders = new List<IBettingProviderService>();
        
        foreach (var provider in enabledProviders)
        {
            try
            {
                var providerSection = _configuration.GetSection($"BettingProviders:{provider.Provider}");
                var username = providerSection["Username"];
                var password = providerSection["Password"];
                
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Credentials not configured for {Provider}", provider.ProviderName);
                    continue;
                }
                
                var loginSuccess = await provider.LoginAsync(username, password);
                if (loginSuccess)
                {
                    loggedInProviders.Add(provider);
                    _logger.LogInformation("Successfully logged in to {Provider}", provider.ProviderName);
                }
                else
                {
                    _logger.LogWarning("Failed to login to {Provider}", provider.ProviderName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login to {Provider}", provider.ProviderName);
            }
        }
        
        if (!loggedInProviders.Any())
        {
            _logger.LogError("Failed to login to any providers. Stopping scraping service.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting scraping cycle at: {time}", DateTimeOffset.Now);
                
                // Check and re-login providers as needed
                var activeProviders = new List<IBettingProviderService>();
                foreach (var provider in loggedInProviders.ToList())
                {
                    try
                    {
                        if (!await provider.IsLoggedInAsync())
                        {
                            _logger.LogWarning("Session expired for {Provider}, attempting re-login", provider.ProviderName);
                            
                            var providerSection = _configuration.GetSection($"BettingProviders:{provider.Provider}");
                            var username = providerSection["Username"];
                            var password = providerSection["Password"];
                            
                            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && 
                                await provider.LoginAsync(username, password))
                            {
                                activeProviders.Add(provider);
                                _logger.LogInformation("Successfully re-logged into {Provider}", provider.ProviderName);
                            }
                            else
                            {
                                _logger.LogError("Re-login failed for {Provider}", provider.ProviderName);
                            }
                        }
                        else
                        {
                            activeProviders.Add(provider);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error checking login status for {Provider}", provider.ProviderName);
                    }
                }
                
                if (!activeProviders.Any())
                {
                    _logger.LogWarning("No active providers available. Waiting before retry.");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    continue;
                }

                // Scrape matches from all active providers
                var allMatches = new List<FootballBetting.Domain.Entities.Match>();
                
                foreach (var provider in activeProviders)
                {
                    try
                    {
                        var matches = await provider.ScrapeUpcomingMatchesAsync(48);
                        allMatches.AddRange(matches);
                        _logger.LogInformation("Scraped {MatchCount} matches from {Provider}", matches.Count(), provider.ProviderName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error scraping matches from {Provider}", provider.ProviderName);
                    }
                }

                _logger.LogInformation("Total scraped {MatchCount} matches from all providers", allMatches.Count);

                // TODO: Save matches to database via HTTP API
                
                // For each match, scrape detailed odds from each provider
                foreach (var match in allMatches.Take(5)) // Limit to prevent overload
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;
                    
                    foreach (var mapping in match.ProviderMappings)
                    {
                        try
                        {
                            var provider = activeProviders.FirstOrDefault(p => p.Provider == mapping.Provider);
                            if (provider != null)
                            {
                                var odds = await provider.ScrapeMatchOddsAsync(mapping.ProviderMatchId);
                                _logger.LogInformation("Scraped {OddsCount} odds for match {HomeTeam} vs {AwayTeam} from {Provider}", 
                                    odds.Count(), match.HomeTeam, match.AwayTeam, provider.ProviderName);
                                    
                                // TODO: Save odds to database via HTTP API
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error scraping odds for match {HomeTeam} vs {AwayTeam} from {Provider}", 
                                match.HomeTeam, match.AwayTeam, mapping.Provider);
                        }
                        
                        // Rate limiting between provider calls
                        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                    }
                    
                    // Rate limiting between matches
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
                
                _logger.LogInformation("Scraping cycle completed. Next cycle in 30 minutes.");
                
                // Wait for 30 minutes before next scraping cycle
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in scraping cycle");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
        
        // Cleanup - logout from all providers
        foreach (var provider in loggedInProviders)
        {
            try
            {
                await provider.LogoutAsync();
                _logger.LogInformation("Logged out from {Provider}", provider.ProviderName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout from {Provider}", provider.ProviderName);
            }
        }
        
        _logger.LogInformation("Football Betting Scraping Service stopped.");
    }
}
