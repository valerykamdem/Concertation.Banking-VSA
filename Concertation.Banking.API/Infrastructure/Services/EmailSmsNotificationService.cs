namespace Concertation.Banking.API.Infrastructure.Services;

public class EmailSmsNotificationService : INotificationService
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        // Ici tu peux utiliser SendGrid, SMTP, etc.
        Console.WriteLine($"📧 Email envoyé à {email} - {subject}");
        await Task.CompletedTask;
    }

    public Task SendSmsAsync(string phoneNumber, string message)
    {
        Console.WriteLine($"📱 SMS envoyé à {phoneNumber} - {message}");
        return Task.CompletedTask;
    }
}
