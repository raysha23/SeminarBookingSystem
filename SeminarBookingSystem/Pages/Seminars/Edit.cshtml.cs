using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Pages.Seminars
{
    public class EditModel : PageModel
    {
        private readonly SeminarBookingSystemContext _context;

        public EditModel(SeminarBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Seminar Seminar { get; set; } = default!;

        // TempData for success/error alerts
        [TempData]
        public string SuccessMessage { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var seminar = await _context.Seminar.FirstOrDefaultAsync(m => m.Id == id);
            if (seminar == null) return NotFound();

            Seminar = seminar;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Seminar).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // --- Add Activity Log for Edit/Update ---
                var log = new ActivityLog
                {
                    UserId = User.Identity?.Name ?? "System",
                    Action = "Updated",
                    Entity = "Seminar",
                    Details = $"Updated seminar: {Seminar.SeminarTitle} (ID: {Seminar.Id})",
                    Timestamp = DateTime.UtcNow
                };
                _context.ActivityLog.Add(log);
                await _context.SaveChangesAsync();
                // ----------------------------------------

                // TempData success message
                SuccessMessage = $"Seminar '{Seminar.SeminarTitle}' updated successfully!";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeminarExists(Seminar.Id))
                {
                    ErrorMessage = "Seminar not found!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ErrorMessage = "Concurrency error occurred while updating seminar.";
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Log any other errors
                var errorLog = new ActivityLog
                {
                    UserId = User.Identity?.Name ?? "System",
                    Action = "Error",
                    Entity = "Seminar",
                    Details = $"Failed to update seminar {Seminar.SeminarTitle}. Exception: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
                _context.ActivityLog.Add(errorLog);
                await _context.SaveChangesAsync();

                ErrorMessage = "Unexpected error: " + ex.Message;
                return Page();
            }
        }

        private bool SeminarExists(int id)
        {
            return _context.Seminar.Any(e => e.Id == id);
        }
    }
}