using BankApi.DTOs.CreateDTOs;
using BankApi.DTOs.ResponseDTOs;
using BankApi.DTOs.UpdateDTOs;
using System.Globalization;

namespace BankApi.Services
{
    public interface IClientService
    {
        Task<ClientResponseDto> CreateClientAsync(CreateClientDto dto);
        Task<ClientResponseDto?> GetClientByIdAsync(int id);
        Task<List<ClientResponseDto>> GetClientsAsync(int pageNumber = 1, int pageSize = 10);
        Task<bool> DeleteClientAsync(int id);
        Task<ClientResponseDto?> UpdateClientAsync(int id, UpdateClientDto updatedDto);
        Task<int> GetClientsCountAsync();
    }
}
