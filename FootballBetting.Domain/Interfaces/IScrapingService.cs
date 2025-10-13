using FootballBetting.Domain.Entities;

namespace FootballBetting.Domain.Interfaces;

public interface IScrapingService
{
    Task<bool> LoginAsync(string username, string password);
    Task<IEnumerable<Match>> ScrapeUpcomingMatchesAsync(int hoursAhead = 48);
    Task<IEnumerable<Odds>> ScrapeMatchOddsAsync(string bet365MatchId);
    Task<bool> PlaceBetAsync(string bet365MatchId, string bet365OddsId, decimal amount, string betType);
    Task<bool> IsLoggedInAsync();
    Task LogoutAsync();
}