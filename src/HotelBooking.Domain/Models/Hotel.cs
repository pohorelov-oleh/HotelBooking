namespace HotelBooking.Domain.Entities;

public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string? Description { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}