namespace BankApi.DTOs.AuthDTOs;

public class LoginDto
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
