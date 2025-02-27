namespace CarsStore.Contracts;
public class RegisterRequest
{
    public string UserName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
