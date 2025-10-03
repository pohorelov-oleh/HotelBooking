using HotelBooking.Application.Dtos;

namespace HotelBooking.Application.Services;

public interface IAdminHotelService
{
    Task<int> CreateHotelAsync(string name, string city, string address, string? description, CancellationToken ct);
    Task UpdateHotelAsync(int id, string name, string city, string address, string? description, CancellationToken ct);
    Task DeleteHotelAsync(int id, CancellationToken ct);
    Task<HotelDto?> GetHotelAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<HotelDto>> GetHotelsAsync(CancellationToken ct);

    Task<int> CreateRoomAsync(int hotelId, string name, int capacity, decimal pricePerNight, CancellationToken ct);
    Task UpdateRoomAsync(int id, string name, int capacity, decimal pricePerNight, bool isActive, CancellationToken ct);
    Task DeleteRoomAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<RoomDto>> GetRoomsByHotelAsync(int hotelId, CancellationToken ct);
}