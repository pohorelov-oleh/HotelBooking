namespace HotelBooking.Web.Models;

public class RoomItemVm
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int Capacity { get; init; }
    public decimal PricePerNight { get; init; }
}