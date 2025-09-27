using Concertation.Banking.API.Features.Users.models;
using Concertation.Banking.API.Shared.Interfaces;

namespace Concertation.Banking.API.Features.Users.UpdateUserProfile;

public record UpdateUserProfileRequest(
    string FirstName,
    string LastName,
    //string PhoneNumber,
    string CivilStatus
    //string? Address = null
);

public record UpdateUserPrepareResponse(
    string UserId,
    string Message
);

public class UpdateUserProfileHandler
{
    
    private readonly IPendingUpdateUserRedisStore _redisStore;
    

    public UpdateUserProfileHandler(
        IPendingUpdateUserRedisStore redisStore)
    {
        _redisStore = redisStore;
    }

    public async Task<UpdateUserPrepareResponse> HandleAsync(Guid userId, UpdateUserProfileRequest request)
    {

        var preview = new PendingUpdateUser
        {
            UserId = userId.ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            CivilStatus = request.CivilStatus,
        };

        await _redisStore.SaveAsync(preview, TimeSpan.FromMinutes(10));

        return new UpdateUserPrepareResponse(userId.ToString(), "Update en attente de code PIN.");
    }
}
