using Carter;
using Concertation.Banking.API.Features.Transactions.Handlers;
using Concertation.Banking.API.Features.Transactions.Requests;
using Concertation.Banking.API.Features.Transactions.Responses;
using Concertation.Banking.API.Infrastructure.Services;
using Concertation.Banking.API.Shared.Dtos;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Concertation.Banking.API.Features.Transactions.Endpoints;

public class TransactionEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/transactions/transfer", TransferEndpoint)
            .RequireAuthorization();
        app.MapPost("/api/transactions/transaction", TransactionEndpoint)
            .RequireAuthorization();
        app.MapPost("/api/transactions/confirm", ConfirmTransactionEndpoint)
            .RequireAuthorization();
        //app.MapGet("/api/transactions/pending/{transferId}", PendingTransferEndpoint)
        //    .RequireAuthorization();
    }

    private static async Task<IResult> TransferEndpoint(
        HttpContext context,
        [FromBody] TransferRequest request,
        PendingTransferHandler pendingHandler,
        IUserContextService userContextService,
        IValidator<TransferRequest> validator)
    {

        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        try
        {
            UserContext userContext = await userContextService.GetCurrentContext(context);

            TransactionPrepareResponse result = await pendingHandler.HandleAsync(request, userContext.LocalUserId.ToString());
            return Results.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> TransactionEndpoint(
        HttpContext context,
        [FromBody] TransactionRequest request,
        PendingTransactionHandler pendingHandler,
        IUserContextService userContextService,
        IValidator<TransactionRequest> validator)
    {

        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        try
        {
            UserContext userContext = await userContextService.GetCurrentContext(context);

            TransactionPrepareResponse result = await pendingHandler.HandleAsync(request, userContext.LocalUserId.ToString());
            return Results.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> ConfirmTransactionEndpoint(
    HttpContext context,
    [FromBody] TransactionConfirmRequest request,
    ConfirmTransactionHandler confirmHandler,
    IUserContextService userContextService,
    IValidator<TransactionConfirmRequest> validator)
    {

        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        try
        {
            UserContext userContext = await userContextService.GetCurrentContext(context);

            TransactionResponse result = await confirmHandler.HandlerAsync(request, userContext.LocalUserId.ToString());
            return Results.Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Unauthorized();
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
}
