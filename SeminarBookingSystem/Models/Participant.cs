namespace SeminarBookingSystem.Models
{
  public class Participant
  {
    public int Id { get; set; }
    public Seminar? SeminarBooking { get; set; } 
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public bool IsDeleted { get; set; } // for soft delete


  }
}

