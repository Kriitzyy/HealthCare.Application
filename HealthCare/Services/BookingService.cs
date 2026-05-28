using Microsoft.EntityFrameworkCore;
using HealthCare;
using HealthcareAPI.DTOs;

namespace HealthcareAPI.Services;

public class BookingService
{
    private readonly ApplicationDbContext _context;

    public BookingService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. Lista tillgängliga tider för en vårdgivare
    public async Task<List<AvailableSlotResponse>> GetAvailableSlotsAsync(int caregiverId)
    {
        var slots = await _context.AppointmentSlots
            .Include(a => a.Caregiver)
            .Where(a => a.CaregiverId == caregiverId && !a.IsBooked)
            .Select(a => new AvailableSlotResponse
            {
                Id = a.Id,
                CaregiverId = a.CaregiverId,
                CaregiverName = a.Caregiver.Name,
                Specialty = a.Caregiver.Specialty,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                IsBooked = a.IsBooked
            })
            .ToListAsync();

        return slots;
    }

    // 2. Boka en tid
    public async Task<BookAppointmentResponse> BookAppointmentAsync(int patientId, int appointmentSlotId)
    {
        // Hitta tiden
        var slot = await _context.AppointmentSlots
            .Include(a => a.Caregiver)
            .FirstOrDefaultAsync(a => a.Id == appointmentSlotId);

        if (slot == null)
            throw new Exception("Appointment slot not found");

        if (slot.IsBooked)
            throw new Exception("This time is already booked");

        // Boka tiden
        slot.IsBooked = true;
        slot.BookedByPatientId = patientId;

        await _context.SaveChangesAsync();

        return new BookAppointmentResponse
        {
            AppointmentId = slot.Id,
            Message = "Appointment booked successfully",
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            CaregiverName = slot.Caregiver.Name
        };
    }

    // 3. Avboka en egen bokning
    public async Task CancelAppointmentAsync(int patientId, int appointmentSlotId)
    {
        // Hitta tiden
        var slot = await _context.AppointmentSlots
            .FirstOrDefaultAsync(a => a.Id == appointmentSlotId);

        if (slot == null)
            throw new Exception("Appointment slot not found");

        // Kontrollera att det är patientens egen bokning
        if (slot.BookedByPatientId != patientId)
            throw new UnauthorizedAccessException("You can only cancel your own bookings");

        // Avboka tiden
        slot.IsBooked = false;
        slot.BookedByPatientId = null;

        await _context.SaveChangesAsync();
    }
}