using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SeminarBookingSystem.Pages
{
  [Authorize]
  public class BookingHistoryModel : PageModel
  {
    public void OnGet()
    {
    }
  }
}
