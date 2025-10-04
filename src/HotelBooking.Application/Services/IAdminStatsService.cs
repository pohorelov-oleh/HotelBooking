using HotelBooking.Application.Dtos;

namespace HotelBooking.Application.Services;

public interface IAdminStatsService
{
    Task<decimal> GetCurrentWeekRevenueAsync(CancellationToken ct);
    Task<IReadOnlyList<BookingListItemDto>> GetLatestBookingsAsync(int count, CancellationToken ct);
    Task<IReadOnlyList<MonthlyRevenuePoint>> GetLast3MonthsRevenueByCheckInAsync(CancellationToken ct); 
}