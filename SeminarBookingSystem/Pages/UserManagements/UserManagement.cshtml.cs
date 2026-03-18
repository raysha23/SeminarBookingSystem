using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SeminarBookingSystem.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SeminarBookingSystem.Pages.UserManagements
{
    public class UserManagementModel : PageModel
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public List<AdminUser> Users { get; set; } = new();
        public Dictionary<string, string> UserRoles { get; set; } = new();

        public UserManagementModel(UserManager<AdminUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public NewAdminInput Input { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Load all users
            Users = await _userManager.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();

            // Load roles for each user
            UserRoles.Clear();
            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserRoles[user.Id] = roles.FirstOrDefault() ?? "No Role";
            }
        }

        public async Task<IActionResult> OnPostCreateAdminAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                return Page();
            }

            // Ensure role exists
            if (!await _roleManager.RoleExistsAsync(Input.Role))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(Input.Role));
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return Page();
                }
            }

            // Create AdminUser
            var user = new AdminUser
            {
                FullName = Input.FullName,
                UserName = Input.Email,
                Email = Input.Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, "admin123");
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            // Assign role
            var roleAssignResult = await _userManager.AddToRoleAsync(user, Input.Role);
            if (!roleAssignResult.Succeeded)
            {
                foreach (var error in roleAssignResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            TempData["Message"] = $"Admin user '{Input.FullName}' created successfully with role '{Input.Role}'!";
            return RedirectToPage(); // Will reload table with OnGetAsync
        }

        public class NewAdminInput
        {
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; } = string.Empty;
        }
    }
}