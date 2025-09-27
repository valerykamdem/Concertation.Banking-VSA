namespace Concertation.Banking.API.Features.Auth.Requests;

public record RegisterUserRequest(
    string Email,
    string Password,
    string CivilStatus,
    string FirstName,
    string LastName);
