//File Path: SeminarBookingSystem/Pages/UserManagements/UserManagement.cshtml.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SeminarBookingSystem.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace SeminarBookingSystem.Pages.UserManagements
{
  [Authorize(Roles = "Super Admin")]
  public class UserManagementModel : PageModel
  {
    private readonly UserManager<AdminUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public string CurrentLoginedInUserId { get; set; }
    public List<AdminUser> Users
    { get; set; } = new();
    public Dictionary<string, string> UserRoles { get; set; } = new();

    public UserManagementModel(UserManager<AdminUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      _userManager = userManager;
      _roleManager = roleManager;
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

        TempData["SuccessMessage"] = "Admin created successfully!";
        Console.WriteLine("Admin created: " + Input.Email);
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



    //Edit Post
    public async Task<IActionResult> OnPostEditAsync()
    {
      try
      {
        // Console.WriteLine("=== EditInput Received ===");
        // Console.WriteLine("Id: " + EditInput.Id);
        // Console.WriteLine("FullName: " + EditInput.FullName);
        // Console.WriteLine("Email: " + EditInput.Email);
        // Console.WriteLine("Role: " + EditInput.Role);

        if (string.IsNullOrWhiteSpace(EditInput.Role))
        {
          TempData["ErrorMessage"] = "Please select a role!";
          await LoadDataAsync();
          return Page();
        }
        // 1️⃣ Find the user by ID
        var user = await _userManager.FindByIdAsync(EditInput.Id);
        if (user == null)
        {
          TempData["ErrorMessage"] = "User not found!";
          return RedirectToPage();
        }

        // 2️⃣ Update user info
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

        // 3️⃣ Update role (optional, only if changed)
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(EditInput.Role))
        {
          await _userManager.RemoveFromRolesAsync(user, currentRoles);
          await _userManager.AddToRoleAsync(user, EditInput.Role);
        }

        // 4️⃣ Success toast
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

      TempData["Message"] = "User deleted successfully!";
      return RedirectToPage();
    }

    public class EditAdminInput
    {
      public string Id { get; set; } = string.Empty;

      [Required(ErrorMessage = "Full Name is required")]
      [StringLength(50, MinimumLength = 5, ErrorMessage = "Full Name must be between 5 and 50 characters.")]
      [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full Name can only contain letters and spaces.")]
      [Display(Name = "Full Name")]
      public string FullName { get; set; } = string.Empty;

      [Required(ErrorMessage = "Email is required")]
      [EmailAddress(ErrorMessage = "Please enter a valid email address")]
      [Display(Name = "Email Address")]
      public string Email { get; set; }

      [Required]
      public string Role { get; set; } = string.Empty;
    }

    public class NewAdminInput
    {
      [Required(ErrorMessage = "Full Name is required")]
      [StringLength(50, MinimumLength = 5, ErrorMessage = "Full Name must be between 5 and 50 characters.")]
      [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full Name can only contain letters and spaces.")]
      [Display(Name = "Full Name")]
      public string FullName { get; set; } = string.Empty;

      [Required(ErrorMessage = "Email is required")]
      [EmailAddress(ErrorMessage = "Please enter a valid email address")]
      [Display(Name = "Email Address")]
      public string Email { get; set; }

      [Required]
      [Display(Name = "Role")]
      public string Role { get; set; } = string.Empty;
    }
  }
}