using FootballBetting.Domain.Enums;

namespace FootballBetting.Domain.Interfaces;

/// <summary>
/// Factory for creating betting provider service instances
/// </summary>
public interface IBettingProviderFactory
{
    /// <summary>
    /// Get a betting provider service by provider type
    /// </summary>
    IBettingProviderService GetProvider(BettingProvider provider);
    
    /// <summary>
    /// Get all available providers
    /// </summary>
    IEnumerable<IBettingProviderService> GetAllProviders();
    
    /// <summary>
    /// Get enabled providers based on configuration
    /// </summary>
    IEnumerable<IBettingProviderService> GetEnabledProviders();
    
    /// <summary>
    /// Check if a provider is available and configured
    /// </summary>
    bool IsProviderAvailable(BettingProvider provider);
}