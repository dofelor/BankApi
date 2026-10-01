using AutoMapper;
using BankApi.Data;
using BankApi.Data.Models;
using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;
using BankApi.Services.Generators;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services
{
    public class CardService : ICardService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly ICardNumberGenerator _cardNumberGenerator;
        public CardService(AppDbContext context, IMapper mapper, ICardNumberGenerator cardNumberGenerator)
        {
            _context = context;
            _mapper = mapper;
            _cardNumberGenerator = cardNumberGenerator;
        }

        public async Task<CardResponseDto> CreateCardAsync(int accountId, CreateCardDto dto)
        {
            var account = await _context.BankAccounts.FirstOrDefaultAsync(b => b.Id == accountId);

            if (account == null)
            {
                throw new KeyNotFoundException($"Bank account with ID = {accountId} not found.");
            }

            if (account.IsClosed)
            {
                throw new InvalidOperationException("Cannot issue a card for a closed bank account.");
            }

            var card = new Card
            {
                BankAccountId = accountId,
                CardType = dto.CardType,
                CardNumber = _cardNumberGenerator.GenerateCardNumber(),
                ValidityPeriod = _cardNumberGenerator.GenerateValidityPeriod(),
                IsBlocked = false
            };

            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return _mapper.Map<CardResponseDto>(card);
        }
        public async Task<CardResponseDto?> GetCardByIdAsync(int id)
        {
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
            if(card == null)
            {
                return null;
            }

            return _mapper.Map<CardResponseDto>(card);
        }

        public async Task<List<CardResponseDto>> GetCardsByAccountIdAsync(int accountId)
        {
            var accountExists = await _context.BankAccounts.AnyAsync(b => b.Id == accountId);
            if (!accountExists)
            {
                throw new KeyNotFoundException($"Bank account with ID = {accountId} not found.");
            }

            var cards = await _context.Cards
                .AsNoTracking()
                .Where(c => c.Id == accountId && !c.IsBlocked)
                .OrderBy(c => c.Id)
                .ToListAsync();

            return _mapper.Map<List<CardResponseDto>>(cards);
        }

        public async Task<bool> BlockCardAsync(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null) return false;

            if (card.IsBlocked)
            {
                throw new InvalidOperationException("Card is already blocked.");
            }

            card.IsBlocked = true;
            await _context.SaveChangesAsync();
            return true;

        }
        public async Task<bool> UnblockCardAsync(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null) return false;

            if (!card.IsBlocked)
            {
                throw new InvalidOperationException("Card is not blocked.");
            }

            card.IsBlocked = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
