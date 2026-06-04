using System.ComponentModel.DataAnnotations;
using DoctorAppointmentSystem.Models;

namespace DoctorAppointmentSystem.DTOs;

public record RegisterRequest(
    [Required, MaxLength(100)] string FullName,
    [Required, EmailAddress, MaxLength(150)] string Email,
    [Required, MinLength(6)] string Password,
    [MaxLength(20)] string? Phone
);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponse(
    string Token,
    int UserId,
    string FullName,
    string Email,
    UserRole Role,
    DateTime ExpiresAt
);
