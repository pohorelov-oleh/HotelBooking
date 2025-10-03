using HotelBooking.Application.Services;
using HotelBooking.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class HotelsController : Controller
{
    private readonly IAdminHotelService _svc;
    public HotelsController(IAdminHotelService svc) => _svc = svc;

    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await _svc.GetHotelsAsync(ct));

    [HttpGet]
    public IActionResult Create() => View(new HotelEditVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HotelEditVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        var id = await _svc.CreateHotelAsync(vm.Name, vm.City, vm.Address, vm.Description, ct);
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var h = await _svc.GetHotelAsync(id, ct);
        if (h is null) return NotFound();
        return View(new HotelEditVm { Id = h.Id, Name = h.Name, City = h.City, Address = h.Address, Description = h.Description });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HotelEditVm vm, CancellationToken ct)
    {
        if (!vm.Id.HasValue) return BadRequest();
        if (!ModelState.IsValid) return View(vm);
        await _svc.UpdateHotelAsync(vm.Id.Value, vm.Name, vm.City, vm.Address, vm.Description, ct);
        TempData["ok"] = "Saved";
        return RedirectToAction(nameof(Edit), new { id = vm.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _svc.DeleteHotelAsync(id, ct);
            TempData["ok"] = "Hotel deleted.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Rooms(int id, CancellationToken ct)
    {
        ViewBag.HotelId = id;
        ViewBag.Hotel = await _svc.GetHotelAsync(id, ct);
        var rooms = await _svc.GetRoomsByHotelAsync(id, ct);
        return View(rooms);
    }
}