using Carter;
using Concertation.Banking.API.Features.Auth.Handlers;
using Concertation.Banking.API.Features.Transactions.Responses;
using Concertation.Banking.API.Features.Users.Requests;
using Concertation.Banking.API.Features.Users.UpdateUserProfile;
using Concertation.Banking.API.Infrastructure.Services;
using Concertation.Banking.API.Shared.Dtos;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Concertation.Banking.API.Features.Users.Endpoints;

public class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Enregistrement des endpoints du user
        app.MapPost("/api/user/set-pin", SetPinEndpoint);
        app.MapPost("/api/user/update", UpdateUserEndpoint)
            .RequireAuthorization();
        app.MapPost("/api/user/confirm", ConfirmUpdateUserEndpoint)
            .RequireAuthorization();
    }

    private static async Task<IResult> UpdateUserEndpoint(
        HttpContext context,
        [FromBody] UpdateUserProfileRequest request,
        UpdateUserProfileHandler updateUserProfileHandler,
        IUserContextService userContextService,
        IValidator<UpdateUserProfileRequest> validator)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        try
        {
            UserContext userContext = await userContextService.GetCurrentContext(context);

            UpdateUserPrepareResponse result = await updateUserProfileHandler.HandleAsync(userContext.LocalUserId, request);
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

    private static async Task<IResult> ConfirmUpdateUserEndpoint(
        HttpContext context,
        [FromBody] UpdateUserProfileConfirmRequest request,
        ConfirmUpdateUserProfileHandler confirmHandler,
        IUserContextService userContextService,
        IValidator<UpdateUserProfileConfirmRequest> validator)
    {

        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        try
        {
            UserContext userContext = await userContextService.GetCurrentContext(context);

            UpdateUserProfileResponse result = await confirmHandler.HandlerAsync(request, userContext.LocalUserId.ToString());
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

    private static async Task<IResult> SetPinEndpoint(
        [FromBody] SetPinRequest request,
        IKeycloakAuthService keycloakAuth,
        IValidator<SetPinRequest> validator)
    {

        ValidationResult validatorResult = await validator.ValidateAsync(request);
        if (!validatorResult.IsValid)
            return Results.ValidationProblem(validatorResult.ToDictionary());

        await keycloakAuth.SetUserPin(request.UserId, request.Pin);
        return Results.Ok("PIN défini avec succès");
    }
}
