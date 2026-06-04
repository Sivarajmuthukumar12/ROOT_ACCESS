using DoctorAppointmentSystem.Data;
using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using DoctorAppointmentSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Controllers.UI;

[Route("Admin")]
public class AdminController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly IDoctorService _doctorService;
    private readonly AppDbContext _db;

    public AdminController(IAppointmentService appointmentService, IDoctorService doctorService, AppDbContext db)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
        _db = db;
    }

    private IActionResult? GuardAdmin()
    {
        if (HttpContext.Session.GetString("Role") != "Admin")
        {
            TempData["Error"] = "Access denied.";
            return RedirectToAction("Login", "Account");
        }
        return null;
    }

    private int AdminUserId => int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

    // GET /Admin/Dashboard
    [HttpGet("Dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var guard = GuardAdmin(); if (guard != null) return guard;

        var today = DateTime.UtcNow.Date;
        var summary = await _appointmentService.GetDailySummaryAsync(today);
        var recentAppointments = await _appointmentService.GetAllAsync(null, null, null);

        var vm = new AdminDashboardViewModel
        {
            TotalDoctors = await _db.Doctors.CountAsync(d => d.IsActive),
            TotalPatients = await _db.Users.CountAsync(u => u.Role == UserRole.Patient),
            TodayAppointments = summary.TotalAppointments,
            PendingAppointments = await _db.Appointments.CountAsync(a => a.Status == AppointmentStatus.Pending),
            TodayRevenue = summary.TotalRevenue,
            TodaySummary = summary,
            RecentAppointments = recentAppointments.Take(10)
        };

        return View("~/Views/Admin/Dashboard.cshtml", vm);
    }

    // GET /Admin/Appointments
    [HttpGet("Appointments")]
    public async Task<IActionResult> Appointments(DateTime? date, AppointmentMode? mode, AppointmentStatus? status)
    {
        var guard = GuardAdmin(); if (guard != null) return guard;

        var appointments = await _appointmentService.GetAllAsync(date, mode, status);
        return View("~/Views/Admin/Appointments.cshtml", new AdminAppointmentFilterViewModel
        {
            Appointments = appointments,
            FilterDate = date,
            FilterMode = mode,
            FilterStatus = status
        });
    }

    // POST /Admin/UpdateStatus
    [HttpPost("UpdateStatus")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, AppointmentStatus status)
    {
        var guard = GuardAdmin(); if (guard != null) return guard;

        var (_, error) = await _appointmentService.UpdateStatusAsync(
            id, AdminUserId, "Admin",
            new UpdateAppointmentStatusRequest(status, null));

        if (error != null) TempData["Error"] = error;
        else TempData["Success"] = $"Appointment status updated to {status}.";

        return RedirectToAction("Appointments");
    }

    // GET /Admin/Doctors
    [HttpGet("Doctors")]
    public async Task<IActionResult> Doctors()
    {
        var guard = GuardAdmin(); if (guard != null) return guard;

        var doctors = await _doctorService.GetAllAsync(null, null);
        return View("~/Views/Admin/Doctors.cshtml", doctors);
    }

    // GET /Admin/CreateDoctor
    [HttpGet("CreateDoctor")]
    public async Task<IActionResult> CreateDoctor()
    {
        var guard = GuardAdmin(); if (guard != null) return guard;

        var specialties = await _db.Specialties
            .Select(s => new SpecialtyResponse(s.Id, s.Name, s.Description, 0))
            .ToListAsync();

        return View("~/Views/Admin/CreateDoctor.cshtml", new CreateDoctorViewModel { Specialties = specialties });
    }

    // POST /Admin/CreateDoctor
    [HttpPost("CreateDoctor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDoctor(CreateDoctorViewModel model)
    {
        var guard = GuardAdmin(); if (guard != null) return guard;

        if (!ModelState.IsValid)
        {
            model.Specialties = await _db.Specialties
                .Select(s => new SpecialtyResponse(s.Id, s.Name, s.Description, 0))
                .ToListAsync();
            return View("~/Views/Admin/CreateDoctor.cshtml", model);
        }

        await _doctorService.CreateAsync(new CreateDoctorRequest(
            model.FullName, model.Email, model.Phone,
            model.SpecialtyId, model.Mode, model.Bio,
            model.ConsultationFee, model.ClinicLocation));

        TempData["Success"] = $"Dr. {model.FullName} added successfully.";
        return RedirectToAction("Doctors");
    }

    // POST /Admin/DeactivateDoctor/{id}
    [HttpPost("DeactivateDoctor/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateDoctor(int id)
    {
        var guard = GuardAdmin(); if (guard != null) return guard;

        await _doctorService.DeleteAsync(id);
        TempData["Success"] = "Doctor deactivated.";
        return RedirectToAction("Doctors");
    }

    // GET /Admin/Specialties
    [HttpGet("Specialties")]
    public async Task<IActionResult> Specialties()
    {
        var guard = GuardAdmin(); if (guard != null) return guard;

        var specialties = await _db.Specialties
            .Include(s => s.Doctors)
            .Select(s => new SpecialtyResponse(s.Id, s.Name, s.Description, s.Doctors.Count(d => d.IsActive)))
            .ToListAsync();

        return View("~/Views/Admin/Specialties.cshtml", specialties);
    }

    // POST /Admin/CreateSpecialty
    [HttpPost("CreateSpecialty")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSpecialty(string name, string? description)
    {
        var guard = GuardAdmin(); if (guard != null) return guard;

        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Specialty name is required.";
            return RedirectToAction("Specialties");
        }

        if (await _db.Specialties.AnyAsync(s => s.Name == name))
        {
            TempData["Error"] = "Specialty already exists.";
            return RedirectToAction("Specialties");
        }

        _db.Specialties.Add(new Specialty { Name = name, Description = description });
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Specialty '{name}' created.";
        return RedirectToAction("Specialties");
    }
}
