using System.Security.Claims;
using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentSystem.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize]
public class AppointmentsApiController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsApiController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)!);

    private string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role)!;

    /// <summary>
    /// Book an appointment. Patients book for themselves.
    /// Strict rule: doctor mode must match the requested mode.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Patient,Admin")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Book([FromBody] BookAppointmentRequest request)
    {
        var (result, error) = await _appointmentService.BookAsync(CurrentUserId, request);
        if (error != null) return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    /// <summary>Get appointment by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id, CurrentUserId, CurrentUserRole);
        return appointment == null ? NotFound() : Ok(appointment);
    }

    /// <summary>Get all appointments for the current patient.</summary>
    [HttpGet("my")]
    [Authorize(Roles = "Patient")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyAppointments()
    {
        var appointments = await _appointmentService.GetPatientAppointmentsAsync(CurrentUserId);
        return Ok(appointments);
    }

    /// <summary>Get appointments for a specific doctor. (Admin only)</summary>
    [HttpGet("doctor/{doctorId:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDoctorAppointments(int doctorId, [FromQuery] DateTime? date)
    {
        var appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId, date);
        return Ok(appointments);
    }

    /// <summary>
    /// Get all appointments with optional filters. (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<AppointmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime? date,
        [FromQuery] AppointmentMode? mode,
        [FromQuery] AppointmentStatus? status)
    {
        var appointments = await _appointmentService.GetAllAsync(date, mode, status);
        return Ok(appointments);
    }

    /// <summary>
    /// Update appointment status.
    /// Patients can only cancel. Doctors/Admins can mark completed, no-show, etc.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusRequest request)
    {
        var (result, error) = await _appointmentService.UpdateStatusAsync(id, CurrentUserId, CurrentUserRole, request);

        if (error != null)
        {
            if (error == "Appointment not found.") return NotFound(new { message = error });
            return BadRequest(new { message = error });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get daily summary report by mode and specialty. (Admin only)
    /// </summary>
    [HttpGet("summary/daily")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DailySummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDailySummary([FromQuery] DateTime? date)
    {
        var summary = await _appointmentService.GetDailySummaryAsync(date ?? DateTime.UtcNow);
        return Ok(summary);
    }
}
