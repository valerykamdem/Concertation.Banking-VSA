using System.Security.Claims;
using Concertation.Banking.API.Features.Users.Entities;
using Concertation.Banking.API.Infrastructure.Database;
using Concertation.Banking.API.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Concertation.Banking.API.Infrastructure.Services;

public class UserContextService : IUserContextService
{
    private readonly AppDbContext _db;
    public UserContextService(AppDbContext db) => _db = db;

    public async Task<UserContext> GetCurrentContext(HttpContext context)
    {
        if (!context.User.Identity!.IsAuthenticated)
            throw new UnauthorizedAccessException("Utilisateur non authentifié");

        // 👇 Récupère le bon claim selon ce que renvoie Keycloak
        string identityId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //string identityId = GetClaim(context, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
        //              ?? GetClaim(context, "sid")
        //              ?? GetClaim(context, "nameidentifier");

        if (string.IsNullOrEmpty(identityId))
            throw new UnauthorizedAccessException("Identifiant utilisateur introuvable");

        User user = await _db.Users.FirstOrDefaultAsync(u => u.IdentityId == identityId) 
            ?? throw new UnauthorizedAccessException("Utilisateur inconnu");

        return new UserContext(identityId, user.Id);
    }

    private static string? GetClaim(HttpContext context, string claimType)
    {
        return context.User.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}
