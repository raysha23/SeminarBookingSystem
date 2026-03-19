using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SeminarBookingSystem.Pages
{
  [Authorize]
  public class ActivityLogsModel : PageModel
  {
    public void OnGet()
    {
    }
  }
}
