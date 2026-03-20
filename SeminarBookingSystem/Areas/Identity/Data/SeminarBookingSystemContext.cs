using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Models;

namespace SeminarBookingSystem.Data;

public class SeminarBookingSystemContext : IdentityDbContext<AdminUser>
{
    public SeminarBookingSystemContext(DbContextOptions<SeminarBookingSystemContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

public DbSet<SeminarBookingSystem.Models.Seminar> Seminar { get; set; } = default!;

public DbSet<SeminarBookingSystem.Models.Participant> Participant { get; set; } = default!;
}
