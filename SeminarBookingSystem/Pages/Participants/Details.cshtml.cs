using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Pages.Participants
{
    public class DetailsModel : PageModel
    {
        private readonly SeminarBookingSystem.Data.SeminarBookingSystemContext _context;

        public DetailsModel(SeminarBookingSystem.Data.SeminarBookingSystemContext context)
        {
            _context = context;
        }

        public Participant Participant { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participant.FirstOrDefaultAsync(m => m.Id == id);
            if (participant == null)
            {
                return NotFound();
            }
            else
            {
                Participant = participant;
            }
            return Page();
        }
    }
}
