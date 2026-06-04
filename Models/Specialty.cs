using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentSystem.Models;

public class Specialty
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Navigation
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
