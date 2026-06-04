using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentSystem.DTOs;

public record SpecialtyResponse(int Id, string Name, string? Description, int DoctorCount);

public record CreateSpecialtyRequest(
    [Required, MaxLength(100)] string Name,
    [MaxLength(500)] string? Description
);
