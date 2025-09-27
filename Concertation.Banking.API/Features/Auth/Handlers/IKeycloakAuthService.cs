using Concertation.Banking.API.Features.Auth.Models;
using Concertation.Banking.API.Features.Auth.Requests;
using Concertation.Banking.API.Features.Users.Entities;
using Concertation.Banking.API.Shared.Dtos;

namespace Concertation.Banking.API.Features.Auth.Handlers;

public interface IKeycloakAuthService
{
    Task<TokenResponse?> ExchangeCredentialsForTokenAsync(string email, string password);
    Task<TokenResponse?> RefreshTokenAsync(string refreshToken);
    Task<ApiResponseDto<bool>> CreateUserInKeycloak(RegisterUserRequest request);
    Task<User> GetUserFromTokenAsync(string identityId, string email, string civilStatus);
    Task<ApiResponseDto<object>> RegisterAsync(RegisterUserRequest request);
    Task<Guid> SetUserPin(Guid userId, string pin);
    Task UpdateUserAsync(string keycloakUserId, string firstName, string lastName);
}
