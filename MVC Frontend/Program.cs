using Microsoft.EntityFrameworkCore;
using Web_API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Register shared DbContext from Web API project
builder.Services.AddDbContext<TrainingInstituteDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
