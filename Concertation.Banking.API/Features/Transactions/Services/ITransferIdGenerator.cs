namespace Concertation.Banking.API.Features.Transactions.Services;

public interface ITransferIdGenerator
{
    string GenerateInternalTransferId();
    string GenerateExternalTransferId();
}
