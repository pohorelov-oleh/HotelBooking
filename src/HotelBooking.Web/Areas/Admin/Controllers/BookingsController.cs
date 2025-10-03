using HotelBooking.Application.Services;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BookingsController : Controller
{
    private readonly IBookingService _booking;
    private readonly AppDbContext _db;

    public BookingsController(IBookingService booking, AppDbContext db)
    {
        _booking = booking;
        _db = db;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var bookings = await _db.Bookings
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAtUtc)
            .Select(b => new
            {
                b.Id,
                b.UserId,
                b.RoomId,
                b.CheckIn,
                b.CheckOut,
                b.TotalPrice,
                b.Status,
                b.CreatedAtUtc
            })
            .ToListAsync(ct);

        return View(bookings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, BookingStatus status, CancellationToken ct)
    {
        var success = await _booking.UpdateBookingStatusAsync(id, status, ct);
        if (success)
        {
            TempData["ok"] = $"Booking status updated to {status}.";
        }
        else
        {
            TempData["error"] = "Cannot update booking status.";
        }
        return RedirectToAction(nameof(Index));
    }
}