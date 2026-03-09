using MediatR;
using Microsoft.AspNetCore.Mvc;
using BankingApp.AppServices.Features.Transactions.Queries;
using BankingApp.AppServices.Features.Transactions.Commands;
using FluentValidation;

namespace BankingApp.Api.Endpoints;

internal sealed class TransactionEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("", async (
            [AsParameters] GetListTransactionRequest request,
            [FromServices] IMediator mediator) =>
        {
            var response = await mediator.Send(request).ConfigureAwait(false);
            return Results.Ok(response);
        })
        .WithName("GetTransactions")
        .WithDescription("Get paginated list of transactions with filtering and sorting")
        .WithSummary("Retrieve transactions with advanced filtering");

        group.MapPost("", async (
            [FromBody] CreateTransactionRequest request,
            [FromServices] IMediator mediator) =>
        {
            try
            {
                var response = await mediator.Send(request).ConfigureAwait(false);
                
                if (response.Status == "Failed")
                {
                    return Results.BadRequest(new { message = response.Message });
                }
                
                return Results.Created($"/api/transactions/{response.TransactionId}", response);
            }
            catch (ValidationException ex)
            {
                var errors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
                return Results.BadRequest(new { message = errors });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("CreateTransaction")
        .WithDescription("Create a new transaction to transfer money between accounts")
        .WithSummary("Transfer money from one account to another");
    }
}
