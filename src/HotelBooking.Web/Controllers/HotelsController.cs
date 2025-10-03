using global::HotelBooking.Application.Services;
using global::HotelBooking.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

public class HotelsController : Controller
{
    private readonly IHotelService _hotels;

    public HotelsController(IHotelService hotels) => _hotels = hotels;

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _hotels.GetHotelsAsync(ct);
        var vm = new HotelListVm
        {
            Hotels = items.Select(h => new HotelItemVm
            {
                Id = h.Id,
                Name = h.Name,
                City = h.City,
                Address = h.Address,
                Description = h.Description
            }).ToList()
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult Search() => View(new SearchRoomsVm());

    [HttpPost]
    public async Task<IActionResult> Search(SearchRoomsVm vm, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(vm.City) || vm.CheckIn is null || vm.CheckOut is null || vm.Capacity is null)
        {
            ModelState.AddModelError("", "Specify city, dates and capacity.");
            return View(vm);
        }

        var req = new Application.Dtos.SearchRoomsRequest(vm.City, vm.CheckIn.Value, vm.CheckOut.Value, vm.Capacity.Value);
        var rooms = await _hotels.SearchRoomsAsync(req, ct);

        vm.Results = rooms.Select(r => new RoomItemVm { Id = r.Id, Name = r.Name, Capacity = r.Capacity, PricePerNight = r.PricePerNight }).ToList();
        return View(vm);
    }
}