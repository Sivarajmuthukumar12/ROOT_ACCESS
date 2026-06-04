using DoctorAppointmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorAppointmentSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Specialty> Specialties => Set<Specialty>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<DoctorAvailability> DoctorAvailabilities => Set<DoctorAvailability>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique email constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Doctor>()
            .HasIndex(d => d.Email)
            .IsUnique();

        // Specialty name unique
        modelBuilder.Entity<Specialty>()
            .HasIndex(s => s.Name)
            .IsUnique();

        // Appointment -> Patient
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(u => u.PatientAppointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Appointment -> Doctor
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Doctor -> Specialty
        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.Specialty)
            .WithMany(s => s.Doctors)
            .HasForeignKey(d => d.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);

        // DoctorAvailability -> Doctor
        modelBuilder.Entity<DoctorAvailability>()
            .HasOne(da => da.Doctor)
            .WithMany(d => d.Availabilities)
            .HasForeignKey(da => da.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Decimal precision for fees
        modelBuilder.Entity<Appointment>()
            .Property(a => a.ConsultationFee)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Doctor>()
            .Property(d => d.ConsultationFee)
            .HasColumnType("decimal(10,2)");

        // Seed specialties
        modelBuilder.Entity<Specialty>().HasData(
            new Specialty { Id = 1, Name = "General Practice", Description = "General medical consultations" },
            new Specialty { Id = 2, Name = "Cardiology", Description = "Heart and cardiovascular system" },
            new Specialty { Id = 3, Name = "Dermatology", Description = "Skin, hair, and nails" },
            new Specialty { Id = 4, Name = "Orthopedics", Description = "Bones, joints, and muscles" },
            new Specialty { Id = 5, Name = "Pediatrics", Description = "Medical care for children" }
        );
    }
}
