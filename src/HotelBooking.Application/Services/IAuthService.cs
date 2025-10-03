using HotelBooking.Application.Dtos;
namespace HotelBooking.Application.Services;

public interface IAuthService
{
    Task<int> RegisterAsync(string email, string password, string? fullName, bool makeAdmin, CancellationToken ct);
    Task<UserDto?> LoginAsync(string email, string password, CancellationToken ct);
}