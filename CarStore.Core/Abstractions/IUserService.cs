using CarsStore.Contracts;
using CarsStore.Core.Models;
namespace CarsStore.Core.Abstractions;

public interface IUserService
{
    Task<List<User>> GetAllUsers();
    Task<string> CreateUser(string userName, string lastName, int age, string email, string password);
    Task<string> UpdateUser(string id, string userName, string lastName, int age, string email);
    Task<string> DeleteUser(string id);
    Task<AuthResponse> Authenticate(string userName, string password);
    Task<AuthResponse> RefreshToken(string token, string refreshToken);
}
