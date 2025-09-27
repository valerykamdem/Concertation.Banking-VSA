using Concertation.Banking.API.Features.Transactions.Entities;

namespace Concertation.Banking.API.Features.Transactions.Requests;

public record TransferRequest(
    string? FromAccountNumber,
    string? ToAccountNumber,
    string? AccountNumber, // Pour dépôt/retrait
    decimal Amount,
    string? Reason,
    TransactionType Type);

public record TransactionRequest(
    string? AccountNumber, // Pour dépôt/retrait
    decimal Amount,
    TransactionType Type);

public record PendingTransactionRequest(
    string TransactionId,
    TransactionType Type,
    decimal Amount,
    string? FromAccountNumber,
    string? ToAccountNumber,
    string? AccountNumber,
    string? Reason,
    DateTime CreatedAt
);

public record TransactionConfirmRequest(
    string TransactionId,
    string Pin
);
