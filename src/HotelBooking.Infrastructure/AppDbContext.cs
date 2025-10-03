using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HotelBooking.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        var dateOnly = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        b.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
        });

        b.Entity<Hotel>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.City);
        });

        b.Entity<Room>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(x => x.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.HotelId, x.IsActive });
        });

        b.Entity<Booking>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.IdempotencyKey).IsUnique();
            e.HasIndex(x => new { x.RoomId, x.CheckIn, x.CheckOut });

            e.Property(x => x.CheckIn).HasConversion(dateOnly);
            e.Property(x => x.CheckOut).HasConversion(dateOnly);

            e.HasOne(x => x.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
