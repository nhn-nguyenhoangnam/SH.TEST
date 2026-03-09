namespace BankingApp.AppServices.Features.Idempotency;

public interface IIdempotencyService
{
    Task<IdempotencyResult<TResponse>?> CheckIdempotencyAsync<TResponse>(string idempotencyKey, string requestHash);
    Task StoreResultAsync<TResponse>(string idempotencyKey, string requestHash, TResponse response, TimeSpan? expiration = null);
}

public class IdempotencyResult<TResponse>
{
    public bool IsProcessing { get; set; }
    public TResponse? CachedResponse { get; set; }
    public string RequestHash { get; set; } = string.Empty;
}
