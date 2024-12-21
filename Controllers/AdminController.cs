using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Models;
using HospitalApp.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace HospitalApp.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly StoreDbContext _context;

        public AdminController(StoreDbContext context)
        {
            _context = context;
        }


        // Adding Clinics
        [HttpGet]
        public IActionResult AddClinic()
        {
            var model = new Clinic();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddClinic(Clinic clinic)
        {
            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        // Adding Doctors
        [HttpGet]
        public ActionResult AddDoctor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDoctor(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");

        }

        // Updating Doctors
        [HttpGet]
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
        public async Task<IActionResult> EditDoctor(long id, Doctor doctor)
        {
            doctor.Id = id;
            try
            {
                _context.Update(doctor);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("EditDoctors", "Appointment");

            return View(doctor);
        }

        // Updating Clinics
        [HttpGet]
        public async Task<IActionResult> EditClinic(long id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound();
            }
            return View(clinic);
        }

        [HttpPost]
        public async Task<IActionResult> EditClinic(long id, Clinic clinic)
        {
            clinic.Id = id;
            try
            {
                _context.Update(clinic);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("EditClinics", "Appointment");

            return View(clinic);
        }

        // Deleting Doctors

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
        public async Task<IActionResult> DeleteDoctor([FromForm] long id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction("DeleteDoctors", "Appointment");
        }

        // Deleting Clinics
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
        public async Task<IActionResult> DeleteClinic(long id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();
            return RedirectToAction("DeleteClinics", "Appointment");
        }
    }
}