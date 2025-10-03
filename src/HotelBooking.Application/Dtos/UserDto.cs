using HotelBooking.Domain.Enums;
namespace HotelBooking.Application.Dtos;

public record UserDto(int Id, string Email, string? FullName, UserRole Role);