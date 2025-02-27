using CarsStore.Core.Models;
using CarStore.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CarStore.DataAccess.Repositories;


public class CarRepository : ICarRepository
{
    private readonly CarStoreDbContext _context;

    public CarRepository(CarStoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<Car>> GetAllCars()
    {
        return await _context.Cars
            .Include(c => c.User)
            .ToListAsync();
    }

    public async Task<Car> GetByIdAsync(int id)
    {
        return await _context.Cars
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> CreateAsync(Car car)
    {
        _context.Cars.Add(car);
        await _context.SaveChangesAsync();
        return car.Id;
    }

    public async Task<int> UpdateAsync(int id, string make, string model, decimal price)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            throw new InvalidOperationException($"Car with ID {id} not found.");
        }

        car.Make = make;
        car.Model = model;
        car.Price = price;

        _context.Cars.Update(car);
        await _context.SaveChangesAsync();
        return car.Id;
    }

    public async Task<int> DeleteAsync(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            throw new InvalidOperationException($"Car with ID {id} not found.");
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();
        return id;
    }
}
