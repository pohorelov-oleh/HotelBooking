using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}