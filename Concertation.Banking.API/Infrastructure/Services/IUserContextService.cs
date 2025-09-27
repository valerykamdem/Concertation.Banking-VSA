using Concertation.Banking.API.Shared.Dtos;

namespace Concertation.Banking.API.Infrastructure.Services;

public interface IUserContextService
{
    Task<UserContext> GetCurrentContext(HttpContext context);
}
