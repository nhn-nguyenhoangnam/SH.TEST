using System.Linq.Expressions;
using BankingApp.AppServices.Features.Accounts.Repos;
using BankingApp.Domains.Features.Accounts;
using BankingApp.Infra.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Infra.Features.Repos;

public class AccountRepository(CoreDbContext dbContext) : IAccountRepository
{
    private readonly CoreDbContext _dbContext = dbContext;

    public async Task<List<Account>> GetListAsync(Expression<Func<Account, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Accounts.AsNoTracking().AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Account?> GetByIdWithTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accounts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}