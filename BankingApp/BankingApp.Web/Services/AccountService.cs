using System.Net.Http.Json;
using BankingApp.Web.Models;

namespace BankingApp.Web.Services;

public class AccountService
{
    private readonly HttpClient _httpClient;

    public AccountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AccountListResponse?> GetAccountsAsync(int pageNumber = 1, int pageSize = 10, string? accountNumber = null, string? accountHolderName = null)
    {
        var queryParams = new List<string>
        {
            $"PageNumber={pageNumber}",
            $"PageSize={pageSize}"
        };

        if (!string.IsNullOrEmpty(accountNumber))
            queryParams.Add($"AccountNumber={accountNumber}");

        if (!string.IsNullOrEmpty(accountHolderName))
            queryParams.Add($"AccountHolderName={accountHolderName}");

        var query = string.Join("&", queryParams);
        var response = await _httpClient.GetFromJsonAsync<AccountListResponse>($"api/accounts?{query}");
        return response;
    }

    public async Task<AccountDto?> GetAccountByIdAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<AccountDto>($"api/accounts/{id}");
        return response;
    }
}
