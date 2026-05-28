namespace HealthcareAPI.DTOs;

public class AvailableSlotResponse
{
    public int Id { get; set; }
    public int CaregiverId { get; set; }
    public string CaregiverName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; }
}

public class BookAppointmentRequest
{
    public int AppointmentSlotId { get; set; }
}

public class BookAppointmentResponse
{
    public int AppointmentId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string CaregiverName { get; set; } = string.Empty;
}