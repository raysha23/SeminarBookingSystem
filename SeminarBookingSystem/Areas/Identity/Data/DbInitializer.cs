using Microsoft.AspNetCore.Identity;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Areas.Identity.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(UserManager<AdminUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed roles
            string[] roles = { "Super Admin", "Staff Admin" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // Seed admin user
            var adminEmail = "admin@seminar.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AdminUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Super Admin",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
            }

            // Assign role
            if (!await userManager.IsInRoleAsync(adminUser, "Super Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Super Admin");
            }
        }
    }
}