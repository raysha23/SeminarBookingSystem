// File Path: SeminarBookingSystem/Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeminarBookingSystem.Areas.Identity.Data;
using SeminarBookingSystem.Data;
using SeminarBookingSystem.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SeminarBookingSystemContextConnection") ?? throw new InvalidOperationException("Connection string 'SeminarBookingSystemContextConnection' not found.");

builder.Services.AddDbContext<SeminarBookingSystemContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<AdminUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<SeminarBookingSystemContext>()
.AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapGet("/", context =>
{
  context.Response.Redirect("/Login");
  return Task.CompletedTask;
});
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AdminUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await DbInitializer.SeedData(userManager, roleManager);
}

app.Run();
