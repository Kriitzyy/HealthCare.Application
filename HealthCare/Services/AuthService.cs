// HealthcareAPI/Services/AuthService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthcareAPI.Models;
using HealthcareAPI.DTOs;
using HealthCare; 

namespace HealthcareAPI.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var exists = await _context.Patients
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (exists != null)
            throw new Exception("Email already exists");

        var patient = new Patient
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Hitta patient med email
        var patient = await _context.Patients
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (patient == null)
            throw new UnauthorizedAccessException("Invalid email or password");

        // Verifiera lösenord
        bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, patient.PasswordHash);
        
        if (!isValidPassword)
            throw new UnauthorizedAccessException("Invalid email or password");

        // Generera JWT token
        var token = GenerateJwtToken(patient);

        return new LoginResponse
        {
            Token = token,
            Email = patient.Email,
            Name = patient.Name
        };
    }

    private string GenerateJwtToken(Patient patient)
    {
        var jwtKey = _configuration["Jwt:Key"] 
            ?? throw new Exception("JWT Key missing");
        
        var key = Encoding.UTF8.GetBytes(jwtKey);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, patient.Id.ToString()),
            new Claim(ClaimTypes.Email, patient.Email),
            new Claim(ClaimTypes.Name, patient.Name),
            new Claim("PatientId", patient.Id.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }
}