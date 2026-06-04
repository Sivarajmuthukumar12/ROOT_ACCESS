namespace DoctorAppointmentSystem.Models;

public enum AppointmentMode
{
    Online = 1,   // Teleconsultation
    Offline = 2   // In-clinic
}

public enum AppointmentStatus
{
    Pending = 1,
    Confirmed = 2,
    Completed = 3,
    Cancelled = 4,
    NoShow = 5
}

public enum UserRole
{
    Patient = 1,
    Doctor = 2,
    Admin = 3
}
