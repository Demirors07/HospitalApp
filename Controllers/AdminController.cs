using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Models;
using HospitalApp.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HospitalApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly StoreDbContext _context;

        public AdminController(StoreDbContext context)
        {
            _context = context;
        }


        // Klinik Ekleme
        [HttpGet]
        public IActionResult AddClinic()
        {
            var model = new Clinic();
            return View(model);
        }

       [HttpPost]
     public async Task<ActionResult> AddClinic(Clinic clinic){
        _context.Clinics.Add(clinic);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

        // Klinik Listeleme
        public async Task<IActionResult> ClinicList()
        {
            var clinics = await _context.Clinics.ToListAsync();
            return View(clinics);
        }

        // Klinik Güncelleme
        [HttpGet]
        public async Task<IActionResult> EditClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }
            return View(clinic);
        }

        [HttpPost]
    public async Task<IActionResult> EditClinic(int id, Clinic clinic){
        clinic.ClinicId = id;

        //if(ModelState.IsValid){
            try{
                _context.Update(clinic);
                await _context.SaveChangesAsync();
            }catch(Exception){
                throw;
            }
            return RedirectToAction("Clinics", "Appointment");
        //}

        return View(clinic);
    }

        

[HttpGet]
     public ActionResult AddDoctor(){
        return View();
    }

        // Doktor Ekleme İşlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDoctor(Doctor doctor)
        {
           _context.Doctors.Add(doctor);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");

        }
            
        


        // Doktor Güncelleme View'ı
public async Task<IActionResult> EditDoctor(long id)
{
    var doctor = await _context.Doctors.FindAsync(id);
    if (doctor == null)
    {
        return NotFound();
    }

    return View(doctor);
}

[HttpPost]
    public async Task<IActionResult> EditDoctor(long id, Doctor doctor){
        doctor.Id = id;

        //if(ModelState.IsValid){
            try{
                _context.Update(doctor);
                await _context.SaveChangesAsync();
            }catch(Exception){
                throw;
            }
            return RedirectToAction("EditDoctors", "Appointment");
        //}

        return View(doctor);
    }

[HttpGet]
public async Task<IActionResult> DeleteDoctor(long? id)
{
    var doctor = await _context.Doctors.FindAsync(id);
    if (doctor == null)
    {
        return NotFound();
    }

    return View(doctor);
}

[HttpPost]
    public async Task<IActionResult> DeleteDoctor([FromForm] long id){
        var doctor = await _context.Doctors.FindAsync(id);
        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
        return RedirectToAction("DeleteClinics", "Appointment");
    }




public async Task<IActionResult> DeleteClinic(long? id)
{
    var clinic = await _context.Clinics.FindAsync(id);
    if (clinic == null)
    {
        return NotFound();
    }

    return View(clinic);
}


[HttpPost]
    public async Task<IActionResult> DeleteClinic( long id){
        var clinic = await _context.Clinics.FindAsync(id);
        _context.Clinics.Remove(clinic);
        await _context.SaveChangesAsync();
        return RedirectToAction("DeleteDoctors", "Appointment");
    }



    }
}