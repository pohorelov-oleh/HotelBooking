using HotelBooking.Application.Services;
using HotelBooking.Infrastructure;
using HotelBooking.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RoomsController : Controller
{
    private readonly IAdminHotelService _svc;
    private readonly AppDbContext _db;

    public RoomsController(IAdminHotelService svc, AppDbContext db)
    {
        _svc = svc;
        _db = db;
    }

    [HttpGet]
    public IActionResult Create(int hotelId) => View(new RoomEditVm { HotelId = hotelId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoomEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        await _svc.CreateRoomAsync(vm.HotelId, vm.Name, vm.Capacity, vm.PricePerNight, ct);
        return RedirectToAction("Rooms", "Hotels", new { area = "Admin", id = vm.HotelId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, int hotelId, CancellationToken ct)
    {
        var room = await _db.Rooms.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (room is null) return NotFound();

        var vm = new RoomEditVm
        {
            Id = room.Id,
            HotelId = room.HotelId,
            Name = room.Name,
            Capacity = room.Capacity,
            PricePerNight = room.PricePerNight,
            IsActive = room.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoomEditVm vm, CancellationToken ct)
    {
        if (!vm.Id.HasValue) return BadRequest();
        if (!ModelState.IsValid) return View(vm);
        await _svc.UpdateRoomAsync(vm.Id.Value, vm.Name, vm.Capacity, vm.PricePerNight, vm.IsActive, ct);
        return RedirectToAction("Rooms", "Hotels", new { area = "Admin", id = vm.HotelId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int hotelId, CancellationToken ct)
    {
        try
        {
            await _svc.DeleteRoomAsync(id, ct);
            return RedirectToAction("Rooms", "Hotels", new { area = "Admin", id = hotelId });
        }
        catch (InvalidOperationException ex)
        {
            TempData["ok"] = ex.Message;
            return RedirectToAction("Rooms", "Hotels", new { area = "Admin", id = hotelId });
        }
    }
}