using HotelBooking.Application.Dtos;
using HotelBooking.Application.Services;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure;

public class AdminHotelService : IAdminHotelService
{
    private readonly AppDbContext _db;
    public AdminHotelService(AppDbContext db) => _db = db;

    public async Task<int> CreateHotelAsync(string name, string city, string address, string? description, CancellationToken ct)
    {
        var h = new Hotel { Name = name, City = city, Address = address, Description = description };
        _db.Hotels.Add(h);
        await _db.SaveChangesAsync(ct);
        return h.Id;
    }

    public async Task UpdateHotelAsync(int id, string name, string city, string address, string? description, CancellationToken ct)
    {
        var h = await _db.Hotels.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new InvalidOperationException("Hotel not found");
        h.Name = name; h.City = city; h.Address = address; h.Description = description;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteHotelAsync(int id, CancellationToken ct)
    {
        var h = await _db.Hotels.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (h is null) return;

        var hasRooms = await _db.Rooms.AnyAsync(r => r.HotelId == id, ct);
        if (hasRooms)
            throw new InvalidOperationException("Unable to delete hotel: Please delete all rooms in this hotel first.");

        _db.Hotels.Remove(h);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<HotelDto?> GetHotelAsync(int id, CancellationToken ct)
    {
        return await _db.Hotels.AsNoTracking()
            .Where(h => h.Id == id)
            .Select(h => new HotelDto(h.Id, h.Name, h.City, h.Address, h.Description))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<HotelDto>> GetHotelsAsync(CancellationToken ct)
    {
        return await _db.Hotels.AsNoTracking()
            .OrderBy(h => h.Name)
            .Select(h => new HotelDto(h.Id, h.Name, h.City, h.Address, h.Description))
            .ToListAsync(ct);
    }

    public async Task<int> CreateRoomAsync(int hotelId, string name, int capacity, decimal pricePerNight, CancellationToken ct)
    {
        var exists = await _db.Hotels.AnyAsync(h => h.Id == hotelId, ct);
        if (!exists) throw new InvalidOperationException("Hotel not found");
        var r = new Room { HotelId = hotelId, Name = name, Capacity = capacity, PricePerNight = pricePerNight, IsActive = true };
        _db.Rooms.Add(r);
        await _db.SaveChangesAsync(ct);
        return r.Id;
    }

    public async Task UpdateRoomAsync(int id, string name, int capacity, decimal pricePerNight, bool isActive, CancellationToken ct)
    {
        var r = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new InvalidOperationException("Room not found");
        r.Name = name; r.Capacity = capacity; r.PricePerNight = pricePerNight; r.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteRoomAsync(int id, CancellationToken ct)
    {
        var r = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (r is null) return;

        var hasBookings = await _db.Bookings.AnyAsync(b => b.RoomId == id, ct);

        if (hasBookings)
        {
            r.IsActive = false;
            await _db.SaveChangesAsync(ct);
            throw new InvalidOperationException("Room deactivated (soft delete) - has existing bookings.");
        }
        else
        {
            _db.Rooms.Remove(r);
            await _db.SaveChangesAsync(ct);
            throw new InvalidOperationException("Room permanently deleted - no bookings found.");
        }
    }

    public async Task<IReadOnlyList<RoomDto>> GetRoomsByHotelAsync(int hotelId, CancellationToken ct)
    {
        return await _db.Rooms.AsNoTracking()
            .Where(r => r.HotelId == hotelId)
            .OrderBy(r => r.Name)
            .Select(r => new RoomDto(r.Id, r.HotelId, r.Name, r.Capacity, r.PricePerNight, r.IsActive))
            .ToListAsync(ct);
    }
}