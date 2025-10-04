using HotelBooking.Application.Dtos;

namespace HotelBooking.Web.Areas.Admin.Models;

public class DashboardVM
{
    public decimal WeeklyRevenue { get; set; }
    public IReadOnlyList<BookingListItemDto> Latest { get; set; } = Array.Empty<BookingListItemDto>();
    public IReadOnlyList<MonthlyRevenuePoint> Last3Months { get; set; } = Array.Empty<MonthlyRevenuePoint>();
}