using HotelBooking.Domain.Enums;

namespace HotelBooking.Application.Dtos;

public class BookingListItemDto
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string UserEmail { get; set; } = "";
    public string HotelName { get; set; } = "";
    public string RoomName { get; set; } = "";
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}