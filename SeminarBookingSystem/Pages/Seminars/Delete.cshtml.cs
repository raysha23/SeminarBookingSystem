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
    public class DeleteModel : PageModel
    {
        private readonly SeminarBookingSystem.Data.SeminarBookingSystemContext _context;

        public DeleteModel(SeminarBookingSystem.Data.SeminarBookingSystemContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Seminar Seminar { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seminar = await _context.Seminar.FirstOrDefaultAsync(m => m.Id == id);

            if (seminar == null)
            {
                return NotFound();
            }
            else
            {
                Seminar = seminar;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seminar = await _context.Seminar.FindAsync(id);
            if (seminar != null)
            {
                Seminar = seminar;
                _context.Seminar.Remove(Seminar);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
