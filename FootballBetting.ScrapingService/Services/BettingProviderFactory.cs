using FootballBetting.Domain.Enums;
using FootballBetting.Domain.Interfaces;
using FootballBetting.ScrapingService.Providers.Bet365;

namespace FootballBetting.ScrapingService.Services;

public class BettingProviderFactory : IBettingProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BettingProviderFactory> _logger;

    public BettingProviderFactory(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<BettingProviderFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public IBettingProviderService GetProvider(BettingProvider provider)
    {
        return provider switch
        {
            BettingProvider.Bet365 => _serviceProvider.GetRequiredService<Bet365ProviderService>(),
            // TODO: Add other providers as they are implemented
            // BettingProvider.WilliamHill => _serviceProvider.GetRequiredService<WilliamHillProviderService>(),
            // BettingProvider.Betfair => _serviceProvider.GetRequiredService<BetfairProviderService>(),
            _ => throw new NotSupportedException($"Provider {provider} is not supported")
        };
    }

    public IEnumerable<IBettingProviderService> GetAllProviders()
    {
        var providers = new List<IBettingProviderService>();

        try
        {
            providers.Add(GetProvider(BettingProvider.Bet365));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create Bet365 provider service");
        }

        // TODO: Add other providers as they are implemented
        /*
        try
        {
            providers.Add(GetProvider(BettingProvider.WilliamHill));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create WilliamHill provider service");
        }
        */

        return providers;
    }

    public IEnumerable<IBettingProviderService> GetEnabledProviders()
    {
        var enabledProviders = new List<IBettingProviderService>();

        // Check configuration for enabled providers
        var enabledProviderNames = _configuration.GetSection("BettingProviders:Enabled").Get<string[]>() ?? Array.Empty<string>();
        
        foreach (var providerName in enabledProviderNames)
        {
            if (Enum.TryParse<BettingProvider>(providerName, ignoreCase: true, out var provider))
            {
                try
                {
                    if (IsProviderAvailable(provider))
                    {
                        enabledProviders.Add(GetProvider(provider));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to create enabled provider service for {Provider}", provider);
                }
            }
        }

        // If no providers configured, return Bet365 as default if available
        if (!enabledProviders.Any() && IsProviderAvailable(BettingProvider.Bet365))
        {
            try
            {
                enabledProviders.Add(GetProvider(BettingProvider.Bet365));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create default Bet365 provider service");
            }
        }

        return enabledProviders;
    }

    public bool IsProviderAvailable(BettingProvider provider)
    {
        var providerSection = _configuration.GetSection($"BettingProviders:{provider}");
        
        if (!providerSection.Exists())
        {
            return false;
        }

        var username = providerSection["Username"];
        var password = providerSection["Password"];

        return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
    }
}