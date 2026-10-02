namespace BankApi.DTOs.AuthDTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public DateTime ExpiresAt { get; set; }
}
