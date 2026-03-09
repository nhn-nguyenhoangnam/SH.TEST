using MediatR;
using FluentValidation;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BankingApp.AppServices;
using BankingApp.AppServices.Features.Idempotency;
using BankingApp.AppServices.Features.Transactions.Repos;
using BankingApp.AppServices.Features.Accounts.Repos;
using BankingApp.AppServices.Features.Transactions.Dtos;
using BankingApp.Domains.Features.Transactions;

namespace BankingApp.AppServices.Features.Transactions.Commands;

public record CreateTransactionRequest : IRequest<CreateTransactionResponse>
{
    public Guid FromAccountId { get; init; }
    public Guid ToAccountId { get; init; }
    public decimal Amount { get; init; }
    public string Description { get; init; } = string.Empty;
    public string IdempotencyKey { get; init; } = string.Empty;
}

public record CreateTransactionResponse(
    Guid TransactionId,
    string Status,
    string Message
);

public class CreateTransactionValidator : AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.FromAccountId)
            .NotEmpty()
            .WithMessage("From account ID is required");

        RuleFor(x => x.ToAccountId)
            .NotEmpty()
            .WithMessage("To account ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");

        RuleFor(x => x.FromAccountId)
            .NotEqual(x => x.ToAccountId)
            .WithMessage("Cannot transfer to the same account");

        RuleFor(x => x.Description)
            .NotNull()
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .WithMessage("Idempotency key is required");
    }
}

public class CreateTransactionCommandHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork,
        IIdempotencyService idempotencyService
    ) : IRequestHandler<CreateTransactionRequest, CreateTransactionResponse>
{
    public async Task<CreateTransactionResponse> Handle(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var requestHash = ComputeRequestHash(request);
        await idempotencyService.CheckIdempotencyAsync<CreateTransactionResponse>(
            request.IdempotencyKey, 
            requestHash
        );

        try
        {
            var fromAccount = await accountRepository.GetByIdWithTrackingAsync(request.FromAccountId, cancellationToken);
            if (fromAccount == null)
            {
                throw new InvalidOperationException($"From account with ID {request.FromAccountId} not found");
            }

            var toAccount = await accountRepository.GetByIdWithTrackingAsync(request.ToAccountId, cancellationToken);
            if (toAccount == null)
            {
                throw new InvalidOperationException($"To account with ID {request.ToAccountId} not found");
            }

            if (!fromAccount.HasSufficientBalance(request.Amount))
            {
                throw new InvalidOperationException(
                    $"Insufficient balance. Current: {fromAccount.Balance:N0}, Required: {request.Amount:N0}");
            }

            var transferTransaction = Transaction.Create(
                request.FromAccountId,
                request.ToAccountId,
                request.Amount,
                request.Description
            );

            fromAccount.Debit(request.Amount);
            toAccount.Credit(request.Amount);

            transferTransaction.MarkAsCompleted();

            await transactionRepository.CreateAsync(transferTransaction, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new CreateTransactionResponse(
                transferTransaction.Id,
                "Completed",
                "Transaction completed successfully"
            );

            await idempotencyService.StoreResultAsync(request.IdempotencyKey, requestHash, response);

            return response;
        }
        catch (Exception ex)
        {
            var errorResponse = new CreateTransactionResponse(
                Guid.Empty,
                "Failed",
                ex.Message
            );

            await idempotencyService.StoreResultAsync(request.IdempotencyKey, requestHash, errorResponse);

            return errorResponse;
        }
    }

    private static string ComputeRequestHash(CreateTransactionRequest request)
    {
        var json = JsonSerializer.Serialize(new
        {
            request.FromAccountId,
            request.ToAccountId,
            request.Amount,
            request.Description
        });

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToBase64String(bytes);
    }
}
