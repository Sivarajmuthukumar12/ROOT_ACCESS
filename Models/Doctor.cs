using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentSystem.Models;

public class Doctor
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    public int SpecialtyId { get; set; }

    /// <summary>
    /// Strict rule: a doctor can only serve one mode (Online OR Offline), not both.
    /// </summary>
    public AppointmentMode Mode { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }

    public decimal ConsultationFee { get; set; }

    /// <summary>For Offline doctors: default clinic location</summary>
    [MaxLength(300)]
    public string? ClinicLocation { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Specialty Specialty { get; set; } = null!;
    public ICollection<DoctorAvailability> Availabilities { get; set; } = new List<DoctorAvailability>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
