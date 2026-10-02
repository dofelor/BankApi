using AutoMapper;
using BankApi.Data;
using BankApi.Data.Models;
using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services
{
    public class PhoneService : IPhoneService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PhoneService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PhoneResponseDto> CreatePhoneAsync(int clientId, CreatePhoneDto dto)
        {
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists)
            {
                throw new KeyNotFoundException($"Client with ID = {clientId} not found.");
            }

            var phoneExists = await _context.Phones.AnyAsync(p => p.PhoneNumber == dto.PhoneNumber);
            if (phoneExists)
            {
                throw new InvalidOperationException("This phone number is already linked to the system.");
            }

            var phone = _mapper.Map<Phone>(dto);
            phone.ClientId = clientId;

            _context.Phones.Add(phone);
            await _context.SaveChangesAsync();

            return _mapper.Map<PhoneResponseDto>(phone);
        }

        public async Task<PhoneResponseDto?> GetPhoneByIdAsync(int id)
        {
            var phone = await _context.Phones.FindAsync(id);
            if (phone == null) return null;

            return _mapper.Map<PhoneResponseDto>(phone);
        }
        public async Task<List<PhoneResponseDto>> GetPhonesByClientIdAsync(int clientId)
        {
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists)
            {
                throw new KeyNotFoundException($"Client with ID = {clientId} not found.");
            }

            var phones = await _context.Phones
                .Where(p => p.ClientId == clientId)
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToListAsync();

            return _mapper.Map<List<PhoneResponseDto>>(phones);
        }

        public async Task<bool> DeletePhoneAsync(int id)
        {
            var phone = await _context.Phones.FindAsync(id);
            if (phone == null) return false;

            phone.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
