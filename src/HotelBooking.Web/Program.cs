using HotelBooking.Application.Services;
using HotelBooking.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Default")
         ?? "server=127.0.0.1;port=3306;database=hotelbooking;user=hb_app;password=StrongP@ss123;TreatTinyAsBoolean=true";

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs), o => o.EnableRetryOnFailure()));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

await DataSeeder.SeedAsync(app.Services);

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

app.MapRazorPages();

app.Run();