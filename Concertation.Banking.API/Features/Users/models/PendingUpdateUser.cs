namespace Concertation.Banking.API.Features.Users.models;

public class PendingUpdateUser
{
    public string UserId { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string CivilStatus { get; set; } = default!;
    //public string PhoneNumber { get; set; } = default!;
    //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //public DateTime? UpdatedAt { get; set; }
}
