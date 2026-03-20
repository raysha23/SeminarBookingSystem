namespace SeminarBookingSystem.Models
{
  public class Seminar
  {
    public int Id { get; set; }
    public string? SeminarTitle { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string? Time { get; set; } // optional, or use DateTime
    public string? Location { get; set; }
    public int Capacity { get; set; } // max participants
    public string? Status { get; set; } // e.g. "Upcoming", "Ongoing", "Completed", "Cancelled"
    public int CurrentParticipants { get; set; } // count of bookings
    public bool IsDeleted { get; set; } // for soft delete
    public DateTime CreatedAt { get; set; }
  }
}
