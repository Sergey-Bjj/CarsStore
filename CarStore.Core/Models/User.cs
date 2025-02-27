using Microsoft.AspNetCore.Identity;

namespace CarsStore.Core.Models;

public class User : IdentityUser
{
    public string LastName { get; set; }
    public int Age { get; set; }
    public List<Car> Cars { get; set; } = new();
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
