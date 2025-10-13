using FootballBetting.Domain.Entities;

namespace FootballBetting.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Match> Matches { get; }
    IRepository<Odds> Odds { get; }
    IRepository<Bet> Bets { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}