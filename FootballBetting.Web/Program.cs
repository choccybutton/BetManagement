using FootballBetting.Web;
using FootballBetting.Web.Components;
using FootballBetting.Web.Services;
using FootballAPIWrapper.Extensions;
using FootballAPIWrapper.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    });

// Configure Football API
// Note: For development, you can set the API key in appsettings.Development.json
// or as a user secret. For production, use environment variables or Azure Key Vault.
var footballApiKey = builder.Configuration["FootballApi:ApiKey"] ?? "your-api-key-here";

builder.Services.AddFootballApi(config =>
{
    config.ApiKey = footballApiKey;
    config.BaseUrl = "https://v3.football.api-sports.io";
    config.RapidApiHost = "v3.football.api-sports.io";
    config.EnableUsageTracking = true;
});

// Register Football API related services
builder.Services.AddScoped<FixtureService>();
builder.Services.AddSingleton<IApiUsageService, ApiUsageService>();
builder.Services.AddScoped<FootballApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
