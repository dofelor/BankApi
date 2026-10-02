using BankApi.Data.Models;
using BankApi.Data;
using BankApi.DTOs;
using BankApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task TransferAsync(TransferDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var fromAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(a => a.Id == dto.FromAccountId && !a.IsClosed);

            if (fromAccount == null)
            {
                throw new KeyNotFoundException($"Sender account with ID = {dto.FromAccountId} not found.");
            }

            var toAccount = await _context.BankAccounts
                .FirstOrDefaultAsync(a => a.Id == dto.ToAccountId && !a.IsClosed);

            if (toAccount == null)
            {
                throw new KeyNotFoundException($"Recipient account with ID = {dto.ToAccountId} not found.");
            }

            if (fromAccount.Currency != toAccount.Currency)
            {
                throw new BusinessRuleException($"Currency mismatch: sender account is {fromAccount.Currency}, recipient account is {toAccount.Currency}. Cross-currency transfers are not supported.");
            }

            if (fromAccount.Balance < dto.Amount)
            {
                throw new BusinessRuleException($"Insufficient funds. Available: {fromAccount.Balance} {fromAccount.Currency}, requested: {dto.Amount} {fromAccount.Currency}.");
            }

            fromAccount.Balance -= dto.Amount;
            toAccount.Balance += dto.Amount;

            await _context.SaveChangesAsync();

            var log = new TransactionLog
            {
                FromAccountId = fromAccount.Id,
                FromAccountNumber = fromAccount.AccountNumber,
                ToAccountId = toAccount.Id,
                ToAccountNumber = toAccount.AccountNumber,
                Amount = dto.Amount,
                Currency = fromAccount.Currency,
                CreatedAt = DateTime.UtcNow,
                Description = "Transfer"
            };

            _context.TransactionLogs.Add(log);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
