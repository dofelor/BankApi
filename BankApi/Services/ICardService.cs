using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;

namespace BankApi.Services
{
    public interface ICardService
    {
        Task<CardResponseDto> CreateCardAsync(int accountId, CreateCardDto dto);
        Task<CardResponseDto?> GetCardByIdAsync(int id);
        Task<List<CardResponseDto>> GetCardsByAccountIdAsync(int accountId);
        Task<bool> BlockCardAsync(int id);
        Task<bool> UnblockCardAsync(int id);
    }
}
