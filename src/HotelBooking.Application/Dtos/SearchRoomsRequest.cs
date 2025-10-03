namespace HotelBooking.Application.Dtos;

public record SearchRoomsRequest(string City, DateOnly CheckIn, DateOnly CheckOut, int Capacity);