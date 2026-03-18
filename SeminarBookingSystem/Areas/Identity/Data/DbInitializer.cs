using Microsoft.AspNetCore.Identity;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Areas.Identity.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminUser(UserManager<AdminUser> userManager)
        {
            string adminEmail = "admin@seminar.com";
            string adminPassword = "Admin123!";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var adminUser = new AdminUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create admin user: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                else
                {
                    Console.WriteLine("Admin user created successfully!");
                }
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }
        }
    }
}