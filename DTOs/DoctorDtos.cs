using System.ComponentModel.DataAnnotations;
using DoctorAppointmentSystem.Models;

namespace DoctorAppointmentSystem.DTOs;

public record DoctorResponse(
    int Id,
    string FullName,
    string Email,
    string? Phone,
    int SpecialtyId,
    string SpecialtyName,
    AppointmentMode Mode,
    string? Bio,
    decimal ConsultationFee,
    string? ClinicLocation,
    bool IsActive
);

public record CreateDoctorRequest(
    [Required, MaxLength(100)] string FullName,
    [Required, EmailAddress, MaxLength(150)] string Email,
    [MaxLength(20)] string? Phone,
    [Required] int SpecialtyId,
    [Required] AppointmentMode Mode,
    [MaxLength(500)] string? Bio,
    [Required, Range(0, 10000)] decimal ConsultationFee,
    [MaxLength(300)] string? ClinicLocation
);

public record UpdateDoctorRequest(
    [MaxLength(100)] string? FullName,
    [MaxLength(20)] string? Phone,
    [MaxLength(500)] string? Bio,
    [Range(0, 10000)] decimal? ConsultationFee,
    [MaxLength(300)] string? ClinicLocation,
    bool? IsActive
);

public record AvailabilitySlot(
    DateTime Date,
    TimeOnly Start,
    TimeOnly End,
    bool IsBooked
);

public record DoctorAvailabilityRequest(
    [Required] DayOfWeek DayOfWeek,
    [Required] TimeOnly StartTime,
    [Required] TimeOnly EndTime,
    [Range(15, 120)] int SlotDurationMinutes = 30
);
