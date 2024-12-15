using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Models {
public static class SeedData {

    //IApplicationBuilder interface used to register middleware components to handle HTTP requests
    public static void EnsurePopulated(IApplicationBuilder app) {
        StoreDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<StoreDbContext>();
        
        if (context.Database.GetPendingMigrations().Any()) {
            context.Database.Migrate();
        }

         if(!context.Clinics.Any()){
            context.Clinics.AddRange(
                new Clinic {
                    ClinicId = 1,
                    ClinicName = "KBB"
                },
                new Clinic {
                    ClinicId = 2,
                    ClinicName = "Üroloji"
                }
            );
            context.SaveChanges();
        }

        if (!context.Doctors.Any()) {
            context.Doctors.AddRange(
                new Doctor {
                    Name = "Arda ALİŞAN",
                    ClinicId = 1},
                    
                    
                new Doctor {
                    Name = "Muhammed DEMİRÖRS", 
                    ClinicId = 2}
                    );
                context.SaveChanges();
            }

         
        
        }
    }
}