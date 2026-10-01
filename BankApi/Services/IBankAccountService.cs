using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;

namespace BankApi.Services
{
    public interface IBankAccountService
    {
        Task<BankAccountResponseDto> CreateAccountAsync(int clientId, CreateAccountDto dto);
        Task<BankAccountResponseDto?> GetAccountByIdAsync(int id);
        Task<List<BankAccountResponseDto>> GetAccountsByClientIdAsync(int clientId);
        Task<bool> CloseAccountAsync(int id);
    }
}
