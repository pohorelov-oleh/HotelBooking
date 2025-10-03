using HotelBooking.Application.Dtos;
using HotelBooking.Application.Services;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db) => _db = db;

    public async Task<int> RegisterAsync(string email, string password, string? fullName, bool makeAdmin, CancellationToken ct)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == email, ct);
        if (exists) throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            Email = email,
            PasswordHash = PasswordHasher.Hash(password),
            FullName = fullName,
            Role = makeAdmin ? UserRole.Admin : UserRole.Client,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task<UserDto?> LoginAsync(string email, string password, CancellationToken ct)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user is null) return null;
        if (!PasswordHasher.Verify(password, user.PasswordHash)) return null;

        return new UserDto(user.Id, user.Email, user.FullName, user.Role);
    }
}