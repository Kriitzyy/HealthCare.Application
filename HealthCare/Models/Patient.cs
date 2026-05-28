using System.ComponentModel.DataAnnotations;

namespace HealthCare;

public class Patient
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    
    public ICollection<AppointmentSlot> BookedSlots { get; set; } = new List<AppointmentSlot>();
}