using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mundial2026.Api.Data;
using Mundial2026.Api.Models;

namespace Mundial2026.Api.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string username, string password);
    Task<AuthResult> RegisterAsync(string username, string email, string password);
}

public record AuthResult(bool Success, string? Token, string? Error, UserDto? User);
public record UserDto(int Id, string Username, string Email, string Role);

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Username == username || u.Email == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return new AuthResult(false, null, "Credenciales inválidas", null);

        var token = GenerateToken(user);
        return new AuthResult(true, token, null, new UserDto(user.Id, user.Username, user.Email, user.Role));
    }

    public async Task<AuthResult> RegisterAsync(string username, string email, string password)
    {
        if (await _db.Users.AnyAsync(u => u.Username == username))
            return new AuthResult(false, null, "El nombre de usuario ya existe", null);

        if (await _db.Users.AnyAsync(u => u.Email == email))
            return new AuthResult(false, null, "El correo ya está registrado", null);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "user"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = GenerateToken(user);
        return new AuthResult(true, token, null, new UserDto(user.Id, user.Username, user.Email, user.Role));
    }

    private string GenerateToken(User user)
    {
        var key = _config["Jwt:Key"] ?? "Mundial2026SecretKey!SuperSecure#2026";
        var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
