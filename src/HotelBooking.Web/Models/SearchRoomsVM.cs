namespace HotelBooking.Web.Models;

public class SearchRoomsVm
{
    public string City { get; set; } = "";
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }
    public int? Capacity { get; set; }
    public IReadOnlyList<RoomItemVm> Results { get; set; } = [];

    public List<string> CityOptions { get; set; } = new();
    public List<int> CapacityOptions { get; set; } = new();
}