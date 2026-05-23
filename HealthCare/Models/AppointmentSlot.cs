namespace HealthcareAPI.Models;

public class AppointmentSlot
{
    public int Id { get; set; }
    public int CaregiverId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsBooked { get; set; } = false;
    public int? BookedByPatientId { get; set; }
    
    public Caregiver Caregiver { get; set; } = null!;
    public Patient? BookedByPatient { get; set; }
}
