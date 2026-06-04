using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentSystem.Controllers;

[ApiController]
[Route("api/doctors")]
public class DoctorsApiController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsApiController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    /// <summary>
    /// Browse doctors. Filter by specialty and/or mode (Online=1, Offline=2).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DoctorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? specialtyId,
        [FromQuery] AppointmentMode? mode)
    {
        var doctors = await _doctorService.GetAllAsync(specialtyId, mode);
        return Ok(doctors);
    }

    /// <summary>Get a doctor by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var doctor = await _doctorService.GetByIdAsync(id);
        return doctor == null ? NotFound() : Ok(doctor);
    }

    /// <summary>
    /// Get available time slots for a doctor on a specific date.
    /// </summary>
    [HttpGet("{id:int}/slots")]
    [ProducesResponseType(typeof(IEnumerable<AvailabilitySlot>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSlots(int id, [FromQuery] DateTime date)
    {
        var slots = await _doctorService.GetAvailableSlotsAsync(id, date);
        return Ok(slots);
    }

    /// <summary>Create a new doctor. (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDoctorRequest request)
    {
        var doctor = await _doctorService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
    }

    /// <summary>Update doctor details. (Admin only)</summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorRequest request)
    {
        var doctor = await _doctorService.UpdateAsync(id, request);
        return doctor == null ? NotFound() : Ok(doctor);
    }

    /// <summary>Deactivate a doctor. (Admin only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _doctorService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }

    /// <summary>Set or update doctor availability for a day. (Admin only)</summary>
    [HttpPost("{id:int}/availability")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetAvailability(int id, [FromBody] DoctorAvailabilityRequest request)
    {
        var doctor = await _doctorService.GetByIdAsync(id);
        if (doctor == null) return NotFound();

        var availability = await _doctorService.SetAvailabilityAsync(id, request);
        return Ok(new
        {
            doctorId = id,
            availability.DayOfWeek,
            availability.StartTime,
            availability.EndTime,
            availability.SlotDurationMinutes
        });
    }
}
