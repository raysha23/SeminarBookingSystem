using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Areas.Identity.Data;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SeminarBookingSystemContextConnection")
    ?? throw new InvalidOperationException("Connection string not found");

// Add DbContext
builder.Services.AddDbContext<SeminarBookingSystemContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity
builder.Services.AddIdentity<AdminUser, IdentityRole>(options =>
{
  options.SignIn.RequireConfirmedAccount = false; // Set false for testing login
})
.AddEntityFrameworkStores<SeminarBookingSystemContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
  options.LoginPath = "/Login";             // Redirect to login if unauthorized
  options.AccessDeniedPath = "/AccessDenied"; // Optional: for forbidden pages
});

// Add Razor Pages
builder.Services.AddRazorPages(options =>
{
  options.Conventions.AuthorizeFolder("/");           // Protect all pages
  options.Conventions.AllowAnonymousToPage("/Login"); // Allow login page
});

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication & Authorization
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();

// Force login if not authenticated (extra safety)
app.Use(async (context, next) =>
{
  // If user is not logged in and not already at /Login, redirect to /Login
  if (!context.User.Identity.IsAuthenticated &&
      !context.Request.Path.StartsWithSegments("/Login"))
  {
    context.Response.Redirect("/Login");
    return;
  }

  // If user is logged in and is at root, redirect to dashboard
  if (context.User.Identity.IsAuthenticated && context.Request.Path == "/")
  {
    context.Response.Redirect("/Dashboard");
    return;
  }

  await next();
});

// Map Razor Pages
app.MapRazorPages();

// Seed roles/users
using (var scope = app.Services.CreateScope())
{
  var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AdminUser>>();
  var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
  var context = scope.ServiceProvider.GetRequiredService<SeminarBookingSystemContext>();
  await DbInitializer.SeedData(userManager, roleManager, context);
}

app.Run();