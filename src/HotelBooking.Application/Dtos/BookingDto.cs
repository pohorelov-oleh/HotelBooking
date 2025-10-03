using HotelBooking.Domain.Enums;
namespace HotelBooking.Application.Dtos;

public record BookingDto(int Id, int RoomId, DateOnly CheckIn, DateOnly CheckOut, decimal TotalPrice, BookingStatus Status);