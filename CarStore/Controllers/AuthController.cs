using System.Text.Json;
using CarsStore.Contracts;
using CarsStore.Core.Abstractions;
using CarsStore.DTO;
using Microsoft.AspNetCore.Mvc;
using RegisterRequest = CarsStore.Contracts.RegisterRequest;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var userId = await _userService.CreateUser(request.UserName, request.LastName, request.Age, request.Email,
                request.Password);
            return Ok(userId);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        Console.WriteLine($"Request received: {JsonSerializer.Serialize(request)}");

        if (request == null)
        {
            return BadRequest("Request body cannot be null.");
        }

        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState errors: " + JsonSerializer.Serialize(ModelState));
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _userService.Authenticate(request.UserName, request.Password);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        Console.WriteLine($"Received Token: {request?.Token}");
        Console.WriteLine($"Received RefreshToken: {request?.RefreshToken}");
        try
        {
            var response = await _userService.RefreshToken(request.Token, request.RefreshToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}
