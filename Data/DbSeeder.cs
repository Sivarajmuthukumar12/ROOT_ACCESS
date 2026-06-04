using DoctorAppointmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Data;

/// <summary>
/// Seeds initial admin user and sample doctors on first run.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        // Seed admin user
        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            context.Users.Add(new User
            {
                FullName = "System Admin",
                Email = "admin@clinic.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                Phone = "0000000000"
            });
            await context.SaveChangesAsync();
        }

        // Seed sample doctors (Online and Offline, different doctors per mode)
        if (!await context.Doctors.AnyAsync())
        {
            var doctors = new List<Doctor>
            {
                // Online doctors
                new Doctor { FullName = "Dr. Sarah Online", Email = "sarah.online@clinic.com", SpecialtyId = 1, Mode = AppointmentMode.Online, ConsultationFee = 50m, Bio = "Teleconsultation specialist in General Practice." },
                new Doctor { FullName = "Dr. James Online", Email = "james.online@clinic.com", SpecialtyId = 2, Mode = AppointmentMode.Online, ConsultationFee = 80m, Bio = "Online cardiologist with 10 years experience." },
                new Doctor { FullName = "Dr. Priya Online", Email = "priya.online@clinic.com", SpecialtyId = 3, Mode = AppointmentMode.Online, ConsultationFee = 60m, Bio = "Dermatology teleconsultations." },

                // Offline doctors
                new Doctor { FullName = "Dr. Mark Offline", Email = "mark.offline@clinic.com", SpecialtyId = 1, Mode = AppointmentMode.Offline, ConsultationFee = 70m, Bio = "In-clinic General Practitioner.", ClinicLocation = "Room 101, Main Clinic" },
                new Doctor { FullName = "Dr. Lisa Offline", Email = "lisa.offline@clinic.com", SpecialtyId = 2, Mode = AppointmentMode.Offline, ConsultationFee = 100m, Bio = "In-clinic cardiologist.", ClinicLocation = "Room 205, Cardiology Wing" },
                new Doctor { FullName = "Dr. Ahmed Offline", Email = "ahmed.offline@clinic.com", SpecialtyId = 4, Mode = AppointmentMode.Offline, ConsultationFee = 90m, Bio = "Orthopedic surgeon.", ClinicLocation = "Room 310, Orthopedics" },
            };

            context.Doctors.AddRange(doctors);
            await context.SaveChangesAsync();

            // Seed availabilities (Mon-Fri, 9am-5pm, 30-min slots)
            var savedDoctors = await context.Doctors.ToListAsync();
            var availabilities = new List<DoctorAvailability>();
            var weekdays = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

            foreach (var doctor in savedDoctors)
            {
                foreach (var day in weekdays)
                {
                    availabilities.Add(new DoctorAvailability
                    {
                        DoctorId = doctor.Id,
                        DayOfWeek = day,
                        StartTime = new TimeOnly(9, 0),
                        EndTime = new TimeOnly(17, 0),
                        SlotDurationMinutes = 30
                    });
                }
            }

            context.DoctorAvailabilities.AddRange(availabilities);
            await context.SaveChangesAsync();
        }
    }
}
