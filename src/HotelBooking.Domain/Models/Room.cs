namespace HotelBooking.Domain.Entities;

public class Room
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string Name { get; set; } = default!;
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsActive { get; set; } = true;

    public Hotel? Hotel { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}