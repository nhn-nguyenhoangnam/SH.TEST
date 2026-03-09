using System.Linq.Expressions;
using BankingApp.Domains.Features.Transactions;

namespace BankingApp.AppServices.Features.Transactions.Repos;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<List<Transaction>> GetListAsync(Expression<Func<Transaction, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default);
}
