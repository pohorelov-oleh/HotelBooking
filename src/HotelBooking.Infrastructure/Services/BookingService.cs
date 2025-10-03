using HotelBooking.Application.Dtos;
using HotelBooking.Application.Services;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure;

public class BookingService : IBookingService
{
    private readonly AppDbContext _db;

    public BookingService(AppDbContext db) => _db = db;

    public async Task<BookingDto> CreateBookingAsync(int userId, int roomId, DateOnly checkIn, DateOnly checkOut, string idempotencyKey, CancellationToken ct)
    {
        if (checkOut <= checkIn) throw new ArgumentException("Invalid date range.");

        var existing = await _db.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.IdempotencyKey == idempotencyKey, ct);
        if (existing is not null)
            return new BookingDto(existing.Id, existing.RoomId, existing.CheckIn, existing.CheckOut, existing.TotalPrice, existing.Status);

        var room = await _db.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roomId && r.IsActive, ct);
        if (room is null) throw new InvalidOperationException("Room not found or inactive.");

        var overlap = await _db.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.CheckIn < checkOut &&
            checkIn < b.CheckOut, ct);
        if (overlap) throw new InvalidOperationException("Room is not available for the selected dates.");

        var nights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;
        var total = room.PricePerNight * nights;

        var booking = new Booking
        {
            UserId = userId,
            RoomId = roomId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            TotalPrice = total,
            Status = BookingStatus.Pending,
            IdempotencyKey = idempotencyKey,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync(ct);

        return new BookingDto(booking.Id, booking.RoomId, booking.CheckIn, booking.CheckOut, booking.TotalPrice, booking.Status);
    }

    public async Task<IReadOnlyList<BookingDto>> GetMyBookingsAsync(int userId, CancellationToken ct)
    {
        return await _db.Bookings
            .AsNoTracking()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAtUtc)
            .Select(b => new BookingDto(b.Id, b.RoomId, b.CheckIn, b.CheckOut, b.TotalPrice, b.Status))
            .ToListAsync(ct);
    }

    public async Task<bool> CancelBookingAsync(int bookingId, int userId, CancellationToken ct)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId, ct);
        if (booking is null) return false;

        if (booking.Status == BookingStatus.Pending)
            _db.Bookings.Remove(booking);
        else if (booking.Status == BookingStatus.Confirmed)
            booking.Status = BookingStatus.Cancelled;
        else
            return false;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus, CancellationToken ct)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, ct);
        if (booking is null) return false;

        switch (booking.Status)
        {
            case BookingStatus.Pending:
                if (newStatus == BookingStatus.Confirmed || newStatus == BookingStatus.Cancelled)
                    booking.Status = newStatus;
                else
                    return false;
                break;
            case BookingStatus.Confirmed:
                if (newStatus == BookingStatus.Cancelled || newStatus == BookingStatus.Completed)
                    booking.Status = newStatus;
                else
                    return false;
                break;
            case BookingStatus.Cancelled:
            case BookingStatus.Completed:
                return false;
            default:
                return false;
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }
}