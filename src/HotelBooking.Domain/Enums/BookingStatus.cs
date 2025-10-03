namespace HotelBooking.Domain.Enums;

public enum BookingStatus : sbyte
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3
}