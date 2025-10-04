using Dapper;
using HotelBooking.Application.Dtos;
using HotelBooking.Application.Services;
using HotelBooking.Domain.Enums;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace HotelBooking.Infrastructure.Services;

public sealed class DapperAdminStatsService : IAdminStatsService
{
    private readonly string _cs;
    public DapperAdminStatsService(IConfiguration cfg)
    {
        _cs = cfg.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing connection string 'ConnectionStrings:Default'.");
        var b = new MySqlConnector.MySqlConnectionStringBuilder(_cs);
    }

    private static (DateTime from, DateTime to) CurrentWeekLocal()
    {
        var today = DateTime.Today;
        int diff = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var start = today.AddDays(-diff);
        var end = start.AddDays(7);
        return (start, end);
    }

    public async Task<decimal> GetCurrentWeekRevenueAsync(CancellationToken ct)
    {
        var (from, to) = CurrentWeekLocal();
        using var con = new MySqlConnection(_cs);

        var sql = @"
        SELECT COALESCE(SUM(b.TotalPrice), 0)
        FROM Bookings b
        WHERE b.CheckIn >= @from AND b.CheckIn < @to
          AND b.Status IN (@Confirmed, @Completed);";

        return await con.ExecuteScalarAsync<decimal>(new CommandDefinition(
            sql,
            new
            {
                from,
                to,
                Confirmed = (int)BookingStatus.Confirmed,
                Completed = (int)BookingStatus.Completed
            },
            cancellationToken: ct));
    }

    public async Task<IReadOnlyList<BookingListItemDto>> GetLatestBookingsAsync(int count, CancellationToken ct)
    {
        using var con = new MySqlConnection(_cs);

        var sql = @"
            SELECT  b.Id,
                    COALESCE(NULLIF(u.FullName,''), u.Email) AS UserName,
                    u.Email AS UserEmail,
                    h.Name AS HotelName,
                    r.Name AS RoomName,
                    b.CheckIn,
                    b.CheckOut,
                    b.TotalPrice,
                    b.Status,
                    b.CreatedAtUtc
            FROM Bookings b
            LEFT JOIN Users  u ON u.Id = b.UserId
            LEFT JOIN Rooms  r ON r.Id = b.RoomId
            LEFT JOIN Hotels h ON h.Id = r.HotelId
            ORDER BY b.CreatedAtUtc DESC
            LIMIT @take;";

        var rows = await con.QueryAsync<BookingListItemDto>(new CommandDefinition(
            sql, new { take = count }, cancellationToken: ct));

        return rows.AsList();
    }

    public async Task<IReadOnlyList<MonthlyRevenuePoint>> GetLast3MonthsRevenueByCheckInAsync(CancellationToken ct)
    {
        using var con = new MySqlConnection(_cs);

        var today = DateTime.Today;
        var start = new DateTime(today.Year, today.Month, 1).AddMonths(-2);
        var end = new DateTime(today.Year, today.Month, 1).AddMonths(1);

        var sql = @"
            SELECT YEAR(b.CheckIn)  AS Year,
                   MONTH(b.CheckIn) AS Month,
                   COALESCE(SUM(b.TotalPrice), 0) AS Revenue
            FROM Bookings b
            WHERE b.CheckIn >= @start AND b.CheckIn < @end
              AND b.Status IN (@Confirmed, @Completed)
            GROUP BY YEAR(b.CheckIn), MONTH(b.CheckIn)
            ORDER BY Year, Month;";

        var data = (await con.QueryAsync<MonthlyRevenuePoint>(new CommandDefinition(
            sql,
            new
            {
                start,
                end,
                Confirmed = (int)BookingStatus.Confirmed,
                Completed = (int)BookingStatus.Completed
            },
            cancellationToken: ct))).ToList();

        var result = new List<MonthlyRevenuePoint>();
        for (int i = 0; i < 3; i++)
        {
            var m = start.AddMonths(i);
            var hit = data.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
            result.Add(hit ?? new MonthlyRevenuePoint { Year = m.Year, Month = m.Month, Revenue = 0m });
        }

        return result;
    }
}