namespace Concertation.Banking.API.Infrastructure.Services;

public class PinService
{
    public string HashPin(string pin) => BCrypt.Net.BCrypt.HashPassword(pin);
    public bool VerifyPin(string enteredPin, string storedHash) =>
        BCrypt.Net.BCrypt.Verify(enteredPin, storedHash);
}
