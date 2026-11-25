using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameApi.Data;
using GameApi.DTOs;
using GameApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GameApi.Services;

public class AuthService
{
    private readonly AppDbContext db;
    private readonly IConfiguration configuration;
    private readonly PasswordHasher<Account> hasher = new();

    public AuthService(AppDbContext db, IConfiguration configuration)
    {
        this.db = db;
        this.configuration = configuration;
    }

    public async Task<AccountResponse?> RegisterAsync(RegisterRequest request)
    {
        string username = request.Username.Trim();

        if (await db.Accounts.AnyAsync(account => account.Username == username))
        {
            return null;
        }

        Account account = new Account
        {
            Username = username,
            CreatedAt = DateTime.UtcNow
        };
        account.PasswordHash = hasher.HashPassword(account, request.Password);

        db.Accounts.Add(account);
        await db.SaveChangesAsync();

        return new AccountResponse(account.Id, account.Username);
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        string username = request.Username.Trim();
        Account? account = await db.Accounts.FirstOrDefaultAsync(item => item.Username == username);

        if (account is null)
        {
            return null;
        }

        PasswordVerificationResult result = hasher.VerifyHashedPassword(account, account.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return new LoginResponse(CreateToken(account), account.Username);
    }

    private string CreateToken(Account account)
    {
        string key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Thiếu Jwt:Key trong appsettings.json");

        Claim[] claims =
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.Username)
        };

        SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        SigningCredentials credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
