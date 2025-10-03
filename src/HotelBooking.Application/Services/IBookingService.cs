using HotelBooking.Application.Dtos;
using HotelBooking.Domain.Enums;
namespace HotelBooking.Application.Services;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(int userId, int roomId, DateOnly checkIn, DateOnly checkOut, string idempotencyKey, CancellationToken ct);
    Task<IReadOnlyList<BookingDto>> GetMyBookingsAsync(int userId, CancellationToken ct);
    Task<bool> CancelBookingAsync(int bookingId, int userId, CancellationToken ct);
    Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus, CancellationToken ct);
}