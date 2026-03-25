namespace SeminarBookingSystem.Models
{
  public class ActivityLog
  {
    public int Id { get; set; }  // Primary key
    public string UserId { get; set; }  // FK to AdminUser.Id
    public string Action { get; set; }  // e.g., "Login", "Created Seminar", "Deleted Booking"
    public string Entity { get; set; }
    public string Details { get; set; } // Optional: JSON or description
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
  }
}
