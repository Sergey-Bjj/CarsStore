using CarsStore.Core.Models;

namespace CarsStore.Core.Abstractions;

public interface IJwtUtils
{
    string GenerateJwtToken(User user);
    RefreshToken GenerateRefreshToken();
    string GetUserIdFromToken(string token);
}
