using FootballBetting.ScrapingService;
using FootballBetting.ScrapingService.Services;
using FootballBetting.ScrapingService.Providers.Bet365;
using FootballBetting.Domain.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

// Register provider services
builder.Services.AddScoped<Bet365ProviderService>();
builder.Services.AddScoped<IBettingProviderFactory, BettingProviderFactory>();
builder.Services.AddHostedService<Worker>();

// Install Playwright browsers if needed
Microsoft.Playwright.Program.Main(new[] { "install" });

var host = builder.Build();
host.Run();
