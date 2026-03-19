using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SeminarBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SeminarBookingSystem.Areas.Identity.Pages.Account
{
  [AllowAnonymous]
  public class LoginModel : PageModel
  {
    private readonly SignInManager<AdminUser> _signInManager;
    private readonly UserManager<AdminUser> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        SignInManager<AdminUser> signInManager,
        UserManager<AdminUser> userManager,
        ILogger<LoginModel> logger)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

    public string ReturnUrl { get; set; } = string.Empty;

    [TempData]
    public string SuccessMessage { get; set; } = string.Empty;

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    public class InputModel
    {
      [Required]
      [EmailAddress]
      public string Email { get; set; } = string.Empty;

      [Required]
      [DataType(DataType.Password)]
      public string Password { get; set; } = string.Empty;

      [Display(Name = "Remember me?")]
      public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
      ReturnUrl = returnUrl ?? Url.Content("~/");

      // Reset messages on GET
      ErrorMessage = string.Empty;

      // Clear external cookies
      await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

      ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
      returnUrl ??= Url.Content("/Dashboard");

      if (!ModelState.IsValid)
      {
        return Page();
      }

      var user = await _userManager.FindByEmailAsync(Input.Email);
      if (user == null || !user.IsActive || !user.EmailConfirmed)
      {
        ErrorMessage = "Invalid email or password";
        return Page();
      }

      var result = await _signInManager.PasswordSignInAsync(
          Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

      if (result.Succeeded)
      {
        _logger.LogInformation("User logged in.");
        SuccessMessage = "Admin logged in successfully!";
        return LocalRedirect(returnUrl);
      }

      if (result.RequiresTwoFactor)
        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });

      if (result.IsLockedOut)
        return RedirectToPage("./Lockout");

      // Login failed
      ErrorMessage = "Invalid email or password.";
      return Page();
    }
  }
}