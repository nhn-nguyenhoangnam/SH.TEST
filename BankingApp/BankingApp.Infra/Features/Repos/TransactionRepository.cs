using System.Linq.Expressions;
using BankingApp.AppServices.Features.Transactions.Repos;
using BankingApp.Domains.Features.Transactions;
using BankingApp.Infra.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Infra.Features.Repos;

public class TransactionRepository(CoreDbContext dbContext) : ITransactionRepository
{
    private readonly CoreDbContext _dbContext = dbContext;

    public async Task<List<Transaction>> GetListAsync(Expression<Func<Transaction, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Transactions
            .Include(t => t.FromAccount)
            .Include(t => t.ToAccount)
            .AsNoTracking()
            .AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
        return transaction;
    }
}
