using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task CreateTicketsAsync(IEnumerable<Ticket> tickets)
        {
            _context.Tickets.AddRange(tickets);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Ticket>? ticket, int totalCount, int filterCount)> GetTickets(string? searchTerm, int pageNumber, int pageSize, string sortBy, bool isDescending)
        {
            var query = _context.Tickets
                                .Include(t => t.ShowtimeSeat)
                                .ThenInclude(ss => ss.Showtime)
                                .ThenInclude(s => s.Movie)
                                .Where(t => !t.IsDeleted);
            
            int totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(t => t.TicketCode.Contains(searchTerm) || 
                                        t.ShowtimeSeat.Showtime.Movie.Title.Contains(searchTerm));
            }

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));

            var tickets = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            
            int filterCount = await query.CountAsync();

            foreach (var ticket in tickets)
            {
                ticket.User = await _userManager.FindByIdAsync(ticket.CreatedBy.ToString());
            }

            return (tickets, totalCount, filterCount);
        }

        public async Task<(IEnumerable<Ticket>? tickets, int totalCount)> GetTicketsByUser(Guid userId, int maxRecord, bool? isUpcoming)
        {
            var query = _context.Tickets
                            .Include(t => t.ShowtimeSeat)
                            .ThenInclude(ss => ss.Showtime)
                                .ThenInclude(st => st.Movie)
                            .Include(t => t.ShowtimeSeat)
                            .ThenInclude(ss => ss.Showtime)
                                .ThenInclude(st => st.Room)
                                    .ThenInclude(r => r.Cinema)
                            .Include(t => t.ShowtimeSeat)
                            .ThenInclude(ss => ss.Seat)
                            .Where(t => t.CreatedBy == userId && !t.IsDeleted)
                            .AsQueryable();
            var totalCount = await query.CountAsync();

            if (isUpcoming.HasValue)
            {
                if (isUpcoming.Value)
                {
                    query = query.Where(t => t.ShowtimeSeat.Showtime.StartTime > DateTime.Now);  // Upcoming tickets
                }
                else
                {
                    query = query.Where(t => t.ShowtimeSeat.Showtime.StartTime <= DateTime.Now);  // Past tickets
                }
            }

            var tickets = await query
                                .OrderByDescending(t => t.PurchaseDate)
                                .Take(maxRecord)
                                .ToListAsync();

            return (tickets, totalCount);
        }
    }
}