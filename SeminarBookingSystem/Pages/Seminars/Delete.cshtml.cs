using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Pages.Seminars
{
    public class DeleteModel : PageModel
    {
        private readonly SeminarBookingSystemContext _context;

        public DeleteModel(SeminarBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Seminar Seminar { get; set; } = default!;

        // Show seminar details before archiving
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var seminar = await _context.Seminar.FirstOrDefaultAsync(m => m.Id == id);
            if (seminar == null) return NotFound();

            Seminar = seminar;
            return Page();
        }

        // Soft delete: mark as IsDeleted = true
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var seminar = await _context.Seminar.FindAsync(id);
            if (seminar == null) return NotFound();

            // Soft delete
            seminar.IsDeleted = true;

            _context.Attach(seminar).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}