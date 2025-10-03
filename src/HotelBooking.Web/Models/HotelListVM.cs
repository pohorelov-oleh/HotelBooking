namespace HotelBooking.Web.Models;

public class HotelListVm
{
    public IReadOnlyList<HotelItemVm> Hotels { get; init; } = [];
}