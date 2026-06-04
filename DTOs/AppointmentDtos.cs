using System.ComponentModel.DataAnnotations;
using DoctorAppointmentSystem.Models;

namespace DoctorAppointmentSystem.DTOs;

public record BookAppointmentRequest(
    [Required] int DoctorId,
    [Required] AppointmentMode Mode,
    [Required] DateTime AppointmentDate,
    [Required] TimeOnly SlotStart,
    [MaxLength(1000)] string? Notes
);

public record UpdateAppointmentStatusRequest(
    [Required] AppointmentStatus Status,
    [MaxLength(1000)] string? Notes
);

public record AppointmentResponse(
    int Id,
    int PatientId,
    string PatientName,
    int DoctorId,
    string DoctorName,
    string SpecialtyName,
    AppointmentMode Mode,
    AppointmentStatus Status,
    DateTime AppointmentDate,
    TimeOnly SlotStart,
    TimeOnly SlotEnd,
    string? Notes,
    string? MeetingLink,
    string? ClinicLocation,
    decimal ConsultationFee,
    DateTime CreatedAt
);

public record DailySummaryResponse(
    DateTime Date,
    int TotalAppointments,
    int Completed,
    int Cancelled,
    int NoShow,
    decimal TotalRevenue,
    IEnumerable<ModeSummary> ByMode,
    IEnumerable<SpecialtySummary> BySpecialty
);

public record ModeSummary(
    AppointmentMode Mode,
    int Count,
    int Completed,
    decimal Revenue
);

public record SpecialtySummary(
    string Specialty,
    int Count,
    int Completed,
    decimal Revenue
);
