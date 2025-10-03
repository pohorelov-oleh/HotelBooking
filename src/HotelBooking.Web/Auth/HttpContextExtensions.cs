using System.Security.Claims;

namespace HotelBooking.Web.Auth;
public static class HttpContextExtensions
{
    public static int? GetUserId(this ClaimsPrincipal user)
        => int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}