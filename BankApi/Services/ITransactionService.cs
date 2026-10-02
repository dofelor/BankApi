using BankApi.DTOs;

namespace BankApi.Services
{
    public interface ITransactionService
    {
        Task TransferAsync(TransferDto dto);
    }
}
