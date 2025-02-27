using CarsStore.Core.Models;

namespace CarStore.Application.Services;


public interface ICarService
{
    Task<List<Car>> GetAllCars();
    Task<Car> GetCar(int id);
    Task<int> CreateCar(Car car);
    Task<int> UpdateCar(int id, string make, string model, decimal price);
    Task<int> DeleteCar(int id);
}
