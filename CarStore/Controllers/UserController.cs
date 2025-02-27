using CarsStore.Contracts;
using CarsStore.Core.Abstractions;
using CarsStore.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;

namespace CarsStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public UserController(IUserService userService, IMapper mapper, IMemoryCache memoryCache)
    {
        _userService = userService;
        _mapper = mapper;
        _memoryCache = memoryCache;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers()
    {
        string cacheKey = "AllUsers";
        if (!_memoryCache.TryGetValue(cacheKey, out List<UserDto> userDtos))
        { 
            var users = await _userService.GetAllUsers();
            userDtos = _mapper.Map<List<UserDto>>(users);
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            _memoryCache.Set(cacheKey, userDtos, cacheOptions);
        }
        return Ok(userDtos);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("{id}")]
    public async Task<ActionResult<string>> UpdateUser(string id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var updatedUserId = await _userService.UpdateUser(id, request.UserName, request.LastName, request.Age, request.Email);
            _memoryCache.Remove("AllUsers");
            return Ok(updatedUserId);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteUser(string id)
    {
        try
        {
            var deletedUserId = await _userService.DeleteUser(id);
            _memoryCache.Remove("AllUsers");
            return Ok(deletedUserId);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
