using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cinema> Cinemas { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Seat> Seats { get; set; } = null!;
        public DbSet<SeatType> SeatTypes { get; set; } = null!;
        public DbSet<Showtime> Showtime { get; set; } = null!;
        public DbSet<ShowtimeSeat> ShowtimeSeats { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
    }
}
