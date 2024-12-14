using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Models {
public static class SeedData {

    //IApplicationBuilder interface used to register middleware components to handle HTTP requests
    public static void EnsurePopulated(IApplicationBuilder app) {
        StoreDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<StoreDbContext>();
        
        if (context.Database.GetPendingMigrations().Any()) {
            context.Database.Migrate();
        }

         
        
        }
    }
}