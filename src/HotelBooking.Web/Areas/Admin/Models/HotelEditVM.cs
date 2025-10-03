namespace HotelBooking.Web.Areas.Admin.Models;
public class HotelEditVm
{
    public int? Id { get; set; }
    public string Name { get; set; } = "";
    public string City { get; set; } = "";
    public string Address { get; set; } = "";
    public string? Description { get; set; }
}