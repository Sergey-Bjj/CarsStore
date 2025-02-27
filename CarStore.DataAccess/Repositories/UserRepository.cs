using CarsStore.Core.Abstractions;
using CarsStore.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarStore.DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly CarStoreDbContext _context;
    private readonly UserManager<User> _userManager;

    public UsersRepository(CarStoreDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Cars)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<string> CreateAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        return user.Id;
    }

    public async Task<string> UpdateAsync(string id, string userName, string lastName, int age, string email)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {id} not found.");
        }

        user.UserName = userName;
        user.LastName = lastName;
        user.Age = age;
        user.Email = email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to update user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        return id;
    }

    public async Task<string> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {id} not found.");
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        return id;
    }

    public async Task<User> FindByUsernameAsync(string userName)
    {
        if (string.IsNullOrEmpty(userName))
            throw new ArgumentException("UserName cannot be null or empty");

        string normalizedUserName = userName.ToUpper(); // Убедись, что нормализация есть

        return await _context.Users
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName);
    }
    
    public async Task<User> FindByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }
}
