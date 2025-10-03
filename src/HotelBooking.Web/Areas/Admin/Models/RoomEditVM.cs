namespace HotelBooking.Web.Areas.Admin.Models;
public class RoomEditVm
{
    public int? Id { get; set; }
    public int HotelId { get; set; }
    public string Name { get; set; } = "";
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsActive { get; set; } = true;
}