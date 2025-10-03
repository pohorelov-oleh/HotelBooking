using HotelBooking.Application.Dtos;
namespace HotelBooking.Application.Services;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(int userId, int roomId, DateOnly checkIn, DateOnly checkOut, string idempotencyKey, CancellationToken ct);
    Task<IReadOnlyList<BookingDto>> GetMyBookingsAsync(int userId, CancellationToken ct);
}