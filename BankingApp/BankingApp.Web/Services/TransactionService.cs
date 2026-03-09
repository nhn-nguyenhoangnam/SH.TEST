using System.Net.Http.Json;
using BankingApp.Web.Models;

namespace BankingApp.Web.Services;

public class TransactionService
{
    private readonly HttpClient _httpClient;

    public TransactionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TransactionListResponse?> GetTransactionsAsync(
        Guid? fromAccountId = null,
        Guid? toAccountId = null,
        string? status = null,
        int pageNumber = 1,
        int pageSize = 100)
    {
        var queryParams = new List<string>
        {
            $"PageNumber={pageNumber}",
            $"PageSize={pageSize}"
        };

        if (fromAccountId.HasValue)
            queryParams.Add($"FromAccountId={fromAccountId}");

        if (toAccountId.HasValue)
            queryParams.Add($"ToAccountId={toAccountId}");

        if (!string.IsNullOrEmpty(status))
            queryParams.Add($"Status={status}");

        var query = string.Join("&", queryParams);
        var response = await _httpClient.GetFromJsonAsync<TransactionListResponse>($"api/transactions?{query}");
        return response;
    }

    public async Task<CreateTransactionResponse?> CreateTransactionAsync(CreateTransactionRequest request)
    {
        request.IdempotencyKey = Guid.NewGuid().ToString();
        var response = await _httpClient.PostAsJsonAsync("api/transactions", request);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<CreateTransactionResponse>();
        }

        try
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new HttpRequestException(errorResponse?.Message ?? "Transaction failed");
        }
        catch (System.Text.Json.JsonException)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(string.IsNullOrEmpty(errorContent) ? "Transaction failed" : errorContent);
        }
    }
}

public class CreateTransactionRequest
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
}

public class CreateTransactionResponse
{
    public Guid TransactionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
}
