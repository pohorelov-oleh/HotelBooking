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
        var bookings = await (
            from b in _db.Bookings.AsNoTracking()
            join u in _db.Users.AsNoTracking() on b.UserId equals u.Id into uj
            from u in uj.DefaultIfEmpty()

            join r in _db.Rooms.AsNoTracking() on b.RoomId equals r.Id into rj
            from r in rj.DefaultIfEmpty()

            join h in _db.Hotels.AsNoTracking() on r.HotelId equals h.Id into hj
            from h in hj.DefaultIfEmpty()

            orderby b.CreatedAtUtc descending
            select new
            {
                b.Id,
                b.UserId,
                UserName = u != null ? (u.FullName ?? u.Email) : null,
                UserEmail = u != null ? u.Email : null,

                b.RoomId,
                RoomName = r != null ? r.Name : null,

                HotelId = r != null ? (int?)r.HotelId : null,
                HotelName = h != null ? h.Name : null,

                b.CheckIn,
                b.CheckOut,
                b.TotalPrice,
                b.Status,
                b.CreatedAtUtc
            }
        ).ToListAsync(ct);

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