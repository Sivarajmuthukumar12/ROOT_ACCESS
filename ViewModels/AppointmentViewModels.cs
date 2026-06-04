using System.ComponentModel.DataAnnotations;
using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Models;

namespace DoctorAppointmentSystem.ViewModels;

public class BookAppointmentViewModel
{
    public DoctorResponse Doctor { get; set; } = null!;
    public IEnumerable<AvailabilitySlot> Slots { get; set; } = new List<AvailabilitySlot>();

    [Required]
    public int DoctorId { get; set; }

    [Required, Display(Name = "Appointment Date")]
    public DateTime AppointmentDate { get; set; } = DateTime.Today.AddDays(1);

    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    public string? SelectedSlot { get; set; }
}

public class AdminAppointmentFilterViewModel
{
    public IEnumerable<AppointmentResponse> Appointments { get; set; } = new List<AppointmentResponse>();
    public DateTime? FilterDate { get; set; }
    public AppointmentMode? FilterMode { get; set; }
    public AppointmentStatus? FilterStatus { get; set; }
}

public class AdminDashboardViewModel
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public int TodayAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public decimal TodayRevenue { get; set; }
    public DailySummaryResponse? TodaySummary { get; set; }
    public IEnumerable<AppointmentResponse> RecentAppointments { get; set; } = new List<AppointmentResponse>();
}
