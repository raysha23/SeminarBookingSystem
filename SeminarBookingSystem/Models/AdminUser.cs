using Microsoft.AspNetCore.Identity;

namespace SeminarBookingSystem.Models
{
  public class AdminUser : IdentityUser
  {
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
  }
}
