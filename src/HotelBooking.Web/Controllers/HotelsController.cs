using global::HotelBooking.Application.Services;
using global::HotelBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Search(string? city, CancellationToken ct)
    {
        var vm = new SearchRoomsVm { City = city };
        vm.CityOptions = (await _hotels.GetCitiesAsync(ct)).ToList();
        vm.CapacityOptions = !string.IsNullOrWhiteSpace(city)
            ? (await _hotels.GetCapacityOptionsAsync(city!, null, null, ct)).ToList()
            : new List<int>();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Search(SearchRoomsVm vm, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (string.IsNullOrWhiteSpace(vm.City) || vm.CheckIn is null || vm.CheckOut is null || vm.Capacity is null)
        {
            ModelState.AddModelError("", "Specify city, dates and capacity.");
            vm.CityOptions = (await _hotels.GetCitiesAsync(ct)).ToList();
            vm.CapacityOptions = string.IsNullOrWhiteSpace(vm.City)
                ? new List<int>()
                : (await _hotels.GetCapacityOptionsAsync(vm.City!, vm.CheckIn, vm.CheckOut, ct)).ToList();
            return View(vm);
        }

        if (vm.CheckIn.Value < today)
        {
            TempData["error"] = $"Check-in cannot be earlier than {today:yyyy-MM-dd}.";
            vm.CityOptions = (await _hotels.GetCitiesAsync(ct)).ToList();
            vm.CapacityOptions = (await _hotels.GetCapacityOptionsAsync(vm.City!, vm.CheckIn, vm.CheckOut, ct)).ToList();
            return View(vm);
        }
        if (vm.CheckOut.Value <= vm.CheckIn.Value)
        {
            TempData["error"] = "Check-out must be after check-in.";
            vm.CityOptions = (await _hotels.GetCitiesAsync(ct)).ToList();
            vm.CapacityOptions = (await _hotels.GetCapacityOptionsAsync(vm.City!, vm.CheckIn, vm.CheckOut, ct)).ToList();
            return View(vm);
        }

        var req = new Application.Dtos.SearchRoomsRequest(vm.City!, vm.CheckIn.Value, vm.CheckOut.Value, vm.Capacity.Value);
        var rooms = await _hotels.SearchRoomsAsync(req, ct);

        vm.Results = rooms.Select(r => new RoomItemVm { Id = r.Id, Name = r.Name, Capacity = r.Capacity, PricePerNight = r.PricePerNight }).ToList();
        vm.CityOptions = (await _hotels.GetCitiesAsync(ct)).ToList();
        vm.CapacityOptions = (await _hotels.GetCapacityOptionsAsync(vm.City!, vm.CheckIn, vm.CheckOut, ct)).ToList();
        return View(vm);
    }

    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> CityOptions(CancellationToken ct)
    {
        var cities = await _hotels.GetCitiesAsync(ct);
        return Json(cities);
    }

    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> CapacityOptions(string city, DateOnly? checkIn, DateOnly? checkOut, CancellationToken ct)
    {
        var caps = await _hotels.GetCapacityOptionsAsync(city, checkIn, checkOut, ct);
        return Json(caps);
    }
}