using AutoMapper;
using BankApi.Data;
using BankApi.DTOs.ResponseDTOs;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services
{
    public class TransactionLogService : ITransactionLogService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TransactionLogService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<TransactionLogResponseDto>> GetLogsAsync(int page = 1, int pageSize = 20)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 20;

            var logs = await _context.TransactionLogs
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<TransactionLogResponseDto>>(logs);
        }

        public async Task<int> GetLogsCountAsync()
        {
            return await _context.TransactionLogs.CountAsync();
        }
    }
}
