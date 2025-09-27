using Concertation.Banking.API.Features.Auth.Handlers;
using Concertation.Banking.API.Features.Users.Entities;
using Concertation.Banking.API.Features.Users.models;
using Concertation.Banking.API.Infrastructure.Database;
using Concertation.Banking.API.Infrastructure.Services;
using Concertation.Banking.API.Shared.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Concertation.Banking.API.Features.Users.UpdateUserProfile;

public record UpdateUserProfileResponse(
    string UserId,
    string Message,
    DateTime Timestamp);

public record UpdateUserProfileConfirmRequest(
    string UserId,
    string Pin
);

public class ConfirmUpdateUserProfileHandler
{
    private readonly AppDbContext _db;
    private readonly IPendingUpdateUserRedisStore _redisStore;
    private readonly ValidateService _validateService;
    private readonly IKeycloakAuthService _keycloakAuthService;

    public ConfirmUpdateUserProfileHandler(
        AppDbContext db,
        IPendingUpdateUserRedisStore redisStore,
        ValidateService validateService,
        IKeycloakAuthService keycloakAuthService)
    {
        _db = db;
        _redisStore = redisStore;
        _validateService = validateService;
        _keycloakAuthService = keycloakAuthService;
    }

    public async Task<UpdateUserProfileResponse> HandlerAsync(UpdateUserProfileConfirmRequest request, string userId)
    {

        // 📦 Exécution transactionnelle EF Core
        await using IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync();

        try
        {

            // Vérifier si l'utilisateur a un enregistrement en attente dans Redis
            PendingUpdateUser pendingUser = await _redisStore.GetAsync(request.UserId)
                ?? throw new InvalidOperationException("Aucune update en attente pour cet identifiant.");

            if (pendingUser.UserId != userId)
                throw new UnauthorizedAccessException("Transaction non autorisée.");

            // Vérifier le code PIN
            // 🔒 Vérification du PIN
            bool validPin = await _validateService.ValidatePinAsync(userId, request.Pin);
            if (!validPin)
                throw new InvalidOperationException("Code PIN invalide.");

            // Récupérer l'utilisateur de la base de données
            User? user = await _db.Users.FindAsync(Guid.Parse(userId)) ?? throw new KeyNotFoundException("User not found");

            // 👇 Met à jour les données dans Keycloak
            await _keycloakAuthService.UpdateUserAsync(user.IdentityId, pendingUser.FirstName, pendingUser.LastName);

            // Mettre à jour les informations de l'utilisateur
            user.FirstName = pendingUser.FirstName;
            user.LastName = pendingUser.LastName;
            //user.PhoneNumber = pendingUser.PhoneNumber;
            user.CivilStatus = pendingUser.CivilStatus;
            //user.Address = request.Address;
            // Enregistrer les modifications dans la base de données
            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
            await _redisStore.RemoveAsync(request.UserId);

            return new UpdateUserProfileResponse(
                UserId: string.Empty,
                Message: "User profile updated successfully",
                Timestamp: DateTime.UtcNow);

        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}
