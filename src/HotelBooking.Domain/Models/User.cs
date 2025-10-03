using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? FullName { get; set; }
    public UserRole Role { get; set; } = UserRole.Client;
    public DateTime CreatedAtUtc { get; set; }
}