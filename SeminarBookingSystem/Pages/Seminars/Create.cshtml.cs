using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace SeminarBookingSystem.Pages.Seminars
{
    [Authorize] // optional: only authorized admins can create
    public class CreateModel : PageModel
    {
        private readonly SeminarBookingSystemContext _context;

        public CreateModel(SeminarBookingSystemContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Seminar Seminar { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Parse Time string (HH:mm) to TimeSpan
                TimeSpan seminarTime = TimeSpan.Parse(Seminar.Time);
                var seminarDateTime = Seminar.Date.Date + seminarTime;

                if (seminarDateTime > DateTime.UtcNow)
                    Seminar.Status = "Upcoming";
                else if (seminarDateTime <= DateTime.UtcNow && seminarDateTime.AddHours(1) > DateTime.UtcNow)
                    Seminar.Status = "Ongoing";
                else
                    Seminar.Status = "Completed";

                // Add seminar to database
                _context.Seminar.Add(Seminar);
                await _context.SaveChangesAsync();

                // --- Add activity log ---
                var log = new ActivityLog
                {
                    UserId = User.Identity?.Name ?? "System",  // current logged-in admin
                    Action = "Created",
                    Entity = "Seminar",
                    Details = $"Created seminar: {Seminar.SeminarTitle} (ID: {Seminar.Id})",
                    Timestamp = DateTime.UtcNow
                };
                _context.ActivityLog.Add(log);
                await _context.SaveChangesAsync();
                // -----------------------

                TempData["SuccessMessage"] = "Seminar created successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Log the error as activity log
                var errorLog = new ActivityLog
                {
                    UserId = User.Identity?.Name ?? "System",
                    Action = "Error",
                    Entity = "Seminar",
                    Details = $"Failed to create seminar {Seminar?.SeminarTitle}. Exception: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
                _context.ActivityLog.Add(errorLog);
                await _context.SaveChangesAsync();

                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return Page();
            }
        }
    }
}