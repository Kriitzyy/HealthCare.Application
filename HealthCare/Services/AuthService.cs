using Microsoft.EntityFrameworkCore;
using HealthCare;
using HealthcareAPI.Models;
using HealthcareAPI.DTOs;

namespace HealthcareAPI.Services;

public class AuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
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
}