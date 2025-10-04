namespace HotelBooking.Application.Dtos;

public sealed class MonthlyRevenuePoint
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Revenue { get; set; }
}