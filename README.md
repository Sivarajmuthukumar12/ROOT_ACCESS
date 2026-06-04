# ROOT_ACCESS
HACKATHON@HCL
# Doctor Appointment Booking System

## Project Overview

The Doctor Appointment Booking System is a web-based healthcare management application developed using **ASP.NET Core** and **Razor Pages**. The application allows patients to register, log in, browse available doctors, and book appointments online. Administrators can manage doctors, appointments, and patient records through a centralized dashboard.

The primary goal of this application is to simplify the appointment scheduling process, reduce manual effort, and provide a seamless experience for both patients and healthcare administrators.

---

# Technologies Used

## Backend

* ASP.NET Core
* C#
* Entity Framework Core
* LINQ
* Dependency Injection
* RESTful Architecture Principles

## Frontend

* Razor Pages
* HTML5
* CSS3
* Bootstrap
* JavaScript

## Database

* SQL Server
* Entity Framework Core Migrations

## Authentication

* Cookie Authentication
* Authorization Policies

---

# Features

## Patient Features

* User Registration
* User Login & Logout
* View Available Doctors
* Book Appointments
* View Appointment History
* Cancel Appointments

## Doctor Features

* View Scheduled Appointments
* Manage Availability
* View Patient Information

## Admin Features

* Add New Doctors
* Update Doctor Information
* Delete Doctors
* View All Appointments
* Manage Users
* Monitor System Activity

---

# Project Architecture

The application follows a layered architecture:

```text
Presentation Layer (Razor Pages)
            ↓
Controller Layer
            ↓
Service Layer
            ↓
Repository/Data Access Layer
            ↓
SQL Server Database
```

### Layers Description

#### 1. Presentation Layer

Responsible for displaying data to users using Razor Views and handling user interactions.

#### 2. Controller Layer

Receives HTTP requests, validates inputs, and communicates with the Service Layer.

#### 3. Service Layer

Contains business logic and processing rules.

#### 4. Data Access Layer

Handles database operations through Entity Framework Core.

#### 5. Database Layer

Stores application data such as users, doctors, and appointments.

---

# Database Entities

## User

| Column       | Type   |
| ------------ | ------ |
| UserId       | int    |
| Name         | string |
| Email        | string |
| PasswordHash | string |
| Role         | string |

---

## Doctor

| Column         | Type   |
| -------------- | ------ |
| DoctorId       | int    |
| DoctorName     | string |
| Specialization | string |
| Experience     | int    |
| Availability   | bool   |

---

## Appointment

| Column          | Type     |
| --------------- | -------- |
| AppointmentId   | int      |
| UserId          | int      |
| DoctorId        | int      |
| AppointmentDate | datetime |
| Status          | string   |

---

Doctor Appointment System
A full‑stack ASP.NET Core MVC / Razor Pages application that enables patients to browse specialties and doctors, book online/offline appointments, and allows doctors to manage their schedules. Built with .NET 8, Entity Framework Core (Code‑First), SQL Server, and ASP.NET Core Identity (custom user model). The frontend is C# Razor Views – no separate JavaScript framework.

✨ Features
Patient

Browse specialties & doctors (filter by mode: Online / Offline)

View available time slots (next 7 days, 30‑min increments)

Book appointments (online → video link, offline → clinic address)

Cancel upcoming appointments

View appointment history (past appointments)

Doctor

Secure login (credentials provided by Admin)

View only their own appointments (upcoming & past)

Mark appointments as Completed or No‑Show

Filter appointments by date

Admin

Create, edit, deactivate doctors

View all appointments (filter by date, mode, status)

Change any appointment status (override)

Generate daily summary reports (by mode & specialty)

Manage specialties

Business Rules

Online and offline appointments use different doctors (strictly enforced)

Slot locking prevents double booking (transaction + overlap check)

Audit trail: every status change is logged

Role‑based access control (Patient / Doctor / Admin)

🛠️ Technology Stack
Layer	Technology
Backend	ASP.NET Core 8 MVC, Entity Framework Core
Database	SQL Server (Express / LocalDB)
Authentication	JWT + Cookie Authentication (MVC friendly)
Logging	Built‑in ILogger
API Documentation	Swagger / OpenAPI
📦 Prerequisites
.NET 8 SDK

SQL Server (or LocalDB / SQL Express)

Git (optional)

A code editor (Visual Studio 2022 / VS Code / Rider)

🚀 Getting Started
1. Clone or download the project
bash
git clone https://github.com/your-repo/doctor-appointment-system.git
cd doctor-appointment-system
2. Restore NuGet packages
bash
dotnet restore
3. Update the connection string
Edit appsettings.json and set your SQL Server connection:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=DoctorAppointmentDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
4. Apply database migrations & seed data
bash
dotnet ef migrations add InitialCreate
dotnet ef database update
The seeding will create:

Admin user (admin@clinic.com / Admin@123)

30 specialties + 60 doctors (online & offline for each)

Doctor availabilities (Mon‑Fri, 9 AM – 5 PM, 30‑min slots)

5. Run the application
bash
dotnet run
Open your browser at https://localhost:5101 (or the port shown in the console).

🔐 Default Credentials
Role	Email	Password
Admin	admin@clinic.com	Admin@123
Doctor	sarah.general@clinic.com	Doctor@123
Doctor	james.cardio@clinic.com	Doctor@123
Doctor	mark.general@clinic.com	Doctor@123
Patient	(register new account)	(user chosen)
Note: The doctor accounts are pre‑seeded in the Users table only if you added the code in DbSeeder.cs. If not, log in as admin and create them via the admin panel.

📁 Project Folder Structure
text
DoctorAppointmentSystem/
│
├── Controllers/
│   ├── Api/                     (REST API controllers – optional)
│   └── UI/                      (MVC controllers for frontend)
│       ├── AccountController.cs
│       ├── AdminController.cs
│       ├── AppointmentsController.cs
│       ├── DoctorsUiController.cs   ← doctor dashboard
│       ├── HomeController.cs
│       └── PatientController.cs
│
├── Data/
│   ├── AppDbContext.cs          (EF Core DbContext)
│   └── DbSeeder.cs              (initial data seeding)
│
├── DTOs/                        (Data Transfer Objects)
│   ├── AppointmentDtos.cs
│   ├── AuthDtos.cs
│   ├── DoctorDtos.cs
│   └── SpecialtyDtos.cs
│
├── Models/                      (Entity models)
│   ├── User.cs
│   ├── Doctor.cs
│   ├── Specialty.cs
│   ├── Appointment.cs
│   ├── DoctorAvailability.cs
│   └── Enums.cs                 (AppointmentMode, AppointmentStatus, UserRole)
│
├── Services/                    (Business logic)
│   ├── AppointmentService.cs
│   ├── AuthService.cs
│   ├── DoctorService.cs
│   └── TokenService.cs
│
├── ViewModels/                  (View models for MVC)
│   ├── AccountViewModels.cs
│   ├── AppointmentViewModels.cs
│   └── DoctorViewModels.cs
│
├── Views/                       (Razor views)
│   ├── Account/
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   ├── Admin/
│   │   ├── Dashboard.cshtml
│   │   ├── Appointments.cshtml
│   │   ├── Doctors.cshtml
│   │   └── CreateDoctor.cshtml
│   ├── Appointments/
│   │   ├── Book.cshtml
│   │   ├── Confirmation.cshtml
│   │   └── MyAppointments.cshtml
│   ├── Doctors/
│   │   └── Dashboard.cshtml     (doctor's view)
│   ├── Home/
│   │   └── Index.cshtml
│   ├── Patient/
│   │   ├── Dashboard.cshtml
│   │   ├── BrowseDoctors.cshtml
│   │   ├── GetSlots.cshtml
│   │   └── History.cshtml
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   └── _ValidationScripts.cshtml
│   └── _ViewStart.cshtml
│
├── wwwroot/                     (Static files: CSS, JS, images)
│   ├── css/
│   └── js/
│
├── Migrations/                  (EF Core migration files – auto‑generated)
├── Properties/
│   └── launchSettings.json
├── appsettings.json             (Configuration – connection strings, JWT)
├── Program.cs                   (Application entry point & DI)
└── README.md                    (this file)
🌐 API Endpoints (Optional REST API)
The project also includes a Swagger UI at /swagger with the following endpoints:

Method	Endpoint	Role	Description
GET	/api/doctors	Anyone	List doctors (filter by specialty/mode)
GET	/api/doctors/{id}/slots	Anyone	Get available slots for a doctor on a date
POST	/api/doctors	Admin	Create a new doctor
PATCH	/api/doctors/{id}	Admin	Update doctor details
DELETE	/api/doctors/{id}	Admin	Deactivate doctor
POST	/api/doctors/{id}/availability	Admin	Set availability
PUT	/api/doctors/appointments/{id}/status	Doctor/Admin	Update appointment status
POST	/api/auth/register	Anyone	Register a new user
POST	/api/auth/login	Anyone	Login (returns JWT)
👥 How to Use the Application
Patient flow
Go to http://localhost:5101 → click Register → create a patient account.

Log in → you are on the patient dashboard.

Click Book New Appointment → select a specialty → filter by Online/Offline mode.

Pick a doctor → choose an available time slot → confirm.

Online → you receive a video meeting link.

Offline → you receive the clinic address.

View your upcoming appointments → cancel if needed.

Click History to see past appointments.

Doctor flow
Log in with doctor credentials (e.g., sarah.general@clinic.com / Doctor@123).

You are redirected to /DoctorsUi/Dashboard.

See Upcoming appointments (status = Confirmed) → click Completed or No‑Show.

The appointment moves to Past section.

Use the date filter to view appointments for a specific day.

Admin flow
Log in as admin@clinic.com / Admin@123.

Admin dashboard shows key metrics + daily summary.

Manage doctors: Doctors → create/edit/deactivate.

View all appointments → filter / change status.

Create new specialties if needed.

⚙️ Configuration Files
appsettings.json (skeleton)
json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DoctorAppointmentDb;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyAtLeast32CharsLong!",
    "Issuer": "MediBookAPI",
    "Audience": "MediBookClient",
    "ExpiryHours": "24"
  },
  "IpRateLimiting": {
    "GeneralRules": [
      { "Endpoint": "*", "Period": "1m", "Limit": 60 },
      { "Endpoint": "post:/api/auth/*", "Period": "5m", "Limit": 10 }
    ]
  },
  "Logging": { ... }
}
🧪 Testing
Use Swagger (/swagger) to test REST endpoints (JWT required for protected endpoints).

For MVC, simply navigate the UI.

Postman collection can be exported from Swagger.

📜 Business Rules Summary (Enforced in Code)
Different doctors per mode – a doctor is either Online or Offline; patients cannot book an online appointment with an offline doctor.

Slot locking – uses database transactions to prevent double booking.

Status lifecycle – Patient can only cancel; Doctor can only set Completed/No‑Show; Admin can override.

Audit trail – every status change is logged in AppointmentAudit (optional, extend the model).

Artifacts – Online appointments generate a unique meeting link; Offline appointments store a snapshot of clinic address.

Daily summary revenue – only Completed appointments contribute; fee is taken from a snapshot (FeeAtBooking).

❓ Troubleshooting
Problem	Solution
UseSqlServer not found	Run dotnet add package Microsoft.EntityFrameworkCore.SqlServer
Migrations fail	Drop database: dotnet ef database drop -f, then re‑migrate
Admin / doctor login fails	Ensure DbSeeder.SeedAsync() is called in Program.cs and that BCrypt is installed.
Doctor dashboard not showing	Check redirect in AccountController: case "Doctor" → RedirectToAction("Dashboard", "DoctorsUi")
Slots not loading	Verify DoctorAvailability records exist for that doctor (seeded automatically).


# Authentication Flow

```text
User Login
     ↓
Cookie Authentication
     ↓
Authentication Middleware
     ↓
Authorization Check
     ↓
Access Granted / Denied
```

---

# Appointment Booking Workflow

```text
Patient Login
      ↓
Browse Doctors
      ↓
Select Doctor
      ↓
Choose Date & Time
      ↓
Submit Appointment
      ↓
Store in Database
      ↓
Confirmation Displayed
```

---

# Entity Framework Core

The application uses Entity Framework Core for:

* Database Creation
* Table Mapping
* CRUD Operations
* Migrations
* Data Seeding

### Migration Commands

```bash
Add-Migration InitialCreate
Update-Database
```

Or using .NET CLI:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

# Configuration

Update the database connection string in:

```json
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=DoctorAppointmentDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

# Running the Application

### Clone Repository

```bash
git clone <repository-url>
```

### Navigate to Project

```bash
cd DoctorAppointmentBooking
```

### Restore Packages

```bash
dotnet restore
```

### Apply Migrations

```bash
dotnet ef database update
```

### Run Application

```bash
dotnet run
```

Or launch directly from **Visual Studio** using:

```text
IIS Express
```

or

```text
HTTPS Profile
```

---

# Design Patterns Used

### Dependency Injection

Used for loose coupling between components.

### Repository Pattern

Used to separate data access logic from business logic.

### Service Layer Pattern

Used to centralize application business rules.

### MVC Pattern

Used to organize Controllers, Views, and Models.

---

# Security Features

* Password Hashing
* Cookie Authentication
* Authorization
* Input Validation
* Anti-Forgery Token Protection
* Secure Database Access using Entity Framework Core

---

# Future Enhancements

* Email Notifications
* SMS Appointment Reminders
* Online Payments
* Doctor Ratings and Reviews
* Telemedicine Integration
* Appointment Rescheduling
* Medical History Management

---

# Conclusion

The Doctor Appointment Booking System is a scalable and user-friendly healthcare application built with **ASP.NET Core, Razor Pages, Entity Framework Core, and SQL Server**. It provides an efficient platform for managing doctor appointments while following modern software architecture principles, security standards, and best development practices.
