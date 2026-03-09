using System.Linq.Expressions;
using BankingApp.Domains.Features.Accounts;

namespace BankingApp.AppServices.Features.Accounts.Repos;

public interface IAccountRepository : IRepository<Account>
{
    Task<List<Account>> GetListAsync(Expression<Func<Account, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Account?> GetByIdWithTrackingAsync(Guid id, CancellationToken cancellationToken = default);
}