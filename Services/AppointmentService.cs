using DoctorAppointmentSystem.Data;
using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Services;

public interface IAppointmentService
{
    Task<(AppointmentResponse? Result, string? Error)> BookAsync(int patientId, BookAppointmentRequest request);
    Task<AppointmentResponse?> GetByIdAsync(int id, int userId, string role);
    Task<IEnumerable<AppointmentResponse>> GetPatientAppointmentsAsync(int patientId);
    Task<IEnumerable<AppointmentResponse>> GetDoctorAppointmentsAsync(int doctorId, DateTime? date);
    Task<IEnumerable<AppointmentResponse>> GetAllAsync(DateTime? date, AppointmentMode? mode, AppointmentStatus? status);
    Task<(AppointmentResponse? Result, string? Error)> UpdateStatusAsync(int id, int userId, string role, UpdateAppointmentStatusRequest request);
    Task<DailySummaryResponse> GetDailySummaryAsync(DateTime date);
}

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _db;

    public AppointmentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(AppointmentResponse? Result, string? Error)> BookAsync(int patientId, BookAppointmentRequest request)
    {
        // Validate doctor exists and is active
        var doctor = await _db.Doctors
            .Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.Id == request.DoctorId && d.IsActive);

        if (doctor == null)
            return (null, "Doctor not found or inactive.");

        // Strict rule: doctor mode must match requested mode
        if (doctor.Mode != request.Mode)
            return (null, $"This doctor only accepts {doctor.Mode} appointments. Online and Offline appointments must use different doctors.");

        // Validate appointment date is in the future
        if (request.AppointmentDate.Date < DateTime.UtcNow.Date)
            return (null, "Appointment date must be in the future.");

        // Check doctor availability for that day
        var dayOfWeek = request.AppointmentDate.DayOfWeek;
        var availability = await _db.DoctorAvailabilities
            .FirstOrDefaultAsync(a => a.DoctorId == request.DoctorId && a.DayOfWeek == dayOfWeek && a.IsActive);

        if (availability == null)
            return (null, "Doctor is not available on this day.");

        var slotEnd = request.SlotStart.AddMinutes(availability.SlotDurationMinutes);

        // Validate slot is within availability window
        if (request.SlotStart < availability.StartTime || slotEnd > availability.EndTime)
            return (null, "Requested slot is outside doctor's working hours.");

        // Check slot is not already booked
        var isSlotTaken = await _db.Appointments.AnyAsync(a =>
            a.DoctorId == request.DoctorId
            && a.AppointmentDate.Date == request.AppointmentDate.Date
            && a.SlotStart == request.SlotStart
            && a.Status != AppointmentStatus.Cancelled);

        if (isSlotTaken)
            return (null, "This time slot is already booked.");

        var appointment = new Appointment
        {
            PatientId = patientId,
            DoctorId = request.DoctorId,
            Mode = request.Mode,
            AppointmentDate = request.AppointmentDate.Date,
            SlotStart = request.SlotStart,
            SlotEnd = slotEnd,
            Notes = request.Notes,
            ConsultationFee = doctor.ConsultationFee,
            Status = AppointmentStatus.Confirmed,
            // Mode-specific details
            MeetingLink = request.Mode == AppointmentMode.Online
                ? $"https://meet.clinic.com/{Guid.NewGuid():N}"
                : null,
            ClinicLocation = request.Mode == AppointmentMode.Offline
                ? doctor.ClinicLocation
                : null
        };

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();

        await _db.Entry(appointment).Reference(a => a.Patient).LoadAsync();
        await _db.Entry(appointment).Reference(a => a.Doctor).LoadAsync();
        await _db.Entry(appointment.Doctor).Reference(d => d.Specialty).LoadAsync();

        return (MapToResponse(appointment), null);
    }

    public async Task<AppointmentResponse?> GetByIdAsync(int id, int userId, string role)
    {
        var appointment = await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null) return null;

        // Patients can only see their own appointments
        if (role == "Patient" && appointment.PatientId != userId)
            return null;

        return MapToResponse(appointment);
    }

    public async Task<IEnumerable<AppointmentResponse>> GetPatientAppointmentsAsync(int patientId)
    {
        return await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => MapToResponse(a))
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentResponse>> GetDoctorAppointmentsAsync(int doctorId, DateTime? date)
    {
        var query = _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
            .Where(a => a.DoctorId == doctorId);

        if (date.HasValue)
            query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);

        return await query
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.SlotStart)
            .Select(a => MapToResponse(a))
            .ToListAsync();
    }

    public async Task<IEnumerable<AppointmentResponse>> GetAllAsync(DateTime? date, AppointmentMode? mode, AppointmentStatus? status)
    {
        var query = _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
            .AsQueryable();

        if (date.HasValue)
            query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
        if (mode.HasValue)
            query = query.Where(a => a.Mode == mode.Value);
        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        return await query
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => MapToResponse(a))
            .ToListAsync();
    }

    public async Task<(AppointmentResponse? Result, string? Error)> UpdateStatusAsync(
        int id, int userId, string role, UpdateAppointmentStatusRequest request)
    {
        var appointment = await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
            return (null, "Appointment not found.");

        // Patients can only cancel their own appointments
        if (role == "Patient")
        {
            if (appointment.PatientId != userId)
                return (null, "Access denied.");
            if (request.Status != AppointmentStatus.Cancelled)
                return (null, "Patients can only cancel appointments.");
        }

        // Cannot change a completed or cancelled appointment
        if (appointment.Status == AppointmentStatus.Completed || appointment.Status == AppointmentStatus.Cancelled)
            return (null, $"Cannot update an appointment that is already {appointment.Status}.");

        appointment.Status = request.Status;
        if (request.Notes != null) appointment.Notes = request.Notes;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return (MapToResponse(appointment), null);
    }

    public async Task<DailySummaryResponse> GetDailySummaryAsync(DateTime date)
    {
        var appointments = await _db.Appointments
            .Include(a => a.Doctor).ThenInclude(d => d.Specialty)
            .Where(a => a.AppointmentDate.Date == date.Date)
            .ToListAsync();

        var completed = appointments.Where(a => a.Status == AppointmentStatus.Completed).ToList();

        var byMode = appointments
            .GroupBy(a => a.Mode)
            .Select(g => new ModeSummary(
                g.Key,
                g.Count(),
                g.Count(a => a.Status == AppointmentStatus.Completed),
                g.Where(a => a.Status == AppointmentStatus.Completed).Sum(a => a.ConsultationFee)
            ));

        var bySpecialty = appointments
            .GroupBy(a => a.Doctor.Specialty.Name)
            .Select(g => new SpecialtySummary(
                g.Key,
                g.Count(),
                g.Count(a => a.Status == AppointmentStatus.Completed),
                g.Where(a => a.Status == AppointmentStatus.Completed).Sum(a => a.ConsultationFee)
            ));

        return new DailySummaryResponse(
            date.Date,
            appointments.Count,
            completed.Count,
            appointments.Count(a => a.Status == AppointmentStatus.Cancelled),
            appointments.Count(a => a.Status == AppointmentStatus.NoShow),
            completed.Sum(a => a.ConsultationFee),
            byMode,
            bySpecialty
        );
    }

    private static AppointmentResponse MapToResponse(Appointment a) => new(
        a.Id,
        a.PatientId,
        a.Patient?.FullName ?? string.Empty,
        a.DoctorId,
        a.Doctor?.FullName ?? string.Empty,
        a.Doctor?.Specialty?.Name ?? string.Empty,
        a.Mode,
        a.Status,
        a.AppointmentDate,
        a.SlotStart,
        a.SlotEnd,
        a.Notes,
        a.MeetingLink,
        a.ClinicLocation,
        a.ConsultationFee,
        a.CreatedAt
    );
}
