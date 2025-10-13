using Microsoft.EntityFrameworkCore;
using FootballBetting.Domain.Entities;
using FootballBetting.Domain.Enums;

namespace FootballBetting.Infrastructure.Data;

public class FootballBettingDbContext : DbContext
{
    public FootballBettingDbContext(DbContextOptions<FootballBettingDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Odds> Odds { get; set; }
    public DbSet<Bet> Bets { get; set; }
    public DbSet<ProviderMatchMapping> ProviderMatchMappings { get; set; }
    public DbSet<UserProviderCredential> UserProviderCredentials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(100).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // Match configuration
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.HomeTeam).HasMaxLength(200).IsRequired();
            entity.Property(m => m.AwayTeam).HasMaxLength(200).IsRequired();
            entity.Property(m => m.League).HasMaxLength(100);
            entity.Property(m => m.Competition).HasMaxLength(100);
            entity.Property(m => m.ScrapedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.HasIndex(m => m.MatchDateTime);
        });

        // Odds configuration
        modelBuilder.Entity<Odds>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OddsValue).HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(o => o.ProviderOddsId).HasMaxLength(100).IsRequired();
            entity.Property(o => o.Description).HasMaxLength(200);
            entity.Property(o => o.ScrapedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(o => o.Match)
                  .WithMany(m => m.Odds)
                  .HasForeignKey(o => o.MatchId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(o => o.ProviderOddsId);
            entity.HasIndex(o => new { o.MatchId, o.BetType });
        });

        // Bet configuration
        modelBuilder.Entity<Bet>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Amount).HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(b => b.OddsAtTimeOfBet).HasColumnType("decimal(10,2)").IsRequired();
            entity.Property(b => b.PotentialReturn).HasColumnType("decimal(10,2)");
            entity.Property(b => b.ActualReturn).HasColumnType("decimal(10,2)");
            entity.Property(b => b.ProviderBetId).HasMaxLength(100);
            entity.Property(b => b.Notes).HasMaxLength(500);
            entity.Property(b => b.ErrorMessage).HasMaxLength(1000);
            entity.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(b => b.User)
                  .WithMany(u => u.Bets)
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(b => b.Match)
                  .WithMany(m => m.Bets)
                  .HasForeignKey(b => b.MatchId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(b => b.Odds)
                  .WithMany(o => o.Bets)
                  .HasForeignKey(b => b.OddsId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasIndex(b => b.ProviderBetId);
            entity.HasIndex(b => b.Status);
            entity.HasIndex(b => b.CreatedAt);
        });

        // Configure enums
        modelBuilder.Entity<Odds>()
            .Property(o => o.BetType)
            .HasConversion<int>();

        modelBuilder.Entity<Bet>()
            .Property(b => b.BetType)
            .HasConversion<int>();

        modelBuilder.Entity<Bet>()
            .Property(b => b.Status)
            .HasConversion<int>();

        // ProviderMatchMapping configuration
        modelBuilder.Entity<ProviderMatchMapping>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.ProviderMatchId).HasMaxLength(100).IsRequired();
            entity.Property(p => p.ProviderUrl).HasMaxLength(500);
            entity.Property(p => p.ProviderEventName).HasMaxLength(200);
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(p => p.LastUpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(p => p.Match)
                  .WithMany(m => m.ProviderMappings)
                  .HasForeignKey(p => p.MatchId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(p => new { p.Provider, p.ProviderMatchId }).IsUnique();
            entity.Property(p => p.Provider).HasConversion<int>();
        });

        // UserProviderCredential configuration
        modelBuilder.Entity<UserProviderCredential>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).HasMaxLength(100).IsRequired();
            entity.Property(u => u.EncryptedPassword).HasMaxLength(500).IsRequired();
            entity.Property(u => u.Notes).HasMaxLength(1000);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(u => u.LastUpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasOne(u => u.User)
                  .WithMany(user => user.ProviderCredentials)
                  .HasForeignKey(u => u.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(u => new { u.UserId, u.Provider }).IsUnique();
            entity.Property(u => u.Provider).HasConversion<int>();
        });
    }
}
