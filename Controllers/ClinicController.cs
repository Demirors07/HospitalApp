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

    // Kliniklerin listelendiği metot
    public IActionResult Index()
    {
        var clinics = _context.Clinics.ToList();
        return View(clinics);
    }

    // Seçilen klinikteki doktorların detaylarının gösterildiği metot
    public IActionResult Details(int id)
    {
        var clinic = _context.Clinics
            .Include(c => c.Doctors) // Doktorları dahil ediyoruz
            .FirstOrDefault(c => c.Id == id);

        if (clinic == null)
            return NotFound();

        return View(clinic);
    }
}