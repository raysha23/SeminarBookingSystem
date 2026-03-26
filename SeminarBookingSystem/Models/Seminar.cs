using System.ComponentModel.DataAnnotations;

namespace SeminarBookingSystem.Models
{
    public class Seminar
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Seminar title is required.")]
        [StringLength(100, ErrorMessage = "Seminar title cannot exceed 100 characters.")]
        public string? SeminarTitle { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        public string? Time { get; set; } // optional

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 500, ErrorMessage = "Capacity must be between 1 and 500.")]
        public int Capacity { get; set; }

        public string? Status { get; set; } = "Upcoming";

        [Range(0, int.MaxValue, ErrorMessage = "Current participants cannot be negative.")]
        public int CurrentParticipants { get; set; } // count of bookings

        public bool IsDeleted { get; set; } // for soft delete

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}