namespace HealthcareAPI.Models;

public class Caregiver
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Specialty { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<AppointmentSlot> AppointmentSlots { get; set; } = new List<AppointmentSlot>();
}