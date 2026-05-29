using Microsoft.EntityFrameworkCore;
using HealthCare;
using HealthcareAPI.DTOs;
using HealthcareAPI.Services;

namespace HealthcareAPI.Tests;

public class BookingServiceTests
{
    // ========== GET AVAILABLE SLOTS TESTS ==========

    [Fact]
    public async Task GetAvailableSlotsAsync_WithValidCaregiverId_ReturnsAvailableSlots()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
       
        var slots = await bookingService.GetAvailableSlotsAsync(1);
       
        Assert.NotNull(slots);
        Assert.All(slots, slot => Assert.False(slot.IsBooked));
    }
   
    [Fact]
    public async Task GetAvailableSlotsAsync_WithInvalidCaregiverId_ReturnsEmptyList()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
       
        var slots = await bookingService.GetAvailableSlotsAsync(999);
       
        Assert.NotNull(slots);
        Assert.Empty(slots);
    }
   
    [Fact]
    public async Task GetAvailableSlotsAsync_WithCaregiverId2_ReturnsCorrectCaregiver()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
       
        var slots = await bookingService.GetAvailableSlotsAsync(2);
       
        Assert.NotNull(slots);
        Assert.All(slots, slot => Assert.Equal(2, slot.CaregiverId));
    }

    // ========== BOOK APPOINTMENT TESTS ==========

    [Fact]
    public async Task BookAppointmentAsync_WithValidRequest_BooksSlot()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
        var patientId = 999;
       
        var availableSlot = await context.AppointmentSlots.FirstOrDefaultAsync(s => !s.IsBooked);
        Assert.NotNull(availableSlot);
       
        var result = await bookingService.BookAppointmentAsync(patientId, availableSlot.Id);
       
        Assert.NotNull(result);
        Assert.Equal("Appointment booked successfully", result.Message);
        Assert.Equal(availableSlot.Id, result.AppointmentId);
       
        var updatedSlot = await context.AppointmentSlots.FindAsync(availableSlot.Id);
        Assert.True(updatedSlot!.IsBooked);
        Assert.Equal(patientId, updatedSlot.BookedByPatientId);
    }
   
    [Fact]
    public async Task BookAppointmentAsync_WithAlreadyBookedSlot_ThrowsException()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
        var patientId = 999;
       
        var slot = await context.AppointmentSlots.FirstOrDefaultAsync(s => !s.IsBooked);
        Assert.NotNull(slot);
       
        await bookingService.BookAppointmentAsync(patientId, slot.Id);
       
        var exception = await Assert.ThrowsAsync<Exception>(
            () => bookingService.BookAppointmentAsync(patientId, slot.Id)
        );
       
        Assert.Equal("This time is already booked", exception.Message);
    }
   
    [Fact]
    public async Task BookAppointmentAsync_WithInvalidSlotId_ThrowsException()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
        var patientId = 999;
       
        var exception = await Assert.ThrowsAsync<Exception>(
            () => bookingService.BookAppointmentAsync(patientId, 999)
        );
       
        Assert.Equal("Appointment slot not found", exception.Message);
    }
   
    [Fact]
    public async Task BookAppointmentAsync_WithDifferentPatients_BooksCorrectly()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
        var patientId1 = 999;
        var patientId2 = 888;
       
        var secondPatient = new Patient
        {
            Id = 888,
            Name = "Second User",
            Email = "second@test.com",
            PasswordHash = "hash123"
        };
        context.Patients.Add(secondPatient);
        await context.SaveChangesAsync();
       
        var slot1 = await context.AppointmentSlots.FirstOrDefaultAsync(s => !s.IsBooked);
        var slot2 = await context.AppointmentSlots.Skip(1).FirstOrDefaultAsync(s => !s.IsBooked);
        Assert.NotNull(slot1);
        Assert.NotNull(slot2);
       
        var result1 = await bookingService.BookAppointmentAsync(patientId1, slot1.Id);
        var result2 = await bookingService.BookAppointmentAsync(patientId2, slot2.Id);
       
        Assert.NotNull(result1);
        Assert.NotNull(result2);
       
        var updatedSlot1 = await context.AppointmentSlots.FindAsync(slot1.Id);
        var updatedSlot2 = await context.AppointmentSlots.FindAsync(slot2.Id);
       
        Assert.Equal(patientId1, updatedSlot1!.BookedByPatientId);
        Assert.Equal(patientId2, updatedSlot2!.BookedByPatientId);
    }

    // ========== CANCEL APPOINTMENT TESTS ==========

    [Fact]
    public async Task CancelAppointmentAsync_WithOwnBooking_CancelsSuccessfully()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
        var patientId = 999;
       
        var slot = await context.AppointmentSlots.FirstOrDefaultAsync(s => !s.IsBooked);
        Assert.NotNull(slot);
       
        await bookingService.BookAppointmentAsync(patientId, slot.Id);
       
        await bookingService.CancelAppointmentAsync(patientId, slot.Id);
       
        var cancelledSlot = await context.AppointmentSlots.FindAsync(slot.Id);
        Assert.False(cancelledSlot!.IsBooked);
        Assert.Null(cancelledSlot.BookedByPatientId);
    }
   
    [Fact]
    public async Task CancelAppointmentAsync_WithSomeoneElsesBooking_ThrowsUnauthorized()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
        var patientId1 = 999;
        var patientId2 = 888;
       
        var secondPatient = new Patient
        {
            Id = 888,
            Name = "Second User",
            Email = "second@test.com",
            PasswordHash = "hash123"
        };
        context.Patients.Add(secondPatient);
        await context.SaveChangesAsync();
       
        var slot = await context.AppointmentSlots.FirstOrDefaultAsync(s => !s.IsBooked);
        Assert.NotNull(slot);
       
        await bookingService.BookAppointmentAsync(patientId1, slot.Id);
       
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => bookingService.CancelAppointmentAsync(patientId2, slot.Id)
        );
       
        Assert.Equal("You can only cancel your own bookings", exception.Message);
       
        var stillBookedSlot = await context.AppointmentSlots.FindAsync(slot.Id);
        Assert.True(stillBookedSlot!.IsBooked);
        Assert.Equal(patientId1, stillBookedSlot.BookedByPatientId);
    }
   
    [Fact]
    public async Task CancelAppointmentAsync_WithInvalidSlotId_ThrowsException()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
        var patientId = 999;
       
        var exception = await Assert.ThrowsAsync<Exception>(
            () => bookingService.CancelAppointmentAsync(patientId, 999)
        );
       
        Assert.Equal("Appointment slot not found", exception.Message);
    }
   
    [Fact]
    public async Task CancelAppointmentAsync_WithAlreadyCancelledSlot_ThrowsUnauthorized()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
        var patientId = 999;
       
        var slot = await context.AppointmentSlots.FirstOrDefaultAsync(s => !s.IsBooked);
        Assert.NotNull(slot);
       
        await bookingService.BookAppointmentAsync(patientId, slot.Id);
        await bookingService.CancelAppointmentAsync(patientId, slot.Id);
       
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => bookingService.CancelAppointmentAsync(patientId, slot.Id)
        );
       
        Assert.Equal("You can only cancel your own bookings", exception.Message);
    }

    // ========== INTEGRATION TESTS ==========

    [Fact]
    public async Task FullBookingFlow_BookAndCancel_WorksCorrectly()
    {
        var context = TestDatabaseHelper.GetInMemoryDbContext();
        var bookingService = new BookingService(context);
        var patientId = 999;
        var caregiverId = 1;
       
        var availableSlotsBefore = await bookingService.GetAvailableSlotsAsync(caregiverId);
       
        // Om inga slots finns för caregiver 1, prova caregiver 2
        if (availableSlotsBefore.Count == 0)
        {
            caregiverId = 2;
            availableSlotsBefore = await bookingService.GetAvailableSlotsAsync(caregiverId);
        }
        
        // Kontrollera att det finns minst en tid
        Assert.NotEmpty(availableSlotsBefore);
        
        var availableCountBefore = availableSlotsBefore.Count;
        var slotToBook = availableSlotsBefore.First();
       
        await bookingService.BookAppointmentAsync(patientId, slotToBook.Id);
       
        var availableSlotsAfter = await bookingService.GetAvailableSlotsAsync(caregiverId);
        Assert.Equal(availableCountBefore - 1, availableSlotsAfter.Count);
        Assert.DoesNotContain(availableSlotsAfter, s => s.Id == slotToBook.Id);
       
        await bookingService.CancelAppointmentAsync(patientId, slotToBook.Id);
       
        var availableSlotsAfterCancel = await bookingService.GetAvailableSlotsAsync(caregiverId);
        Assert.Equal(availableCountBefore, availableSlotsAfterCancel.Count);
        Assert.Contains(availableSlotsAfterCancel, s => s.Id == slotToBook.Id);
    }
}