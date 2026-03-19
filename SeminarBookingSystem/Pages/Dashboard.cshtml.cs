using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;

namespace SeminarBookingSystem.Pages
{
  [Authorize] // Optional, global auth already covers this
  public class DashboardModel : PageModel
  {
    private readonly ILogger<DashboardModel> _logger;

    public DashboardModel(ILogger<DashboardModel> logger)
    {
      _logger = logger;
    }

    public void OnGet()
    {
      // Optional: show username
      var username = User.Identity.Name;
      Console.WriteLine(username);
      // You can use User.Identity.Name to show the logged-in user
    }
  }
}