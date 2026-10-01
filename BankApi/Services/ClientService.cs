using AutoMapper;
using BankApi.Data;
using BankApi.Data.Models;
using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;
using BankApi.DTOs.UpdateDTOs;
using BankApi.Services.Generators;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ClientService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ClientResponseDto> CreateClientAsync(CreateClientDto dto)
        {
            var client = _mapper.Map<Client>(dto);

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return _mapper.Map<ClientResponseDto>(client);
        }
        public async Task<ClientResponseDto?> GetClientByIdAsync(int id)
        {
            var client = await _context.Clients
                .Include(c => c.PhoneNumbers)
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return null;

            return _mapper.Map<ClientResponseDto>(client);
        }
        public async Task<List<ClientResponseDto>> GetClientsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var clients = await _context.Clients
                .Include(c => c.PhoneNumbers)
                .Include(c => c.BankAccounts)
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<ClientResponseDto>>(clients);
        }
        public async Task<bool> DeleteClientAsync(int id)
        {
            bool hasBankAccounts = await _context.BankAccounts.AnyAsync(b => b.ClientId == id);

            if (hasBankAccounts)
            {
                throw new InvalidOperationException("A client with open bank accounts cannot be deleted.");
            }

            var client = await _context.Clients.FindAsync(id);
            if(client == null) return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<ClientResponseDto?> UpdateClientAsync(int id, UpdateClientDto updatedDto)
        {
            var client = await _context.Clients
                .Include(c => c.PhoneNumbers)
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return null;

            _mapper.Map(updatedDto, client);

            await _context.SaveChangesAsync();

            return _mapper.Map<ClientResponseDto?>(client);
        }
    }
}
