namespace HotelBooking.Web.Models;

public class HotelItemVm
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string City { get; init; } = "";
    public string Address { get; init; } = "";
    public string? Description { get; init; }
    public IReadOnlyList<RoomItemVm> Rooms { get; init; } = [];
}