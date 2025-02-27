namespace CarsStore.Contracts;
public class AuthResponse
{
    public string JwtToken { get; set; }
    public string RefreshToken { get; set; }
}
