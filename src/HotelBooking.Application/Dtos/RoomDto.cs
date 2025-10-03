namespace HotelBooking.Application.Dtos;

public record RoomDto(int Id, int HotelId, string Name, int Capacity, decimal PricePerNight);