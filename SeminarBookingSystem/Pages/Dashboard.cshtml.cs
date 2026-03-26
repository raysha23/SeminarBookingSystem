using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly SeminarBookingSystemContext _context;

        public DashboardModel(SeminarBookingSystemContext context)
        {
            _context = context;
        }
        public Seminar Seminar { get; set; } = default!;
        // Total seminars
        public int TotalSeminars { get; set; }

        // Upcoming seminars list
        public List<Seminar> UpcomingSeminars { get; set; } = new();

        // Total admin activities
        public int TotalActivities { get; set; }

        // Recent activity logs
        public List<ActivityLog> RecentActivities { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Total upcoming seminars
            TotalSeminars = await _context.Seminar
                .Where(s => !s.IsDeleted && s.Date >= DateTime.UtcNow)
                .CountAsync();

            // Upcoming seminars (limit to next 5)
            UpcomingSeminars = await _context.Seminar
                .Where(s => !s.IsDeleted && s.Date >= DateTime.UtcNow)
                .OrderBy(s => s.Date)
                .Take(5)
                .ToListAsync();

            // Total admin activities
            TotalActivities = await _context.ActivityLog.CountAsync();

            // Recent activity logs (latest 5)
            RecentActivities = await _context.ActivityLog
                .OrderByDescending(a => a.Timestamp)
                .Take(5)
                .ToListAsync();
        }
    }
}