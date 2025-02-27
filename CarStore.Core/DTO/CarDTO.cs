namespace CarsStore.Core.DTO;

public class CarDto
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string UserId { get; set; } 
}
