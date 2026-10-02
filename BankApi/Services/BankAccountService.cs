using AutoMapper;
using BankApi.Data;
using BankApi.Data.Models;
using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;
using BankApi.Infrastructure.Exceptions;
using BankApi.Services.Generators;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountNumberGenerator _accountNumberGenerator;

        public BankAccountService(AppDbContext context, IMapper mapper, IAccountNumberGenerator accountNumberGenerator)
        {
            _context = context;
            _mapper = mapper;
            _accountNumberGenerator = accountNumberGenerator;

        }

        public async Task<BankAccountResponseDto> CreateAccountAsync(int clientId, CreateAccountDto dto)
        {
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists)
            {
                throw new KeyNotFoundException($"Client with ID = {clientId} not found.");
            }

            var account = _mapper.Map<BankAccount>(dto);
            account.ClientId = clientId;
            account.AccountNumber = _accountNumberGenerator.Generate();
            account.Balance = 0;
            account.IsClosed = false;

            _context.BankAccounts.Add(account);
            await _context.SaveChangesAsync();

            return _mapper.Map<BankAccountResponseDto>(account);
        }

        public async Task<BankAccountResponseDto?> GetAccountByIdAsync(int id)
        {
            var account = await _context.BankAccounts
                .Include(b => b.Cards)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (account == null) return null;

            return _mapper.Map<BankAccountResponseDto>(account);
        }

        public async Task<List<BankAccountResponseDto>> GetAccountsByClientIdAsync(int clientId)
        {
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists)
            {
                throw new KeyNotFoundException($"Client with ID = {clientId} not found.");
            }

            var clientAccounts = await _context.BankAccounts
                .Include(b => b.Cards)
                .AsNoTracking()
                .Where(b => b.ClientId == clientId && !b.IsClosed)
                .OrderBy(b => b.Id)
                .ToListAsync();

            return _mapper.Map<List<BankAccountResponseDto>>(clientAccounts);
        }

        public async Task<bool> CloseAccountAsync(int id)
        {
            var account = await _context.BankAccounts
                .Include(b => b.Cards)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (account == null) return false;

            if (account.IsClosed)
            {
                throw new BusinessRuleException("This account is already closed.");
            }

            if(account.Balance != 0)
            {
                throw new BusinessRuleException($"Cannot close account with a non-zero balance. Current balance: {account.Balance} {account.Currency}. Please transfer or withdraw all funds first.");
            }

            bool hasActiveCards = account.Cards.Any(c => !c.IsBlocked);
            if (hasActiveCards)
            {
                throw new BusinessRuleException("Cannot close account with active linked cards. Block all cards first, then try again.");
            }
            account.IsClosed = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BankAccountResponseDto> AddBalanceAsync(int accountId, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));
            }

            var account = await _context.BankAccounts
                .FirstOrDefaultAsync(b => b.Id == accountId);

            if (account == null)
            {
                throw new NotFoundException("BankAccount", accountId);
            }

            if (account.IsClosed)
            {
                throw new BusinessRuleException("Cannot deposit to a closed account.");
            }

            // Обновляем баланс и логируем операцию в рамках одной транзакции
            account.Balance += amount;

            var log = new TransactionLog
            {
                FromAccountId = null,
                FromAccountNumber = string.Empty,
                ToAccountId = account.Id,
                ToAccountNumber = account.AccountNumber,
                Amount = amount,
                Currency = account.Currency,
                CreatedAt = DateTime.UtcNow,
                Description = "Deposit"
            };

            _context.TransactionLogs.Add(log);
            await _context.SaveChangesAsync();

            return _mapper.Map<BankAccountResponseDto>(account);
        }

    }
}
