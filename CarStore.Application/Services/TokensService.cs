using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarsStore.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class JwtSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenLifetimeMinutes { get; set; } = 5;
    public int RefreshTokenLifetimeDays { get; set; } = 7;
}

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<User> _userManager;
    private readonly CarStoreDbContext _context;

    public TokenService(
        IOptions<JwtSettings> jwtSettings,
        UserManager<User> userManager,
        CarStoreDbContext context)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
        _context = context;
    }

    public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var accessToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes),
            signingCredentials: creds);
        var refreshToken = Guid.NewGuid().ToString();
        await _context.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays)
        });
        await _context.SaveChangesAsync();
        return (new JwtSecurityTokenHandler().WriteToken(accessToken), refreshToken);
    }

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);
        if (token == null || token.Expires < DateTime.UtcNow)
            return null;
        var user = token.User;
        var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(user);
        token.IsRevoked = true;
        await _context.SaveChangesAsync();
        return (newAccessToken, newRefreshToken);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        
        if (token != null)
        {
            token.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }
}
