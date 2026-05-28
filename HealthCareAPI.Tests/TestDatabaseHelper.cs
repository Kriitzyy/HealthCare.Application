using Microsoft.EntityFrameworkCore;
using HealthCare;

namespace HealthcareAPI.Tests;

public static class TestDatabaseHelper
{
    public static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        
        SeedTestData(context);
        
        return context;
    }

    private static void SeedTestData(ApplicationDbContext context)
    {
        var existingPatient = new Patient
        {
            Id = 999,
            Name = "Existing User",
            Email = "existing@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!")
        };
        context.Patients.Add(existingPatient);
        
        context.SaveChanges();
    }
}
