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
    public class IndexModel : PageModel
    {
        private readonly SeminarBookingSystem.Data.SeminarBookingSystemContext _context;

        public IndexModel(SeminarBookingSystem.Data.SeminarBookingSystemContext context)
        {
            _context = context;
        }

        public IList<Participant> Participant { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Participant = await _context.Participant.ToListAsync();
        }
    }
}
