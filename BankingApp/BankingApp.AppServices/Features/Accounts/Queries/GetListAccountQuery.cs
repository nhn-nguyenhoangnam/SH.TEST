using MediatR;
using FluentValidation;
using BankingApp.AppServices.Features.Accounts.Repos;
using BankingApp.Domains.Features.Accounts;
using System.Linq.Expressions;
using LinqKit;

namespace BankingApp.AppServices.Features.Accounts.Queries;

public record GetListAccountRequest : IRequest<GetListAccountResponse>
{
    public string? AccountNumber { get; init; }
    public string? AccountHolderName { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record GetListAccountResponse(
    List<AccountDto> Accounts,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

public class GetListAccountQueryValidator : AbstractValidator<GetListAccountRequest>
{
    public GetListAccountQueryValidator()
    {
        RuleFor(x => x.AccountNumber)
            .MaximumLength(20)
            .WithMessage("Search text cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.AccountNumber));

        RuleFor(x => x.AccountHolderName)
            .MaximumLength(100)
            .WithMessage("Search text cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.AccountHolderName));

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");
    }
}

public class GetListAccountQueryHandler(
        IAccountRepository accountRepository
    ) : IRequestHandler<GetListAccountRequest, GetListAccountResponse>
{
    public async Task<GetListAccountResponse> Handle(GetListAccountRequest request, CancellationToken cancellationToken)
    {
        var predicate = PredicateBuilder.New<Account>(true);

        if (!string.IsNullOrEmpty(request.AccountNumber))
        {
            predicate = predicate.And(x => x.AccountNumber.Contains(request.AccountNumber, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(request.AccountHolderName))
        {
            predicate = predicate.And(x => x.AccountHolderName.Contains(request.AccountHolderName, StringComparison.OrdinalIgnoreCase));
        }

        var accounts = await accountRepository.GetListAsync(predicate, cancellationToken).ConfigureAwait(false);

        var totalCount = accounts.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedAccounts = accounts
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new AccountDto
            {
                Id = x.Id,
                AccountNumber = x.AccountNumber,
                AccountHolderName = x.AccountHolderName,
                Balance = x.Balance
            })
            .ToList();

        var response = new GetListAccountResponse(
            pagedAccounts,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages
        );

        return response;
    }
}