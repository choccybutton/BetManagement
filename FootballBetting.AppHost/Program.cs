var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server databases
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume();

var footballBettingDb = sqlServer.AddDatabase("footballbetting");
var footballDataCacheDb = sqlServer.AddDatabase("footballdatacache");

// Add API service
var apiService = builder.AddProject<Projects.FootballBetting_ApiService>("apiservice")
    .WithReference(footballBettingDb)
    .WithReference(footballDataCacheDb);

// Add Scraping service
var scrapingService = builder.AddProject<Projects.FootballBetting_ScrapingService>("scrapingservice")
    .WithReference(apiService);

// Add Web frontend  
builder.AddProject<Projects.FootballBetting_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
