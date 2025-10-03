namespace HotelBooking.Web.Models;

public class CreateBookingVm
{
    public int RoomId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
}