namespace BankingApp.AppServices.Features.Transactions.Dtos;

public sealed record TransactionDto
{
    public Guid Id { get; init; }
    public Guid FromAccountId { get; init; }
    public Guid ToAccountId { get; init; }
    public string? FromAccountNumber { get; init; }
    public string? ToAccountNumber { get; init; }
    public decimal Amount { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime TransactionDate { get; init; }
    public string? Status { get; init; }
}
