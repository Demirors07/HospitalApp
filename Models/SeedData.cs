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
                    Id = 1,
                    Name = "KBB"
                },
                new Clinic {
                    Id = 2,
                    Name = "Üroloji"
                }
            );
            context.SaveChanges();
        }

        if (!context.Doctors.Any()) {
            context.Doctors.AddRange(
                new Doctor {
                    Name = "Muhammed DEMİRÖRS",
                    ClinicId = 1},
                    
                    
                new Doctor {
                    Name = "Arda ALİŞAN", 
                    ClinicId = 2}
                    );
                context.SaveChanges();
            }

         
        
        }
    }
}