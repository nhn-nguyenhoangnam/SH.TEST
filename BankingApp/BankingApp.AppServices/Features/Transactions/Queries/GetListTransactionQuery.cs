using MediatR;
using FluentValidation;
using BankingApp.AppServices.Features.Transactions.Repos;
using BankingApp.AppServices.Features.Transactions.Dtos;
using BankingApp.Domains.Features.Transactions;
using System.Linq.Expressions;
using LinqKit;

namespace BankingApp.AppServices.Features.Transactions.Queries;

public record GetListTransactionRequest : IRequest<GetListTransactionResponse>
{
    public Guid? FromAccountId { get; init; }
    public Guid? ToAccountId { get; init; }
    public string? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record GetListTransactionResponse(
    List<TransactionDto> Transactions,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

public class GetListTransactionQueryValidator : AbstractValidator<GetListTransactionRequest>
{
    public GetListTransactionQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be less than or equal to To date");
    }
}

public class GetListTransactionQueryHandler(
        ITransactionRepository transactionRepository
    ) : IRequestHandler<GetListTransactionRequest, GetListTransactionResponse>
{
    public async Task<GetListTransactionResponse> Handle(GetListTransactionRequest request, CancellationToken cancellationToken)
    {
        var predicate = PredicateBuilder.New<Transaction>(true);

        if (request.FromAccountId.HasValue)
        {
            predicate = predicate.And(x => x.FromAccountId == request.FromAccountId.Value);
        }

        if (request.ToAccountId.HasValue)
        {
            predicate = predicate.And(x => x.ToAccountId == request.ToAccountId.Value);
        }

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<TransactionStatus>(request.Status, true, out var status))
        {
            predicate = predicate.And(x => x.Status == status);
        }

        if (request.FromDate.HasValue)
        {
            predicate = predicate.And(x => x.TransactionDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            predicate = predicate.And(x => x.TransactionDate <= request.ToDate.Value);
        }

        var transactions = await transactionRepository.GetListAsync(predicate, cancellationToken).ConfigureAwait(false);

        var totalCount = transactions.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedTransactions = transactions
            .OrderByDescending(x => x.TransactionDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList()
            .Select(x => new TransactionDto
            {
                Id = x.Id,
                FromAccountId = x.FromAccountId,
                ToAccountId = x.ToAccountId,
                FromAccountNumber = x.FromAccount?.AccountNumber,
                ToAccountNumber = x.ToAccount?.AccountNumber,
                Amount = x.Amount,
                Description = x.Description,
                TransactionDate = x.TransactionDate,
                Status = x.Status.ToString()
            })
            .ToList();

        var response = new GetListTransactionResponse(
            pagedTransactions,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages
        );

        return response;
    }
}
