namespace BankingApp.Web.Models;

public class AccountDto
{
    public Guid Id { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountHolderName { get; set; }
    public decimal Balance { get; set; }
}

public class AccountListResponse
{
    public List<AccountDto> Accounts { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
