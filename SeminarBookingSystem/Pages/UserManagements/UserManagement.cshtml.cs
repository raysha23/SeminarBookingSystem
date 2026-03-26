//File Path: SeminarBookingSystem/Pages/UserManagements/UserManagement.cshtml.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace SeminarBookingSystem.Pages.UserManagements
{
    [Authorize(Roles = "Super Admin")]
    public class UserManagementModel : PageModel
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SeminarBookingSystemContext _context;

        public string CurrentLoginedInUserId { get; set; }
        public List<AdminUser> Users
        { get; set; } = new();
        public Dictionary<string, string> UserRoles { get; set; } = new();

        public UserManagementModel(UserManager<AdminUser> userManager, RoleManager<IdentityRole> roleManager, SeminarBookingSystemContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public NewAdminInput Input { get; set; } = new();

        [BindProperty]
        public EditAdminInput EditInput { get; set; } = new();
        private async Task LoadDataAsync()
        {
            Users = await _userManager.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            UserRoles.Clear();
            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserRoles[user.Id] = roles.FirstOrDefault() ?? "No Role";
            }
        }

        public async Task OnGetAsync()
        {
            CurrentLoginedInUserId = _userManager.GetUserId(User);
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostCreateAdminAsync()
        {
            try
            {
                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null)
                {
                    TempData["ErrorMessage"] = "Error, Email already exists!";
                    Console.WriteLine("Email already exists: " + Input.Email);
                    await LoadDataAsync();
                    return Page();
                }

                // Create the new admin user
                var user = new AdminUser
                {
                    FullName = Input.FullName,
                    UserName = Input.Email,
                    Email = Input.Email,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, "Admin123!");
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = "Create error: " + errors;
                    Console.WriteLine("CreateAsync errors: " + errors);
                    await LoadDataAsync();
                    return Page();
                }

                // Assign role
                await _userManager.AddToRoleAsync(user, Input.Role);

                // --- Add Activity Log here ---
                var log = new ActivityLog
                {
                    UserId = User.Identity.Name ?? "System",  // current logged-in admin
                    Action = "Created",
                    Entity = "User",
                    Details = $"Created admin: {Input.FullName} ({Input.Email}) with role {Input.Role}",
                    Timestamp = DateTime.UtcNow
                };

                _context.ActivityLog.Add(log);
                await _context.SaveChangesAsync();
                // ------------------------------

                TempData["SuccessMessage"] = "Admin created successfully!";
                Console.WriteLine("Admin created: " + Input.Email);
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                Console.WriteLine("Exception: " + ex);

                // Optional: log exception as ActivityLog
                var errorLog = new ActivityLog
                {
                    UserId = User.Identity.Name ?? "System",
                    Action = "Error",
                    Entity = "User",
                    Details = $"Failed to create admin {Input.Email}. Exception: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
                _context.ActivityLog.Add(errorLog);
                await _context.SaveChangesAsync();

                await LoadDataAsync();
                return Page();
            }
        }



        //Edit Post
        public async Task<IActionResult> OnPostEditAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditInput.Role))
                {
                    TempData["ErrorMessage"] = "Please select a role!";
                    await LoadDataAsync();
                    return Page();
                }

                var user = await _userManager.FindByIdAsync(EditInput.Id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found!";
                    return RedirectToPage();
                }

                // Store old values for logging
                var oldFullName = user.FullName;
                var oldEmail = user.Email;
                var oldRoles = await _userManager.GetRolesAsync(user);

                // Update user info
                user.FullName = EditInput.FullName;
                user.Email = EditInput.Email.Trim().ToLower();

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = "Update failed: " + errors;
                    await LoadDataAsync();
                    return Page();
                }

                // Update role if changed
                if (!oldRoles.Contains(EditInput.Role))
                {
                    await _userManager.RemoveFromRolesAsync(user, oldRoles);
                    await _userManager.AddToRoleAsync(user, EditInput.Role);
                }

                // --- Log edit action ---
                var log = new ActivityLog
                {
                    UserId = User.Identity.Name ?? "System",
                    Action = "Updated",
                    Entity = "User",
                    Details = $"Edited admin: {oldFullName} ({oldEmail}) -> {EditInput.FullName} ({EditInput.Email}), Role: {string.Join(",", oldRoles)} -> {EditInput.Role}",
                    Timestamp = DateTime.UtcNow
                };

                _context.ActivityLog.Add(log);
                await _context.SaveChangesAsync();
                // ----------------------

                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                Console.WriteLine("Exception: " + ex);
                await LoadDataAsync();
                return Page();
            }
        }

        //Delete
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToPage();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return RedirectToPage();

            await _userManager.DeleteAsync(user);

            // --- Log delete action ---
            var log = new ActivityLog
            {
                UserId = User.Identity.Name ?? "System",
                Action = "Deleted",
                Entity = "User",
                Details = $"Deleted admin: {user.FullName} ({user.Email})",
                Timestamp = DateTime.UtcNow
            };
            _context.ActivityLog.Add(log);
            await _context.SaveChangesAsync();
            // --------------------------

            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToPage();
        }

        public class EditAdminInput
        {
            public string Id { get; set; } = string.Empty;

            [Required(ErrorMessage = "Full Name is required")]
            [StringLength(50, MinimumLength = 5, ErrorMessage = "Full Name must be between 5 and 50 characters.")]
            [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)+$", ErrorMessage = "Full Name must contain at least two words and letters only.")]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address")]
            [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$",
     ErrorMessage = "Only Gmail accounts (example@gmail.com) are allowed.")]
            public string Email { get; set; }

            [Required]
            public string Role { get; set; } = string.Empty;
        }

        public class NewAdminInput
        {
            [Required(ErrorMessage = "Full Name is required")]
            [StringLength(50, MinimumLength = 5, ErrorMessage = "Full Name must be between 5 and 50 characters.")]
            [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)+$", ErrorMessage = "Full Name must contain at least two words and letters only.")]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address")]
            [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$",
    ErrorMessage = "Only Gmail accounts (example@gmail.com) are allowed.")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; } = string.Empty;
        }
    }
}