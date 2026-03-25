using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Pages
{
    [Authorize]
    public class ActivityLogsModel : PageModel
    {
        private readonly SeminarBookingSystemContext _context;

        public ActivityLogsModel(SeminarBookingSystemContext context)
        {
            _context = context;
        }

        public IList<ActivityLog> Logs { get; set; } = new List<ActivityLog>();

        public int TotalActivities => Logs.Count;
        public int CreatedCount => Logs.Count(l => l.Action.ToLower() == "created");
        public int UpdatedCount => Logs.Count(l => l.Action.ToLower() == "updated");
        public int DeletedCount => Logs.Count(l => l.Action.ToLower() == "deleted");

        public async Task OnGetAsync()
        {
            // Load latest 100 logs for example, ordered by Timestamp descending
            Logs = await _context.ActivityLog
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .ToListAsync();
        }
    }
}