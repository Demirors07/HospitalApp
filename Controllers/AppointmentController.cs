using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Models;
using HospitalApp.ViewModels;
using HospitalApp;
using Microsoft.AspNetCore.Authorization;
using HospitalApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

[Authorize(Roles = "Admin,User")]
public class AppointmentController : Controller
{
    private readonly StoreDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public AppointmentController(StoreDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Appointments/Create
    public IActionResult Create()
    {
        ViewBag.Clinics = _context.Clinics.ToList();
        ViewBag.Doctors = _context.Doctors.ToList();
        ViewBag.user = _userManager.GetUserId(User);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Appointment model)
    {
        var userId = _userManager.GetUserId(User);
        model.PatientId = userId;

        // Get selected doctor information
        var doctor = await _context.Doctors.FindAsync(model.DoctorId);
        model.Doctor = doctor; // Add doctor information to the model

        ViewBag.DName = model.Doctor.Name;

        // Get timeslots
        var availableTimes = GetTimes();
        ViewBag.time = availableTimes;

        return View("AvailableTimes", model);
    }

    [HttpPost]
    public async Task<ActionResult> AvailableTimes(Appointment appointment)
    {
        // Fill timeslots
        ViewBag.time = GetTimes();

        // Check and fill in doctor information if it is missing
        if (appointment.DoctorId > 0)
        {
            var doctor = await _context.Doctors.FindAsync(appointment.DoctorId);
            if (doctor != null)
            {
                ViewBag.DoctorName = doctor.Name;
            }
            else
            {
                ViewBag.DoctorName = "Unknown Doctor";
            }
        }

        // Check appointments for the same date and time only for the selected doctor
        var existingAppointment = await _context.Appointments
            .Where(a =>
                a.DoctorId == appointment.DoctorId && // Only this doctor
                a.AppointmentDate.Date == appointment.AppointmentDate.Date && // Same Day
                a.AppointmentTime == appointment.AppointmentTime) // Same Hour
            .FirstOrDefaultAsync();

        if (existingAppointment != null)
        {
            // If the appointment is already booked, add an error message and show the form again.
            ViewData["ErrorMessage"] = $"Doctor {ViewBag.DoctorName} is already booked at {appointment.AppointmentTime}. Please select another time.";
            return View("AvailableTimes", appointment);
        }

        // If there is no appointment on that date and day, add it to the database
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // If added successfully, redirect to appointment details page
        return RedirectToAction("AppointmentDetails", "Appointment");
    }

    // time production method
    private List<TimeSpan> GetTimes()
    {
        var startTime = new TimeSpan(9, 0, 0); // Starting: 09:00
        var endTime = new TimeSpan(17, 0, 0);  // Finishing: 17:00
        var timeSlots = new List<TimeSpan>();

        for (var time = startTime; time < endTime; time += TimeSpan.FromMinutes(20))
        {
            timeSlots.Add(time);
        }
        return timeSlots;
    }

    // For fetch process with Ajax.
    [HttpGet]
    public IActionResult GetDoctorsByClinic(int clinicId)
    {
        var doctors = _context.Doctors
            .Where(d => d.ClinicId == clinicId)
            .Select(d => new { d.Id, d.Name })
            .ToList();

        return Json(doctors);
    }

    [HttpGet]
    public async Task<IActionResult> AppointmentDetails()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var pastAppointments = await _context.Appointments
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == userId && a.AppointmentDate < DateTime.Now)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();

        var upcomingAppointments = await _context.Appointments
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == userId && a.AppointmentDate >= DateTime.Now)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();

        var model = new AppointmentDetailsViewModel
        {
            PastAppointments = pastAppointments,
            UpcomingAppointments = upcomingAppointments
        };

        return View(model);
    }

    // Some Crud Operations
    public IActionResult EditClinics()
    {
        var clinics = _context.Clinics.ToList();
        return View(clinics);
    }

    public IActionResult EditDoctors()
    {
        var doctors = _context.Doctors.ToList();
        return View(doctors);
    }

    public IActionResult DeleteClinics()
    {
        var clinics = _context.Clinics.ToList();
        return View(clinics);
    }

    public IActionResult DeleteDoctors()
    {
        var doctors = _context.Doctors.ToList();
        return View(doctors);
    }
}