namespace SeminarBookingSystem.Models
{
  public class Booking
  {
    public int Id { get; set; }
    public int SeminarId { get; set; }
    public Seminar? Seminar { get; set; } // Navigation property
    public int ParticipantId { get; set; }
    public Participant? Participant { get; set; } // Navigation property
    public DateTime BookingDate { get; set; }
    public bool IsDeleted { get; set; } // for soft delete
  }
}
