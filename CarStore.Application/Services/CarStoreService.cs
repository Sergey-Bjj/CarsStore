using System.Security.Claims;
using CarsStore.Core.Models;
using CarStore.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CarStore.Application.Services;
public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CarService(ICarRepository carRepository, IHttpContextAccessor httpContextAccessor)
    {
        _carRepository = carRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<Car>> GetAllCars()
    {
        return await _carRepository.GetAllCars();
    }

    public async Task<Car> GetCar(int id)
    {
        var car = await _carRepository.GetByIdAsync(id);
        if (car == null)
        {
            throw new InvalidOperationException($"Car with ID {id} not found.");
        }
        return car;
    }

    public async Task<int> CreateCar(Car car)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("User not authenticated.");
        }
        car.UserId = userId;
        return await _carRepository.CreateAsync(car);
    }

    public async Task<int> UpdateCar(int id, string make, string model, decimal price)
    {
        return await _carRepository.UpdateAsync(id, make, model, price);
    }

    public async Task<int> DeleteCar(int id)
    {
        return await _carRepository.DeleteAsync(id);
    }
    
}
