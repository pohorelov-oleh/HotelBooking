using HotelBooking.Domain.Enums;
namespace HotelBooking.Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public string IdempotencyKey { get; set; } = default!;
    public DateTime CreatedAtUtc { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Room? Room { get; set; }
}