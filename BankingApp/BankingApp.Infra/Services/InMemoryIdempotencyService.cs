using BankingApp.AppServices.Features.Idempotency;
using Microsoft.Extensions.Caching.Memory;

namespace BankingApp.Infra.Services;

public class InMemoryIdempotencyService : IIdempotencyService
{
    private readonly IMemoryCache _cache;
    private const string ProcessingPrefix = "processing:";
    private const string ResultPrefix = "result:";

    public InMemoryIdempotencyService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<IdempotencyResult<TResponse>?> CheckIdempotencyAsync<TResponse>(string idempotencyKey, string requestHash)
    {
        if (_cache.TryGetValue($"{ProcessingPrefix}{idempotencyKey}", out string? storedHash))
        {
            throw new InvalidOperationException("Idempotency key is already in use. Please generate a new key.");
        }

        if (_cache.TryGetValue($"{ResultPrefix}{idempotencyKey}", out CachedResult<TResponse>? cached))
        {
            throw new InvalidOperationException("Idempotency key has already been used. Please generate a new key.");
        }

        _cache.Set($"{ProcessingPrefix}{idempotencyKey}", requestHash, TimeSpan.FromMinutes(5));

        return Task.FromResult<IdempotencyResult<TResponse>?>(null);
    }

    public Task StoreResultAsync<TResponse>(string idempotencyKey, string requestHash, TResponse response, TimeSpan? expiration = null)
    {
        _cache.Remove($"{ProcessingPrefix}{idempotencyKey}");

        var cacheExpiration = expiration ?? TimeSpan.FromHours(24);
        _cache.Set($"{ResultPrefix}{idempotencyKey}", new CachedResult<TResponse>
        {
            RequestHash = requestHash,
            Response = response
        }, cacheExpiration);

        return Task.CompletedTask;
    }

    private class CachedResult<TResponse>
    {
        public string RequestHash { get; set; } = string.Empty;
        public TResponse? Response { get; set; }
    }
}
