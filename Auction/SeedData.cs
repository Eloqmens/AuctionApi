using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Auction
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var identityDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await identityDbContext.Database.EnsureCreatedAsync();
                await appDbContext.Database.EnsureCreatedAsync();
          
                var adminRole = new IdentityRole("Admin");
                if (!await roleManager.RoleExistsAsync(adminRole.Name))
                {
                    await roleManager.CreateAsync(adminRole);
                }

                var userRole = new IdentityRole("User");
                if (!await roleManager.RoleExistsAsync(userRole.Name))
                {
                    await roleManager.CreateAsync(userRole);
                }


                if (!appDbContext.Categories.Any())
                {
                    appDbContext.Categories.AddRange(
                        new Category { Name = "Electronics" },
                        new Category { Name = "Books" },
                        new Category { Name = "Fashion" },
                        new Category { Name = "Home & Garden" },
                        new Category { Name = "Sports" }
                    );
                }

                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
