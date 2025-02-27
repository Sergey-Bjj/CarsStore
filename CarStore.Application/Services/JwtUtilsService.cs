using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CarsStore.Core.Abstractions;
using CarsStore.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CarStore.Application.Services;

public class JwtUtils : IJwtUtils
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtUtils> _logger;

    public JwtUtils(IConfiguration configuration, ILogger<JwtUtils> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    public string GenerateJwtToken(User user)
    {
        if (user == null || string.IsNullOrEmpty(user.Id))
            throw new ArgumentException("User or User ID cannot be null");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"] ?? 
                                         throw new ArgumentNullException("JWT Secret Key is missing"));
        if (key.Length < 16)
            throw new ArgumentException("JWT Secret Key must be at least 16 bytes long");

        var now = DateTime.UtcNow;
        _logger.LogInformation("Generating JWT token at UTC time: {Time}", now);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? "")
            }),
            NotBefore = now,
            Expires = now.AddMinutes(15),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = 
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        _logger.LogInformation("Generated token: {Token}", tokenString);
        return tokenString;
    }

    public RefreshToken GenerateRefreshToken()
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        };
    }

    public string GetUserIdFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new Exception("JWT token is empty or null");

        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken;
        try
        {
            jwtToken = tokenHandler.ReadJwtToken(token);
            Console.WriteLine("Token Claims: " +
                              string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}")));
        }
        catch (Exception ex)
        {
            throw new Exception($"Invalid JWT token format: {ex.Message}");
        }

        var claim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            Console.WriteLine("ClaimTypes.NameIdentifier not found. Trying 'nameid' directly...");
            claim = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid");
            if (claim == null)
                throw new Exception("User ID claim not found in token");
        }

        return claim.Value;
    }
}
