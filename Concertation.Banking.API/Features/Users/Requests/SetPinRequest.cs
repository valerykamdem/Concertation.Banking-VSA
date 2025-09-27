namespace Concertation.Banking.API.Features.Users.Requests;

public record SetPinRequest(Guid UserId, string Pin);
