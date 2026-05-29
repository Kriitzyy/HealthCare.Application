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
        // Add caregivers
        var caregivers = new List<Caregiver>
        {
            new Caregiver { Id = 1, Name = "Dr. Anna Andersson", Specialty = "Allmänmedicin", CreatedAt = DateTime.UtcNow },
            new Caregiver { Id = 2, Name = "Dr. Björn Berg", Specialty = "Kardiologi", CreatedAt = DateTime.UtcNow },
            new Caregiver { Id = 3, Name = "Dr. Cecilia Carlsson", Specialty = "Dermatologi", CreatedAt = DateTime.UtcNow }
        };
        context.Caregivers.AddRange(caregivers);
       
        // Add appointment slots
        var appointmentSlots = new List<AppointmentSlot>
        {
            new AppointmentSlot { Id = 1, CaregiverId = 1, StartTime = DateTime.UtcNow.AddDays(10), EndTime = DateTime.UtcNow.AddDays(10).AddHours(1), IsBooked = false, BookedByPatientId = null },
            new AppointmentSlot { Id = 2, CaregiverId = 1, StartTime = DateTime.UtcNow.AddDays(10).AddHours(2), EndTime = DateTime.UtcNow.AddDays(10).AddHours(3), IsBooked = false, BookedByPatientId = null },
            new AppointmentSlot { Id = 3, CaregiverId = 2, StartTime = DateTime.UtcNow.AddDays(11), EndTime = DateTime.UtcNow.AddDays(11).AddHours(1), IsBooked = false, BookedByPatientId = null },
            new AppointmentSlot { Id = 4, CaregiverId = 2, StartTime = DateTime.UtcNow.AddDays(11).AddHours(2), EndTime = DateTime.UtcNow.AddDays(11).AddHours(3), IsBooked = false, BookedByPatientId = null },
            new AppointmentSlot { Id = 5, CaregiverId = 3, StartTime = DateTime.UtcNow.AddDays(12), EndTime = DateTime.UtcNow.AddDays(12).AddHours(1), IsBooked = false, BookedByPatientId = null }
        };
        context.AppointmentSlots.AddRange(appointmentSlots);
       
        // Add test patients
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