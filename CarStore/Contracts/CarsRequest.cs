namespace CarsStore.Contracts;
public class CreateCarRequest
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
}

public class UpdateCarRequest
{
    public string Make { get; set; }
    public string Model { get; set; }
    public decimal Price { get; set; }
}
