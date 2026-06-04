using DoctorAppointmentSystem.Data;
using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using DoctorAppointmentSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Controllers.UI;

[Route("Doctors")]
public class DoctorsController : Controller
{
    private readonly IDoctorService _doctorService;
    private readonly AppDbContext _db;

    public DoctorsController(IDoctorService doctorService, AppDbContext db)
    {
        _doctorService = doctorService;
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int? specialtyId, AppointmentMode? mode, string? search)
    {
        var doctors = await _doctorService.GetAllAsync(specialtyId, mode);

        if (!string.IsNullOrWhiteSpace(search))
            doctors = doctors.Where(d =>
                d.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                d.SpecialtyName.Contains(search, StringComparison.OrdinalIgnoreCase));

        var specialties = await _db.Specialties
            .Select(s => new SpecialtyResponse(s.Id, s.Name, s.Description, s.Doctors.Count(d => d.IsActive)))
            .ToListAsync();

        return View("~/Views/Doctors/Index.cshtml", new DoctorListViewModel
        {
            Doctors = doctors,
            Specialties = specialties,
            SelectedSpecialtyId = specialtyId,
            SelectedMode = mode,
            SearchTerm = search
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Detail(int id, DateTime? date)
    {
        var doctor = await _doctorService.GetByIdAsync(id);
        if (doctor == null) return NotFound();

        var selectedDate = date ?? DateTime.Today.AddDays(1);
        var slots = await _doctorService.GetAvailableSlotsAsync(id, selectedDate);

        return View("~/Views/Doctors/Detail.cshtml", new DoctorDetailViewModel
        {
            Doctor = doctor,
            Slots = slots,
            SelectedDate = selectedDate
        });
    }
}
