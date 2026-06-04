using DoctorAppointmentSystem.Data;
using DoctorAppointmentSystem.DTOs;
using DoctorAppointmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Services;

public interface IDoctorService
{
    Task<IEnumerable<DoctorResponse>> GetAllAsync(int? specialtyId, AppointmentMode? mode);
    Task<DoctorResponse?> GetByIdAsync(int id);
    Task<DoctorResponse> CreateAsync(CreateDoctorRequest request);
    Task<DoctorResponse?> UpdateAsync(int id, UpdateDoctorRequest request);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<AvailabilitySlot>> GetAvailableSlotsAsync(int doctorId, DateTime date);
    Task<DoctorAvailability> SetAvailabilityAsync(int doctorId, DoctorAvailabilityRequest request);
}

public class DoctorService : IDoctorService
{
    private readonly AppDbContext _db;

    public DoctorService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<DoctorResponse>> GetAllAsync(int? specialtyId, AppointmentMode? mode)
    {
        var query = _db.Doctors
            .Include(d => d.Specialty)
            .Where(d => d.IsActive);

        if (specialtyId.HasValue)
            query = query.Where(d => d.SpecialtyId == specialtyId.Value);

        if (mode.HasValue)
            query = query.Where(d => d.Mode == mode.Value);

        return await query.Select(d => MapToResponse(d)).ToListAsync();
    }

    public async Task<DoctorResponse?> GetByIdAsync(int id)
    {
        var doctor = await _db.Doctors
            .Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

        return doctor == null ? null : MapToResponse(doctor);
    }

    public async Task<DoctorResponse> CreateAsync(CreateDoctorRequest request)
    {
        var doctor = new Doctor
        {
            FullName = request.FullName,
            Email = request.Email.ToLower(),
            Phone = request.Phone,
            SpecialtyId = request.SpecialtyId,
            Mode = request.Mode,
            Bio = request.Bio,
            ConsultationFee = request.ConsultationFee,
            ClinicLocation = request.ClinicLocation
        };

        _db.Doctors.Add(doctor);
        await _db.SaveChangesAsync();

        await _db.Entry(doctor).Reference(d => d.Specialty).LoadAsync();
        return MapToResponse(doctor);
    }

    public async Task<DoctorResponse?> UpdateAsync(int id, UpdateDoctorRequest request)
    {
        var doctor = await _db.Doctors
            .Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doctor == null) return null;

        if (request.FullName != null) doctor.FullName = request.FullName;
        if (request.Phone != null) doctor.Phone = request.Phone;
        if (request.Bio != null) doctor.Bio = request.Bio;
        if (request.ConsultationFee.HasValue) doctor.ConsultationFee = request.ConsultationFee.Value;
        if (request.ClinicLocation != null) doctor.ClinicLocation = request.ClinicLocation;
        if (request.IsActive.HasValue) doctor.IsActive = request.IsActive.Value;

        await _db.SaveChangesAsync();
        return MapToResponse(doctor);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var doctor = await _db.Doctors.FindAsync(id);
        if (doctor == null) return false;

        // Soft delete
        doctor.IsActive = false;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AvailabilitySlot>> GetAvailableSlotsAsync(int doctorId, DateTime date)
    {
        var dayOfWeek = date.DayOfWeek;

        var availability = await _db.DoctorAvailabilities
            .FirstOrDefaultAsync(a => a.DoctorId == doctorId && a.DayOfWeek == dayOfWeek && a.IsActive);

        if (availability == null) return Enumerable.Empty<AvailabilitySlot>();

        // Get already booked slots for that day
        var bookedSlots = await _db.Appointments
            .Where(a => a.DoctorId == doctorId
                && a.AppointmentDate.Date == date.Date
                && a.Status != AppointmentStatus.Cancelled)
            .Select(a => a.SlotStart)
            .ToListAsync();

        var slots = new List<AvailabilitySlot>();
        var current = availability.StartTime;

        while (current.AddMinutes(availability.SlotDurationMinutes) <= availability.EndTime)
        {
            var slotEnd = current.AddMinutes(availability.SlotDurationMinutes);
            slots.Add(new AvailabilitySlot(
                date.Date,
                current,
                slotEnd,
                bookedSlots.Contains(current)
            ));
            current = slotEnd;
        }

        return slots;
    }

    public async Task<DoctorAvailability> SetAvailabilityAsync(int doctorId, DoctorAvailabilityRequest request)
    {
        var existing = await _db.DoctorAvailabilities
            .FirstOrDefaultAsync(a => a.DoctorId == doctorId && a.DayOfWeek == request.DayOfWeek);

        if (existing != null)
        {
            existing.StartTime = request.StartTime;
            existing.EndTime = request.EndTime;
            existing.SlotDurationMinutes = request.SlotDurationMinutes;
            existing.IsActive = true;
        }
        else
        {
            existing = new DoctorAvailability
            {
                DoctorId = doctorId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                SlotDurationMinutes = request.SlotDurationMinutes
            };
            _db.DoctorAvailabilities.Add(existing);
        }

        await _db.SaveChangesAsync();
        return existing;
    }

    private static DoctorResponse MapToResponse(Doctor d) => new(
        d.Id, d.FullName, d.Email, d.Phone,
        d.SpecialtyId, d.Specialty?.Name ?? string.Empty,
        d.Mode, d.Bio, d.ConsultationFee, d.ClinicLocation, d.IsActive
    );
}
