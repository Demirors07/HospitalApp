using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalApp.Models;
using HospitalApp;
using Microsoft.AspNetCore.Authorization;
using HospitalApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
public class ClinicController : Controller
{
    private readonly StoreDbContext _context;

    public ClinicController(StoreDbContext context)
    {
        _context = context;
    }

    // Method by which clinics are listed
    public IActionResult Index()
    {
        var clinics = _context.Clinics.ToList();
        return View(clinics);
    }

    // Method to show details of doctors in the selected clinic
    public IActionResult Details(int id)
    {
        var clinic = _context.Clinics
            .Include(c => c.Doctors)
            .FirstOrDefault(c => c.Id == id);

        if (clinic == null)
            return NotFound();

        return View(clinic);
    }
}