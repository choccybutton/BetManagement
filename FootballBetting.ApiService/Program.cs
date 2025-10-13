using FootballBetting.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Aspire SQL Server
builder.AddSqlServerDbContext<FootballBetting.Infrastructure.Data.FootballBettingDbContext>("footballbetting");
builder.AddSqlServerDbContext<FootballBetting.Infrastructure.Data.FootballDataCacheDbContext>("footballdatacache");

// Add Infrastructure services (without EF since Aspire handles it)
builder.Services.AddScoped(typeof(FootballBetting.Domain.Interfaces.IRepository<>), typeof(FootballBetting.Infrastructure.Repositories.Repository<>));
builder.Services.AddScoped<FootballBetting.Domain.Interfaces.IUnitOfWork, FootballBetting.Infrastructure.Repositories.UnitOfWork>();

// TODO: Add Application services
// builder.Services.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
