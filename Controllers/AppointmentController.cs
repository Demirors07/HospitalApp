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
        
        // Seçilen doktor bilgisini al
        var doctor = await _context.Doctors.FindAsync(model.DoctorId);
        model.Doctor = doctor; // Doctor bilgisini modele ekle

        // Zaman dilimlerini al
        var availableTimes = GetTimes();
       // nerede yazıyor bu ViewData["aaa"] ="aaaaaaaaaaaaaaaaaaaaaaaaaaaaasdsadasdsaaa";
        ViewBag.time = availableTimes;

        // ViewBag.aaa = "sss";
        
        // Aynı doktor, aynı tarih ve saatte başka bir randevu olup olmadığını kontrol et
        var existingAppointment = await _context.Appointments
            .Where(a => a.DoctorId == model.DoctorId &&
                        a.AppointmentDate.Date == model.AppointmentDate.Date &&
                        a.AppointmentTime == model.AppointmentTime)
            .FirstOrDefaultAsync();

        if (existingAppointment != null)
        {
            // Eğer randevu zaten alınmışsa, hata mesajı ekle
            ViewData["ErrorMessage"] = "This appointment time is already taken. Please choose another time.";
            return View("AvailableTimes", model); // Kullanıcıyı AvailableTimes sayfasına yönlendir
        }

        return View("AvailableTimes", model);
    }

    [HttpPost]
    public async Task<ActionResult> AvailableTimes(Appointment appointment)
    {
        // Zaman dilimlerini doldur
        ViewBag.time = GetTimes();

        // Doktor bilgisi eksikse kontrol et ve doldur
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

        // Sadece seçilen doktor için aynı tarih ve saatte randevuları kontrol et
        var existingAppointment = await _context.Appointments
            .Where(a => 
                a.DoctorId == appointment.DoctorId && // Sadece bu doktor
                a.AppointmentDate.Date == appointment.AppointmentDate.Date && // Aynı gün
                a.AppointmentTime == appointment.AppointmentTime) // Aynı saat
            .FirstOrDefaultAsync();

        if (existingAppointment != null)
        {
            // Eğer randevu zaten alınmışsa, hata mesajı ekleyip tekrar formu göster
            ViewData["ErrorMessage"] = $"Doctor {ViewBag.DoctorName} is already booked at {appointment.AppointmentTime}. Please select another time.";
            return View("AvailableTimes", appointment);
        }

        // Eğer randevu alınmamışsa veritabanına ekle
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        // Başarıyla eklenirse, randevu detayları sayfasına yönlendir
        return RedirectToAction("AppointmentDetails", "Appointment");
    }

    // Örnek bir zaman üretim metodu
    private List<TimeSpan> GetTimes()
    {
        var startTime = new TimeSpan(9, 0, 0); // Başlangıç: 09:00
        var endTime = new TimeSpan(17, 0, 0);  // Bitiş: 17:00
        var timeSlots = new List<TimeSpan>();

        for (var time = startTime; time < endTime; time += TimeSpan.FromMinutes(20))
        {
            timeSlots.Add(time);
        }

        // Mevcut randevuları kontrol edip dolu zamanları çıkarabilirsiniz
    // var bookedTimes = _context.Appointments
    //     .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
    //     .Select(a => a.AppointmentTime)
    //     .ToList();

        return timeSlots;
    }

    [HttpGet]
    public IActionResult GetDoctorsByClinic(int clinicId)
    {
        var doctors = _context.Doctors
            .Where(d => d.ClinicId == clinicId)
            .Select(d => new { d.Id, d.Name })
            .ToList();

        return Json(doctors);
    }
    

//  // Randevu al ve kaydet
//     [HttpPost]
//     public IActionResult BookAppointment(long doctorId, DateTime date, TimeSpan time, string PatientID)
//     {
//         // Randevu modelini oluştur
//         var appointment = new Appointment
//         {
//             DoctorId = doctorId,
//             AppointmentDate = date,
//             AppointmentTimeInMinutes = (int)time.TotalMinutes, // TimeSpan'den int'e dönüştürme
//             ClinicId = _context.Doctors.FirstOrDefault(d => d.Id == doctorId)?.Id ?? 0,
//             PatientId = PatientID,
//         };

//         _context.Appointments.Add(appointment);
//         _context.SaveChanges();

//         return RedirectToAction("AppointmentDetails");
//     }


    [HttpGet]
    public async Task<IActionResult> AppointmentDetails()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var pastAppointments = await _context.Appointments
            .Include(a => a.Doctor) // Doktor bilgilerini dahil et
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



