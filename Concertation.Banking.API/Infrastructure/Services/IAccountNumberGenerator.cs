namespace Concertation.Banking.API.Infrastructure.Services;

public interface IAccountNumberGenerator
{
    Task<string> GenerateUniqueAccountNumber(string prefix);
    //string GenerateCheckingAccountNumber();
    //string GenerateSavingsAccountNumber();
    //bool IsValidAccountNumber(string accountNumber);
    //bool IsCheckingAccountNumber(string accountNumber);
    //bool IsSavingsAccountNumber(string accountNumber);
}
