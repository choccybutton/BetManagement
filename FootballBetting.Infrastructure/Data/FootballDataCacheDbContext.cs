using Microsoft.EntityFrameworkCore;
using FootballBetting.Domain.Entities.Cache;

namespace FootballBetting.Infrastructure.Data;

public class FootballDataCacheDbContext : DbContext
{
    public FootballDataCacheDbContext(DbContextOptions<FootballDataCacheDbContext> options) : base(options)
    {
    }

    // League and Competition data
    public DbSet<CachedLeague> Leagues { get; set; }
    public DbSet<CachedSeason> Seasons { get; set; }
    public DbSet<CachedCountry> Countries { get; set; }
    
    // Team data
    public DbSet<CachedTeam> Teams { get; set; }
    public DbSet<CachedVenue> Venues { get; set; }
    
    // Fixture data
    public DbSet<CachedFixture> Fixtures { get; set; }
    
    // Player data
    public DbSet<CachedPlayer> Players { get; set; }
    public DbSet<CachedPlayerStatistic> PlayerStatistics { get; set; }
    
    // API usage tracking
    public DbSet<ApiUsageLog> ApiUsageLogs { get; set; }
    public DbSet<CacheMetadata> CacheMetadata { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // League configuration
        modelBuilder.Entity<CachedLeague>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.ApiFootballId).IsRequired();
            entity.Property(l => l.Name).HasMaxLength(200).IsRequired();
            entity.Property(l => l.Type).HasMaxLength(50);
            entity.Property(l => l.Logo).HasMaxLength(500);
            entity.Property(l => l.CachedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(l => l.Country)
                  .WithMany(c => c.Leagues)
                  .HasForeignKey(l => l.CountryId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasIndex(l => l.ApiFootballId).IsUnique();
            entity.HasIndex(l => l.Name);
        });

        // Country configuration
        modelBuilder.Entity<CachedCountry>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Code).HasMaxLength(5);
            entity.Property(c => c.Flag).HasMaxLength(500);
            entity.Property(c => c.CachedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(c => c.Name).IsUnique();
            entity.HasIndex(c => c.Code);
        });

        // Season configuration
        modelBuilder.Entity<CachedSeason>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Year).IsRequired();
            entity.Property(s => s.Start).HasMaxLength(20);
            entity.Property(s => s.End).HasMaxLength(20);
            entity.Property(s => s.CachedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(s => s.League)
                  .WithMany(l => l.Seasons)
                  .HasForeignKey(s => s.LeagueId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(s => new { s.LeagueId, s.Year }).IsUnique();
        });

        // Team configuration
        modelBuilder.Entity<CachedTeam>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.ApiFootballId).IsRequired();
            entity.Property(t => t.Name).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Code).HasMaxLength(10);
            entity.Property(t => t.Country).HasMaxLength(100);
            entity.Property(t => t.Logo).HasMaxLength(500);
            entity.Property(t => t.CachedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(t => t.Venue)
                  .WithMany(v => v.Teams)
                  .HasForeignKey(t => t.VenueId)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasIndex(t => t.ApiFootballId).IsUnique();
            entity.HasIndex(t => t.Name);
        });

        // Venue configuration
        modelBuilder.Entity<CachedVenue>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.ApiFootballId);
            entity.Property(v => v.Name).HasMaxLength(200).IsRequired();
            entity.Property(v => v.Address).HasMaxLength(500);
            entity.Property(v => v.City).HasMaxLength(100);
            entity.Property(v => v.Surface).HasMaxLength(50);
            entity.Property(v => v.Image).HasMaxLength(500);
            entity.Property(v => v.CachedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasIndex(v => v.ApiFootballId).IsUnique();
        });

        // Fixture configuration
        modelBuilder.Entity<CachedFixture>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.ApiFootballId).IsRequired();
            entity.Property(f => f.Referee).HasMaxLength(200);
            entity.Property(f => f.Timezone).HasMaxLength(50);
            entity.Property(f => f.StatusShort).HasMaxLength(50);
            entity.Property(f => f.StatusLong).HasMaxLength(100);
            entity.Property(f => f.CachedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(f => f.League)
                  .WithMany()
                  .HasForeignKey(f => f.LeagueId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(f => f.HomeTeam)
                  .WithMany()
                  .HasForeignKey(f => f.HomeTeamId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(f => f.AwayTeam)
                  .WithMany()
                  .HasForeignKey(f => f.AwayTeamId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(f => f.Venue)
                  .WithMany(v => v.HomeFixtures)
                  .HasForeignKey(f => f.VenueId)
                  .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasIndex(f => f.ApiFootballId).IsUnique();
            entity.HasIndex(f => f.Date);
            entity.HasIndex(f => f.LeagueId);
        });

        // Player configuration
        modelBuilder.Entity<CachedPlayer>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.ApiFootballId).IsRequired();
            entity.Property(p => p.Name).HasMaxLength(200).IsRequired();
            entity.Property(p => p.FirstName).HasMaxLength(100);
            entity.Property(p => p.LastName).HasMaxLength(100);
            entity.Property(p => p.Nationality).HasMaxLength(100);
            entity.Property(p => p.Height).HasMaxLength(20);
            entity.Property(p => p.Weight).HasMaxLength(20);
            entity.Property(p => p.Photo).HasMaxLength(500);
            entity.Property(p => p.BirthPlace).HasMaxLength(200);
            entity.Property(p => p.BirthCountry).HasMaxLength(100);
            entity.Property(p => p.CachedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(p => p.CurrentTeam)
                  .WithMany()
                  .HasForeignKey(p => p.CurrentTeamId)
                  .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasIndex(p => p.ApiFootballId).IsUnique();
            entity.HasIndex(p => p.Name);
        });

        // Player Statistics configuration
        modelBuilder.Entity<CachedPlayerStatistic>(entity =>
        {
            entity.HasKey(ps => ps.Id);
            entity.Property(ps => ps.Position).HasMaxLength(50);
            entity.Property(ps => ps.Rating).HasMaxLength(10);
            entity.Property(ps => ps.CachedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(ps => ps.Player)
                  .WithMany(p => p.Statistics)
                  .HasForeignKey(ps => ps.PlayerId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(ps => ps.Team)
                  .WithMany()
                  .HasForeignKey(ps => ps.TeamId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(ps => ps.League)
                  .WithMany()
                  .HasForeignKey(ps => ps.LeagueId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(ps => ps.Season)
                  .WithMany()
                  .HasForeignKey(ps => ps.SeasonId)
                  .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(ps => new { ps.PlayerId, ps.SeasonId, ps.LeagueId, ps.TeamId }).IsUnique();
        });

        // Configure many-to-many relationship between Players and Fixtures
        modelBuilder.Entity<CachedPlayer>()
            .HasMany(p => p.Fixtures)
            .WithMany(f => f.Players);

        // API Usage Log configuration
        modelBuilder.Entity<ApiUsageLog>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Endpoint).HasMaxLength(500).IsRequired();
            entity.Property(a => a.HttpMethod).HasMaxLength(10);
            entity.Property(a => a.Parameters).HasMaxLength(2000);
            entity.Property(a => a.ResponseSize).HasMaxLength(50);
            entity.Property(a => a.RequestTimestamp).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(a => a.ErrorMessage).HasMaxLength(1000);
            entity.Property(a => a.CostCurrency).HasMaxLength(10);
            entity.Property(a => a.Cost).HasPrecision(10, 4);
            
            entity.HasIndex(a => a.RequestTimestamp);
            entity.HasIndex(a => a.Endpoint);
        });

        // Cache Metadata configuration
        modelBuilder.Entity<CacheMetadata>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(c => c.EntityKey).HasMaxLength(200).IsRequired();
            entity.Property(c => c.CachedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(c => c.ExpiresAt);
            entity.Property(c => c.LastApiCall);
            entity.Property(c => c.ApiCallCount);
            
            entity.HasIndex(c => new { c.EntityType, c.EntityKey }).IsUnique();
            entity.HasIndex(c => c.ExpiresAt);
        });
    }
}