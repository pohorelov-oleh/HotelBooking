using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Infrastructure;

public static class DataSeeder
{
    // Seeds only one admin if none exists
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await ctx.Database.EnsureCreatedAsync();

        var hasAdmin = await ctx.Users.AnyAsync(u => u.Role == UserRole.Admin);
        if (hasAdmin) return;

        // Default admin values
        var email = "admin@hotel.com";
        var password = "Admin#12345";
        var user = new User
        {
            Email = email,
            PasswordHash = PasswordHasher.Hash(password),
            FullName = "Administrator",
            Role = UserRole.Admin,
            CreatedAtUtc = DateTime.UtcNow
        };

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
    }
}