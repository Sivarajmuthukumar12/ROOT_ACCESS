using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentSystem.Models;

public class DoctorAvailability
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    /// <summary>0 = Sunday, 1 = Monday, ..., 6 = Saturday</summary>
    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    /// <summary>Slot duration in minutes (e.g. 30)</summary>
    public int SlotDurationMinutes { get; set; } = 30;

    public bool IsActive { get; set; } = true;

    // Navigation
    public Doctor Doctor { get; set; } = null!;
}
