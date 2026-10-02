using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankApi.Data.Models;
using BankApi.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BankApi.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingByEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existingByEmail != null)
        {
            throw new InvalidOperationException($"User with email '{dto.Email}' already exists.");
        }

        var existingByName = await _userManager.FindByNameAsync(dto.Username);
        if (existingByName != null)
        {
            throw new InvalidOperationException($"User with username '{dto.Username}' already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            FullName = dto.FullName ?? dto.Username,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        var roleName = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role;
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        await _userManager.AddToRoleAsync(user, roleName);

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = GenerateJwtToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.UsernameOrEmail)
                   ?? await _userManager.FindByNameAsync(dto.UsernameOrEmail);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            throw new UnauthorizedAccessException("Invalid username/email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = GenerateJwtToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = roles,
            ExpiresAt = expiresAt
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Roles = roles
        };
    }

    public async Task SeedDefaultAdminAsync()
    {
        const string adminRole = "Admin";
        const string userRole = "User";

        if (!await _roleManager.RoleExistsAsync(adminRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        if (!await _roleManager.RoleExistsAsync(userRole))
        {
            await _roleManager.CreateAsync(new IdentityRole(userRole));
        }

        const string adminEmail = "admin@bank.com";
        const string adminUsername = "admin";

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminUsername,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }

    private (string token, DateTime expiresAt) GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "BankApi_Super_Secret_Jwt_Security_Key_2026_With_Minimum_32_Chars!";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "BankApi";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "BankApiClient";
        var expireMinutesStr = _configuration["Jwt:ExpireMinutes"] ?? "1440";
        var expireMinutes = double.TryParse(expireMinutesStr, out var m) ? m : 1440;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return (tokenHandler.WriteToken(tokenDescriptor), expiresAt);
    }
}
