using HotelBooking.Application.Dtos;
namespace HotelBooking.Application.Services;

public interface IHotelService
{
    Task<IReadOnlyList<HotelDto>> GetHotelsAsync(CancellationToken ct);
    Task<IReadOnlyList<RoomDto>> SearchRoomsAsync(SearchRoomsRequest req, CancellationToken ct);
}