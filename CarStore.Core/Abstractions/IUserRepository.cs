using CarsStore.Core.Models;

namespace CarsStore.Core.Abstractions;

public interface IUsersRepository
{
    Task<List<User>> GetAllAsync();
    Task<string> CreateAsync(User user, string password);
    Task<string> UpdateAsync(string id, string userName, string lastName, int age, string email);
    Task<string> DeleteAsync(string id);
    Task<User> FindByUsernameAsync(string userName);
    Task<User> FindByIdAsync(string id);
}
