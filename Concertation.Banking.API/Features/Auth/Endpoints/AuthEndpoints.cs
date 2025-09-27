using Carter;
using Concertation.Banking.API.Features.Auth.Handlers;
using Concertation.Banking.API.Features.Auth.Models;
using Concertation.Banking.API.Features.Auth.Requests;
using Concertation.Banking.API.Features.Users.Entities;
using Concertation.Banking.API.Shared.Dtos;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Concertation.Banking.API.Features.Auth.Endpoints;

public class AuthEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Enregistrement des endpoints d'authentification
        app.MapPost("/api/auth/login", LoginEndpoint);
        app.MapPost("/api/auth/register", RegisterEndpoint);
        app.MapPost("/api/auth/refresh-token", RefreshTokenEndpoint);
    }

    private static async Task<IResult> LoginEndpoint(
        [FromBody] LoginRequest request,
        IKeycloakAuthService keycloakAuth,
        IValidator<LoginRequest> validator)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        // Appel à Keycloak pour obtenir le token
        TokenResponse? tokenResponse = await keycloakAuth.ExchangeCredentialsForTokenAsync(request.Email, request.Password);
        if (tokenResponse == null)
            return Results.Unauthorized();
        // Créer ou récupérer l'utilisateur dans ton app
        User user = await keycloakAuth.GetUserFromTokenAsync(tokenResponse.UserId, tokenResponse.Access_Token, string.Empty);
        return Results.Ok(new
        {
            UserId = user.Id,
            user.Email,
            Roles = user.Roles.Select(r => r.Role.Name),
            tokenResponse.Access_Token,
            tokenResponse.Refresh_Token
        });
    }

    private static async Task<IResult> RegisterEndpoint(
        [FromBody] RegisterUserRequest request,
        IKeycloakAuthService keycloakAuth,
        IValidator<RegisterUserRequest> validator)
    {

        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        // Appel à Keycloak pour créer l'utilisateur
        ApiResponseDto<object> response = await keycloakAuth.RegisterAsync(request);

        if (!response.Success)
            return Results.BadRequest(response.Errors);

        // Retourner les informations de l'utilisateur et le token
        return Results.Ok(response.Data);
    }

    private static async Task<IResult> RefreshTokenEndpoint(
        [FromBody] RefreshTokenRequest request,
        IKeycloakAuthService keycloakAuth,
        IValidator<RefreshTokenRequest> validator)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        // Appel à Keycloak pour obtenir le token
        TokenResponse? tokenResponse = await keycloakAuth.RefreshTokenAsync(request.RefreshToken);
        if (tokenResponse is null)
            return Results.Unauthorized();
        // Créer ou récupérer l'utilisateur dans ton app
        User user = await keycloakAuth.GetUserFromTokenAsync(tokenResponse.UserId, tokenResponse.Access_Token, string.Empty);
        return Results.Ok(tokenResponse);// new
        //{
        //    UserId = user.Id,
        //    user.Email,
        //    Roles = user.Roles.Select(r => r.Role.Name),
        //    tokenResponse.Access_Token,
        //    tokenResponse.Refresh_Token
        //});
    }
}
