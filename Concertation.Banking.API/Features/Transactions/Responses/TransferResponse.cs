namespace Concertation.Banking.API.Features.Transactions.Responses;

public record TransactionResponse(
    string TransactionId,
    string Message,
    decimal NewFromBalance,
    decimal NewToBalance,
    DateTime Timestamp);


public record TransactionPrepareResponse(
    string TransactionId,
    string Message
);
