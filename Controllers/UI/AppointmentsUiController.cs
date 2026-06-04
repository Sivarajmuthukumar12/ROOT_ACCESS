using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Models;
using DoctorAppointmentSystem.Services;
using DoctorAppointmentSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentSystem.Controllers.UI;

[Route("Appointments")]
public class AppointmentsController : Controller
{
    private readonly IAppointmentService _appointmentService;
    private readonly IDoctorService _doctorService;

    public AppointmentsController(IAppointmentService appointmentService, IDoctorService doctorService)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
    }

    private int? CurrentUserId
    {
        get
        {
            var id = HttpContext.Session.GetString("UserId");
            return id != null ? int.Parse(id) : null;
        }
    }

    private string? CurrentRole => HttpContext.Session.GetString("Role");

    private IActionResult RequireLogin()
    {
        TempData["Error"] = "Please login to continue.";
        return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });
    }

    // GET /Appointments/Book/{doctorId}?date=...
    [HttpGet("Book/{doctorId:int}")]
    public async Task<IActionResult> Book(int doctorId, DateTime? date)
    {
        if (CurrentRole == null) return RequireLogin();

        var doctor = await _doctorService.GetByIdAsync(doctorId);
        if (doctor == null) return NotFound();

        var selectedDate = date ?? DateTime.Today.AddDays(1);
        var slots = await _doctorService.GetAvailableSlotsAsync(doctorId, selectedDate);

        return View("~/Views/Appointments/Book.cshtml", new BookAppointmentViewModel
        {
            Doctor = doctor,
            DoctorId = doctorId,
            Slots = slots,
            AppointmentDate = selectedDate
        });
    }

    // POST /Appointments/Book
    [HttpPost("Book")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BookConfirm(BookAppointmentViewModel model)
    {
        if (CurrentRole == null) return RequireLogin();
        if (CurrentUserId == null) return RequireLogin();

        if (string.IsNullOrEmpty(model.SelectedSlot))
        {
            TempData["Error"] = "Please select a time slot.";
            return RedirectToAction("Book", new { doctorId = model.DoctorId, date = model.AppointmentDate });
        }

        if (!TimeOnly.TryParse(model.SelectedSlot, out var slotStart))
        {
            TempData["Error"] = "Invalid time slot.";
            return RedirectToAction("Book", new { doctorId = model.DoctorId, date = model.AppointmentDate });
        }

        var doctor = await _doctorService.GetByIdAsync(model.DoctorId);
        if (doctor == null) return NotFound();

        var request = new BookAppointmentRequest(
            model.DoctorId,
            doctor.Mode,
            model.AppointmentDate,
            slotStart,
            model.Notes
        );

        var (result, error) = await _appointmentService.BookAsync(CurrentUserId.Value, request);

        if (error != null)
        {
            TempData["Error"] = error;
            return RedirectToAction("Book", new { doctorId = model.DoctorId, date = model.AppointmentDate });
        }

        TempData["Success"] = "Appointment booked successfully!";
        return RedirectToAction("Confirmation", new { id = result!.Id });
    }

    // GET /Appointments/Confirmation/{id}
    [HttpGet("Confirmation/{id:int}")]
    public async Task<IActionResult> Confirmation(int id)
    {
        if (CurrentRole == null) return RequireLogin();
        if (CurrentUserId == null) return RequireLogin();

        var appointment = await _appointmentService.GetByIdAsync(id, CurrentUserId.Value, CurrentRole);
        if (appointment == null) return NotFound();

        return View("~/Views/Appointments/Confirmation.cshtml", appointment);
    }

    // GET /Appointments/My
    [HttpGet("My")]
    public async Task<IActionResult> MyAppointments()
    {
        if (CurrentRole == null) return RequireLogin();
        if (CurrentUserId == null) return RequireLogin();

        var appointments = await _appointmentService.GetPatientAppointmentsAsync(CurrentUserId.Value);
        return View("~/Views/Appointments/MyAppointments.cshtml", appointments);
    }

    // POST /Appointments/Cancel/{id}
    [HttpPost("Cancel/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        if (CurrentRole == null) return RequireLogin();
        if (CurrentUserId == null) return RequireLogin();

        var (_, error) = await _appointmentService.UpdateStatusAsync(
            id, CurrentUserId.Value, CurrentRole,
            new UpdateAppointmentStatusRequest(AppointmentStatus.Cancelled, null));

        if (error != null)
            TempData["Error"] = error;
        else
            TempData["Success"] = "Appointment cancelled.";

        return RedirectToAction("MyAppointments");
    }
}
