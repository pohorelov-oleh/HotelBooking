using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await ctx.Database.EnsureCreatedAsync();

        try
        {
            if (!await ctx.Users.AnyAsync(u => u.Role == UserRole.Admin))
            {
                ctx.Users.Add(new User
                {
                    Email = "admin@hotel.com",
                    PasswordHash = PasswordHasher.Hash("Admin#12345"),
                    FullName = "Administrator",
                    Role = UserRole.Admin,
                    CreatedAtUtc = DateTime.UtcNow
                });
                await ctx.SaveChangesAsync();
            }

            var users = new List<User>
            {
                new User
                {
                    Email = "john.doe@email.com",
                    PasswordHash = PasswordHasher.Hash("User123!"),
                    FullName = "John Doe",
                    Role = UserRole.Client,
                    CreatedAtUtc = DateTime.UtcNow
                },
                new User
                {
                    Email = "jane.smith@email.com",
                    PasswordHash = PasswordHasher.Hash("User123!"),
                    FullName = "Jane Smith",
                    Role = UserRole.Client,
                    CreatedAtUtc = DateTime.UtcNow
                }
                ,
                new User
                {
                    Email = "oleh.pohorelov@email.com",
                    PasswordHash = PasswordHasher.Hash("User123!"),
                    FullName = "Oleh Pohorelov",
                    Role = UserRole.Client,
                    CreatedAtUtc = DateTime.UtcNow
                }
            };

            ctx.Users.AddRange(users);
            await ctx.SaveChangesAsync();

            var hotels = new List<Hotel>
            {
                new Hotel
                {
                    Name = "Grand Palace Hotel",
                    City = "Kyiv",
                    Address = "Khreshchatyk St, 1",
                    Description = "Luxury hotel in the heart of Kyiv"
                },
                new Hotel
                {
                    Name = "Seaside Resort",
                    City = "Odesa",
                    Address = "Primorsky Blvd, 15",
                    Description = "Beautiful resort by the Black Sea"
                },
                new Hotel
                {
                    Name = "Mountain View Inn",
                    City = "Lviv",
                    Address = "Rynok Square, 8",
                    Description = "Cozy hotel in historic Lviv"
                },
                new Hotel
                {
                    Name = "Port hotel",
                    City = "Mykolaiv",
                    Address = "Shevchenka, 22",
                    Description = "Hotel near the city port"
                }
            };

            ctx.Hotels.AddRange(hotels);
            await ctx.SaveChangesAsync();

            var rooms = new List<Room>
            {
                new Room
                {
                    HotelId = hotels[0].Id,
                    Name = "Deluxe Suite",
                    Capacity = 2,
                    PricePerNight = 250.00m,
                    IsActive = true
                },
                new Room
                {
                    HotelId = hotels[0].Id,
                    Name = "Standard Room",
                    Capacity = 4,
                    PricePerNight = 150.00m,
                    IsActive = true
                },
                new Room
                {
                    HotelId = hotels[1].Id,
                    Name = "Sea View Room",
                    Capacity = 2,
                    PricePerNight = 180.00m,
                    IsActive = true
                },
                new Room
                {
                    HotelId = hotels[1].Id,
                    Name = "Garden Room",
                    Capacity = 3,
                    PricePerNight = 120.00m,
                    IsActive = true
                },
                new Room
                {
                    HotelId = hotels[2].Id,
                    Name = "Historic Room",
                    Capacity = 2,
                    PricePerNight = 100.00m,
                    IsActive = true
                },
                new Room
                {
                    HotelId = hotels[2].Id,
                    Name = "Historic Room #2",
                    Capacity = 4,
                    PricePerNight = 120.00m,
                    IsActive = true
                },
                new Room
                {
                    HotelId = hotels[3].Id,
                    Name = "Port View Room",
                    Capacity = 2,
                    PricePerNight = 130.00m,
                    IsActive = true
                },
                new Room
                {
                    HotelId = hotels[3].Id,
                    Name = "Harbor Deluxe",
                    Capacity = 3,
                    PricePerNight = 180.00m,
                    IsActive = true
                }
            };

            ctx.Rooms.AddRange(rooms);
            await ctx.SaveChangesAsync();

            var bookings = new List<Booking>
            {

                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[0].Id,
                    CheckIn = new DateOnly(2025, 8, 1),
                    CheckOut = new DateOnly(2025, 8, 4),
                    TotalPrice = 750.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[1].Id,
                    CheckIn = new DateOnly(2025, 8, 7),
                    CheckOut = new DateOnly(2025, 8, 10),
                    TotalPrice = 450.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[0].Id,
                    CheckIn = new DateOnly(2025, 8, 15),
                    CheckOut = new DateOnly(2025, 8, 17),
                    TotalPrice = 500.00m,
                    Status = BookingStatus.Confirmed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[1].Id,
                    CheckIn = new DateOnly(2025, 8, 22),
                    CheckOut = new DateOnly(2025, 8, 25),
                    TotalPrice = 450.00m,
                    Status = BookingStatus.Pending,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[0].Id,
                    CheckIn = new DateOnly(2025, 9, 2),
                    CheckOut = new DateOnly(2025, 9, 6),
                    TotalPrice = 1000.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[0].Id,
                    RoomId = rooms[1].Id,
                    CheckIn = new DateOnly(2025, 9, 10),
                    CheckOut = new DateOnly(2025, 9, 12),
                    TotalPrice = 300.00m,
                    Status = BookingStatus.Cancelled,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },

                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[2].Id,
                    CheckIn = new DateOnly(2025, 8, 5),
                    CheckOut = new DateOnly(2025, 8, 9),
                    TotalPrice = 720.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[3].Id,
                    CheckIn = new DateOnly(2025, 8, 12),
                    CheckOut = new DateOnly(2025, 8, 15),
                    TotalPrice = 360.00m,
                    Status = BookingStatus.Confirmed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[2].Id,
                    CheckIn = new DateOnly(2025, 8, 20),
                    CheckOut = new DateOnly(2025, 8, 24),
                    TotalPrice = 720.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[3].Id,
                    CheckIn = new DateOnly(2025, 9, 3),
                    CheckOut = new DateOnly(2025, 9, 6),
                    TotalPrice = 360.00m,
                    Status = BookingStatus.Confirmed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[2].Id,
                    CheckIn = new DateOnly(2025, 9, 14),
                    CheckOut = new DateOnly(2025, 9, 18),
                    TotalPrice = 720.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[3].Id,
                    CheckIn = new DateOnly(2025, 9, 20),
                    CheckOut = new DateOnly(2025, 9, 22),
                    TotalPrice = 240.00m,
                    Status = BookingStatus.Pending,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },

                new Booking
                {
                    UserId = users[0].Id,
                    RoomId = rooms[4].Id,
                    CheckIn = new DateOnly(2025, 8, 3),
                    CheckOut = new DateOnly(2025, 8, 7),
                    TotalPrice = 400.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[5].Id,
                    CheckIn = new DateOnly(2025, 8, 12),
                    CheckOut = new DateOnly(2025, 8, 14),
                    TotalPrice = 240.00m,
                    Status = BookingStatus.Confirmed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[4].Id,
                    CheckIn = new DateOnly(2025, 8, 18),
                    CheckOut = new DateOnly(2025, 8, 21),
                    TotalPrice = 300.00m,
                    Status = BookingStatus.Pending,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[5].Id,
                    CheckIn = new DateOnly(2025, 9, 1),
                    CheckOut = new DateOnly(2025, 9, 5),
                    TotalPrice = 480.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[4].Id,
                    CheckIn = new DateOnly(2025, 9, 8),
                    CheckOut = new DateOnly(2025, 9, 11),
                    TotalPrice = 300.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[5].Id,
                    CheckIn = new DateOnly(2025, 9, 20),
                    CheckOut = new DateOnly(2025, 9, 23),
                    TotalPrice = 360.00m,
                    Status = BookingStatus.Confirmed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },

                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[6].Id,
                    CheckIn = new DateOnly(2025, 8, 6),
                    CheckOut = new DateOnly(2025, 8, 10),
                    TotalPrice = 520.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[7].Id,
                    CheckIn = new DateOnly(2025, 8, 14),
                    CheckOut = new DateOnly(2025, 8, 16),
                    TotalPrice = 360.00m,
                    Status = BookingStatus.Confirmed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[6].Id,
                    CheckIn = new DateOnly(2025, 8, 21),
                    CheckOut = new DateOnly(2025, 8, 24),
                    TotalPrice = 390.00m,
                    Status = BookingStatus.Pending,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[7].Id,
                    CheckIn = new DateOnly(2025, 9, 3),
                    CheckOut = new DateOnly(2025, 9, 5),
                    TotalPrice = 360.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[0].Id,
                    RoomId = rooms[6].Id,
                    CheckIn = new DateOnly(2025, 9, 10),
                    CheckOut = new DateOnly(2025, 9, 14),
                    TotalPrice = 520.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[7].Id,
                    CheckIn = new DateOnly(2025, 9, 22),
                    CheckOut = new DateOnly(2025, 9, 25),
                    TotalPrice = 540.00m,
                    Status = BookingStatus.Confirmed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[0].Id,
                    CheckIn = new DateOnly(2025, 10, 1),
                    CheckOut = new DateOnly(2025, 10, 4),
                    TotalPrice = 750.00m,
                    Status = BookingStatus.Confirmed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[2].Id,
                    CheckIn = new DateOnly(2025, 10, 1),
                    CheckOut = new DateOnly(2025, 10, 4),
                    TotalPrice = 540.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[5].Id,
                    CheckIn = new DateOnly(2025, 10, 1),
                    CheckOut = new DateOnly(2025, 10, 4),
                    TotalPrice = 360.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[7].Id,
                    CheckIn = new DateOnly(2025, 10, 1),
                    CheckOut = new DateOnly(2025, 10, 4),
                    TotalPrice = 540.00m,
                    Status = BookingStatus.Confirmed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[3].Id,
                    CheckIn = new DateOnly(2025, 10, 2),
                    CheckOut = new DateOnly(2025, 10, 4),
                    TotalPrice = 240.00m,
                    Status = BookingStatus.Pending,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },

                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[1].Id,
                    CheckIn = new DateOnly(2025, 10, 6),
                    CheckOut = new DateOnly(2025, 10, 8),
                    TotalPrice = 300.00m,
                    Status = BookingStatus.Confirmed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[6].Id,
                    CheckIn = new DateOnly(2025, 10, 6),
                    CheckOut = new DateOnly(2025, 10, 8),
                    TotalPrice = 260.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[2].Id,
                    RoomId = rooms[4].Id,
                    CheckIn = new DateOnly(2025, 10, 6),
                    CheckOut = new DateOnly(2025, 10, 8),
                    TotalPrice = 200.00m,
                    Status = BookingStatus.Completed,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                },
                new Booking
                {
                    UserId = users[1].Id,
                    RoomId = rooms[2].Id,
                    CheckIn = new DateOnly(2025, 10, 6),
                    CheckOut = new DateOnly(2025, 10, 8),
                    TotalPrice = 360.00m,
                    Status = BookingStatus.Pending,
                    IdempotencyKey = Guid.NewGuid().ToString(),
                    CreatedAtUtc = DateTime.UtcNow
                }
            };

            ctx.Bookings.AddRange(bookings);
            await ctx.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Seed error: {ex.Message}");
            return;
        }
    }
}