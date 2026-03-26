using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Areas.Identity.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(
            UserManager<AdminUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SeminarBookingSystemContext context)
        {
            // 1️⃣ Seed roles
            string[] roles = { "Super Admin", "Staff Admin" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                    Console.WriteLine($"Role created: {roleName}");
                }
            }

            // 2️⃣ Seed admin user
            var adminEmail = "admin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AdminUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Raymund Sismundo",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    adminUser.EmailConfirmed = true;
                    await userManager.UpdateAsync(adminUser);

                    await userManager.AddToRoleAsync(adminUser, "Super Admin");
                    Console.WriteLine("Admin created.");
                }
            }

            // 3️⃣ Seed seminars
            if (!await context.Seminar.AnyAsync())
            {
                var seminars = new List<Seminar>
                {
                    new Seminar
                    {
                        SeminarTitle = "Web Development Basics",
                        Description = "Learn HTML, CSS, and JavaScript fundamentals.",
                        Date = DateTime.Now.AddDays(7),
                        Time = "10:00",
                        Location = "Conference Room A",
                        Capacity = 50,
                        CurrentParticipants = 0,
                        CreatedAt = DateTime.Now
                    },
                    new Seminar
                    {
                        SeminarTitle = "C# and .NET Fundamentals",
                        Description = "Introduction to C# programming and .NET.",
                        Date = DateTime.Now.AddDays(10),
                        Time = "13:00",
                        Location = "Conference Room B",
                        Capacity = 40,
                        CurrentParticipants = 0,
                        CreatedAt = DateTime.Now
                    },
                    new Seminar
                    {
                        SeminarTitle = "Database Management with SQL",
                        Description = "Learn database design and queries.",
                        Date = DateTime.Now.AddDays(14),
                        Time = "15:00",
                        Location = "Conference Room C",
                        Capacity = 60,
                        CurrentParticipants = 0,
                        CreatedAt = DateTime.Now
                    }
                };

                // ✅ Assign dynamic status (initial only)
                foreach (var seminar in seminars)
                {
                    seminar.Status = GetSeminarStatus(seminar.Date, seminar.Time);
                }

                await context.Seminar.AddRangeAsync(seminars);
                await context.SaveChangesAsync();

                // ✅ Activity logs
                var logs = seminars.Select(s => new ActivityLog
                {
                    UserId = "System",
                    Action = "Created",
                    Entity = "Seminar",
                    Details = $"Seeded seminar: {s.SeminarTitle} (ID: {s.Id})",
                    Timestamp = DateTime.Now
                }).ToList();

                await context.ActivityLog.AddRangeAsync(logs);
                await context.SaveChangesAsync();

                Console.WriteLine("Seminars and logs seeded successfully.");
            }
        }

        // ✅ Status Logic
        private static string GetSeminarStatus(DateTime seminarDate, string seminarTime, double durationHours = 1)
        {
            if (!TimeSpan.TryParse(seminarTime, out var timeSpan))
                timeSpan = TimeSpan.Zero;

            var seminarDateTime = seminarDate.Date + timeSpan;
            var endTime = seminarDateTime.AddHours(durationHours);
            var now = DateTime.Now;

            if (now < seminarDateTime)
                return "Upcoming";
            else if (now >= seminarDateTime && now <= endTime)
                return "Ongoing";
            else
                return "Completed";
        }
    }
}