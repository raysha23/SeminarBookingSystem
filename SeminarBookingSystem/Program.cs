using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Areas.Identity.Data;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ Connection string
var connectionString = builder.Configuration.GetConnectionString("SeminarBookingSystemContextConnection")
    ?? throw new InvalidOperationException("Connection string not found");

// ✅ Add DbContext
builder.Services.AddDbContext<SeminarBookingSystemContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Add Identity with Roles
builder.Services.AddIdentity<AdminUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // set false for testing
})
.AddEntityFrameworkStores<SeminarBookingSystemContext>()
.AddDefaultTokenProviders();

// ✅ Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";

    // ⏱ For testing Remember Me (1 minute)
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    options.SlidingExpiration = true;
});

// ✅ Register IEmailSender (for forgot password)
builder.Services.AddTransient<IEmailSender, EmailSender>();

// ✅ Add Razor Pages
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");           // Protect all pages
    options.Conventions.AllowAnonymousToPage("/Login"); // Allow login
    options.Conventions.AllowAnonymousToPage("/Identity/Account/ForgotPassword"); // Allow forgot password
});

// ✅ Build app
var app = builder.Build();

// ✅ Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Force login if not authenticated
app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated &&
        !context.Request.Path.StartsWithSegments("/Login") &&
        !context.Request.Path.StartsWithSegments("/Identity"))
    {
        context.Response.Redirect("/Login");
        return;
    }

    if (context.User.Identity.IsAuthenticated && context.Request.Path == "/")
    {
        context.Response.Redirect("/Dashboard");
        return;
    }

    await next();
});

// ✅ Map Razor Pages
app.MapRazorPages();

// ✅ Seed roles, admin, and seminars
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AdminUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var context = scope.ServiceProvider.GetRequiredService<SeminarBookingSystemContext>();

    await DbInitializer.SeedData(userManager, roleManager, context);
}

app.Run();
