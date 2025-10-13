using Microsoft.Playwright;
using FootballBetting.Domain.Entities;
using FootballBetting.Domain.Interfaces;
using FootballBetting.Domain.Enums;

namespace FootballBetting.ScrapingService.Providers.Bet365;

public class Bet365ProviderService : IBettingProviderService, IDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private bool _isLoggedIn = false;
    private readonly ILogger<Bet365ProviderService> _logger;

    public BettingProvider Provider => BettingProvider.Bet365;
    public string ProviderName => "Bet365";

    public Bet365ProviderService(ILogger<Bet365ProviderService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            await InitializeBrowserAsync();
            
            if (_page == null)
                return false;

            // Navigate to Bet365 login page
            await _page.GotoAsync("https://www.bet365.com");
            
            // Wait for and click login button
            await _page.WaitForSelectorAsync("[data-ui='LoginButton']", new() { Timeout = 10000 });
            await _page.ClickAsync("[data-ui='LoginButton']");

            // Fill in credentials
            await _page.FillAsync("[data-ui='UsernameInput']", username);
            await _page.FillAsync("[data-ui='PasswordInput']", password);
            
            // Click login submit
            await _page.ClickAsync("[data-ui='LoginSubmitButton']");
            
            // Wait for successful login (check for account balance or profile)
            try
            {
                await _page.WaitForSelectorAsync("[data-ui='AccountBalance']", new() { Timeout = 5000 });
                _isLoggedIn = true;
                _logger.LogInformation("Successfully logged in to {Provider}", ProviderName);
                return true;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Login failed or timed out for {Provider}", ProviderName);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login process for {Provider}", ProviderName);
            return false;
        }
    }

    public Task<bool> IsLoggedInAsync()
    {
        return Task.FromResult(_isLoggedIn);
    }

    public async Task LogoutAsync()
    {
        try
        {
            if (_page != null && _isLoggedIn)
            {
                await _page.ClickAsync("[data-ui='LogoutButton']");
                _isLoggedIn = false;
                _logger.LogInformation("Successfully logged out from {Provider}", ProviderName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout from {Provider}", ProviderName);
        }
    }

    public async Task<IEnumerable<Match>> ScrapeUpcomingMatchesAsync(int hoursAhead = 48)
    {
        var matches = new List<Match>();
        
        if (!_isLoggedIn || _page == null)
        {
            _logger.LogWarning("Not logged in to {Provider}, cannot scrape matches", ProviderName);
            return matches;
        }

        try
        {
            // Navigate to football section
            await _page.GotoAsync("https://www.bet365.com/#/AC/B1/C1/D8/E45482/F19/");
            
            // Wait for matches to load
            await _page.WaitForSelectorAsync(".sl-CouponParticipantWithBookCloses", new() { Timeout = 10000 });
            
            var matchElements = await _page.QuerySelectorAllAsync(".sl-CouponParticipantWithBookCloses");
            
            foreach (var element in matchElements)
            {
                try
                {
                    var homeTeam = await element.QuerySelectorAsync(".sl-CouponParticipantWithBookCloses_Name:first-child");
                    var awayTeam = await element.QuerySelectorAsync(".sl-CouponParticipantWithBookCloses_Name:last-child");
                    var dateTimeElement = await element.QuerySelectorAsync(".sl-CouponParticipantWithBookCloses_BookCloses");
                    
                    if (homeTeam != null && awayTeam != null && dateTimeElement != null)
                    {
                        var homeTeamText = await homeTeam.TextContentAsync();
                        var awayTeamText = await awayTeam.TextContentAsync();
                        var dateTimeText = await dateTimeElement.TextContentAsync();
                        
                        // Parse date/time (simplified - would need more robust parsing)
                        if (DateTime.TryParse(dateTimeText, out var matchDateTime))
                        {
                            var providerMatchId = await element.GetAttributeAsync("data-event-id") ?? Guid.NewGuid().ToString();
                            
                            var match = new Match
                            {
                                HomeTeam = homeTeamText?.Trim() ?? string.Empty,
                                AwayTeam = awayTeamText?.Trim() ?? string.Empty,
                                MatchDateTime = matchDateTime,
                                League = "Premier League", // Would need to extract from page
                                ScrapedAt = DateTime.UtcNow
                            };
                            
                            // Add provider mapping
                            match.ProviderMappings.Add(new ProviderMatchMapping
                            {
                                Provider = Provider,
                                ProviderMatchId = providerMatchId,
                                CreatedAt = DateTime.UtcNow,
                                LastUpdatedAt = DateTime.UtcNow
                            });
                            
                            matches.Add(match);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error scraping individual match for {Provider}", ProviderName);
                }
            }
            
            _logger.LogInformation("Scraped {MatchCount} matches from {Provider}", matches.Count, ProviderName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scraping matches from {Provider}", ProviderName);
        }
        
        return matches;
    }

    public async Task<IEnumerable<Odds>> ScrapeMatchOddsAsync(string providerMatchId)
    {
        var odds = new List<Odds>();
        
        if (!_isLoggedIn || _page == null)
            return odds;

        try
        {
            // Navigate to specific match odds page
            await _page.GotoAsync($"https://www.bet365.com/#/AC/B1/C1/D8/E{providerMatchId}/F19/");
            
            // Wait for odds to load
            await _page.WaitForSelectorAsync(".gl-MarketColumnHeader", new() { Timeout = 10000 });
            
            // Scrape 1X2 odds
            var oddsElements = await _page.QuerySelectorAllAsync(".gl-Participant_General");
            
            for (int i = 0; i < Math.Min(3, oddsElements.Count); i++)
            {
                var oddElement = oddsElements[i];
                var oddText = await oddElement.TextContentAsync();
                
                if (decimal.TryParse(oddText, out var oddValue))
                {
                    var betType = i switch
                    {
                        0 => BetType.HomeWin,
                        1 => BetType.Draw,
                        2 => BetType.AwayWin,
                        _ => BetType.HomeWin
                    };
                    
                    odds.Add(new Odds
                    {
                        Provider = Provider,
                        BetType = betType,
                        OddsValue = oddValue,
                        ScrapedAt = DateTime.UtcNow,
                        ProviderOddsId = await oddElement.GetAttributeAsync("data-odds-id") ?? Guid.NewGuid().ToString(),
                        Description = betType.ToString()
                    });
                }
            }
            
            // TODO: Add more bet types (Over/Under, Cards, Corners, etc.)
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scraping odds for match {MatchId} from {Provider}", providerMatchId, ProviderName);
        }
        
        return odds;
    }

    public async Task<BetPlacementResult> PlaceBetAsync(BetPlacementRequest request)
    {
        if (!_isLoggedIn || _page == null)
        {
            return new BetPlacementResult
            {
                Success = false,
                ErrorMessage = $"Not logged in to {ProviderName}"
            };
        }

        try
        {
            // Navigate to match and find the specific odd
            await _page.GotoAsync($"https://www.bet365.com/#/AC/B1/C1/D8/E{request.ProviderMatchId}/F19/");
            
            // Find and click the specific odds
            var oddElement = await _page.QuerySelectorAsync($"[data-odds-id='{request.ProviderOddsId}']");
            if (oddElement != null)
            {
                await oddElement.ClickAsync();
                
                // Wait for bet slip to appear
                await _page.WaitForSelectorAsync(".bss-StakeBox_StakeValueInput", new() { Timeout = 5000 });
                
                // Enter bet amount
                await _page.FillAsync(".bss-StakeBox_StakeValueInput", request.Amount.ToString());
                
                // Place the bet
                await _page.ClickAsync(".bss-PlaceBetButton");
                
                // Wait for confirmation
                try
                {
                    await _page.WaitForSelectorAsync(".bss-ReceiptContent", new() { Timeout = 5000 });
                    
                    // Extract bet ID if possible
                    var betIdElement = await _page.QuerySelectorAsync("[data-receipt-bet-id]");
                    var providerBetId = betIdElement != null ? await betIdElement.GetAttributeAsync("data-receipt-bet-id") : null;
                    
                    _logger.LogInformation("Bet placed successfully on {Provider}", ProviderName);
                    
                    return new BetPlacementResult
                    {
                        Success = true,
                        ProviderBetId = providerBetId,
                        AcceptedStake = request.Amount,
                        AcceptedOdds = request.ExpectedOdds,
                        PlacedAt = DateTime.UtcNow
                    };
                }
                catch (TimeoutException)
                {
                    return new BetPlacementResult
                    {
                        Success = false,
                        ErrorMessage = "Bet placement confirmation not received"
                    };
                }
            }
            else
            {
                return new BetPlacementResult
                {
                    Success = false,
                    ErrorMessage = "Odds element not found"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error placing bet on {Provider}", ProviderName);
            return new BetPlacementResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<decimal?> GetAccountBalanceAsync()
    {
        if (!_isLoggedIn || _page == null)
            return null;

        try
        {
            var balanceElement = await _page.QuerySelectorAsync("[data-ui='AccountBalance']");
            if (balanceElement != null)
            {
                var balanceText = await balanceElement.TextContentAsync();
                if (decimal.TryParse(balanceText?.Replace("£", "").Replace("$", "").Replace("€", ""), out var balance))
                {
                    return balance;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account balance from {Provider}", ProviderName);
        }

        return null;
    }

    public async Task<IEnumerable<ProviderBetHistory>> GetBetHistoryAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var history = new List<ProviderBetHistory>();
        
        if (!_isLoggedIn || _page == null)
            return history;

        try
        {
            // Navigate to bet history page
            await _page.GotoAsync("https://www.bet365.com/#/MB/");
            
            // Wait for bet history to load
            await _page.WaitForSelectorAsync(".mbs-BetHistoryContainer", new() { Timeout = 10000 });
            
            // TODO: Implement actual bet history scraping logic
            _logger.LogInformation("Bet history scraping not yet implemented for {Provider}", ProviderName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bet history from {Provider}", ProviderName);
        }

        return history;
    }

    private async Task InitializeBrowserAsync()
    {
        if (_playwright == null)
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-dev-shm-usage" }
            });
            
            _page = await _browser.NewPageAsync();
            
            // Set user agent to avoid detection
            await _page.SetExtraHTTPHeadersAsync(new Dictionary<string, string>
            {
                ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
            });
        }
    }

    public void Dispose()
    {
        _page?.CloseAsync().Wait();
        _browser?.CloseAsync().Wait();
        _playwright?.Dispose();
    }
}