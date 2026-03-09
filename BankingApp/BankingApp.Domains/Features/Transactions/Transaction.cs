using BankingApp.Domains.Features.Accounts;

namespace BankingApp.Domains.Features.Transactions;

public enum TransactionStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2
}

public class Transaction
{
    private Transaction()
    {
        Description = string.Empty;
        FromAccount = null!;
        ToAccount = null!;
    }

    public static Transaction Create(
        Guid fromAccountId,
        Guid toAccountId,
        decimal amount,
        string description = "")
    {
        if (fromAccountId == Guid.Empty)
            throw new ArgumentException("Invalid from account", nameof(fromAccountId));

        if (toAccountId == Guid.Empty)
            throw new ArgumentException("Invalid to account", nameof(toAccountId));

        if (fromAccountId == toAccountId)
            throw new ArgumentException("Cannot transfer to the same account");

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero", nameof(amount));

        return new Transaction
        {
            FromAccountId = fromAccountId,
            ToAccountId = toAccountId,
            Amount = amount,
            Description = description,
            TransactionDate = DateTime.UtcNow,
            Status = TransactionStatus.Pending
        };
    }

    public Guid Id { get; private set; }
    public Guid FromAccountId { get; private set; }
    public Guid ToAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public TransactionStatus Status { get; private set; }

    public Account? FromAccount { get; private set; }
    public Account? ToAccount { get; private set; }

    public void MarkAsCompleted()
    {
        if (Status == TransactionStatus.Completed)
            throw new InvalidOperationException("Transaction already completed");

        Status = TransactionStatus.Completed;
    }

    public void MarkAsFailed()
    {
        if (Status == TransactionStatus.Completed)
            throw new InvalidOperationException("Cannot fail a completed transaction");

        Status = TransactionStatus.Failed;
    }

    public bool IsPending() => Status == TransactionStatus.Pending;
    public bool IsCompleted() => Status == TransactionStatus.Completed;
}