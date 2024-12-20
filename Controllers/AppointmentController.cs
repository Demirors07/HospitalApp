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
            ViewData["aaa"] = "dataaaaaaaaaaa";
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Create(Appointment model)
    {
        
        var userId = _userManager.GetUserId(User);
        model.PatientId = userId;
        // var doctor = await _context.Doctors.FindAsync(model.DoctorId);
        // model.Doctor = doctor;
        var doctor = await _context.Doctors.FindAsync(model.DoctorId);
        model.Doctor = doctor; // Doctor bilgisini modele ekle

        var availableTimes = GetTimes();
        
        ViewData["aaa"] ="aaaaaaaaaaaaaaaaaaaaaaaaaaaaasdsadasdsaaa";
        ViewBag.time = availableTimes;

        // ViewBag.aaa = "sss";
        
        
            return View("AvailableTimes",model);
        
    
            

        return View(model);
    }

   [HttpGet]
[HttpPost]
public IActionResult AvailableTimes(DateTime appointmentDate, string AppointmentTime)
{
    // Çalışma saatleri
    var allTimes = new List<string>
    {
        "09:00", "09:20", "09:40", "10:00", "10:20", "10:40", "11:00", "11:20", "11:40",
        "13:40", "14:00", "14:20", "14:40", "15:00", "15:20", "15:40", "16:00", "16:20", "16:40", "17:00"
    };

    // Bugün için saati kontrol et
    if (appointmentDate.Date == DateTime.Today)
    {
        var currentTime = DateTime.Now.TimeOfDay;
        allTimes = allTimes
            .Where(t => TimeSpan.Parse(t) > currentTime) // Geçmiş saatleri filtrele
            .ToList();
    }

    // Saat kontrolü
    if (appointmentDate.Date == DateTime.Today && !string.IsNullOrEmpty(AppointmentTime))
    {
        var selectedTime = TimeSpan.Parse(AppointmentTime);
        if (selectedTime <= DateTime.Now.TimeOfDay)
        {
            ModelState.AddModelError("AppointmentTime", "You cannot select a past time.");
            ViewBag.time = allTimes;  // Zamanları tekrar gönder
            return View();
        }
    }

    // Randevuyu kaydetmek için buraya logik ekleyebilirsiniz
    // Örnek: Veritabanına kaydetme işlemi

    return RedirectToAction("AppointmentDetails"); // Başarılı işlem sonrası yönlendirme
}

// [HttpPost]
// public IActionResult AvailableTimes(Appointment model)
// {
   

//     return View("/Home/Index");
// }

 [HttpPost]
 // End of the adding appointment processes
 // Finally, We added the database
     public async Task<ActionResult> AvailableTimes(Appointment appointment){
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return RedirectToAction("AppointmentDetails", "Appointment");
    }


// Örnek bir zaman üretim metodu
private List<TimeSpan> GetTimes()
{
    // Örneğin 9:00 ile 17:00 arasında 20 dakikalık dilimler oluştur
    var startTime = new TimeSpan(9, 0, 0);
    var endTime = new TimeSpan(17, 0, 0);
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

    return timeSlots.ToList();
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
            // Kullanıcının kimliğini al
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Geçmiş randevular
            var pastAppointments = await _context.Appointments
                .Include(a => a.Doctor) // Doktor bilgilerini dahil et
                .Where(a => a.PatientId == userId && a.AppointmentDate < DateTime.Now)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            // Gelecekteki randevular
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
