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


[HttpGet]
public IActionResult GetDoctorsByClinic(int clinicId)
{
    if (clinicId <= 0)
        return BadRequest("Invalid clinic ID");

    var doctors = _context.Doctors
        .Where(d => d.ClinicId == clinicId)
        .Select(d => new { d.Id, d.Name })
        .ToList();

    return Json(doctors); // JSON olarak doktorları döner
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
    public IActionResult GetDoctors(int clinicId)
    {
        var doctors = _context.Doctors.Where(d => d.ClinicId == clinicId).ToList();
        return Json(doctors); // Return the doctors for the selected clinic
    }



   [HttpGet]
public IActionResult AvailableTimes(long doctorId, DateTime appointmentDate)
{
    // Doktorun o tarihteki müsait zamanlarını alın
    var availableTimes = GetAvailableTimes(doctorId, appointmentDate);

    // ViewModel oluştur
    var model = new AvailableTimeViewModel
    {
        DoctorId = doctorId,
        AppointmentDate = appointmentDate,
        AvailableTimes = availableTimes
    };

    return View(model);
}

[HttpPost]
public IActionResult AvailableTimes(Appointment model)
{
    // DoctorId ve Date doğrulama
    if (model.DoctorId <= 0 || model.AppointmentDate == DateTime.MinValue)
    {
        ModelState.AddModelError("", "Doctor and date must be selected.");
        ViewBag.Doctors = _context.Doctors.ToList(); // Formu tekrar doldurmak için
        return View("Create");
    }

    // Seçilen doktora uygun zamanları hesapla
    var availableTimes = GetAvailableTimes(model.DoctorId, model.AppointmentDate);

    // ViewModel ile verileri AvailableTimes.cshtml'e taşı
    var viewModel = new AvailableTimeViewModel
    {
        DoctorName = model.DoctorName,
        PatientID = model.PatientId,
        DoctorId = model.DoctorId,
        AppointmentDate = model.AppointmentDate,
        AvailableTimes = availableTimes
    };

    return View(viewModel);
}


// Örnek bir zaman üretim metodu
private List<TimeSpan> GetAvailableTimes(long doctorId, DateTime date)
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
    var bookedTimes = _context.Appointments
        .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
        .Select(a => a.AppointmentTime)
        .ToList();

    return timeSlots.Except(bookedTimes).ToList();
}

    

    // [HttpPost]
    //  public async Task<ActionResult> Create(Product product){
    //     _context.Products.Add(product);
    //     await _context.SaveChangesAsync();
    //     return RedirectToAction("Index");
    // }





     [HttpPost]
    public async Task<IActionResult> Create(Appointment model)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        model.PatientId = userId;
        
        // model.DoctorId = 1;
        // model.ClinicId = 1;
        // model.AppointmentDate = new DateTime(01/08/2025);
        // model.DoctorId = 1;

        
            // _context.Appointments.Add(model);
            // await _context.SaveChangesAsync();
            return RedirectToAction("AvailableTimes",model);
        

//         if (!ModelState.IsValid)
// {
//     var errors = ModelState.Values.SelectMany(v => v.Errors)
//                                    .Select(e => e.ErrorMessage)
//                                    .ToList();
//     // Hata mesajlarını loglayabilirsiniz
//     foreach(var error in errors)
//     {
//         Console.WriteLine(error);
//     }
//     ViewBag.Clinics = _context.Clinics.ToList();
//     return View(model);
// }

        ViewBag.Clinics = _context.Clinics.ToList();
        return View(model);
    }


      private List<TimeSpan> GenerateTimeSlots(TimeSpan start, TimeSpan end, int interval = 20)
    {
        var slots = new List<TimeSpan>();
        while (start < end)
        {
            slots.Add(start);
            start = start.Add(TimeSpan.FromMinutes(interval));
        }
        return slots;
    }



 // Randevu al ve kaydet
    [HttpPost]
    public IActionResult BookAppointment(long doctorId, string DoctorName, DateTime date, TimeSpan time, string PatientID)
    {
        // Randevu modelini oluştur
        var appointment = new Appointment
        {
            DoctorId = doctorId,
            DoctorName = DoctorName,
            AppointmentDate = date,
            AppointmentTimeInMinutes = (int)time.TotalMinutes, // TimeSpan'den int'e dönüştürme
            ClinicId = _context.Doctors.FirstOrDefault(d => d.Id == doctorId)?.ClinicId ?? 0,
            PatientId = PatientID,
        };

        _context.Appointments.Add(appointment);
        _context.SaveChanges();

        return RedirectToAction("AppointmentDetails");
    }



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

    // Randevu geçmişi ve geleceği
    // public IActionResult AppointmentDetails()
    // {
    //     var userId = User.Identity.Name; // Kullanıcı kimliği
    //     // var appointments = _context.Appointments
    //     //     .Where(a => a.PatientId == userId) // Kullanıcının randevularını filtrele
    //     //     .OrderBy(a => a.AppointmentDate)
    //     //     .ThenBy(a => a.AppointmentTime)
    //     //     .ToList();

    //     return View();
    // }
    


[HttpGet]
public async Task<IActionResult> GetDoctorsByClinic(long clinicId)
{
    // Klinik ID'ye göre doktorları al
    var doctors = await _context.Doctors
        .Where(d => d.ClinicId == clinicId)
        .Select(d => new
        {
            Id = d.Id,
            Name = d.Name
        })
        .ToListAsync();

    return Json(doctors); // JSON formatında doktor listesini döndür
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
