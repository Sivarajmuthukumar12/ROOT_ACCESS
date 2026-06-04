using DoctorAppointmentSystem.Data;
using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecialtiesController : ControllerBase
{
    private readonly AppDbContext _db;

    public SpecialtiesController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>Get all specialties with doctor count.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SpecialtyResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var specialties = await _db.Specialties
            .Include(s => s.Doctors)
            .Select(s => new SpecialtyResponse(
                s.Id,
                s.Name,
                s.Description,
                s.Doctors.Count(d => d.IsActive)
            ))
            .ToListAsync();

        return Ok(specialties);
    }

    /// <summary>Get a specialty by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SpecialtyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var specialty = await _db.Specialties
            .Include(s => s.Doctors)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (specialty == null) return NotFound();

        return Ok(new SpecialtyResponse(
            specialty.Id, specialty.Name, specialty.Description,
            specialty.Doctors.Count(d => d.IsActive)
        ));
    }

    /// <summary>Create a new specialty. (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SpecialtyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateSpecialtyRequest request)
    {
        if (await _db.Specialties.AnyAsync(s => s.Name == request.Name))
            return Conflict(new { message = "Specialty already exists." });

        var specialty = new Specialty { Name = request.Name, Description = request.Description };
        _db.Specialties.Add(specialty);
        await _db.SaveChangesAsync();

        var response = new SpecialtyResponse(specialty.Id, specialty.Name, specialty.Description, 0);
        return CreatedAtAction(nameof(GetById), new { id = specialty.Id }, response);
    }
}
