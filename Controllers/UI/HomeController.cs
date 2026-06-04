using DoctorAppointmentSystem.Data;
using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Controllers.UI;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    private readonly IDoctorService _doctorService;

    public HomeController(AppDbContext db, IDoctorService doctorService)
    {
        _db = db;
        _doctorService = doctorService;
    }

    public async Task<IActionResult> Index()
    {
        var specialties = await _db.Specialties
            .Include(s => s.Doctors)
            .Select(s => new SpecialtyResponse(s.Id, s.Name, s.Description, s.Doctors.Count(d => d.IsActive)))
            .ToListAsync();

        var featuredDoctors = await _doctorService.GetAllAsync(null, null);

        ViewBag.Specialties = specialties;
        ViewBag.FeaturedDoctors = featuredDoctors.Take(6);
        ViewBag.TotalDoctors = featuredDoctors.Count();
        ViewBag.TotalSpecialties = specialties.Count;

        return View("~/Views/Home/Index.cshtml");
    }
}
