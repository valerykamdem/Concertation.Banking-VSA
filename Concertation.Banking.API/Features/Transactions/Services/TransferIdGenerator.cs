namespace Concertation.Banking.API.Features.Transactions.Services;

public class TransferIdGenerator : ITransferIdGenerator
{
    public string GenerateInternalTransferId() =>
        $"INT-{Guid.CreateVersion7():N}";

    public string GenerateExternalTransferId() =>
        $"EXT-{Guid.CreateVersion7():N}";
}
