using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Pages.Seminars
{
    public class CreateModel : PageModel
    {
        private readonly SeminarBookingSystem.Data.SeminarBookingSystemContext _context;

        public CreateModel(SeminarBookingSystem.Data.SeminarBookingSystemContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Seminar Seminar { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Seminar.Add(Seminar);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
