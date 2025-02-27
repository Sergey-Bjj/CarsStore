namespace CarsStore.Core.DTO;

public class UserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public List<CarDto> Cars { get; set; } = new List<CarDto>();
}