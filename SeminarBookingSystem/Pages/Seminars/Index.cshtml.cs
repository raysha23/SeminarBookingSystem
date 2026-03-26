using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Pages.Seminars
{
    public class IndexModel : PageModel
    {
        private readonly SeminarBookingSystemContext _context;

        public IndexModel(SeminarBookingSystemContext context)
        {
            _context = context;
        }

        public IList<Seminar> Seminar { get; set; } = default!;

        [TempData]
        public string SuccessMessage { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task OnGetAsync(bool showDeleted = false)
        {
            var query = _context.Seminar.AsQueryable();

            if (showDeleted)
                query = query.Where(s => s.IsDeleted);
            else
                query = query.Where(s => !s.IsDeleted);

            // Get all seminars (newest first)
            var list = await query.OrderByDescending(s => s.Id).ToListAsync();

            // --- Automatically update status in DB ---
            foreach (var s in list)
            {
                var newStatus = GetSeminarStatus(s.Date, s.Time);
                if (s.Status != newStatus)
                {
                    s.Status = newStatus;
                    _context.Attach(s).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
            // ----------------------------------------

            // Map to view model
            Seminar = list;
        }

        // Calculate seminar status dynamically
        private string GetSeminarStatus(DateTime seminarDate, string seminarTime, double durationHours = 1)
        {
            if (!TimeSpan.TryParse(seminarTime, out var timeSpan))
                timeSpan = TimeSpan.Zero;

            var seminarDateTime = seminarDate.Date + timeSpan;
            var endTime = seminarDateTime.AddHours(durationHours);
            var now = DateTime.Now; // server local time

            if (now < seminarDateTime)
                return "Upcoming";
            else if (now >= seminarDateTime && now <= endTime)
                return "Ongoing";
            else
                return "Completed";
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var seminar = await _context.Seminar.FindAsync(id);
            if (seminar == null)
            {
                ErrorMessage = "Seminar not found!";
                return RedirectToPage();
            }

            seminar.IsDeleted = true;
            _context.Attach(seminar).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Activity log
            _context.ActivityLog.Add(new ActivityLog
            {
                UserId = User.Identity?.Name ?? "System",
                Action = "Deleted",
                Entity = "Seminar",
                Details = $"Soft deleted seminar: {seminar.SeminarTitle} (ID: {seminar.Id})",
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            SuccessMessage = $"Seminar '{seminar.SeminarTitle}' deleted successfully!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRestoreAsync(int id)
        {
            var seminar = await _context.Seminar.FindAsync(id);
            if (seminar != null && seminar.IsDeleted)
            {
                seminar.IsDeleted = false;
                _context.Attach(seminar).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Activity log
                _context.ActivityLog.Add(new ActivityLog
                {
                    UserId = User.Identity?.Name ?? "System",
                    Action = "Restored",
                    Entity = "Seminar",
                    Details = $"Restored seminar: {seminar.SeminarTitle} (ID: {seminar.Id})",
                    Timestamp = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();

                SuccessMessage = $"Seminar '{seminar.SeminarTitle}' restored successfully!";
            }
            else
            {
                ErrorMessage = "Seminar not found or already active!";
            }

            return new JsonResult(new { success = true });
        }
    }
}   