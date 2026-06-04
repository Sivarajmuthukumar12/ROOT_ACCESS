using System.ComponentModel.DataAnnotations;
using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Models;

namespace DoctorAppointmentSystem.ViewModels;

public class DoctorListViewModel
{
    public IEnumerable<DoctorResponse> Doctors { get; set; } = new List<DoctorResponse>();
    public IEnumerable<SpecialtyResponse> Specialties { get; set; } = new List<SpecialtyResponse>();
    public int? SelectedSpecialtyId { get; set; }
    public AppointmentMode? SelectedMode { get; set; }
    public string? SearchTerm { get; set; }
}

public class DoctorDetailViewModel
{
    public DoctorResponse Doctor { get; set; } = null!;
    public IEnumerable<AvailabilitySlot> Slots { get; set; } = new List<AvailabilitySlot>();
    public DateTime SelectedDate { get; set; } = DateTime.Today.AddDays(1);
}

public class CreateDoctorViewModel
{
    [Required, MaxLength(100), Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20), Display(Name = "Phone")]
    public string? Phone { get; set; }

    [Required, Display(Name = "Specialty")]
    public int SpecialtyId { get; set; }

    [Required, Display(Name = "Appointment Mode")]
    public AppointmentMode Mode { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }

    [Required, Range(0, 10000), Display(Name = "Consultation Fee ($)")]
    public decimal ConsultationFee { get; set; }

    [MaxLength(300), Display(Name = "Clinic Location")]
    public string? ClinicLocation { get; set; }

    public IEnumerable<SpecialtyResponse> Specialties { get; set; } = new List<SpecialtyResponse>();
}
