using CarsStore.Contracts;
using CarsStore.Core.Models;
using CarStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;
using CarsStore.Core.DTO;
using Microsoft.Extensions.Caching.Memory;


[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public CarController(ICarService carService, IMapper mapper, IMemoryCache memoryCache)
    {
        _carService = carService;
        _mapper = mapper;
        _memoryCache = memoryCache;
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<ActionResult<List<CarDto>>> GetAllCars()
    {
        string cacheKey = "AllCars";
        if (!_memoryCache.TryGetValue(cacheKey, out List<CarDto> carDtos))
        {
            var cars = await _carService.GetAllCars();
            carDtos = _mapper.Map<List<CarDto>>(cars);
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            _memoryCache.Set(cacheKey, carDtos, cacheOptions);
        }

        return Ok(carDtos);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("{id}")]
    public async Task<ActionResult<CarDto>> GetCar(int id)
    {
        string cacheKey = $"Car_{id}";
        if (!_memoryCache.TryGetValue(cacheKey, out CarDto carDto))
        {
            var car = await _carService.GetCar(id);
            if (car == null)
            {
                return NotFound("Car not found");
            }
            carDto = _mapper.Map<CarDto>(car);
            _memoryCache.Set(cacheKey, carDto, TimeSpan.FromMinutes(5));
        }
        return Ok(carDto);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    public async Task<ActionResult<int>> CreateCar([FromBody] CreateCarRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated");
        }
        var car = new Car
        {
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            Price = request.Price,
            UserId = userId
        };
        var carId = await _carService.CreateCar(car);
        _memoryCache.Remove("AllCars");
        return CreatedAtAction(nameof(GetCar), new { id = carId }, carId);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("{id}")]
    public async Task<ActionResult<int>> UpdateCar(int id, [FromBody] UpdateCarRequest request)
    {
        try
        {
            var updatedCarId = await _carService.UpdateCar(id, request.Make, request.Model, request.Price);
            _memoryCache.Remove("AllCars");
            return Ok(updatedCarId);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<int>> DeleteCar(int id)
    {
        try
        {
            var deletedCarId = await _carService.DeleteCar(id);
            _memoryCache.Remove("AllCars");
            return Ok(deletedCarId);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
    