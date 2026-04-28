using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Api.Data;
using Api.DTOs;
using Api.Models;

namespace Api.Services;

/// Handles all authentication logic 
/// Communicates with DB to create jwt tokens

public class AuthService
{
    private readonly AppDbContext _context; /// db access
    private readonly IConfiguration _configuration; /// reads appsettings.json

    /// passing dependencies
    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// Register User
    
    public async Task<AuthResponse?> Register(RegisterRequest request, UserRole role = UserRole.Rider)
    {
        /// checks if email exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

        /// return null if taken
        if (existingUser != null)
            return null;
        
        var user = new User
        {
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            DisplayName = request.DisplayName,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        ///  adds the user to the database and save

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        /// return the response with a JWT token
        return CreateAuthResponse(user);
    }


    /// takes email and password, checks them, and returns a token if valid
    public async Task<AuthResponse?> Login(LoginRequest request)
    {
        /// finds user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

        /// if no user found, return null
        if (user == null)
            return null;

        /// if the account has been deactivated, return null
        if (!user.IsActive)
            return null;

        /// BCrypt.Verify compares the plain text password with the stored hash
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        /// everything is correct = return the response with a JWT token
        return CreateAuthResponse(user);
    }


    /// builds the response object with a JWT token
    private AuthResponse CreateAuthResponse(User user)
    {
        return new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role.ToString(),
            Token = GenerateJwtToken(user)
        };
    }


    /// generates JWT token string that gets sent to the frontend
    private string GenerateJwtToken(User user)
    {
        /// the frontend can read these to know who is logged in and what role they have
        var claims = new[]
        {
            /// claim for user ID
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            /// claim for email
            new Claim(ClaimTypes.Email, user.Email),
            /// claim for role (Admin, Driver, or Rider)
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            /// claim for display name
            new Claim("DisplayName", user.DisplayName)
        };

        /// create the signing key from our secret in appsettings.json
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        /// Create the credentials using the key 
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        /// Build the token 
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
            signingCredentials: credentials
        );

        /// Convert the token into a string and return it
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
    

