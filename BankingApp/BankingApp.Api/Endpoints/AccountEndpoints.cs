using MediatR;
using Microsoft.AspNetCore.Mvc;
using BankingApp.AppServices.Features.Accounts.Queries;

namespace BankingApp.Api.Endpoints;

internal sealed class AccountEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("", async (
            [AsParameters] GetListAccountRequest request,
            [FromServices] IMediator mediator) =>
        {
            var response = await mediator.Send(request).ConfigureAwait(false);
            return Results.Ok(response);
        })
        .WithName("GetAccounts")
        .WithDescription("Get paginated list of accounts with filtering and sorting")
        .WithSummary("Retrieve accounts with advanced filtering");

        group.MapGet("{id:guid}", async (
            Guid id,
            [FromServices] IMediator mediator) =>
        {
            var request = new GetAccountByIdRequest(id);
            var response = await mediator.Send(request).ConfigureAwait(false);
            
            if (response.Account == null)
            {
                return Results.NotFound(new { message = "Account not found" });
            }
            
            return Results.Ok(response.Account);
        })
        .WithName("GetAccountById")
        .WithDescription("Get account details by ID")
        .WithSummary("Retrieve a specific account by its unique identifier");
    }
}