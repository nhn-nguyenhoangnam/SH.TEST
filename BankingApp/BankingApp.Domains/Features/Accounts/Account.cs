using BankingApp.Domains.Features.Transactions;

namespace BankingApp.Domains.Features.Accounts;

public class Account
{
    private Account()
    {
        AccountNumber = string.Empty;
        AccountHolderName = string.Empty;
        SentTransactions = new List<Transaction>();
        ReceivedTransactions = new List<Transaction>();
    }

    public static Account Create(string accountNumber, string accountHolderName, decimal initialBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("Account number is required", nameof(accountNumber));

        if (string.IsNullOrWhiteSpace(accountHolderName))
            throw new ArgumentException("Account holder name is required", nameof(accountHolderName));

        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative", nameof(initialBalance));

        return new Account
        {
            AccountNumber = accountNumber,
            AccountHolderName = accountHolderName,
            Balance = initialBalance
        };
    }

    public Guid Id { get; private set; }
    public string AccountNumber { get; private set; }
    public string AccountHolderName { get; private set; }
    public decimal Balance { get; private set; }

    public ICollection<Transaction> SentTransactions { get; private set; }
    public ICollection<Transaction> ReceivedTransactions { get; private set; }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Debit amount must be greater than zero", nameof(amount));

        if (Balance < amount)
            throw new InvalidOperationException($"Insufficient balance. Current: {Balance}, Required: {amount}");

        Balance -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Credit amount must be greater than zero", nameof(amount));

        Balance += amount;
    }


    public bool HasSufficientBalance(decimal amount) => Balance >= amount;
}