using FootballBetting.Application.DTOs;
using FootballBetting.Domain.Enums;

namespace FootballBetting.Application.Interfaces;

public interface IMatchService
{
    Task<IEnumerable<MatchDto>> GetUpcomingMatchesAsync(int hoursAhead = 48);
    Task<IEnumerable<MatchDto>> GetUpcomingMatchesAsync(BettingProvider? provider, int hoursAhead = 48);
    Task<MatchDto?> GetMatchByIdAsync(int id);
    Task<IEnumerable<MatchDto>> GetMatchesByDateRangeAsync(DateTime from, DateTime to);
    Task<IEnumerable<OddsDto>> GetMatchOddsAsync(int matchId, BettingProvider? provider = null);
    Task<IEnumerable<OddsComparisonDto>> CompareOddsAsync(int matchId);
    Task TriggerScrapingAsync();
    Task TriggerScrapingAsync(BettingProvider provider);
}

public interface IBettingService
{
    Task<BetDto> CreateBetAsync(int userId, CreateBetDto createBetDto);
    Task<bool> PlaceBetAsync(int betId);
    Task<IEnumerable<BetDto>> GetUserBetsAsync(int userId);
    Task<IEnumerable<BetDto>> GetUserBetsByProviderAsync(int userId, BettingProvider provider);
    Task<BetDto?> GetBetByIdAsync(int betId);
    Task<bool> CancelBetAsync(int betId);
    Task<IEnumerable<ProviderAccountDto>> GetUserProviderAccountsAsync(int userId);
}

public interface IUserService
{
    Task<UserDto> RegisterUserAsync(RegisterUserDto registerDto);
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<bool> UpdateProviderCredentialsAsync(int userId, UpdateProviderCredentialsDto credentialsDto);
    Task<bool> ValidateTokenAsync(string token);
    Task<IEnumerable<BettingProvider>> GetUserProvidersAsync(int userId);
}
