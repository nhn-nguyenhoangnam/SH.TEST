using BankingApp.Domains.Features.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankingApp.Infra.Features.Configs;

public class TransactionConfiguration: IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.FromAccountId)
            .IsRequired();

        builder.Property(t => t.ToAccountId)
            .IsRequired();

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.TransactionDate)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(TransactionStatus.Pending);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Transactions_Amount_Positive", 
            "[Amount] > 0"));

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Transactions_DifferentAccounts", 
            "[FromAccountId] <> [ToAccountId]"));
    }
}