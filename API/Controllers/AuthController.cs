using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Models;
using Api.Services;

namespace Api.Controllers;

// This is the API controller that handles registration and login requests
// [Route("api/[controller]")] means all endpoints start with /api/auth
// So register is POST /api/auth/register and login is POST /api/auth/login
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    // AuthService gets automatically injected
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }


    // POST /api/auth/register
    // Anyone can hit this endpoint - no login required
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // Call the service to create the user
        var response = await _authService.Register(request);

        // If null, the email was already taken
        if (response == null)
            return BadRequest(new { message = "Email is already registered" });

        // Return with the user info and token
        return CreatedAtAction(nameof(Register), response);
    }

    
    // REGISTER AN ADMIN (only existing admins can create new admins)
    // POST /api/auth/register-admin
    // [Authorize(Roles = "Admin")] means only logged-in admins can use this
    // If a Rider or Driver tries to login as admin they get a 403 Forbidden
    [HttpPost("register-admin")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterAdmin(RegisterRequest request)
    {
        var response = await _authService.Register(request, UserRole.Admin);

        if (response == null)
            return BadRequest(new { message = "Email is already registered" });

        return CreatedAtAction(nameof(RegisterAdmin), response);
    }

    // REGISTER A DRIVER (only admins can create driver accounts)
    // POST /api/auth/register-driver
    [HttpPost("register-driver")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegisterDriver(RegisterRequest request)
    {
        var response = await _authService.Register(request, UserRole.Driver);

        if (response == null)
            return BadRequest(new { message = "Email is already registered" });

        return CreatedAtAction(nameof(RegisterDriver), response);
    }

    // LOGIN
    // POST /api/auth/login
    // Anyone can hit this endpoint - no login required
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.Login(request);

        // If null, either email not found or wrong password
        // We don't tell them which one for security reasons
        if (response == null)
            return Unauthorized(new { message = "Invalid email or password" });

        // Return 200 OK with the user info and token
        return Ok(response);
    }
}