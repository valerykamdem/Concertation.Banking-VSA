using Concertation.Banking.API.Features.Transactions.Models;
using Concertation.Banking.API.Features.Transactions.Requests;
using Concertation.Banking.API.Features.Transactions.Responses;
using Concertation.Banking.API.Features.Transactions.Services;
using Concertation.Banking.API.Shared.Interfaces;

namespace Concertation.Banking.API.Features.Transactions.Handlers;

public class PendingTransactionHandler
{
    private readonly ITransferIdGenerator _idGenerator;
    private readonly IPendingTransactionRedisStore _redisStore;


    public PendingTransactionHandler(
        ITransferIdGenerator idGenerator,
        IPendingTransactionRedisStore redisStore)
    {
        _idGenerator = idGenerator;
        _redisStore = redisStore;
    }

    public async Task<TransactionPrepareResponse> HandleAsync(TransactionRequest request, string userId)
    {

        string transferId = _idGenerator.GenerateInternalTransferId();

        var preview = new PendingTransaction
        {
            TransferId = transferId,
            UserId = userId,
            Type = request.Type,
            Amount = request.Amount,
            AccountNumber = request.AccountNumber
        };

        await _redisStore.SaveAsync(preview, TimeSpan.FromMinutes(10));

        return new TransactionPrepareResponse(transferId, "Transaction en attente de code PIN.");
    }
}
