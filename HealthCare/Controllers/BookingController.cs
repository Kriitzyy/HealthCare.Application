using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthcareAPI.DTOs;
using HealthcareAPI.Services;

namespace HealthcareAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]  
public class BookingController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // GET: api/booking/available?caregiverId=1
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableSlots([FromQuery] int caregiverId)
    {
        try
        {
            var slots = await _bookingService.GetAvailableSlotsAsync(caregiverId);
            return Ok(slots);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST: api/booking/book
    [HttpPost("book")]
    public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
    {
        try
        {
            // Hämta patientId från JWT-token
            var patientIdClaim = User.FindFirst("PatientId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (patientIdClaim == null)
                return Unauthorized(new { error = "Patient ID not found in token" });

            var patientId = int.Parse(patientIdClaim.Value);

            var result = await _bookingService.BookAppointmentAsync(patientId, request.AppointmentSlotId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE: api/booking/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        try
        {
            var patientIdClaim = User.FindFirst("PatientId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (patientIdClaim == null)
                return Unauthorized(new { error = "Patient ID not found in token" });

            var patientId = int.Parse(patientIdClaim.Value);

            await _bookingService.CancelAppointmentAsync(patientId, id);
            return Ok(new { message = "Appointment cancelled successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}