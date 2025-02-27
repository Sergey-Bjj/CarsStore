using CarsStore.Core.Models;

namespace CarStore.Core.Abstractions;


public interface ICarRepository
{
    Task<List<Car>> GetAllCars();
    Task<Car> GetByIdAsync(int id);
    Task<int> CreateAsync(Car car);
    Task<int> UpdateAsync(int id, string make, string model, decimal price);
    Task<int> DeleteAsync(int id);
}
