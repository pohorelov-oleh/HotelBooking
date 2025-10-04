using HotelBooking.Application.Services;
using HotelBooking.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IAdminStatsService _stats;
    public DashboardController(IAdminStatsService stats) => _stats = stats;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = new DashboardVM
        {
            WeeklyRevenue = await _stats.GetCurrentWeekRevenueAsync(ct),
            Latest = await _stats.GetLatestBookingsAsync(5, ct),
            Last3Months = await _stats.GetLast3MonthsRevenueByCheckInAsync(ct)
        };
        return View(vm);
    }
}