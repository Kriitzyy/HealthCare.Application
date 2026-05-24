using Microsoft.EntityFrameworkCore;
using HealthcareAPI.Models;

namespace HealthCare;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Caregiver> Caregivers { get; set; }
    public DbSet<AppointmentSlot> AppointmentSlots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Caregiver>().HasData(
            new Caregiver
            {
                Id = 1,
                Name = "Dr. Anna Andersson",
                Specialty = "Allmänmedicin",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Caregiver
            {
                Id = 2,
                Name = "Dr. Björn Berg",
                Specialty = "Kardiologi",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Caregiver
            {
                Id = 3,
                Name = "Dr. Cecilia Carlsson",
                Specialty = "Dermatologi",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<AppointmentSlot>().HasData(
            new AppointmentSlot
            {
                Id = 1,
                CaregiverId = 1,
                StartTime = new DateTime(2026, 6, 1, 9, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc),
                IsBooked = false
            },
            new AppointmentSlot
            {
                Id = 2,
                CaregiverId = 1,
                StartTime = new DateTime(2026, 6, 1, 11, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc),
                IsBooked = false
            },
            new AppointmentSlot
            {
                Id = 3,
                CaregiverId = 2,
                StartTime = new DateTime(2026, 6, 2, 9, 0, 0, DateTimeKind.Utc),
                EndTime = new DateTime(2026, 6, 2, 10, 0, 0, DateTimeKind.Utc),
                IsBooked = false
            }
        );
    }
}