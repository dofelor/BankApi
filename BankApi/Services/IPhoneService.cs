using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;

namespace BankApi.Services
{
    public interface IPhoneService
    {
        Task<PhoneResponseDto> CreatePhoneAsync(int clientId, CreatePhoneDto dto);
        Task<PhoneResponseDto?> GetPhoneByIdAsync(int id);
        Task<List<PhoneResponseDto>> GetPhonesByClientIdAsync(int clientId);
        Task<bool> DeletePhoneAsync(int id);
    }
}
