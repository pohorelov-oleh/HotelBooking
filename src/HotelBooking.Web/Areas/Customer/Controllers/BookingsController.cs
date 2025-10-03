using System.Security.Claims;
using HotelBooking.Application.Services;
using HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class BookingsController : Controller
{
    private readonly IBookingService _booking;

    public BookingsController(IBookingService booking) => _booking = booking;

    [HttpGet]
    public IActionResult Create(int roomId, DateOnly checkIn, DateOnly checkOut)
    {
        return View(new CreateBookingVm { RoomId = roomId, CheckIn = checkIn, CheckOut = checkOut });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingVm vm, CancellationToken ct)
    {
        if (vm.CheckOut <= vm.CheckIn)
        {
            ModelState.AddModelError("", "Invalid date range.");
            return View(vm);
        }

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Challenge(); // redirect to /account/login

        var idem = $"{userId}:{vm.RoomId}:{vm.CheckIn}:{vm.CheckOut}:{DateTime.UtcNow.Ticks}";

        try
        {
            var dto = await _booking.CreateBookingAsync(userId, vm.RoomId, vm.CheckIn, vm.CheckOut, idem, ct);
            TempData["ok"] = $"Booking #{dto.Id} created.";
            return RedirectToAction("My", "Bookings");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Challenge();

        var success = await _booking.CancelBookingAsync(id, userId, ct);
        if (success)
            TempData["ok"] = "Booking cancelled.";
        else
            TempData["error"] = "Cannot cancel this booking.";

        return RedirectToAction(nameof(My));
    }

    [HttpGet]
    public async Task<IActionResult> My(CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Challenge();

        var list = await _booking.GetMyBookingsAsync(userId, ct);
        return View(list);
    }
}