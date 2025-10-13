using FootballBetting.Domain.Entities;
using FootballBetting.Domain.Enums;

namespace FootballBetting.Domain.Interfaces;

/// <summary>
/// Generic interface for betting provider services (Bet365, William Hill, etc.)
/// </summary>
public interface IBettingProviderService
{
    /// <summary>
    /// The betting provider this service implements
    /// </summary>
    BettingProvider Provider { get; }
    
    /// <summary>
    /// Human-readable name of the provider
    /// </summary>
    string ProviderName { get; }
    
    /// <summary>
    /// Login to the betting provider
    /// </summary>
    Task<bool> LoginAsync(string username, string password);
    
    /// <summary>
    /// Check if currently logged in
    /// </summary>
    Task<bool> IsLoggedInAsync();
    
    /// <summary>
    /// Logout from the betting provider
    /// </summary>
    Task LogoutAsync();
    
    /// <summary>
    /// Scrape upcoming matches from the provider
    /// </summary>
    Task<IEnumerable<Match>> ScrapeUpcomingMatchesAsync(int hoursAhead = 48);
    
    /// <summary>
    /// Scrape odds for a specific match
    /// </summary>
    Task<IEnumerable<Odds>> ScrapeMatchOddsAsync(string providerMatchId);
    
    /// <summary>
    /// Place a bet on the provider platform
    /// </summary>
    Task<BetPlacementResult> PlaceBetAsync(BetPlacementRequest request);
    
    /// <summary>
    /// Get account balance from the provider
    /// </summary>
    Task<decimal?> GetAccountBalanceAsync();
    
    /// <summary>
    /// Get betting history from the provider
    /// </summary>
    Task<IEnumerable<ProviderBetHistory>> GetBetHistoryAsync(DateTime? fromDate = null, DateTime? toDate = null);
}

/// <summary>
/// Result of a bet placement operation
/// </summary>
public class BetPlacementResult
{
    public bool Success { get; set; }
    public string? ProviderBetId { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal? AcceptedStake { get; set; }
    public decimal? AcceptedOdds { get; set; }
    public DateTime PlacedAt { get; set; }
}

/// <summary>
/// Request to place a bet
/// </summary>
public class BetPlacementRequest
{
    public string ProviderMatchId { get; set; } = string.Empty;
    public string ProviderOddsId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public BetType BetType { get; set; }
    public decimal ExpectedOdds { get; set; }
}

/// <summary>
/// Provider-specific bet history item
/// </summary>
public class ProviderBetHistory
{
    public string ProviderBetId { get; set; } = string.Empty;
    public string MatchDescription { get; set; } = string.Empty;
    public BetType BetType { get; set; }
    public decimal Stake { get; set; }
    public decimal Odds { get; set; }
    public decimal? Return { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime PlacedAt { get; set; }
    public DateTime? SettledAt { get; set; }
}