using System.ComponentModel.DataAnnotations;

namespace HealthcareAPI.Models;

public class Patient
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    
    public ICollection<AppointmentSlot> BookedSlots { get; set; } = new List<AppointmentSlot>();
}