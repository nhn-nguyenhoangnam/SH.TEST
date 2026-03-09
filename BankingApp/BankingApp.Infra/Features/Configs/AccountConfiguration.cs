using BankingApp.Domains.Features.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Infra.Features.Configs;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.AccountNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.AccountHolderName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Balance)
            .IsRequired()
            .HasPrecision(18, 4)
            .HasDefaultValue(0);

        builder.HasIndex(a => a.AccountNumber)
            .IsUnique();

        builder.HasMany(a => a.SentTransactions)
            .WithOne(t => t.FromAccount)
            .HasForeignKey(t => t.FromAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.ReceivedTransactions)
            .WithOne(t => t.ToAccount)
            .HasForeignKey(t => t.ToAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}