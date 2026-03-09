using MediatR;
using FluentValidation;
using BankingApp.AppServices.Features.Accounts.Repos;

namespace BankingApp.AppServices.Features.Accounts.Queries;

public record GetAccountByIdRequest(Guid Id) : IRequest<GetAccountByIdResponse>;

public record GetAccountByIdResponse(AccountDto? Account);

public class GetAccountByIdQueryValidator : AbstractValidator<GetAccountByIdRequest>
{
    public GetAccountByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Account ID is required");
    }
}

public class GetAccountByIdQueryHandler(
        IAccountRepository accountRepository
    ) : IRequestHandler<GetAccountByIdRequest, GetAccountByIdResponse>
{
    public async Task<GetAccountByIdResponse> Handle(GetAccountByIdRequest request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);

        if (account == null)
        {
            return new GetAccountByIdResponse(null);
        }

        var accountDto = new AccountDto
        {
            Id = account.Id,
            AccountNumber = account.AccountNumber,
            AccountHolderName = account.AccountHolderName,
            Balance = account.Balance
        };

        return new GetAccountByIdResponse(accountDto);
    }
}
