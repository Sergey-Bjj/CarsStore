using CarsStore.Contracts;
using CarsStore.Core.Abstractions;
using CarsStore.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarStore.Application.Services;

public class UserService : IUserService
{
    private readonly IUsersRepository _usersRepository;
    private readonly UserManager<User> _userManager;
    private readonly IJwtUtils _jwtUtils;
    private readonly CarStoreDbContext _context;
    

    public UserService(IUsersRepository usersRepository, UserManager<User> userManager, IJwtUtils jwtUtils, CarStoreDbContext context)
    {
        _usersRepository = usersRepository;
        _userManager = userManager;
        _jwtUtils = jwtUtils;
        _context = context;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await _usersRepository.GetAllAsync();
    }

    public async Task<string> CreateUser(string userName, string lastName, int age, string email, string password)
    {
        var user = new User
        {
            UserName = userName,
            LastName = lastName,
            Age = age,
            Email = email
        };
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        return user.Id;
    }

    public async Task<string> UpdateUser(string id, string userName, string lastName, int age, string email)
    {
        return await _usersRepository.UpdateAsync(id, userName, lastName, age, email);
    }

    public async Task<string> DeleteUser(string id)
    {
        return await _usersRepository.DeleteAsync(id);
    }

    public async Task<AuthResponse> Authenticate(string userName, string password)
    {
        var user = await _usersRepository.FindByUsernameAsync(userName);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            throw new Exception("Invalid username or password");
        }

        var token = _jwtUtils.GenerateJwtToken(user);
        var refreshToken = _jwtUtils.GenerateRefreshToken();
        user.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            JwtToken = token,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<AuthResponse> RefreshToken(string token, string refreshToken)
    {
        var userId = _jwtUtils.GetUserIdFromToken(token);
        Console.WriteLine($"UserId from token: {userId}");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            Console.WriteLine($"User not found for ID: {userId}");
            throw new Exception("Invalid or expired refresh token");
        }

        Console.WriteLine($"User found: {user.UserName}");
        
        var validRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == refreshToken && !rt.IsRevoked && rt.Expires > DateTime.UtcNow);

        if (validRefreshToken == null)
        {
            Console.WriteLine($"No valid refresh token found for: {refreshToken}");
            var allTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();
            Console.WriteLine("All stored RefreshTokens: " + string.Join(", ", allTokens.Select(rt => $"Token={rt.Token}, Expires={rt.Expires}, IsRevoked={rt.IsRevoked}")));
            throw new Exception("Invalid or expired refresh token");
        }

        Console.WriteLine($"Valid RefreshToken found: {validRefreshToken.Token}, Expires: {validRefreshToken.Expires}");

        var newJwtToken = _jwtUtils.GenerateJwtToken(user);
        var newRefreshToken = _jwtUtils.GenerateRefreshToken();
        
        _context.RefreshTokens.Remove(validRefreshToken);
        newRefreshToken.UserId = userId;
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            JwtToken = newJwtToken,
            RefreshToken = newRefreshToken.Token
        };
    }
}
