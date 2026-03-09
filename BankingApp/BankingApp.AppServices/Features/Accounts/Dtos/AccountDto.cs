namespace BankingApp.AppServices.Features.Accounts.Repos;

public sealed record AccountDto
{
    public Guid Id { get; init; }
    public string? AccountNumber { get; init; }
    public string? AccountHolderName { get; init; }
    public decimal Balance { get; init; }
}