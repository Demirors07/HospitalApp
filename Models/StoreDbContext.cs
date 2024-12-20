using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Models {
    public class StoreDbContext : DbContext {
        public StoreDbContext(DbContextOptions<StoreDbContext> options): base(options) { }
        
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Clinic> Clinics => Set<Clinic>();
       


    }
}