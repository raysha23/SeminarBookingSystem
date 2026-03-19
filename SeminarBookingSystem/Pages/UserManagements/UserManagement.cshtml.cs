//File Path: SeminarBookingSystem/Pages/UserManagements/UserManagement.cshtml.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SeminarBookingSystem.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SeminarBookingSystem.Pages.UserManagements
{
  [Authorize(Roles = "Super Admin")]
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
      await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostCreateAdminAsync()
    {
      try
      {
        if (!ModelState.IsValid)
        {
          await LoadDataAsync();
          return Page();
        }

        // Check if email exists
        var existingUser = await _userManager.FindByEmailAsync(Input.Email);
        if (existingUser != null)
        {
          ModelState.AddModelError(string.Empty, "Email already exists.");
          await LoadDataAsync();
          return Page();
        }

        // Create user
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
          foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

          await LoadDataAsync();
          return Page();
        }

        // Just assign role (NO NEED to create it)
        await _userManager.AddToRoleAsync(user, Input.Role);

        TempData["Message"] = "Admin created successfully!";
        return RedirectToPage();
      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
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

    //Edit Post
    public async Task<IActionResult> OnPostEditAsync()
    {
      if (!ModelState.IsValid)
      {
        await LoadDataAsync();
        return Page();
      }

      var user = await _userManager.FindByIdAsync(EditInput.Id);

      if (user == null)
        return RedirectToPage();

      // Update basic info
      user.FullName = EditInput.FullName;
      user.Email = EditInput.Email;
      user.UserName = EditInput.Email;

      var updateResult = await _userManager.UpdateAsync(user);

      if (!updateResult.Succeeded)
      {
        foreach (var error in updateResult.Errors)
          ModelState.AddModelError(string.Empty, error.Description);

        await LoadDataAsync();
        return Page();
      }

      // Update role
      var currentRoles = await _userManager.GetRolesAsync(user);
      await _userManager.RemoveFromRolesAsync(user, currentRoles);
      await _userManager.AddToRoleAsync(user, EditInput.Role);

      TempData["Message"] = "User updated successfully!";
      return RedirectToPage();
    }
    public class EditAdminInput
    {
      public string Id { get; set; } = string.Empty;

      [Required]
      [Display(Name = "Full Name")]
      public string FullName { get; set; } = string.Empty;

      [Required]
      [EmailAddress]
      public string Email { get; set; } = string.Empty;

      [Required]
      public string Role { get; set; } = string.Empty;
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