using Microsoft.AspNetCore.Identity;
using SeminarBookingSystem.Models;
using SeminarBookingSystem.Data;
using Microsoft.EntityFrameworkCore;

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
      var adminEmail = "admin@seminar.com";
      var adminUser = await userManager.FindByEmailAsync(adminEmail);

      if (adminUser == null)
      {
        adminUser = new AdminUser
        {
          UserName = adminEmail,
          Email = adminEmail,
          FullName = "Raymund Sismundo",
          CreatedAt = DateTime.UtcNow,
          IsActive = true
        };

        var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
        if (createResult.Succeeded)
        {
          adminUser.EmailConfirmed = true;
          await userManager.UpdateAsync(adminUser);

          await userManager.AddToRoleAsync(adminUser, "Super Admin");
          Console.WriteLine("Admin created and assigned role.");
        }
      }
      else
      {
        if (!adminUser.EmailConfirmed)
        {
          adminUser.EmailConfirmed = true;
          await userManager.UpdateAsync(adminUser);
        }

        var rolesForUser = await userManager.GetRolesAsync(adminUser);
        if (!rolesForUser.Contains("Super Admin"))
        {
          await userManager.AddToRoleAsync(adminUser, "Super Admin");
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
                        Date = DateTime.UtcNow.AddDays(7),
                        Time = "10:00 AM", // ✅ added
                        Location = "Conference Room A",
                        Capacity = 50,
                        Status = "Upcoming", // ✅ IMPORTANT
                        CurrentParticipants = 0, // ✅ IMPORTANT
                        CreatedAt = DateTime.UtcNow
                    },
                    new Seminar
                    {
                        SeminarTitle = "C# and .NET Fundamentals",
                        Description = "Introduction to C# programming and .NET framework.",
                        Date = DateTime.UtcNow.AddDays(10),
                        Time = "1:00 PM",
                        Location = "Conference Room B",
                        Capacity = 40,
                        Status = "Upcoming",
                        CurrentParticipants = 0,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Seminar
                    {
                        SeminarTitle = "Database Management with SQL",
                        Description = "Learn how to design and query databases.",
                        Date = DateTime.UtcNow.AddDays(14),
                        Time = "3:00 PM",
                        Location = "Conference Room C",
                        Capacity = 60,
                        Status = "Upcoming",
                        CurrentParticipants = 0,
                        CreatedAt = DateTime.UtcNow
                    }
                };

        await context.Seminar.AddRangeAsync(seminars);
        await context.SaveChangesAsync();

        Console.WriteLine("Seminars seeded successfully.");
      }
      else
      {
        Console.WriteLine("Seminars already exist.");
      }
    }
  }
}