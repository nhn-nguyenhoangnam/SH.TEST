using BankingApp.Domains.Features.Accounts;
using BankingApp.Domains.Features.Transactions;
using BankingApp.Infra.Features.Configs;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Infra.Contexts;

public sealed class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());

        SeedAccounts(modelBuilder);
    }

    private void SeedAccounts(ModelBuilder modelBuilder)
    {
        var accounts = new[]
        {
            CreateAccountForSeed(Guid.Parse("11111111-1111-1111-1111-111111111111"), "ACC001", "John Smith", 5000000m),
            CreateAccountForSeed(Guid.Parse("22222222-2222-2222-2222-222222222222"), "ACC002", "Mary Johnson", 10000000m),
            CreateAccountForSeed(Guid.Parse("33333333-3333-3333-3333-333333333333"), "ACC003", "Robert Williams", 7500000m),
            CreateAccountForSeed(Guid.Parse("44444444-4444-4444-4444-444444444444"), "ACC004", "Patricia Brown", 15000000m),
            CreateAccountForSeed(Guid.Parse("55555555-5555-5555-5555-555555555555"), "ACC005", "Michael Davis", 3000000m),
            CreateAccountForSeed(Guid.Parse("66666666-6666-6666-6666-666666666666"), "ACC006", "Jennifer Miller", 20000000m),
            CreateAccountForSeed(Guid.Parse("77777777-7777-7777-7777-777777777777"), "ACC007", "William Wilson", 12000000m),
            CreateAccountForSeed(Guid.Parse("88888888-8888-8888-8888-888888888888"), "ACC008", "Linda Moore", 8500000m),
            CreateAccountForSeed(Guid.Parse("99999999-9999-9999-9999-999999999999"), "ACC009", "David Taylor", 6000000m),
            CreateAccountForSeed(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "ACC010", "Elizabeth Anderson", 25000000m)
        };

        modelBuilder.Entity<Account>().HasData(accounts);
    }

    private object CreateAccountForSeed(Guid id, string accountNumber, string accountHolderName, decimal balance)
    {
        return new
        {
            Id = id,
            AccountNumber = accountNumber,
            AccountHolderName = accountHolderName,
            Balance = balance
        };
    }
}