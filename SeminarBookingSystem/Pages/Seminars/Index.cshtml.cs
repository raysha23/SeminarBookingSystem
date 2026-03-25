using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // Get seminars: either deleted or not based on checkbox
        public async Task OnGetAsync(bool showDeleted = false)
        {
            if (showDeleted)
            {
                Seminar = await _context.Seminar
                    .Where(s => s.IsDeleted) // only deleted
                    .ToListAsync();
            }
            else
            {
                Seminar = await _context.Seminar
                    .Where(s => !s.IsDeleted) // only active
                    .ToListAsync();
            }
        }

        // Soft delete handler called from the delete modal
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var seminar = await _context.Seminar.FindAsync(id);
            if (seminar == null)
            {
                return NotFound();
            }

            // Mark as deleted instead of removing from database
            seminar.IsDeleted = true;

            _context.Attach(seminar).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Stay on Index page
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
            }

            // Return success for AJAX requests
            return new JsonResult(new { success = true });
        }
    }
}