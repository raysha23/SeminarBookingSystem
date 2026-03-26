using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SeminarBookingSystem.Models
{
    public class AdminUser : IdentityUser
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)+$",
          ErrorMessage = "Full Name must contain at least two words and letters only.")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
