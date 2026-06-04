using System.ComponentModel.DataAnnotations;

namespace DoctorAppointmentSystem.ViewModels;

public class LoginViewModel
{
    [Required, EmailAddress, Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required, MaxLength(100), Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150), Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare("Password"), Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MaxLength(20), Display(Name = "Phone Number")]
    public string? Phone { get; set; }
}
