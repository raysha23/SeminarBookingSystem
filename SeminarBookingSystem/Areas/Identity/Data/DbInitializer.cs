using Microsoft.AspNetCore.Identity;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Areas.Identity.Data
{
  public static class DbInitializer
  {
    public static async Task SeedData(UserManager<AdminUser> userManager, RoleManager<IdentityRole> roleManager)
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
          Console.WriteLine("Admin user created successfully.");

          // ✅ Explicitly confirm email
          adminUser.EmailConfirmed = true;
          await userManager.UpdateAsync(adminUser);
          Console.WriteLine("Admin email confirmed.");

          // ✅ Assign Super Admin role
          await userManager.AddToRoleAsync(adminUser, "Super Admin");
          Console.WriteLine("Admin assigned to Super Admin role.");
        }
        else
        {
          Console.WriteLine("Failed to create admin user:");
          foreach (var error in createResult.Errors)
          {
            Console.WriteLine($" - {error.Description}");
          }
        }
      }
      else
      {
        // User already exists, ensure email is confirmed and role assigned
        if (!adminUser.EmailConfirmed)
        {
          adminUser.EmailConfirmed = true;
          await userManager.UpdateAsync(adminUser);

          Console.WriteLine("Admin email confirmed (existing user).");
        }

        var rolesForUser = await userManager.GetRolesAsync(adminUser);
        if (!rolesForUser.Contains("Super Admin"))
        {
          await userManager.AddToRoleAsync(adminUser, "Super Admin");
          Console.WriteLine("Admin assigned to Super Admin role (existing user).");
        }

        Console.WriteLine("Admin user already exists.");
      }
    }
  }
}