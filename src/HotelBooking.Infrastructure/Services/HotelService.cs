using HotelBooking.Application.Dtos;
using HotelBooking.Application.Services;
using HotelBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure;

public class HotelService : IHotelService
{
    private readonly AppDbContext _db;

    public HotelService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<HotelDto>> GetHotelsAsync(CancellationToken ct)
    {
        return await _db.Hotels
            .AsNoTracking()
            .Select(h => new HotelDto(h.Id, h.Name, h.City, h.Address, h.Description))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<RoomDto>> SearchRoomsAsync(SearchRoomsRequest req, CancellationToken ct)
    {
        var rooms = await _db.Rooms
            .AsNoTracking()
            .Where(r => r.IsActive
                        && r.Capacity >= req.Capacity
                        && r.Hotel!.City == req.City)
            .Where(r => !_db.Bookings.Any(b =>
                b.RoomId == r.Id &&
                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed) &&
                b.CheckIn < req.CheckOut &&
                req.CheckIn < b.CheckOut))
            .Select(r => new RoomDto(r.Id, r.HotelId, r.Name, r.Capacity, r.PricePerNight, r.IsActive))
            .ToListAsync(ct);

        return rooms;
    }

    public async Task<IReadOnlyList<string>> GetCitiesAsync(CancellationToken ct)
    {
        return await _db.Hotels
            .AsNoTracking()
            .Select(h => h.City)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<int>> GetCapacityOptionsAsync(string city, DateOnly? checkIn, DateOnly? checkOut, CancellationToken ct)
    {
        var q = _db.Rooms
            .AsNoTracking()
            .Where(r => r.IsActive && r.Hotel!.City == city);

        if (checkIn.HasValue && checkOut.HasValue)
        {
            var ci = checkIn.Value;
            var co = checkOut.Value;

            q = q.Where(r => !_db.Bookings.Any(b =>
                b.RoomId == r.Id &&
                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed) &&
                b.CheckIn < co &&
                ci < b.CheckOut));
        }

        return await q
            .Select(r => r.Capacity)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);
    }
}