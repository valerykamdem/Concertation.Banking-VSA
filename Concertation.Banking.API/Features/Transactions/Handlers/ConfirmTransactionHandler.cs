using Concertation.Banking.API.Features.Accounts.Entities;
using Concertation.Banking.API.Features.Accounts.Handlers;
using Concertation.Banking.API.Features.Transactions.Entities;
using Concertation.Banking.API.Features.Transactions.Models;
using Concertation.Banking.API.Features.Transactions.Requests;
using Concertation.Banking.API.Features.Transactions.Responses;
using Concertation.Banking.API.Features.Transactions.Services;
using Concertation.Banking.API.Infrastructure.Database;
using Concertation.Banking.API.Infrastructure.Services;
using Concertation.Banking.API.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Concertation.Banking.API.Features.Transactions.Handlers;

internal sealed class ConfirmTransactionHandler
{
    private readonly AppDbContext _db;
    private readonly BalanceHistoryService _history;
    private readonly ValidateService _validateService;
    private readonly CheckUserRoleHandler _roleChecker;
    private readonly TransactionFactory _transactionFactory;
    private readonly IPendingTransactionRedisStore _redisStore;

    public ConfirmTransactionHandler(
        AppDbContext db,
        BalanceHistoryService history,
        ValidateService validateService,
        CheckUserRoleHandler roleChecker,
        TransactionFactory transactionFactory,
        IPendingTransactionRedisStore redisStore)
    {
        _db = db;
        _history = history;
        _validateService = validateService;
        _roleChecker = roleChecker;
        _transactionFactory = transactionFactory;
        _redisStore = redisStore;
    }

    public async Task<TransactionResponse> HandlerAsync(TransactionConfirmRequest request, string userId)
    {

        PendingTransaction pendingTrans = await _redisStore.GetAsync(request.TransactionId)
            ?? throw new InvalidOperationException("Aucune transaction en attente pour cet identifiant.");

        if (pendingTrans.UserId != userId)
            throw new UnauthorizedAccessException("Transaction non autorisée.");

        bool isAdmin = await _roleChecker.IsAdminAsync(userId);

        switch (pendingTrans.Type)
        {
            case TransactionType.Withdrawal:
            case TransactionType.Deposit:
                if (!isAdmin)
                    throw new UnauthorizedAccessException("Seul un administrateur peut effectuer cette opération");
                break;

        }

        // 🔒 Vérification du PIN
        bool validPin = await _validateService.ValidatePinAsync(userId, request.Pin);
        if (!validPin)
            throw new InvalidOperationException("Code PIN invalide.");

        // 📦 Exécution transactionnelle EF Core
        await using IDbContextTransaction transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            Account fromAccount = null!;
            Account toAccount = null!;
            Account targetAccount = null!;

            switch (pendingTrans.Type)
            {
                case TransactionType.InternalTransfer:
                    fromAccount = await GetAccountByNumber(pendingTrans.FromAccountNumber!);
                    toAccount = await GetAccountByNumber(pendingTrans.ToAccountNumber!);

                    if (!await _validateService.ValidateLinkedAccounts(fromAccount.AccountNumber, toAccount.AccountNumber))
                        throw new InvalidOperationException("Les comptes ne sont pas liés.");

                    if (fromAccount.Balance < pendingTrans.Amount)
                        throw new InvalidOperationException("Solde insuffisant.");

                    fromAccount.Balance -= pendingTrans.Amount;
                    toAccount.Balance += pendingTrans.Amount;

                    _db.Accounts.UpdateRange(fromAccount, toAccount);
                    await _db.SaveChangesAsync();

                    await _transactionFactory.CreateAsync(
                        fromAccount.Id, toAccount.Id, null, pendingTrans.Amount, request.TransactionId, pendingTrans.Reason, pendingTrans.Type);

                    await LogBalanceChange(fromAccount.Id, fromAccount.Balance + pendingTrans.Amount, fromAccount.Balance);
                    await LogBalanceChange(toAccount.Id, toAccount.Balance - pendingTrans.Amount, toAccount.Balance);

                    break;

                case TransactionType.Withdrawal:
                    targetAccount = await GetAccountByNumber(pendingTrans.AccountNumber!);

                    if (targetAccount.Balance < pendingTrans.Amount)
                        throw new InvalidOperationException("Solde insuffisant.");

                    targetAccount.Balance -= pendingTrans.Amount;
                    _db.Accounts.Update(targetAccount);
                    await _db.SaveChangesAsync();

                    await _transactionFactory.CreateAsync(
                        null, null, targetAccount.Id, pendingTrans.Amount, request.TransactionId, pendingTrans.Reason, pendingTrans.Type);

                    await LogBalanceChange(targetAccount.Id, targetAccount.Balance + pendingTrans.Amount, targetAccount.Balance);
                    break;

                case TransactionType.Deposit:
                    targetAccount = await GetAccountByNumber(pendingTrans.AccountNumber!);

                    targetAccount.Balance += pendingTrans.Amount;
                    _db.Accounts.Update(targetAccount);
                    await _db.SaveChangesAsync();

                    await _transactionFactory.CreateAsync(
                        null, null, targetAccount.Id, pendingTrans.Amount, request.TransactionId, pendingTrans.Reason, pendingTrans.Type);

                    await LogBalanceChange(targetAccount.Id, targetAccount.Balance - pendingTrans.Amount, targetAccount.Balance);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(pendingTrans.Type), "Type de transaction non supporté.");
            }

            await transaction.CommitAsync();
            await _redisStore.RemoveAsync(request.TransactionId);

            return new TransactionResponse(
                TransactionId: request.TransactionId,
                Message: "Transaction confirmée avec succès",
                NewFromBalance: fromAccount?.Balance ?? targetAccount?.Balance ?? 0,
                NewToBalance: toAccount?.Balance ?? 0,
                Timestamp: DateTime.UtcNow
            );
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    private async Task<Account> GetAccountByNumber(string accountNumber)
    {
        Account account = await _db.Accounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber) ?? throw new ArgumentException("Compte introuvable");

        return account;
    }

    //private async Task ValidateOwnership(string userId, Guid accountId) =>
    //    await _db.Accounts.AnyAsync(a => a.Id == accountId && a.UserId == Guid.Parse(userId));

    //private async Task ValidatePin(string userId, string pinInput, string storedHash)
    //{
    //    User? user = await _db.Users.FindAsync(Guid.Parse(userId));
    //    if (!_pinService.VerifyPin(pinInput, storedHash))
    //        throw new UnauthorizedAccessException("Code PIN invalide");
    //}

    private async Task LogBalanceChange(Guid accountId, decimal oldBalance, decimal newBalance)
    {
        await _history.LogAsync(accountId, oldBalance, newBalance);
    }
}
