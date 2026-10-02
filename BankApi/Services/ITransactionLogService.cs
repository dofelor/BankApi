using BankApi.DTOs.ResponseDTOs;

namespace BankApi.Services
{
    public interface ITransactionLogService
    {
        Task<List<TransactionLogResponseDto>> GetLogsAsync(int page = 1, int pageSize = 20);
    }
}
