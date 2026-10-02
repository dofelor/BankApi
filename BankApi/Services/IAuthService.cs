using BankApi.DTOs.AuthDTOs;

namespace BankApi.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task SeedDefaultAdminAsync();
}
