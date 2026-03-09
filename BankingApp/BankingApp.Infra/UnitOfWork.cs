using BankingApp.AppServices;
using BankingApp.Infra.Contexts;

namespace BankingApp.Infra;

public class UnitOfWork(CoreDbContext dbContext) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
