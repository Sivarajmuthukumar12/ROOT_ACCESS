using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentSystem.Models;

public class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public AppointmentMode Mode { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public DateTime AppointmentDate { get; set; }

    public TimeOnly SlotStart { get; set; }

    public TimeOnly SlotEnd { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>For Online appointments: meeting link or video call URL</summary>
    [MaxLength(500)]
    public string? MeetingLink { get; set; }

    /// <summary>For Offline appointments: clinic address or room number</summary>
    [MaxLength(300)]
    public string? ClinicLocation { get; set; }

    public decimal ConsultationFee { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public User Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
}
